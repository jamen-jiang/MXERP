package com.mxerp.system.application.app.query;

import java.util.List;
import org.axonframework.messaging.queryhandling.annotation.QueryHandler;
import org.jspecify.annotations.NullMarked;
import org.springframework.stereotype.Component;

/** 查询所有应用处理器 */
@NullMarked
@Component
public class GetAppsQueryHandler {

  /** 查询服务 */
  private final AppQueryService appQueryService;

  /**
   * 构造器注入
   *
   * @param appQueryService 查询服务
   */
  public GetAppsQueryHandler(AppQueryService appQueryService) {
    this.appQueryService = appQueryService;
  }

  /**
   * 处理查询
   *
   * @param query 查询
   * @return 应用响应列表
   */
  @QueryHandler
  public List<AppResponse> handle(GetAppsQuery query) {
    return appQueryService.getList();
  }
}
