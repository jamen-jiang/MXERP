package com.mxerp.production.domain.workorder;

import com.mxerp.production.domain.bom.Bom;
import com.mxerp.production.domain.bom.BomLine;
import com.mxerp.production.domain.bom.BomRepository;
import com.mxerp.production.domain.bom.IssueTriggerEnum;
import com.mxerp.production.domain.operation.Operation;
import com.mxerp.production.domain.operation.OperationRepository;
import com.mxerp.production.domain.operationrate.OperationRate;
import com.mxerp.production.domain.operationrate.OperationRateRepository;
import com.mxerp.production.domain.routing.Routing;
import com.mxerp.production.domain.routing.RoutingOperation;
import com.mxerp.production.domain.routing.RoutingRepository;
import com.mxerp.production.domain.workcenter.WorkCenter;
import com.mxerp.production.domain.workcenter.WorkCenterRepository;
import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 工单领域服务
 *
 * <p>负责跨聚合编排：下达工单时把 BOM 展开成用料、把工艺路线快照成工序。这两件事都需要读别的聚合
 * （{@code Bom} / {@code Routing} / {@code Operation} / {@code WorkCenter}），所以放在领域服务而不是
 * {@code WorkOrder} 自己身上——聚合根只能管自己的不变量。
 *
 * <p>下达遵循「先全量校验、再一次性装配」：校验不过就抛异常，工单还停在草稿上，不会留下半套工序或用料。
 */
@NullMarked
public class WorkOrderDomainService {

  /** 工单工厂 */
  private final WorkOrderFactory workOrderFactory;

  /** 工单仓储 */
  private final WorkOrderRepository workOrderRepository;

  /** BOM 仓储 */
  private final BomRepository bomRepository;

  /** 工艺路线仓储 */
  private final RoutingRepository routingRepository;

  /** 工序仓储 */
  private final OperationRepository operationRepository;

  /** 工作中心仓储 */
  private final WorkCenterRepository workCenterRepository;

  /** 工序费率卡仓储 */
  private final OperationRateRepository operationRateRepository;

  /**
   * 构造器注入
   *
   * @param workOrderFactory 工单工厂
   * @param workOrderRepository 工单仓储
   * @param bomRepository BOM 仓储
   * @param routingRepository 工艺路线仓储
   * @param operationRepository 工序仓储
   * @param workCenterRepository 工作中心仓储
   * @param operationRateRepository 工序费率卡仓储
   */
  public WorkOrderDomainService(
      WorkOrderFactory workOrderFactory,
      WorkOrderRepository workOrderRepository,
      BomRepository bomRepository,
      RoutingRepository routingRepository,
      OperationRepository operationRepository,
      WorkCenterRepository workCenterRepository,
      OperationRateRepository operationRateRepository) {
    this.workOrderFactory = workOrderFactory;
    this.workOrderRepository = workOrderRepository;
    this.bomRepository = bomRepository;
    this.routingRepository = routingRepository;
    this.operationRepository = operationRepository;
    this.workCenterRepository = workCenterRepository;
    this.operationRateRepository = operationRateRepository;
  }

  /**
   * 创建工单（草稿）
   *
   * @param materialId 产出SPU
   * @param variantId 产出SKU
   * @param quantity 计划数量
   * @param unitId 单位Id
   * @param sourceType 来源类型编码
   * @param sourceId 来源单据Id（可空，库存补货时为空）
   * @param sourceNumber 来源单号（可空，冗余存储便于追溯与展示）
   * @param bomId 指定BOM（可空，为空时下达自动取生效版本）
   * @param routingId 指定工艺路线（可空，为空时下达自动取生效版本）
   * @param dueTime 交期（可空，承诺的完工时间，生产侧只读不写）
   * @param warehouseId 仓库Id（可空，完工入库单没指定仓库时的默认收货仓）
   * @param remark 备注（可空）
   * @return 工单聚合
   */
  public WorkOrder create(
      Long materialId,
      Long variantId,
      BigDecimal quantity,
      Long unitId,
      String sourceType,
      @Nullable Long sourceId,
      @Nullable String sourceNumber,
      @Nullable Long bomId,
      @Nullable Long routingId,
      @Nullable LocalDateTime dueTime,
      @Nullable Long warehouseId,
      @Nullable String remark) {
    // 传空串：单号交给 OrderAddedEvent 的监听器分配
    return workOrderFactory.create(
        materialId,
        variantId,
        quantity,
        unitId,
        sourceType,
        sourceId,
        sourceNumber,
        bomId,
        routingId,
        dueTime,
        warehouseId,
        remark,
        "");
  }

  /**
   * 修改工单（仅草稿可改）
   *
   * @param id 工单Id
   * @param materialId 产出SPU
   * @param variantId 产出SKU
   * @param quantity 计划数量
   * @param unitId 单位Id
   * @param sourceType 来源类型编码
   * @param sourceId 来源单据Id（可空，库存补货时为空）
   * @param sourceNumber 来源单号（可空，冗余存储便于追溯与展示）
   * @param bomId 指定BOM（可空，为空时下达自动取生效版本）
   * @param routingId 指定工艺路线（可空，为空时下达自动取生效版本）
   * @param dueTime 交期（可空，承诺的完工时间，生产侧只读不写）
   * @param warehouseId 仓库Id（可空，完工入库单没指定仓库时的默认收货仓）
   * @param remark 备注（可空）
   * @return 工单聚合
   * @throws DomainException 工单不存在、非草稿状态时抛出
   */
  public WorkOrder modify(
      long id,
      Long materialId,
      Long variantId,
      BigDecimal quantity,
      Long unitId,
      String sourceType,
      @Nullable Long sourceId,
      @Nullable String sourceNumber,
      @Nullable Long bomId,
      @Nullable Long routingId,
      @Nullable LocalDateTime dueTime,
      @Nullable Long warehouseId,
      @Nullable String remark) {
    WorkOrder model = getExist(id);
    model.modify(
        materialId,
        variantId,
        quantity,
        unitId,
        sourceType,
        sourceId,
        sourceNumber,
        bomId,
        routingId,
        dueTime,
        warehouseId,
        remark);
    return model;
  }

  /**
   * 下达工单：按 BOM 展开用料、按工艺路线快照工序
   *
   * <p>BOM 与路线都遵循「工单上指定优先，没指定才取生效版本」，且都按下达时点取版本：补一张上个月的单，
   * 拿到的是上个月那一版配方与工艺，不是今天最新版。路线挂在物料系列上，工单没指定路线时，取该物料在
   * 下达时点生效的主路线。
   *
   * @param id 工单Id
   * @param routingId 本次下达使用的工艺路线（可空，为空时取工单上指定的，再为空取物料的主路线）
   * @param releaseTime 下达时点：这一时刻落在生效区间的那一版 BOM、路线与费率卡才被快照
   * @return 工单聚合
   * @throws DomainException 工单不存在、物料没有生效的主路线、路线在该时点不生效、引用的工序、工作中心或
   *     费率卡不可用时抛出
   */
  public WorkOrder release(long id, @Nullable Long routingId, LocalDateTime releaseTime) {
    WorkOrder model = getExist(id);

    // 1. 定路线：本次传入优先，其次用工单上指定的，最后取物料在该时点生效的主路线
    Long resolvedRoutingId = routingId == null ? model.getRoutingId() : routingId;
    Routing routing;
    if (resolvedRoutingId == null) {
      routing =
          routingRepository.getEffectivePrimaryByMaterialIdAt(model.getMaterialId(), releaseTime);
      if (routing == null) {
        throw new DomainException(
            "物料" + model.getMaterialId() + "在该时点没有生效的主工艺路线，不能下达");
      }
    } else {
      routing = routingRepository.get(resolvedRoutingId);
      if (routing == null) {
        throw new DomainException("工艺路线" + resolvedRoutingId + "不存在!");
      }
    }
    if (!routing.isEffectiveAt(releaseTime)) {
      throw new DomainException(
          "工艺路线" + routing.getCode() + "版本" + routing.getVersion() + "在该时点不生效，不能下达工单");
    }

    // 2. 定 BOM：工单指定优先，其次取该时点生效的已发布版本
    Long resolvedBomId = model.getBomId();
    Bom bom =
        resolvedBomId == null
            ? bomRepository.getEffectiveByVariantIdAt(model.getVariantId(), releaseTime)
            : bomRepository.get(resolvedBomId);
    if (resolvedBomId != null && bom == null) {
      throw new DomainException("BOM" + resolvedBomId + "不存在!");
    }
    if (bom != null && !bom.isEffectiveAt(releaseTime)) {
      throw new DomainException(
          "BOM 版本" + bom.getVersion() + "在该时点不生效，不能用于下达工单");
    }

    // 3. 先全量校验外部引用，确认能装配再动手
    List<OperationSnapshot> snapshots = resolveOperations(routing, releaseTime);
    List<MaterialRequirement> requirements = resolveMaterials(model, bom);

    // 4. 一次性装配并固化
    for (OperationSnapshot snapshot : snapshots) {
      model.addOperation(
          snapshot.sequence(),
          snapshot.operationId(),
          snapshot.operationName(),
          snapshot.isOptional(),
          snapshot.isOutsourced(),
          snapshot.workCenterId(),
          snapshot.workCenterName(),
          snapshot.setupTime(),
          snapshot.runTime(),
          snapshot.queueTime(),
          snapshot.rateId(),
          snapshot.setupRate(),
          snapshot.runRate(),
          snapshot.quantityRate(),
          snapshot.timeFormula(),
          snapshot.standardBatchSize(),
          snapshot.reworkOperationId(),
          snapshot.remark());
    }
    for (MaterialRequirement requirement : requirements) {
      model.addMaterial(
          requirement.bomLineId(),
          requirement.materialId(),
          requirement.variantId(),
          requirement.requiredQuantity(),
          requirement.unitQuantity(),
          requirement.unitId(),
          requirement.issueMode(),
          requirement.issueTrigger(),
          requirement.isKeyPart(),
          requirement.warehouseId(),
          requirement.remark());
    }
    model.release(
        bom == null ? null : bom.getId(),
        bom == null ? null : bom.getVersion(),
        routing.getId(),
        routing.getVersion());
    return model;
  }

  /**
   * 取消工单
   *
   * @param id 工单Id
   * @return 工单聚合
   * @throws DomainException 工单不存在、或当前状态不允许取消时抛出
   */
  public WorkOrder cancel(long id) {
    WorkOrder model = getExist(id);
    model.cancel();
    return model;
  }

  // ==================== 私有：展开与快照 ====================

  /**
   * 取工单，不存在则抛异常
   *
   * @param id 工单Id
   * @return 工单聚合
   * @throws DomainException 工单不存在时抛出
   */
  private WorkOrder getExist(long id) {
    WorkOrder model = workOrderRepository.get(id);
    if (model == null) {
      throw new DomainException("工单" + id + "不存在!");
    }
    return model;
  }

  /**
   * 解析工艺路线，得到可直接快照成工单工序的行
   *
   * <p>工序名、工作中心名、工时与费率、取时公式、可选与外协标识全部冗余存一份：路线或工序主数据后续改名改配置，
   * 在制工单不受影响。这里先跑完整条路线再返回，任何一行引用失效都会在任何工序进工单之前抛出来。
   *
   * @param routing 工艺路线
   * @param moment 取价时点
   * @return 有序、可装配的工序快照列表
   * @throws DomainException 引用的工序、工作中心或费率卡不可用、或工序顺序重复时抛出
   */
  private List<OperationSnapshot> resolveOperations(Routing routing, LocalDateTime moment) {
    List<RoutingOperation> items =
        routing.getRoutingOperations().stream()
            .filter(o -> !o.getIsDeleted())
            .sorted(Comparator.comparing(RoutingOperation::getSequence))
            .toList();
    Set<Integer> sequences = new HashSet<>();
    List<OperationSnapshot> result = new ArrayList<>(items.size());
    for (RoutingOperation item : items) {
      if (!sequences.add(item.getSequence())) {
        throw new DomainException(
            "工艺路线" + routing.getCode() + "的工序顺序" + item.getSequence() + "重复!");
      }
      Operation operation = operationRepository.get(item.getOperationId());
      if (operation == null) {
        throw new DomainException("工序" + item.getOperationId() + "不存在!");
      }
      WorkCenter workCenter = workCenterRepository.get(item.getWorkCenterId());
      if (workCenter == null) {
        throw new DomainException("工作中心" + item.getWorkCenterId() + "不存在!");
      }
      Price price = resolvePrice(routing, item, moment);
      result.add(
          new OperationSnapshot(
              item.getSequence(),
              operation.getId(),
              operation.getName(),
              Boolean.TRUE.equals(item.getIsOptional()),
              Boolean.TRUE.equals(item.getIsOutsourced()),
              workCenter.getId(),
              workCenter.getName(),
              item.getSetupTime(),
              item.getRunTime(),
              item.getQueueTime(),
              price.rateId(),
              price.setupRate(),
              price.runRate(),
              price.quantityRate(),
              item.getTimeFormula(),
              item.getStandardBatchSize(),
              item.getReworkOperationId(),
              item.getRemark()));
    }
    return List.copyOf(result);
  }

  /**
   * 取路线工序行在下达时点的费率
   *
   * <p>路线行引用的是费率卡，价格要按取价时点落在生效区间的那一张拿——这样调价只用激活新卡，路线一个字节都不改。
   * 没挂费率卡的工序返回全零价（这道工序不计成本）；挂了卡却取不到生效价格就直接拒绝下达，不能悄悄按零成本开工。
   *
   * @param routing 工艺路线（只用于拼装错误提示）
   * @param item 路线工序行
   * @param moment 取价时点
   * @return 可装配的价格快照
   * @throws DomainException 费率卡不存在、或该时点没有生效价格时抛出
   */
  private Price resolvePrice(Routing routing, RoutingOperation item, LocalDateTime moment) {
    Long rateId = item.getRateId();
    if (rateId == null) {
      return new Price(null, BigDecimal.ZERO, BigDecimal.ZERO, BigDecimal.ZERO);
    }
    OperationRate anchor = operationRateRepository.get(rateId);
    if (anchor == null) {
      throw new DomainException("工艺路线" + routing.getCode() + "引用的费率卡" + rateId + "不存在!");
    }
    OperationRate effective =
        operationRateRepository.getEffectiveByCodeAt(anchor.getCode(), moment);
    if (effective == null) {
      throw new DomainException("费率卡" + anchor.getCode() + "在该时点没有生效价格，不能下达工单");
    }
    return new Price(
        effective.getId(),
        effective.getSetupRate(),
        effective.getRunRate(),
        effective.getQuantityRate());
  }

  /**
   * 展开 BOM，得到可直接装配成工单用料的行
   *
   * <p>当前只做单层展开：每一行子件都按「能从仓库领」处理，不判断它是不是自制件。真领不到的时候由库存那边拦——
   * 出库时「该键没有库存行」就是答案。这里不做提前判定，是因为「自制」并不等于「不能从仓里领」：备库的半成品
   * 本来就该能领；反过来外购件仓里没货照样领不到，类型标签也提醒不了什么。
   *
   * <p>物料没有 BOM 时返回空列表——纯加工类工单不消耗物料，允许下达。
   *
   * <p>倒冲设置、净单位用量与发料仓都在这里一次算好、快照到用料行：之后报工或入库开倒冲单只做工单上的乘法，
   * 不再回头读 BOM，BOM 之后改版才不会把在制工单的领料量和发料仓改掉。
   *
   * @param model 工单聚合
   * @param bom 展开所用的 BOM（可空，为空表示物料没有 BOM）
   * @return 可装配的用料需求列表
   */
  private List<MaterialRequirement> resolveMaterials(WorkOrder model, @Nullable Bom bom) {
    if (bom == null) {
      return List.of();
    }
    List<BomLine> lines = bom.getBomLines().stream().filter(i -> !i.getIsDeleted()).toList();
    List<MaterialRequirement> result = new ArrayList<>();
    for (BomLine line : lines) {
      boolean backflush = !IssueTriggerEnum.MANUAL.getCode().equals(line.getIssueTrigger());
      result.add(
          new MaterialRequirement(
              line.getId(),
              line.getMaterialId(),
              line.getVariantId(),
              line.requiredQuantity(model.getQuantity()),
              // 净单位用量只在倒冲行上快照：这一列的约定就是「有值才能按产出乘回去领料」
              backflush ? line.netUnitQuantity() : null,
              line.getUnitId(),
              line.getIssueMode(),
              line.getIssueTrigger(),
              Boolean.TRUE.equals(line.getIsKeyPart()),
              line.getWarehouseId(),
              line.getRemark()));
    }
    return List.copyOf(result);
  }

  /** 待装配的工序快照，字段与 {@link WorkOrder#addOperation} 一一对应 */
  private record OperationSnapshot(
      Integer sequence,
      Long operationId,
      String operationName,
      boolean isOptional,
      boolean isOutsourced,
      Long workCenterId,
      String workCenterName,
      BigDecimal setupTime,
      BigDecimal runTime,
      BigDecimal queueTime,
      @Nullable Long rateId,
      BigDecimal setupRate,
      BigDecimal runRate,
      BigDecimal quantityRate,
      String timeFormula,
      @Nullable BigDecimal standardBatchSize,
      @Nullable Long reworkOperationId,
      @Nullable String remark) {}

  /** 取价结果：实际用到那张费率卡的Id（可空）与三块费率 */
  private record Price(
      @Nullable Long rateId,
      BigDecimal setupRate,
      BigDecimal runRate,
      BigDecimal quantityRate) {}

  /** 待装配的用料需求，字段与 {@link WorkOrder#addMaterial} 一一对应 */
  private record MaterialRequirement(
      @Nullable Long bomLineId,
      Long materialId,
      Long variantId,
      BigDecimal requiredQuantity,
      @Nullable BigDecimal unitQuantity,
      Long unitId,
      String issueMode,
      String issueTrigger,
      boolean isKeyPart,
      @Nullable Long warehouseId,
      @Nullable String remark) {}
}
