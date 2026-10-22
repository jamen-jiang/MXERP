package com.mxerp.basedata.domain.supplier;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 供应商联系人领域服务 */
@NullMarked
public class SupplierContactDomainService {

  /** 联系人工厂 */
  private final SupplierContactFactory supplierContactFactory;

  /** 联系人仓储 */
  private final SupplierContactRepository supplierContactRepository;

  /** 供应商仓储：建联系人前要确认供应商存在 */
  private final SupplierRepository supplierRepository;

  /**
   * 构造器注入
   *
   * @param supplierContactFactory 联系人工厂
   * @param supplierContactRepository 联系人仓储
   * @param supplierRepository 供应商仓储
   */
  public SupplierContactDomainService(
      SupplierContactFactory supplierContactFactory,
      SupplierContactRepository supplierContactRepository,
      SupplierRepository supplierRepository) {
    this.supplierContactFactory = supplierContactFactory;
    this.supplierContactRepository = supplierContactRepository;
    this.supplierRepository = supplierRepository;
  }

  /**
   * 新建联系人
   *
   * @param supplierId 供应商Id
   * @param name 姓名
   * @param telephone 固定电话（可空）
   * @param mobile 移动电话（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param sequence 排序（可空）
   * @return 联系人聚合
   * @throws DomainException 供应商不存在、或同一供应商下姓名重复时抛出
   */
  public SupplierContact create(
      long supplierId,
      String name,
      @Nullable String telephone,
      @Nullable String mobile,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Integer sequence) {
    requireSupplier(supplierId);
    return supplierContactFactory.create(supplierId, name, telephone, mobile, fax, email, sequence);
  }

  /**
   * 导入生成：同一供应商下姓名存在则修改，不存在则创建
   *
   * @param supplierId 供应商Id
   * @param name 姓名
   * @param telephone 固定电话（可空）
   * @param mobile 移动电话（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param sequence 排序（可空）
   * @return 联系人聚合
   */
  public SupplierContact importGenerate(
      long supplierId,
      String name,
      @Nullable String telephone,
      @Nullable String mobile,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Integer sequence) {
    // 按供应商 + 姓名查联系人
    SupplierContact model =
        supplierContactRepository.getBySupplierIdAndName(supplierId, name);

    // 不存在则创建，存在则修改
    if (model == null) {
      model = create(supplierId, name, telephone, mobile, fax, email, sequence);
    } else {
      modify(model, name, telephone, mobile, fax, email, sequence);
    }

    return model;
  }

  /**
   * 修改联系人
   *
   * @param id 联系人Id
   * @param name 姓名
   * @param telephone 固定电话（可空）
   * @param mobile 移动电话（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param sequence 排序（可空）
   * @return 联系人聚合
   * @throws DomainException 联系人不存在时抛出
   */
  public SupplierContact modify(
      long id,
      String name,
      @Nullable String telephone,
      @Nullable String mobile,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Integer sequence) {
    SupplierContact model = supplierContactRepository.get(id);
    if (model == null) {
      throw new DomainException("联系人" + id + "不存在!");
    }
    modify(model, name, telephone, mobile, fax, email, sequence);
    return model;
  }

  /**
   * 修改联系人（使用已有聚合）
   *
   * @param model 联系人聚合
   * @param name 姓名
   * @param telephone 固定电话（可空）
   * @param mobile 移动电话（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param sequence 排序（可空）
   * @throws DomainException 同一供应商下姓名重复时抛出
   */
  public void modify(
      SupplierContact model,
      String name,
      @Nullable String telephone,
      @Nullable String mobile,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Integer sequence) {
    supplierContactFactory.checkUnique(model.getSupplierId(), name, model.getId());
    model.modify(name, telephone, mobile, fax, email, sequence);
  }

  // ==================== 私有：校验 ====================

  /**
   * 校验供应商存在
   *
   * @param supplierId 供应商Id
   * @throws DomainException 供应商不存在时抛出
   */
  private void requireSupplier(long supplierId) {
    if (supplierRepository.get(supplierId) == null) {
      throw new DomainException("供应商" + supplierId + "不存在!");
    }
  }
}
