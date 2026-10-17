package com.mxerp.production.domain.routing;

import com.mxerp.shared.domain.repository.Repository;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 工艺路线仓储接口 */
@NullMarked
public interface RoutingRepository extends Repository<Routing> {

  /**
   * 按代码 + 版本查询
   *
   * @param code 工艺路线代码
   * @param version 版本号
   * @return 工艺路线（可空），不存在返回 null
   */
  @Nullable Routing getByCodeAndVersion(String code, String version);

  /**
   * 按代码查询全部版本
   *
   * @param code 工艺路线代码
   * @return 工艺路线列表
   */
  List<Routing> getListByCode(String code);

  /**
   * 判断代码 + 版本是否存在
   *
   * @param code 工艺路线代码
   * @param version 版本号
   * @param id 排除的工艺路线Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistByCodeAndVersionExcludeId(String code, String version, @Nullable Long id);

  /**
   * 判断代码 + 版本是否存在
   *
   * @param code 工艺路线代码
   * @param version 版本号
   * @return true 表示已存在
   */
  default boolean isExistByCodeAndVersionExcludeId(String code, String version) {
    return isExistByCodeAndVersionExcludeId(code, version, null);
  }
}
