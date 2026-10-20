package com.mxerp.inventory.domain.warehouse;

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
 * 仓库
 *
 * <p>仓库是库存记账的边界：一笔数量落在哪个仓，决定它在报表上算哪一笔。仓库本身不带类型标签——需要「原料仓、
 * 车间仓、成品仓」这类区分时，就各建一个仓库，由引用它的那张单据来表达用途（领料单行上的发料仓、
 * 完工入库单上的收货仓都是这么用的；工单头上的 {@code WarehouseId} 只是入库单没填仓库时的默认收货仓）。
 * 这是 D365 的口径：仓库是库存维度之一，「usually a geographic location or a particular function」，功能靠建仓
 * 而不是靠打标签。
 *
 * <p>「这批数量能不能动用」不在仓库上表达，那是库存状态那一维的事（见 {@code StockStatusEnum}）：同一个仓里
 * 可以有可用、待检、冻结、废品四行。拿「待检仓」「废品仓」去顶这一维，仓的数量就会随状态翻倍，报表里的可用量
 * 还得维护一份仓库白名单。
 *
 * <p>库位挂在仓库下面，是仓内的物理坐标。这一层是可选粒度：入仓未分拣时数量只记到仓库，需要精确到
 * 货架再填库位。库位同样不带用途标签，理由和仓库一样：一个标签两层挂，就会出现「待检库位上的可用数量」这种
 * 两处说同一件事、还可能对不上的情况。
 *
 * <p>要不要用库位由仓库上的 {@code LocationManaged}（启用库位管理）决定，这是金蝶云星空/星辰的做法：
 * 开关在仓库资料上，勾了才允许建仓位、单据行上仓位变成必录；不勾就只记到仓库，行上也不许填仓位。
 * 开关放在仓上而不是放在单据或库存行上，是因为「这个仓有没有货架可管」是仓自己的事，同一个仓在不同
 * 单据里不该有不同答案。星空还有一条运维约束：仓库一旦被业务单据引用就不能再启用/取消仓位管理，官方给的
 * 办法是新建一个仓、开调拨单把库存移过去、再禁用旧仓——那条要查引用，属于应用层的活；本聚合只守住
 * 「仓里还有库位就不许关掉开关」这条不变式，免得关掉开关后留下一批没人认领的库位。
 *
 * <p>{@code AllowNegativeStock}（允许负库存）同样按金蝶的口径挂在仓库上：出库时数量不够，允许负库存的仓
 * 照样把账记下去（库存变负数，等入库补回），不允许的仓当场抛错。这个开关只能按仓定，不能全局定——
 * 原料仓、线边仓常常是「先领后补」，成品仓和客供料仓一负就是事故。BC、NetSuite 也都是按仓（或按物料×仓）
 * 给这个许可，没有一个ERP让你全局放开。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Warehouse",
    indexes = {
      @Index(name = "IX_Warehouse_Code", columnList = "Code"),
      @Index(name = "IX_Warehouse_Name", columnList = "Name"),
      @Index(name = "IX_Warehouse_Disabled", columnList = "Disabled")
      // 注意：Code 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class Warehouse extends UpdateAggregateRoot {

  /** 代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 是否启用库位管理 */
  @Column(name = "LocationManaged", nullable = false)
  private Boolean locationManaged;

  /** 是否允许负库存出库 */
  @Column(name = "AllowNegativeStock", nullable = false)
  private Boolean allowNegativeStock;

  /** 是否停用 */
  @Column(name = "Disabled", nullable = false)
  private Boolean disabled;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /** 库位列表 */
  @OneToMany(
      fetch = FetchType.EAGER,
      cascade = {CascadeType.PERSIST, CascadeType.MERGE},
      orphanRemoval = true)
  @JoinColumn(name = "WarehouseId")
  @SQLRestriction("IsDeleted = 0")
  private List<WarehouseLocation> locations = new ArrayList<>();

  /**
   * 获取库位列表（只读）
   *
   * @return 不可变列表
   */
  public List<WarehouseLocation> getLocations() {
    return Collections.unmodifiableList(locations);
  }

  /**
   * 构造器
   *
   * @param code 代码
   * @param name 名称
   * @param locationManaged 是否启用库位管理
   * @param allowNegativeStock 是否允许负库存出库
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  Warehouse(
      String code,
      String name,
      boolean locationManaged,
      boolean allowNegativeStock,
      boolean disabled,
      @Nullable String remark) {
    apply(code, name, locationManaged, allowNegativeStock, disabled, remark);
  }

  /**
   * 修改仓库
   *
   * @param code 代码
   * @param name 名称
   * @param locationManaged 是否启用库位管理
   * @param allowNegativeStock 是否允许负库存出库
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  void modify(
      String code,
      String name,
      boolean locationManaged,
      boolean allowNegativeStock,
      boolean disabled,
      @Nullable String remark) {
    apply(code, name, locationManaged, allowNegativeStock, disabled, remark);
  }

  /**
   * 校验一次数量变动上的库位填法：启用库位管理的仓必须有库位，没启用的不许填
   *
   * <p>开关在仓库上，答案就只能在仓库上问：单据行、库存行自己去猜「这个仓该不该有库位」会猜出两种口径。
   *
   * @param locationId 库位Id（可空）
   * @throws DomainException 启用库位管理却没给库位、或未启用却填了库位时抛出
   */
  public void validateLocationUsage(@Nullable Long locationId) {
    if (Boolean.TRUE.equals(locationManaged) && locationId == null) {
      throw new DomainException("仓库" + code + "已启用库位管理，库位不能为空!");
    }
    if (!Boolean.TRUE.equals(locationManaged) && locationId != null) {
      throw new DomainException("仓库" + code + "未启用库位管理，不能填库位" + locationId + "!");
    }
  }

  /**
   * 添加库位
   *
   * @param code 库位代码
   * @param name 库位名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @throws DomainException 本仓未启用库位管理、或库位代码在本仓已存在时抛出
   */
  public void addLocation(String code, String name, boolean disabled, @Nullable String remark) {
    if (!Boolean.TRUE.equals(locationManaged)) {
      throw new DomainException("仓库" + this.code + "未启用库位管理，不能建库位!");
    }
    if (isExistCode(code, null)) {
      throw new DomainException("库位代码" + code + "已存在!");
    }
    locations.add(new WarehouseLocation(code, name, disabled, remark));
  }

  /**
   * 修改库位
   *
   * @param locationId 库位Id
   * @param code 库位代码
   * @param name 库位名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @throws DomainException 库位不存在、或代码被本仓其它库位占用时抛出
   */
  public void modifyLocation(
      Long locationId, String code, String name, boolean disabled, @Nullable String remark) {
    WarehouseLocation target = findLocation(locationId);
    if (target == null) {
      throw new DomainException("库位不存在!");
    }
    if (isExistCode(code, locationId)) {
      throw new DomainException("库位代码" + code + "已存在!");
    }
    target.modify(code, name, disabled, remark);
  }

  /**
   * 删除库位（逻辑删除）
   *
   * <p>停用与删除的区别：停用是给「以后还会用」的库位挂起，删除是给录错的编码收口。有库存数量的库位
   * 不该删，那是台账的事，本版本不做引用校验。
   *
   * @param locationId 库位Id
   */
  public void deleteLocation(Long locationId) {
    WarehouseLocation target = findLocation(locationId);
    if (target == null) {
      return;
    }
    target.delete();
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * <p>把「去空格 → 非空校验 → 赋值」收敛到一处，避免构造器和 {@link #modify} 各写一遍导致规则漂移。
   *
   * @param code 代码
   * @param name 名称
   * @param locationManaged 是否启用库位管理
   * @param allowNegativeStock 是否允许负库存出库
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @throws DomainException 代码或名称为空、或取消库位管理但仓里还有库位时抛出
   */
  private void apply(
      String code,
      String name,
      boolean locationManaged,
      boolean allowNegativeStock,
      boolean disabled,
      @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("仓库代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("仓库名称不能为空");
    }
    // 关掉开关要先把库位收干净，否则留下一批没人认领的库位，单据上也填不了它们
    if (!locationManaged && hasActiveLocation()) {
      throw new DomainException("仓库" + trimmedCode + "还有库位，不能取消库位管理，请先删除库位!");
    }

    this.code = trimmedCode;
    this.name = trimmedName;
    this.locationManaged = locationManaged;
    this.allowNegativeStock = allowNegativeStock;
    this.disabled = disabled;
    this.remark = remark;
  }

  /**
   * 仓里是否还有未删除的库位
   *
   * @return true：有
   */
  private boolean hasActiveLocation() {
    return locations.stream().anyMatch(l -> !l.getIsDeleted());
  }

  /**
   * 按库位Id查找未删除的库位
   *
   * @param locationId 库位Id
   * @return 匹配到的库位，找不到返回 {@code null}
   */
  @Nullable
  private WarehouseLocation findLocation(Long locationId) {
    return locations.stream()
        .filter(l -> Objects.equals(l.getId(), locationId) && !l.getIsDeleted())
        .findFirst()
        .orElse(null);
  }

  /**
   * 判断库位代码是否已被本仓占用
   *
   * <p>代码只在仓内唯一：不同仓可以有同名库位，跨仓定位靠「仓库 + 库位」两个维度。
   *
   * @param code 待校验的库位代码
   * @param excludeId 需要排除的库位Id（新增时传 {@code null}）
   * @return true：已被其它未删除的库位占用
   */
  private boolean isExistCode(String code, @Nullable Long excludeId) {
    String trimmedCode = StringUtils.trimToNull(code);
    if (trimmedCode == null) {
      return false;
    }
    return locations.stream()
        .filter(l -> !l.getIsDeleted())
        .filter(l -> !Objects.equals(l.getId(), excludeId))
        .anyMatch(l -> trimmedCode.equals(l.getCode()));
  }
}
