package com.mxerp.system.application.company.query;

import com.mxerp.shared.application.response.BaseResponse;
import io.swagger.v3.oas.annotations.media.Schema;
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

  /** 区域 */
  @Schema(description = "区域", requiredMode = Schema.RequiredMode.REQUIRED)
  private String region = "";

  /** 组别（可空） */
  @Schema(description = "组别")
  @Nullable
  private String group;

  /** 公司代码 */
  @Schema(description = "公司代码", requiredMode = Schema.RequiredMode.REQUIRED)
  private String code = "";

  /** 公司名称 */
  @Schema(description = "公司名称", requiredMode = Schema.RequiredMode.REQUIRED)
  private String name = "";

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

  /** 地区代码（可空） */
  @Schema(description = "地区代码")
  @Nullable
  private String areaCode;

  /** 邮政编码（可空） */
  @Schema(description = "邮政编码")
  @Nullable
  private String postCode;

  /** 币种Id */
  @Schema(description = "币种Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private Long currencyId;

  /** 加成 */
  @Schema(description = "加成", requiredMode = Schema.RequiredMode.REQUIRED)
  private Double markUp;

  /** 地址（可空） */
  @Schema(description = "地址")
  @Nullable
  private String address;

  /** 账户地址（可空） */
  @Schema(description = "账户地址")
  @Nullable
  private String billingAddress;

  /** 排序（可空） */
  @Schema(description = "排序")
  @Nullable
  private Integer sort;

  /** 备注（可空） */
  @Schema(description = "备注")
  @Nullable
  private String remark;
}
