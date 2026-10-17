package com.mxerp.basedata.domain.sku;

import com.mxerp.shared.domain.repository.Repository;
import java.util.Collection;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** SKU 仓储 */
@NullMarked
public interface SkuRepository extends Repository<Sku> {
  /**
   * 按Id列表查询
   *
   * @param ids Id列表
   * @return Sku列表
   */
  List<Sku> getListByIds(Collection<Long> ids);

  /**
   * 按属性标识码查询
   *
   * @param attributesSignature 属性标识码
   * @return SKU（可空），不存在返回 null
   */
  @Nullable Sku getByAttributesSignature(String attributesSignature);

  /**
   * 按 SpuId 查询列表
   *
   * @param spuId SpuId
   * @return SKU 列表
   */
  List<Sku> getListBySpuId(long spuId);

  /**
   * 判断属性标识码是否存在
   *
   * @param attributesSignature 属性标识码
   * @return true：存在
   */
  boolean isExistByAttributesSignature(String attributesSignature);

  /**
   * 判断 OPC 是否存在（排除指定 Id，用于修改时排除自身）
   *
   * @param spuId SpuId
   * @param opc OPC
   * @param id 排除的 SKU Id
   * @return true：存在
   */
  boolean isExistBySpuIdAndOpcExcludeId(long spuId, String opc, @Nullable Long id);

  /**
   * 判断 OPC 是否存在
   *
   * @param spuId SpuId
   * @param opc OPC
   * @return true：存在
   */
  default boolean isExistBySpuIdAndOpcExcludeId(long spuId, String opc) {
    return this.isExistBySpuIdAndOpcExcludeId(spuId, opc, null);
  }
}
