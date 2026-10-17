package com.mxerp.system.infrastructure.post;

import com.mxerp.system.domain.post.Post;
import jakarta.persistence.criteria.Predicate;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import java.util.Optional;
import java.util.Set;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.JpaSpecificationExecutor;
import org.springframework.stereotype.Repository;

/** 岗位 JPA 接口 */
@NullMarked
@Repository
public interface PostJpa extends JpaRepository<Post, Long>, JpaSpecificationExecutor<Post> {

  /**
   * 按岗位名称查询
   *
   * @param name 岗位名称
   * @return 岗位（可能为空）
   */
  Optional<Post> findByName(String name);

  /**
   * 按部门Id集合批量查询
   *
   * @param departmentIds 部门Id集合
   * @return 岗位列表
   */
  List<Post> findByDepartmentIdIn(Collection<Long> departmentIds);

  /**
   * 按部门Id集合 + 职级Id集合查岗位
   *
   * @param departmentIds 部门Id集合
   * @param rankIds 职级Id集合
   * @return 岗位列表
   */
  List<Post> findByDepartmentIdInAndRankIdIn(Set<Long> departmentIds, Set<Long> rankIds);

  /**
   * 判断岗位名称是否已存在（可排除指定Id）
   *
   * @param name 岗位名称
   * @param id 排除的岗位Id（可空）
   * @return true 表示已存在
   */
  default boolean existsByNameExcludeId(String name, @Nullable Long id) {
    Specification<Post> spec =
        (root, query, cb) -> {
          List<Predicate> predicates = new ArrayList<>();
          predicates.add(cb.equal(root.get("name"), name));
          if (id != null) {
            predicates.add(cb.notEqual(root.get("id"), id));
          }
          return cb.and(predicates.toArray(new Predicate[0]));
        };
    return exists(spec);
  }
}
