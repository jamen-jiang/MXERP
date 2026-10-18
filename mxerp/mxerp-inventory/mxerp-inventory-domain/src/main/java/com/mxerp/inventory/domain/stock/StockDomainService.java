package com.mxerp.inventory.domain.stock;

import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 库存领域服务，负责按键定位库存行并驱动数量变动 */
@NullMarked
public class StockDomainService {

  /** 库存仓储 */
  private final StockRepository stockRepository;

  /**
   * 构造器注入
   *
   * @param stockRepository 库存仓储
   */
  public StockDomainService(StockRepository stockRepository) {
    this.stockRepository = stockRepository;
  }

  /**
   * 入库：没有库存行就先建一行零数量，再加在库数（采购收货、完工入库、盘盈）
   *
   * <p>只有入库会建行：一键一行没有唯一索引兜底（SQL Server 唯一索引把 NULL 当值），建行前先查一次，
   * 让「先全量校验、再一次性装配」的规矩在库存这边同样成立。出库、预留不建行，没货就是没货，直接报错。
   *
   * @param spuId SKU所属SPU（只在新建行时写入，按系列汇总用）
   * @param skuId SKU
   * @param warehouseId 仓库Id
   * @param locationId 库位Id（可空，空表示数量记到仓库这一层）
   * @param quantity 入库数量
   * @return 库存行（新建的为未落库实体，由调用方保存）
   */
  public Stock inbound(
      Long spuId,
      Long skuId,
      Long warehouseId,
      @Nullable Long locationId,
      BigDecimal quantity) {
    Stock model = stockRepository.getByKey(skuId, warehouseId, locationId);
    if (model == null) {
      model = new Stock(spuId, skuId, warehouseId, locationId);
    }
    model.inbound(quantity);
    return model;
  }

  /**
   * 出库：扣在库数并核销已预留部分（领料、销售出库、盘亏）
   *
   * @param skuId SKU
   * @param warehouseId 仓库Id
   * @param locationId 库位Id（可空）
   * @param quantity 出库数量
   * @return 库存行
   * @throws DomainException 该键没有库存行、或在库数量不足时抛出
   */
  public Stock outbound(
      Long skuId, Long warehouseId, @Nullable Long locationId, BigDecimal quantity) {
    Stock model = getExist(skuId, warehouseId, locationId);
    model.outbound(quantity);
    return model;
  }

  /**
   * 预留：占用可用量（工单领料预占、出库单分配）
   *
   * @param skuId SKU
   * @param warehouseId 仓库Id
   * @param locationId 库位Id（可空）
   * @param quantity 预留数量
   * @return 库存行
   * @throws DomainException 该键没有库存行、或可用库存不足时抛出
   */
  public Stock reserve(
      Long skuId, Long warehouseId, @Nullable Long locationId, BigDecimal quantity) {
    Stock model = getExist(skuId, warehouseId, locationId);
    model.reserve(quantity);
    return model;
  }

  /**
   * 取消预留：把先前占住的数量放回可用量（单据作废、需求减少）
   *
   * @param skuId SKU
   * @param warehouseId 仓库Id
   * @param locationId 库位Id（可空）
   * @param quantity 取消预留数量
   * @return 库存行
   * @throws DomainException 该键没有库存行、或预留数量不足时抛出
   */
  public Stock unreserve(
      Long skuId, Long warehouseId, @Nullable Long locationId, BigDecimal quantity) {
    Stock model = getExist(skuId, warehouseId, locationId);
    model.unreserve(quantity);
    return model;
  }

  // ==================== 私有 ====================

  /**
   * 按键取库存行，不存在则抛异常
   *
   * @param skuId SKU
   * @param warehouseId 仓库Id
   * @param locationId 库位Id（可空）
   * @return 库存行
   * @throws DomainException 该键还没有库存记录时抛出
   */
  private Stock getExist(Long skuId, Long warehouseId, @Nullable Long locationId) {
    Stock model = stockRepository.getByKey(skuId, warehouseId, locationId);
    if (model == null) {
      throw new DomainException("SKU" + skuId + "在仓库" + warehouseId + "没有库存记录!");
    }
    return model;
  }
}
