package com.mxerp.system.application.post.command;

import com.mxerp.shared.application.cqrs.Command;
import io.swagger.v3.oas.annotations.media.Schema;
import java.util.List;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 添加岗位命令 */
@NullMarked
@Getter
@Schema(description = "添加岗位命令")
public class AddPostCommand extends Command {

  /** 岗位名称 */
  @Schema(description = "岗位名称", requiredMode = Schema.RequiredMode.REQUIRED)
  private final String name;

  /** 组织机构Id */
  @Schema(description = "组织机构Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private final Long departmentId;

  /** 职级Id */
  @Schema(description = "职级Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private final Long rankId;

  /** 备注（可空） */
  @Schema(description = "备注")
  @Nullable
  private final String remark;

  /** 角色Id列表（可空） */
  @Schema(description = "角色Id列表")
  @Nullable
  private final List<Long> roleIds;

  /**
   * 构造器
   *
   * @param name 岗位名称
   * @param departmentId 组织机构Id
   * @param rankId 职级Id
   * @param remark 备注（可空）
   * @param roleIds 角色Id列表（可空）
   */
  public AddPostCommand(
      String name,
      Long departmentId,
      Long rankId,
      @Nullable String remark,
      @Nullable List<Long> roleIds) {
    this.name = name;
    this.departmentId = departmentId;
    this.rankId = rankId;
    this.remark = remark;
    this.roleIds = roleIds;
  }
}
