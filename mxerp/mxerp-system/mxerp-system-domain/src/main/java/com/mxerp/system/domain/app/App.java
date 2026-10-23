package com.mxerp.system.domain.app;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.entity.Parent;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.EnumUtils;
import jakarta.persistence.*;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 应用 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "App",
    indexes = {
      @Index(name = "IX_App_Type", columnList = "Type"),
      @Index(name = "IX_App_ParentId", columnList = "ParentId"),
      @Index(name = "IX_App_ParentPath", columnList = "ParentPath"),
      @Index(name = "IX_App_Name", columnList = "Name"),
      @Index(name = "IX_App_Sequence", columnList = "Sequence"),
    })
public class App extends UpdateAggregateRoot implements Parent {

  /** 类型 */
  @Nationalized
  @Column(name = "Type", nullable = false, length = 50)
  private String type;

  /** 父Id（可空） */
  @Column(name = "ParentId")
  @Nullable
  private Long parentId;

  /** 父级路径（可空，形如 "1-3-5"） */
  @Nationalized
  @Column(name = "ParentPath")
  @Nullable
  private String parentPath;

  /** 名称（可空） */
  @Nationalized
  @Column(name = "Name", length = 200)
  @Nullable
  private String name;

  /** 路径（可空） */
  @Nationalized
  @Column(name = "Path", length = 500)
  @Nullable
  private String path;

  /** 组件路径（可空） */
  @Nationalized
  @Column(name = "Component", length = 500)
  @Nullable
  private String component;

  /** 路由重定向（可空） */
  @Nationalized
  @Column(name = "Redirect", length = 500)
  @Nullable
  private String redirect;

  /** 许可证（可空） */
  @Nationalized
  @Column(name = "License", length = 200)
  @Nullable
  private String license;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /** 路由元数据 */
  @Embedded private MetaVO meta;

  App(
      String type,
      @Nullable Long parentId,
      @Nullable String parentPath,
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
      @Nullable Integer sequence) {
    this.type = validateType(type);
    this.parentId = parentId;
    this.parentPath = StringUtils.trimToNull(parentPath);
    this.name = StringUtils.trimToNull(name);
    this.path = StringUtils.trimToNull(path);
    this.component = StringUtils.trimToNull(component);
    this.redirect = StringUtils.trimToNull(redirect);
    this.license = StringUtils.trimToNull(license);
    this.remark = remark;
    this.meta =
        new MetaVO(
            title, i18nKey, icon, isKeepAlive, fixedIndexInTab, isMultiTab, isHide, sequence);
  }

  void modify(
      @Nullable Long parentId,
      @Nullable String parentPath,
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
      @Nullable Integer sequence) {
    if (parentId != null && parentId.equals(this.getId())) {
      throw new DomainException("上级应用不能是自己!");
    }
    this.parentId = parentId;
    this.parentPath = parentPath;
    this.name = StringUtils.trimToNull(name);
    this.path = StringUtils.trimToNull(path);
    this.component = StringUtils.trimToNull(component);
    this.redirect = StringUtils.trimToNull(redirect);
    this.license = StringUtils.trimToNull(license);
    this.remark = remark;
    this.meta =
        new MetaVO(
            title, i18nKey, icon, isKeepAlive, fixedIndexInTab, isMultiTab, isHide, sequence);
  }

  @Override
  public void changeParentPath(@Nullable String parentPath) {
    this.parentPath = parentPath;
  }

  /**
   * 校验并规范化应用类型
   *
   * @param type 应用类型代码（可空）
   * @return 去除首尾空格后的类型代码
   * @throws DomainException 类型为空或不在 {@link AppTypeEnum} 范围内时抛出
   */
  private static String validateType(@Nullable String type) {
    String trimmed = StringUtils.trimToNull(type);
    if (trimmed == null || EnumUtils.fromCode(AppTypeEnum.class, trimmed) == null) {
      throw new DomainException("类型" + type + "有误!");
    }
    return trimmed;
  }
}
