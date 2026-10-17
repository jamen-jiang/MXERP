package com.mxerp.system.application.company.payload;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 公司联系人入参 */
@NullMarked
@Getter
@Setter
@Schema(description = "公司联系人入参")
public class CompanyContactPayload {

  /** 联系人Id（修改时必填，新增时留空） */
  @Schema(description = "联系人Id")
  @Nullable
  private Long id;

  /** 姓名 */
  @Schema(description = "姓名", requiredMode = Schema.RequiredMode.REQUIRED)
  private String name;

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

  /** 无参构造 */
  public CompanyContactPayload() {}

  /**
   * 全参构造
   *
   * @param id 联系人Id（可空）
   * @param name 姓名
   * @param telephone 电话（可空）
   * @param area 地区（可空）
   * @param address 地址（可空）
   */
  public CompanyContactPayload(
      @Nullable Long id,
      String name,
      @Nullable String telephone,
      @Nullable String area,
      @Nullable String address) {
    this.id = id;
    this.name = name;
    this.telephone = telephone;
    this.area = area;
    this.address = address;
  }
}
