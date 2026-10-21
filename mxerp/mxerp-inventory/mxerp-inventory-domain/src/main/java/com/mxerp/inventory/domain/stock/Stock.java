package com.mxerp.inventory.domain.stock;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.EnumUtils;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 库存（一个SKU在一个仓位的某个库存状态上的在库快照）
 *
 * <p>一行回答的是「这个SKU，在这个仓库（可能再加一个库位）、这个库存状态上，现在有多少」：定位键是四列
 * {@code VariantId + WarehouseId + LocationId + StockStatus}，数量用SKU基本单位计，不另存单位列。{@code MaterialId}
 * 是SKU所属的SPU，只给「按系列汇总有多少货」这类查询当分组列用，不参与定位，也不是键的一部分。
 *
 * <p>库位这一维可空：没分拣到货架时数量只记到仓库，LocationId 留空；一旦要按位置拣货再落到具体库位。
 *
 * <p>两个数量各说一件事：{@code Quantity} 是「架上现在有多少」，{@code ReservedQuantity} 是「其中已经答应给
 * 某张单、还没发出的有多少」。后者是**派生的合计**——名记在领料行自己身上（批了多少、发没发），这边只是把
 * 没发完的加起来，为的是别每次查可用量都去翻所有未过账的领料单；随时能按领料行重算核对，对不上就是回写漏了。
 * 金蝶的即时库存锁库量、Odoo 的 {@code stock.quant.reserved_quantity}、ERPNext 的 {@code Bin.reserved_qty}
 * 都是这种"存着的派生数"。可用量不存，{@link #availableQuantity()} 现算。
 *
 * <p>状态走维度而不是走列或走仓：D365 把库存状态当存储维度组里的一维，金蝶把它放在数量行上、仓库只限定
 * 允许哪些状态。SAP 那种「非限制/质检/冻结」各占一列的写法，多一种状态就要加一列改一遍程序；靠建仓或建库位
 * 表达状态（Odoo 的 QC 区、Blocked 区），又会让物理位置和会计语义挤在同一个字段上，还会让报表里的可用量
 * 变成一份仓库白名单。见 {@link StockStatusEnum}。
 *
 * <p>数量只能通过入库、出库、预留、取消预留四个动作变动，没有直接改数的入口：库存要能被对账，就必须保证
 * 每次变动都有明确的业务动作对应。表上没有唯一索引兜底（SQL Server 的唯一索引把 NULL 当值，同键下只留得下一条 LocationId
 * 为空的行），一键一行由 {@code StockDomainService} 先查后建保证。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Stock",
    indexes = {
      @Index(
          name = "IX_Stock_VariantId_WarehouseId_StockStatus",
          columnList = "VariantId,WarehouseId,StockStatus")
    })
public class Stock extends UpdateAggregateRoot {

  /** SPU（汇总维度，不参与定位键） */
  @Column(name = "MaterialId", nullable = false)
  private Long materialId;

  /** SKU */
  @Column(name = "VariantId", nullable = false)
  private Long variantId;

  /** 仓库Id */
  @Column(name = "WarehouseId", nullable = false)
  private Long warehouseId;

  /** 库位Id（可空，空表示数量只记到仓库这一层） */
  @Column(name = "LocationId")
  @Nullable
  private Long locationId;

  /** 库存状态编码（定位键的一维，见 {@link StockStatusEnum}） */
  @Nationalized
  @Column(name = "StockStatus", nullable = false, length = 50)
  private String stockStatus;

  /** 在库数量（SKU基本单位） */
  @Column(name = "Quantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal quantity;

  /** 预留数量（已答应给某张需求单、还没发出的部分，是未发完需求量的合计） */
  @Column(name = "ReservedQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal reservedQuantity;

  /**
   * 构造器，新建的行数量为零，之后由入库动作累加
   *
   * @param materialId SKU所属SPU（汇总维度）
   * @param variantId SKU
   * @param warehouseId 仓库Id
   * @param locationId 库位Id（可空）
   * @param stockStatus 库存状态编码
   * @throws DomainException SPU或SKU或仓库不合法、库位Id非法、库存状态不在枚举内时抛出
   */
  Stock(
      long materialId,
      long variantId,
      long warehouseId,
      @Nullable Long locationId,
      String stockStatus) {
    if (materialId <= 0) {
      throw new DomainException("SPU不能为空");
    }
    if (variantId <= 0) {
      throw new DomainException("SKU不能为空");
    }
    if (warehouseId <= 0) {
      throw new DomainException("仓库不能为空");
    }
    if (locationId != null && locationId <= 0) {
      throw new DomainException("库位" + locationId + "有误!");
    }
    String trimmedStatus = StringUtils.trimToNull(stockStatus);
    if (trimmedStatus == null || EnumUtils.fromCode(StockStatusEnum.class, trimmedStatus) == null) {
      throw new DomainException("库存状态" + stockStatus + "有误!");
    }

    this.materialId = materialId;
    this.variantId = variantId;
    this.warehouseId = warehouseId;
    this.locationId = locationId;
    this.stockStatus = trimmedStatus;
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
   * 出库：在库数量减少
   *
   * <p>闸是两道：先判架上够不够（缺货），再判可用量够不够（抢单）——可用量 = 在库 − 已答应别人的部分。
   * 例：架上 100，工单批了 30 还没发，别人来领 90 就当场抛错，而不是等发完才发现那 30 是空的。
   *
   * <p>要领自己那张单批下来的货，**先 {@link #unreserve} 把这笔预留退掉、再出库**，别在出库里顺手核销：
   * 库存行只有一个预留合计，不知道这 30 是谁占的，出库自动扣就会扣到别人头上（BC 的预留登记、SAP 的
   * {@code RESB} 提领、Odoo 让 move line 自己带 {@code reserved_qty}，都把"释放占用"当成出库前的独立一步）。
   *
   * <p>「不够能不能照样发」按仓放行（金蝶的「按仓库控制负库存」）：允许负库存的<b>可用行</b>跳过这两道闸，
   * 在库数变负、等入库补回——「先领后补」只发生在能领的货上。待检、冻结、废品那些行没有这个说法，照样卡死。
   *
   * @param quantity 出库数量，必须为正数
   * @param allowNegativeStock 该仓是否允许负库存出库
   * @throws DomainException 出库量不为正数，或在库 / 可用不足且该仓不允许负库存时抛出
   */
  public void outbound(BigDecimal quantity, boolean allowNegativeStock) {
    validatePositive("出库数量", quantity);
    if (!allowNegativeStock || !isAvailable()) {
      validateEnough(quantity);
    }
    this.quantity = this.quantity.subtract(quantity);
  }

  /**
   * 预留：把可用量占住（领料单批准、出库单分配时调）
   *
   * <p>只有可用那一行能被占。状态管「能不能动用」，预留管「答应给谁」：待检、冻结、废品的数量谈不上
   * 答应给谁，那些行的预留恒为零。
   *
   * @param quantity 预留数量，必须为正数
   * @throws DomainException 不是可用状态、数量不为正数、或可用量不足时抛出
   */
  public void reserve(BigDecimal quantity) {
    validateAvailableForReservation();
    validatePositive("预留数量", quantity);
    if (availableQuantity().compareTo(quantity) < 0) {
      throw new DomainException("可用库存不足，当前" + availableQuantity() + "，不能预留" + quantity);
    }
    this.reservedQuantity = this.reservedQuantity.add(quantity);
  }

  /**
   * 取消预留：把先前答应的数量放回可用量（发料过账、单据作废、批准量调小时调）
   *
   * @param quantity 取消预留数量，必须为正数
   * @throws DomainException 不是可用状态、数量不为正数、或超过已预留时抛出
   */
  public void unreserve(BigDecimal quantity) {
    validateAvailableForReservation();
    validatePositive("取消预留数量", quantity);
    if (this.reservedQuantity.compareTo(quantity) < 0) {
      throw new DomainException(
          "预留数量不足，当前" + this.reservedQuantity + "，不能取消预留" + quantity);
    }
    this.reservedQuantity = this.reservedQuantity.subtract(quantity);
  }

  // ==================== 查询 ====================

  /**
   * 这一行是不是可用状态
   *
   * @return true：可用，能领、能发、能进计划；待检、冻结、废品都不行
   */
  public boolean isAvailable() {
    return StockStatusEnum.AVAILABLE.getCode().equals(this.stockStatus);
  }

  /**
   * 可用量：在库数量减预留数量
   *
   * <p>非可用行预留恒为零，算出来就是在库数——那不是「能领走的量」，那一格根本没领的资格；要知道全仓
   * 还能动多少，得按可用状态过滤后再汇总。
   *
   * @return 还能被预留或领用的数量
   */
  public BigDecimal availableQuantity() {
    return quantity.subtract(reservedQuantity);
  }

  // ==================== 私有：校验 ====================

  /**
   * 校验架上和可用量够不够发
   *
   * <p>两种不足分开报：架上不够是缺货，架上够但已答应别人是抢单，处理办法完全不同。
   *
   * @param quantity 出库数量
   * @throws DomainException 任何一种不够时抛出
   */
  private void validateEnough(BigDecimal quantity) {
    if (this.quantity.compareTo(quantity) < 0) {
      throw new DomainException("在库数量不足，当前" + this.quantity + "，不能出库" + quantity);
    }
    BigDecimal available = availableQuantity();
    if (quantity.compareTo(available) > 0) {
      throw new DomainException(
          "可用数量不足，当前"
              + available
              + "（在库"
              + this.quantity
              + "，其中"
              + this.reservedQuantity
              + "已答应给别的单据还没发），不能出库"
              + quantity);
    }
  }

  /**
   * 校验这一行有资格被预留或取消预留
   *
   * @throws DomainException 不是可用状态时抛出
   */
  private void validateAvailableForReservation() {
    if (!isAvailable()) {
      throw new DomainException(
          "库存状态" + this.stockStatus + "的数量不能动用，谈不上预留或取消预留，要先转成可用");
    }
  }

  /**
   * 校验变动数量必须为正数  /**
   * 校验变动数量必须为正数
   *
   * <p>入库与出库各自决定加还是减，入参只允许正数，避免「传个负数做出库」把方向搞混。
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
