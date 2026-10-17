package com.mxerp.system.domain.rank;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.entity.Parent;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 职级 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Rank",
    indexes = {@Index(name = "IX_Rank_Name", columnList = "Name")})
public class Rank extends UpdateAggregateRoot implements Parent {

  /** 职级级别字符串（可空） */
  @Nationalized
  @Column(name = "Level")
  @Nullable
  private String level;

  /** 父Id（可空） */
  @Column(name = "ParentId")
  @Nullable
  private Long parentId;

  /** 职级名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 50)
  private String name;

  /** 排序（可空） */
  @Column(name = "Sort")
  @Nullable
  private Integer sort;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param level 职级级别字符串（可空）
   * @param parentId 父Id（可空）
   * @param name 职级名称
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  Rank(
      @Nullable String level,
      @Nullable Long parentId,
      String name,
      @Nullable Integer sort,
      @Nullable String remark) {
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedName == null) {
      throw new DomainException("职级名称不能为空");
    }
    this.level = StringUtils.trimToNull(level);
    this.parentId = parentId;
    this.name = trimmedName;
    this.sort = sort;
    this.remark = remark;
  }

  /**
   * 修改职级
   *
   * @param level 职级级别字符串（可空）
   * @param parentId 父Id（可空）
   * @param name 职级名称
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  void modify(
      @Nullable String level,
      @Nullable Long parentId,
      String name,
      @Nullable Integer sort,
      @Nullable String remark) {
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedName == null) {
      throw new DomainException("职级名称不能为空");
    }
    this.level = StringUtils.trimToNull(level);
    this.parentId = parentId;
    this.name = trimmedName;
    this.sort = sort;
    this.remark = remark;
  }

  /**
   * 设置层级
   *
   * @param level 层级字符串
   */
  @Override
  public void changeLevel(@Nullable String level) {
    this.level = level;
  }
}
