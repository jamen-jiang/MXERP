package com.mxerp.system.application.rank.command;

import com.mxerp.shared.application.cqrs.Command;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 添加职级命令 */
@NullMarked
@Getter
@Schema(description = "添加职级命令")
public class AddRankCommand extends Command {

  /** 父Id（可空） */
  @Schema(description = "父Id")
  @Nullable
  private final Long parentId;

  /** 职级名称 */
  @Schema(description = "职级名称", requiredMode = Schema.RequiredMode.REQUIRED)
  private final String name;

  /** 排序（可空） */
  @Schema(description = "排序")
  @Nullable
  private final Integer sort;

  /** 备注（可空） */
  @Schema(description = "备注")
  @Nullable
  private final String remark;

  /**
   * 构造器
   *
   * @param parentId 父Id（可空）
   * @param name 职级名称
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  public AddRankCommand(
      @Nullable Long parentId, String name, @Nullable Integer sort, @Nullable String remark) {
    this.parentId = parentId;
    this.name = name;
    this.sort = sort;
    this.remark = remark;
  }
}
