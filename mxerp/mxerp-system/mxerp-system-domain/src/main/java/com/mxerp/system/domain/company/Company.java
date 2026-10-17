package com.mxerp.system.domain.company;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.EnumUtils;
import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.Index;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToMany;
import jakarta.persistence.Table;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Objects;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.hibernate.annotations.SQLRestriction;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 公司（集团下属分工厂） */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Company",
    indexes = {
      @Index(name = "IX_Company_Group", columnList = "[Group]"),
      @Index(name = "IX_Company_Code", columnList = "Code"),
      @Index(name = "IX_Company_Name", columnList = "Name")
      // 注意：Code 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class Company extends UpdateAggregateRoot {

  /** 区域 */
  @Nationalized
  @Column(name = "Region", nullable = false, length = 50)
  private String region;

  /** 组别（可空） */
  @Nationalized
  @Column(name = "Group", length = 50)
  @Nullable
  private String group;

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

  /** 地区代码（可空） */
  @Nationalized
  @Column(name = "AreaCode", length = 20)
  @Nullable
  private String areaCode;

  /** 邮政编码（可空） */
  @Nationalized
  @Column(name = "PostCode", length = 20)
  @Nullable
  private String postCode;

  /** 币种Id */
  @Column(name = "CurrencyId", nullable = false)
  private Long currencyId;

  /** 加成 */
  @Column(name = "MarkUp", nullable = false)
  private Double markUp = 0.0;

  /** 地址（可空） */
  @Nationalized
  @Column(name = "Address", length = 800)
  @Nullable
  private String address;

  /** 账户地址（可空） */
  @Nationalized
  @Column(name = "BillingAddress", length = 800)
  @Nullable
  private String billingAddress;

  /** 排序（可空） */
  @Column(name = "Sort")
  @Nullable
  private Integer sort;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /** 联系人列表 */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "CompanyId")
  @SQLRestriction("IsDeleted = 0")
  private List<CompanyContact> companyContacts = new ArrayList<>();

  /**
   * 获取联系人列表（只读）
   *
   * @return 不可变视图
   */
  public List<CompanyContact> getCompanyContacts() {
    return Collections.unmodifiableList(companyContacts);
  }

  /**
   * 构造器
   *
   * @param region 区域
   * @param group 组别（可空）
   * @param code 代码
   * @param name 名称
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param areaCode 地区代码（可空）
   * @param postCode 邮政编码（可空）
   * @param currencyId 币种Id
   * @param address 地址（可空）
   * @param billingAddress 账户地址（可空）
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @throws DomainException 代码为空或区域/组别不在枚举范围内时抛出
   */
  Company(
      String region,
      @Nullable String group,
      String code,
      String name,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String areaCode,
      @Nullable String postCode,
      Long currencyId,
      @Nullable String address,
      @Nullable String billingAddress,
      @Nullable Integer sort,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    if (trimmedCode == null) {
      throw new DomainException("公司代码不能为空");
    }
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedName == null) {
      throw new DomainException("公司名称不能为空");
    }
    this.region = validateRegion(region);
    this.group = validateGroup(group);
    this.code = trimmedCode;
    this.name = trimmedName;
    this.telephone = telephone;
    this.fax = fax;
    this.email = email;
    this.areaCode = areaCode;
    this.postCode = postCode;
    this.currencyId = currencyId;
    this.address = address;
    this.billingAddress = billingAddress;
    this.sort = sort;
    this.remark = remark;
  }

  /**
   * 修改公司
   *
   * @param region 区域
   * @param group 组别（可空）
   * @param code 代码
   * @param name 名称
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param areaCode 地区代码（可空）
   * @param postCode 邮政编码（可空）
   * @param currencyId 币种Id
   * @param address 地址（可空）
   * @param billingAddress 账户地址（可空）
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @throws DomainException 代码为空或区域/组别不在枚举范围内时抛出
   */
  public void modify(
      String region,
      @Nullable String group,
      String code,
      String name,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String areaCode,
      @Nullable String postCode,
      Long currencyId,
      @Nullable String address,
      @Nullable String billingAddress,
      @Nullable Integer sort,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    if (trimmedCode == null) {
      throw new DomainException("公司代码不能为空");
    }
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedName == null) {
      throw new DomainException("公司名称不能为空");
    }
    this.region = validateRegion(region);
    this.group = validateGroup(group);
    this.code = trimmedCode;
    this.name = trimmedName;
    this.telephone = telephone;
    this.fax = fax;
    this.email = email;
    this.areaCode = areaCode;
    this.postCode = postCode;
    this.currencyId = currencyId;
    this.address = address;
    this.billingAddress = billingAddress;
    this.sort = sort;
    this.remark = remark;
  }

  /**
   * 修改加成
   *
   * @param markUp 加成
   */
  public void modify(Double markUp) {
    this.markUp = markUp;
  }

  /**
   * 添加联系人
   *
   * @param name 姓名
   * @param telephone 电话（可空）
   * @param area 地区（可空）
   * @param address 地址（可空）
   * @throws DomainException 联系人姓名已存在时抛出
   */
  public void addContact(
      String name, @Nullable String telephone, @Nullable String area, @Nullable String address) {
    if (isExistContact(name)) {
      throw new DomainException("联系人" + name + "已存在!");
    }
    companyContacts.add(new CompanyContact(name, telephone, area, address));
  }

  /**
   * 修改联系人
   *
   * @param contactId 联系人Id
   * @param telephone 电话（可空）
   * @param area 地区（可空）
   * @param address 地址（可空）
   * @throws DomainException 联系人不存在时抛出
   */
  public void modifyContact(
      Long contactId, @Nullable String telephone, @Nullable String area, @Nullable String address) {
    CompanyContact contact = getContact(contactId);
    if (contact == null) {
      throw new DomainException("联系人Id" + contactId + "不存在!");
    }
    contact.modify(telephone, area, address);
  }

  /**
   * 删除联系人（逻辑删除）
   *
   * @param contactId 联系人Id
   */
  public void deleteContact(Long contactId) {
    CompanyContact contact = getContact(contactId);
    if (contact == null) {
      return;
    }
    contact.delete();
  }

  /**
   * 按Id查联系人
   *
   * @param contactId 联系人Id
   * @return 联系人，不存在返回 null（可空）
   */
  @Nullable
  private CompanyContact getContact(Long contactId) {
    return companyContacts.stream()
        .filter(c -> Objects.equals(c.getId(), contactId))
        .findFirst()
        .orElse(null);
  }

  /**
   * 校验并返回区域
   *
   * @param region 区域
   * @return trim 后的区域
   * @throws DomainException 区域不在枚举范围内时抛出
   */
  private static String validateRegion(String region) {
    String trimmed = StringUtils.trimToNull(region);
    if (trimmed == null || EnumUtils.fromCode(CompanyRegionEnum.class, trimmed) == null) {
      throw new DomainException("公司区域" + region + "有误!");
    }
    return trimmed;
  }

  /**
   * 校验并返回组别
   *
   * @param group 组别（可空）
   * @return trim 后的组别，空值返回 null
   * @throws DomainException 组别不在枚举范围内时抛出
   */
  @Nullable
  private static String validateGroup(@Nullable String group) {
    String trimmed = StringUtils.trimToNull(group);
    if (trimmed == null) {
      return null;
    }
    if (EnumUtils.fromCode(CompanyGroupEnum.class, trimmed) == null) {
      throw new DomainException("公司组别" + group + "有误!");
    }
    return trimmed;
  }

  /**
   * 判断联系人是否存在
   *
   * @param name 姓名
   * @return true 表示已存在
   */
  private boolean isExistContact(String name) {
    return companyContacts.stream()
        .anyMatch(c -> Objects.equals(c.getName(), name) && !c.getIsDeleted());
  }
}
