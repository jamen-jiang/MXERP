package com.mxerp.production.domain.operation;

import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

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
   * @param code 工序代码
   * @param name 工序名称
   * @param workCenterId 默认工作中心Id（可空）
   * @param batchSize 默认批量（可空）
   * @param description 工序说明（可空）
   * @param disabled 是否禁用
   * @return 工序聚合
   * @throws DomainException 工序代码已存在时抛出
   */
  public Operation create(
      String code,
      String name,
      @Nullable Long workCenterId,
      @Nullable BigDecimal batchSize,
      @Nullable String description,
      boolean disabled) {
    // 校验工序代码唯一
    if (operationRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("工序代码" + code + "已存在!");
    }
    return new Operation(code, name, workCenterId, batchSize, description, disabled);
  }
}
