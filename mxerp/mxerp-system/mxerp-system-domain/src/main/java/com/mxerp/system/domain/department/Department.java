package com.mxerp.system.domain.department;

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

/** 部门 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Department",
    indexes = {
      @Index(name = "IX_Department_CompanyId", columnList = "CompanyId"),
      @Index(name = "IX_Department_ParentId", columnList = "ParentId"),
      @Index(name = "IX_Department_Code", columnList = "Code"),
      @Index(name = "IX_Department_Name", columnList = "Name")
    })
public class Department extends UpdateAggregateRoot implements Parent {

  /** 组织机构级别字符串（可空） */
  @Nationalized
  @Column(name = "Level")
  @Nullable
  private String level;

  /** 公司Id */
  @Column(name = "CompanyId", nullable = false)
  private Long companyId;

  /** 父Id（可空） */
  @Column(name = "ParentId")
  @Nullable
  private Long parentId;

  /** 代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 电话号码（可空） */
  @Nationalized
  @Column(name = "Telephone", length = 50)
  @Nullable
  private String telephone;

  /** 传真（可空） */
  @Nationalized
  @Column(name = "Fax", length = 50)
  @Nullable
  private String fax;

  /** 邮箱（可空） */
  @Nationalized
  @Column(name = "Email", length = 200)
  @Nullable
  private String email;

  /** 排序（可空） */
  @Column(name = "Sort")
  @Nullable
  private Integer sort;

  /** 地址（可空） */
  @Nationalized
  @Column(name = "Address", length = 800)
  @Nullable
  private String address;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param level 级别字符串（可空）
   * @param companyId 公司Id
   * @param parentId 父Id（可空）
   * @param code 代码
   * @param name 名称
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param sort 排序（可空）
   * @param address 地址（可空）
   * @param remark 备注（可空）
   */
  public Department(
      @Nullable String level,
      Long companyId,
      @Nullable Long parentId,
      String code,
      String name,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Integer sort,
      @Nullable String address,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("部门代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("部门名称不能为空");
    }
    this.level = StringUtils.trimToNull(level);
    this.companyId = companyId;
    this.parentId = parentId;
    this.code = trimmedCode;
    this.name = trimmedName;
    this.telephone = telephone;
    this.fax = fax;
    this.email = email;
    this.sort = sort;
    this.address = address;
    this.remark = remark;
  }

  /**
   * 修改部门
   *
   * @param level 级别字符串（可空）
   * @param companyId 公司Id
   * @param parentId 父Id（可空）
   * @param code 代码
   * @param name 名称
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param sort 排序（可空）
   * @param address 地址（可空）
   * @param remark 备注（可空）
   */
  public void modify(
      @Nullable String level,
      Long companyId,
      @Nullable Long parentId,
      String code,
      String name,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Integer sort,
      @Nullable String address,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("部门代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("部门名称不能为空");
    }
    this.level = StringUtils.trimToNull(level);
    this.companyId = companyId;
    this.parentId = parentId;
    this.code = trimmedCode;
    this.name = trimmedName;
    this.telephone = telephone;
    this.fax = fax;
    this.email = email;
    this.sort = sort;
    this.address = address;
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
