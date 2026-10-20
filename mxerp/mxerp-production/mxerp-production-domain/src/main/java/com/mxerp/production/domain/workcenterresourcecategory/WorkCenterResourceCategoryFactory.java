package com.mxerp.production.domain.workcenterresourcecategory;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 工作中心资源类别工厂，负责创建工作中心资源类别 */
@NullMarked
public class WorkCenterResourceCategoryFactory {

  /** 工作中心资源类别仓储 */
  private final WorkCenterResourceCategoryRepository workCenterResourceCategoryRepository;

  /**
   * 构造器注入
   *
   * @param workCenterResourceCategoryRepository 工作中心资源类别仓储
   */
  public WorkCenterResourceCategoryFactory(
      WorkCenterResourceCategoryRepository workCenterResourceCategoryRepository) {
    this.workCenterResourceCategoryRepository = workCenterResourceCategoryRepository;
  }

  /**
   * 创建工作中心资源类别
   *
   * @param code 代码
   * @param name 名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @return 工作中心资源类别聚合
   * @throws DomainException 工作中心资源类别代码已存在时抛出
   */
  public WorkCenterResourceCategory create(
      String code, String name, boolean disabled, @Nullable String remark) {
    // 校验工作中心资源类别代码唯一
    if (workCenterResourceCategoryRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("工作中心资源类别代码" + code + "已存在!");
    }
    return new WorkCenterResourceCategory(code, name, disabled, remark);
  }
}
