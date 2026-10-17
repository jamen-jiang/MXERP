package com.mxerp.system.application.user.command;

import com.mxerp.shared.application.cqrs.Command;
import io.swagger.v3.oas.annotations.media.Schema;
import java.util.List;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 添加用户命令 */
@NullMarked
@Getter
@Schema(description = "添加用户命令")
public class AddUserCommand extends Command {

  /** 用户名称 */
  @Schema(description = "用户名称", requiredMode = Schema.RequiredMode.REQUIRED)
  private final String name;

  /** 账号 */
  @Schema(description = "账号", requiredMode = Schema.RequiredMode.REQUIRED)
  private final String userName;

  /** 邮箱（可空） */
  @Schema(description = "邮箱")
  @Nullable
  private final String email;

  /** 密码 */
  @Schema(description = "密码", requiredMode = Schema.RequiredMode.REQUIRED)
  private final String password;

  /** 备注（可空） */
  @Schema(description = "备注")
  @Nullable
  private final String remark;

  /** 岗位Id列表 */
  @Schema(description = "岗位Id列表", requiredMode = Schema.RequiredMode.REQUIRED)
  private final List<Long> postIds;

  /** 角色Id列表（可空） */
  @Schema(description = "角色Id列表")
  @Nullable
  private final List<Long> roleIds;

  /**
   * 构造器
   *
   * @param name 用户名称
   * @param userName 账号
   * @param email 邮箱（可空）
   * @param password 密码
   * @param remark 备注（可空）
   * @param postIds 岗位Id列表
   * @param roleIds 角色Id列表（可空）
   */
  public AddUserCommand(
      String name,
      String userName,
      @Nullable String email,
      String password,
      @Nullable String remark,
      List<Long> postIds,
      @Nullable List<Long> roleIds) {
    this.name = name;
    this.userName = userName;
    this.email = email;
    this.password = password;
    this.remark = remark;
    this.postIds = postIds;
    this.roleIds = roleIds;
  }
}
