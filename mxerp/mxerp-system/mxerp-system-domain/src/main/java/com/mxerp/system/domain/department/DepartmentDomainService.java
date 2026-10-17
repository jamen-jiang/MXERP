package com.mxerp.system.domain.department;

import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.HierarchyUtils;
import java.util.ArrayList;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 部门领域服务，负责层级编排 */
@NullMarked
public class DepartmentDomainService {

  /** 部门仓储 */
  private final DepartmentRepository departmentRepository;

  /**
   * 构造器注入
   *
   * @param departmentRepository 部门仓储
   */
  public DepartmentDomainService(DepartmentRepository departmentRepository) {
    this.departmentRepository = departmentRepository;
  }

  /**
   * 修改部门（含层级编排）
   *
   * @param id 部门Id
   * @param companyId 公司Id
   * @param parentId 父Id（可空）
   * @param code 部门代码
   * @param name 部门名称
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param sort 排序（可空）
   * @param address 地址（可空）
   * @param remark 备注（可空）
   * @return 需要持久化的聚合列表（自身 + 受影响的子孙）
   * @throws DomainException 部门代码已存在时抛出
   * @throws DomainException 同级部门名称已存在时抛出
   * @throws DomainException 部门不存在时抛出
   */
  public List<Department> modify(
      long id,
      long companyId,
      @Nullable Long parentId,
      String code,
      String name,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Integer sort,
      @Nullable String address,
      @Nullable String remark) {
    // 校验部门代码唯一（排除自身）
    if (departmentRepository.isExistByCodeExcludeId(code, id)) {
      throw new DomainException("部门代码" + code + "已存在!");
    }
    // 校验同级部门名称唯一（排除自身）
    if (departmentRepository.isExistByNameAndParentIdExcludeId(name, parentId, id)) {
      throw new DomainException("同级部门名称" + name + "已存在!");
    }
    Department model = departmentRepository.get(id);
    if (model == null) {
      throw new DomainException("部门不存在!");
    }
    model.modify(
        HierarchyUtils.resolveLevel(parentId, getParent(parentId), "父部门"),
        companyId,
        parentId,
        code,
        name,
        telephone,
        fax,
        email,
        sort,
        address,
        remark);
    List<Department> list = new ArrayList<>();
    setChildrenLevel(model, list);
    list.addFirst(model);
    return list;
  }

  /**
   * 删除自身及所有子孙部门
   *
   * @param id 部门Id
   * @return 需要持久化的聚合列表（已标记删除）
   */
  public List<Department> removeWithChildren(long id) {
    List<Department> list = departmentRepository.getWithChildren(id);
    for (Department item : list) {
      item.delete();
    }
    return list;
  }

  /**
   * 查询父部门
   *
   * @param parentId 父Id（可空）
   * @return 父部门，无父Id或不存在时返回 null（可空）
   */
  @Nullable
  private Department getParent(@Nullable Long parentId) {
    return parentId == null ? null : departmentRepository.get(parentId);
  }

  /**
   * 递归设置子部门层级
   *
   * @param model 部门
   * @param list 子部门收集列表
   */
  private void setChildrenLevel(Department model, List<Department> list) {
    if (model.getId() == 0) {
      return;
    }
    List<Department> children = departmentRepository.getListByParentId(model.getId());
    HierarchyUtils.resetChildrenLevel(model, children);
    for (Department child : children) {
      setChildrenLevel(child, list);
      list.add(child);
    }
  }
}
