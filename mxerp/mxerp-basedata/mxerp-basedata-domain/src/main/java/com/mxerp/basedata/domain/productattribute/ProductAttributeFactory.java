package com.mxerp.basedata.domain.productattribute;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 规格属性工厂，负责创建规格属性 */
@NullMarked
public class ProductAttributeFactory {

  /** 规格属性仓储 */
  private final ProductAttributeRepository productAttributeRepository;

  /**
   * 构造器注入
   *
   * @param productAttributeRepository 规格属性仓储
   */
  public ProductAttributeFactory(ProductAttributeRepository productAttributeRepository) {
    this.productAttributeRepository = productAttributeRepository;
  }

  /**
   * 创建规格属性
   *
   * @param categoryId 所属分类Id
   * @param code 规格属性代码
   * @param label 规格属性标签
   * @param inputType 输入类型
   * @param dataType 数据类型
   * @param isFilter 是否用于筛选
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @return 规格属性聚合
   * @throws DomainException 同一分类下规格属性代码已存在时抛出
   */
  public ProductAttribute create(
      Long categoryId,
      String code,
      String label,
      String inputType,
      String dataType,
      boolean isFilter,
      @Nullable Integer sort,
      @Nullable String remark) {
    // 校验同一分类下规格属性代码唯一
    if (productAttributeRepository.isExistByCategoryIdAndCodeExcludeId(categoryId, code)) {
      throw new DomainException("分类" + categoryId + "下规格属性代码" + code + "已存在!");
    }
    return new ProductAttribute(
        categoryId, code, label, inputType, dataType, isFilter, sort, remark);
  }
}
