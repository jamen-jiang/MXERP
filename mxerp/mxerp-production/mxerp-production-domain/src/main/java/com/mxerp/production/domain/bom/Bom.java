package com.mxerp.production.domain.bom;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
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
 * Sku 表上，谁有 BOM 谁就能被展开。多层 BOM 不需要额外字段——毛坯 SKU 自己再有一份 BOM 就是三层。
 *
 * <p>唯一键是 {@code skuId + version}，不用 {@code code}：BOM 是产品的附属数据，没有独立编码。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Bom",
    indexes = {
      @Index(name = "IX_Bom_SkuId", columnList = "SkuId"),
      @Index(name = "IX_Bom_Status", columnList = "Status")
      // 注意：SkuId + Version 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class Bom extends UpdateAggregateRoot {

  /** 父件SKU */
  @Column(name = "SkuId", nullable = false)
  private Long skuId;

  /** 版本号 */
  @Nationalized
  @Column(name = "Version", nullable = false, length = 50)
  private String version;

  /** 状态：草稿、已发布、已作废 */
  @Nationalized
  @Column(name = "Status", nullable = false, length = 50)
  private String status;

  /** 生效时间（可空） */
  @Column(name = "EffectiveFrom")
  @Nullable
  private LocalDateTime effectiveFrom;

  /** 失效时间（可空） */
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
  private List<BomItem> bomItems = new ArrayList<>();

  /**
   * 获取用料明细（只读）
   *
   * @return 不可变列表
   */
  public List<BomItem> getBomItems() {
    return Collections.unmodifiableList(bomItems);
  }

  /**
   * 构造器
   *
   * @param skuId 父件SKU
   * @param version 版本号
   * @param status 状态
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   */
  Bom(
      Long skuId,
      String version,
      String status,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    apply(skuId, version, status, effectiveFrom, effectiveTo, remark);
  }

  /**
   * 修改 BOM 表头
   *
   * @param skuId 父件SKU
   * @param version 版本号
   * @param status 状态
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   */
  void modify(
      Long skuId,
      String version,
      String status,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    apply(skuId, version, status, effectiveFrom, effectiveTo, remark);
  }

  /**
   * 添加用料行
   *
   * @param sequence 行号
   * @param componentSkuId 子件SKU
   * @param quantity 单位用量
   * @param lossRate 损耗率（百分比）
   * @param unitId 单位Id
   * @param issueMode 发料方式编码
   * @param isKeyPart 是否关键件
   * @param remark 备注（可空）
   * @throws DomainException 行号或子件重复时抛出
   */
  public void addItem(
      Integer sequence,
      Long componentSkuId,
      BigDecimal quantity,
      BigDecimal lossRate,
      Long unitId,
      String issueMode,
      boolean isKeyPart,
      @Nullable String remark) {
    if (isExistSequence(sequence, null)) {
      throw new DomainException("BOM 行号" + sequence + "已存在!");
    }
    if (isExistComponent(componentSkuId, null)) {
      throw new DomainException("子件已存在于当前 BOM 中!");
    }
    bomItems.add(
        new BomItem(sequence, componentSkuId, quantity, lossRate, unitId, issueMode, isKeyPart, remark));
  }

  /**
   * 修改用料行
   *
   * @param bomItemId BOM 行Id
   * @param sequence 行号
   * @param componentSkuId 子件SKU
   * @param quantity 单位用量
   * @param lossRate 损耗率（百分比）
   * @param unitId 单位Id
   * @param issueMode 发料方式编码
   * @param isKeyPart 是否关键件
   * @param remark 备注（可空）
   * @throws DomainException 行不存在、行号或子件重复时抛出
   */
  public void modifyItem(
      Long bomItemId,
      Integer sequence,
      Long componentSkuId,
      BigDecimal quantity,
      BigDecimal lossRate,
      Long unitId,
      String issueMode,
      boolean isKeyPart,
      @Nullable String remark) {
    BomItem target = findItem(bomItemId);
    if (target == null) {
      throw new DomainException("BOM 行不存在!");
    }
    if (isExistSequence(sequence, bomItemId)) {
      throw new DomainException("BOM 行号" + sequence + "已存在!");
    }
    if (isExistComponent(componentSkuId, bomItemId)) {
      throw new DomainException("子件已存在于当前 BOM 中!");
    }
    target.modify(sequence, componentSkuId, quantity, lossRate, unitId, issueMode, isKeyPart, remark);
  }

  /**
   * 删除用料行（逻辑删除）
   *
   * @param bomItemId BOM 行Id
   */
  public void deleteItem(Long bomItemId) {
    BomItem target = findItem(bomItemId);
    if (target == null) {
      return;
    }
    target.delete();
  }

  /**
   * 按行Id查找未删除的用料行
   *
   * @param bomItemId BOM 行Id
   * @return 匹配到的用料行，找不到返回 {@code null}
   */
  @Nullable
  public BomItem findItem(Long bomItemId) {
    return bomItems.stream()
        .filter(i -> Objects.equals(i.getId(), bomItemId) && !i.getIsDeleted())
        .findFirst()
        .orElse(null);
  }

  /**
   * 按子件SKU查找未删除的用料行
   *
   * @param componentSkuId 子件SKU
   * @return 匹配到的用料行，找不到返回 {@code null}
   */
  @Nullable
  public BomItem findItemByComponentSkuId(Long componentSkuId) {
    return bomItems.stream()
        .filter(i -> Objects.equals(i.getComponentSkuId(), componentSkuId) && !i.getIsDeleted())
        .findFirst()
        .orElse(null);
  }

  /**
   * 发布：草稿 → 已发布
   *
   * @throws DomainException 不是草稿状态、或没有任何用料行时抛出
   */
  public void publish() {
    if (!BomStatusEnum.DRAFT.getCode().equals(this.status)) {
      throw new DomainException("只有草稿状态的 BOM 才能发布");
    }
    if (bomItems.stream().noneMatch(i -> !i.getIsDeleted())) {
      throw new DomainException("BOM 没有用料行，不能发布");
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
    if (!BomStatusEnum.RELEASED.getCode().equals(this.status)) {
      throw new DomainException("只有已发布状态的 BOM 才能作废");
    }
    this.status = BomStatusEnum.OBSOLETE.getCode();
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * @param skuId 父件SKU
   * @param version 版本号
   * @param status 状态
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @throws DomainException 父件为空、版本为空、状态非法、生效区间倒挂时抛出
   */
  private void apply(
      Long skuId,
      String version,
      String status,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    if (skuId <= 0) {
      throw new DomainException("父件不能为空");
    }
    String trimmedVersion = StringUtils.trimToNull(version);
    String trimmedStatus = StringUtils.trimToNull(status);
    if (trimmedVersion == null) {
      throw new DomainException("版本号不能为空");
    }
    if (trimmedStatus == null) {
      throw new DomainException("状态不能为空");
    }
    if (EnumUtils.fromCode(BomStatusEnum.class, trimmedStatus) == null) {
      throw new DomainException("状态" + trimmedStatus + "有误!");
    }
    if (effectiveFrom != null && effectiveTo != null && effectiveFrom.isAfter(effectiveTo)) {
      throw new DomainException("生效时间不能晚于失效时间");
    }

    this.skuId = skuId;
    this.version = trimmedVersion;
    this.status = trimmedStatus;
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
  private boolean isExistSequence(Integer sequence, @Nullable Long excludeId) {
    return bomItems.stream()
        .filter(i -> !i.getIsDeleted())
        .filter(i -> !Objects.equals(i.getId(), excludeId))
        .anyMatch(i -> Objects.equals(i.getSequence(), sequence));
  }

  /**
   * 判断子件是否已存在于当前 BOM
   *
   * <p>同一子件在同一张 BOM 里只能出现一次，否则展开成工单用料会出现两条同 SKU 的记录，领料时无法区分。
   *
   * @param componentSkuId 待校验的子件SKU
   * @param excludeId 需要排除的 BOM 行Id（新增时传 {@code null}）
   * @return true：已存在
   */
  private boolean isExistComponent(Long componentSkuId, @Nullable Long excludeId) {
    return bomItems.stream()
        .filter(i -> !i.getIsDeleted())
        .filter(i -> !Objects.equals(i.getId(), excludeId))
        .anyMatch(i -> Objects.equals(i.getComponentSkuId(), componentSkuId));
  }
}
