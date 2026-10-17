package com.mxerp.system.application.department.query;

import com.mxerp.shared.application.response.BaseResponse;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 部门响应 */
@NullMarked
@Getter
@Setter
@Schema(description = "部门响应")
public class DepartmentResponse extends BaseResponse {

  /** 部门Id */
  @Schema(description = "部门Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private Long id;

  /** 公司Id */
  @Schema(description = "公司Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private Long companyId;

  /** 公司代码 */
  @Schema(description = "公司代码", requiredMode = Schema.RequiredMode.REQUIRED)
  private String companyCode = "";

  /** 公司名称 */
  @Schema(description = "公司名称", requiredMode = Schema.RequiredMode.REQUIRED)
  private String companyName = "";

  /** 部门级别字符串（可空） */
  @Schema(description = "部门级别字符串")
  @Nullable
  private String level;

  /** 父Id（可空） */
  @Schema(description = "父Id")
  @Nullable
  private Long parentId;

  /** 父级部门（可空） */
  @Schema(description = "父级部门代码")
  @Nullable
  private String parentCode;

  /** 父级部门（可空） */
  @Schema(description = "父级部门名称")
  @Nullable
  private String parentName;

  /** 部门代码 */
  @Schema(description = "部门代码", requiredMode = Schema.RequiredMode.REQUIRED)
  private String code = "";

  /** 部门名称 */
  @Schema(description = "部门名称", requiredMode = Schema.RequiredMode.REQUIRED)
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

  /** 排序（可空） */
  @Schema(description = "排序")
  @Nullable
  private Integer sort;

  /** 地址（可空） */
  @Schema(description = "地址")
  @Nullable
  private String address;

  /** 备注（可空） */
  @Schema(description = "备注")
  @Nullable
  private String remark;
}
