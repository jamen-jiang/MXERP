package com.mxerp.production.domain.workorder;

import com.mxerp.shared.domain.repository.Repository;
import java.time.LocalDateTime;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 工单仓储接口 */
@NullMarked
public interface WorkOrderRepository extends Repository<WorkOrder> {

  /**
   * 按单号查询
   *
   * @param number 工单号
   * @return 工单（可空），不存在返回 null
   */
  @Nullable WorkOrder getByNumber(String number);

  /**
   * 按来源单据查询工单
   *
   * <p>销售订单行拆批投产时，一个来源会对应多张工单，所以返回列表。
   *
   * @param sourceType 来源类型编码
   * @param sourceId 来源单据Id
   * @return 工单列表
   */
  List<WorkOrder> getListBySource(String sourceType, Long sourceId);

  /**
   * 按产出SKU查询工单
   *
   * @param skuId 产出SKU
   * @return 工单列表
   */
  List<WorkOrder> getListBySkuId(Long skuId);

  /**
   * 取某段时间内已排产的工序占用
   *
   * <p>排产要绕开别的工单已经占住的设备时段，所以把这段时间里所有带计划时间的工序读出来。排除正在排的
   * 那张工单，否则会把自己上一次的排产结果当成别人的占用。
   *
   * @param from 起始时间（含）
   * @param to 结束时间（含）
   * @param excludeWorkOrderId 要排除的工单Id（可空，批量排产时传 null）
   * @return 占用时段列表
   */
  List<WorkCenterReservation> getReservations(
      LocalDateTime from, LocalDateTime to, @Nullable Long excludeWorkOrderId);

  /**
   * 判断单号是否存在
   *
   * @param number 工单号
   * @param id 排除的工单Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistByNumberExcludeId(String number, @Nullable Long id);

  /**
   * 判断单号是否存在
   *
   * @param number 工单号
   * @return true 表示已存在
   */
  default boolean isExistByNumberExcludeId(String number) {
    return isExistByNumberExcludeId(number, null);
  }
}
