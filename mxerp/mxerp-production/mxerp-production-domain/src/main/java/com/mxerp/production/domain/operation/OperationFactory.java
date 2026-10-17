package com.mxerp.production.domain.operation;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;

/** 工序工厂，负责创建工序 */
@NullMarked
public class OperationFactory {

  /** 工序仓储 */
  private final OperationRepository operationRepository;

  /**
   * 构造器注入
   *
   * @param operationRepository 工序仓储
   */
  public OperationFactory(OperationRepository operationRepository) {
    this.operationRepository = operationRepository;
  }

  /**
   * 创建工序
   *
   * @param data 工序数据
   * @return 工序聚合
   * @throws DomainException 工序代码已存在时抛出
   */
  public Operation create(OperationData data) {
    // 校验工序代码唯一
    if (operationRepository.isExistByCodeExcludeId(data.code())) {
      throw new DomainException("工序代码" + data.code() + "已存在!");
    }
    return new Operation(
        data.code(),
        data.name(),
        data.defaultWorkCenterId(),
        data.batchSize(),
        data.isCorrective(),
        data.qualityInspectionRequired(),
        data.description(),
        data.disabled());
  }
}
