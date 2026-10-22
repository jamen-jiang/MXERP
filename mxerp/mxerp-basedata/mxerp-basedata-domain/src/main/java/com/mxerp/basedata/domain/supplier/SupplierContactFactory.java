package com.mxerp.basedata.domain.supplier;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 供应商联系人工厂，负责创建联系人 */
@NullMarked
public class SupplierContactFactory {

  /** 供应商联系人仓储 */
  private final SupplierContactRepository supplierContactRepository;

  /**
   * 构造器注入
   *
   * @param supplierContactRepository 供应商联系人仓储
   */
  public SupplierContactFactory(SupplierContactRepository supplierContactRepository) {
    this.supplierContactRepository = supplierContactRepository;
  }

  /**
   * 创建联系人
   *
   * @param supplierId 供应商Id
   * @param name 姓名
   * @param telephone 固定电话（可空）
   * @param mobile 移动电话（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param sequence 排序（可空）
   * @return 联系人聚合
   * @throws DomainException 同一供应商下姓名重复时抛出
   */
  public SupplierContact create(
      long supplierId,
      String name,
      @Nullable String telephone,
      @Nullable String mobile,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Integer sequence) {
    checkUnique(supplierId, name, null);
    return new SupplierContact(supplierId, name, telephone, mobile, fax, email, sequence);
  }

  /**
   * 校验同一供应商下联系人姓名的唯一性
   *
   * @param supplierId 供应商Id
   * @param name 姓名
   * @param id 排除的联系人Id（可空，新增时传 null）
   * @throws DomainException 同一供应商下姓名重复时抛出
   */
  void checkUnique(long supplierId, String name, @Nullable Long id) {
    if (supplierContactRepository.isExistBySupplierIdAndNameExcludeId(supplierId, name, id)) {
      throw new DomainException("供应商" + supplierId + "已经有联系人" + name + "了");
    }
  }
}
