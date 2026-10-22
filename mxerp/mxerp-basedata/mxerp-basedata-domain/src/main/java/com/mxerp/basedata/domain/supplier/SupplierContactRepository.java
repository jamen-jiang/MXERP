package com.mxerp.basedata.domain.supplier;

import com.mxerp.shared.domain.repository.Repository;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 供应商联系人仓储接口 */
@NullMarked
public interface SupplierContactRepository extends Repository<SupplierContact> {

  /**
   * 按供应商查询全部联系人
   *
   * @param supplierId 供应商Id
   * @return 联系人列表
   */
  List<SupplierContact> getListBySupplierId(long supplierId);

  /**
   * 按供应商 + 姓名查询联系人
   *
   * @param supplierId 供应商Id
   * @param name 联系人姓名
   * @return 联系人（可空），不存在返回 null
   */
  @Nullable SupplierContact getBySupplierIdAndName(long supplierId, String name);

  /**
   * 判断同一供应商下联系人姓名是否已存在
   *
   * @param supplierId 供应商Id
   * @param name 联系人姓名
   * @param id 排除的联系人Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistBySupplierIdAndNameExcludeId(long supplierId, String name, @Nullable Long id);

  /**
   * 判断同一供应商下联系人姓名是否已存在
   *
   * @param supplierId 供应商Id
   * @param name 联系人姓名
   * @return true 表示已存在
   */
  default boolean isExistBySupplierIdAndNameExcludeId(long supplierId, String name) {
    return isExistBySupplierIdAndNameExcludeId(supplierId, name, null);
  }
}
