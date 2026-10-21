package com.mxerp.basedata.domain.category;

import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.HierarchyUtils;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 物料分类工厂，负责创建分类 */
@NullMarked
public class CategoryFactory {

  /** 分类仓储 */
  private final CategoryRepository categoryRepository;

  /**
   * 构造器注入
   *
   * @param categoryRepository 分类仓储
   */
  public CategoryFactory(CategoryRepository categoryRepository) {
    this.categoryRepository = categoryRepository;
  }

  /**
   * 创建分类
   *
   * @param parentId 父分类Id（可空）
   * @param code 分类代码
   * @param name 分类名称
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @return 分类聚合
   * @throws DomainException 分类代码已存在时抛出
   * @throws DomainException 同级分类名称已存在时抛出
   * @throws DomainException 父分类不存在时抛出
   */
  public Category create(
      @Nullable Long parentId,
      String code,
      String name,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    // 校验分类代码全局唯一
    if (categoryRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("分类代码" + code + "已存在!");
    }
    // 校验同级分类名称唯一
    if (categoryRepository.isExistByNameAndParentIdExcludeId(name, parentId)) {
      throw new DomainException("同级分类名称" + name + "已存在!");
    }
    return new Category(
        HierarchyUtils.resolveLevel(parentId, getParent(parentId), "父分类"),
        parentId,
        code,
        name,
        disabled,
        sort,
        remark);
  }

  /**
   * 查询父分类
   *
   * @param parentId 父分类Id（可空）
   * @return 父分类，无父Id或不存在时返回 null（可空）
   */
  @Nullable
  private Category getParent(@Nullable Long parentId) {
    return parentId == null ? null : categoryRepository.get(parentId);
  }
}
