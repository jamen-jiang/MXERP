package com.mxerp.system.application.app.query;

import com.mxerp.shared.application.cqrs.Query;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;

/** 按Id查询应用 */
@NullMarked
@Getter
@Setter
@Schema(description = "按Id查询应用")
public class GetAppByIdQuery implements Query {

  /** 应用Id */
  @Schema(description = "应用Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private Long id;

  /** 无参构造：供 Jackson 反序列化用 */
  public GetAppByIdQuery() {}

  /**
   * 有参构造
   *
   * @param id 应用Id
   */
  public GetAppByIdQuery(Long id) {
    this.id = id;
  }
}
