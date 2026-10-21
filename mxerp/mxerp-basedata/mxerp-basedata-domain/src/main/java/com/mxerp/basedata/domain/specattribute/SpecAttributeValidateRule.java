package com.mxerp.basedata.domain.specattribute;

import com.mxerp.shared.domain.entity.BaseDeleteEntity;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 规格属性校验规则 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "SpecAttributeValidateRule",
    indexes = {
      @Index(
          name = "IX_SpecAttributeValidateRule_SpecAttributeId",
          columnList = "SpecAttributeId"),
      @Index(name = "IX_SpecAttributeValidateRule_Sort", columnList = "Sort")
    })
public class SpecAttributeValidateRule extends BaseDeleteEntity {

  /** 规格属性Id */
  @Column(name = "SpecAttributeId", nullable = false)
  private Long specAttributeId;

  /** 表达式 */
  @Nationalized
  @Column(name = "Expression", nullable = false, length = 500)
  private String expression;

  /** 错误提示 */
  @Nationalized
  @Column(name = "ErrorMessage", nullable = false, length = 200)
  private String errorMessage;

  /** 排序（可空） */
  @Column(name = "Sort")
  @Nullable
  private Integer sort;

  /**
   * 构造器
   *
   * @param expression 表达式
   * @param errorMessage 错误提示
   * @param sort 排序（可空）
   */
  SpecAttributeValidateRule(
      String expression, String errorMessage, @Nullable Integer sort) {
    apply(expression, errorMessage, sort);
  }

  /**
   * 修改校验规则
   *
   * @param expression 表达式
   * @param errorMessage 错误提示
   * @param sort 排序（可空）
   */
  void modify(String expression, String errorMessage, @Nullable Integer sort) {
    apply(expression, errorMessage, sort);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>把「去空格 → 非空校验 → 赋值」收敛到一处，避免构造器和 {@link #modify} 各写一遍导致规则漂移。
   *
   * @param expression 表达式
   * @param errorMessage 错误提示
   * @param sort 排序（可空）
   */
  private void apply(String expression, String errorMessage, @Nullable Integer sort) {
    String trimmedExpression = StringUtils.trimToNull(expression);
    String trimmedErrorMessage = StringUtils.trimToNull(errorMessage);
    if (trimmedExpression == null) {
      throw new DomainException("表达式不能为空");
    }
    if (trimmedErrorMessage == null) {
      throw new DomainException("错误提示不能为空");
    }
    this.expression = trimmedExpression;
    this.errorMessage = trimmedErrorMessage;
    this.sort = sort;
  }
}
