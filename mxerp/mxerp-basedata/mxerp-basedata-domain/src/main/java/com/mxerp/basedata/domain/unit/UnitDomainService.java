package com.mxerp.basedata.domain.unit;

import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 单位领域服务，负责跨聚合编排 */
@NullMarked
public class UnitDomainService {

  /** 单位工厂 */
  private final UnitFactory unitFactory;

  /** 单位仓储 */
  private final UnitRepository unitRepository;

  /** 单位组仓储：建单位前要确认这个组存在 */
  private final UnitGroupRepository unitGroupRepository;

  /**
   * 构造器注入
   *
   * @param unitFactory 单位工厂
   * @param unitRepository 单位仓储
   * @param unitGroupRepository 单位组仓储
   */
  public UnitDomainService(
      UnitFactory unitFactory,
      UnitRepository unitRepository,
      UnitGroupRepository unitGroupRepository) {
    this.unitFactory = unitFactory;
    this.unitRepository = unitRepository;
    this.unitGroupRepository = unitGroupRepository;
  }

  /**
   * 导入生成：单位存在则修改，不存在则创建
   *
   * @param groupId 所属单位组Id
   * @param code 单位代码
   * @param name 单位名称
   * @param factor 换算率（1 基准 = factor 本单位）
   * @param isBase 是否组内基准单位
   * @param rounding 数量落数步长
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @return 单位聚合
   */
  public Unit importGenerate(
      long groupId,
      String code,
      String name,
      BigDecimal factor,
      boolean isBase,
      BigDecimal rounding,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    // 按代码查单位
    Unit model = unitRepository.getByCode(code);

    // 不存在则创建，存在则修改
    if (model == null) {
      requireGroup(groupId);
      model =
          unitFactory.create(
              groupId, code, name, factor, isBase, rounding, disabled, sort, remark);
    } else {
      modify(model, code, name, factor, isBase, rounding, disabled, sort, remark);
    }

    return model;
  }

  /**
   * 修改单位
   *
   * @param id 单位Id
   * @param code 单位代码
   * @param name 单位名称
   * @param factor 换算率（1 基准 = factor 本单位）
   * @param isBase 是否组内基准单位
   * @param rounding 数量落数步长
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @return 单位聚合
   * @throws DomainException 单位不存在时抛出
   */
  public Unit modify(
      long id,
      String code,
      String name,
      BigDecimal factor,
      boolean isBase,
      BigDecimal rounding,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    Unit model = unitRepository.get(id);
    if (model == null) {
      throw new DomainException("单位" + id + "不存在!");
    }
    modify(model, code, name, factor, isBase, rounding, disabled, sort, remark);
    return model;
  }

  /**
   * 修改单位（使用已有聚合）
   *
   * <p>单位组不可改：换组等于换量纲，历史单据上的单位快照没法和旧数据对齐。
   *
   * @param model 单位
   * @param code 单位代码
   * @param name 单位名称
   * @param factor 换算率（1 基准 = factor 本单位）
   * @param isBase 是否组内基准单位
   * @param rounding 数量落数步长
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  public void modify(
      Unit model,
      String code,
      String name,
      BigDecimal factor,
      boolean isBase,
      BigDecimal rounding,
      boolean disabled,
      @Nullable Integer sort,
      @Nullable String remark) {
    // 校验单位代码唯一
    if (unitRepository.isExistByCodeExcludeId(code, model.getId())) {
      throw new DomainException("单位代码" + code + "已存在!");
    }
    // 一个单位组只能有一个基准单位（排除自身）
    if (isBase
        && unitRepository.isExistByGroupIdAndIsBaseExcludeId(model.getGroupId(), model.getId())) {
      throw new DomainException("该单位组已经有基准单位了，换算率要相对它来填");
    }
    model.modify(code, name, factor, isBase, rounding, disabled, sort, remark);
  }

  // ==================== 私有：校验 ====================

  /**
   * 校验单位组存在
   *
   * @param groupId 单位组Id
   * @throws DomainException 单位组不存在时抛出
   */
  private void requireGroup(long groupId) {
    if (unitGroupRepository.get(groupId) == null) {
      throw new DomainException("单位组" + groupId + "不存在!");
    }
  }
}
