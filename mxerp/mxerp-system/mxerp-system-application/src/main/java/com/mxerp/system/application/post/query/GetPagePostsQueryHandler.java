package com.mxerp.system.application.post.query;

import com.mxerp.shared.application.response.PageResponse;
import org.axonframework.messaging.queryhandling.annotation.QueryHandler;
import org.jspecify.annotations.NullMarked;
import org.springframework.stereotype.Component;

/** 分页查询岗位列表查询处理器 */
@NullMarked
@Component
public class GetPagePostsQueryHandler {

  /** 岗位查询服务 */
  private final PostQueryService postQueryService;

  /**
   * 构造器注入
   *
   * @param postQueryService 岗位查询服务
   */
  public GetPagePostsQueryHandler(PostQueryService postQueryService) {
    this.postQueryService = postQueryService;
  }

  /**
   * 处理查询
   *
   * @param query 查询
   * @return 分页响应
   */
  @QueryHandler
  public PageResponse<PostResponse> handle(GetPagePostsQuery query) {
    return postQueryService.getPage(query);
  }
}
