package com.mxerp.production.domain.productionissue;

import com.mxerp.production.domain.bom.IssueTriggerEnum;
import com.mxerp.production.domain.workorder.WorkOrder;
import com.mxerp.production.domain.workorder.WorkOrderMaterial;
import com.mxerp.production.domain.workorder.WorkOrderRepository;
import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 倒冲领料服务：按产出量替车间开一张生产领料单并照批
 *
 * <p>倒冲不是「悄悄把库存扣掉」，而是系统替人把单据补齐。各家都这么建模：金蝶在入库单审核时生成「倒冲生产
 * 生产领料单」，用友生成「倒冲材料出库单」（可再配自动审核），SAP 与 D365 走报工日记帐。留下一张单据，才有地方
 * 回答「这几片毛坯是哪一次报工吃掉的、超了没有、谁批的」；直接动库存就把这些全抹平了。
 *
 * <p>触发由调用方给：按件耗料挂在工序报工上（产出一次领一次），只在整批结束才说得清的挂在完工入库上。
 * 领域服务不自己挑时机——报工提交与入库审核各自只知道自己那一步做完没做完，跨聚合的这一跳仍归应用层编排，
 * 与「库存动账归应用层」是同一个道理。
 *
 * <p>核定数量等于申请数量：倒冲没有人现场点数，所以不给短装的机会；真不够（超领、库存不足）就让工单与库存
 * 那两道上限把它抛出来，由人去改用量或补料，而不是系统悄悄放行。
 */
@NullMarked
public class BackflushIssueDomainService {

  /** 生产领料单工厂 */
  private final ProductionIssueFactory productionIssueFactory;

  /** 工单仓储 */
  private final WorkOrderRepository workOrderRepository;

  /**
   * 构造器注入
   *
   * @param productionIssueFactory 生产领料单工厂
   * @param workOrderRepository 工单仓储
   */
  public BackflushIssueDomainService(
      ProductionIssueFactory productionIssueFactory, WorkOrderRepository workOrderRepository) {
    this.productionIssueFactory = productionIssueFactory;
    this.workOrderRepository = workOrderRepository;
  }

  /**
   * 生成并过账倒冲生产领料单
   *
   * <p>只处理这张工单上标了「在这个触发上倒冲」的用料行；一行都不该倒冲、或本次产出为零时返回 {@code null}，
   * 不为它开一张空单。发料仓库只认用料行上快照的那个（下达时从 BOM 行的发料仓抄来）——没有第二个来源可退，
   * 行上没仓就报错，正解是回 BOM 上给这个子件填发料仓。
   *
   * @param workOrderId 工单Id
   * @param trigger 发料触发（报工倒冲或入库倒冲）
   * @param outputQuantity 本次产出数量（报工口径填良品加报废，入库口径填实收数量）
   * @return 已发料的倒冲生产领料单；本次没有可倒冲的料时为 {@code null}
   * @throws DomainException 工单不存在、用料行没带发料仓库、或倒冲后净领料超过需求量时抛出
   */
  public @Nullable ProductionIssue generate(
      Long workOrderId, IssueTriggerEnum trigger, BigDecimal outputQuantity) {
    if (outputQuantity.signum() <= 0) {
      return null;
    }
    WorkOrder workOrder = requireWorkOrder(workOrderId);
    List<BackflushLine> lines = new ArrayList<>();
    for (WorkOrderMaterial material : workOrder.getWorkOrderMaterials()) {
      if (!material.isBackflushAt(trigger)) {
        continue;
      }
      BigDecimal quantity = material.backflushQuantity(outputQuantity);
      if (quantity.signum() > 0) {
        lines.add(new BackflushLine(material, quantity));
      }
    }
    if (lines.isEmpty()) {
      return null;
    }

    // 传空串：单号交给 OrderAddedEvent 的监听器分配
    ProductionIssue model =
        productionIssueFactory.create(
            workOrder.getId(),
            workOrder.getNumber(),
            null,
            trigger.getLabel() + "，产出" + outputQuantity,
            "");
    for (BackflushLine line : lines) {
      WorkOrderMaterial material = line.material();
      model.addLine(
          material.getId(),
          material.getMaterialId(),
          material.getVariantId(),
          line.quantity(),
          material.getUnitId(),
          resolveWarehouse(workOrder, material),
          null,
          null);
      // 加一行的同时就把量累加工单已领（倒冲行发即耗，已耗跟着加）：超领在这一步抛出时单据还停在草稿，
      // 不会留下「已发料却没计入工单」的半张单
      workOrder.issueMaterial(material.getId(), line.quantity());
    }
    model.post(LocalDateTime.now());
    return model;
  }

  /** 一条倒冲结果：工单用料行与本次该领的量 */
  private record BackflushLine(WorkOrderMaterial material, BigDecimal quantity) {}

  // ==================== 私有 ====================

  /**
   * 取工单，不存在则抛异常
   *
   * @param workOrderId 工单Id
   * @return 工单聚合
   * @throws DomainException 工单不存在时抛出
   */
  private WorkOrder requireWorkOrder(Long workOrderId) {
    WorkOrder workOrder = workOrderRepository.get(workOrderId);
    if (workOrder == null) {
      throw new DomainException("工单" + workOrderId + "不存在!");
    }
    return workOrder;
  }

  /**
   * 定发料仓库：只认用料行上快照的那个仓
   *
   * <p>倒冲没人现场选仓，也没有第二个来源可退——行上没仓就报错。生产领料单行没有仓库就定位不到库存行，随便凑一个
   * 反而是凭空造数：报工那条链没人知道料在哪，入库那条链只知道收成品的仓，料根本不在那儿。正解是回 BOM 行上
   * 给这个子件填发料仓，所以报错文案直接点出来。
   *
   * @param workOrder 工单聚合（只用于拼装错误提示）
   * @param material 工单用料行
   * @return 发料仓库Id
   * @throws DomainException 用料行没有发料仓库时抛出
   */
  private Long resolveWarehouse(WorkOrder workOrder, WorkOrderMaterial material) {
    Long warehouseId = material.getWarehouseId();
    if (warehouseId == null) {
      throw new DomainException(
          "工单"
              + workOrder.getNumber()
              + "的用料"
              + material.getVariantId()
              + "没有发料仓库，不能倒冲，请在 BOM 行上补发料仓");
    }
    return warehouseId;
  }
}
