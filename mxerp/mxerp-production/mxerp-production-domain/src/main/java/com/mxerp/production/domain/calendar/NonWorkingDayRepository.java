package com.mxerp.production.domain.calendar;

import com.mxerp.shared.domain.repository.Repository;
import java.time.LocalDate;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 非工作日仓储接口 */
@NullMarked
public interface NonWorkingDayRepository extends Repository<NonWorkingDay> {

  /**
   * 取某段时间内的非工作日
   *
   * @param from 起始日期（含）
   * @param to 结束日期（含）
   * @return 非工作日列表
   */
  List<NonWorkingDay> getListBetween(LocalDate from, LocalDate to);

  /**
   * 按日期查询
   *
   * @param date 日期
   * @return 非工作日（可空），该日期是工作日时返回 null
   */
  @Nullable NonWorkingDay getByDate(LocalDate date);
}
