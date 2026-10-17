package com.mxerp.basedata.domain.brand;

import com.mxerp.shared.domain.repository.Repository;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 产品品牌仓储接口 */
@NullMarked
public interface BrandRepository extends Repository<Brand> {

  /**
   * 按品牌代码查询
   *
   * @param code 品牌代码
   * @return 品牌（可空），不存在返回 null
   */
  @Nullable Brand getByCode(String code);

  /**
   * 判断品牌代码是否存在
   *
   * @param code 品牌代码
   * @param id 排除的品牌Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistByCodeExcludeId(String code, @Nullable Long id);

  /**
   * 判断品牌代码是否存在
   *
   * @param code 品牌代码
   * @return true 表示已存在
   */
  default boolean isExistByCodeExcludeId(String code) {
    return isExistByCodeExcludeId(code, null);
  }
}
