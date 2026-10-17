package com.mxerp.system.application.app.command;

import com.mxerp.shared.application.cqrs.Command;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 删除应用命令 */
@NullMarked
@Getter
@Schema(description = "删除应用命令")
public class RemoveAppCommand extends Command {

  /** 应用Id */
  @Schema(description = "应用Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private final Long id;

  /**
   * 构造器
   *
   * @param id 应用Id
   */
  public RemoveAppCommand(Long id) {
    this.id = id;
  }
}
