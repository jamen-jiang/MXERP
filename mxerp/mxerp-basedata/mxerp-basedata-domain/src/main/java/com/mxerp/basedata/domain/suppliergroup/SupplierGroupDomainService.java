package com.mxerp.basedata.domain.suppliergroup;

import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.HierarchyUtils;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 供应商分类领域服务，负责层级编排 */
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
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   * @return 需要持久化的聚合列表（新建只有自身；修改则含受影响的子孙）
   * @throws DomainException 上级分类不存在、指向上级会成环、或代码冲突时抛出
   */
  public List<SupplierGroup> importGenerate(
      String code,
      String name,
      @Nullable Long parentId,
      boolean disabled,
      @Nullable Integer sequence,
      @Nullable String remark) {
    // 按代码查供应商分类
    SupplierGroup model = supplierGroupRepository.getByCode(code);

    // 不存在则创建，存在则修改（含层级编排）
    if (model == null) {
      return List.of(
          supplierGroupFactory.create(code, name, parentId, disabled, sequence, remark));
    }
    return modify(model.getId(), code, name, parentId, disabled, sequence, remark);
  }

  /**
   * 修改供应商分类（含层级编排）
   *
   * @param id 供应商分类Id
   * @param code 代码
   * @param name 名称
   * @param parentId 上级分类Id（可空）
   * @param disabled 是否禁用
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   * @return 需要持久化的聚合列表（自身 + 受影响的子孙）
   * @throws DomainException 供应商分类不存在、代码冲突、上级分类不存在、或指向上级会成环时抛出
   */
  public List<SupplierGroup> modify(
      long id,
      String code,
      String name,
      @Nullable Long parentId,
      boolean disabled,
      @Nullable Integer sequence,
      @Nullable String remark) {
    SupplierGroup model = getExist(id);
    if (supplierGroupRepository.isExistByCodeExcludeId(code, id)) {
      throw new DomainException("供应商分类代码" + code + "已存在!");
    }
    SupplierGroup parent = getParent(parentId);
    validateParent(model, parentId, parent);
    model.modify(
        HierarchyUtils.resolveParentPath(parentId, parent, "上级分类"),
        code,
        name,
        parentId,
        disabled,
        sequence,
        remark);
    List<SupplierGroup> list = new ArrayList<>();
    setChildrenParentPath(model, list);
    list.addFirst(model);
    return list;
  }

  // ==================== 私有：取数与校验 ====================

  /**
   * 校验上级分类合法：不能是自己，也不能是自己的子孙
   *
   * <p>否则层级路径会自我包含，树变成环，之后按路径查子孙会无限套。
   *
   * @param model 当前供应商分类
   * @param parentId 上级分类Id（可空）
   * @param parent 上级分类（可空）
   * @throws DomainException 上级分类不存在、是自己、或自己的子孙时抛出
   */
  private void validateParent(
      SupplierGroup model, @Nullable Long parentId, @Nullable SupplierGroup parent) {
    if (parentId == null) {
      return;
    }
    if (Objects.equals(parentId, model.getId())) {
      throw new DomainException("上级分类不能是自己!");
    }
    if (parent == null) {
      throw new DomainException("上级分类" + parentId + "不存在!");
    }
    boolean isDescendant =
        supplierGroupRepository.getWithChildren(model.getId()).stream()
            .anyMatch(item -> Objects.equals(item.getId(), parentId));
    if (isDescendant) {
      throw new DomainException("上级分类不能是自己的子孙分类!");
    }
  }

  /**
   * 取供应商分类，不存在则抛异常
   *
   * @param id 供应商分类Id
   * @return 供应商分类聚合
   * @throws DomainException 供应商分类不存在时抛出
   */
  private SupplierGroup getExist(long id) {
    SupplierGroup model = supplierGroupRepository.get(id);
    if (model == null) {
      throw new DomainException("供应商分类" + id + "不存在!");
    }
    return model;
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

  /**
   * 递归重设子孙分类的层级路径
   *
   * @param model 供应商分类
   * @param list 子孙收集列表
   */
  private void setChildrenParentPath(SupplierGroup model, List<SupplierGroup> list) {
    if (model.getId() == 0) {
      return;
    }
    List<SupplierGroup> children = supplierGroupRepository.getListByParentId(model.getId());
    HierarchyUtils.resetChildrenParentPath(model, children);
    for (SupplierGroup child : children) {
      setChildrenParentPath(child, list);
      list.add(child);
    }
  }
}
