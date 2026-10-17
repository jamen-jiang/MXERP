package com.mxerp.production.domain.routing;

import com.mxerp.shared.domain.exception.DomainException;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.stream.Collectors;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 工艺路线领域服务，负责跨聚合编排与子实体同步 */
@NullMarked
public class RoutingDomainService {

  /** 工艺路线工厂 */
  private final RoutingFactory routingFactory;

  /** 工艺路线仓储 */
  private final RoutingRepository routingRepository;

  /**
   * 构造器注入
   *
   * @param routingFactory 工艺路线工厂
   * @param routingRepository 工艺路线仓储
   */
  public RoutingDomainService(
      RoutingFactory routingFactory, RoutingRepository routingRepository) {
    this.routingFactory = routingFactory;
    this.routingRepository = routingRepository;
  }

  /**
   * 导入生成：工艺路线存在则修改，不存在则创建
   *
   * @param code 路线代码
   * @param name 路线名称
   * @param version 版本号
   * @param status 状态
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @return 工艺路线聚合
   */
  public Routing importGenerate(
      String code,
      String name,
      String version,
      String status,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    // 按代码 + 版本查工艺路线
    Routing model = routingRepository.getByCodeAndVersion(code, version);

    // 不存在则创建，存在则修改
    if (model == null) {
      model =
          routingFactory.create(code, name, version, status, effectiveFrom, effectiveTo, remark);
    } else {
      modify(model, code, name, version, status, effectiveFrom, effectiveTo, remark);
    }

    return model;
  }

  /**
   * 修改工艺路线
   *
   * @param id 工艺路线Id
   * @param code 路线代码
   * @param name 路线名称
   * @param version 版本号
   * @param status 状态
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @return 工艺路线聚合
   */
  public Routing modify(
      long id,
      String code,
      String name,
      String version,
      String status,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    Routing model = routingRepository.get(id);
    if (model == null) {
      throw new DomainException("工艺路线" + id + "不存在!");
    }
    modify(model, code, name, version, status, effectiveFrom, effectiveTo, remark);
    return model;
  }

  /**
   * 修改工艺路线
   *
   * @param model 工艺路线
   * @param code 路线代码
   * @param name 路线名称
   * @param version 版本号
   * @param status 状态
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   */
  public void modify(
      Routing model,
      String code,
      String name,
      String version,
      String status,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    // 校验路线代码 + 版本唯一
    if (routingRepository.isExistByCodeAndVersionExcludeId(code, version, model.getId())) {
      throw new DomainException("工艺路线" + code + "的版本" + version + "已存在!");
    }
    model.modify(code, name, version, status, effectiveFrom, effectiveTo, remark);
  }

  /**
   * 同步路线工序列表（按工序顺序匹配：删除不在输入中的、更新已存在的、添加新的）
   *
   * @param routing 工艺路线聚合
   * @param operations 目标工序列表（可空，null 视为清空）
   */
  public void syncOperations(Routing routing, @Nullable List<RoutingOperationData> operations) {
    List<RoutingOperationData> target = operations == null ? List.of() : operations;

    // 1. 删除未在输入中出现的工序（按顺序匹配）
    List<RoutingOperation> toRemoves =
        routing.getRoutingOperations().stream()
            .filter(
                o ->
                    target.stream()
                        .noneMatch(d -> Objects.equals(d.sequence(), o.getSequence())))
            .toList();
    for (RoutingOperation operation : toRemoves) {
      routing.deleteOperation(operation.getId());
    }

    // 2. 建立"顺序 → 已存在工序Id"的映射（只统计未删除的）
    Map<Integer, Long> existingDict =
        routing.getRoutingOperations().stream()
            .filter(o -> !o.getIsDeleted())
            .collect(
                Collectors.toMap(
                    RoutingOperation::getSequence, RoutingOperation::getId, (a, b) -> a));

    // 3. 遍历输入：存在则修改，不存在则添加
    for (RoutingOperationData data : target) {
      Long id = existingDict.get(data.sequence());
      if (id != null) {
        // 存在：修改
        routing.modifyOperation(
            id,
            data.sequence(),
            data.operationId(),
            data.workCenterId(),
            data.setupTime(),
            data.runTime(),
            data.queueTime(),
            data.standardBatchSize(),
            data.isOutsourced(),
            data.isOptional(),
            data.reworkOperationId(),
            data.remark());
      } else {
        // 不存在：添加
        routing.addOperation(
            data.sequence(),
            data.operationId(),
            data.workCenterId(),
            data.setupTime(),
            data.runTime(),
            data.queueTime(),
            data.standardBatchSize(),
            data.isOutsourced(),
            data.isOptional(),
            data.reworkOperationId(),
            data.remark());
      }
    }
  }
}
