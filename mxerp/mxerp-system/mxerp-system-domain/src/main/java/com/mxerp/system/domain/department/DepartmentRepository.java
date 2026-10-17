package com.mxerp.system.domain.department;

import com.mxerp.shared.domain.repository.Repository;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 部门仓储接口 */
@NullMarked
public interface DepartmentRepository extends Repository<Department> {

  /**
   * 根据编码查询部门
   *
   * @param code 部门编码
   * @return 部门实体（可空），不存在返回 null
   */
  @Nullable Department getByCode(String code);

  /**
   * 查询指定部门及其所有子部门（包含自身）
   *
   * @param id 部门ID
   * @return 部门及其子部门列表
   */
  List<Department> getWithChildren(long id);

  /**
   * 查询指定父级下的所有直接子部门
   *
   * @param parentId 父级ID
   * @return 子部门实体列表
   */
  List<Department> getListByParentId(long parentId);

  /**
   * 判断编码是否已存在
   *
   * @param code 部门编码
   * @param id 排除的ID（用于更新时校验，可为空）
   * @return 是否存在
   */
  boolean isExistByCodeExcludeId(String code, @Nullable Long id);

  /**
   * 判断编码是否已存在（新增时校验，无需排除ID）
   *
   * @param code 部门编码
   * @return 是否存在
   */
  default boolean isExistByCodeExcludeId(String code) {
    return isExistByCodeExcludeId(code, null);
  }

  /**
   * 判断同一父级下名称是否已存在
   *
   * @param name 部门名称
   * @param parentId 父级ID（可为空）
   * @param id 排除的ID（用于更新时校验，可为空）
   * @return 是否存在
   */
  boolean isExistByNameAndParentIdExcludeId(
      String name, @Nullable Long parentId, @Nullable Long id);

  /**
   * 判断编码是否已存在（新增时校验，无需排除ID）
   *
   * @param name 部门名称
   * @param parentId 父级ID（可为空）
   * @return 是否存在
   */
  default boolean isExistByNameAndParentIdExcludeId(String name, @Nullable Long parentId) {
    return isExistByNameAndParentIdExcludeId(name, parentId, null);
  }
}
