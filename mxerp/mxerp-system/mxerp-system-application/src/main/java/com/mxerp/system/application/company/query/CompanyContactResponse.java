package com.mxerp.system.application.company.query;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 公司联系人响应 */
@NullMarked
@Getter
@Setter
@Schema(description = "公司联系人响应")
public class CompanyContactResponse {

  /** 联系人Id */
  @Schema(description = "联系人Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private Long id;

  /** 公司Id */
  @Schema(description = "公司Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private Long companyId;

  /** 姓名 */
  @Schema(description = "姓名", requiredMode = Schema.RequiredMode.REQUIRED)
  private String name = "";

  /** 电话（可空） */
  @Schema(description = "电话")
  @Nullable
  private String telephone;

  /** 地区（可空） */
  @Schema(description = "地区")
  @Nullable
  private String area;

  /** 地址（可空） */
  @Schema(description = "地址")
  @Nullable
  private String address;
}
