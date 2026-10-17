package com.mxerp.system.application.post.query;

import com.mxerp.shared.application.response.PageResponse;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 岗位查询服务接口 */
@NullMarked
public interface PostQueryService {

  /**
   * 按Id查询
   *
   * @param id 岗位Id
   * @return 岗位响应（可空），不存在返回 null
   */
  @Nullable PostResponse getById(long id);

  /**
   * 查询所有岗位
   *
   * @return 岗位响应列表
   */
  List<PostResponse> getList();

  /**
   * 分页查询岗位
   *
   * @param query 分页查询
   * @return 分页响应
   */
  PageResponse<PostResponse> getPage(GetPagePostsQuery query);
}
