package com.mxerp.production.domain.operation;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;

/** 工序领域服务，负责跨聚合编排 */
@NullMarked
public class OperationDomainService {

  /** 工序工厂 */
  private final OperationFactory operationFactory;

  /** 工序仓储 */
  private final OperationRepository operationRepository;

  /**
   * 构造器注入
   *
   * @param operationFactory 工序工厂
   * @param operationRepository 工序仓储
   */
  public OperationDomainService(
      OperationFactory operationFactory, OperationRepository operationRepository) {
    this.operationFactory = operationFactory;
    this.operationRepository = operationRepository;
  }

  /**
   * 导入生成：工序存在则修改，不存在则创建
   *
   * @param data 工序数据
   * @return 工序聚合
   */
  public Operation importGenerate(OperationData data) {
    // 按代码查工序
    Operation model = operationRepository.getByCode(data.code());

    // 不存在则创建，存在则修改
    if (model == null) {
      model = operationFactory.create(data);
    } else {
      modify(model, data);
    }

    return model;
  }

  /**
   * 修改工序
   *
   * @param id 工序Id
   * @param data 工序数据
   * @return 工序聚合
   * @throws DomainException 工序不存在时抛出
   */
  public Operation modify(long id, OperationData data) {
    Operation model = operationRepository.get(id);
    if (model == null) {
      throw new DomainException("工序" + id + "不存在!");
    }
    modify(model, data);
    return model;
  }

  /**
   * 修改工序
   *
   * @param model 工序
   * @param data 工序数据
   * @throws DomainException 工序代码已被其它工序占用时抛出
   */
  public void modify(Operation model, OperationData data) {
    // 校验工序代码唯一
    if (operationRepository.isExistByCodeExcludeId(data.code(), model.getId())) {
      throw new DomainException("工序代码" + data.code() + "已存在!");
    }
    model.modify(
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
