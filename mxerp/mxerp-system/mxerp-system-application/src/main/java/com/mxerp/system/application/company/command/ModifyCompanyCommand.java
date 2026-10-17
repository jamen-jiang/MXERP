package com.mxerp.system.application.company.command;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 修改公司命令 */
@NullMarked
@Getter
@Schema(description = "修改公司命令")
public class ModifyCompanyCommand extends AddCompanyCommand {

  /** 公司Id */
  @Schema(description = "公司Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private final Long id;

  /**
   * 构造器
   *
   * @param id 公司Id
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
  public ModifyCompanyCommand(
      Long id,
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
    super(
        code,
        name,
        telephone,
        fax,
        email,
        areaCode,
        postCode,
        address,
        billingAddress,
        sort,
        remark);
    this.id = id;
  }
}
