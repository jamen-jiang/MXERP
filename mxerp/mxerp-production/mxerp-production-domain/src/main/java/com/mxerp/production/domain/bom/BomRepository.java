package com.mxerp.production.domain.bom;

import com.mxerp.shared.domain.repository.Repository;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** BOM 仓储接口 */
@NullMarked
public interface BomRepository extends Repository<Bom> {

  /**
   * 按父件SKU + 版本查询
   *
   * @param skuId 父件SKU
   * @param version 版本号
   * @return BOM（可空），不存在返回 null
   */
  @Nullable Bom getBySkuIdAndVersion(Long skuId, String version);

  /**
   * 按父件SKU查询全部版本
   *
   * @param skuId 父件SKU
   * @return BOM 列表
   */
  List<Bom> getListBySkuId(Long skuId);

  /**
   * 取父件当前生效的已发布 BOM
   *
   * <p>工单下达时用它挑默认 BOM：状态为已发布、且当前时间落在生效区间内，按生效时间倒序取第一条。
   *
   * @param skuId 父件SKU
   * @return BOM（可空），没有生效版本时返回 null
   */
  @Nullable Bom getReleasedBySkuId(Long skuId);

  /**
   * 判断父件SKU + 版本是否存在
   *
   * @param skuId 父件SKU
   * @param version 版本号
   * @param id 排除的BOM Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistBySkuIdAndVersionExcludeId(Long skuId, String version, @Nullable Long id);

  /**
   * 判断父件SKU + 版本是否存在
   *
   * @param skuId 父件SKU
   * @param version 版本号
   * @return true 表示已存在
   */
  default boolean isExistBySkuIdAndVersionExcludeId(Long skuId, String version) {
    return isExistBySkuIdAndVersionExcludeId(skuId, version, null);
  }
}
