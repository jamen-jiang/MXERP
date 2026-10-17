package com.mxerp.system.application.rank.query;

import com.mxerp.shared.application.exception.ApiException;
import org.axonframework.messaging.queryhandling.annotation.QueryHandler;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.springframework.stereotype.Component;

/** 按Id查询职级查询处理器 */
@NullMarked
@Component
public class GetRankByIdQueryHandler {

  /** 职级查询服务 */
  private final RankQueryService rankQueryService;

  /**
   * 构造器注入
   *
   * @param rankQueryService 职级查询服务
   */
  public GetRankByIdQueryHandler(RankQueryService rankQueryService) {
    this.rankQueryService = rankQueryService;
  }

  /**
   * 处理查询
   *
   * @param query 查询
   * @return 职级响应
   */
  @QueryHandler
  public @Nullable RankResponse handle(GetRankByIdQuery query) {
    RankResponse response = rankQueryService.getById(query.getId());
    if (response == null) {
      throw new ApiException("职级不存在!");
    }
    return response;
  }
}
