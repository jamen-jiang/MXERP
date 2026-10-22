package com.mxerp.basedata.domain.unit;

import com.mxerp.shared.domain.repository.Repository;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 计量单位组仓储接口 */
@NullMarked
public interface UnitGroupRepository extends Repository<UnitGroup> {

  /**
   * 按代码查询
   *
   * @param code 单位组代码
   * @return 单位组（可空），不存在返回 null
   */
  @Nullable UnitGroup getByCode(String code);

  /**
   * 判断单位组代码是否存在
   *
   * @param code 单位组代码
   * @param id 排除的单位组Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistByCodeExcludeId(String code, @Nullable Long id);

  /**
   * 判断单位组代码是否存在
   *
   * @param code 单位组代码
   * @return true 表示已存在
   */
  default boolean isExistByCodeExcludeId(String code) {
    return isExistByCodeExcludeId(code, null);
  }
}
