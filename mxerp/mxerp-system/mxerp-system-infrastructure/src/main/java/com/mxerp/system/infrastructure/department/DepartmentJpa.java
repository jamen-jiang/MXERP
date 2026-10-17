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
   * 按代码查询
   *
   * @param code 部门代码
   * @return 部门
   */
  Optional<Department> findByCode(String code);

  /**
   * 查询指定父级下的所有直接子部门
   *
   * @param parentId 父Id
   * @return 部门列表
   */
  List<Department> findByParentId(Long parentId);

  /**
   * 判断部门编码是否存在（可排除指定 Id）
   *
   * @param code 部门编码
   * @param id 排除的ID（可空，null 表示不排除）
   * @return true 表示已存在
   */
  default boolean existsByCodeExcludeId(String code, @Nullable Long id) {
    Specification<Department> spec =
        (root, query, cb) -> {
          List<Predicate> predicates = new ArrayList<>();
          predicates.add(cb.equal(root.get("code"), code));
          if (id != null) {
            predicates.add(cb.notEqual(root.get("id"), id));
          }
          return cb.and(predicates.toArray(new Predicate[0]));
        };
    return exists(spec);
  }

  /**
   * 判断同一父级下名称是否存在（可排除指定 Id）
   *
   * @param name 部门名称
   * @param parentId 父级ID（可空，null 表示父级为 null）
   * @param id 排除的ID（可空，null 表示不排除）
   * @return true 表示已存在
   */
  default boolean existsByNameAndParentIdExcludeId(
      String name, @Nullable Long parentId, @Nullable Long id) {
    Specification<Department> spec =
        (root, query, cb) -> {
          List<Predicate> predicates = new ArrayList<>();
          predicates.add(cb.equal(root.get("name"), name));
          if (parentId == null) {
            predicates.add(cb.isNull(root.get("parentId")));
          } else {
            predicates.add(cb.equal(root.get("parentId"), parentId));
          }
          if (id != null) {
            predicates.add(cb.notEqual(root.get("id"), id));
          }
          return cb.and(predicates.toArray(new Predicate[0]));
        };
    return exists(spec);
  }

  /**
   * 按Id集合查部门（含子孙，Level 路径包含）
   *
   * @param ids 部门Id集合
   * @return 部门列表
   */
  default List<Department> findByIdsOrDescendants(Set<Long> ids) {
    Specification<Department> spec =
        (root, query, cb) -> {
          var idPredicate = root.get("id").in(ids);
          var levelPredicates =
              ids.stream()
                  .map(
                      id ->
                          cb.like(
                              cb.concat(cb.concat(",", root.get("level")), ","), "%," + id + ",%"))
                  .toArray(jakarta.persistence.criteria.Predicate[]::new);
          return cb.or(idPredicate, cb.or(levelPredicates));
        };
    return findAll(spec);
  }

  /**
   * 查询指定部门及其所有子部门（包含自身）
   *
   * @param id 部门ID
   * @return 部门及其子部门列表
   */
  default List<Department> findWithChildren(Long id) {
    Specification<Department> spec =
        (root, query, cb) ->
            cb.or(
                cb.equal(root.get("id"), id),
                cb.like(cb.concat(cb.concat(",", root.get("level")), ","), "%," + id + ",%"));
    return findAll(spec);
  }
}
