package com.mxerp.system.application.rank.query;

import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 职级查询服务接口 */
@NullMarked
public interface RankQueryService {

  /**
   * 按Id查询
   *
   * @param id 职级Id
   * @return 职级响应（可空），不存在返回 null
   */
  @Nullable RankResponse getById(long id);

  /**
   * 查询所有职级（按排序升序）
   *
   * @return 职级响应列表
   */
  List<RankResponse> getList();
}
