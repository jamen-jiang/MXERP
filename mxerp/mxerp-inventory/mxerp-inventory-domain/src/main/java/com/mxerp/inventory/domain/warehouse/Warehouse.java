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
 * <p>仓库是库存记账的边界：一笔数量落在哪个仓，决定它在报表上算什么。仓库本身不带类型标签——需要「原料仓、
 * 车间仓、成品仓、废品仓」这类区分时，就各建一个仓库，由引用它的那张单据来表达用途（领料单行上的发料仓、
 * 完工入库单上的收货仓都是这么用的；工单头上的 {@code WarehouseId} 只是入库单没填仓库时的默认收货仓）。
 * 这是 D365 的口径：仓库是库存维度之一，「usually a geographic location or a particular function」，功能靠建仓
 * 而不是靠打标签。
 *
 * <p>库位挂在仓库下面，是仓内的物理坐标。这一层是可选粒度：入仓未分拣时数量只记到仓库，需要精确到
 * 货架再填库位。库位同样不带用途标签，理由和仓库一样：一个标签两层挂，就会出现「废品仓里的报废库位」这种
 * 两处说同一件事、还可能对不上的情况。
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
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  Warehouse(String code, String name, boolean disabled, @Nullable String remark) {
    apply(code, name, disabled, remark);
  }

  /**
   * 修改仓库
   *
   * @param code 代码
   * @param name 名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  void modify(String code, String name, boolean disabled, @Nullable String remark) {
    apply(code, name, disabled, remark);
  }

  /**
   * 添加库位
   *
   * @param code 库位代码
   * @param name 库位名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @throws DomainException 库位代码在本仓已存在时抛出
   */
  public void addLocation(String code, String name, boolean disabled, @Nullable String remark) {
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
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @throws DomainException 代码或名称为空时抛出
   */
  private void apply(String code, String name, boolean disabled, @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("仓库代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("仓库名称不能为空");
    }

    this.code = trimmedCode;
    this.name = trimmedName;
    this.disabled = disabled;
    this.remark = remark;
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
