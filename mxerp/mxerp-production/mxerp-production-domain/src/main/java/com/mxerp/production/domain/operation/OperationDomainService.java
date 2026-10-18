package com.mxerp.production.domain.operation;

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
   * @param code 工序代码
   * @param name 工序名称
   * @param defaultWorkCenterId 默认工作中心（可空，建路线时预填）
   * @param batchSize 标准批量（可空，一次能同时处理的数量）
   * @param isCorrective 是否返修工序
   * @param qualityInspectionRequired 是否需要质检
   * @param description 工序说明（可空）
   * @param disabled 是否禁用
   * @return 工序聚合
   */
  public Operation importGenerate(
      String code,
      String name,
      @Nullable Long defaultWorkCenterId,
      @Nullable BigDecimal batchSize,
      boolean isCorrective,
      boolean qualityInspectionRequired,
      @Nullable String description,
      boolean disabled) {
    // 按代码查工序
    Operation model = operationRepository.getByCode(code);

    // 不存在则创建，存在则修改
    if (model == null) {
      model =
          operationFactory.create(
              code,
              name,
              defaultWorkCenterId,
              batchSize,
              isCorrective,
              qualityInspectionRequired,
              description,
              disabled);
    } else {
      modify(
          model,
          code,
          name,
          defaultWorkCenterId,
          batchSize,
          isCorrective,
          qualityInspectionRequired,
          description,
          disabled);
    }

    return model;
  }

  /**
   * 修改工序
   *
   * @param id 工序Id
   * @param code 工序代码
   * @param name 工序名称
   * @param defaultWorkCenterId 默认工作中心（可空，建路线时预填）
   * @param batchSize 标准批量（可空，一次能同时处理的数量）
   * @param isCorrective 是否返修工序
   * @param qualityInspectionRequired 是否需要质检
   * @param description 工序说明（可空）
   * @param disabled 是否禁用
   * @return 工序聚合
   * @throws DomainException 工序不存在时抛出
   */
  public Operation modify(
      long id,
      String code,
      String name,
      @Nullable Long defaultWorkCenterId,
      @Nullable BigDecimal batchSize,
      boolean isCorrective,
      boolean qualityInspectionRequired,
      @Nullable String description,
      boolean disabled) {
    Operation model = operationRepository.get(id);
    if (model == null) {
      throw new DomainException("工序" + id + "不存在!");
    }
    modify(
        model,
        code,
        name,
        defaultWorkCenterId,
        batchSize,
        isCorrective,
        qualityInspectionRequired,
        description,
        disabled);
    return model;
  }

  /**
   * 修改工序
   *
   * @param model 工序
   * @param code 工序代码
   * @param name 工序名称
   * @param defaultWorkCenterId 默认工作中心（可空，建路线时预填）
   * @param batchSize 标准批量（可空，一次能同时处理的数量）
   * @param isCorrective 是否返修工序
   * @param qualityInspectionRequired 是否需要质检
   * @param description 工序说明（可空）
   * @param disabled 是否禁用
   * @throws DomainException 工序代码已被其它工序占用时抛出
   */
  public void modify(
      Operation model,
      String code,
      String name,
      @Nullable Long defaultWorkCenterId,
      @Nullable BigDecimal batchSize,
      boolean isCorrective,
      boolean qualityInspectionRequired,
      @Nullable String description,
      boolean disabled) {
    // 校验工序代码唯一
    if (operationRepository.isExistByCodeExcludeId(code, model.getId())) {
      throw new DomainException("工序代码" + code + "已存在!");
    }
    model.modify(
        code,
        name,
        defaultWorkCenterId,
        batchSize,
        isCorrective,
        qualityInspectionRequired,
        description,
        disabled);
  }
}
