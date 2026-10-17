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
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "BomItem",
    indexes = {
      @Index(name = "IX_BomItem_BomId", columnList = "BomId"),
      @Index(name = "IX_BomItem_ComponentSkuId", columnList = "ComponentSkuId")
      // 注意：BomId + ComponentSkuId 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class BomItem extends BaseDeleteEntity {

  /** BOM Id */
  @Column(name = "BomId")
  private Long bomId;

  /** 行号 */
  @Column(name = "Sequence", nullable = false)
  private Integer sequence;

  /** 子件SKU */
  @Column(name = "ComponentSkuId", nullable = false)
  private Long componentSkuId;

  /** 单位用量（生产 1 个父件消耗的子件数量） */
  @Column(name = "Quantity", nullable = false, precision = 18, scale = 4)
  private BigDecimal quantity;

  /** 损耗率（百分比） */
  @Column(name = "LossRate", nullable = false, precision = 9, scale = 4)
  private BigDecimal lossRate;

  /** 单位Id */
  @Column(name = "UnitId", nullable = false)
  private Long unitId;

  /** 发料方式 */
  @Nationalized
  @Column(name = "IssueMode", nullable = false, length = 50)
  private String issueMode;

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
   * @param componentSkuId 子件SKU
   * @param quantity 单位用量
   * @param lossRate 损耗率（百分比）
   * @param unitId 单位Id
   * @param issueMode 发料方式编码
   * @param isKeyPart 是否关键件
   * @param remark 备注（可空）
   */
  BomItem(
      Integer sequence,
      Long componentSkuId,
      BigDecimal quantity,
      BigDecimal lossRate,
      Long unitId,
      String issueMode,
      boolean isKeyPart,
      @Nullable String remark) {
    apply(sequence, componentSkuId, quantity, lossRate, unitId, issueMode, isKeyPart, remark);
  }

  /**
   * 修改 BOM 行
   *
   * @param sequence 行号
   * @param componentSkuId 子件SKU
   * @param quantity 单位用量
   * @param lossRate 损耗率（百分比）
   * @param unitId 单位Id
   * @param issueMode 发料方式编码
   * @param isKeyPart 是否关键件
   * @param remark 备注（可空）
   */
  void modify(
      Integer sequence,
      Long componentSkuId,
      BigDecimal quantity,
      BigDecimal lossRate,
      Long unitId,
      String issueMode,
      boolean isKeyPart,
      @Nullable String remark) {
    apply(sequence, componentSkuId, quantity, lossRate, unitId, issueMode, isKeyPart, remark);
  }

  /**
   * 按父件数量计算需求数量
   *
   * <p>需求量 = 单位用量 × 父件数量 × (1 + 损耗率 / 100)，保留 4 位小数。
   *
   * @param parentQuantity 父件生产数量
   * @return 需求数量
   */
  public BigDecimal requiredQuantity(BigDecimal parentQuantity) {
    if (parentQuantity.signum() < 0) {
      throw new DomainException("父件数量不能为负数");
    }
    BigDecimal rate = this.lossRate.divide(BigDecimal.valueOf(100), 6, RoundingMode.HALF_UP);
    BigDecimal factor = BigDecimal.ONE.add(rate);
    return this.quantity
        .multiply(parentQuantity)
        .multiply(factor)
        .setScale(4, RoundingMode.HALF_UP);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * @param sequence 行号
   * @param componentSkuId 子件SKU
   * @param quantity 单位用量
   * @param lossRate 损耗率（百分比）
   * @param unitId 单位Id
   * @param issueMode 发料方式编码
   * @param isKeyPart 是否关键件
   * @param remark 备注（可空）
   * @throws DomainException 行号、子件、用量、单位、发料方式非法时抛出
   */
  private void apply(
      Integer sequence,
      Long componentSkuId,
      BigDecimal quantity,
      BigDecimal lossRate,
      Long unitId,
      String issueMode,
      boolean isKeyPart,
      @Nullable String remark) {
    if (sequence <= 0) {
      throw new DomainException("BOM 行号必须大于0");
    }
    if (componentSkuId <= 0) {
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
    if (EnumUtils.fromCode(IssueModeEnum.class, trimmedIssueMode) == null) {
      throw new DomainException("发料方式" + trimmedIssueMode + "有误!");
    }

    this.sequence = sequence;
    this.componentSkuId = componentSkuId;
    this.quantity = quantity;
    this.lossRate = lossRate;
    this.unitId = unitId;
    this.issueMode = trimmedIssueMode;
    this.isKeyPart = isKeyPart;
    this.remark = remark;
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
