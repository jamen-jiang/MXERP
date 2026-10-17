package com.mxerp.system.domain.company;

import com.mxerp.shared.domain.entity.BaseDeleteEntity;
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

/** 公司联系人 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "CompanyContact",
    indexes = {@Index(name = "IX_CompanyContact_CompanyId", columnList = "CompanyId")})
public class CompanyContact extends BaseDeleteEntity {

  /** 公司Id */
  @Column(name = "CompanyId")
  private Long companyId;

  /** 姓名 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 50)
  private String name;

  /** 电话（可空） */
  @Nationalized
  @Column(name = "Telephone", length = 50)
  @Nullable
  private String telephone;

  /** 地区（可空） */
  @Nationalized
  @Column(name = "Area", length = 50)
  @Nullable
  private String area;

  /** 地址（可空） */
  @Nationalized
  @Column(name = "Address", length = 800)
  @Nullable
  private String address;

  /**
   * 构造器
   *
   * @param name 姓名
   * @param telephone 电话（可空）
   * @param area 地区（可空）
   * @param address 地址（可空）
   * @throws DomainException 姓名为空时抛出
   */
  CompanyContact(
      String name, @Nullable String telephone, @Nullable String area, @Nullable String address) {
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedName == null) {
      throw new DomainException("联系人姓名不能为空");
    }
    this.name = trimmedName;
    this.telephone = telephone;
    this.area = area;
    this.address = address;
  }

  /**
   * 修改联系人
   *
   * @param telephone 电话（可空）
   * @param area 地区（可空）
   * @param address 地址（可空）
   */
  void modify(@Nullable String telephone, @Nullable String area, @Nullable String address) {
    this.telephone = telephone;
    this.area = area;
    this.address = address;
  }
}
