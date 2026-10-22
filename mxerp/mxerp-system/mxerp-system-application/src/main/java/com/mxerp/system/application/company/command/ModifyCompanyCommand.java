package com.mxerp.system.application.company.command;

import io.swagger.v3.oas.annotations.media.Schema;
import java.time.LocalDate;
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
  public ModifyCompanyCommand(
      long id,
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
    super(
        code,
        name,
        shortName,
        parentId,
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
        sequence,
        remark);
    this.id = id;
  }
}
