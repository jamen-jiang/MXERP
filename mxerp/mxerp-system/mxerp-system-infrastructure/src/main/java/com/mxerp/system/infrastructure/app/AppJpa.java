package com.mxerp.system.infrastructure.app;

import com.mxerp.system.domain.app.App;
import java.util.List;
import java.util.Set;
import org.jspecify.annotations.NullMarked;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.JpaSpecificationExecutor;
import org.springframework.stereotype.Repository;

/** 应用 JPA 接口 */
@NullMarked
@Repository
public interface AppJpa extends JpaRepository<App, Long>, JpaSpecificationExecutor<App> {

  /**
   * 按Id集合查应用
   *
   * @param ids 应用Id集合
   * @return 应用集合
   */
  Set<App> findByIdIn(Set<Long> ids);

  /**
   * 查询指定父级下的所有直接子应用
   *
   * @param parentId 父应用Id
   * @return 子应用列表
   */
  List<App> findByParentId(long parentId);

  /**
   * 查询自身及所有后代
   *
   * @param id 应用 ID
   * @return 应用及其后代列表
   */
  default List<App> findWithChildren(long id) {
    Specification<App> spec =
        (root, query, cb) ->
            cb.or(
                cb.equal(root.get("id"), id),
                cb.like(cb.concat(cb.concat("-", root.get("parentPath")), "-"), "%-" + id + "-%"));
    return findAll(spec);
  }
}
