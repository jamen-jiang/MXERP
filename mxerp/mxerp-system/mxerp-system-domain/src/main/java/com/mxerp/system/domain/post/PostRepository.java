package com.mxerp.system.domain.post;

import com.mxerp.shared.domain.repository.Repository;
import java.util.Collection;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 岗位仓储接口 */
@NullMarked
public interface PostRepository extends Repository<Post> {

  /**
   * 按岗位名称查询
   *
   * @param name 岗位名称
   * @return 岗位（可空），不存在返回 null
   */
  @Nullable Post getByName(String name);

  /**
   * 按部门Id列表批量查询
   *
   * @param departmentIds 部门Id列表
   * @return 岗位列表
   */
  List<Post> getListByDepartmentIds(Collection<Long> departmentIds);

  /**
   * 判断岗位名称是否存在
   *
   * @param name 岗位名称
   * @param id 排除的岗位Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistByNameExcludeId(String name, @Nullable Long id);

  /**
   * 判断岗位名称是否存在（新增时校验）
   *
   * @param name 岗位名称
   * @return true 表示已存在
   */
  default boolean isExistByNameExcludeId(String name) {
    return isExistByNameExcludeId(name, null);
  }
}
