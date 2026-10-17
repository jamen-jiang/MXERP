package com.mxerp.production.domain.workcenter;

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

  /**
   * 构造器注入
   *
   * @param workCenterFactory 工作中心工厂
   * @param workCenterRepository 工作中心仓储
   */
  public WorkCenterDomainService(
      WorkCenterFactory workCenterFactory, WorkCenterRepository workCenterRepository) {
    this.workCenterFactory = workCenterFactory;
    this.workCenterRepository = workCenterRepository;
  }

  /**
   * 导入生成：工作中心存在则修改，不存在则创建
   *
   * @param code 代码
   * @param name 名称
   * @param type 类型
   * @param capacityPerDay 日产能
   * @param costRate 费率
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @return 工作中心聚合
   */
  public WorkCenter importGenerate(
      String code,
      String name,
      String type,
      BigDecimal capacityPerDay,
      BigDecimal costRate,
      boolean disabled,
      @Nullable String remark) {
    // 按代码查工作中心
    WorkCenter model = workCenterRepository.getByCode(code);

    // 不存在则创建，存在则修改
    if (model == null) {
      model =
          workCenterFactory.create(
              code, name, type, capacityPerDay, costRate, disabled, remark);
    } else {
      modify(model, code, name, type, capacityPerDay, costRate, disabled, remark);
    }

    return model;
  }

  /**
   * 修改工作中心
   *
   * @param id 工作中心Id
   * @param code 代码
   * @param name 名称
   * @param type 类型
   * @param capacityPerDay 日产能
   * @param costRate 费率
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @return 工作中心聚合
   */
  public WorkCenter modify(
      long id,
      String code,
      String name,
      String type,
      BigDecimal capacityPerDay,
      BigDecimal costRate,
      boolean disabled,
      @Nullable String remark) {
    WorkCenter model = workCenterRepository.get(id);
    if (model == null) {
      throw new DomainException("工作中心" + id + "不存在!");
    }
    modify(model, code, name, type, capacityPerDay, costRate, disabled, remark);
    return model;
  }

  /**
   * 修改工作中心
   *
   * @param model 工作中心
   * @param code 代码
   * @param name 名称
   * @param type 类型
   * @param capacityPerDay 日产能
   * @param costRate 费率
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  public void modify(
      WorkCenter model,
      String code,
      String name,
      String type,
      BigDecimal capacityPerDay,
      BigDecimal costRate,
      boolean disabled,
      @Nullable String remark) {
    // 校验工作中心代码唯一
    if (workCenterRepository.isExistByCodeExcludeId(code, model.getId())) {
      throw new DomainException("工作中心代码" + code + "已存在!");
    }
    model.modify(code, name, type, capacityPerDay, costRate, disabled, remark);
  }
}
