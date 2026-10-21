package com.mxerp.system.application.rank.command;

import com.mxerp.shared.application.cqrs.Command;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 删除职级命令 */
@NullMarked
@Getter
@Schema(description = "删除职级命令")
public class RemoveRankCommand extends Command {

  /** 职级Id */
  @Schema(description = "职级Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private final Long id;

  /**
   * 构造器
   *
   * @param id 职级Id
   */
  public RemoveRankCommand(long id) {
    this.id = id;
  }
}
