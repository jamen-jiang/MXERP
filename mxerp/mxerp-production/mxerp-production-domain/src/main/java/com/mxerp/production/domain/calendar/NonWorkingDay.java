package com.mxerp.production.domain.calendar;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import java.time.LocalDate;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 非工作日
 *
 * <p>排产只关心「哪天不干活」，所以这里只记例外，不记工作日：没有登记的日期一律当工作日。这样维护成本最低——
 * 一年只要录春节、国庆那几天；而「工作日历」那种把每天上下班时段都排进去的做法，只有在班次随时段变化时才值得。
 *
 * <p>日期是全厂共用的。工作中心级的日历（某条线周日也开）以后要的话，在这里加一个可空的工作中心Id 即可，
 * 现在不加：没有数据支撑的字段只会被空着。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "NonWorkingDay",
    indexes = {@Index(name = "IX_NonWorkingDay_Date", columnList = "Date")})
public class NonWorkingDay extends UpdateAggregateRoot {

  /** 日期 */
  @Column(name = "Date", nullable = false)
  private LocalDate date;

  /** 名称（可空，例如「春节」「国庆」） */
  @Nationalized
  @Column(name = "Name", length = 50)
  @Nullable
  private String name;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param date 日期
   * @param name 名称（可空）
   * @param remark 备注（可空）
   * @throws DomainException 日期为空时抛出
   */
  NonWorkingDay(LocalDate date, @Nullable String name, @Nullable String remark) {
    if (date == null) {
      throw new DomainException("非工作日日期不能为空");
    }
    this.date = date;
    this.name = StringUtils.trimToNull(name);
    this.remark = remark;
  }
}
