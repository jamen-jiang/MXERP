package com.mxerp.system.application.user.command;

import io.swagger.v3.oas.annotations.media.Schema;
import java.util.List;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 修改用户命令 */
@NullMarked
@Getter
@Schema(description = "修改用户命令")
public class ModifyUserCommand extends AddUserCommand {

  /** 用户Id */
  @Schema(description = "用户Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private final Long id;

  /**
   * 构造器
   *
   * @param id 用户Id
   * @param name 用户名称
   * @param userName 账号
   * @param email 邮箱（可空）
   * @param password 密码
   * @param remark 备注（可空）
   * @param postIds 岗位Id列表
   * @param roleIds 角色Id列表（可空）
   */
  public ModifyUserCommand(
      Long id,
      String name,
      String userName,
      @Nullable String email,
      String password,
      @Nullable String remark,
      List<Long> postIds,
      @Nullable List<Long> roleIds) {
    super(name, userName, email, password, remark, postIds, roleIds);
    this.id = id;
  }
}
