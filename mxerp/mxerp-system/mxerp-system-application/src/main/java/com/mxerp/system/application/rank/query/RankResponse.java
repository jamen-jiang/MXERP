package com.mxerp.system.application.rank.query;

import com.mxerp.shared.application.response.BaseResponse;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 职级响应 */
@NullMarked
@Getter
@Setter
@Schema(description = "职级响应")
public class RankResponse extends BaseResponse {

  /** 职级Id */
  @Schema(description = "职级Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private Long id;

  /** 职级级别字符串（可空） */
  @Schema(description = "职级级别字符串")
  @Nullable
  private String level;

  /** 父Id（可空） */
  @Schema(description = "父Id")
  @Nullable
  private Long parentId;

  /** 职级名称（可空） */
  @Schema(description = "职级名称")
  @Nullable
  private String name;

  /** 排序（可空） */
  @Schema(description = "排序")
  @Nullable
  private Integer sort;

  /** 备注（可空） */
  @Schema(description = "备注")
  @Nullable
  private String remark;
}
