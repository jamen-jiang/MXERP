package com.mxerp.production.domain.workcenter;

import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 工作中心工厂，负责创建工作中心 */
@NullMarked
public class WorkCenterFactory {

  /** 工作中心仓储 */
  private final WorkCenterRepository workCenterRepository;

  /**
   * 构造器注入
   *
   * @param workCenterRepository 工作中心仓储
   */
  public WorkCenterFactory(WorkCenterRepository workCenterRepository) {
    this.workCenterRepository = workCenterRepository;
  }

  /**
   * 创建工作中心
   *
   * @param code 代码
   * @param name 名称
   * @param type 类型
   * @param capacityMinutesPerDay 日可用工时（分钟）
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @return 工作中心聚合
   * @throws DomainException 工作中心代码已存在时抛出
   */
  public WorkCenter create(
      String code,
      String name,
      String type,
      BigDecimal capacityMinutesPerDay,
      boolean disabled,
      @Nullable String remark) {
    // 校验工作中心代码唯一
    if (workCenterRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("工作中心代码" + code + "已存在!");
    }
    return new WorkCenter(code, name, type, capacityMinutesPerDay, disabled, remark);
  }
}
