package com.mxerp.system.infrastructure.rank;

import com.mxerp.system.domain.rank.Rank;
import com.mxerp.system.domain.rank.RankRepository;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.springframework.stereotype.Repository;

/** 职级仓储实现 */
@NullMarked
@Repository
public class RankRepositoryImpl implements RankRepository {

  /** JPA 接口 */
  private final RankJpa jpa;

  /**
   * 构造器注入
   *
   * @param jpa JPA 接口
   */
  public RankRepositoryImpl(RankJpa jpa) {
    this.jpa = jpa;
  }

  /**
   * 按主键查询
   *
   * @param id 主键
   * @return 职级，不存在返回 null
   */
  @Override
  @Nullable
  public Rank get(long id) {
    return jpa.findById(id).orElse(null);
  }

  /**
   * 查询指定职级及其所有子职级（含自身）
   *
   * @param id 职级Id
   * @return 职级及其子职级列表
   */
  @Override
  public List<Rank> getWithChildren(long id) {
    return jpa.findWithChildren(id);
  }

  /**
   * 查询指定父级下的所有直接子职级
   *
   * @param parentId 父级Id
   * @return 子职级列表
   */
  @Override
  public List<Rank> getByParentId(long parentId) {
    return jpa.findByParentId(parentId);
  }

  /**
   * 判断同一父级下职级名称是否已存在（可排除自身）
   *
   * @param name 职级名称
   * @param parentId 父级Id（可空）
   * @param id 排除的职级Id（可空）
   * @return 是否存在
   */
  @Override
  public boolean isExist(String name, @Nullable Long parentId, @Nullable Long id) {
    return jpa.existsByNameAndParentIdExcludeId(name, parentId, id);
  }

  /**
   * 保存（新增或更新）
   *
   * @param model 职级聚合
   */
  @Override
  public void save(Rank model) {
    jpa.save(model);
  }
}
