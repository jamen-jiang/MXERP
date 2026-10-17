package com.mxerp.system.infrastructure.app;

import com.mxerp.system.domain.app.App;
import jakarta.persistence.criteria.Predicate;
import java.util.ArrayList;
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
   * 查询指定应用及其所有子应用（含自身，用 Level 路径匹配）
   *
   * @param id 应用Id
   * @return 应用及其子孙列表
   */
  default List<App> findWithChildren(long id) {
    return findAll(buildDescendantsSpec(id));
  }

  /**
   * 构造「自身或子孙」条件
   *
   * @param id 应用Id
   * @return 查询条件
   */
  private Specification<App> buildDescendantsSpec(long id) {
    return (root, query, cb) -> {
      List<Predicate> predicates = new ArrayList<>();
      predicates.add(cb.equal(root.get("id"), id));
      predicates.add(
          cb.like(cb.concat(cb.concat(",", root.get("level")), ","), "%," + id + ",%"));
      return cb.or(predicates.toArray(new Predicate[0]));
    };
  }
}
