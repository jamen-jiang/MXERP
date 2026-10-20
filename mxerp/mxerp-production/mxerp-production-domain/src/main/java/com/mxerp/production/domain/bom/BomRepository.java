package com.mxerp.production.domain.bom;

import com.mxerp.shared.domain.repository.Repository;
import java.time.LocalDateTime;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** BOM 仓储接口 */
@NullMarked
public interface BomRepository extends Repository<Bom> {

  /**
   * 按父件SKU + 版本查询
   *
   * @param variantId 父件SKU
   * @param version 版本号
   * @return BOM（可空），不存在返回 null
   */
  @Nullable Bom getByVariantIdAndVersion(Long variantId, String version);

  /**
   * 按父件SKU查询全部版本
   *
   * @param variantId 父件SKU
   * @return BOM 列表
   */
  List<Bom> getListByVariantId(Long variantId);

  /**
   * 取父件在指定时点生效的 BOM 版本
   *
   * <p>工单下达按这一刻挑版本，而不是按「当前最新」：补一张上个月的生产单，应该拿到上个月那版配方。
   * 生效区间左闭右开，同一父件的已发布版本不重叠，所以至多命中一条；命中多条时按生效时间倒序取最近一版。
   *
   * @param variantId 父件SKU
   * @param moment 判定时点
   * @return BOM（可空），该时点没有生效版本时返回 null
   */
  @Nullable Bom getEffectiveByVariantIdAt(Long variantId, LocalDateTime moment);

  /**
   * 判断父件SKU + 版本是否存在
   *
   * @param variantId 父件SKU
   * @param version 版本号
   * @param id 排除的BOM Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistByVariantIdAndVersionExcludeId(Long variantId, String version, @Nullable Long id);

  /**
   * 判断父件SKU + 版本是否存在
   *
   * @param variantId 父件SKU
   * @param version 版本号
   * @return true 表示已存在
   */
  default boolean isExistByVariantIdAndVersionExcludeId(Long variantId, String version) {
    return isExistByVariantIdAndVersionExcludeId(variantId, version, null);
  }
}
