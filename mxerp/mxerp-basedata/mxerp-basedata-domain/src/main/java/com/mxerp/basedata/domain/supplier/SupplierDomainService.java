package com.mxerp.basedata.domain.supplier;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 供应商领域服务，负责跨聚合编排 */
@NullMarked
public class SupplierDomainService {

  /** 供应商工厂 */
  private final SupplierFactory supplierFactory;

  /** 供应商仓储 */
  private final SupplierRepository supplierRepository;

  /**
   * 构造器注入
   *
   * @param supplierFactory 供应商工厂
   * @param supplierRepository 供应商仓储
   */
  public SupplierDomainService(
      SupplierFactory supplierFactory, SupplierRepository supplierRepository) {
    this.supplierFactory = supplierFactory;
    this.supplierRepository = supplierRepository;
  }

  /**
   * 修改供应商（按Id查询后修改）
   *
   * @param id 供应商Id
   * @param code 供应商代码
   * @param name 供应商名称
   * @param shortName 供应商简称（可空）
   * @param address 地址（可空）
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param contactPerson 负责人（可空）
   * @param taxRateId 税率Id（可空）
   * @param payMode 支付方式（可空）
   * @param deliveryMode 收货方式（可空）
   * @param remark 备注（可空）
   * @return 供应商聚合
   * @throws DomainException 供应商不存在时抛出
   * @throws DomainException 供应商代码已存在时抛出
   */
  public Supplier modify(
      long id,
      String code,
      String name,
      @Nullable String shortName,
      @Nullable String address,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String contactPerson,
      @Nullable Long taxRateId,
      @Nullable String payMode,
      @Nullable String deliveryMode,
      @Nullable String remark) {
    // 1. 查供应商
    Supplier model = supplierRepository.get(id);
    if (model == null) {
      throw new DomainException("供应商不存在!");
    }
    // 2. 委托给使用已有聚合的重载
    modify(
        model,
        code,
        name,
        shortName,
        address,
        telephone,
        fax,
        email,
        contactPerson,
        taxRateId,
        payMode,
        deliveryMode,
        remark);
    return model;
  }

  /**
   * 修改供应商（使用已有聚合，避免重复查询）
   *
   * @param supplier 供应商聚合
   * @param code 供应商代码
   * @param name 供应商名称
   * @param shortName 供应商简称（可空）
   * @param address 地址（可空）
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param contactPerson 负责人（可空）
   * @param taxRateId 税率Id（可空）
   * @param payMode 支付方式（可空）
   * @param deliveryMode 收货方式（可空）
   * @param remark 备注（可空）
   * @throws DomainException 供应商代码已存在时抛出
   */
  public void modify(
      Supplier supplier,
      String code,
      String name,
      @Nullable String shortName,
      @Nullable String address,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String contactPerson,
      @Nullable Long taxRateId,
      @Nullable String payMode,
      @Nullable String deliveryMode,
      @Nullable String remark) {
    // 校验供应商代码唯一（排除自身）
    if (supplierRepository.isExistByCodeExcludeId(code, supplier.getId())) {
      throw new DomainException("供应商代码" + code + "已存在!");
    }
    supplier.modify(
        code,
        name,
        shortName,
        address,
        telephone,
        fax,
        email,
        contactPerson,
        taxRateId,
        payMode,
        deliveryMode,
        remark);
  }

  /**
   * 导入生成：供应商存在则修改，不存在则创建
   *
   * @param code 供应商代码
   * @param name 供应商名称
   * @param shortName 供应商简称（可空）
   * @param address 地址（可空）
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param contactPerson 负责人（可空）
   * @param taxRateId 税率Id（可空）
   * @param payMode 支付方式（可空）
   * @param deliveryMode 收货方式（可空）
   * @param remark 备注（可空）
   * @return 供应商聚合
   * @throws DomainException 供应商代码已存在时抛出
   */
  public Supplier importGenerate(
      String code,
      String name,
      @Nullable String shortName,
      @Nullable String address,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable String contactPerson,
      @Nullable Long taxRateId,
      @Nullable String payMode,
      @Nullable String deliveryMode,
      @Nullable String remark) {
    // 按代码查供应商
    Supplier model = supplierRepository.getByCode(code);

    // 不存在则走工厂创建，存在则调自己的 modify 重载
    if (model == null) {
      model =
          supplierFactory.create(
              code,
              name,
              shortName,
              address,
              telephone,
              fax,
              email,
              contactPerson,
              taxRateId,
              payMode,
              deliveryMode,
              remark);
    } else {
      modify(
          model,
          code,
          name,
          shortName,
          address,
          telephone,
          fax,
          email,
          contactPerson,
          taxRateId,
          payMode,
          deliveryMode,
          remark);
    }
    return model;
  }
}
