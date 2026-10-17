package com.mxerp.system.domain.app;

import com.mxerp.shared.domain.repository.Repository;
import java.util.List;
import org.jspecify.annotations.NullMarked;

/** 应用仓储接口 */
@NullMarked
public interface AppRepository extends Repository<App> {

  /**
   * 查询指定应用及其所有子应用（包含自身）
   *
   * @param id 应用ID
   * @return 应用及其子应用列表
   */
  List<App> getWithChildren(long id);

  /**
   * 查询指定父级下的所有直接子应用
   *
   * @param parentId 父级ID
   * @return 子应用实体列表
   */
  List<App> getByParentId(long parentId);
}
