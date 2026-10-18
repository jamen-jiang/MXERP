package com.mxerp.inventory.domain.stock;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 库存（一个SKU在一个仓位上的在库快照）
 *
 * <p>一行回答的是「这个SKU，在这个仓库（可能再加一个库位）上，现在有多少」：定位键是三列
 * {@code SkuId + WarehouseId + LocationId}，数量用SKU基本单位计，不另存单位列。{@code SpuId} 是SKU所属的
 * SPU，只给「按系列汇总有多少货」这类查询当分组列用，不参与定位，也不是键的一部分。
 *
 * <p>库位这一维可空：没分拣到货架时数量只记到仓库，LocationId 留空；一旦要按位置拣货再落到具体库位。
 * 「可用量」不是第三列而是 {@link #availableQuantity()} 派生的（在库数减预留数），存第三列迟早和两列对不上。
 * 待检、报废这类状态也不在这里分列：仓库和库位都不带类型标签，数量落在哪个仓、由哪张单据引用它，就是它的状态；
 * 再加一列状态，等于让同一个事实在两处各说一遍。
 *
 * <p>数量只能通过入库、出库、预留、取消预留四个动作变动，没有直接改数的入口：库存要能被对账，就必须保证每次变动
 * 都有明确的业务动作对应。表上没有唯一索引兜底（SQL Server 的唯一索引把 NULL 当值，同键下只留得下一条
 * LocationId 为空的行），一键一行由 {@code StockDomainService} 先查后建保证。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Stock",
    indexes = {@Index(name = "IX_Stock_SkuId_WarehouseId", columnList = "SkuId,WarehouseId")})
public class Stock extends UpdateAggregateRoot {

  /** SPU（汇总维度，不参与定位键） */
  @Column(name = "SpuId", nullable = false)
  private Long spuId;

  /** SKU */
  @Column(name = "SkuId", nullable = false)
  private Long skuId;

  /** 仓库Id */
  @Column(name = "WarehouseId", nullable = false)
  private Long warehouseId;

  /** 库位Id（可空，空表示数量只记到仓库这一层） */
  @Column(name = "LocationId")
  @Nullable
  private Long locationId;

  /** 在库数量（SKU基本单位） */
  @Column(name = "Quantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal quantity;

  /** 预留数量（已分配给领料或出库、但还没真正发出的部分） */
  @Column(name = "ReservedQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal reservedQuantity;

  /**
   * 构造器，新建的行数量为零，之后由入库、预留等动作累加
   *
   * @param spuId SKU所属SPU（汇总维度）
   * @param skuId SKU
   * @param warehouseId 仓库Id
   * @param locationId 库位Id（可空）
   * @throws DomainException SPU或SKU或仓库不合法、库位Id非法时抛出
   */
  Stock(Long spuId, Long skuId, Long warehouseId, @Nullable Long locationId) {
    if (spuId <= 0) {
      throw new DomainException("SPU不能为空");
    }
    if (skuId <= 0) {
      throw new DomainException("SKU不能为空");
    }
    if (warehouseId <= 0) {
      throw new DomainException("仓库不能为空");
    }
    if (locationId != null && locationId <= 0) {
      throw new DomainException("库位" + locationId + "有误!");
    }

    this.spuId = spuId;
    this.skuId = skuId;
    this.warehouseId = warehouseId;
    this.locationId = locationId;
    this.quantity = BigDecimal.ZERO;
    this.reservedQuantity = BigDecimal.ZERO;
  }

  // ==================== 数量变动 ====================

  /**
   * 入库：在库数量增加（采购收货、完工入库、盘盈都走这里）
   *
   * @param quantity 入库数量，必须为正数
   * @throws DomainException 数量不为正数时抛出
   */
  public void inbound(BigDecimal quantity) {
    validatePositive("入库数量", quantity);
    this.quantity = this.quantity.add(quantity);
  }

  /**
   * 出库：在库数量减少，已预留的部分随发出核销
   *
   * <p>发出的一批如果事先预留过，预留数要同时减掉，否则可用量会凭空多出来。超出预留的部分（比如没预留
   * 直接发）只扣在库数。
   *
   * @param quantity 出库数量，必须为正数
   * @throws DomainException 数量不为正数、或在库数量不足时抛出
   */
  public void outbound(BigDecimal quantity) {
    validatePositive("出库数量", quantity);
    if (this.quantity.compareTo(quantity) < 0) {
      throw new DomainException("在库数量不足，当前" + this.quantity + "，不能出库" + quantity);
    }
    this.quantity = this.quantity.subtract(quantity);
    this.reservedQuantity = this.reservedQuantity.subtract(quantity.min(this.reservedQuantity));
  }

  /**
   * 预留：把可用量占住，供后续领料或出库核销
   *
   * @param quantity 预留数量，必须为正数
   * @throws DomainException 数量不为正数、或可用量不足时抛出
   */
  public void reserve(BigDecimal quantity) {
    validatePositive("预留数量", quantity);
    if (availableQuantity().compareTo(quantity) < 0) {
      throw new DomainException("可用库存不足，当前" + availableQuantity() + "，不能预留" + quantity);
    }
    this.reservedQuantity = this.reservedQuantity.add(quantity);
  }

  /**
   * 取消预留：把先前占住的数量放回可用量（单据作废、需求减少时用）
   *
   * @param quantity 取消预留数量，必须为正数
   * @throws DomainException 数量不为正数、或超过已预留数量时抛出
   */
  public void unreserve(BigDecimal quantity) {
    validatePositive("取消预留数量", quantity);
    if (this.reservedQuantity.compareTo(quantity) < 0) {
      throw new DomainException("预留数量不足，当前" + this.reservedQuantity + "，不能取消预留" + quantity);
    }
    this.reservedQuantity = this.reservedQuantity.subtract(quantity);
  }

  // ==================== 查询 ====================

  /**
   * 可用量：在库数量减预留数量
   *
   * @return 还能被预留或领用的数量
   */
  public BigDecimal availableQuantity() {
    return quantity.subtract(reservedQuantity);
  }

  // ==================== 私有：校验 ====================

  /**
   * 校验变动数量必须为正数
   *
   * <p>四个动作各自决定加还是减，入参只允许正数，避免「传个负数做出库」把方向搞混，也不允许库存出现负数。
   *
   * @param field 数量字段中文名，用于拼装错误提示
   * @param value 待校验的数量
   * @throws DomainException 数量不为正数时抛出
   */
  private void validatePositive(String field, BigDecimal value) {
    if (value.signum() <= 0) {
      throw new DomainException(field + "必须大于零");
    }
  }
}
