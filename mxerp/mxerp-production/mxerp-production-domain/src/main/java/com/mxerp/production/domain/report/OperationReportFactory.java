package com.mxerp.production.domain.report;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;

/** 工序报工单工厂，负责创建报工单 */
@NullMarked
public class OperationReportFactory {

  /** 报工单仓储 */
  private final OperationReportRepository operationReportRepository;

  /**
   * 构造器注入
   *
   * @param operationReportRepository 报工单仓储
   */
  public OperationReportFactory(OperationReportRepository operationReportRepository) {
    this.operationReportRepository = operationReportRepository;
  }

  /**
   * 创建报工单（单号由 OrderAddedEvent 的监听器分配）
   *
   * @param data 报工数据
   * @return 报工单聚合
   */
  public OperationReport create(OperationReportData data) {
    return create(data, "");
  }

  /**
   * 创建报工单
   *
   * @param data 报工数据
   * @param number 报工单号（传空串表示由 OrderAddedEvent 的监听器分配）
   * @return 报工单聚合
   * @throws DomainException 单号已存在时抛出
   */
  public OperationReport create(OperationReportData data, String number) {
    // 单号非空时才校验唯一，空串代表尚未分配
    if (!number.isBlank() && operationReportRepository.isExistByNumberExcludeId(number)) {
      throw new DomainException("报工单号" + number + "已存在!");
    }
    return new OperationReport(
        data.workOrderId(),
        data.workOrderNumber(),
        data.workOrderOperationId(),
        data.operationId(),
        data.operationName(),
        data.workCenterId(),
        data.workCenterName(),
        data.operatorId(),
        data.startTime(),
        data.endTime(),
        data.workTime(),
        data.goodQuantity(),
        data.scrapQuantity(),
        data.remark(),
        number);
  }
}
