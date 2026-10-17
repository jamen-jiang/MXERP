package com.mxerp.basedata.domain.taxrate;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 税率 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "TaxRate",
    indexes = {
      @Index(name = "IX_TaxRate_Name", columnList = "Name"),
      @Index(name = "IX_TaxRate_Sort", columnList = "Sort")
      // 注意：Code 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class TaxRate extends UpdateAggregateRoot {

  /** 代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 税率值 */
  @Column(name = "Rate", nullable = false, precision = 18, scale = 2)
  private BigDecimal rate;

  /** 排序（可空） */
  @Column(name = "Sort")
  @Nullable
  private Integer sort;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param code 代码
   * @param name 名称
   * @param rate 税率值
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  TaxRate(
      String code, String name, BigDecimal rate, @Nullable Integer sort, @Nullable String remark) {
    apply(code, name, rate, sort, remark);
  }

  /**
   * 修改税率
   *
   * @param code 代码
   * @param name 名称
   * @param rate 税率值
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  void modify(
      String code, String name, BigDecimal rate, @Nullable Integer sort, @Nullable String remark) {
    apply(code, name, rate, sort, remark);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>把「去空格 → 非空校验 → 赋值」收敛到一处，避免构造器和 {@link #modify} 各写一遍导致规则漂移。
   *
   * @param code 代码
   * @param name 名称
   * @param rate 税率值
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  private void apply(
      String code, String name, BigDecimal rate, @Nullable Integer sort, @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("税率代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("税率名称不能为空");
    }
    this.code = trimmedCode;
    this.name = trimmedName;
    this.rate = rate;
    this.sort = sort;
    this.remark = remark;
  }
}
