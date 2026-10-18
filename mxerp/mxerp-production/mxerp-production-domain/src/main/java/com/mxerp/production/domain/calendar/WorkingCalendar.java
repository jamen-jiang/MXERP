package com.mxerp.production.domain.calendar;

import com.mxerp.shared.domain.exception.DomainException;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 工作日历
 *
 * <p>排产要知道「哪天干活」，本类把这件事收成一个口子：给定一段时间，告诉你哪些日期不上班。数据只存非工作日，
 * 没有登记的日期就是工作日。
 *
 * <p>只做工厂级日历：全厂共用一套。工作中心各自不同的班次（某条线周日也开）以后再说。
 */
@NullMarked
public class WorkingCalendar {

  /** 非工作日仓储 */
  private final NonWorkingDayRepository nonWorkingDayRepository;

  /**
   * 构造器注入
   *
   * @param nonWorkingDayRepository 非工作日仓储
   */
  public WorkingCalendar(NonWorkingDayRepository nonWorkingDayRepository) {
    this.nonWorkingDayRepository = nonWorkingDayRepository;
  }

  /**
   * 判断某天是否工作日
   *
   * @param date 日期
   * @return true 表示工作日
   */
  public boolean isWorkingDay(LocalDate date) {
    return nonWorkingDayRepository.getByDate(date) == null;
  }

  /**
   * 从某个时点往后找最近的工作日
   *
   * <p>时点本身在工作日就原样返回；在非工作日则推到下一个工作日的同一时刻。
   *
   * @param time 时点
   * @return 不早于入参的工作日时点
   */
  public LocalDateTime nextWorkingMoment(LocalDateTime time) {
    LocalDate date = time.toLocalDate();
    while (!isWorkingDay(date)) {
      date = date.plusDays(1);
    }
    return date.equals(time.toLocalDate()) ? time : date.atTime(time.toLocalTime());
  }

  /**
   * 从某个时点往前找最近的工作日
   *
   * <p>时点本身在工作日就原样返回；在非工作日则退到上一个工作日的同一时刻。
   *
   * @param time 时点
   * @return 不晚于入参的工作日时点
   */
  public LocalDateTime previousWorkingMoment(LocalDateTime time) {
    LocalDate date = time.toLocalDate();
    while (!isWorkingDay(date)) {
      date = date.minusDays(1);
    }
    return date.equals(time.toLocalDate()) ? time : date.atTime(time.toLocalTime());
  }

  /**
   * 取某段时间内的非工作日
   *
   * @param from 起始日期（含）
   * @param to 结束日期（含）
   * @return 非工作日列表
   */
  public List<NonWorkingDay> getNonWorkingDays(LocalDate from, LocalDate to) {
    if (from.isAfter(to)) {
      throw new DomainException("起始日期不能晚于结束日期");
    }
    return nonWorkingDayRepository.getListBetween(from, to);
  }

  /**
   * 登记一个非工作日
   *
   * @param date 日期
   * @param name 名称（可空）
   * @param remark 备注（可空）
   * @return 非工作日聚合
   * @throws DomainException 该日期已登记时抛出
   */
  public NonWorkingDay add(LocalDate date, @Nullable String name, @Nullable String remark) {
    if (nonWorkingDayRepository.getByDate(date) != null) {
      throw new DomainException("日期" + date + "已经登记为非工作日");
    }
    return new NonWorkingDay(date, name, remark);
  }
}
