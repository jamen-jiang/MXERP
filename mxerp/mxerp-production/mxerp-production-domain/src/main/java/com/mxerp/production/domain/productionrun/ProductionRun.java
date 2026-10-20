package com.mxerp.production.domain.productionrun;

import com.mxerp.shared.domain.aggregate.OrderAggregateRoot;
import com.mxerp.shared.domain.enums.SerialNumberTypeEnum;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.EnumUtils;
import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.Index;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToMany;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import java.math.RoundingMode;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Objects;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.hibernate.annotations.Nationalized;
import org.hibernate.annotations.SQLRestriction;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 炉号单：一次物理加工的成本归集对象
 *
 * <p>一张单就是「一炉、一缸、一车挂具」这一次物理加工，它只干一件事：把这一炉整体发生的工时与费用，按各行
 * 的占用量摊回涉及的工单。<b>工单照旧自己报工、自己记产出</b>，这张单不参与执行，也不改工单上的任何口径。
 *
 * <p><b>为什么要这么一张单。</b>《企业产品成本核算制度（试行）》（财会〔2013〕17号）第9条允许把产品性质相近的
 * 几张订单并成一批归集费用，第36条要求制造费用按机器工时、人工工时、计划分配率这类标准分——并归并、分分摊，总账
 * 上需要一个东西承接这一炉的数，否则炉次成本、装载率损失、事后的成本还原都没有落点。金蝶星空没有这种跨工单的
 * 运行单，靠批号（钢铁的炉号、染色的缸号）当载体；我们把炉号独立成单，是为了让它在成本口径上可查、可追、可红冲，
 * 而不只是编号规则里的一个串。
 *
 * <p><b>为什么不做成工单合并。</b>星瀚那种「生产工单合并单」是把原单关闭、生成一张新单，代价是原单从此不能按
 * 单追溯，而且只允许同组织同产品合并——一炉做不同 SKU 的现实场景直接装不进去。这里反过来：炉号单引用工单的
 * 工序，原单一张都不动。
 *
 * <p><b>为什么这么薄。</b>容量上限、相容规则、装具档位、介质寿命、配方链这些都能长，但它们是排产与工艺的范围，
 * 不是成本归集的前提。先只做归集这一件事，等排产真要按容量配炉时再往主数据上长，别的路径不受影响。
 *
 * <p><b>摊回算法只有一条：</b>本行分到的 = 本炉总数 × 本行权重 ÷ 权重合计，保留 2 位小数，除不尽的零头给权重
 * 最大的那一行。零头必须有人接，否则各行加起来比总数少两分，成本表跟总账对不上，而这种分差在几十炉的月账里天天有。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "ProductionRun",
    indexes = {
      @Index(name = "IX_ProductionRun_Number", columnList = "Number"),
      @Index(name = "IX_ProductionRun_OperationId", columnList = "OperationId"),
      @Index(name = "IX_ProductionRun_WorkCenterId", columnList = "WorkCenterId"),
      @Index(name = "IX_ProductionRun_Status", columnList = "Status"),
      @Index(name = "IX_ProductionRun_RunTime", columnList = "RunTime")
    })
public class ProductionRun extends OrderAggregateRoot {

  /** 工序Id：这一炉做的是哪道工序，全部行必须属于这道工序 */
  @Column(name = "OperationId", nullable = false)
  private Long operationId;

  /** 工作中心Id：哪台设备/哪条线，炉次统计按它分组 */
  @Column(name = "WorkCenterId", nullable = false)
  private Long workCenterId;

  /** 分配标准：这一炉的工时与费用按什么口径分给各行 */
  @Nationalized
  @Column(name = "AllocationBasis", nullable = false, length = 50)
  private String allocationBasis;

  /** 占用量单位Id（可空）：行上的占用量按什么单位计，按系数分时可以不填 */
  @Column(name = "OccupyUnitId")
  @Nullable
  private Long occupyUnitId;

  /** 炉次时间（可空）：这一炉实际发生的时间，追溯与按日统计都按它找 */
  @Column(name = "RunTime")
  @Nullable
  private LocalDateTime runTime;

  /** 本炉实际工时（分钟，过账时给定） */
  @Column(name = "ActualMinutes", nullable = false, precision = 18, scale = 2)
  private BigDecimal actualMinutes;

  /** 本炉实际费用（元，过账时给定） */
  @Column(name = "ActualCost", nullable = false, precision = 18, scale = 2)
  private BigDecimal actualCost;

  /** 过账时间（可空） */
  @Column(name = "PostedTime")
  @Nullable
  private LocalDateTime postedTime;

  /** 状态 */
  @Nationalized
  @Column(name = "Status", nullable = false, length = 50)
  private String status;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /** 炉号单行：哪些工单的哪道工序进这一炉，各占多少 */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "RunId")
  @SQLRestriction("IsDeleted = 0")
  private List<ProductionRunLine> runLines = new ArrayList<>();

  /**
   * 构造器：新建的炉号单一律是草稿，本炉的工时与费用还没发生，先记零
   *
   * @param operationId 工序Id
   * @param workCenterId 工作中心Id
   * @param allocationBasis 分配标准编码
   * @param occupyUnitId 占用量单位Id（可空）
   * @param runTime 炉次时间（可空）
   * @param remark 备注（可空）
   * @param number 炉号（传空串表示由 {@code OrderAddedEvent} 的监听器后续分配）
   */
  ProductionRun(
      Long operationId,
      Long workCenterId,
      String allocationBasis,
      @Nullable Long occupyUnitId,
      @Nullable LocalDateTime runTime,
      @Nullable String remark,
      String number) {
    super(number);
    apply(operationId, workCenterId, allocationBasis, occupyUnitId, runTime, remark);
    this.actualMinutes = BigDecimal.ZERO;
    this.actualCost = BigDecimal.ZERO;
    this.status = ProductionRunStatusEnum.DRAFT.getCode();
  }

  /**
   * 流水号类型
   *
   * @return 生产批次类型
   */
  @Override
  public SerialNumberTypeEnum getSerialNumberType() {
    return SerialNumberTypeEnum.PRODUCTION_RUN;
  }

  /**
   * 获取炉号单行（只读）
   *
   * @return 不可变列表
   */
  public List<ProductionRunLine> getRunLines() {
    return Collections.unmodifiableList(runLines);
  }

  // ==================== 草稿期：改结构与口径 ====================

  /**
   * 修改表头（仅草稿）
   *
   * @param operationId 工序Id
   * @param workCenterId 工作中心Id
   * @param allocationBasis 分配标准编码
   * @param occupyUnitId 占用量单位Id（可空）
   * @param runTime 炉次时间（可空）
   * @param remark 备注（可空）
   * @throws DomainException 已不是草稿时抛出
   */
  public void modify(
      Long operationId,
      Long workCenterId,
      String allocationBasis,
      @Nullable Long occupyUnitId,
      @Nullable LocalDateTime runTime,
      @Nullable String remark) {
    ensureDraft("修改炉号单");
    apply(operationId, workCenterId, allocationBasis, occupyUnitId, runTime, remark);
  }

  /**
   * 改分配标准与占用单位（仅草稿）
   *
   * <p>口径换了要把已录的占用量一起换算，那是人的判断，这里不代劳：只换表头两个字段，行上的数原样留着，
   * 改口径的人自己去把行核一遍。
   *
   * @param allocationBasis 分配标准编码
   * @param occupyUnitId 占用量单位Id（可空）
   * @throws DomainException 已不是草稿或标准非法时抛出
   */
  public void setAllocationBasis(String allocationBasis, @Nullable Long occupyUnitId) {
    ensureDraft("改分配标准");
    this.allocationBasis = requireBasis(allocationBasis).getCode();
    this.occupyUnitId = occupyUnitId;
  }

  /**
   * 加行：把某张工单的某道工序请进这一炉
   *
   * @param workOrderId 工单Id
   * @param workOrderOperationId 工单工序Id
   * @param workOrderNumber 工单号（快照）
   * @param productId 产出SPU
   * @param variantId 产出SKU
   * @param occupyQuantity 占用量（按表头占用单位计）
   * @param allocationWeight 分摊权重（可空，取占用量）
   * @param remark 备注（可空）
   * @throws DomainException 已不是草稿、或同一道工单工序已在本单中时抛出
   */
  public void addLine(
      Long workOrderId,
      Long workOrderOperationId,
      String workOrderNumber,
      Long productId,
      Long variantId,
      BigDecimal occupyQuantity,
      @Nullable BigDecimal allocationWeight,
      @Nullable String remark) {
    ensureDraft("加炉号单行");
    ProductionRunLine duplicate = findLineByOperation(workOrderOperationId);
    if (duplicate != null) {
      throw new DomainException(
          "工单" + duplicate.getWorkOrderNumber() + "的这道工序已在本炉中，不能重复装载");
    }
    runLines.add(
        new ProductionRunLine(
            workOrderId,
            workOrderOperationId,
            workOrderNumber,
            productId,
            variantId,
            occupyQuantity,
            allocationWeight,
            remark));
  }

  /**
   * 修改行的占用量与权重（仅草稿）
   *
   * @param runLineId 行Id
   * @param occupyQuantity 占用量
   * @param allocationWeight 分摊权重（可空，取占用量）
   * @param remark 备注（可空）
   * @throws DomainException 已不是草稿或行不存在时抛出
   */
  public void modifyLine(
      Long runLineId,
      BigDecimal occupyQuantity,
      @Nullable BigDecimal allocationWeight,
      @Nullable String remark) {
    ensureDraft("修改炉号单行");
    requireLine(runLineId).modify(occupyQuantity, allocationWeight, remark);
  }

  /**
   * 把某行从本炉提出（仅草稿）
   *
   * <p>软删而不是物理删：行删了那道工单工序就能进别的炉（过滤唯一索引只看未删的行），但这一行还是「当时打算
   * 这么合」的记录，事后查为什么这炉少了一张单要能翻到。
   *
   * @param runLineId 行Id
   * @throws DomainException 已不是草稿或行不存在时抛出
   */
  public void removeLine(Long runLineId) {
    ensureDraft("提出炉号单行");
    requireLine(runLineId).delete();
  }

  // ==================== 过账与撤销 ====================

  /**
   * 过账：登记本炉实际工时与费用，并按权重摊到各行
   *
   * <p>登记与摊回是同一个动作，不拆开：这张单存在的唯一意义就是把一炉的账落到行上，中间留一个「数已填、还没摊」
   * 的态，只会让人对着摊回前的行做判断。数录错了就红冲重过，改完的数自动重摊。
   *
   * @param moment 过账时间
   * @param actualMinutes 本炉实际工时（分钟，非负）
   * @param actualCost 本炉实际费用（元，非负）
   * @throws DomainException 不是草稿、没有行、权重合计为0或数值为负时抛出
   */
  public void post(LocalDateTime moment, BigDecimal actualMinutes, BigDecimal actualCost) {
    ensureDraft("过账");
    if (actualMinutes.signum() < 0 || actualCost.signum() < 0) {
      throw new DomainException("本炉工时与费用不能为负数");
    }
    List<ProductionRunLine> targets = liveLines();
    if (targets.isEmpty()) {
      throw new DomainException("炉号单没有工单行，不能过账");
    }
    BigDecimal totalWeight = totalAllocationWeight();
    if (totalWeight.signum() <= 0) {
      throw new DomainException("分摊权重合计必须大于0");
    }

    allocate(targets, totalWeight, actualMinutes, actualCost);
    this.actualMinutes = actualMinutes;
    this.actualCost = actualCost;
    this.status = ProductionRunStatusEnum.POSTED.getCode();
    this.postedTime = moment;
    if (this.runTime == null) {
      this.runTime = moment;
    }
    audit("过账", "炉号" + getNumber() + "摊回完成");
  }

  /**
   * 红冲：整炉的账撤销
   *
   * <p>过账是唯一需要对称撤销的动作。摊后数清零、行一并软删（那几道工序要能进下一炉），本炉的工时与费用留在
   * 表头当历史——红冲之后这张单还是「这一炉发生过什么」的证据，只是不再参与算账。
   *
   * @param moment 红冲时间
   * @throws DomainException 未过账时抛出
   */
  public void reverse(LocalDateTime moment) {
    if (!isPosted()) {
      throw new DomainException("只有已过账的炉号单才能红冲");
    }
    for (ProductionRunLine line : liveLines()) {
      line.clearAllocation();
      line.delete();
    }
    this.status = ProductionRunStatusEnum.REVERSED.getCode();
    this.postedTime = moment;
    audit("红冲", "炉号" + getNumber() + "整炉撤回");
  }

  /**
   * 取消：过账之前作废这一炉
   *
   * <p>没有数摊回去，作废只是解散组批，行软删释放那几道工序。过了账别走这里，走红冲——账已经落到工单口径上
   * 了，直接作废会让成本表和总账对不上。
   *
   * @throws DomainException 已过账或已作废时抛出
   */
  public void cancel() {
    if (!isDraft()) {
      throw new DomainException("炉号单" + getNumber() + "已过账或已作废，不能取消，请走红冲");
    }
    for (ProductionRunLine line : liveLines()) {
      line.delete();
    }
    this.status = ProductionRunStatusEnum.CANCELLED.getCode();
    audit("取消", null);
  }

  // ==================== 派生量与状态判断 ====================

  /**
   * 占用量合计（按表头占用单位）
   *
   * <p>这就是这一炉的分母。炉子能装多少是工艺与排产的事，这里只说实际占了多少——装载率低不体现在容量上，而
   * 体现在单件摊到的费用上，所以不必另设一套容量口径。
   *
   * @return 各有效行占用量之和
   */
  public BigDecimal totalOccupyQuantity() {
    return liveLines().stream()
        .map(ProductionRunLine::getOccupyQuantity)
        .reduce(BigDecimal.ZERO, BigDecimal::add);
  }

  /**
   * 分摊权重合计
   *
   * @return 各有效行权重之和
   */
  public BigDecimal totalAllocationWeight() {
    return liveLines().stream()
        .map(ProductionRunLine::getAllocationWeight)
        .reduce(BigDecimal.ZERO, BigDecimal::add);
  }

  /** 是否草稿：行与口径都还能改 */
  public boolean isDraft() {
    return ProductionRunStatusEnum.DRAFT.getCode().equals(this.status);
  }

  /** 是否已过账：这一炉的账已摊到各行 */
  public boolean isPosted() {
    return ProductionRunStatusEnum.POSTED.getCode().equals(this.status);
  }

  /** 是否已取消 */
  public boolean isCancelled() {
    return ProductionRunStatusEnum.CANCELLED.getCode().equals(this.status);
  }

  /** 是否已红冲 */
  public boolean isReversed() {
    return ProductionRunStatusEnum.REVERSED.getCode().equals(this.status);
  }

  // ==================== 私有 ====================

  /**
   * 未作废的行
   *
   * @return 参与摊回的行
   */
  private List<ProductionRunLine> liveLines() {
    return runLines.stream().filter(line -> !line.getIsDeleted()).toList();
  }

  /**
   * 摊回：按权重分本炉的工时与费用
   *
   * @param targets 参与摊回的行
   * @param totalWeight 权重合计
   * @param minutes 本炉实际工时（分钟）
   * @param cost 本炉实际费用（元）
   */
  private void allocate(
      List<ProductionRunLine> targets,
      BigDecimal totalWeight,
      BigDecimal minutes,
      BigDecimal cost) {
    ProductionRunLine biggest =
        targets.stream()
            .max((a, b) -> a.getAllocationWeight().compareTo(b.getAllocationWeight()))
            .orElseThrow(() -> new DomainException("炉号单没有工单行，不能过账"));
    for (ProductionRunLine line : targets) {
      if (line == biggest) {
        continue;
      }
      line.applyAllocation(
          share(minutes, line.getAllocationWeight(), totalWeight),
          share(cost, line.getAllocationWeight(), totalWeight));
    }
    // 零头给权重最大的一行：其余行各自四舍五入过，只有这里能保证合计与本炉数严格相等
    BigDecimal allocatedMinutes =
        targets.stream()
            .filter(line -> line != biggest)
            .map(ProductionRunLine::getAllocatedMinutes)
            .filter(Objects::nonNull)
            .reduce(BigDecimal.ZERO, BigDecimal::add);
    BigDecimal allocatedCost =
        targets.stream()
            .filter(line -> line != biggest)
            .map(ProductionRunLine::getAllocatedCost)
            .filter(Objects::nonNull)
            .reduce(BigDecimal.ZERO, BigDecimal::add);
    biggest.applyAllocation(minutes.subtract(allocatedMinutes), cost.subtract(allocatedCost));
  }

  /**
   * 单份份额：本炉数 × 权重 ÷ 权重合计，保留 2 位小数
   *
   * @param total 本炉数
   * @param weight 本行权重
   * @param totalWeight 权重合计
   * @return 摊到本行的数
   */
  private BigDecimal share(BigDecimal total, BigDecimal weight, BigDecimal totalWeight) {
    return total.multiply(weight).divide(totalWeight, 2, RoundingMode.HALF_UP);
  }

  /**
   * 结构动作的前置：只有草稿能改
   *
   * @param action 动作中文名，用于拼装错误提示
   * @throws DomainException 状态不是草稿时抛出
   */
  private void ensureDraft(String action) {
    if (!isDraft()) {
      throw new DomainException(
          "炉号单" + getNumber() + "已不是草稿，不能" + action + "，请红冲后重做");
    }
  }

  /**
   * 按行Id查找行，找不到抛异常
   *
   * @param runLineId 行Id
   * @return 炉号单行
   * @throws DomainException 行不存在时抛出
   */
  private ProductionRunLine requireLine(Long runLineId) {
    ProductionRunLine target =
        runLines.stream()
            .filter(i -> Objects.equals(i.getId(), runLineId) && !i.getIsDeleted())
            .findFirst()
            .orElse(null);
    if (target == null) {
      throw new DomainException("炉号单行" + runLineId + "不存在!");
    }
    return target;
  }

  /**
   * 按工单工序Id查找行：同一道工单工序至多存在于一个未作废的行里
   *
   * @param workOrderOperationId 工单工序Id
   * @return 匹配到的行，找不到返回 {@code null}
   */
  @Nullable
  private ProductionRunLine findLineByOperation(Long workOrderOperationId) {
    return runLines.stream()
        .filter(i -> !i.getIsDeleted())
        .filter(i -> Objects.equals(i.getWorkOrderOperationId(), workOrderOperationId))
        .findFirst()
        .orElse(null);
  }

  /**
   * 取分配标准枚举，非法则抛异常
   *
   * @param allocationBasis 分配标准编码
   * @return 分配标准
   * @throws DomainException 编码不在枚举内时抛出
   */
  private AllocationBasisEnum requireBasis(String allocationBasis) {
    AllocationBasisEnum basis = EnumUtils.fromCode(AllocationBasisEnum.class, allocationBasis);
    if (basis == null) {
      throw new DomainException("分配标准" + allocationBasis + "有误!");
    }
    return basis;
  }

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>状态与本炉的数不在这里赋值：状态由 {@link #post}、{@link #cancel}、{@link #reverse} 推进，工时与
   * 费用只有过账那一下才给——接受外部入参就能把已过账的一炉退回草稿，那种口子不能留。
   *
   * @param operationId 工序Id
   * @param workCenterId 工作中心Id
   * @param allocationBasis 分配标准编码
   * @param occupyUnitId 占用量单位Id（可空）
   * @param runTime 炉次时间（可空）
   * @param remark 备注（可空）
   * @throws DomainException 工序、设备非法或标准有误时抛出
   */
  private void apply(
      Long operationId,
      Long workCenterId,
      String allocationBasis,
      @Nullable Long occupyUnitId,
      @Nullable LocalDateTime runTime,
      @Nullable String remark) {
    if (operationId <= 0) {
      throw new DomainException("工序不能为空");
    }
    if (workCenterId <= 0) {
      throw new DomainException("工作中心不能为空");
    }
    if (occupyUnitId != null && occupyUnitId <= 0) {
      throw new DomainException("占用量单位有误");
    }
    this.allocationBasis = requireBasis(allocationBasis).getCode();

    this.operationId = operationId;
    this.workCenterId = workCenterId;
    this.occupyUnitId = occupyUnitId;
    this.runTime = runTime;
    this.remark = remark;
  }
}
