package com.mxerp.system.application.rank.query;

import java.util.List;
import org.axonframework.messaging.queryhandling.annotation.QueryHandler;
import org.jspecify.annotations.NullMarked;
import org.springframework.stereotype.Component;

/** 查询所有职级列表查询处理器 */
@NullMarked
@Component
public class GetRanksQueryHandler {

  /** 职级查询服务 */
  private final RankQueryService rankQueryService;

  /**
   * 构造器注入
   *
   * @param rankQueryService 职级查询服务
   */
  public GetRanksQueryHandler(RankQueryService rankQueryService) {
    this.rankQueryService = rankQueryService;
  }

  /**
   * 处理查询
   *
   * @param query 查询
   * @return 职级响应列表
   */
  @QueryHandler
  public List<RankResponse> handle(GetRanksQuery query) {
    return rankQueryService.getList();
  }
}
