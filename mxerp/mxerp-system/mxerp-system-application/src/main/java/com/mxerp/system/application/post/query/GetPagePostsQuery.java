package com.mxerp.system.application.post.query;

import com.mxerp.shared.application.request.PageRequest;
import com.mxerp.shared.application.cqrs.Query;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 分页查询岗位列表 */
@NullMarked
@Getter
@Setter
@Schema(description = "分页查询岗位列表")
public class GetPagePostsQuery extends PageRequest implements Query {

  /** 公司Id（可空） */
  @Schema(description = "公司Id")
  @Nullable
  private Long companyId;

  /** 岗位名称（可空） */
  @Schema(description = "岗位名称")
  @Nullable
  private String name;

  /** 组织机构Id（可空） */
  @Schema(description = "组织机构Id")
  @Nullable
  private Long departmentId;

  /** 职级Id（可空） */
  @Schema(description = "职级Id")
  @Nullable
  private Long rankId;
}
