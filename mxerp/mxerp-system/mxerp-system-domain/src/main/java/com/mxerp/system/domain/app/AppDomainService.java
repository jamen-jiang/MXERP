package com.mxerp.system.domain.app;

import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.HierarchyUtils;
import java.util.ArrayList;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 应用领域服务，负责应用的修改和层级编排 */
@NullMarked
public class AppDomainService {

  /** 应用仓储 */
  private final AppRepository appRepository;

  /**
   * 构造器注入
   *
   * @param appRepository 应用仓储
   */
  public AppDomainService(AppRepository appRepository) {
    this.appRepository = appRepository;
  }

  /**
   * 修改应用（含层级编排）
   *
   * @param id 应用Id
   * @param parentId 父Id（可空）
   * @param name 名称（可空）
   * @param path 路径（可空）
   * @param component 组件路径（可空）
   * @param redirect 路由重定向（可空）
   * @param license 许可证（可空）
   * @param remark 备注（可空）
   * @param title 模块标题
   * @param i18nKey 国际化Key（可空）
   * @param icon 图标（可空）
   * @param isKeepAlive 是否缓存
   * @param fixedIndexInTab 固定页签序号（可空）
   * @param isMultiTab 是否多页签
   * @param isHide 是否隐藏
   * @param sort 排序（可空）
   * @return 需要持久化的聚合列表（自身 + 受影响的子孙）
   * @throws DomainException 应用不存在时抛出
   */
  public List<App> modify(
      Long id,
      @Nullable Long parentId,
      @Nullable String name,
      @Nullable String path,
      @Nullable String component,
      @Nullable String redirect,
      @Nullable String license,
      @Nullable String remark,
      String title,
      @Nullable String i18nKey,
      @Nullable String icon,
      boolean isKeepAlive,
      @Nullable Integer fixedIndexInTab,
      boolean isMultiTab,
      boolean isHide,
      @Nullable Integer sort) {
    App model = appRepository.get(id);
    if (model == null) {
      throw new DomainException("应用不存在!");
    }
    model.modify(
        parentId,
        HierarchyUtils.resolveLevel(parentId, getParent(parentId), "父应用"),
        name,
        path,
        component,
        redirect,
        license,
        remark,
        title,
        i18nKey,
        icon,
        isKeepAlive,
        fixedIndexInTab,
        isMultiTab,
        isHide,
        sort);
    List<App> list = new ArrayList<>();
    setChildrenLevel(model, list);
    list.addFirst(model);
    return list;
  }

  /**
   * 删除自身及所有子孙应用
   *
   * @param id 应用Id
   * @return 需要持久化的聚合列表（已标记删除）
   */
  public List<App> removeWithChildren(Long id) {
    List<App> list = appRepository.getWithChildren(id);
    for (App item : list) {
      item.delete();
    }
    return list;
  }

  /**
   * 查询父应用
   *
   * @param parentId 父Id（可空）
   * @return 父应用，无父Id或不存在时返回 null（可空）
   */
  @Nullable
  private App getParent(@Nullable Long parentId) {
    return parentId == null ? null : appRepository.get(parentId);
  }

  /**
   * 递归设置子应用层级
   *
   * @param model 应用
   * @param list 子应用收集列表
   */
  private void setChildrenLevel(App model, List<App> list) {
    if (model.getId() == 0) {
      return;
    }
    List<App> children = appRepository.getByParentId(model.getId());
    HierarchyUtils.resetChildrenLevel(model, children);
    for (App child : children) {
      setChildrenLevel(child, list);
      list.add(child);
    }
  }
}
