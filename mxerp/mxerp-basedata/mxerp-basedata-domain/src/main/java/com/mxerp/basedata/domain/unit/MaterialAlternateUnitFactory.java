package com.mxerp.basedata.domain.unit;

import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;

/** 物料替代单位工厂，负责创建换算关系 */
@NullMarked
public class MaterialAlternateUnitFactory {

  /** 物料替代单位仓储 */
  private final MaterialAlternateUnitRepository materialAlternateUnitRepository;

  /**
   * 构造器注入
   *
   * @param materialAlternateUnitRepository 物料替代单位仓储
   */
  public MaterialAlternateUnitFactory(
      MaterialAlternateUnitRepository materialAlternateUnitRepository) {
    this.materialAlternateUnitRepository = materialAlternateUnitRepository;
  }

  /**
   * 创建物料替代单位
   *
   * @param materialId 物料Id
   * @param unitId 替代单位Id
   * @param factor 换算率（1 个该单位 = factor 个物料基本单位）
   * @return 物料替代单位聚合
   * @throws DomainException 该物料下这个单位已有换算关系时抛出
   */
  public MaterialAlternateUnit create(long materialId, long unitId, BigDecimal factor) {
    // 一个物料的一个单位只留一条，另一个方向靠它取倒数
    if (materialAlternateUnitRepository.isExistByMaterialIdAndUnitIdExcludeId(
        materialId, unitId)) {
      throw new DomainException("物料" + materialId + "已经有单位" + unitId + "的换算关系了");
    }
    return new MaterialAlternateUnit(materialId, unitId, factor);
  }
}
