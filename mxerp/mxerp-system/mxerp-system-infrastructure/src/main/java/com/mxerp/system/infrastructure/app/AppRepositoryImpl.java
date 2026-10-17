package com.mxerp.system.infrastructure.app;

import com.mxerp.system.domain.app.App;
import com.mxerp.system.domain.app.AppRepository;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.springframework.stereotype.Repository;

/** 应用仓储实现 */
@NullMarked
@Repository
public class AppRepositoryImpl implements AppRepository {

  /** JPA 接口 */
  private final AppJpa jpa;

  /**
   * 构造器注入
   *
   * @param jpa JPA 接口
   */
  public AppRepositoryImpl(AppJpa jpa) {
    this.jpa = jpa;
  }

  /**
   * 按主键查询
   *
   * @param id 主键
   * @return 应用，不存在返回 null
   */
  @Override
  @Nullable
  public App get(long id) {
    return jpa.findById(id).orElse(null);
  }

  /**
   * 查询指定应用及其所有子应用（含自身）
   *
   * @param id 应用Id
   * @return 应用及子应用列表
   */
  @Override
  public List<App> getWithChildren(long id) {
    return jpa.findWithChildren(id);
  }

  /**
   * 查询指定父级下的所有直接子应用
   *
   * @param parentId 父应用Id
   * @return 子应用列表
   */
  @Override
  public List<App> getByParentId(long parentId) {
    return jpa.findByParentId(parentId);
  }

  /**
   * 保存（新增或更新）
   *
   * @param model 应用聚合
   */
  @Override
  public void save(App model) {
    jpa.save(model);
  }
}
