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

/** 供应商 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Supplier",
    indexes = {@Index(name = "IX_Supplier_Name", columnList = "Name")
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

  /** 地址（可空） */
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

  /** 负责人（可空） */
  @Nationalized
  @Column(name = "ContactPerson", length = 30)
  @Nullable
  private String contactPerson;

  /** 税率Id（可空） */
  @Column(name = "TaxRateId")
  @Nullable
  private Long taxRateId;

  /** 支付方式（可空） */
  @Nationalized
  @Column(name = "PayMode", length = 200)
  @Nullable
  private String payMode;

  /** 收货方式（可空） */
  @Nationalized
  @Column(name = "DeliveryMode", length = 200)
  @Nullable
  private String deliveryMode;

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
   * @param address 地址（可空）
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param contactPerson 负责人（可空）
   * @param taxRateId 税率Id（可空）
   * @param payMode 支付方式（可空）
   * @param deliveryMode 收货方式（可空）
   * @param remark 备注（可空）
   */
  Supplier(
      String code,
      String name,
      @Nullable String shortName,
      @Nullable String address,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String contactPerson,
      @Nullable Long taxRateId,
      @Nullable String payMode,
      @Nullable String deliveryMode,
      @Nullable String remark) {
    apply(
        code,
        name,
        shortName,
        address,
        telephone,
        fax,
        email,
        contactPerson,
        taxRateId,
        payMode,
        deliveryMode,
        remark);
  }

  /**
   * 修改供应商
   *
   * @param code 供应商代码
   * @param name 供应商名称
   * @param shortName 供应商简称（可空）
   * @param address 地址（可空）
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param contactPerson 负责人（可空）
   * @param taxRateId 税率Id（可空）
   * @param payMode 支付方式（可空）
   * @param deliveryMode 收货方式（可空）
   * @param remark 备注（可空）
   */
  void modify(
      String code,
      String name,
      @Nullable String shortName,
      @Nullable String address,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String contactPerson,
      @Nullable Long taxRateId,
      @Nullable String payMode,
      @Nullable String deliveryMode,
      @Nullable String remark) {
    apply(
        code,
        name,
        shortName,
        address,
        telephone,
        fax,
        email,
        contactPerson,
        taxRateId,
        payMode,
        deliveryMode,
        remark);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>供应商字段多达 12 个，构造器与 {@link #modify} 若各写一遍极易漏改。这里把「去空格 → 非空校验 →
   * 赋值」收敛到一处，后续新增字段只需改这一个方法。
   *
   * @param code 供应商代码
   * @param name 供应商名称
   * @param shortName 供应商简称（可空）
   * @param address 地址（可空）
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param contactPerson 负责人（可空）
   * @param taxRateId 税率Id（可空）
   * @param payMode 支付方式（可空）
   * @param deliveryMode 收货方式（可空）
   * @param remark 备注（可空）
   */
  private void apply(
      String code,
      String name,
      @Nullable String shortName,
      @Nullable String address,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String contactPerson,
      @Nullable Long taxRateId,
      @Nullable String payMode,
      @Nullable String deliveryMode,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("供应商代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("供应商名称不能为空");
    }
    this.code = trimmedCode;
    this.name = trimmedName;
    this.shortName = shortName;
    this.address = address;
    this.telephone = telephone;
    this.fax = fax;
    this.email = email;
    this.contactPerson = contactPerson;
    this.taxRateId = taxRateId;
    this.payMode = payMode;
    this.deliveryMode = deliveryMode;
    this.remark = remark;
  }
}
