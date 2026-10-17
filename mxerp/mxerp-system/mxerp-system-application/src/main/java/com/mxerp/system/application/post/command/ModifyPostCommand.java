package com.mxerp.system.application.post.command;

import io.swagger.v3.oas.annotations.media.Schema;
import java.util.List;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 修改岗位命令 */
@NullMarked
@Getter
@Schema(description = "修改岗位命令")
public class ModifyPostCommand extends AddPostCommand {

  /** 岗位Id */
  @Schema(description = "岗位Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private final Long id;

  /**
   * 构造器
   *
   * @param id 岗位Id
   * @param name 岗位名称
   * @param departmentId 组织机构Id
   * @param rankId 职级Id
   * @param remark 备注（可空）
   * @param roleIds 角色Id列表（可空）
   */
  public ModifyPostCommand(
      Long id,
      String name,
      Long departmentId,
      Long rankId,
      @Nullable String remark,
      @Nullable List<Long> roleIds) {
    super(name, departmentId, rankId, remark, roleIds);
    this.id = id;
  }
}
