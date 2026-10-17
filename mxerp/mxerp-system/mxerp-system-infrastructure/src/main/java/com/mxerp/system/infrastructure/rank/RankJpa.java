package com.mxerp.system.infrastructure.rank;

import com.mxerp.system.domain.rank.Rank;
import jakarta.persistence.criteria.Predicate;
import java.util.ArrayList;
import java.util.List;
import java.util.Set;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.JpaSpecificationExecutor;
import org.springframework.stereotype.Repository;

/** 职级 JPA 接口 */
@NullMarked
@Repository
public interface RankJpa extends JpaRepository<Rank, Long>, JpaSpecificationExecutor<Rank> {

  /**
   * 查询指定父级下的所有直接子职级
   *
   * @param parentId 父级职级Id
   * @return 直接子职级列表
   */
  List<Rank> findByParentId(long parentId);

  /**
   * 查询指定职级及其所有子职级（包含自身）
   *
   * @param id 职级ID
   * @return 指定职级及其所有子职级列表（包含自身）
   */
  default List<Rank> findWithChildren(long id) {
    Specification<Rank> spec =
        (root, query, cb) ->
            cb.or(
                cb.equal(root.get("id"), id),
                cb.like(cb.concat(cb.concat(",", root.get("level")), ","), "%," + id + ",%"));
    return findAll(spec);
  }

  /**
   * 按Id集合查职级（含子孙，Level 路径包含）
   *
   * @param ids 职级Id集合
   * @return 匹配的职级列表（包含指定Id本身及其子孙职级）；若 ids 为空则返回空列表
   */
  default List<Rank> findByIdsOrDescendants(Set<Long> ids) {
    if (ids.isEmpty()) {
      return List.of();
    }
    Specification<Rank> spec =
        (root, query, cb) -> {
          List<Predicate> predicates = new ArrayList<>();
          predicates.add(root.get("id").in(ids));
          Predicate[] levelPredicates =
              ids.stream()
                  .map(
                      id ->
                          cb.like(
                              cb.concat(cb.concat(",", root.get("level")), ","), "%," + id + ",%"))
                  .toArray(Predicate[]::new);
          predicates.add(cb.or(levelPredicates));
          return cb.or(predicates.toArray(new Predicate[0]));
        };
    return findAll(spec);
  }

  /**
   * 判断同一父级下职级名称是否已存在（可排除自身）
   *
   * @param name 职级名称
   * @param parentId 父级Id（可空）
   * @param id 排除的职级Id（可空）
   * @return 是否存在
   */
  default boolean existsByNameAndParentIdExcludeId(
      String name, @Nullable Long parentId, @Nullable Long id) {
    Specification<Rank> spec =
        (root, query, cb) -> {
          var predicate = cb.equal(root.get("name"), name);
          predicate =
              parentId == null
                  ? cb.and(predicate, cb.isNull(root.get("parentId")))
                  : cb.and(predicate, cb.equal(root.get("parentId"), parentId));
          if (id != null) {
            predicate = cb.and(predicate, cb.notEqual(root.get("id"), id));
          }
          return predicate;
        };
    return exists(spec);
  }
}
