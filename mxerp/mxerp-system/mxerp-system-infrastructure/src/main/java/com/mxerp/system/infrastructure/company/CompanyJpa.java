package com.mxerp.system.infrastructure.company;

import com.mxerp.system.domain.company.Company;
import jakarta.persistence.criteria.Predicate;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.JpaSpecificationExecutor;
import org.springframework.stereotype.Repository;

/** 公司 JPA 接口 */
@NullMarked
@Repository
public interface CompanyJpa
    extends JpaRepository<Company, Long>, JpaSpecificationExecutor<Company> {

  /**
   * 按公司代码查询
   *
   * @param code 公司代码
   * @return 公司信息（可能为空）
   */
  Optional<Company> findByCode(String code);

  /**
   * 判断公司代码是否已存在（可排除指定Id）
   *
   * @param code 公司代码
   * @param id 排除的公司Id（可空，null 表示不排除）
   * @return true 表示已存在
   */
  default boolean existsByCodeExcludeId(String code, @Nullable Long id) {
    Specification<Company> spec =
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
}
