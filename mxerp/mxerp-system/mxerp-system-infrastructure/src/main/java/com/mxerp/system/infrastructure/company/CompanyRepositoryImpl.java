package com.mxerp.system.infrastructure.company;

import com.mxerp.system.domain.company.Company;
import com.mxerp.system.domain.company.CompanyRepository;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.springframework.stereotype.Repository;

/** 公司仓储实现 */
@NullMarked
@Repository
public class CompanyRepositoryImpl implements CompanyRepository {

  /** JPA 接口 */
  private final CompanyJpa jpa;

  /**
   * 构造器注入
   *
   * @param jpa JPA 接口
   */
  public CompanyRepositoryImpl(CompanyJpa jpa) {
    this.jpa = jpa;
  }

  /**
   * 按主键查询
   *
   * @param id 主键
   * @return 公司，不存在返回 null
   */
  @Override
  @Nullable
  public Company get(long id) {
    return jpa.findById(id).orElse(null);
  }

  /**
   * 按公司代码查询
   *
   * @param code 公司代码
   * @return 公司，不存在返回 null
   */
  @Override
  @Nullable
  public Company getByCode(String code) {
    return jpa.findByCode(code).orElse(null);
  }

  /**
   * 按Id列表批量查询公司
   *
   * @param ids 公司Id列表
   * @return 公司列表
   */
  @Override
  public List<Company> getListByIds(List<Long> ids) {
    if (ids.isEmpty()) {
      return List.of();
    }
    return jpa.findAllById(ids);
  }

  /**
   * 判断公司代码是否已存在（可排除指定Id）
   *
   * @param code 公司代码
   * @param id 排除的公司Id（可空）
   * @return true 表示已存在
   */
  @Override
  public boolean isExistByCodeExcludeId(String code, @Nullable Long id) {
    return jpa.existsByCodeExcludeId(code, id);
  }

  /**
   * 保存（新增或更新）
   *
   * @param model 公司聚合
   */
  @Override
  public void save(Company model) {
    jpa.save(model);
  }
}
