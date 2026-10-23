package com.mxerp.production.domain.operation;

import com.mxerp.production.domain.workcenter.WorkCenterRepository;
import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 工序领域服务，负责跨聚合编排 */
@NullMarked
public class OperationDomainService {

  /** 工序工厂 */
  private final OperationFactory operationFactory;

  /** 工序仓储 */
  private final OperationRepository operationRepository;

  /** 工作中心仓储 */
  private final WorkCenterRepository workCenterRepository;

  /**
   * 构造器注入
   *
   * @param operationFactory 工序工厂
   * @param operationRepository 工序仓储
   * @param workCenterRepository 工作中心仓储
   */
  public OperationDomainService(
      OperationFactory operationFactory,
      OperationRepository operationRepository,
      WorkCenterRepository workCenterRepository) {
    this.operationFactory = operationFactory;
    this.operationRepository = operationRepository;
    this.workCenterRepository = workCenterRepository;
  }

  /**
   * 导入生成：工序存在则修改，不存在则创建
   *
   * @param code 工序代码
   * @param name 工序名称
   * @param workCenterId 默认工作中心Id（可空）
   * @param batchSize 默认批量（可空）
   * @param description 工序说明（可空）
   * @param disabled 是否禁用
   * @return 工序聚合
   */
  public Operation importGenerate(
      String code,
      String name,
      @Nullable Long workCenterId,
      @Nullable BigDecimal batchSize,
      @Nullable String description,
      boolean disabled) {
    // 按代码查工序
    Operation model = operationRepository.getByCode(code);

    // 不存在则创建，存在则修改
    if (model == null) {
      requireWorkCenter(workCenterId);
      model =
          operationFactory.create(code, name, workCenterId, batchSize, description, disabled);
    } else {
      modify(model, code, name, workCenterId, batchSize, description, disabled);
    }

    return model;
  }

  /**
   * 修改工序
   *
   * @param id 工序Id
   * @param code 工序代码
   * @param name 工序名称
   * @param workCenterId 默认工作中心Id（可空）
   * @param batchSize 默认批量（可空）
   * @param description 工序说明（可空）
   * @param disabled 是否禁用
   * @return 工序聚合
   * @throws DomainException 工序不存在时抛出
   */
  public Operation modify(
      long id,
      String code,
      String name,
      @Nullable Long workCenterId,
      @Nullable BigDecimal batchSize,
      @Nullable String description,
      boolean disabled) {
    Operation model = operationRepository.get(id);
    if (model == null) {
      throw new DomainException("工序" + id + "不存在!");
    }
    modify(model, code, name, workCenterId, batchSize, description, disabled);
    return model;
  }

  /**
   * 修改工序
   *
   * @param model 工序
   * @param code 工序代码
   * @param name 工序名称
   * @param workCenterId 默认工作中心Id（可空）
   * @param batchSize 默认批量（可空）
   * @param description 工序说明（可空）
   * @param disabled 是否禁用
   * @throws DomainException 工序代码已被其它工序占用、或工作中心不存在时抛出
   */
  public void modify(
      Operation model,
      String code,
      String name,
      @Nullable Long workCenterId,
      @Nullable BigDecimal batchSize,
      @Nullable String description,
      boolean disabled) {
    // 校验工序代码唯一
    if (operationRepository.isExistByCodeExcludeId(code, model.getId())) {
      throw new DomainException("工序代码" + code + "已存在!");
    }
    requireWorkCenter(workCenterId);
    model.modify(code, name, workCenterId, batchSize, description, disabled);
  }

  /**
   * 校验默认工作中心存在（可空：为空表示建路线时必须自己选）
   *
   * @param workCenterId 工作中心Id（可空）
   * @throws DomainException 工作中心不存在时抛出
   */
  private void requireWorkCenter(@Nullable Long workCenterId) {
    if (workCenterId == null) {
      return;
    }
    if (workCenterRepository.get(workCenterId) == null) {
      throw new DomainException("工作中心" + workCenterId + "不存在!");
    }
  }
}
