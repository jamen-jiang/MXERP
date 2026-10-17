package com.mxerp.system.domain.department;

import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.HierarchyUtils;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 部门工厂，负责创建部门 */
@NullMarked
public class DepartmentFactory {

  /** 部门仓储 */
  private final DepartmentRepository departmentRepository;

  /**
   * 构造器注入
   *
   * @param departmentRepository 部门仓储
   */
  public DepartmentFactory(DepartmentRepository departmentRepository) {
    this.departmentRepository = departmentRepository;
  }

  /**
   * 创建部门
   *
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
   * @return 部门聚合
   * @throws DomainException 部门代码已存在时抛出
   * @throws DomainException 同级部门名称已存在时抛出
   * @throws DomainException 父部门不存在时抛出
   */
  public Department create(
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
    // 校验部门代码唯一
    if (departmentRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("部门代码" + code + "已存在!");
    }
    // 校验同级部门名称唯一
    if (departmentRepository.isExistByNameAndParentIdExcludeId(name, parentId)) {
      throw new DomainException("同级部门名称" + name + "已存在!");
    }
    return new Department(
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
}
