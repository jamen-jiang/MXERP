package com.mxerp.production.domain.productionrun;

import com.mxerp.shared.domain.exception.DomainException;
import java.time.LocalDateTime;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 炉号单工厂，负责建单 */
@NullMarked
public class ProductionRunFactory {

  /** 炉号单仓储 */
  private final ProductionRunRepository productionRunRepository;

  /**
   * 构造器注入
   *
   * @param productionRunRepository 炉号单仓储
   */
  public ProductionRunFactory(ProductionRunRepository productionRunRepository) {
    this.productionRunRepository = productionRunRepository;
  }

  /**
   * 创建炉号单（草稿）
   *
   * <p>工序、设备与分配标准在建单时就定下来，不允许留空：一炉做的是哪道工序、账按什么口径分，是加行与摊回的
   * 前提，缺一样后面都得回头改表头。炉次时间可以后补——车间常常是先按炉记行、出炉之后才知道该挂哪天。
   *
   * @param operationId 工序Id
   * @param workCenterId 工作中心Id
   * @param allocationBasis 分配标准编码
   * @param occupyUnitId 占用量单位Id（可空）
   * @param runTime 炉次时间（可空）
   * @param remark 备注（可空）
   * @param number 炉号（传空串表示由 {@code OrderAddedEvent} 的监听器后续分配）
   * @return 炉号单聚合
   * @throws DomainException 炉号已存在或分配标准非法时抛出
   */
  public ProductionRun create(
      Long operationId,
      Long workCenterId,
      String allocationBasis,
      @Nullable Long occupyUnitId,
      @Nullable LocalDateTime runTime,
      @Nullable String remark,
      String number) {
    // 单号非空时才校验唯一，空串代表尚未分配
    if (!number.isBlank() && productionRunRepository.isExistByNumberExcludeId(number)) {
      throw new DomainException("炉号" + number + "已存在!");
    }
    return new ProductionRun(
        operationId, workCenterId, allocationBasis, occupyUnitId, runTime, remark, number);
  }
}
