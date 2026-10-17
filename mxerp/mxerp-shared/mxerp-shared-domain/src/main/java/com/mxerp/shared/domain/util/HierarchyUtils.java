package com.mxerp.shared.domain.util;

import com.mxerp.shared.domain.entity.Parent;
import com.mxerp.shared.domain.exception.DomainException;
import java.util.List;
import org.apache.commons.lang3.StringUtils;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 树形层级编排工具：只做纯计算，数据由调用方查好后传入 */
@NullMarked
public final class HierarchyUtils {

  private HierarchyUtils() {}

  /**
   * 根据父节点计算层级路径
   *
   * @param parentId 父节点Id（可空）
   * @param parent 父节点实体，由调用方查询后传入（可空）
   * @param label 父节点名称，用于异常信息
   * @param <T> 树形层级实体类型
   * @return 层级路径（如 "1,3,5"），无父节点返回 null（可空）
   * @throws DomainException 父节点Id非空但实体不存在时抛出
   */
  public static <T extends Parent> @Nullable String resolveLevel(
      @Nullable Long parentId, @Nullable T parent, String label) {
    if (parentId == null) {
      return null;
    }
    if (parent == null) {
      throw new DomainException(label + "不存在: " + parentId);
    }
    if (StringUtils.isBlank(parent.getLevel())) {
      return parent.getId() + "";
    }
    return parent.getLevel() + "," + parent.getId();
  }

  /**
   * 给一批子节点重设层级路径
   *
   * @param parent 父节点
   * @param children 子节点列表，由调用方查询后传入
   * @param <T> 树形层级实体类型
   */
  public static <T extends Parent> void resetChildrenLevel(T parent, List<T> children) {
    if (children.isEmpty()) {
      return;
    }
    String level =
        StringUtils.isBlank(parent.getLevel())
            ? parent.getId().toString()
            : parent.getLevel() + "," + parent.getId();
    for (T child : children) {
      child.changeLevel(level);
    }
  }
}
