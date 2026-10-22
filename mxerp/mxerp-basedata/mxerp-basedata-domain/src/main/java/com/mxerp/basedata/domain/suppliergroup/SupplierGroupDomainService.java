package com.mxerp.basedata.domain.suppliergroup;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 供应商分类领域服务 */
@NullMarked
public class SupplierGroupDomainService {

  /** 供应商分类工厂 */
  private final SupplierGroupFactory supplierGroupFactory;

  /** 供应商分类仓储 */
  private final SupplierGroupRepository supplierGroupRepository;

  /**
   * 构造器注入
   *
   * @param supplierGroupFactory 供应商分类工厂
   * @param supplierGroupRepository 供应商分类仓储
   */
  public SupplierGroupDomainService(
      SupplierGroupFactory supplierGroupFactory,
      SupplierGroupRepository supplierGroupRepository) {
    this.supplierGroupFactory = supplierGroupFactory;
    this.supplierGroupRepository = supplierGroupRepository;
  }

  /**
   * 导入生成：供应商分类存在则修改，不存在则创建
   *
   * @param code 代码
   * @param name 名称
   * @param parentId 上级分类Id（可空）
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @return 供应商分类聚合
   */
  public SupplierGroup importGenerate(
      String code,
      String name,
      @Nullable Long parentId,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    // 按代码查供应商分类
    SupplierGroup model = supplierGroupRepository.getByCode(code);

    // 不存在则创建，存在则修改
    if (model == null) {
      model = supplierGroupFactory.create(code, name, parentId, disabled, sort, remark);
    } else {
      modify(model, code, name, parentId, disabled, sort, remark);
    }

    return model;
  }

  /**
   * 修改供应商分类
   *
   * @param id 供应商分类Id
   * @param code 代码
   * @param name 名称
   * @param parentId 上级分类Id（可空）
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @return 供应商分类聚合
   * @throws DomainException 供应商分类不存在时抛出
   */
  public SupplierGroup modify(
      long id,
      String code,
      String name,
      @Nullable Long parentId,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    SupplierGroup model = supplierGroupRepository.get(id);
    if (model == null) {
      throw new DomainException("供应商分类" + id + "不存在!");
    }
    modify(model, code, name, parentId, disabled, sort, remark);
    return model;
  }

  /**
   * 修改供应商分类（使用已有聚合）
   *
   * @param model 供应商分类聚合
   * @param code 代码
   * @param name 名称
   * @param parentId 上级分类Id（可空）
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @throws DomainException 代码与其它供应商分类冲突、上级分类不存在、或上级指向自己（成环）时抛出
   */
  public void modify(
      SupplierGroup model,
      String code,
      String name,
      @Nullable Long parentId,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    if (supplierGroupRepository.isExistByCodeExcludeId(code, model.getId())) {
      throw new DomainException("供应商分类代码" + code + "已存在!");
    }
    requireParent(parentId, model);
    model.modify(code, name, parentId, disabled, sort, remark);
  }

  // ==================== 私有：校验 ====================

  /**
   * 校验上级分类存在、且不是自己（不能成环）
   *
   * @param parentId 上级分类Id（可空）
   * @param self 当前分类
   * @throws DomainException 上级分类不存在、或上级指向自己时抛出
   */
  private void requireParent(@Nullable Long parentId, SupplierGroup self) {
    if (parentId == null) {
      return;
    }
    if (parentId == self.getId()) {
      throw new DomainException("供应商分类的上级分类不能是自己");
    }
    if (supplierGroupRepository.get(parentId) == null) {
      throw new DomainException("上级分类" + parentId + "不存在!");
    }
  }
}
