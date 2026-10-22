package com.mxerp.basedata.domain.supplier;

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

/**
 * 供应商
 *
 * <p>结算币别、税率是引用不是文本：金蝶供应商财务页签（结算币别 {@code FPayCurrencyId} /
 * 默认税率 {@code FTaxRateId}）、D365 的 Currency 都是引用。存文本的话，采购单取不到默认值。
 *
 * <p>拦不拦交易只有一个 {@code disabled}：勾了就表示这个供应商不再用，采购单、询价、发票付款都不许再选它。
 * 系统里没有「资料照用、只停某个环节」的流程（没有 POS，解冻定时任务也不存在），所以不预置冻结范围、
 * 冻结日期这类字段——等真出现按环节拦截的需求，连同判断它的单据一起加。
 *
 * <p>银行账户不在这里，见 {@link com.mxerp.basedata.domain.bankaccount.BankAccount}：它是通用聚合，
 * 用「归属类型 + 归属Id」挂到供应商上，一个供应商可以有多个账户。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Supplier",
    indexes = {
      @Index(name = "IX_Supplier_Name", columnList = "Name"),
      @Index(name = "IX_Supplier_GroupId", columnList = "SupplierGroupId")
      // 注意：Code 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class Supplier extends UpdateAggregateRoot {

  /** 供应商代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 供应商名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 50)
  private String name;

  /** 供应商简称（可空） */
  @Nationalized
  @Column(name = "ShortName", length = 20)
  @Nullable
  private String shortName;

  /** 供应商分类Id */
  @Column(name = "SupplierGroupId", nullable = false)
  private Long supplierGroupId;

  /** 结算币别Id */
  @Column(name = "CurrencyId", nullable = false)
  private Long currencyId;

  /** 税务登记号（可空） */
  @Nationalized
  @Column(name = "TaxRegisterCode", length = 50)
  @Nullable
  private String taxRegisterCode;

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
  @Column(name = "Address", length = 500)
  @Nullable
  private String address;

  /** 电话号码（可空） */
  @Nationalized
  @Column(name = "Telephone", length = 50)
  @Nullable
  private String telephone;

  /** 传真（可空） */
  @Nationalized
  @Column(name = "Fax", length = 20)
  @Nullable
  private String fax;

  /** 邮箱（可空） */
  @Nationalized
  @Column(name = "Email", length = 200)
  @Nullable
  private String email;

  /** 税率Id（可空） */
  @Column(name = "TaxRateId")
  @Nullable
  private Long taxRateId;

  /** 是否停用 */
  @Column(name = "Disabled", nullable = false)
  private Boolean disabled;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param code 供应商代码
   * @param name 供应商名称
   * @param shortName 供应商简称（可空）
   * @param supplierGroupId 供应商分类Id
   * @param currencyId 结算币别Id
   * @param taxRegisterCode 税务登记号（可空）
   * @param country 国家（可空）
   * @param province 省份（可空）
   * @param city 城市（可空）
   * @param district 区县（可空）
   * @param zip 邮政编码（可空）
   * @param address 详细地址（可空）
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param taxRateId 税率Id（可空）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  Supplier(
      String code,
      String name,
      @Nullable String shortName,
      long supplierGroupId,
      long currencyId,
      @Nullable String taxRegisterCode,
      @Nullable String country,
      @Nullable String province,
      @Nullable String city,
      @Nullable String district,
      @Nullable String zip,
      @Nullable String address,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Long taxRateId,
      boolean disabled,
      @Nullable String remark) {
    apply(
        code,
        name,
        shortName,
        supplierGroupId,
        currencyId,
        taxRegisterCode,
        country,
        province,
        city,
        district,
        zip,
        address,
        telephone,
        fax,
        email,
        taxRateId,
        disabled,
        remark);
  }

  /**
   * 修改供应商
   *
   * @param code 供应商代码
   * @param name 供应商名称
   * @param shortName 供应商简称（可空）
   * @param supplierGroupId 供应商分类Id
   * @param currencyId 结算币别Id
   * @param taxRegisterCode 税务登记号（可空）
   * @param country 国家（可空）
   * @param province 省份（可空）
   * @param city 城市（可空）
   * @param district 区县（可空）
   * @param zip 邮政编码（可空）
   * @param address 详细地址（可空）
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param taxRateId 税率Id（可空）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  void modify(
      String code,
      String name,
      @Nullable String shortName,
      long supplierGroupId,
      long currencyId,
      @Nullable String taxRegisterCode,
      @Nullable String country,
      @Nullable String province,
      @Nullable String city,
      @Nullable String district,
      @Nullable String zip,
      @Nullable String address,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Long taxRateId,
      boolean disabled,
      @Nullable String remark) {
    apply(
        code,
        name,
        shortName,
        supplierGroupId,
        currencyId,
        taxRegisterCode,
        country,
        province,
        city,
        district,
        zip,
        address,
        telephone,
        fax,
        email,
        taxRateId,
        disabled,
        remark);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>供应商字段多，构造器与 {@link #modify} 若各写一遍极易漏改。这里把「去空格 → 非空校验 →
   * 赋值」收敛到一处，后续新增字段只需改这一个方法。
   *
   * @param code 供应商代码
   * @param name 供应商名称
   * @param shortName 供应商简称（可空）
   * @param supplierGroupId 供应商分类Id
   * @param currencyId 结算币别Id
   * @param taxRegisterCode 税务登记号（可空）
   * @param country 国家（可空）
   * @param province 省份（可空）
   * @param city 城市（可空）
   * @param district 区县（可空）
   * @param zip 邮政编码（可空）
   * @param address 详细地址（可空）
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param taxRateId 税率Id（可空）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @throws DomainException 代码或名称为空、结算币别为空时抛出
   */
  private void apply(
      String code,
      String name,
      @Nullable String shortName,
      long supplierGroupId,
      long currencyId,
      @Nullable String taxRegisterCode,
      @Nullable String country,
      @Nullable String province,
      @Nullable String city,
      @Nullable String district,
      @Nullable String zip,
      @Nullable String address,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Long taxRateId,
      boolean disabled,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("供应商代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("供应商名称不能为空");
    }
    if (currencyId <= 0) {
      throw new DomainException("结算币别不能为空");
    }
    if (supplierGroupId <= 0) {
      throw new DomainException("供应商分类不能为空");
    }

    this.code = trimmedCode;
    this.name = trimmedName;
    this.shortName = shortName;
    this.supplierGroupId = supplierGroupId;
    this.currencyId = currencyId;
    this.taxRegisterCode = StringUtils.trimToNull(taxRegisterCode);
    this.country = country;
    this.province = province;
    this.city = city;
    this.district = district;
    this.zip = zip;
    this.address = address;
    this.telephone = telephone;
    this.fax = fax;
    this.email = email;
    this.taxRateId = taxRateId;
    this.disabled = disabled;
    this.remark = remark;
  }
}
