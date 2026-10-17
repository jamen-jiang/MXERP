package com.mxerp.system.infrastructure.department;

import com.mxerp.system.domain.department.Department;
import jakarta.persistence.criteria.Predicate;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.Set;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.JpaSpecificationExecutor;
import org.springframework.stereotype.Repository;

/** 部门 JPA 接口 */
@NullMarked
@Repository
public interface DepartmentJpa
    extends JpaRepository<Department, Long>, JpaSpecificationExecutor<Department> {

  /**
   * 按部门编码查询
   *
   * @param code 部门编码
   * @return 部门（可能为空）
   */
  Optional<Department> findByCode(String code);

  /**
   * 查询指定父级下的所有直接子部门
   *
   * @param parentId 父Id
   * @return 子部门列表
   */
  List<Department> findByParentId(Long parentId);

  /**
   * 判断部门编码是否已存在（可排除指定Id）
   *
   * @param code 部门编码
   * @param id 排除的Id（可空，null 表示不排除）
   * @return true 表示已存在
   */
  default boolean existsByCodeExcludeId(String code, @Nullable Long id) {
    return exists(buildExcludeIdSpec(code, null, id));
  }

  /**
   * 判断同一父级下部门名称是否已存在（可排除指定Id）
   *
   * @param name 部门名称
   * @param parentId 父级Id（可空，null 表示父级为 null）
   * @param id 排除的Id（可空，null 表示不排除）
   * @return true 表示已存在
   */
  default boolean existsByNameAndParentIdExcludeId(
      String name, @Nullable Long parentId, @Nullable Long id) {
    return exists(buildExcludeIdSpec(null, name, id).and(buildParentIdSpec(parentId)));
  }

  /**
   * 查询指定部门及其所有子部门（含自身，用 Level 路径匹配）
   *
   * @param id 部门Id
   * @return 部门及其子孙列表
   */
  default List<Department> findWithChildren(long id) {
    return findAll(buildDescendantsSpec(Set.of(id)));
  }

  /**
   * 按Id集合查部门（含子孙，用 Level 路径匹配）
   *
   * @param ids 部门Id集合
   * @return 部门列表，ids 为空时返回空列表
   */
  default List<Department> findByIdsOrDescendants(Set<Long> ids) {
    if (ids.isEmpty()) {
      return List.of();
    }
    return findAll(buildDescendantsSpec(ids));
  }

  /**
   * 构造「编码或名称 + 排除Id」条件
   *
   * @param code 部门编码（可空，与 name 二选一）
   * @param name 部门名称（可空，与 code 二选一）
   * @param id 排除的Id（可空）
   * @return 查询条件
   */
  private Specification<Department> buildExcludeIdSpec(
      @Nullable String code, @Nullable String name, @Nullable Long id) {
    return (root, query, cb) -> {
      List<Predicate> predicates = new ArrayList<>();
      if (code != null) {
        predicates.add(cb.equal(root.get("code"), code));
      }
      if (name != null) {
        predicates.add(cb.equal(root.get("name"), name));
      }
      if (id != null) {
        predicates.add(cb.notEqual(root.get("id"), id));
      }
      return cb.and(predicates.toArray(new Predicate[0]));
    };
  }

  /**
   * 构造「父级」条件（null 表示查根节点）
   *
   * @param parentId 父级Id（可空）
   * @return 查询条件
   */
  private Specification<Department> buildParentIdSpec(@Nullable Long parentId) {
    return (root, query, cb) ->
        parentId == null
            ? cb.isNull(root.get("parentId"))
            : cb.equal(root.get("parentId"), parentId);
  }

  /**
   * 构造「自身或子孙」条件
   *
   * @param ids 部门Id集合
   * @return 查询条件
   */
  private Specification<Department> buildDescendantsSpec(Set<Long> ids) {
    return (root, query, cb) -> {
      List<Predicate> predicates = new ArrayList<>();
      predicates.add(root.get("id").in(ids));
      for (Long id : ids) {
        predicates.add(
            cb.like(cb.concat(cb.concat(",", root.get("level")), ","), "%," + id + ",%"));
      }
      return cb.or(predicates.toArray(new Predicate[0]));
    };
  }
}
