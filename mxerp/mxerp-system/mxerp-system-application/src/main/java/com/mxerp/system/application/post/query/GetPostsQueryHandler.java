package com.mxerp.system.application.post.query;

import java.util.List;
import org.axonframework.messaging.queryhandling.annotation.QueryHandler;
import org.jspecify.annotations.NullMarked;
import org.springframework.stereotype.Component;

/** 查询所有岗位列表查询处理器 */
@NullMarked
@Component
public class GetPostsQueryHandler {

  /** 岗位查询服务 */
  private final PostQueryService postQueryService;

  /**
   * 构造器注入
   *
   * @param postQueryService 岗位查询服务
   */
  public GetPostsQueryHandler(PostQueryService postQueryService) {
    this.postQueryService = postQueryService;
  }

  /**
   * 处理查询
   *
   * @param query 查询
   * @return 岗位响应列表
   */
  @QueryHandler
  public List<PostResponse> handle(GetPostsQuery query) {
    return postQueryService.getList();
  }
}
