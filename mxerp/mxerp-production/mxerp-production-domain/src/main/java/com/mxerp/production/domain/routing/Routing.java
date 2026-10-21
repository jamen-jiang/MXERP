package com.mxerp.production.domain.routing;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.Index;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToMany;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Objects;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.hibernate.annotations.SQLRestriction;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 工艺路线（挂在物料系列上）
 *
 * <p>路线按 SPU 维护，不按 SKU：同一系列下几百个度数走的是同一套工序，挂到变体等于给每个度数复制一份。
 * 一个系列可以有多条路线（标准、试制、返工），其中一条标为主路线；工单下达时没指定路线，就取该物料在 该时点生效的主路线。个别度数要走别的路线，在工单上直接指定即可。
 *
 * <p><b>版本与生效链。</b>同一代码可以并存多个版本，用生效日期接力：草稿可随便改，发布后冻结（要改就 {@code revise}
 * 升版），作废只退出可选池。已发布版本必须带生效时间，且同一代码的生效区间不许重叠， 发布新版时生效更早的那一版自动收口到新版起点。主路线跨代码也守同一条时间线：新主路线发布时，
 * 旧主路线在交接点收口。工单下达按「时点」取版本，历史工单的工序与工时才不会漂。
 *
 * <p>费率不在这里：工序行只引用费率卡（{@code RoutingOperation.rateId}）。电价、人工、外协谈价随时在变，
 * 调价由费率卡按生效日期换版，不需要动工艺，路线也就不会被价格变更拖着升版。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Routing",
    indexes = {
      @Index(name = "IX_Routing_MaterialId", columnList = "MaterialId"),
      @Index(name = "IX_Routing_Code", columnList = "Code"),
      @Index(name = "IX_Routing_Name", columnList = "Name"),
      @Index(name = "IX_Routing_Status", columnList = "Status")
      // 注意：Code + Version 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class Routing extends UpdateAggregateRoot {

  /** 物料系列Id（SPU）：同系列的度数共用路线 */
  @Column(name = "MaterialId", nullable = false)
  private Long materialId;

  /** 是否主路线：同一物料任一时刻至多一条主路线生效，下达没指定路线时取它 */
  @Column(name = "IsPrimary", nullable = false)
  private Boolean isPrimary;

  /** 路线代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 路线名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 版本号：与路线代码共同构成唯一键，升版就是换一个号 */
  @Nationalized
  @Column(name = "Version", nullable = false, length = 50)
  private String version;

  /** 状态：草稿、已发布、已作废（只能由 {@link #publish()} 与 {@link #obsolete()} 推进，不是入参） */
  @Nationalized
  @Column(name = "Status", nullable = false, length = 50)
  private String status;

  /** 生效时间（可空，草稿期允许未定；发布必填） */
  @Column(name = "EffectiveFrom")
  @Nullable
  private LocalDateTime effectiveFrom;

  /** 失效时间（可空，空表示无限期；发布下一版时由前一版自动收口） */
  @Column(name = "EffectiveTo")
  @Nullable
  private LocalDateTime effectiveTo;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /** 路线工序列表 */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "RoutingId")
  @SQLRestriction("IsDeleted = 0")
  private List<RoutingOperation> routingOperations = new ArrayList<>();

  /**
   * 获取路线工序列表（只读）
   *
   * @return 不可变列表
   */
  public List<RoutingOperation> getRoutingOperations() {
    return Collections.unmodifiableList(routingOperations);
  }

  /**
   * 构造器：新建的工艺路线一律是草稿
   *
   * @param materialId 物料系列Id（SPU）
   * @param code 路线代码
   * @param name 路线名称
   * @param version 版本号
   * @param isPrimary 是否主路线
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   */
  Routing(
      long materialId,
      String code,
      String name,
      String version,
      boolean isPrimary,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    apply(materialId, code, name, version, isPrimary, effectiveFrom, effectiveTo, remark);
    this.status = RoutingStatusEnum.DRAFT.getCode();
  }

  /**
   * 修改工艺路线
   *
   * @param materialId 物料系列Id（SPU）
   * @param code 路线代码
   * @param name 路线名称
   * @param version 版本号
   * @param isPrimary 是否主路线
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @throws DomainException 已发布或已作废时抛出
   */
  void modify(
      long materialId,
      String code,
      String name,
      String version,
      boolean isPrimary,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    checkDraft();
    apply(materialId, code, name, version, isPrimary, effectiveFrom, effectiveTo, remark);
  }

  /**
   * 添加工序
   *
   * @param sequence 工序顺序
   * @param operationId 工序Id
   * @param workCenterId 工作中心Id
   * @param setupTime 准备工时（分钟）
   * @param runTime 加工工时（分钟）
   * @param queueTime 排队工时（分钟）
   * @param rateId 费率卡Id（可空，为空表示这道工序不计加工成本）
   * @param timeFormula 工时取时公式编码
   * @param standardBatchSize 标准批量（可空）
   * @param isOutsourced 是否外协
   * @param isOptional 是否可选工序
   * @param reworkOperationId 返工回到的工序Id（可空）
   * @param remark 备注（可空）
   * @throws DomainException 已发布或已作废、工序顺序重复时抛出
   */
  public void addOperation(
      int sequence,
      long operationId,
      long workCenterId,
      BigDecimal setupTime,
      BigDecimal runTime,
      BigDecimal queueTime,
      @Nullable Long rateId,
      String timeFormula,
      @Nullable BigDecimal standardBatchSize,
      boolean isOutsourced,
      boolean isOptional,
      @Nullable Long reworkOperationId,
      @Nullable String remark) {
    checkDraft();
    if (isExistSequence(sequence, null)) {
      throw new DomainException("工序顺序" + sequence + "已存在!");
    }
    routingOperations.add(
        new RoutingOperation(
            sequence,
            operationId,
            workCenterId,
            setupTime,
            runTime,
            queueTime,
            rateId,
            timeFormula,
            standardBatchSize,
            isOutsourced,
            isOptional,
            reworkOperationId,
            remark));
  }

  /**
   * 修改工序
   *
   * @param routingOperationId 路线工序Id
   * @param sequence 工序顺序
   * @param operationId 工序Id
   * @param workCenterId 工作中心Id
   * @param setupTime 准备工时（分钟）
   * @param runTime 加工工时（分钟）
   * @param queueTime 排队工时（分钟）
   * @param rateId 费率卡Id（可空，为空表示这道工序不计加工成本）
   * @param timeFormula 工时取时公式编码
   * @param standardBatchSize 标准批量（可空）
   * @param isOutsourced 是否外协
   * @param isOptional 是否可选工序
   * @param reworkOperationId 返工回到的工序Id（可空）
   * @param remark 备注（可空）
   * @throws DomainException 已发布或已作废、路线工序不存在、工序顺序重复时抛出
   */
  public void modifyOperation(
      long routingOperationId,
      int sequence,
      long operationId,
      long workCenterId,
      BigDecimal setupTime,
      BigDecimal runTime,
      BigDecimal queueTime,
      @Nullable Long rateId,
      String timeFormula,
      @Nullable BigDecimal standardBatchSize,
      boolean isOutsourced,
      boolean isOptional,
      @Nullable Long reworkOperationId,
      @Nullable String remark) {
    checkDraft();
    RoutingOperation target = findOperation(routingOperationId);
    if (target == null) {
      throw new DomainException("路线工序不存在!");
    }
    if (isExistSequence(sequence, routingOperationId)) {
      throw new DomainException("工序顺序" + sequence + "已存在!");
    }
    target.modify(
        sequence,
        operationId,
        workCenterId,
        setupTime,
        runTime,
        queueTime,
        rateId,
        timeFormula,
        standardBatchSize,
        isOutsourced,
        isOptional,
        reworkOperationId,
        remark);
  }

  /**
   * 删除工序（逻辑删除）
   *
   * @param routingOperationId 路线工序Id
   * @throws DomainException 已发布或已作废时抛出
   */
  public void deleteOperation(long routingOperationId) {
    checkDraft();
    RoutingOperation target = findOperation(routingOperationId);
    if (target == null) {
      return;
    }
    target.delete();
  }

  // ==================== 版本与生效链 ====================

  /**
   * 发布：草稿 → 已发布
   *
   * <p>发布即冻结：这一版的工序与工时从此不能再动，要改走 {@code RoutingDomainService.revise} 升版。
   *
   * @throws DomainException 不是草稿状态、没有工序、或没有生效时间时抛出
   */
  public void publish() {
    if (!isDraft()) {
      throw new DomainException("只有草稿状态的工艺路线才能发布");
    }
    if (routingOperations.stream().noneMatch(o -> !o.getIsDeleted())) {
      throw new DomainException("工艺路线" + this.code + "没有工序，不能发布");
    }
    if (this.effectiveFrom == null) {
      throw new DomainException("工艺路线版本" + this.version + "没有生效时间，不能发布");
    }
    this.status = RoutingStatusEnum.RELEASED.getCode();
  }

  /**
   * 作废：已发布 → 已作废
   *
   * <p>作废只影响新工单，已按此路线快照出工序的工单不受影响。
   *
   * @throws DomainException 不是已发布状态时抛出
   */
  public void obsolete() {
    if (!isReleased()) {
      throw new DomainException("只有已发布状态的工艺路线才能作废");
    }
    this.status = RoutingStatusEnum.OBSOLETE.getCode();
  }

  /**
   * 收口生效区间：发布下一版时把本版的失效时间挪到下一版生效点
   *
   * <p>只有已发布的版本参与接力，收口不改状态——它仍是历史记录，只是不再覆盖之后的时点。
   *
   * @param moment 新的失效时间（下一版的生效时间）
   * @throws DomainException 不是已发布状态、或收口时间不晚于本版生效时间时抛出
   */
  public void closeAt(LocalDateTime moment) {
    if (!isReleased()) {
      throw new DomainException("只有已发布状态的工艺路线才能收口生效区间");
    }
    if (this.effectiveFrom != null && !moment.isAfter(this.effectiveFrom)) {
      throw new DomainException("失效时间必须晚于生效时间");
    }
    this.effectiveTo = moment;
  }

  /**
   * 是否草稿
   *
   * @return true：草稿，可以改工序
   */
  public boolean isDraft() {
    return RoutingStatusEnum.DRAFT.getCode().equals(this.status);
  }

  /**
   * 是否已发布
   *
   * @return true：已发布（含已收口的历史版本）
   */
  public boolean isReleased() {
    return RoutingStatusEnum.RELEASED.getCode().equals(this.status);
  }

  /**
   * 判断指定时点本版路线是否生效
   *
   * <p>生效区间左闭右开：{@code effectiveFrom <= moment < effectiveTo}，失效时间为空表示无限期。 未发布的版本对任何时点都不生效。
   *
   * @param moment 判定时点（由调用方传入，领域层不取当前时间）
   * @return true：该时点可以用这一版下达工单
   */
  public boolean isEffectiveAt(LocalDateTime moment) {
    if (!isReleased()) {
      return false;
    }
    if (this.effectiveFrom == null || this.effectiveFrom.isAfter(moment)) {
      return false;
    }
    return this.effectiveTo == null || moment.isBefore(this.effectiveTo);
  }

  /**
   * 判断生效区间是否与给定区间重叠
   *
   * <p>两个左闭右开区间有交集即算重叠；开区间端（{@code null}）视为无穷。同一代码的已发布版本必须 互不重叠，否则「按时点取版本」会取出两条，下达时无法确定用哪一版。
   *
   * @param from 待比较区间的生效时间（可空，空视为无起始）
   * @param to 待比较区间的失效时间（可空，空视为无限期）
   * @return true：区间重叠
   */
  public boolean overlaps(@Nullable LocalDateTime from, @Nullable LocalDateTime to) {
    if (this.effectiveFrom != null && to != null && !this.effectiveFrom.isBefore(to)) {
      return false;
    }
    return from == null || this.effectiveTo == null || from.isBefore(this.effectiveTo);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 已发布或已作废的版本不许再改
   *
   * <p>在制工单是按某一版路线快照出工序的，改这一版等于改写已经下达的生产事实。
   *
   * @throws DomainException 状态不是草稿时抛出
   */
  private void checkDraft() {
    if (!isDraft()) {
      throw new DomainException("工艺路线版本" + this.version + "已不是草稿，不能修改，请升版");
    }
  }

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>状态不在这里赋值：它由 {@link #publish()} 与 {@link #obsolete()} 推进，不接受外部入参， 否则导入或改表头就能把已发布版本悄悄退回草稿。
   *
   * @param materialId 物料系列Id（SPU）
   * @param code 路线代码
   * @param name 路线名称
   * @param version 版本号
   * @param isPrimary 是否主路线
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   */
  private void apply(
      long materialId,
      String code,
      String name,
      String version,
      boolean isPrimary,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    String trimmedVersion = StringUtils.trimToNull(version);
    if (materialId <= 0) {
      throw new DomainException("工艺路线必须挂在物料系列下");
    }
    if (trimmedCode == null) {
      throw new DomainException("工艺路线代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("工艺路线名称不能为空");
    }
    if (trimmedVersion == null) {
      throw new DomainException("版本号不能为空");
    }
    if (effectiveFrom != null && effectiveTo != null && !effectiveTo.isAfter(effectiveFrom)) {
      throw new DomainException("失效时间必须晚于生效时间");
    }

    this.materialId = materialId;
    this.code = trimmedCode;
    this.name = trimmedName;
    this.version = trimmedVersion;
    this.isPrimary = isPrimary;
    this.effectiveFrom = effectiveFrom;
    this.effectiveTo = effectiveTo;
    this.remark = remark;
  }

  /**
   * 在路线工序列表中按 Id 查找未删除的工序
   *
   * @param routingOperationId 路线工序Id
   * @return 匹配到的工序，找不到返回 {@code null}
   */
  @Nullable
  private RoutingOperation findOperation(long routingOperationId) {
    return routingOperations.stream()
        .filter(o -> Objects.equals(o.getId(), routingOperationId) && !o.getIsDeleted())
        .findFirst()
        .orElse(null);
  }

  /**
   * 判断工序顺序是否已被占用
   *
   * <p>工序顺序在单条路线内必须唯一；修改工序时需排除自身，否则会把「保持原顺序」误判为冲突。
   *
   * @param sequence 待校验的工序顺序
   * @param excludeId 需要排除的路线工序Id（新增时传 {@code null}）
   * @return true：已被其它未删除的工序占用
   */
  private boolean isExistSequence(int sequence, @Nullable Long excludeId) {
    return routingOperations.stream()
        .filter(o -> !o.getIsDeleted())
        .filter(o -> !Objects.equals(o.getId(), excludeId))
        .anyMatch(o -> Objects.equals(o.getSequence(), sequence));
  }
}
