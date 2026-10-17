package com.mxerp.system.application.post.query;

import com.mxerp.shared.application.response.BaseResponse;
import com.mxerp.system.application.role.query.RoleResponse;
import io.swagger.v3.oas.annotations.media.Schema;
import java.util.ArrayList;
import java.util.List;
import lombok.Getter;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 岗位响应 */
@NullMarked
@Getter
@Setter
@Schema(description = "岗位响应")
public class PostResponse extends BaseResponse {

  /** 岗位Id */
  @Schema(description = "岗位Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private Long id;

  /** 岗位名称 */
  @Schema(description = "岗位名称", requiredMode = Schema.RequiredMode.REQUIRED)
  private String name = "";

  /** 公司Id */
  @Schema(description = "公司Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private Long companyId = 0L;

  /** 公司代码 */
  @Schema(description = "公司代码", requiredMode = Schema.RequiredMode.REQUIRED)
  private String companyCode = "";

  /** 公司名称 */
  @Schema(description = "公司名称", requiredMode = Schema.RequiredMode.REQUIRED)
  private String companyName = "";

  /** 部门Id */
  @Schema(description = "部门Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private Long departmentId = 0L;

  /** 部门代码 */
  @Schema(description = "部门代码", requiredMode = Schema.RequiredMode.REQUIRED)
  private String departmentCode = "";

  /** 部门名称 */
  @Schema(description = "部门名称", requiredMode = Schema.RequiredMode.REQUIRED)
  private String departmentName = "";

  /** 职级Id */
  @Schema(description = "职级Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private Long rankId = 0L;

  /** 职级名称 */
  @Schema(description = "职级名称", requiredMode = Schema.RequiredMode.REQUIRED)
  private String rankName = "";

  /** 备注（可空） */
  @Schema(description = "备注")
  @Nullable
  private String remark;

  /** 角色列表 */
  @Schema(description = "角色列表", requiredMode = Schema.RequiredMode.REQUIRED)
  private List<RoleResponse> roles = new ArrayList<>();

  /**
   * 获取角色Id列表
   *
   * @return 角色Id列表
   */
  public List<Long> getRoleIds() {
    return roles.stream().map(role -> role.getId()).toList();
  }
}
