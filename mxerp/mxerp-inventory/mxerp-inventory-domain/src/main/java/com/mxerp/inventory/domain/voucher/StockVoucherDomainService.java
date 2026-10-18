package com.mxerp.inventory.domain.voucher;

import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Objects;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 库存单据领域服务
 *
 * <p>开单只认来源单据：一张库存单据必须说得清是哪张业务单让它动的，仓库不会自己动账。红冲是唯一能凭空开出一张
 * 单据的动作，而它也只能照着已过账的原单开——挑原单上的行，照它的位置，数量取负，方向照抄原单，冲多少由开单
 * 的人定。
 *
 * <p>库存加减不在这里：过账这一步按行调 {@code StockDomainService} 入库或出库，由应用层编排，本模块只保证
 * 单据自己那半落定。红字单也一样，开出来先是草稿：过不过得去账取决于仓里还有没有货，过不去就让它停在草稿挂着，
 * 等人先去处理下游那张单。
 */
@NullMarked
public class StockVoucherDomainService {

  /** 库存单据仓储 */
  private final StockVoucherRepository stockVoucherRepository;

  /**
   * 构造器注入
   *
   * @param stockVoucherRepository 库存单据仓储
   */
  public StockVoucherDomainService(StockVoucherRepository stockVoucherRepository) {
    this.stockVoucherRepository = stockVoucherRepository;
  }

  /**
   * 按来源单据开一张库存单据（草稿）
   *
   * <p>不接单号入参：这张单永远由系统按来源业务单生成，不存在从别处导进来一张带号的库存单据的情况。
   *
   * @param sourceType 来源单据类型编码
   * @param sourceId 来源单据Id
   * @param sourceNumber 来源单据号（快照）
   * @param direction 出入方向编码（整张单一个方向）
   * @param operatorId 经办人Id（可空）
   * @param remark 备注（可空）
   * @param lines 库存单据行数据
   * @return 库存单据聚合
   */
  public StockVoucher create(
      String sourceType,
      Long sourceId,
      String sourceNumber,
      String direction,
      @Nullable Long operatorId,
      @Nullable String remark,
      List<StockVoucherLineData> lines) {
    // 传空串：单号交给 OrderAddedEvent 的监听器分配
    StockVoucher model =
        new StockVoucher(
            sourceType, sourceId, sourceNumber, direction, operatorId, remark, "", null);
    for (StockVoucherLineData data : lines) {
      model.addLine(
          data.spuId(),
          data.skuId(),
          data.quantity(),
          data.warehouseId(),
          data.locationId(),
          data.remark());
    }
    return model;
  }

  /**
   * 过账：单据转入已过账
   *
   * <p>按行加减库存由应用层在同一个事务里完成，本方法只把单据落定。
   *
   * @param id 库存单据Id
   * @param time 过账时间
   * @return 库存单据聚合
   * @throws DomainException 单据不存在、已过账或已作废、或单内没有行时抛出
   */
  public StockVoucher post(long id, LocalDateTime time) {
    StockVoucher model = getExist(id);
    model.post(time);
    return model;
  }

  /**
   * 红冲：照已过账的原单开一张红字单，只冲指定的几行几量
   *
   * <p>原单不动：它当初真的动过库存，改它等于把这笔账抹掉。红字单挑原单上的行、照它的位置、数量取负、方向照抄
   * 原单、指回原单。一次只冲一部分也行——同一张原单能开好几张红字单，今天冲 20、过两天再冲 10。
   *
   * <p>护栏只有一条：同一位置累计冲掉的不超过原单当初记的量。算「已冲多少」时，已经开出来的红字单都算数
   * （草稿也算、作废的不算）：草稿虽然还没搬货，可它已经把这部分额度占住了；不占的话两张草稿各冲满额，过账时
   * 谁也拦不住——仓里别的批次有货，第二张会去偷别人的账。
   *
   * <p>红字单本身不能再红冲：它当初就是往回冲的，要纠正它得另开一张正向单。
   *
   * @param id 被冲的原单Id
   * @param operatorId 经办人Id（可空）
   * @param remark 备注（可空，写冲销原因）
   * @param lines 要冲掉的行与量（数量填正数）
   * @return 红字单（草稿，由调用方保存与过账）
   * @throws DomainException 原单不存在、还没过账、本身是红字单、没给行、行对不上原单、或累计冲超原量时抛出
   */
  public StockVoucher reverse(
      long id,
      @Nullable Long operatorId,
      @Nullable String remark,
      List<StockVoucherLineData> lines) {
    StockVoucher origin = getExist(id);
    if (!origin.isPosted()) {
      throw new DomainException("库存单据" + origin.getNumber() + "还没过账，作废就行，不用红冲");
    }
    if (origin.isReversal()) {
      throw new DomainException("库存单据" + origin.getNumber() + "本身就是红字单，不能再红冲");
    }
    if (lines.isEmpty()) {
      throw new DomainException("红字单得有行：说清冲原单的哪几行、各冲多少");
    }
    List<StockVoucher> reversals =
        stockVoucherRepository.getListByReversalOfVoucherId(origin.getId());
    StockVoucher model =
        new StockVoucher(
            origin.getSourceType(),
            origin.getSourceId(),
            origin.getSourceNumber(),
            origin.getDirection(),
            operatorId,
            remark,
            "",
            origin.getId());
    for (StockVoucherLineData data : lines) {
      if (data.quantity().signum() <= 0) {
        throw new DomainException("冲多少要填正数，负号由单子自己落");
      }
      StockVoucherLine originLine = getOriginLine(origin, data);
      BigDecimal remained = getRemainedQuantity(originLine, reversals);
      if (data.quantity().compareTo(remained) > 0) {
        throw new DomainException(
            "物料" + data.skuId() + "在原单上只剩" + remained + "能冲，填了" + data.quantity());
      }
      model.addLine(
          data.spuId(),
          data.skuId(),
          data.quantity().negate(),
          data.warehouseId(),
          data.locationId(),
          data.remark());
    }
    return model;
  }

  /**
   * 作废：草稿不用了撕掉
   *
   * @param id 库存单据Id
   * @return 库存单据聚合
   * @throws DomainException 单据不存在、已过账或已作废时抛出
   */
  public StockVoucher cancel(long id) {
    StockVoucher model = getExist(id);
    model.cancel();
    return model;
  }

  // ==================== 私有 ====================

  /**
   * 在原单里找同位置的那一行
   *
   * @param origin 被冲的原单
   * @param data 要冲的行
   * @return 原单上的行
   * @throws DomainException 原单上没有这一行时抛出
   */
  private StockVoucherLine getOriginLine(StockVoucher origin, StockVoucherLineData data) {
    for (StockVoucherLine line : origin.getStockVoucherLines()) {
      if (isSamePosition(line, data.skuId(), data.warehouseId(), data.locationId())) {
        return line;
      }
    }
    throw new DomainException(
        "原单上没有物料" + data.skuId() + "在仓库" + data.warehouseId() + "的这一行，冲不了");
  }

  /**
   * 算原单某一行还剩多少能冲
   *
   * @param originLine 原单上的行
   * @param reversals 该原单名下已开出来的红字单（含草稿，含已作废）
   * @return 还能冲掉的量
   */
  private BigDecimal getRemainedQuantity(
      StockVoucherLine originLine, List<StockVoucher> reversals) {
    BigDecimal reversed = BigDecimal.ZERO;
    for (StockVoucher reversal : reversals) {
      if (reversal.isVoided()) {
        continue;
      }
      for (StockVoucherLine line : reversal.getStockVoucherLines()) {
        if (isSamePosition(line, originLine)) {
          reversed = reversed.add(line.getQuantity());
        }
      }
    }
    return originLine.getQuantity().add(reversed);
  }

  /**
   * 比位置：物料、仓库、库位三项都对上，才算同一行
   *
   * @param line 待比的行
   * @param skuId 物料SKU
   * @param warehouseId 仓库Id
   * @param locationId 库位Id（可空）
   * @return true 表示同一位置
   */
  private boolean isSamePosition(
      StockVoucherLine line, Long skuId, Long warehouseId, @Nullable Long locationId) {
    return Objects.equals(line.getSkuId(), skuId)
        && Objects.equals(line.getWarehouseId(), warehouseId)
        && Objects.equals(line.getLocationId(), locationId);
  }

  /**
   * 比位置：另一行与本行是不是同一个位置
   *
   * @param line 待比的行
   * @param other 另一行
   * @return true 表示同一位置
   */
  private boolean isSamePosition(StockVoucherLine line, StockVoucherLine other) {
    return isSamePosition(line, other.getSkuId(), other.getWarehouseId(), other.getLocationId());
  }

  /**
   * 取库存单据，不存在则抛异常
   *
   * @param id 库存单据Id
   * @return 库存单据聚合
   * @throws DomainException 库存单据不存在时抛出
   */
  private StockVoucher getExist(long id) {
    StockVoucher model = stockVoucherRepository.get(id);
    if (model == null) {
      throw new DomainException("库存单据" + id + "不存在!");
    }
    return model;
  }
}
