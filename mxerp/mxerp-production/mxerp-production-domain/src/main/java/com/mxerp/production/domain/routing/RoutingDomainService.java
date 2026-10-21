package com.mxerp.production.domain.routing;

import com.mxerp.shared.domain.exception.DomainException;
import java.time.LocalDateTime;
import java.util.ArrayList;
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
   * <p>导入只维护草稿版本：状态由发布/作废推进，不接受表里填的状态，否则一份表就能把冻结的版本改开。
   *
   * @param materialId 物料系列Id（SPU）
   * @param code 路线代码
   * @param name 路线名称
   * @param version 版本号
   * @param isPrimary 是否主路线
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @return 工艺路线聚合
   */
  public Routing importGenerate(
      long materialId,
      String code,
      String name,
      String version,
      boolean isPrimary,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    // 按代码 + 版本查工艺路线
    Routing model = routingRepository.getByCodeAndVersion(code, version);

    // 不存在则创建，存在则修改
    if (model == null) {
      model =
          routingFactory.create(
              materialId, code, name, version, isPrimary, effectiveFrom, effectiveTo, remark);
    } else {
      modify(
          model,
          materialId,
          code,
          name,
          version,
          isPrimary,
          effectiveFrom,
          effectiveTo,
          remark);
    }

    return model;
  }

  /**
   * 修改工艺路线
   *
   * @param id 工艺路线Id
   * @param materialId 物料系列Id（SPU）
   * @param code 路线代码
   * @param name 路线名称
   * @param version 版本号
   * @param isPrimary 是否主路线
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @return 工艺路线聚合
   * @throws DomainException 工艺路线不存在时抛出
   */
  public Routing modify(
      long id,
      long materialId,
      String code,
      String name,
      String version,
      boolean isPrimary,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    Routing model = getExist(id);
    modify(
        model,
        materialId,
        code,
        name,
        version,
        isPrimary,
        effectiveFrom,
        effectiveTo,
        remark);
    return model;
  }

  /**
   * 修改工艺路线
   *
   * @param model 工艺路线
   * @param materialId 物料系列Id（SPU）
   * @param code 路线代码
   * @param name 路线名称
   * @param version 版本号
   * @param isPrimary 是否主路线
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @throws DomainException 路线代码 + 版本与其它路线冲突、或版本已不是草稿时抛出
   */
  public void modify(
      Routing model,
      long materialId,
      String code,
      String name,
      String version,
      boolean isPrimary,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    // 校验路线代码 + 版本唯一
    if (routingRepository.isExistByCodeAndVersionExcludeId(code, version, model.getId())) {
      throw new DomainException("工艺路线" + code + "的版本" + version + "已存在!");
    }
    model.modify(
        materialId, code, name, version, isPrimary, effectiveFrom, effectiveTo, remark);
  }

  // ==================== 版本与生效链 ====================

  /**
   * 发布版本，并按生效日期接力：生效更早的已发布版本自动收口到本版生效点
   *
   * <p>同一代码任一时刻至多一个生效版本，所以下达时才谈得上「按这个时点取那一版」。发布前先跟其它已发布
   * 版本比区间：落在本版生效点上还活着的那一版收口给本版让位，其余版本与本版区间交叠的直接拒绝。
   *
   * <p>同一物料的其它主路线也参与同一条时间线：新主路线发布时，旧主路线在交接点收口，保证任一时刻
   * 至多一条主路线生效。
   *
   * @param id 工艺路线Id
   * @return 工艺路线聚合
   * @throws DomainException 工艺路线不存在、没有生效时间、或生效区间与同代码版本、同物料主路线重叠时抛出
   */
  public Routing publish(long id) {
    Routing model = getExist(id);
    LocalDateTime effectiveFrom = model.getEffectiveFrom();
    if (effectiveFrom == null) {
      throw new DomainException("工艺路线版本" + model.getVersion() + "没有生效时间，不能发布");
    }

    List<Routing> released =
        routingRepository.getListByMaterialId(model.getMaterialId()).stream()
            .filter(r -> !Objects.equals(r.getId(), model.getId()))
            .filter(Routing::isReleased)
            .filter(
                r ->
                    r.getCode().equals(model.getCode())
                        || (Boolean.TRUE.equals(model.getIsPrimary())
                            && Boolean.TRUE.equals(r.getIsPrimary())))
            .toList();

    // 1. 先全量校验：生效更早且覆盖本版起点的那一版由本版接棒，其余版本区间不许与本版交叠
    List<Routing> predecessors = new ArrayList<>();
    for (Routing other : released) {
      LocalDateTime otherFrom = other.getEffectiveFrom();
      if (otherFrom != null
          && otherFrom.isBefore(effectiveFrom)
          && other.isEffectiveAt(effectiveFrom)) {
        predecessors.add(other);
        continue;
      }
      if (other.overlaps(effectiveFrom, model.getEffectiveTo())) {
        throw new DomainException(
            "工艺路线"
                + model.getCode()
                + "版本"
                + model.getVersion()
                + "的生效区间与"
                + other.getCode()
                + "版本"
                + other.getVersion()
                + "重叠!");
      }
    }

    // 2. 前一版收口到本版生效点，区间既不留缝也不重叠
    for (Routing predecessor : predecessors) {
      predecessor.closeAt(effectiveFrom);
    }

    model.publish();
    return model;
  }

  /**
   * 作废版本
   *
   * @param id 工艺路线Id
   * @return 工艺路线聚合
   * @throws DomainException 工艺路线不存在、或不是已发布状态时抛出
   */
  public Routing obsolete(long id) {
    Routing model = getExist(id);
    model.obsolete();
    return model;
  }

  /**
   * 升版：把已发布版本的工序整单复制成一个新草稿
   *
   * <p>已发布版本冻结以后，改工艺只能开新版本，复制是这一步唯一的省事办法。新草稿沿用代码、名称与全部工序
   * （含工序引用的费率卡），只换版本号和生效区间，改完再走发布。
   *
   * @param id 源工艺路线Id（一般是被替换的那一版）
   * @param version 新版本号
   * @param effectiveFrom 新版本生效时间（可空，发布前必填）
   * @param effectiveTo 新版本失效时间（可空）
   * @param remark 备注（可空）
   * @return 新草稿工艺路线（由调用方落库）
   * @throws DomainException 工艺路线不存在、源版本还是草稿、或新版本号已存在时抛出
   */
  public Routing revise(
      long id,
      String version,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    Routing source = getExist(id);
    if (source.isDraft()) {
      throw new DomainException("工艺路线版本" + source.getVersion() + "还是草稿，直接改就行，不必升版");
    }

    Routing model =
        routingFactory.create(
            source.getMaterialId(),
            source.getCode(),
            source.getName(),
            version,
            Boolean.TRUE.equals(source.getIsPrimary()),
            effectiveFrom,
            effectiveTo,
            remark);
    for (RoutingOperation operation : source.getRoutingOperations()) {
      model.addOperation(
          operation.getSequence(),
          operation.getOperationId(),
          operation.getWorkCenterId(),
          operation.getSetupTime(),
          operation.getRunTime(),
          operation.getQueueTime(),
          operation.getRateId(),
          operation.getTimeFormula(),
          operation.getStandardBatchSize(),
          operation.getIsOutsourced(),
          operation.getIsOptional(),
          operation.getReworkOperationId(),
          operation.getRemark());
    }
    return model;
  }

  // ==================== 私有 ====================

  /**
   * 取工艺路线，不存在则抛异常
   *
   * @param id 工艺路线Id
   * @return 工艺路线聚合
   * @throws DomainException 不存在时抛出
   */
  private Routing getExist(long id) {
    Routing model = routingRepository.get(id);
    if (model == null) {
      throw new DomainException("工艺路线" + id + "不存在!");
    }
    return model;
  }

  /**
   * 同步路线工序列表（按工序顺序匹配：删除不在输入中的、更新已存在的、添加新的）
   *
   * @param routing 工艺路线聚合
   * @param operations 目标工序列表（可空，null 视为清空）
   * @throws DomainException 路线已不是草稿时抛出
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
            data.rateId(),
            data.timeFormula(),
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
            data.rateId(),
            data.timeFormula(),
            data.standardBatchSize(),
            data.isOutsourced(),
            data.isOptional(),
            data.reworkOperationId(),
            data.remark());
      }
    }
  }
}
