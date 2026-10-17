package com.mxerp.basedata.domain.attribute;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 属性工厂，负责创建属性 */
@NullMarked
public class AttributeFactory {

  /** 属性仓储 */
  private final AttributeRepository attributeRepository;

  /**
   * 构造器注入
   *
   * @param attributeRepository 属性仓储
   */
  public AttributeFactory(AttributeRepository attributeRepository) {
    this.attributeRepository = attributeRepository;
  }

  /**
   * 创建属性
   *
   * @param productType 产品类型
   * @param scope 范围:Sku或Spu
   * @param code 属性代码
   * @param name 属性名称
   * @param isKey 是否关键属性
   * @param inputType 输入类型
   * @param dataType 数据类型
   * @param isRequired 是否必填
   * @param isFilter 是否用于筛选
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @return 属性聚合
   * @throws DomainException 属性代码已存在时抛出
   */
  public Attribute create(
      String productType,
      String scope,
      String code,
      String name,
      boolean isKey,
      String inputType,
      String dataType,
      boolean isRequired,
      boolean isFilter,
      @Nullable Integer sort,
      @Nullable String remark) {
    // 校验属性代码唯一
    if (attributeRepository.isExistByProductTypeAndScopeAndCodeExcludeId(
        productType, scope, code)) {
      throw new DomainException("属性代码" + code + "已存在!");
    }
    return new Attribute(
        productType,
        scope,
        code,
        name,
        isKey,
        inputType,
        dataType,
        isRequired,
        isFilter,
        sort,
        remark);
  }
}
