package com.mxerp.system.application.app.query;

import java.util.Collection;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 应用查询服务接口 */
@NullMarked
public interface AppQueryService {

  /**
   * 按Id查询
   *
   * @param id 应用Id
   * @return 应用响应
   */
  @Nullable AppResponse getById(long id);

  /**
   * 按Id列表查询
   *
   * @param ids 应用Id列表（可空）
   * @return 应用响应列表
   */
  List<AppResponse> getListByIds(Collection<Long> ids);

  /**
   * 查询所有
   *
   * @return 应用响应列表
   */
  List<AppResponse> getList();
}
