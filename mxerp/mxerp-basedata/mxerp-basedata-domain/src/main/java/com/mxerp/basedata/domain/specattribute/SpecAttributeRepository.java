package com.mxerp.basedata.domain.specattribute;

import com.mxerp.shared.domain.repository.Repository;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 规格属性仓储接口 */
@NullMarked
public interface SpecAttributeRepository extends Repository<SpecAttribute> {

  /**
   * 按分类 + 代码查询
   *
   * @param categoryId 所属分类Id
   * @param code 规格属性代码
   * @return 规格属性（可空），不存在返回 null
   */
  @Nullable SpecAttribute getByCategoryIdAndCode(long categoryId, String code);

  /**
   * 按分类查询列表
   *
   * @param categoryId 所属分类Id
   * @return 规格属性列表
   */
  List<SpecAttribute> getListByCategoryId(long categoryId);

  /**
   * 判断规格属性代码在分类下是否已存在
   *
   * @param categoryId 所属分类Id
   * @param code 规格属性代码
   * @param id 排除的规格属性Id（可空）
   * @return true 表示已存在
   */
  boolean isExistByCategoryIdAndCodeExcludeId(long categoryId, String code, @Nullable Long id);

  /**
   * 判断规格属性代码在分类下是否已存在
   *
   * @param categoryId 所属分类Id
   * @param code 规格属性代码
   * @return true 表示已存在
   */
  default boolean isExistByCategoryIdAndCodeExcludeId(long categoryId, String code) {
    return isExistByCategoryIdAndCodeExcludeId(categoryId, code, null);
  }
}
