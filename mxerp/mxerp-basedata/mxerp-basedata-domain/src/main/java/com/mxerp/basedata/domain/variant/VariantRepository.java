package com.mxerp.basedata.domain.variant;

import com.mxerp.shared.domain.repository.Repository;
import java.util.Collection;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** SKU 仓储 */
@NullMarked
public interface VariantRepository extends Repository<Variant> {
  /**
   * 按Id列表查询
   *
   * @param ids Id列表
   * @return Variant列表
   */
  List<Variant> getListByIds(Collection<Long> ids);

  /**
   * 按规格属性标识码查询
   *
   * @param attributeSignature 规格属性标识码
   * @return SKU（可空），不存在返回 null
   */
  @Nullable Variant getByAttributeSignature(String attributeSignature);

  /**
   * 按 ProductId 查询列表
   *
   * @param productId ProductId
   * @return SKU 列表
   */
  List<Variant> getListByProductId(long productId);

  /**
   * 判断规格属性标识码是否存在
   *
   * @param attributeSignature 规格属性标识码
   * @return true：存在
   */
  boolean isExistByAttributeSignature(String attributeSignature);

  /**
   * 判断 OPC 是否存在（排除指定 Id，用于修改时排除自身）
   *
   * @param productId ProductId
   * @param opc OPC
   * @param id 排除的 SKU Id
   * @return true：存在
   */
  boolean isExistByProductIdAndOpcExcludeId(long productId, String opc, @Nullable Long id);

  /**
   * 判断 OPC 是否存在
   *
   * @param productId ProductId
   * @param opc OPC
   * @return true：存在
   */
  default boolean isExistByProductIdAndOpcExcludeId(long productId, String opc) {
    return this.isExistByProductIdAndOpcExcludeId(productId, opc, null);
  }
}
