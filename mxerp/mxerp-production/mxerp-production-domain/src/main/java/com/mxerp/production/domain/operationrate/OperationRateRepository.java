package com.mxerp.production.domain.operationrate;

import com.mxerp.shared.domain.repository.Repository;
import java.time.LocalDateTime;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 工序费率卡仓储接口 */
@NullMarked
public interface OperationRateRepository extends Repository<OperationRate> {

  /**
   * 取某代码当前生效的那张费率卡
   *
   * @param code 费率卡代码
   * @return 费率卡（可空），没有当前生效版本返回 null
   */
  @Nullable OperationRate getCurrentByCode(String code);

  /**
   * 取某代码在指定时刻生效的那张费率卡
   *
   * @param code 费率卡代码
   * @param moment 时刻
   * @return 费率卡（可空），区间内没有卡返回 null
   */
  @Nullable OperationRate getEffectiveByCodeAt(String code, LocalDateTime moment);

  /**
   * 按代码查询全部费率卡（含历史，按生效时间倒序）
   *
   * @param code 费率卡代码
   * @return 费率卡列表
   */
  List<OperationRate> getListByCode(String code);

  /**
   * 判断代码 + 生效时间是否已被占用
   *
   * @param code 费率卡代码
   * @param effectiveFrom 生效时间
   * @param id 排除的费率卡Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistByCodeAndEffectiveFromExcludeId(
      String code, LocalDateTime effectiveFrom, @Nullable Long id);

  /**
   * 判断代码 + 生效时间是否已被占用
   *
   * @param code 费率卡代码
   * @param effectiveFrom 生效时间
   * @return true 表示已存在
   */
  default boolean isExistByCodeAndEffectiveFromExcludeId(String code, LocalDateTime effectiveFrom) {
    return isExistByCodeAndEffectiveFromExcludeId(code, effectiveFrom, null);
  }
}
