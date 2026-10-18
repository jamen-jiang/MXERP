package com.mxerp.production.domain.workorder;

import com.mxerp.production.domain.calendar.WorkingCalendar;
import com.mxerp.production.domain.workcenter.WorkCenter;
import com.mxerp.production.domain.workcenter.WorkCenterRepository;
import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.math.RoundingMode;
import java.time.Duration;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import org.jspecify.annotations.NullMarked;

/**
 * 工单排产服务：把工序按工作中心的产能排到具体时段
 *
 * <p>方向按 SAP / D365 的默认做法：从工单的计划完工时间往前倒排；倒排推到「现在」之前就说明交期做不到，
 * 改从「现在」正排，算出一个更晚的完工时间。倒排算的是「最晚什么时候开工还来得及」，正排算的是
 * 「最早什么时候能做完」——同一套工序时长，两个方向各用一次。
 *
 * <p>产能按工作中心每天可用分钟数算，不排班次：每天从 {@link #WORKDAY_START} 开始，干满
 * {@code capacityMinutesPerDay} 分钟就收工。哪天不上班由 {@link WorkingCalendar} 说了算，非工作日整天跳过。
 *
 * <p>工作中心一次只干一道工序：同一台设备上的两道工序不并行，后面的顺延到前一道做完。这与 BC 的
 * 「有限产能」一致，也是没有 APS 时的标准做法。别的工单已经排好的时段也会绕开。
 *
 * <p>排产只算日期、不动状态：草稿也能排，排完仍然是草稿；已下达的工单排完也仍是已下达。
 */
@NullMarked
public class WorkOrderSchedulingDomainService {

  /** 每天开工时间（暂时固定，等有了班次主数据再变成参数） */
  private static final LocalTime WORKDAY_START = LocalTime.of(8, 0);

  /** 回看多少天内的占用（排产通常只关心近期，取太长会把无关的旧计划也读出来） */
  private static final int RESERVATION_LOOKBACK_DAYS = 30;

  /** 工单仓储 */
  private final WorkOrderRepository workOrderRepository;

  /** 工作中心仓储 */
  private final WorkCenterRepository workCenterRepository;

  /** 工作日历 */
  private final WorkingCalendar workingCalendar;

  /**
   * 构造器注入
   *
   * @param workOrderRepository 工单仓储
   * @param workCenterRepository 工作中心仓储
   * @param workingCalendar 工作日历
   */
  public WorkOrderSchedulingDomainService(
      WorkOrderRepository workOrderRepository,
      WorkCenterRepository workCenterRepository,
      WorkingCalendar workingCalendar) {
    this.workOrderRepository = workOrderRepository;
    this.workCenterRepository = workCenterRepository;
    this.workingCalendar = workingCalendar;
  }

  /**
   * 排一张工单：按计划完工时间倒排，倒不进未来就改从「现在」正排
   *
   * @param id 工单Id
   * @param now 当前时间（调用方传入，便于测试与补排）
   * @return 排产后的工单聚合
   * @throws DomainException 工单不存在、没有工序、计划完工时间为空、或工作中心日产能未维护时抛出
   */
  public WorkOrder schedule(long id, LocalDateTime now) {
    WorkOrder model = getExist(id);
    List<WorkCenterReservation> reservations =
        workOrderRepository.getReservations(
            now.minusDays(RESERVATION_LOOKBACK_DAYS), futureHorizon(now), model.getId());
    scheduleOne(model, now, reservations);
    return model;
  }

  /**
   * 批量排产：按优先级依次排，后排的会绕开先排的
   *
   * <p>排序口径：优先级小的先排；优先级相同按计划完工时间早的先排；都相同按 Id 先后。先排的单先占设备，
   * 后排在剩下空档里找位置——这就是「有限产能」在批量上的含义。
   *
   * <p>已经在库里排过的其他工单照样会被绕开（{@link WorkOrderRepository#getReservations}），
   * 本次批量里刚排完的也会立即被后面的单看见。
   *
   * @param ids 要排的工单Id
   * @param now 当前时间
   * @return 排好序并完成排产的工单列表
   * @throws DomainException 某张工单不存在、没有工序、或计划完工时间为空时抛出
   */
  public List<WorkOrder> scheduleBatch(List<Long> ids, LocalDateTime now) {
    if (ids.isEmpty()) {
      return List.of();
    }
    List<WorkOrder> models = new ArrayList<>(ids.size());
    for (Long id : ids) {
      models.add(getExist(id));
    }
    models.sort(
        Comparator.comparing(WorkOrder::getSchedulingPriority)
            .thenComparing(
                WorkOrder::getPlannedEndTime,
                Comparator.nullsLast(Comparator.naturalOrder()))
            .thenComparing(WorkOrder::getId));
    List<WorkCenterReservation> reservations =
        new ArrayList<>(
            workOrderRepository.getReservations(
                now.minusDays(RESERVATION_LOOKBACK_DAYS), futureHorizon(now), null));
    for (WorkOrder model : models) {
      reservations.removeIf(r -> r.workOrderId().equals(model.getId()));
      scheduleOne(model, now, reservations);
      reservations.addAll(reservationsOf(model));
    }
    return models;
  }

  // ==================== 私有：单张排产 ====================

  /**
   * 排一张工单并把结果写回聚合
   *
   * @param model 工单
   * @param now 当前时间
   * @param reservations 别的工单已占用的设备时段
   * @throws DomainException 没有工序、没有计划完工时间、或工作中心不可用时抛出
   */
  private void scheduleOne(
      WorkOrder model, LocalDateTime now, List<WorkCenterReservation> reservations) {
    List<WorkOrderOperation> operations = orderedOperations(model);
    if (operations.isEmpty()) {
      throw new DomainException("工单" + model.getNumber() + "没有工序，不能排产");
    }
    LocalDateTime deadline = model.getPlannedEndTime();
    if (deadline == null) {
      throw new DomainException("工单" + model.getNumber() + "没有计划完工时间，无法倒排");
    }

    Map<Long, BigDecimal> capacityByWorkCenter = loadCapacity(operations);
    List<WorkOrder.OperationPlan> backward =
        placeBackward(
            operations, capacityByWorkCenter, reservations, model.getQuantity(), deadline);
    if (backward.get(0).startTime().isBefore(now)) {
      // 倒排推到了过去，说明按交期做不完，改成正排给出最早能完工的日期
      model.applySchedule(
          placeForward(
              operations, capacityByWorkCenter, reservations, model.getQuantity(), now));
    } else {
      model.applySchedule(backward);
    }
  }

  /**
   * 把一张工单的工序计划转成占用时段
   *
   * @param model 已排产的工单
   * @return 占用时段列表
   */
  private List<WorkCenterReservation> reservationsOf(WorkOrder model) {
    List<WorkCenterReservation> result = new ArrayList<>();
    for (WorkOrderOperation operation : orderedOperations(model)) {
      if (operation.getPlannedStartTime() == null || operation.getPlannedEndTime() == null) {
        continue;
      }
      result.add(
          new WorkCenterReservation(
              model.getId(),
              operation.getWorkCenterId(),
              operation.getPlannedStartTime(),
              operation.getPlannedEndTime()));
    }
    return result;
  }

  /**
   * 排产要读到的未来范围：交期最远的那张单，加一个月缓冲
   *
   * @param now 当前时间
   * @return 读取占用的截止时间
   */
  private LocalDateTime futureHorizon(LocalDateTime now) {
    return now.plusMonths(1);
  }

  // ==================== 私有：两个方向 ====================

  /**
   * 倒排：从交期往前推，每道工序尽量晚开工
   *
   * <p>工序按顺序反过来处理：末道贴着交期结束，前一道贴着后一道开工结束。同一台设备已经被别的工单占住的
   * 时段要绕开——倒排下绕开就是再往前挪，直到这段工时能完整塞进设备空闲的时间里。
   *
   * @param operations 工序（按顺序）
   * @param capacityByWorkCenter 各工作中心的日可用分钟数
   * @param reservations 别的工单已占用的设备时段
   * @param quantity 计划数量
   * @param deadline 交期
   * @return 各工序的计划时段（与工序同序）
   */
  private List<WorkOrder.OperationPlan> placeBackward(
      List<WorkOrderOperation> operations,
      Map<Long, BigDecimal> capacityByWorkCenter,
      List<WorkCenterReservation> reservations,
      BigDecimal quantity,
      LocalDateTime deadline) {
    WorkOrder.OperationPlan[] plans = new WorkOrder.OperationPlan[operations.size()];
    LocalDateTime cursor = deadline;
    for (int i = operations.size() - 1; i >= 0; i--) {
      WorkOrderOperation operation = operations.get(i);
      BigDecimal minutes = operation.estimatedMinutes(quantity);
      BigDecimal capacity = capacityOf(capacityByWorkCenter, operation);
      LocalDateTime end = cursor;
      LocalDateTime start = subtractWorkingMinutes(end, minutes, capacity);
      // 撞上别的工单占用就整段往前挪，挪到这段工时不再与任何占用重叠为止
      WorkCenterReservation clash =
          findClash(operation.getWorkCenterId(), start, end, reservations);
      while (clash != null) {
        end = clash.startTime();
        start = subtractWorkingMinutes(end, minutes, capacity);
        clash = findClash(operation.getWorkCenterId(), start, end, reservations);
      }
      plans[i] = new WorkOrder.OperationPlan(start, end);
      cursor = start;
    }
    return List.of(plans);
  }

  /**
   * 正排：从「现在」往后推，每道工序放到工作中心最早的空档
   *
   * <p>按工序顺序处理：本道的最早开工取「上道完工」与「该工作中心上一道占用结束」两者中较晚的那个；
   * 再绕开别的工单已占住的时段——正排下绕开就是再往后挪。
   *
   * @param operations 工序（按顺序）
   * @param capacityByWorkCenter 各工作中心的日可用分钟数
   * @param reservations 别的工单已占用的设备时段
   * @param quantity 计划数量
   * @param now 起点
   * @return 各工序的计划时段（与工序同序）
   */
  private List<WorkOrder.OperationPlan> placeForward(
      List<WorkOrderOperation> operations,
      Map<Long, BigDecimal> capacityByWorkCenter,
      List<WorkCenterReservation> reservations,
      BigDecimal quantity,
      LocalDateTime now) {
    Map<Long, LocalDateTime> workCenterFreeAt = new HashMap<>();
    List<WorkOrder.OperationPlan> plans = new ArrayList<>(operations.size());
    LocalDateTime cursor = now;
    for (WorkOrderOperation operation : operations) {
      LocalDateTime earliest = cursor;
      LocalDateTime freeAt = workCenterFreeAt.get(operation.getWorkCenterId());
      if (freeAt != null && freeAt.isAfter(earliest)) {
        earliest = freeAt;
      }
      BigDecimal minutes = operation.estimatedMinutes(quantity);
      BigDecimal capacity = capacityOf(capacityByWorkCenter, operation);
      LocalDateTime start = earliest;
      LocalDateTime end = addWorkingMinutes(start, minutes, capacity);
      // 撞上别的工单占用就整段往后挪，挪到不再与任何占用重叠为止
      WorkCenterReservation clash =
          findClash(operation.getWorkCenterId(), start, end, reservations);
      while (clash != null) {
        start = clash.endTime();
        end = addWorkingMinutes(start, minutes, capacity);
        clash = findClash(operation.getWorkCenterId(), start, end, reservations);
      }
      plans.add(new WorkOrder.OperationPlan(start, end));
      workCenterFreeAt.put(operation.getWorkCenterId(), end);
      cursor = end;
    }
    return plans;
  }

  // ==================== 私有：产能与工时 ====================

  /**
   * 在给定设备上找与指定时段重叠的占用
   *
   * @param workCenterId 工作中心Id
   * @param start 时段开始
   * @param end 时段结束
   * @param reservations 全部占用
   * @return 撞上的那段占用，没有重叠返回 {@code null}
   */
  private WorkCenterReservation findClash(
      Long workCenterId,
      LocalDateTime start,
      LocalDateTime end,
      List<WorkCenterReservation> reservations) {
    for (WorkCenterReservation reservation : reservations) {
      if (!reservation.workCenterId().equals(workCenterId)) {
        continue;
      }
      if (start.isBefore(reservation.endTime()) && end.isAfter(reservation.startTime())) {
        return reservation;
      }
    }
    return null;
  }

  /**
   * 取各工序所属工作中心的日可用分钟数
   *
   * @param operations 工序列表
   * @return 工作中心Id 到日可用分钟数的映射
   * @throws DomainException 工作中心不存在或已停用时抛出
   */
  private Map<Long, BigDecimal> loadCapacity(List<WorkOrderOperation> operations) {
    Map<Long, BigDecimal> result = new HashMap<>();
    for (WorkOrderOperation operation : operations) {
      Long workCenterId = operation.getWorkCenterId();
      if (result.containsKey(workCenterId)) {
        continue;
      }
      WorkCenter workCenter = workCenterRepository.get(workCenterId);
      if (workCenter == null) {
        throw new DomainException("工作中心" + workCenterId + "不存在!");
      }
      if (Boolean.TRUE.equals(workCenter.getDisabled())) {
        throw new DomainException("工作中心" + workCenter.getName() + "已停用，不能排产");
      }
      result.put(workCenterId, workCenter.getCapacityMinutesPerDay());
    }
    return result;
  }

  /**
   * 取本道工序所在工作中心的日可用分钟数
   *
   * @param capacityByWorkCenter 工作中心产能表
   * @param operation 工序
   * @return 日可用分钟数
   */
  private BigDecimal capacityOf(
      Map<Long, BigDecimal> capacityByWorkCenter, WorkOrderOperation operation) {
    return capacityByWorkCenter.get(operation.getWorkCenterId());
  }

  /**
   * 从某个时点往后推若干工作分钟
   *
   * <p>每天从 {@link #WORKDAY_START} 开始干满 {@code capacityMinutesPerDay} 分钟；当天不够就顺延到下一个
   * 工作日，非工作日整天跳过。起点若落在非工作日或收工之后，先挪到下一个工作日的开工时间。
   *
   * @param from 起点
   * @param minutes 需要的工作分钟数
   * @param capacityMinutesPerDay 每天可用分钟数
   * @return 完工时间
   */
  private LocalDateTime addWorkingMinutes(
      LocalDateTime from, BigDecimal minutes, BigDecimal capacityMinutesPerDay) {
    long capacity = capacityMinutesPerDay.longValueExact();
    BigDecimal remaining = minutes.setScale(0, RoundingMode.CEILING);
    LocalDateTime cursor = nextWorkStart(from, capacity);
    while (remaining.signum() > 0) {
      LocalDateTime dayEnd = cursor.toLocalDate().atTime(WORKDAY_START).plusMinutes(capacity);
      BigDecimal available =
          BigDecimal.valueOf(Duration.between(cursor, dayEnd).toMinutes());
      if (remaining.compareTo(available) <= 0) {
        return cursor.plusMinutes(remaining.longValueExact());
      }
      remaining = remaining.subtract(available);
      cursor = nextWorkStart(cursor.toLocalDate().plusDays(1).atStartOfDay(), capacity);
    }
    return cursor;
  }

  /**
   * 从某个时点往前推若干工作分钟
   *
   * <p>与 {@link #addWorkingMinutes} 对称：终点若落在非工作日或开工之前，先挪到上一个工作日的收工时间。
   *
   * @param from 终点
   * @param minutes 需要的工作分钟数
   * @param capacityMinutesPerDay 每天可用分钟数
   * @return 开工时间
   */
  private LocalDateTime subtractWorkingMinutes(
      LocalDateTime from, BigDecimal minutes, BigDecimal capacityMinutesPerDay) {
    long capacity = capacityMinutesPerDay.longValueExact();
    BigDecimal remaining = minutes.setScale(0, RoundingMode.CEILING);
    LocalDateTime cursor = previousWorkEnd(from, capacity);
    while (remaining.signum() > 0) {
      LocalDateTime dayStart = cursor.toLocalDate().atTime(WORKDAY_START);
      BigDecimal available = BigDecimal.valueOf(Duration.between(dayStart, cursor).toMinutes());
      if (remaining.compareTo(available) <= 0) {
        return cursor.minusMinutes(remaining.longValueExact());
      }
      remaining = remaining.subtract(available);
      cursor = previousWorkEnd(cursor.toLocalDate().minusDays(1).atTime(WORKDAY_START), capacity);
    }
    return cursor;
  }

  /**
   * 找不早于入参的下一个可开工时点
   *
   * @param time 入参
   * @param capacity 每天可用分钟数
   * @return 可开工时点
   */
  private LocalDateTime nextWorkStart(LocalDateTime time, long capacity) {
    LocalDate date = time.toLocalDate();
    while (true) {
      if (!workingCalendar.isWorkingDay(date)) {
        date = date.plusDays(1);
        time = date.atStartOfDay();
        continue;
      }
      LocalDateTime dayStart = date.atTime(WORKDAY_START);
      LocalDateTime dayEnd = dayStart.plusMinutes(capacity);
      if (time.isBefore(dayStart)) {
        return dayStart;
      }
      if (time.isBefore(dayEnd)) {
        return time;
      }
      date = date.plusDays(1);
      time = date.atStartOfDay();
    }
  }

  /**
   * 找不晚于入参的上一个可收工时点
   *
   * @param time 入参
   * @param capacity 每天可用分钟数
   * @return 可收工时点
   */
  private LocalDateTime previousWorkEnd(LocalDateTime time, long capacity) {
    LocalDate date = time.toLocalDate();
    while (true) {
      if (!workingCalendar.isWorkingDay(date)) {
        date = date.minusDays(1);
        time = date.atTime(LocalTime.MAX);
        continue;
      }
      LocalDateTime dayStart = date.atTime(WORKDAY_START);
      LocalDateTime dayEnd = dayStart.plusMinutes(capacity);
      if (time.isAfter(dayEnd)) {
        return dayEnd;
      }
      if (time.isAfter(dayStart)) {
        return time;
      }
      date = date.minusDays(1);
      time = date.atTime(LocalTime.MAX);
    }
  }

  // ==================== 私有：取工单 ====================

  /**
   * 按工序顺序排列未删除的工序
   *
   * <p>与 {@code WorkOrder.applySchedule} 用同一口径：都按序号排。聚合里的集合顺序由 JPA 决定，不保证有序，
   * 排产算法依赖「前一道做完才轮到后一道」，所以两边必须显式排序，否则算出来的时段会写到别的工序上。
   *
   * @param model 工单
   * @return 有序工序列表
   */
  private List<WorkOrderOperation> orderedOperations(WorkOrder model) {
    return model.getWorkOrderOperations().stream()
        .filter(o -> !o.getIsDeleted())
        .sorted(Comparator.comparing(WorkOrderOperation::getSequence))
        .toList();
  }

  /**
   * 取工单，不存在则抛异常
   *
   * @param id 工单Id
   * @return 工单聚合
   * @throws DomainException 工单不存在时抛出
   */
  private WorkOrder getExist(long id) {
    WorkOrder model = workOrderRepository.get(id);
    if (model == null) {
      throw new DomainException("工单" + id + "不存在!");
    }
    return model;
  }
}
