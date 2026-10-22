package com.mxerp.basedata.domain.unit;

import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 单位工厂，负责创建单位 */
@NullMarked
public class UnitFactory {

  /** 单位仓储 */
  private final UnitRepository unitRepository;

  /**
   * 构造器注入
   *
   * @param unitRepository 单位仓储
   */
  public UnitFactory(UnitRepository unitRepository) {
    this.unitRepository = unitRepository;
  }

  /**
   * 创建单位
   *
   * @param groupId 所属单位组Id
   * @param code 代码
   * @param name 名称
   * @param factor 换算率（1 本单位 = factor 基准单位）
   * @param isBase 是否组内基准单位
   * @param rounding 数量落数步长
   * @param disabled 是否禁用
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   * @return 单位聚合
   * @throws DomainException 单位代码已存在、或该组已有基准单位时抛出
   */
  public Unit create(
      long groupId,
      String code,
      String name,
      BigDecimal factor,
      boolean isBase,
      BigDecimal rounding,
      boolean disabled,
      @Nullable Integer sequence,
      @Nullable String remark) {
    // 校验单位代码唯一
    if (unitRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("单位代码" + code + "已存在!");
    }
    // 一个单位组只能有一个基准单位
    if (isBase && unitRepository.isExistByGroupIdAndIsBaseExcludeId(groupId, null)) {
      throw new DomainException("该单位组已经有基准单位了，换算率要相对它来填");
    }
    return new Unit(
        groupId, code, name, factor, isBase, rounding, disabled, sequence, remark);
  }
}
