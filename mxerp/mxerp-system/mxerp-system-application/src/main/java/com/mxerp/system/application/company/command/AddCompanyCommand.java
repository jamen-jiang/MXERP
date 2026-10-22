package com.mxerp.system.application.company.command;

import com.mxerp.shared.application.cqrs.Command;
import io.swagger.v3.oas.annotations.media.Schema;
import java.time.LocalDate;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 添加公司命令 */
@NullMarked
@Getter
@Schema(description = "添加公司命令")
public class AddCompanyCommand extends Command {

  /** 公司代码 */
  @Schema(description = "公司代码", requiredMode = Schema.RequiredMode.REQUIRED)
  private final String code;

  /** 公司名称 */
  @Schema(description = "公司名称", requiredMode = Schema.RequiredMode.REQUIRED)
  private final String name;

  /** 简称（可空） */
  @Schema(description = "简称")
  @Nullable
  private final String shortName;

  /** 上级公司Id（可空） */
  @Schema(description = "上级公司Id")
  @Nullable
  private final Long parentId;

  /** 电话号码（可空） */
  @Schema(description = "电话号码")
  @Nullable
  private final String telephone;

  /** 传真（可空） */
  @Schema(description = "传真")
  @Nullable
  private final String fax;

  /** 邮箱（可空） */
  @Schema(description = "邮箱")
  @Nullable
  private final String email;

  /** 国家（可空） */
  @Schema(description = "国家")
  @Nullable
  private final String country;

  /** 省份（可空） */
  @Schema(description = "省份")
  @Nullable
  private final String province;

  /** 城市（可空） */
  @Schema(description = "城市")
  @Nullable
  private final String city;

  /** 区县（可空） */
  @Schema(description = "区县")
  @Nullable
  private final String district;

  /** 邮政编码（可空） */
  @Schema(description = "邮政编码")
  @Nullable
  private final String zip;

  /** 详细地址（可空） */
  @Schema(description = "详细地址")
  @Nullable
  private final String address;

  /** 税务登记号（可空） */
  @Schema(description = "税务登记号")
  @Nullable
  private final String taxRegisterCode;

  /** 工商注册号（可空） */
  @Schema(description = "工商注册号")
  @Nullable
  private final String registryCode;

  /** 成立日期（可空） */
  @Schema(description = "成立日期")
  @Nullable
  private final LocalDate establishedDate;

  /** 默认币别Id */
  @Schema(description = "默认币别Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private final Long currencyId;

  /** 是否禁用 */
  @Schema(description = "是否禁用", requiredMode = Schema.RequiredMode.REQUIRED)
  private final boolean disabled;

  /** 排序（可空） */
  @Schema(description = "排序")
  @Nullable
  private final Integer sequence;

  /** 备注（可空） */
  @Schema(description = "备注")
  @Nullable
  private final String remark;

  /**
   * 构造器
   *
   * @param code 公司代码
   * @param name 公司名称
   * @param shortName 简称（可空）
   * @param parentId 上级公司Id（可空）
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
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   */
  public AddCompanyCommand(
      String code,
      String name,
      @Nullable String shortName,
      @Nullable Long parentId,
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
      Long currencyId,
      boolean disabled,
      @Nullable Integer sequence,
      @Nullable String remark) {
    this.code = code;
    this.name = name;
    this.shortName = shortName;
    this.parentId = parentId;
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
    this.sequence = sequence;
    this.remark = remark;
  }
}
