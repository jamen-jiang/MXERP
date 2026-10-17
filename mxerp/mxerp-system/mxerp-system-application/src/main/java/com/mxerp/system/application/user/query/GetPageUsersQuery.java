package com.mxerp.system.application.user.query;

import com.mxerp.shared.application.request.PageRequestWithUnique;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 分页获取用户列表 */
@NullMarked
@Getter
@Setter
@Schema(description = "分页获取用户列表")
public class GetPageUsersQuery extends PageRequestWithUnique<Long> {

  /** 公司Id（可空） */
  @Schema(description = "公司Id")
  @Nullable
  private Long companyId;

  /** 部门Id（可空） */
  @Schema(description = "部门Id")
  @Nullable
  private Long departmentId;

  /** 岗位Id（可空） */
  @Schema(description = "岗位Id")
  @Nullable
  private Long postId;

  /** 职级Id（可空） */
  @Schema(description = "职级Id")
  @Nullable
  private Long rankId;

  /** 用户名称（可空） */
  @Schema(description = "用户名称")
  @Nullable
  private String name;

  /** 账号（可空） */
  @Schema(description = "账号")
  @Nullable
  private String userName;

  /** 邮箱（可空） */
  @Schema(description = "邮箱")
  @Nullable
  private String email;
}
