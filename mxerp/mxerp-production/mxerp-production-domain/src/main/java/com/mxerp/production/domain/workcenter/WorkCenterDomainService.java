package com.mxerp.production.domain.workcenter;

import com.mxerp.production.domain.workcenterresourcecategory.WorkCenterResourceCategoryRepository;
import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 工作中心领域服务，负责跨聚合编排 */
@NullMarked
public class WorkCenterDomainService {

  /** 工作中心工厂 */
  private final WorkCenterFactory workCenterFactory;

  /** 工作中心仓储 */
  private final WorkCenterRepository workCenterRepository;

  /** 工作中心资源类别仓储 */
  private final WorkCenterResourceCategoryRepository workCenterResourceCategoryRepository;

  /**
   * 构造器注入
   *
   * @param workCenterFactory 工作中心工厂
   * @param workCenterRepository 工作中心仓储
   * @param workCenterResourceCategoryRepository 工作中心资源类别仓储
   */
  public WorkCenterDomainService(
      WorkCenterFactory workCenterFactory,
      WorkCenterRepository workCenterRepository,
      WorkCenterResourceCategoryRepository workCenterResourceCategoryRepository) {
    this.workCenterFactory = workCenterFactory;
    this.workCenterRepository = workCenterRepository;
    this.workCenterResourceCategoryRepository = workCenterResourceCategoryRepository;
  }

  /**
   * 导入生成：工作中心存在则修改，不存在则创建
   *
   * @param code 代码
   * @param name 名称
   * @param capacityMinutesPerDay 日可用工时（分钟）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @return 工作中心聚合
   */
  public WorkCenter importGenerate(
      String code,
      String name,
      BigDecimal capacityMinutesPerDay,
      boolean disabled,
      @Nullable String remark) {
    // 按代码查工作中心
    WorkCenter model = workCenterRepository.getByCode(code);

    // 不存在则创建，存在则修改
    if (model == null) {
      model = workCenterFactory.create(code, name, capacityMinutesPerDay, disabled, remark);
    } else {
      modify(model, code, name, capacityMinutesPerDay, disabled, remark);
    }

    return model;
  }

  /**
   * 修改工作中心
   *
   * @param id 工作中心Id
   * @param code 代码
   * @param name 名称
   * @param capacityMinutesPerDay 日可用工时（分钟）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @return 工作中心聚合
   */
  public WorkCenter modify(
      long id,
      String code,
      String name,
      BigDecimal capacityMinutesPerDay,
      boolean disabled,
      @Nullable String remark) {
    WorkCenter model = getExist(id);
    modify(model, code, name, capacityMinutesPerDay, disabled, remark);
    return model;
  }

  /**
   * 修改工作中心
   *
   * @param model 工作中心
   * @param code 代码
   * @param name 名称
   * @param capacityMinutesPerDay 日可用工时（分钟）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  public void modify(
      WorkCenter model,
      String code,
      String name,
      BigDecimal capacityMinutesPerDay,
      boolean disabled,
      @Nullable String remark) {
    // 校验工作中心代码唯一
    if (workCenterRepository.isExistByCodeExcludeId(code, model.getId())) {
      throw new DomainException("工作中心代码" + code + "已存在!");
    }
    model.modify(code, name, capacityMinutesPerDay, disabled, remark);
  }

  /**
   * 添加资源
   *
   * @param workCenterId 工作中心Id
   * @param code 代码
   * @param name 名称
   * @param workCenterResourceCategoryId 工作中心资源类别Id
   * @param quantity 数量
   * @param efficiency 效率（百分比）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @throws DomainException 工作中心或资源类别不存在、资源代码重复时抛出
   */
  public void addResource(
      long workCenterId,
      String code,
      String name,
      long workCenterResourceCategoryId,
      int quantity,
      BigDecimal efficiency,
      boolean disabled,
      @Nullable String remark) {
    WorkCenter model = getExist(workCenterId);
    validateCategory(workCenterResourceCategoryId);
    model.addResource(
        code, name, workCenterResourceCategoryId, quantity, efficiency, disabled, remark);
  }

  /**
   * 修改资源
   *
   * @param workCenterId 工作中心Id
   * @param resourceId 资源Id
   * @param code 代码
   * @param name 名称
   * @param workCenterResourceCategoryId 工作中心资源类别Id
   * @param quantity 数量
   * @param efficiency 效率（百分比）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @throws DomainException 工作中心、资源或资源类别不存在、资源代码重复时抛出
   */
  public void modifyResource(
      long workCenterId,
      long resourceId,
      String code,
      String name,
      long workCenterResourceCategoryId,
      int quantity,
      BigDecimal efficiency,
      boolean disabled,
      @Nullable String remark) {
    WorkCenter model = getExist(workCenterId);
    validateCategory(workCenterResourceCategoryId);
    model.modifyResource(
        resourceId,
        code,
        name,
        workCenterResourceCategoryId,
        quantity,
        efficiency,
        disabled,
        remark);
  }

  /**
   * 删除资源（逻辑删除）
   *
   * @param workCenterId 工作中心Id
   * @param resourceId 资源Id
   * @throws DomainException 工作中心不存在时抛出
   */
  public void deleteResource(long workCenterId, long resourceId) {
    WorkCenter model = getExist(workCenterId);
    model.deleteResource(resourceId);
  }

  /**
   * 取工作中心，不存在直接报错
   *
   * @param id 工作中心Id
   * @return 工作中心
   * @throws DomainException 工作中心不存在时抛出
   */
  private WorkCenter getExist(long id) {
    WorkCenter model = workCenterRepository.get(id);
    if (model == null) {
      throw new DomainException("工作中心" + id + "不存在!");
    }
    return model;
  }

  /**
   * 校验资源类别存在
   *
   * @param workCenterResourceCategoryId 工作中心资源类别Id
   * @throws DomainException 工作中心资源类别不存在时抛出
   */
  private void validateCategory(long workCenterResourceCategoryId) {
    if (workCenterResourceCategoryRepository.get(workCenterResourceCategoryId) == null) {
      throw new DomainException("工作中心资源类别" + workCenterResourceCategoryId + "不存在!");
    }
  }
}
