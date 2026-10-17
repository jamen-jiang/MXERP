package com.mxerp.system.domain.company;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
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

/** 公司（集团下属分工厂） */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Company",
    indexes = {
      @Index(name = "IX_Company_Code", columnList = "Code"),
      @Index(name = "IX_Company_Name", columnList = "Name")
      // 注意：Code 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class Company extends UpdateAggregateRoot {

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

  /**
   * 构造器
   *
   * @param code 代码
   * @param name 名称
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param areaCode 地区代码（可空）
   * @param postCode 邮政编码（可空）
   * @param address 地址（可空）
   * @param billingAddress 账户地址（可空）
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @throws DomainException 代码为空或区域/组别不在枚举范围内时抛出
   */
  Company(
      String code,
      String name,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String areaCode,
      @Nullable String postCode,
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
    this.code = trimmedCode;
    this.name = trimmedName;
    this.telephone = telephone;
    this.fax = fax;
    this.email = email;
    this.areaCode = areaCode;
    this.postCode = postCode;
    this.address = address;
    this.billingAddress = billingAddress;
    this.sort = sort;
    this.remark = remark;
  }

  /**
   * 修改公司
   *
   * @param code 代码
   * @param name 名称
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param areaCode 地区代码（可空）
   * @param postCode 邮政编码（可空）
   * @param address 地址（可空）
   * @param billingAddress 账户地址（可空）
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @throws DomainException 代码为空或区域/组别不在枚举范围内时抛出
   */
  public void modify(
      String code,
      String name,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String areaCode,
      @Nullable String postCode,
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
    this.code = trimmedCode;
    this.name = trimmedName;
    this.telephone = telephone;
    this.fax = fax;
    this.email = email;
    this.areaCode = areaCode;
    this.postCode = postCode;
    this.address = address;
    this.billingAddress = billingAddress;
    this.sort = sort;
    this.remark = remark;
  }
}
