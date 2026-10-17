package com.mxerp.system.domain.rank;

import com.mxerp.shared.domain.repository.Repository;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 职级仓储接口 */
@NullMarked
public interface RankRepository extends Repository<Rank> {

  /**
   * 查询指定职级及其所有子职级（含自身）
   *
   * @param id 职级Id
   * @return 职级及其子职级列表
   */
  List<Rank> getWithChildren(long id);

  /**
   * 查询指定父级下的所有直接子职级
   *
   * @param parentId 父级Id
   * @return 子职级列表
   */
  List<Rank> getByParentId(long parentId);

  /**
   * 判断同一父级下职级名称是否存在（新增时校验）
   *
   * @param name 职级名称
   * @param parentId 父级Id（可空）
   * @return true 表示已存在
   */
  default boolean isExist(String name, @Nullable Long parentId) {
    return isExist(name, parentId, null);
  }

  /**
   * 判断同一父级下职级名称是否存在
   *
   * @param name 职级名称
   * @param parentId 父级Id（可空）
   * @param id 排除的职级Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExist(String name, @Nullable Long parentId, @Nullable Long id);
}
