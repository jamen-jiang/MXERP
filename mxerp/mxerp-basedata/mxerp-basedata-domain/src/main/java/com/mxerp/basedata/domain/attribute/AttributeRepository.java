package com.mxerp.basedata.domain.attribute;

import com.mxerp.shared.domain.repository.Repository;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 属性仓储接口 */
@NullMarked
public interface AttributeRepository extends Repository<Attribute> {

  /**
   * 按产品类型 + 范围 + 代码查询
   *
   * @param productType 产品类型
   * @param scope 范围
   * @param code 属性代码
   * @return 属性（可空），不存在返回 null
   */
  @Nullable Attribute getByProductTypeAndScopeAndCode(
      String productType, String scope, String code);

  /**
   * 按产品类型 + 范围查询列表
   *
   * @param productType 产品类型
   * @param scope 范围
   * @return 属性列表
   */
  List<Attribute> getListByProductTypeAndScope(String productType, String scope);

  /**
   * 判断属性代码是否存在
   *
   * @param productType 产品类型
   * @param scope 范围
   * @param code 属性代码
   * @param id 排除的属性Id（可空）
   * @return true 表示已存在
   */
  boolean isExistByProductTypeAndScopeAndCodeExcludeId(
      String productType, String scope, String code, @Nullable Long id);

  /**
   * 判断属性代码是否存在
   *
   * @param productType 产品类型
   * @param scope 范围
   * @param code 属性代码
   * @return true 表示已存在
   */
  default boolean isExistByProductTypeAndScopeAndCodeExcludeId(
      String productType, String scope, String code) {
    return this.isExistByProductTypeAndScopeAndCodeExcludeId(productType, scope, code, null);
  }
}
