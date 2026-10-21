package com.mxerp.inventory.domain.warehouse;

import com.mxerp.shared.domain.entity.BaseDeleteEntity;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 库位（仓内的物理存放位置）
 *
 * <p>一行就是一个能放东西的具体位置：A 排 3 号架、线边的一格托盘位、暂存区的一个格。它属于某个仓库，
 * 代码在仓内唯一，跨仓不重复，所以「WH01-A03」和「WH02-A03」是两个库位。
 *
 * <p>库位只回答「在哪儿」，不回答「算什么」：这批数量算不算可用，看它自己那一行的库存状态（见
 * {@code StockStatusEnum}），库位上不再挂用途标签，否则同一个事实会有两处说法。
 *
 * <p>库位是可选粒度：库存数量可以先只记到仓库这一层，需要精确到货架或要按位置拣货时再填库位。
 * 这里不保存数量，数量属于库存台账（{@code (variant, warehouse, location, stockStatus)}）。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "WarehouseLocation",
    indexes = {
      @Index(name = "IX_WarehouseLocation_WarehouseId", columnList = "WarehouseId"),
      @Index(name = "IX_WarehouseLocation_Code", columnList = "Code"),
      @Index(name = "IX_WarehouseLocation_Disabled", columnList = "Disabled")
      // 注意：WarehouseId + Code 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class WarehouseLocation extends BaseDeleteEntity {

  /** 仓库Id */
  @Column(name = "WarehouseId", nullable = false)
  private Long warehouseId;

  /** 库位代码（仓内唯一） */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 库位名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 是否停用 */
  @Column(name = "Disabled", nullable = false)
  private Boolean disabled;

/** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param code 库位代码
   * @param name 库位名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  WarehouseLocation(String code, String name, boolean disabled, @Nullable String remark) {
    apply(code, name, disabled, remark);
  }

  /**
   * 修改库位
   *
   * @param code 库位代码
   * @param name 库位名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  void modify(String code, String name, boolean disabled, @Nullable String remark) {
    apply(code, name, disabled, remark);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>把「去空格 → 非空校验 → 赋值」收敛到一处，避免构造器和 {@link #modify} 各写一遍导致规则漂移。
   *
   * @param code 库位代码
   * @param name 库位名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @throws DomainException 代码或名称为空时抛出
   */
  private void apply(String code, String name, boolean disabled, @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("库位代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("库位名称不能为空");
    }

    this.code = trimmedCode;
    this.name = trimmedName;
    this.disabled = disabled;
    this.remark = remark;
  }
}
