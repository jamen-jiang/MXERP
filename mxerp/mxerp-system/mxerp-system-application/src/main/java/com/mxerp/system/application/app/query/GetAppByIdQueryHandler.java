package com.mxerp.system.application.app.query;

import org.axonframework.messaging.queryhandling.annotation.QueryHandler;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.springframework.stereotype.Component;

/** 按Id查询应用处理器 */
@NullMarked
@Component
public class GetAppByIdQueryHandler {

  /** 查询服务 */
  private final AppQueryService appQueryService;

  /**
   * 构造器注入
   *
   * @param appQueryService 查询服务
   */
  public GetAppByIdQueryHandler(AppQueryService appQueryService) {
    this.appQueryService = appQueryService;
  }

  /**
   * 处理查询
   *
   * @param query 查询
   * @return 应用响应
   */
  @QueryHandler
  public @Nullable AppResponse handle(GetAppByIdQuery query) {
    return appQueryService.getById(query.getId());
  }
}
