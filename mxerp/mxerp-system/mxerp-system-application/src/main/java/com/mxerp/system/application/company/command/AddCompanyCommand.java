package com.mxerp.system.application.company.command;

import com.mxerp.shared.application.cqrs.Command;
import io.swagger.v3.oas.annotations.media.Schema;
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

  /** 地区代码（可空） */
  @Schema(description = "地区代码")
  @Nullable
  private final String areaCode;

  /** 邮政编码（可空） */
  @Schema(description = "邮政编码")
  @Nullable
  private final String postCode;

  /** 地址（可空） */
  @Schema(description = "地址")
  @Nullable
  private final String address;

  /** 账户地址（可空） */
  @Schema(description = "账户地址")
  @Nullable
  private final String billingAddress;

  /** 排序（可空） */
  @Schema(description = "排序")
  @Nullable
  private final Integer sort;

  /** 备注（可空） */
  @Schema(description = "备注")
  @Nullable
  private final String remark;

  /**
   * 构造器
   *
   * @param code 公司代码
   * @param name 公司名称
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param areaCode 地区代码（可空）
   * @param postCode 邮政编码（可空）
   * @param address 地址（可空）
   * @param billingAddress 账户地址（可空）
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  public AddCompanyCommand(
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
    this.code = code;
    this.name = name;
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
