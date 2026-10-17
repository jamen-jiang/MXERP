package com.mxerp.system.application.department.command;

import com.mxerp.shared.application.cqrs.Command;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 删除部门命令 */
@NullMarked
@Getter
@Schema(description = "删除部门命令")
public class RemoveDepartmentCommand extends Command {

  /** 部门Id */
  @Schema(description = "部门Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private final Long id;

  /**
   * 构造器
   *
   * @param id 部门Id
   */
  public RemoveDepartmentCommand(Long id) {
    this.id = id;
  }
}
