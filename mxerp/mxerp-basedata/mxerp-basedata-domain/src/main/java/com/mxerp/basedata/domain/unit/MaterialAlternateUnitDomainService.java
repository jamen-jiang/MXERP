package com.mxerp.basedata.domain.unit;

import com.mxerp.basedata.domain.material.Material;
import com.mxerp.basedata.domain.material.MaterialRepository;
import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.math.RoundingMode;
import org.jspecify.annotations.NullMarked;

/**
 * 物料替代单位领域服务
 *
 * <p>把「这个物料从 A 单位换到 B 单位」收在一个入口：同组走 {@link Unit#convertTo}（组内换算率），
 * 跨组一律折两次——本单位 → 物料基本单位 → 目标单位，其中不属于物料基本单位所在组的那一段查
 * {@link MaterialAlternateUnit}（一个物料、一个单位、一个系数）。单据上只在行里存单位 Id 和数量，
 * 换算时调这里，不要各模块自己乘系数。
 *
 * <p>中间不落数，只在**目标单位**的步长上落一次；两次除法各保留 12 位。
 */
@NullMarked
public class MaterialAlternateUnitDomainService {

  /** 除法中间结果保留的小数位 */
  private static final int CALC_SCALE = 12;

  /** 物料替代单位工厂 */
  private final MaterialAlternateUnitFactory materialAlternateUnitFactory;

  /** 物料替代单位仓储 */
  private final MaterialAlternateUnitRepository materialAlternateUnitRepository;

  /** 物料仓储：换算要以物料基本单位为参照 */
  private final MaterialRepository materialRepository;

  /** 单位仓储 */
  private final UnitRepository unitRepository;

  /**
   * 构造器注入
   *
   * @param materialAlternateUnitFactory 物料替代单位工厂
   * @param materialAlternateUnitRepository 物料替代单位仓储
   * @param materialRepository 物料仓储
   * @param unitRepository 单位仓储
   */
  public MaterialAlternateUnitDomainService(
      MaterialAlternateUnitFactory materialAlternateUnitFactory,
      MaterialAlternateUnitRepository materialAlternateUnitRepository,
      MaterialRepository materialRepository,
      UnitRepository unitRepository) {
    this.materialAlternateUnitFactory = materialAlternateUnitFactory;
    this.materialAlternateUnitRepository = materialAlternateUnitRepository;
    this.materialRepository = materialRepository;
    this.unitRepository = unitRepository;
  }

  /**
   * 新建物料替代单位
   *
   * <p>只收**不属于物料基本单位所在组**的单位：同组的单位比值由 {@code Unit.factor} 决定，
   * 再存一条就成了两套数。
   *
   * @param materialId 物料Id
   * @param unitId 替代单位Id
   * @param factor 换算率（1 个该单位 = factor 个物料基本单位）
   * @return 物料替代单位聚合
   * @throws DomainException 物料或单位不存在、单位与基本单位同组、或该单位已有换算关系时抛出
   */
  public MaterialAlternateUnit create(long materialId, long unitId, BigDecimal factor) {
    Material material = requireMaterial(materialId);
    Unit unit = requireUnit(unitId);
    Unit baseUnit = requireUnit(material.getUnitId());
    requireOtherGroup(materialId, unit, baseUnit);
    return materialAlternateUnitFactory.create(materialId, unitId, factor);
  }

  /**
   * 导入生成：该单位有换算关系则改换算率，没有则新建
   *
   * @param materialId 物料Id
   * @param unitId 替代单位Id
   * @param factor 换算率（1 个该单位 = factor 个物料基本单位）
   * @return 物料替代单位聚合
   */
  public MaterialAlternateUnit importGenerate(
      long materialId, long unitId, BigDecimal factor) {
    // 按物料 + 单位查换算关系
    MaterialAlternateUnit model =
        materialAlternateUnitRepository.getByMaterialIdAndUnitId(materialId, unitId);

    // 不存在则创建，存在则修改
    if (model == null) {
      model = create(materialId, unitId, factor);
    } else {
      modify(model, factor);
    }

    return model;
  }

  /**
   * 修改换算率
   *
   * @param id 物料替代单位Id
   * @param factor 换算率（1 个该单位 = factor 个物料基本单位）
   * @return 物料替代单位聚合
   * @throws DomainException 换算关系不存在时抛出
   */
  public MaterialAlternateUnit modify(long id, BigDecimal factor) {
    MaterialAlternateUnit model = materialAlternateUnitRepository.get(id);
    if (model == null) {
      throw new DomainException("物料替代单位" + id + "不存在!");
    }
    modify(model, factor);
    return model;
  }

  /**
   * 修改换算率（使用已有聚合）
   *
   * @param model 物料替代单位聚合
   * @param factor 换算率（1 个该单位 = factor 个物料基本单位）
   */
  public void modify(MaterialAlternateUnit model, BigDecimal factor) {
    model.modify(factor);
  }

  /**
   * 数量换算
   *
   * <p>同组直接按单位换算率折；跨组先折到物料基本单位、再从它折出去。跨组那一段查不到换算关系就报错——
   * 不猜、不按单位名相近去凑，否则算出来的数量没人能解释。
   *
   * @param materialId 物料Id
   * @param fromUnitId 源单位Id
   * @param toUnitId 目标单位Id
   * @param quantity 原数量
   * @return 换算后的数量
   * @throws DomainException 物料或单位不存在、数量为负、或跨组但没有换算关系时抛出
   */
  public BigDecimal convert(
      long materialId, long fromUnitId, long toUnitId, BigDecimal quantity) {
    if (quantity.signum() < 0) {
      throw new DomainException("换算数量不能是负数，当前" + quantity);
    }
    if (fromUnitId == toUnitId) {
      return requireUnit(fromUnitId).roundQuantity(quantity);
    }

    Unit fromUnit = requireUnit(fromUnitId);
    Unit toUnit = requireUnit(toUnitId);

    // 同组：单位自己的换算率就够了
    if (sameGroup(fromUnit, toUnit)) {
      return fromUnit.convertTo(toUnit, quantity);
    }

    // 跨组：折到物料基本单位，再从它折出去
    Unit baseUnit = requireUnit(requireMaterial(materialId).getUnitId());
    BigDecimal inBase = toBase(materialId, fromUnitId, fromUnit, baseUnit, quantity);
    BigDecimal amount = fromBase(materialId, toUnitId, toUnit, baseUnit, inBase);
    return toUnit.roundQuantity(amount);
  }

  // ==================== 私有：折算 ====================

  /**
   * 折到物料基本单位（不落数）
   *
   * @param materialId 物料Id
   * @param unitId 源单位Id
   * @param unit 源单位
   * @param baseUnit 物料基本单位
   * @param quantity 源数量
   * @return 以物料基本单位计的数量
   */
  private BigDecimal toBase(
      long materialId, long unitId, Unit unit, Unit baseUnit, BigDecimal quantity) {
    if (sameGroup(unit, baseUnit)) {
      // 1 个本单位 = 基本单位的组换算率 ÷ 本单位的组换算率 个物料基本单位
      return quantity
          .multiply(baseUnit.getFactor())
          .divide(unit.getFactor(), CALC_SCALE, RoundingMode.HALF_UP);
    }
    return quantity.multiply(requireConversion(materialId, unitId, unit).getFactor());
  }

  /**
   * 从物料基本单位折出去（不落数）
   *
   * @param materialId 物料Id
   * @param unitId 目标单位Id
   * @param unit 目标单位
   * @param baseUnit 物料基本单位
   * @param quantity 以物料基本单位计的数量
   * @return 以目标单位计的数量
   */
  private BigDecimal fromBase(
      long materialId, long unitId, Unit unit, Unit baseUnit, BigDecimal quantity) {
    if (sameGroup(unit, baseUnit)) {
      return quantity
          .multiply(unit.getFactor())
          .divide(baseUnit.getFactor(), CALC_SCALE, RoundingMode.HALF_UP);
    }
    BigDecimal factor = requireConversion(materialId, unitId, unit).getFactor();
    return quantity.divide(factor, CALC_SCALE, RoundingMode.HALF_UP);
  }

  /**
   * 取该物料该单位的换算关系，没有则抛异常
   *
   * @param materialId 物料Id
   * @param unitId 单位Id
   * @param unit 单位（只为报错时带上代码）
   * @return 物料替代单位聚合
   * @throws DomainException 换算关系不存在时抛出
   */
  private MaterialAlternateUnit requireConversion(long materialId, long unitId, Unit unit) {
    MaterialAlternateUnit conversion =
        materialAlternateUnitRepository.getByMaterialIdAndUnitId(materialId, unitId);
    if (conversion == null) {
      throw new DomainException("物料" + materialId + "没有单位" + unit.getCode() + "的换算关系");
    }
    return conversion;
  }

  // ==================== 私有：校验 ====================

  /**
   * 取物料，不存在则抛异常
   *
   * @param materialId 物料Id
   * @return 物料
   * @throws DomainException 物料不存在时抛出
   */
  private Material requireMaterial(long materialId) {
    Material model = materialRepository.get(materialId);
    if (model == null) {
      throw new DomainException("物料" + materialId + "不存在!");
    }
    return model;
  }

  /**
   * 取单位，不存在则抛异常
   *
   * @param unitId 单位Id
   * @return 单位
   * @throws DomainException 单位不存在时抛出
   */
  private Unit requireUnit(long unitId) {
    Unit unit = unitRepository.get(unitId);
    if (unit == null) {
      throw new DomainException("单位" + unitId + "不存在!");
    }
    return unit;
  }

  /**
   * 校验替代单位不属于物料基本单位所在的那个组
   *
   * @param materialId 物料Id
   * @param unit 替代单位
   * @param baseUnit 物料基本单位
   * @throws DomainException 同组时抛出
   */
  private void requireOtherGroup(long materialId, Unit unit, Unit baseUnit) {
    if (sameGroup(unit, baseUnit)) {
      throw new DomainException(
          "单位" + unit.getCode() + "与物料" + materialId + "的基本单位" + baseUnit.getCode()
              + "在同一个单位组，组内换算由单位换算率决定，不需要物料替代单位");
    }
  }

  /**
   * 两个单位是否属于同一个单位组
   *
   * @param one 单位一
   * @param other 单位二
   * @return true 表示同组
   */
  private boolean sameGroup(Unit one, Unit other) {
    return one.getGroupId().equals(other.getGroupId());
  }
}
