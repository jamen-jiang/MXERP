package com.mxerp.system.application.post.query;

import com.mxerp.shared.application.cqrs.Query;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;

/** 按Id查询岗位 */
@NullMarked
@Getter
@Setter
@Schema(description = "按Id查询岗位")
public class GetPostByIdQuery implements Query {

  /** 岗位Id */
  @Schema(description = "岗位Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private Long id;

  /** 无参构造 */
  public GetPostByIdQuery() {}

  /**
   * 有参构造
   *
   * @param id 岗位Id
   */
  public GetPostByIdQuery(long id) {
    this.id = id;
  }
}
