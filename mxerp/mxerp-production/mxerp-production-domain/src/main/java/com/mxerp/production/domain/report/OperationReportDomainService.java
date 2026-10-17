package com.mxerp.production.domain.report;

import com.mxerp.production.domain.workorder.WorkOrder;
import com.mxerp.production.domain.workorder.WorkOrderRepository;
import com.mxerp.shared.domain.exception.DomainException;
import java.util.List;
import org.jspecify.annotations.NullMarked;

/**
 * 工序报工单领域服务
 *
 * <p>提交报工要同时动两个聚合：报工单自己置为已提交，工单工序累加良品/报废/工时。这是跨聚合的
 * 一致性动作，所以放在领域服务里编排，而不是让报工单去反向修改工单。
 */
@NullMarked
public class OperationReportDomainService {

  /** 报工单工厂 */
  private final OperationReportFactory operationReportFactory;

  /** 报工单仓储 */
  private final OperationReportRepository operationReportRepository;

  /** 工单仓储 */
  private final WorkOrderRepository workOrderRepository;

  /**
   * 构造器注入
   *
   * @param operationReportFactory 报工单工厂
   * @param operationReportRepository 报工单仓储
   * @param workOrderRepository 工单仓储
   */
  public OperationReportDomainService(
      OperationReportFactory operationReportFactory,
      OperationReportRepository operationReportRepository,
      WorkOrderRepository workOrderRepository) {
    this.operationReportFactory = operationReportFactory;
    this.operationReportRepository = operationReportRepository;
    this.workOrderRepository = workOrderRepository;
  }

  /**
   * 创建报工单（草稿）
   *
   * @param data 报工数据
   * @return 报工单聚合
   */
  public OperationReport create(OperationReportData data) {
    return operationReportFactory.create(data);
  }

  /**
   * 修改报工单（仅草稿可改）
   *
   * @param id 报工单Id
   * @param data 报工数据
   * @return 报工单聚合
   * @throws DomainException 报工单不存在时抛出
   */
  public OperationReport modify(long id, OperationReportData data) {
    OperationReport model = getExist(id);
    model.modify(
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
        data.remark());
    return model;
  }

  /**
   * 提交报工单：置为已提交，并累加到工单工序
   *
   * <p>先让工单侧校验通过再改自己的状态——反过来的话，工单侧一抛异常就会留下一张「已提交但没汇总」
   * 的报工单。
   *
   * @param id 报工单Id
   * @return 报工单聚合
   * @throws DomainException 报工单或工单不存在、工序未开工、报工数超过投入数时抛出
   */
  public OperationReport submit(long id) {
    OperationReport model = getExist(id);
    if (model.isSubmitted()) {
      throw new DomainException("报工单" + model.getNumber() + "已提交，不能重复提交");
    }

    WorkOrder workOrder = workOrderRepository.get(model.getWorkOrderId());
    if (workOrder == null) {
      throw new DomainException("工单" + model.getWorkOrderId() + "不存在!");
    }
    workOrder.reportOperation(
        model.getWorkOrderOperationId(),
        model.getGoodQuantity(),
        model.getScrapQuantity(),
        model.getWorkTime());

    model.submit();
    return model;
  }

  /**
   * 按工单查询已提交的报工单
   *
   * @param workOrderId 工单Id
   * @return 已提交的报工单列表
   */
  public List<OperationReport> getSubmittedByWorkOrderId(Long workOrderId) {
    return operationReportRepository.getListByWorkOrderId(workOrderId).stream()
        .filter(OperationReport::isSubmitted)
        .toList();
  }

  /**
   * 取报工单，不存在则抛异常
   *
   * @param id 报工单Id
   * @return 报工单聚合
   * @throws DomainException 报工单不存在时抛出
   */
  private OperationReport getExist(long id) {
    OperationReport model = operationReportRepository.get(id);
    if (model == null) {
      throw new DomainException("报工单" + id + "不存在!");
    }
    return model;
  }
}
