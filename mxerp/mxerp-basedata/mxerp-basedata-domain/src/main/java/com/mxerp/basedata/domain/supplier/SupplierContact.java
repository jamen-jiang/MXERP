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
 * 供应商联系人
 *
 * <p>按金蝶供应商「联系人」页签做：姓名必填，固定电话/移动电话/传真/邮箱选填，一行一个联系人、
 * 一个供应商可以挂多个。采购单打印时从这里带对接人。
 *
 * <p>与银行账户（{@code com.mxerp.basedata.domain.bankaccount.BankAccount}）一样是独立聚合：一个供应商多个联系人，
 * 一个字符串字段装不下「业务对接、对账、收货」三个不同的人。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "SupplierContact",
    indexes = {
      @Index(name = "IX_SupplierContact_SupplierId", columnList = "SupplierId")
      // 注意：(SupplierId, Name) 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class SupplierContact extends UpdateAggregateRoot {

  /** 供应商Id */
  @Column(name = "SupplierId", nullable = false)
  private Long supplierId;

  /** 姓名 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 50)
  private String name;

  /** 固定电话（可空） */
  @Nationalized
  @Column(name = "Telephone", length = 50)
  @Nullable
  private String telephone;

  /** 移动电话（可空） */
  @Nationalized
  @Column(name = "Mobile", length = 50)
  @Nullable
  private String mobile;

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

  /** 排序（可空） */
  @Column(name = "Sort")
  @Nullable
  private Integer sort;

  /**
   * 构造器
   *
   * @param supplierId 供应商Id
   * @param name 姓名
   * @param telephone 固定电话（可空）
   * @param mobile 移动电话（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param sort 排序（可空）
   */
  SupplierContact(
      long supplierId,
      String name,
      @Nullable String telephone,
      @Nullable String mobile,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Integer sort) {
    if (supplierId <= 0) {
      throw new DomainException("联系人必须属于某个供应商");
    }
    this.supplierId = supplierId;
    apply(name, telephone, mobile, fax, email, sort);
  }

  /**
   * 修改联系人
   *
   * @param name 姓名
   * @param telephone 固定电话（可空）
   * @param mobile 移动电话（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param sort 排序（可空）
   */
  void modify(
      String name,
      @Nullable String telephone,
      @Nullable String mobile,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Integer sort) {
    apply(name, telephone, mobile, fax, email, sort);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * @param name 姓名
   * @param telephone 固定电话（可空）
   * @param mobile 移动电话（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param sort 排序（可空）
   * @throws DomainException 姓名为空时抛出
   */
  private void apply(
      String name,
      @Nullable String telephone,
      @Nullable String mobile,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Integer sort) {
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedName == null) {
      throw new DomainException("联系人姓名不能为空");
    }
    this.name = trimmedName;
    this.telephone = StringUtils.trimToNull(telephone);
    this.mobile = StringUtils.trimToNull(mobile);
    this.fax = StringUtils.trimToNull(fax);
    this.email = StringUtils.trimToNull(email);
    this.sort = sort;
  }
}
