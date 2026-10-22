package com.mxerp.system.domain.company;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.entity.Parent;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import java.time.LocalDate;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 公司（集团下属分工厂）
 *
 * <p>字段照别家的公司主数据取：ERPNext {@code Company} 的 {@code abbr}（简称，单据页眉和编号前缀要用）、
 * {@code parent_company}（上级公司）、{@code tax_id}、{@code date_of_establishment}、
 * {@code default_currency}（必填）；Odoo {@code res.company} 的 {@code company_registry}（工商注册号）、
 * {@code currency_id}（必填）；金蝶「组织机构」的上级组织与禁用。
 *
 * <p>地址与 {@code Supplier} 同形：{@code Country/Province/City/District/Zip} + 详细地址一段（{@code Address}），
 * 便于以后按省市统计和打印。
 *
 * <p>上级公司构成一棵树，层级路径由 {@link CompanyDomainService} 维护（与 {@code Category}、{@code Department}
 * 同一套）；往返、跨公司校验放在领域服务里。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Company",
    indexes = {
      @Index(name = "IX_Company_Name", columnList = "Name"),
      @Index(name = "IX_Company_ParentId", columnList = "ParentId"),
      @Index(name = "IX_Company_Level", columnList = "Level")
      // 注意：同级公司名称唯一由 CompanyRepository.isExistByNameAndParentIdExcludeId 校验
    })
public class Company extends UpdateAggregateRoot implements Parent {

  /** 名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 简称（可空），单据页眉与编号前缀用 */
  @Nationalized
  @Column(name = "ShortName", length = 20)
  @Nullable
  private String shortName;

  /** 上级公司Id（可空，空表示顶级） */
  @Column(name = "ParentId")
  @Nullable
  private Long parentId;

  /** 层级路径（可空，形如 "1,3,5"，由层级编排逻辑维护） */
  @Nationalized
  @Column(name = "Level")
  @Nullable
  private String level;

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

  /** 国家（可空） */
  @Nationalized
  @Column(name = "Country", length = 50)
  @Nullable
  private String country;

  /** 省份（可空） */
  @Nationalized
  @Column(name = "Province", length = 50)
  @Nullable
  private String province;

  /** 城市（可空） */
  @Nationalized
  @Column(name = "City", length = 50)
  @Nullable
  private String city;

  /** 区县（可空） */
  @Nationalized
  @Column(name = "District", length = 50)
  @Nullable
  private String district;

  /** 邮政编码（可空） */
  @Nationalized
  @Column(name = "Zip", length = 20)
  @Nullable
  private String zip;

  /** 详细地址（可空） */
  @Nationalized
  @Column(name = "Address", length = 800)
  @Nullable
  private String address;

  /** 税务登记号（可空） */
  @Nationalized
  @Column(name = "TaxRegisterCode", length = 50)
  @Nullable
  private String taxRegisterCode;

  /** 工商注册号（可空） */
  @Nationalized
  @Column(name = "RegistryCode", length = 50)
  @Nullable
  private String registryCode;

  /** 成立日期（可空） */
  @Column(name = "EstablishedDate")
  @Nullable
  private LocalDate establishedDate;

  /** 默认币别Id */
  @Column(name = "CurrencyId", nullable = false)
  private Long currencyId;

  /** 是否禁用 */
  @Column(name = "Disabled", nullable = false)
  private Boolean disabled = false;

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
   * @param name 名称
   * @param shortName 简称（可空）
   * @param parentId 上级公司Id（可空）
   * @param level 层级路径（可空）
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param country 国家（可空）
   * @param province 省份（可空）
   * @param city 城市（可空）
   * @param district 区县（可空）
   * @param zip 邮政编码（可空）
   * @param address 详细地址（可空）
   * @param taxRegisterCode 税务登记号（可空）
   * @param registryCode 工商注册号（可空）
   * @param establishedDate 成立日期（可空）
   * @param currencyId 默认币别Id
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  Company(
      String name,
      @Nullable String shortName,
      @Nullable Long parentId,
      @Nullable String level,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String country,
      @Nullable String province,
      @Nullable String city,
      @Nullable String district,
      @Nullable String zip,
      @Nullable String address,
      @Nullable String taxRegisterCode,
      @Nullable String registryCode,
      @Nullable LocalDate establishedDate,
      long currencyId,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    apply(
        name,
        shortName,
        parentId,
        level,
        telephone,
        fax,
        email,
        country,
        province,
        city,
        district,
        zip,
        address,
        taxRegisterCode,
        registryCode,
        establishedDate,
        currencyId,
        disabled,
        sort,
        remark);
  }

  /**
   * 修改公司
   *
   * @param name 名称
   * @param shortName 简称（可空）
   * @param parentId 上级公司Id（可空）
   * @param level 层级路径（可空）
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param country 国家（可空）
   * @param province 省份（可空）
   * @param city 城市（可空）
   * @param district 区县（可空）
   * @param zip 邮政编码（可空）
   * @param address 详细地址（可空）
   * @param taxRegisterCode 税务登记号（可空）
   * @param registryCode 工商注册号（可空）
   * @param establishedDate 成立日期（可空）
   * @param currencyId 默认币别Id
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  public void modify(
      String name,
      @Nullable String shortName,
      @Nullable Long parentId,
      @Nullable String level,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String country,
      @Nullable String province,
      @Nullable String city,
      @Nullable String district,
      @Nullable String zip,
      @Nullable String address,
      @Nullable String taxRegisterCode,
      @Nullable String registryCode,
      @Nullable LocalDate establishedDate,
      long currencyId,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    apply(
        name,
        shortName,
        parentId,
        level,
        telephone,
        fax,
        email,
        country,
        province,
        city,
        district,
        zip,
        address,
        taxRegisterCode,
        registryCode,
        establishedDate,
        currencyId,
        disabled,
        sort,
        remark);
  }

  /**
   * 重设层级路径（由层级编排逻辑调用）
   *
   * @param level 层级路径（可空）
   */
  @Override
  public void changeLevel(@Nullable String level) {
    this.level = level;
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * @param name 名称
   * @param shortName 简称（可空）
   * @param parentId 上级公司Id（可空）
   * @param level 层级路径（可空）
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param country 国家（可空）
   * @param province 省份（可空）
   * @param city 城市（可空）
   * @param district 区县（可空）
   * @param zip 邮政编码（可空）
   * @param address 详细地址（可空）
   * @param taxRegisterCode 税务登记号（可空）
   * @param registryCode 工商注册号（可空）
   * @param establishedDate 成立日期（可空）
   * @param currencyId 默认币别Id
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @throws DomainException 代码/名称/默认币别为空时抛出
   */
  private void apply(
      String name,
      @Nullable String shortName,
      @Nullable Long parentId,
      @Nullable String level,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String country,
      @Nullable String province,
      @Nullable String city,
      @Nullable String district,
      @Nullable String zip,
      @Nullable String address,
      @Nullable String taxRegisterCode,
      @Nullable String registryCode,
      @Nullable LocalDate establishedDate,
      long currencyId,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedName == null) {
      throw new DomainException("公司名称不能为空");
    }
    if (currencyId <= 0) {
      throw new DomainException("公司默认币别不能为空");
    }

    this.name = trimmedName;
    this.shortName = shortName;
    this.parentId = parentId;
    this.level = level;
    this.telephone = telephone;
    this.fax = fax;
    this.email = email;
    this.country = country;
    this.province = province;
    this.city = city;
    this.district = district;
    this.zip = zip;
    this.address = address;
    this.taxRegisterCode = taxRegisterCode;
    this.registryCode = registryCode;
    this.establishedDate = establishedDate;
    this.currencyId = currencyId;
    this.disabled = disabled;
    this.sort = sort;
    this.remark = remark;
  }
}
