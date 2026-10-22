package com.mxerp.basedata.domain.suppliergroup;

import com.mxerp.shared.domain.repository.Repository;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 供应商分类仓储接口 */
@NullMarked
public interface SupplierGroupRepository extends Repository<SupplierGroup> {

  /**
   * 按代码查询
   *
   * @param code 供应商分类代码
   * @return 供应商分类（可空），不存在返回 null
   */
  @Nullable SupplierGroup getByCode(String code);

  /**
   * 查询指定供应商分类及其所有子孙分类（包含自身）
   *
   * @param id 供应商分类Id
   * @return 供应商分类及其子孙列表
   */
  List<SupplierGroup> getWithChildren(long id);

  /**
   * 按上级分类查询子分类
   *
   * @param parentId 上级分类Id
   * @return 子分类列表
   */
  List<SupplierGroup> getListByParentId(long parentId);

  /**
   * 判断供应商分类代码是否存在
   *
   * @param code 供应商分类代码
   * @param id 排除的供应商分类Id（可空，新增时传 null）
   * @return true 表示已存在
   */
  boolean isExistByCodeExcludeId(String code, @Nullable Long id);

  /**
   * 判断供应商分类代码是否存在
   *
   * @param code 供应商分类代码
   * @return true 表示已存在
   */
  default boolean isExistByCodeExcludeId(String code) {
    return isExistByCodeExcludeId(code, null);
  }
}
