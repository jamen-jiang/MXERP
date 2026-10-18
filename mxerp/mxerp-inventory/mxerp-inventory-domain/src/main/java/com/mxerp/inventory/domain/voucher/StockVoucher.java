package com.mxerp.inventory.domain.voucher;

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
 * 库存单据（一次库存动账）
 *
 * <p>业务单据回答「为什么动」——哪张工单领料、哪张工单完工；这张单回答「动了什么」——哪个仓、哪个 SKU、多少、
 * 什么时候、谁办的。分开之后，仓里现在剩多少只是这些单据累出来的结果，{@code Stock} 那份当前数量不再是谁都能
 * 直接改的孤岛；库存也终于有流水可查，「这批料什么时候进的、谁动的」答得出来。
 *
 * <p>方向整张单一个，挂在头上：入库单就是货进仓，出库单就是货出仓。数量带符号，普通行只许正数，红字行取负，
 * 方向照旧。
 *
 * <p>撤销在这层走红冲，不是翻状态：过过账的原单从此冻住，照它开一张同方向、数量取负的红字单，用
 * {@code reversalOfVoucherId} 指回原单，两张加一块是零。原单上不另存「已红冲」——那和可用量一样是查得出来的
 * 事实，存一列迟早对不上。冲也分次：一张单今天冲 20、过两天再冲 10，红字单挑哪几行、各冲多少由开单的人定，
 * 只要同一原单上同一位置累计不超过它当初记的量。只有还没过账的草稿才配作废：它一分货没动，没有要冲的东西。
 *
 * <p>预留不在这里：预留只占住可用量，货没动，所以不留单据，{@code Stock} 上那两列就够。库存加减也不在这里：
 * 过账这一步的入库、出库由应用层调 {@code StockDomainService} 按行执行，本聚合只保证单据自身说得清。红字单
 * 过不去账（比如成品仓不够）就停在草稿挂着，等人先处理下游那张单，而不是凭空消失。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "StockVoucher",
    indexes = {
      @Index(name = "IX_StockVoucher_Number", columnList = "Number"),
      @Index(name = "IX_StockVoucher_SourceType_SourceId", columnList = "SourceType,SourceId"),
      @Index(name = "IX_StockVoucher_Status", columnList = "Status"),
      @Index(name = "IX_StockVoucher_ReversalOfVoucherId", columnList = "ReversalOfVoucherId")
    })
public class StockVoucher extends OrderAggregateRoot {

  /** 来源单据类型（编码取自流水号类型，如 ProductionIssue） */
  @Nationalized
  @Column(name = "SourceType", nullable = false, length = 50)
  private String sourceType;

  /** 来源单据Id */
  @Column(name = "SourceId", nullable = false)
  private Long sourceId;

  /** 来源单据号（快照，单据要能独立打印与核对） */
  @Nationalized
  @Column(name = "SourceNumber", nullable = false, length = 30)
  private String sourceNumber;

  /** 出入方向编码（整张单一个方向） */
  @Nationalized
  @Column(name = "Direction", nullable = false, length = 50)
  private String direction;

  /** 状态：草稿、已过账、已作废 */
  @Nationalized
  @Column(name = "Status", nullable = false, length = 50)
  private String status;

  /** 经办人Id（可空，发货或收料的人） */
  @Column(name = "OperatorId")
  @Nullable
  private Long operatorId;

  /** 过账时间（可空，转入已过账时写入） */
  @Column(name = "PostedTime")
  @Nullable
  private LocalDateTime postedTime;

  /** 被冲的原单Id（可空，非空即本单是红字单） */
  @Column(name = "ReversalOfVoucherId")
  @Nullable
  private Long reversalOfVoucherId;

  /** 备注（可空，红字单上写冲销原因） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /** 库存单据行 */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "StockVoucherId")
  @SQLRestriction("IsDeleted = 0")
  private List<StockVoucherLine> stockVoucherLines = new ArrayList<>();

  /**
   * 构造器
   *
   * @param sourceType 来源单据类型编码
   * @param sourceId 来源单据Id
   * @param sourceNumber 来源单据号（快照）
   * @param direction 出入方向编码（整张单一个方向）
   * @param operatorId 经办人Id（可空）
   * @param remark 备注（可空）
   * @param number 单号（传空串表示由 OrderAddedEvent 的监听器后续分配）
   * @param reversalOfVoucherId 被冲的原单Id（可空，普通单据传 null）
   */
  StockVoucher(
      String sourceType,
      Long sourceId,
      String sourceNumber,
      String direction,
      @Nullable Long operatorId,
      @Nullable String remark,
      String number,
      @Nullable Long reversalOfVoucherId) {
    super(number);
    apply(sourceType, sourceId, sourceNumber, direction, operatorId, remark);
    if (reversalOfVoucherId != null && reversalOfVoucherId <= 0) {
      throw new DomainException("红字单要指回被冲的原单!");
    }
    this.reversalOfVoucherId = reversalOfVoucherId;
    this.status = StockVoucherStatusEnum.DRAFT.getCode();
  }

  /**
   * 流水号类型
   *
   * @return 库存单据类型（红字单同用这一段号）
   */
  @Override
  public SerialNumberTypeEnum getSerialNumberType() {
    return SerialNumberTypeEnum.STOCK_VOUCHER;
  }

  /**
   * 获取库存单据行（只读）
   *
   * @return 不可变列表
   */
  public List<StockVoucherLine> getStockVoucherLines() {
    return Collections.unmodifiableList(stockVoucherLines);
  }

  /**
   * 添加库存单据行（仅草稿可加）
   *
   * <p>带符号数量只在这一个入口检查：负数等于「把一笔已过账的账倒回来」，所以只有红字单开得出来，普通单据要撤销
   * 得走红冲，不能自己填个负数；反过来红字单也不许填正数，它的量永远是往回冲的。开红字单的人只填「要冲掉多少」，
   * 那个负号由领域服务落。
   *
   * @param spuId 物料SPU（快照）
   * @param skuId 物料SKU
   * @param quantity 数量（普通单为正，红字单为负）
   * @param warehouseId 仓库Id
   * @param locationId 库位Id（可空）
   * @param remark 备注（可空）
   * @throws DomainException 非草稿状态、正负号与本单性质不符、或同一位置已在单内时抛出
   */
  public void addLine(
      Long spuId,
      Long skuId,
      BigDecimal quantity,
      Long warehouseId,
      @Nullable Long locationId,
      @Nullable String remark) {
    ensureDraft();
    if (this.reversalOfVoucherId == null && quantity.signum() < 0) {
      throw new DomainException("只有红字单能开负数数量，要撤销已过账的单请走红冲");
    }
    if (this.reversalOfVoucherId != null && quantity.signum() > 0) {
      throw new DomainException("红字单的数量要往回冲，正数请开普通单");
    }
    if (findLine(skuId, warehouseId, locationId) != null) {
      throw new DomainException("物料" + skuId + "在同一位置已在本单中!");
    }
    stockVoucherLines.add(
        new StockVoucherLine(spuId, skuId, quantity, warehouseId, locationId, remark));
  }

  /**
   * 过账：单据转入已过账并记下过账时间
   *
   * <p>按行加减库存那一下由应用层在同一个事务里调库存服务完成，本方法只把单据落定，避免同一张单重复动账。
   *
   * @param time 过账时间
   * @throws DomainException 非草稿状态、或单内没有行时抛出
   */
  public void post(LocalDateTime time) {
    ensureDraft();
    if (stockVoucherLines.isEmpty()) {
      throw new DomainException("库存单据没有行，不能过账");
    }
    this.status = StockVoucherStatusEnum.POSTED.getCode();
    this.postedTime = time;
  }

  /**
   * 作废：草稿不用了撕掉
   *
   * <p>只有草稿作废得动：过过账的已经动过库存，改状态改不回去库存，只能红冲。
   *
   * @throws DomainException 已过账或已作废时抛出
   */
  public void cancel() {
    if (isVoided()) {
      throw new DomainException("库存单据已作废，不能再作废");
    }
    if (isPosted()) {
      throw new DomainException("库存单据已过账，只能红冲，不能作废");
    }
    this.status = StockVoucherStatusEnum.VOIDED.getCode();
  }

  /**
   * 是否草稿
   *
   * @return true 表示草稿
   */
  public boolean isDraft() {
    return StockVoucherStatusEnum.DRAFT.getCode().equals(this.status);
  }

  /**
   * 是否已过账
   *
   * @return true 表示已过账
   */
  public boolean isPosted() {
    return StockVoucherStatusEnum.POSTED.getCode().equals(this.status);
  }

  /**
   * 是否已作废
   *
   * @return true 表示已作废
   */
  public boolean isVoided() {
    return StockVoucherStatusEnum.VOIDED.getCode().equals(this.status);
  }

  /**
   * 是否红字单
   *
   * @return true 表示本单是冲掉另一张单的红字单
   */
  public boolean isReversal() {
    return this.reversalOfVoucherId != null;
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口
   *
   * @param sourceType 来源单据类型编码
   * @param sourceId 来源单据Id
   * @param sourceNumber 来源单据号（快照）
   * @param direction 出入方向编码（整张单一个方向）
   * @param operatorId 经办人Id（可空）
   * @param remark 备注（可空）
   * @throws DomainException 来源单据类型、Id、号或方向非法时抛出
   */
  private void apply(
      String sourceType,
      Long sourceId,
      String sourceNumber,
      String direction,
      @Nullable Long operatorId,
      @Nullable String remark) {
    String trimmedSourceType = StringUtils.trimToEmpty(sourceType);
    if (EnumUtils.fromCode(SerialNumberTypeEnum.class, trimmedSourceType) == null) {
      throw new DomainException("来源单据类型" + trimmedSourceType + "有误!");
    }
    if (sourceId <= 0) {
      throw new DomainException("来源单据不能为空");
    }
    if (StringUtils.isBlank(sourceNumber)) {
      throw new DomainException("来源单据号不能为空");
    }
    String trimmedDirection = StringUtils.trimToEmpty(direction);
    if (EnumUtils.fromCode(StockVoucherDirectionEnum.class, trimmedDirection) == null) {
      throw new DomainException("方向" + trimmedDirection + "有误!");
    }
    this.sourceType = trimmedSourceType;
    this.sourceId = sourceId;
    this.sourceNumber = sourceNumber;
    this.direction = trimmedDirection;
    this.operatorId = operatorId;
    this.remark = remark;
  }

  /**
   * 校验单据仍处于草稿
   *
   * @throws DomainException 已过账或已作废时抛出
   */
  private void ensureDraft() {
    if (isVoided()) {
      throw new DomainException("库存单据已作废，不能再修改");
    }
    if (isPosted()) {
      throw new DomainException("库存单据已过账，不能再修改");
    }
  }

  /**
   * 按位置与方向查找未删除的行
   *
   * <p>同一张单里重复记同一个位置，等于把一笔账记两遍，过账时库存会多走一遍。
   *
   * @param skuId 物料SKU
   * @param warehouseId 仓库Id
   * @param locationId 库位Id（可空）
   * @return 匹配到的行，找不到返回 {@code null}
   */
  @Nullable
  private StockVoucherLine findLine(Long skuId, Long warehouseId, @Nullable Long locationId) {
    return stockVoucherLines.stream()
        .filter(
            l ->
                Objects.equals(l.getSkuId(), skuId)
                    && Objects.equals(l.getWarehouseId(), warehouseId)
                    && Objects.equals(l.getLocationId(), locationId))
        .findFirst()
        .orElse(null);
  }
}
