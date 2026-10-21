package com.mxerp.production.domain.bom;

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
 * 物料清单（BOM）
 *
 * <p>父件与子件都是 SKU，所以 BOM 本质是 SKU 对 SKU 的自关联：成品、毛坯、靶材、包装袋都挂在同一张
 * Variant 表上，谁有 BOM 谁就能被展开。多层 BOM 不需要额外字段——毛坯 SKU 自己再有一份 BOM 就是三层。
 *
 * <p>唯一键是 {@code variantId + version}，不用 {@code code}：BOM 是物料的附属数据，没有独立编码。
 *
 * <p>{@code materialId} 是父件所属的 SPU，由调用方随 SKU 一起提交，只当「按系列浏览 BOM」的分组列用。物料类型
 * 不在这里存：它记在 {@code Variant} 主数据上，BOM 再抄一份就成了两处说法，筛选时按 SPU 分组即可。
 *
 * <p><b>版本与生效链。</b>同一父件可以并存多个版本，用生效日期接力：状态只有三条路——草稿可随便改，
 * 发布后冻结（要改就 {@code revise} 升版，把行复制到新草稿），作废只退出可选池。已发布版本必须带生效时间，
 * 且同一父件的生效区间不许重叠；发布新版时，生效更早的那一版自动收口到新版的生效点，所以任一时刻至多
 * 只有一个生效版本，工单下达按「时点」取版本而不是按「当前状态」，补单和回溯才不会拿错配方。口径与
 * Dynamics 365 的 BOM version（Active/Previous + Effective from/to）一致。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Bom",
    indexes = {
      @Index(name = "IX_Bom_MaterialId", columnList = "MaterialId"),
      @Index(name = "IX_Bom_VariantId", columnList = "VariantId"),
      @Index(name = "IX_Bom_Status", columnList = "Status")
      // 注意：VariantId + Version 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class Bom extends UpdateAggregateRoot {

  /** 父件SPU（按系列查 BOM 用） */
  @Column(name = "MaterialId", nullable = false)
  private Long materialId;

  /** 父件SKU */
  @Column(name = "VariantId", nullable = false)
  private Long variantId;

  /** 版本号：与父件SKU 共同构成唯一键，升版就是换一个号 */
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

  /** 用料明细 */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "BomId")
  @SQLRestriction("IsDeleted = 0")
  private List<BomLine> bomLines = new ArrayList<>();

  /**
   * 获取用料明细（只读）
   *
   * @return 不可变列表
   */
  public List<BomLine> getBomLines() {
    return Collections.unmodifiableList(bomLines);
  }

  /**
   * 构造器：新建的 BOM 一律是草稿
   *
   * @param materialId 父件SPU
   * @param variantId 父件SKU
   * @param version 版本号
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   */
  Bom(
      long materialId,
      long variantId,
      String version,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    apply(materialId, variantId, version, effectiveFrom, effectiveTo, remark);
    this.status = BomStatusEnum.DRAFT.getCode();
  }

  /**
   * 修改 BOM 表头
   *
   * @param materialId 父件SPU
   * @param variantId 父件SKU
   * @param version 版本号
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @throws DomainException 已发布或已作废时抛出
   */
  void modify(
      long materialId,
      long variantId,
      String version,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    checkDraft();
    apply(materialId, variantId, version, effectiveFrom, effectiveTo, remark);
  }

  /**
   * 添加用料行
   *
   * @param sequence 行号
   * @param materialId 子件SPU
   * @param variantId 子件SKU
   * @param quantity 单位用量
   * @param lossRate 损耗率（百分比）
   * @param unitId 单位Id
   * @param issueMode 发料方式编码
   * @param issueTrigger 发料触发编码
   * @param warehouseId 发料仓库Id（可空）
   * @param isKeyPart 是否关键件
   * @param remark 备注（可空）
   * @throws DomainException 已发布或已作废、行号或子件重复时抛出
   */
  public void addLine(
      int sequence,
      long materialId,
      long variantId,
      BigDecimal quantity,
      BigDecimal lossRate,
      long unitId,
      String issueMode,
      String issueTrigger,
      @Nullable Long warehouseId,
      boolean isKeyPart,
      @Nullable String remark) {
    checkDraft();
    if (isExistSequence(sequence, null)) {
      throw new DomainException("BOM 行号" + sequence + "已存在!");
    }
    if (isExistComponent(variantId, null)) {
      throw new DomainException("子件已存在于当前 BOM 中!");
    }
    bomLines.add(
        new BomLine(
            sequence,
            materialId,
            variantId,
            quantity,
            lossRate,
            unitId,
            issueMode,
            issueTrigger,
            warehouseId,
            isKeyPart,
            remark));
  }

  /**
   * 修改用料行
   *
   * @param bomLineId BOM 行Id
   * @param sequence 行号
   * @param materialId 子件SPU
   * @param variantId 子件SKU
   * @param quantity 单位用量
   * @param lossRate 损耗率（百分比）
   * @param unitId 单位Id
   * @param issueMode 发料方式编码
   * @param issueTrigger 发料触发编码
   * @param warehouseId 发料仓库Id（可空）
   * @param isKeyPart 是否关键件
   * @param remark 备注（可空）
   * @throws DomainException 已发布或已作废、行不存在、行号或子件重复时抛出
   */
  public void modifyLine(
      long bomLineId,
      int sequence,
      long materialId,
      long variantId,
      BigDecimal quantity,
      BigDecimal lossRate,
      long unitId,
      String issueMode,
      String issueTrigger,
      @Nullable Long warehouseId,
      boolean isKeyPart,
      @Nullable String remark) {
    checkDraft();
    BomLine target = findLine(bomLineId);
    if (target == null) {
      throw new DomainException("BOM 行不存在!");
    }
    if (isExistSequence(sequence, bomLineId)) {
      throw new DomainException("BOM 行号" + sequence + "已存在!");
    }
    if (isExistComponent(variantId, bomLineId)) {
      throw new DomainException("子件已存在于当前 BOM 中!");
    }
    target.modify(
        sequence,
        materialId,
        variantId,
        quantity,
        lossRate,
        unitId,
        issueMode,
        issueTrigger,
        warehouseId,
        isKeyPart,
        remark);
  }

  /**
   * 删除用料行（逻辑删除）
   *
   * @param bomLineId BOM 行Id
   * @throws DomainException 已发布或已作废时抛出
   */
  public void deleteLine(long bomLineId) {
    checkDraft();
    BomLine target = findLine(bomLineId);
    if (target == null) {
      return;
    }
    target.delete();
  }

  /**
   * 按行Id查找未删除的用料行
   *
   * @param bomLineId BOM 行Id
   * @return 匹配到的用料行，找不到返回 {@code null}
   */
  @Nullable
  private BomLine findLine(long bomLineId) {
    return bomLines.stream()
        .filter(i -> Objects.equals(i.getId(), bomLineId) && !i.getIsDeleted())
        .findFirst()
        .orElse(null);
  }

  /**
   * 发布：草稿 → 已发布
   *
   * <p>发布即冻结：这一版的用料从此不能再动，要改走 {@code BomDomainService.revise} 升版。
   *
   * @throws DomainException 不是草稿状态、没有用料行、或没有生效时间时抛出
   */
  public void publish() {
    if (!isDraft()) {
      throw new DomainException("只有草稿状态的 BOM 才能发布");
    }
    if (bomLines.stream().noneMatch(i -> !i.getIsDeleted())) {
      throw new DomainException("BOM 没有用料行，不能发布");
    }
    if (this.effectiveFrom == null) {
      throw new DomainException("BOM 版本" + this.version + "没有生效时间，不能发布");
    }
    this.status = BomStatusEnum.RELEASED.getCode();
  }

  /**
   * 作废：已发布 → 已作废
   *
   * <p>作废只影响新工单，已按此 BOM 展开的工单用料不受影响。
   *
   * @throws DomainException 不是已发布状态时抛出
   */
  public void obsolete() {
    if (!isReleased()) {
      throw new DomainException("只有已发布状态的 BOM 才能作废");
    }
    this.status = BomStatusEnum.OBSOLETE.getCode();
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
      throw new DomainException("只有已发布状态的 BOM 才能收口生效区间");
    }
    if (this.effectiveFrom != null && !moment.isAfter(this.effectiveFrom)) {
      throw new DomainException("失效时间必须晚于生效时间");
    }
    this.effectiveTo = moment;
  }

  /**
   * 是否草稿
   *
   * @return true：草稿，可以改行
   */
  public boolean isDraft() {
    return BomStatusEnum.DRAFT.getCode().equals(this.status);
  }

  /**
   * 是否已发布
   *
   * @return true：已发布（含已收口的历史版本）
   */
  public boolean isReleased() {
    return BomStatusEnum.RELEASED.getCode().equals(this.status);
  }

  /**
   * 判断指定时点本版本是否生效
   *
   * <p>生效区间左闭右开：{@code effectiveFrom <= moment < effectiveTo}，失效时间为空表示无限期。
   * 未发布的版本对任何时点都不生效。
   *
   * @param moment 判定时点（由调用方传入，领域层不取当前时间）
   * @return true：该时点可以用这一版展开工单
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
   * <p>两个左闭右开区间有交集即算重叠；开区间端（{@code null}）视为无穷。同一父件的已发布版本必须
   * 互不重叠，否则「按时点取版本」会取出两条，领料和成本都对不上。
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
   * <p>版本化的意义就在这一步：在制的工单、已算出的成本要能回溯到当时那一版，改行等于改写历史。
   *
   * @throws DomainException 状态不是草稿时抛出
   */
  private void checkDraft() {
    if (!isDraft()) {
      throw new DomainException("BOM 版本" + this.version + "已不是草稿，不能修改，请升版");
    }
  }

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>状态不在这里赋值：它由 {@link #publish()} 与 {@link #obsolete()} 推进，不接受外部入参，
   * 否则导入或改表头就能把已发布版本悄悄退回草稿。
   *
   * @param materialId 父件SPU
   * @param variantId 父件SKU
   * @param version 版本号
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @throws DomainException SPU 或 SKU 为空、版本为空、生效区间倒挂时抛出
   */
  private void apply(
      long materialId,
      long variantId,
      String version,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    if (materialId <= 0) {
      throw new DomainException("父件SPU不能为空");
    }
    if (variantId <= 0) {
      throw new DomainException("父件不能为空");
    }
    String trimmedVersion = StringUtils.trimToNull(version);
    if (trimmedVersion == null) {
      throw new DomainException("版本号不能为空");
    }
    if (effectiveFrom != null && effectiveTo != null && !effectiveTo.isAfter(effectiveFrom)) {
      throw new DomainException("失效时间必须晚于生效时间");
    }

    this.materialId = materialId;
    this.variantId = variantId;
    this.version = trimmedVersion;
    this.effectiveFrom = effectiveFrom;
    this.effectiveTo = effectiveTo;
    this.remark = remark;
  }

  /**
   * 判断行号是否已被占用
   *
   * <p>行号在单张 BOM 内必须唯一；修改时需排除自身，否则会把「保持原行号」误判为冲突。
   *
   * @param sequence 待校验的行号
   * @param excludeId 需要排除的 BOM 行Id（新增时传 {@code null}）
   * @return true：已被其它未删除的行占用
   */
  private boolean isExistSequence(int sequence, @Nullable Long excludeId) {
    return bomLines.stream()
        .filter(i -> !i.getIsDeleted())
        .filter(i -> !Objects.equals(i.getId(), excludeId))
        .anyMatch(i -> Objects.equals(i.getSequence(), sequence));
  }

  /**
   * 判断子件是否已存在于当前 BOM
   *
   * <p>同一子件在同一张 BOM 里只能出现一次，否则展开成工单用料会出现两条同 SKU 的记录，领料时无法区分。
   *
   * @param variantId 待校验的子件SKU
   * @param excludeId 需要排除的 BOM 行Id（新增时传 {@code null}）
   * @return true：已存在
   */
  private boolean isExistComponent(long variantId, @Nullable Long excludeId) {
    return bomLines.stream()
        .filter(i -> !i.getIsDeleted())
        .filter(i -> !Objects.equals(i.getId(), excludeId))
        .anyMatch(i -> Objects.equals(i.getVariantId(), variantId));
  }
}
