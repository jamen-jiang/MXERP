package com.mxerp.production.domain.rate;

import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 工序费率卡领域服务，负责跨聚合编排
 *
 * <p>这里没有 {@code importGenerate}：费率卡天生一个代码对应多张卡，按代码查不到唯一一条，批量导入只能走「新建一张待生效的卡」。
 */
@NullMarked
public class OperationRateDomainService {

  /** 工序费率卡工厂 */
  private final OperationRateFactory operationRateFactory;

  /** 工序费率卡仓储 */
  private final OperationRateRepository operationRateRepository;

  /**
   * 构造器注入
   *
   * @param operationRateFactory 工序费率卡工厂
   * @param operationRateRepository 工序费率卡仓储
   */
  public OperationRateDomainService(
      OperationRateFactory operationRateFactory,
      OperationRateRepository operationRateRepository) {
    this.operationRateFactory = operationRateFactory;
    this.operationRateRepository = operationRateRepository;
  }

  /**
   * 新建费率卡（待生效）
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
    return operationRateFactory.create(
        code, name, setupRate, runRate, quantityRate, effectiveFrom, effectiveTo, remark);
  }

  /**
   * 修改费率卡
   *
   * @param id 费率卡Id
   * @param code 费率卡代码
   * @param name 费率卡名称
   * @param setupRate 准备费率（元/小时）
   * @param runRate 加工费率（元/小时）
   * @param quantityRate 计件费率（元/件）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @return 费率卡聚合
   * @throws DomainException 费率卡不存在、同代码同生效时间已被占用、或卡已生效不可修改时抛出
   */
  public OperationRate modify(
      long id,
      String code,
      String name,
      BigDecimal setupRate,
      BigDecimal runRate,
      BigDecimal quantityRate,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    OperationRate model = getExist(id);
    if (operationRateRepository.isExistByCodeAndEffectiveFromExcludeId(
        code, effectiveFrom, model.getId())) {
      throw new DomainException("费率卡" + code + "在该生效时间已存在!");
    }
    model.modify(
        code, name, setupRate, runRate, quantityRate, effectiveFrom, effectiveTo, remark);
    return model;
  }

  /**
   * 激活费率卡：本卡转当前生效，同代码的旧卡全部转历史
   *
   * <p>先接替旧卡再激活本卡，中间出异常时两条都不动，不会留下同代码两张当前生效。
   *
   * @param id 费率卡Id
   * @param now 激活时点，必须已落在本卡的有效区间内
   * @return 费率卡聚合
   * @throws DomainException 费率卡不存在、未到生效时间、或本卡不是待生效状态时抛出
   */
  public OperationRate activate(long id, LocalDateTime now) {
    OperationRate model = getExist(id);
    if (!model.covers(now)) {
      throw new DomainException("费率卡" + model.getCode() + "未到生效时间，不能激活");
    }
    List<OperationRate> outgoing =
        operationRateRepository.getListByCode(model.getCode()).stream()
            .filter(OperationRate::isCurrent)
            .toList();
    for (OperationRate previous : outgoing) {
      previous.retire();
    }
    model.activate();
    return model;
  }

  /**
   * 让费率卡提前退位（转历史）
   *
   * <p>用于报价作废：调价来不及等生效期，直接置历史，之后取价取不到它。
   *
   * @param id 费率卡Id
   * @return 费率卡聚合
   * @throws DomainException 费率卡不存在、或不是当前生效状态时抛出
   */
  public OperationRate retire(long id) {
    OperationRate model = getExist(id);
    model.retire();
    return model;
  }

  // ==================== 私有：取数 ====================

  /**
   * 取费率卡，不存在则抛异常
   *
   * @param id 费率卡Id
   * @return 费率卡聚合
   * @throws DomainException 费率卡不存在时抛出
   */
  private OperationRate getExist(long id) {
    OperationRate model = operationRateRepository.get(id);
    if (model == null) {
      throw new DomainException("费率卡" + id + "不存在!");
    }
    return model;
  }
}
