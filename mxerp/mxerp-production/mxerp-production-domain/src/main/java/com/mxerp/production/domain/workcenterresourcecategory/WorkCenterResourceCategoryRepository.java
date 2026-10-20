package com.mxerp.production.domain.workcenterresourcecategory;

import com.mxerp.shared.domain.repository.Repository;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 工作中心资源类别仓储接口 */
@NullMarked
public interface WorkCenterResourceCategoryRepository
    extends Repository<WorkCenterResourceCategory> {

  /**
   * 按代码查询
   *
   * @param code 工作中心资源类别代码
   * @return 工作中心资源类别（可空），不存在返回 null
   */
  @Nullable WorkCenterResourceCategory getByCode(String code);

  /**
   * 判断工作中心资源类别代码是否存在
   *
   * @param code 工作中心资源类别代码
   * @param id 排除的工作中心资源类别Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistByCodeExcludeId(String code, @Nullable Long id);

  /**
   * 判断工作中心资源类别代码是否存在
   *
   * @param code 工作中心资源类别代码
   * @return true 表示已存在
   */
  default boolean isExistByCodeExcludeId(String code) {
    return isExistByCodeExcludeId(code, null);
  }
}
