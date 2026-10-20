package com.mxerp.inventory.domain.warehouse;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 仓库工厂，负责创建仓库 */
@NullMarked
public class WarehouseFactory {

  /** 仓库仓储 */
  private final WarehouseRepository warehouseRepository;

  /**
   * 构造器注入
   *
   * @param warehouseRepository 仓库仓储
   */
  public WarehouseFactory(WarehouseRepository warehouseRepository) {
    this.warehouseRepository = warehouseRepository;
  }

  /**
   * 创建仓库
   *
   * @param code 代码
   * @param name 名称
   * @param locationManaged 是否启用库位管理
   * @param allowNegativeStock 是否允许负库存出库
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @return 仓库聚合
   * @throws DomainException 仓库代码已存在时抛出
   */
  public Warehouse create(
      String code,
      String name,
      boolean locationManaged,
      boolean allowNegativeStock,
      boolean disabled,
      @Nullable String remark) {
    // 校验仓库代码唯一
    if (warehouseRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("仓库代码" + code + "已存在!");
    }
    return new Warehouse(
        code, name, locationManaged, allowNegativeStock, disabled, remark);
  }
}
