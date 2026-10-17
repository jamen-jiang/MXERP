package com.mxerp.system.application.app.query;

import io.swagger.v3.oas.annotations.media.Schema;
import java.util.List;
import lombok.Getter;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 当前用户权限响应 */
@NullMarked
@Getter
@Setter
@Schema(description = "当前用户权限响应")
public class CurrentUserPermissionResponse {

  /** 应用列表（可空） */
  @Schema(description = "应用列表")
  @Nullable
  private List<AppResponse> apps;

  /** 权限列表（可空） */
  @Schema(description = "权限列表")
  @Nullable
  private List<String> licenses;
}
