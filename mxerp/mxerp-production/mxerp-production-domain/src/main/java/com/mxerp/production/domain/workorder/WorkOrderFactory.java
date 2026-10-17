package com.mxerp.production.domain.workorder;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;

/** 工单工厂，负责创建工单 */
@NullMarked
public class WorkOrderFactory {

  /** 工单仓储 */
  private final WorkOrderRepository workOrderRepository;

  /**
   * 构造器注入
   *
   * @param workOrderRepository 工单仓储
   */
  public WorkOrderFactory(WorkOrderRepository workOrderRepository) {
    this.workOrderRepository = workOrderRepository;
  }

  /**
   * 创建工单（单号由 OrderAddedEvent 的监听器分配）
   *
   * @param data 工单数据
   * @return 工单聚合
   */
  public WorkOrder create(WorkOrderData data) {
    return create(data, "");
  }

  /**
   * 创建工单
   *
   * @param data 工单数据
   * @param number 工单号（传空串表示由 OrderAddedEvent 的监听器分配）
   * @return 工单聚合
   * @throws DomainException 单号已存在时抛出
   */
  public WorkOrder create(WorkOrderData data, String number) {
    // 单号非空时才校验唯一，空串代表尚未分配
    if (!number.isBlank() && workOrderRepository.isExistByNumberExcludeId(number)) {
      throw new DomainException("工单号" + number + "已存在!");
    }
    return new WorkOrder(
        data.skuId(),
        data.quantity(),
        data.unitId(),
        data.sourceType(),
        data.sourceId(),
        data.sourceNumber(),
        data.bomId(),
        data.routingId(),
        data.plannedStartTime(),
        data.plannedEndTime(),
        data.wipWarehouseId(),
        data.fgWarehouseId(),
        data.scrapWarehouseId(),
        data.remark(),
        number);
  }
}
