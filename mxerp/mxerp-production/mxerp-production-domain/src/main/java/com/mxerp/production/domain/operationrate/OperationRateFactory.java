package com.mxerp.production.domain.operationrate;

import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 工序费率卡工厂，负责创建费率卡 */
@NullMarked
public class OperationRateFactory {

  /** 工序费率卡仓储 */
  private final OperationRateRepository operationRateRepository;

  /**
   * 构造器注入
   *
   * @param operationRateRepository 工序费率卡仓储
   */
  public OperationRateFactory(OperationRateRepository operationRateRepository) {
    this.operationRateRepository = operationRateRepository;
  }

  /**
   * 创建费率卡（初始状态为待生效）
   *
   * @param code 费率卡代码
   * @param name 费率卡名称
   * @param setupRate 准备费率（元/小时）
   * @param runRate 加工费率（元/小时）
   * @param quantityRate 计件费率（元/件）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @return 费率卡聚合
   * @throws DomainException 同代码同生效时间已有卡时抛出
   */
  public OperationRate create(
      String code,
      String name,
      BigDecimal setupRate,
      BigDecimal runRate,
      BigDecimal quantityRate,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    // 同一代码下生效时间不能撞车，否则取价会出现两张都合法
    if (operationRateRepository.isExistByCodeAndEffectiveFromExcludeId(code, effectiveFrom)) {
      throw new DomainException("费率卡" + code + "在该生效时间已存在!");
    }
    return new OperationRate(
        code, name, setupRate, runRate, quantityRate, effectiveFrom, effectiveTo, remark);
  }
}
