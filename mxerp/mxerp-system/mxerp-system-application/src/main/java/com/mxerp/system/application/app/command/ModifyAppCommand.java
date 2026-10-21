package com.mxerp.system.application.app.command;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 修改应用命令 */
@NullMarked
@Getter
@Schema(description = "修改应用命令")
public class ModifyAppCommand extends AddAppCommand {

  /** 应用Id */
  @Schema(description = "应用Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private final Long id;

  /**
   * 构造器
   *
   * @param id 应用Id
   * @param type 类型
   * @param parentId 父Id（可空）
   * @param name 名称（可空）
   * @param path 路径（可空）
   * @param component 组件路径（可空）
   * @param redirect 路由重定向（可空）
   * @param license 许可证（可空）
   * @param remark 备注（可空）
   * @param meta 路由元数据
   */
  public ModifyAppCommand(
      long id,
      String type,
      @Nullable Long parentId,
      @Nullable String name,
      @Nullable String path,
      @Nullable String component,
      @Nullable String redirect,
      @Nullable String license,
      @Nullable String remark,
      MetaPayload meta) {
    super(type, parentId, name, path, component, redirect, license, remark, meta);
    this.id = id;
  }
}
