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
   * 判断同一父级下职级名称是否已存在（可排除指定Id）
   *
   * @param name 职级名称
   * @param parentId 父级Id（可空，null 表示父级为 null）
   * @param id 排除的职级Id（可空）
   * @return true 表示已存在
   */
  default boolean existsByNameAndParentIdExcludeId(
      String name, @Nullable Long parentId, @Nullable Long id) {
    Specification<Rank> spec =
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
   * 查询指定职级及其所有子职级（含自身，用 Level 路径匹配）
   *
   * @param id 职级Id
   * @return 职级及其子孙列表
   */
  default List<Rank> findWithChildren(long id) {
    return findAll(buildDescendantsSpec(Set.of(id)));
  }

  /**
   * 按Id集合查职级（含子孙，用 Level 路径匹配）
   *
   * @param ids 职级Id集合
   * @return 职级列表，ids 为空时返回空列表
   */
  default List<Rank> findByIdsOrDescendants(Set<Long> ids) {
    if (ids.isEmpty()) {
      return List.of();
    }
    return findAll(buildDescendantsSpec(ids));
  }

  /**
   * 构造「自身或子孙」条件
   *
   * @param ids 职级Id集合
   * @return 查询条件
   */
  private Specification<Rank> buildDescendantsSpec(Set<Long> ids) {
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
