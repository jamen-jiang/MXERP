package com.mxerp.production.domain.productionrun;

import com.mxerp.shared.domain.entity.BaseDeleteEntity;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import java.math.RoundingMode;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 炉号单行：某张工单的某道工序进了这一炉，占了多少
 *
 * <p>挂载点是 {@code workOrderOperationId}，不是工单。一炉做的是「同一道工序、几张工单」，一张工单可能只有
 * 镀膜这道工序进这炉、加硬那道另开一炉，挂在工单上就表达不了。外键只从这边指过去，工单不知道自己被合批，它原有
 * 的口径（可投入数量上限、报工与倒序红冲）一条都不用改；要反查谁在做这一道，按本列查即可。
 *
 * <p>同一道工单工序至多存在于一个未作废的行里。这条约束由数据库的过滤唯一索引守住
 * （{@code WorkOrderOperationId WHERE IsDeleted = 0}），JPA 表达不了；应用层的重复检查只是让报错更早更友好。
 *
 * <p>行上没有产出数：良品与报废是工单自己报工报出来的，这张单只管「这一炉的账里有我几份」。两边各记一份产出，
 * 迟早对不上账。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "ProductionRunLine",
    indexes = {
      @Index(name = "IX_RunLine_RunId", columnList = "RunId"),
      @Index(name = "IX_RunLine_WorkOrderId", columnList = "WorkOrderId"),
      @Index(name = "IX_RunLine_WorkOrderOperationId", columnList = "WorkOrderOperationId"),
      @Index(name = "IX_RunLine_VariantId", columnList = "VariantId")
      // 注意：WorkOrderOperationId 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class ProductionRunLine extends BaseDeleteEntity {

  /** 炉号单Id */
  @Column(name = "RunId", nullable = false)
  private Long runId;

  /** 工单Id（快照，便于按单折叠与追溯） */
  @Column(name = "WorkOrderId", nullable = false)
  private Long workOrderId;

  /** 工单工序Id：本行对应的物理工序，唯一挂载点 */
  @Column(name = "WorkOrderOperationId", nullable = false)
  private Long workOrderOperationId;

  /** 工单号（快照，单据打印与成本还原直接用，省一次关联） */
  @Nationalized
  @Column(name = "WorkOrderNumber", nullable = false, length = 30)
  private String workOrderNumber;

  /** 产出SPU（快照，按系列分组看这炉做了哪些单） */
  @Column(name = "ProductId", nullable = false)
  private Long productId;

  /** 产出SKU（快照） */
  @Column(name = "VariantId", nullable = false)
  private Long variantId;

  /**
   * 占用量
   *
   * <p>单位跟着表头的分配标准走：按件数就是件，按重量就是公斤，按机器工时就是分钟。它是「这一炉里我占几份」的
   * 原始事实，一旦过账就锁死，事后的成本追溯全靠它可对。
   */
  @Column(name = "OccupyQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal occupyQuantity;

  /**
   * 分摊权重：本行实际占几份
   *
   * <p>建行时默认取占用量，之后可以覆写——膜系难度系数、异形件多占位这类修正落在这里而不是改占用量：占用量说的
   * 是物理事实，权重说的是算账份数，两根轴各管一件事，混成一列就再也问不出「谁改过、为什么改」。
   */
  @Column(name = "AllocationWeight", nullable = false, precision = 18, scale = 4)
  private BigDecimal allocationWeight;

  /** 摊后工时（分钟，可空，过账时按权重算出，落库以便事后核对） */
  @Column(name = "AllocatedMinutes", precision = 18, scale = 2)
  @Nullable
  private BigDecimal allocatedMinutes;

  /** 摊后费用（元，可空） */
  @Column(name = "AllocatedCost", precision = 18, scale = 2)
  @Nullable
  private BigDecimal allocatedCost;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器：新建的行还没有摊后数
   *
   * @param workOrderId 工单Id
   * @param workOrderOperationId 工单工序Id
   * @param workOrderNumber 工单号（快照）
   * @param productId 产出SPU
   * @param variantId 产出SKU
   * @param occupyQuantity 占用量
   * @param allocationWeight 分摊权重（传 {@code null} 表示取占用量）
   * @param remark 备注（可空）
   */
  ProductionRunLine(
      Long workOrderId,
      Long workOrderOperationId,
      String workOrderNumber,
      Long productId,
      Long variantId,
      BigDecimal occupyQuantity,
      @Nullable BigDecimal allocationWeight,
      @Nullable String remark) {
    apply(
        workOrderId,
        workOrderOperationId,
        workOrderNumber,
        productId,
        variantId,
        occupyQuantity,
        allocationWeight,
        remark);
  }

  /**
   * 修改占用量、权重与备注
   *
   * @param occupyQuantity 占用量
   * @param allocationWeight 分摊权重（传 {@code null} 表示重新取占用量）
   * @param remark 备注（可空）
   */
  void modify(
      BigDecimal occupyQuantity, @Nullable BigDecimal allocationWeight, @Nullable String remark) {
    apply(
        this.workOrderId,
        this.workOrderOperationId,
        this.workOrderNumber,
        this.productId,
        this.variantId,
        occupyQuantity,
        allocationWeight,
        remark);
  }

  /**
   * 写入摊回结果
   *
   * @param allocatedMinutes 摊后工时（分钟）
   * @param allocatedCost 摊后费用（元）
   */
  void applyAllocation(BigDecimal allocatedMinutes, BigDecimal allocatedCost) {
    this.allocatedMinutes = allocatedMinutes;
    this.allocatedCost = allocatedCost;
  }

  /** 清掉摊回结果（红冲：这一炉的账不算了，行退回未摊状态） */
  void clearAllocation() {
    this.allocatedMinutes = null;
    this.allocatedCost = null;
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口
   *
   * @param workOrderId 工单Id
   * @param workOrderOperationId 工单工序Id
   * @param workOrderNumber 工单号
   * @param productId 产出SPU
   * @param variantId 产出SKU
   * @param occupyQuantity 占用量
   * @param allocationWeight 分摊权重（可空，取占用量）
   * @param remark 备注（可空）
   * @throws DomainException Id 非法、工单号为空、占用量或权重非正时抛出
   */
  private void apply(
      Long workOrderId,
      Long workOrderOperationId,
      String workOrderNumber,
      Long productId,
      Long variantId,
      BigDecimal occupyQuantity,
      @Nullable BigDecimal allocationWeight,
      @Nullable String remark) {
    if (workOrderId <= 0) {
      throw new DomainException("工单不能为空");
    }
    if (workOrderOperationId <= 0) {
      throw new DomainException("工单工序不能为空");
    }
    if (workOrderNumber == null || workOrderNumber.isBlank()) {
      throw new DomainException("工单号不能为空");
    }
    if (productId <= 0 || variantId <= 0) {
      throw new DomainException("产出SKU不能为空");
    }
    if (occupyQuantity.signum() <= 0) {
      throw new DomainException("占用量必须大于0");
    }
    BigDecimal weight = allocationWeight == null ? occupyQuantity : allocationWeight;
    if (weight.signum() <= 0) {
      throw new DomainException("分摊权重必须大于0");
    }

    this.workOrderId = workOrderId;
    this.workOrderOperationId = workOrderOperationId;
    this.workOrderNumber = workOrderNumber;
    this.productId = productId;
    this.variantId = variantId;
    this.occupyQuantity = occupyQuantity;
    this.allocationWeight = weight.setScale(4, RoundingMode.HALF_UP);
    this.remark = remark;
  }
}
