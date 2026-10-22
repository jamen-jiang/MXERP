package com.mxerp.basedata.domain.suppliergroup;

import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.HierarchyUtils;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 供应商分类工厂，负责创建供应商分类 */
@NullMarked
public class SupplierGroupFactory {

  /** 供应商分类仓储 */
  private final SupplierGroupRepository supplierGroupRepository;

  /**
   * 构造器注入
   *
   * @param supplierGroupRepository 供应商分类仓储
   */
  public SupplierGroupFactory(SupplierGroupRepository supplierGroupRepository) {
    this.supplierGroupRepository = supplierGroupRepository;
  }

  /**
   * 创建供应商分类
   *
   * @param code 代码
   * @param name 名称
   * @param parentId 上级分类Id（可空）
   * @param disabled 是否禁用
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   * @return 供应商分类聚合
   * @throws DomainException 供应商分类代码已存在时抛出
   */
  public SupplierGroup create(
      String code,
      String name,
      @Nullable Long parentId,
      boolean disabled,
      @Nullable Integer sequence,
      @Nullable String remark) {
    // 校验供应商分类代码唯一
    if (supplierGroupRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("供应商分类代码" + code + "已存在!");
    }
    return new SupplierGroup(
        HierarchyUtils.resolveParentPath(parentId, getParent(parentId), "上级分类"),
        code,
        name,
        parentId,
        disabled,
        sequence,
        remark);
  }

  /**
   * 查询上级分类
   *
   * @param parentId 上级分类Id（可空）
   * @return 上级分类，无父Id或不存在时返回 null（可空）
   */
  @Nullable
  private SupplierGroup getParent(@Nullable Long parentId) {
    return parentId == null ? null : supplierGroupRepository.get(parentId);
  }
}
