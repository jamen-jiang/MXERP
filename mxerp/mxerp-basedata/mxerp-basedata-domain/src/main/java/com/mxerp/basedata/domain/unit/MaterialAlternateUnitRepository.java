package com.mxerp.basedata.domain.unit;

import com.mxerp.shared.domain.repository.Repository;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 物料替代单位仓储接口 */
@NullMarked
public interface MaterialAlternateUnitRepository extends Repository<MaterialAlternateUnit> {

  /**
   * 按物料查询全部换算关系
   *
   * @param materialId 物料Id
   * @return 换算关系列表
   */
  List<MaterialAlternateUnit> getListByMaterialId(long materialId);

  /**
   * 按物料 + 单位查询换算关系
   *
   * @param materialId 物料Id
   * @param unitId 替代单位Id
   * @return 换算关系（可空），不存在返回 null
   */
  @Nullable MaterialAlternateUnit getByMaterialIdAndUnitId(long materialId, long unitId);

  /**
   * 判断这个单位在该物料下是否已有换算关系
   *
   * @param materialId 物料Id
   * @param unitId 替代单位Id
   * @param id 排除的换算关系Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistByMaterialIdAndUnitIdExcludeId(
      long materialId, long unitId, @Nullable Long id);

  /**
   * 判断这个单位在该物料下是否已有换算关系（新增时校验）
   *
   * @param materialId 物料Id
   * @param unitId 替代单位Id
   * @return true 表示已存在
   */
  default boolean isExistByMaterialIdAndUnitIdExcludeId(long materialId, long unitId) {
    return isExistByMaterialIdAndUnitIdExcludeId(materialId, unitId, null);
  }
}
