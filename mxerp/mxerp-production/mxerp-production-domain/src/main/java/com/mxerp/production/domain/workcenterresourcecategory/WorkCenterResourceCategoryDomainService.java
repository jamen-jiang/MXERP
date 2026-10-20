package com.mxerp.production.domain.workcenterresourcecategory;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 工作中心资源类别领域服务，负责跨聚合编排 */
@NullMarked
public class WorkCenterResourceCategoryDomainService {

  /** 工作中心资源类别工厂 */
  private final WorkCenterResourceCategoryFactory workCenterResourceCategoryFactory;

  /** 工作中心资源类别仓储 */
  private final WorkCenterResourceCategoryRepository workCenterResourceCategoryRepository;

  /**
   * 构造器注入
   *
   * @param workCenterResourceCategoryFactory 工作中心资源类别工厂
   * @param workCenterResourceCategoryRepository 工作中心资源类别仓储
   */
  public WorkCenterResourceCategoryDomainService(
      WorkCenterResourceCategoryFactory workCenterResourceCategoryFactory,
      WorkCenterResourceCategoryRepository workCenterResourceCategoryRepository) {
    this.workCenterResourceCategoryFactory = workCenterResourceCategoryFactory;
    this.workCenterResourceCategoryRepository = workCenterResourceCategoryRepository;
  }

  /**
   * 导入生成：工作中心资源类别存在则修改，不存在则创建
   *
   * @param code 代码
   * @param name 名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @return 工作中心资源类别聚合
   */
  public WorkCenterResourceCategory importGenerate(
      String code, String name, boolean disabled, @Nullable String remark) {
    // 按代码查工作中心资源类别
    WorkCenterResourceCategory model = workCenterResourceCategoryRepository.getByCode(code);

    // 不存在则创建，存在则修改
    if (model == null) {
      model = workCenterResourceCategoryFactory.create(code, name, disabled, remark);
    } else {
      modify(model, code, name, disabled, remark);
    }

    return model;
  }

  /**
   * 修改工作中心资源类别
   *
   * @param id 工作中心资源类别Id
   * @param code 代码
   * @param name 名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @return 工作中心资源类别聚合
   */
  public WorkCenterResourceCategory modify(
      long id, String code, String name, boolean disabled, @Nullable String remark) {
    WorkCenterResourceCategory model = workCenterResourceCategoryRepository.get(id);
    if (model == null) {
      throw new DomainException("工作中心资源类别" + id + "不存在!");
    }
    modify(model, code, name, disabled, remark);
    return model;
  }

  /**
   * 修改工作中心资源类别
   *
   * @param model 工作中心资源类别
   * @param code 代码
   * @param name 名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  public void modify(
      WorkCenterResourceCategory model,
      String code,
      String name,
      boolean disabled,
      @Nullable String remark) {
    // 校验工作中心资源类别代码唯一
    if (workCenterResourceCategoryRepository.isExistByCodeExcludeId(code, model.getId())) {
      throw new DomainException("工作中心资源类别代码" + code + "已存在!");
    }
    model.modify(code, name, disabled, remark);
  }
}
