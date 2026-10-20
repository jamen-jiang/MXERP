package com.mxerp.production.domain.bom;

import com.mxerp.shared.domain.entity.BaseDeleteEntity;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.EnumUtils;
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
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * BOM 行（用料明细）
 *
 * <p>一行只描述「生产 1 个父件需要多少子件」。真正的需求量在工单下达时按父件数量与损耗率算出来，
 * 存到 {@code WorkOrderMaterial}，不在这里保存任何订单相关数据。
 *
 * <p>{@code materialId} 是子件所属的 SPU，由调用方随子件一起提交，只当「按系列查哪些 BOM 用到了它」的分组列用。
 * 物料类型不在这里存：它记在 {@code Variant} 主数据上；「子件是不是自制件」也不靠类型标签判定，而是看这个子件
 * 自己在生产侧有没有 BOM，那是本上下文里能直接问出的事实。
 *
 * <p>倒冲（{@code issueTrigger}）是「不用人开领料单，产出多少就自动领多少」：这一列同时说清要不要自动、
 * 挂在工序报工还是完工入库上，取值手工领料 / 报工倒冲 / 入库倒冲。它和 {@code issueMode} 是两根轴——发料方式
 * 说的是「用量怎么乘」（按件还是整批），触发说的是「谁来发起」，所以两列各存各的。按批的料不允许倒冲：
 * 一缸染料到底这次用没用完，只有配缸的人知道，按产出反推只会算错。
 *
 * <p>{@code warehouseId} 是这一行子件的发料仓（可空）。仓挂在行上而不是表头，因为同一张 BOM 的主料可能在原料仓、
 * 包装袋在辅料仓，挂表头只能靠事后改领料单补。工单下达时原样抄到用料行上，BOM 之后改版不影响在制工单。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "BomLine",
    indexes = {
      @Index(name = "IX_BomLine_BomId", columnList = "BomId"),
      @Index(name = "IX_BomLine_MaterialId", columnList = "MaterialId"),
      @Index(name = "IX_BomLine_VariantId", columnList = "VariantId")
      // 注意：BomId + VariantId 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class BomLine extends BaseDeleteEntity {

  /** BOM Id */
  @Column(name = "BomId", nullable = false)
  private Long bomId;

  /** 行号 */
  @Column(name = "Sequence", nullable = false)
  private Integer sequence;

  /** 子件SPU（按系列查哪些 BOM 用到了它） */
  @Column(name = "MaterialId", nullable = false)
  private Long materialId;

  /** 子件SKU */
  @Column(name = "VariantId", nullable = false)
  private Long variantId;

  /** 单位用量（生产 1 个父件消耗的子件数量） */
  @Column(name = "Quantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal quantity;

  /** 损耗率（百分比） */
  @Column(name = "LossRate", nullable = false, precision = 9, scale = 2)
  private BigDecimal lossRate;

  /** 单位Id */
  @Column(name = "UnitId", nullable = false)
  private Long unitId;

  /** 发料方式 */
  @Nationalized
  @Column(name = "IssueMode", nullable = false, length = 50)
  private String issueMode;

  /** 发料触发：手工领料、报工倒冲、入库倒冲 */
  @Nationalized
  @Column(name = "IssueTrigger", nullable = false, length = 50)
  private String issueTrigger;

  /** 发料仓库Id（可空，空表示下达时回落到工单的默认仓） */
  @Column(name = "WarehouseId")
  @Nullable
  private Long warehouseId;

  /** 是否关键件（缺料即停线） */
  @Column(name = "IsKeyPart", nullable = false)
  private Boolean isKeyPart;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param sequence 行号
   * @param materialId 子件SPU
   * @param variantId 子件SKU
   * @param quantity 单位用量
   * @param lossRate 损耗率（百分比）
   * @param unitId 单位Id
   * @param issueMode 发料方式编码
   * @param issueTrigger 发料触发编码
   * @param warehouseId 发料仓库Id（可空）
   * @param isKeyPart 是否关键件
   * @param remark 备注（可空）
   */
  BomLine(
      Integer sequence,
      Long materialId,
      Long variantId,
      BigDecimal quantity,
      BigDecimal lossRate,
      Long unitId,
      String issueMode,
      String issueTrigger,
      @Nullable Long warehouseId,
      boolean isKeyPart,
      @Nullable String remark) {
    apply(
        sequence,
        materialId,
        variantId,
        quantity,
        lossRate,
        unitId,
        issueMode,
        issueTrigger,
        warehouseId,
        isKeyPart,
        remark);
  }

  /**
   * 修改 BOM 行
   *
   * @param sequence 行号
   * @param materialId 子件SPU
   * @param variantId 子件SKU
   * @param quantity 单位用量
   * @param lossRate 损耗率（百分比）
   * @param unitId 单位Id
   * @param issueMode 发料方式编码
   * @param issueTrigger 发料触发编码
   * @param warehouseId 发料仓库Id（可空）
   * @param isKeyPart 是否关键件
   * @param remark 备注（可空）
   */
  void modify(
      Integer sequence,
      Long materialId,
      Long variantId,
      BigDecimal quantity,
      BigDecimal lossRate,
      Long unitId,
      String issueMode,
      String issueTrigger,
      @Nullable Long warehouseId,
      boolean isKeyPart,
      @Nullable String remark) {
    apply(
        sequence,
        materialId,
        variantId,
        quantity,
        lossRate,
        unitId,
        issueMode,
        issueTrigger,
        warehouseId,
        isKeyPart,
        remark);
  }

  /**
   * 按父件数量与发料方式计算需求数量，保留 2 位小数
   *
   * <p>两种发料方式算法不同，直接把「按件」公式套到按批上，会把一缸染料乘上工单数量：
   *
   * <ul>
   *   <li>按件：单位用量 × 父件数量 × (1 + 损耗率 / 100)
   *   <li>按批：单位用量就是整批的固定领用量，与父件数量无关，只叠加损耗
   * </ul>
   *
   * @param parentQuantity 父件生产数量
   * @return 需求数量
   * @throws DomainException 父件数量为负或发料方式非法时抛出
   */
  public BigDecimal requiredQuantity(BigDecimal parentQuantity) {
    if (parentQuantity.signum() < 0) {
      throw new DomainException("父件数量不能为负数");
    }
    BigDecimal factor = lossFactor();
    IssueModeEnum mode = EnumUtils.fromCode(IssueModeEnum.class, this.issueMode);
    if (mode == null) {
      throw new DomainException("发料方式" + this.issueMode + "有误!");
    }
    return switch (mode) {
      case PIECE ->
          this.quantity.multiply(parentQuantity).multiply(factor).setScale(2, RoundingMode.HALF_UP);
      case BATCH -> this.quantity.multiply(factor).setScale(2, RoundingMode.HALF_UP);
    };
  }

  /**
   * 生产 1 个父件的净耗量：单位用量 × (1 + 损耗率 / 100)
   *
   * <p>倒冲是按「本次产出 × 这一条量」扣料的，所以工单下达时要把这个派生值快照到用料行上：BOM 之后改版、
   * 损耗率调过，都不该影响已在制的工单。只有按件行会走到这一步——按批行不允许倒冲。
   *
   * @return 含损耗的单位用量，保留 2 位小数
   */
  public BigDecimal netUnitQuantity() {
    return this.quantity.multiply(lossFactor()).setScale(2, RoundingMode.HALF_UP);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 损耗系数：1 + 损耗率 / 100
   *
   * @return 损耗系数
   */
  private BigDecimal lossFactor() {
    BigDecimal rate = this.lossRate.divide(BigDecimal.valueOf(100), 6, RoundingMode.HALF_UP);
    return BigDecimal.ONE.add(rate);
  }

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * @param sequence 行号
   * @param materialId 子件SPU
   * @param variantId 子件SKU
   * @param quantity 单位用量
   * @param lossRate 损耗率（百分比）
   * @param unitId 单位Id
   * @param issueMode 发料方式编码
   * @param issueTrigger 发料触发编码
   * @param warehouseId 发料仓库Id（可空）
   * @param isKeyPart 是否关键件
   * @param remark 备注（可空）
   * @throws DomainException 行号、SPU、子件、用量、单位、发料方式、发料触发非法时抛出
   */
  private void apply(
      Integer sequence,
      Long materialId,
      Long variantId,
      BigDecimal quantity,
      BigDecimal lossRate,
      Long unitId,
      String issueMode,
      String issueTrigger,
      @Nullable Long warehouseId,
      boolean isKeyPart,
      @Nullable String remark) {
    if (sequence <= 0) {
      throw new DomainException("BOM 行号必须大于0");
    }
    if (materialId <= 0) {
      throw new DomainException("子件SPU不能为空");
    }
    if (variantId <= 0) {
      throw new DomainException("子件不能为空");
    }
    if (quantity.signum() <= 0) {
      throw new DomainException("单位用量必须大于0");
    }
    if (unitId <= 0) {
      throw new DomainException("单位不能为空");
    }
    validateNonNegative("损耗率", lossRate);
    String trimmedIssueMode = StringUtils.trimToNull(issueMode);
    if (trimmedIssueMode == null) {
      throw new DomainException("发料方式不能为空");
    }
    IssueModeEnum mode = EnumUtils.fromCode(IssueModeEnum.class, trimmedIssueMode);
    if (mode == null) {
      throw new DomainException("发料方式" + trimmedIssueMode + "有误!");
    }
    IssueTriggerEnum trigger = resolveIssueTrigger(mode, issueTrigger);

    this.sequence = sequence;
    this.materialId = materialId;
    this.variantId = variantId;
    this.quantity = quantity;
    this.lossRate = lossRate;
    this.unitId = unitId;
    this.issueMode = trimmedIssueMode;
    this.issueTrigger = trigger.getCode();
    this.warehouseId = warehouseId;
    this.isKeyPart = isKeyPart;
    this.remark = remark;
  }

  /**
   * 校验发料触发
   *
   * <p>按批行不允许倒冲：整批领用跟「按产出反推」是相反的算法，一缸染料这次用没用完只有配缸的人知道，
   * 让系统乘一遍只会算错或重复扣，所以在这个入口就拒绝，不留到工单那边。
   *
   * @param mode 发料方式
   * @param issueTrigger 发料触发编码
   * @return 校验通过的触发枚举
   * @throws DomainException 触发为空、非法，或按批行选了倒冲时抛出
   */
  private IssueTriggerEnum resolveIssueTrigger(IssueModeEnum mode, String issueTrigger) {
    String trimmed = StringUtils.trimToNull(issueTrigger);
    if (trimmed == null) {
      throw new DomainException("发料触发不能为空");
    }
    IssueTriggerEnum trigger = EnumUtils.fromCode(IssueTriggerEnum.class, trimmed);
    if (trigger == null) {
      throw new DomainException("发料触发" + trimmed + "有误!");
    }
    if (mode == IssueModeEnum.BATCH && trigger != IssueTriggerEnum.MANUAL) {
      throw new DomainException("按批发料的子件不能倒冲，请改为按件或改回手工领料");
    }
    return trigger;
  }

  /**
   * 校验数值字段非负
   *
   * <p>参数类型在 {@code @NullMarked} 下即非空契约，调用方必须保证不为 {@code null}，因此这里只判负数。
   *
   * @param field 字段中文名，用于拼装错误提示
   * @param value 待校验的数值
   * @throws DomainException 值为负数时抛出
   */
  private void validateNonNegative(String field, BigDecimal value) {
    if (value.signum() < 0) {
      throw new DomainException(field + "不能为负数");
    }
  }
}
