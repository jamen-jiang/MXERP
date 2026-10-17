package com.mxerp.system.application.rank.command;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 修改职级命令 */
@NullMarked
@Getter
@Schema(description = "修改职级命令")
public class ModifyRankCommand extends AddRankCommand {

  /** 职级Id */
  @Schema(description = "职级Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private final Long id;

  /**
   * 构造器
   *
   * @param id 职级Id
   * @param parentId 父Id（可空）
   * @param name 职级名称
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  public ModifyRankCommand(
      Long id,
      @Nullable Long parentId,
      String name,
      @Nullable Integer sort,
      @Nullable String remark) {
    super(parentId, name, sort, remark);
    this.id = id;
  }
}
