package com.mxerp.system.application.rank.query;

import com.mxerp.shared.application.cqrs.Query;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;

/** 按Id查询职级 */
@NullMarked
@Getter
@Setter
@Schema(description = "按Id查询职级")
public class GetRankByIdQuery implements Query {

  /** 职级Id */
  @Schema(description = "职级Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private Long id;

  /** 无参构造 */
  public GetRankByIdQuery() {}

  /**
   * 有参构造
   *
   * @param id 职级Id
   */
  public GetRankByIdQuery(Long id) {
    this.id = id;
  }
}
