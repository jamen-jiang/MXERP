package com.mxerp.system.application.company.query;

import com.mxerp.shared.application.response.BaseResponse;
import io.swagger.v3.oas.annotations.media.Schema;
import java.time.LocalDate;
import lombok.Getter;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 公司响应 */
@NullMarked
@Getter
@Setter
@Schema(description = "公司响应")
public class CompanyResponse extends BaseResponse {

  /** 公司Id */
  @Schema(description = "公司Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private Long id;

  /** 公司名称 */
  @Schema(description = "公司名称", requiredMode = Schema.RequiredMode.REQUIRED)
  private String name = "";

  /** 简称（可空） */
  @Schema(description = "简称")
  @Nullable
  private String shortName;

  /** 上级公司Id（可空） */
  @Schema(description = "上级公司Id")
  @Nullable
  private Long parentId;

  /** 层级路径（可空） */
  @Schema(description = "层级路径")
  @Nullable
  private String level;

  /** 电话号码（可空） */
  @Schema(description = "电话号码")
  @Nullable
  private String telephone;

  /** 传真（可空） */
  @Schema(description = "传真")
  @Nullable
  private String fax;

  /** 邮箱（可空） */
  @Schema(description = "邮箱")
  @Nullable
  private String email;

  /** 国家（可空） */
  @Schema(description = "国家")
  @Nullable
  private String country;

  /** 省份（可空） */
  @Schema(description = "省份")
  @Nullable
  private String province;

  /** 城市（可空） */
  @Schema(description = "城市")
  @Nullable
  private String city;

  /** 区县（可空） */
  @Schema(description = "区县")
  @Nullable
  private String district;

  /** 邮政编码（可空） */
  @Schema(description = "邮政编码")
  @Nullable
  private String zip;

  /** 详细地址（可空） */
  @Schema(description = "详细地址")
  @Nullable
  private String address;

  /** 税务登记号（可空） */
  @Schema(description = "税务登记号")
  @Nullable
  private String taxRegisterCode;

  /** 工商注册号（可空） */
  @Schema(description = "工商注册号")
  @Nullable
  private String registryCode;

  /** 成立日期（可空） */
  @Schema(description = "成立日期")
  @Nullable
  private LocalDate establishedDate;

  /** 默认币别Id */
  @Schema(description = "默认币别Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private Long currencyId;

  /** 是否禁用 */
  @Schema(description = "是否禁用", requiredMode = Schema.RequiredMode.REQUIRED)
  private boolean disabled;

  /** 排序（可空） */
  @Schema(description = "排序")
  @Nullable
  private Integer sort;

  /** 备注（可空） */
  @Schema(description = "备注")
  @Nullable
  private String remark;
}
