package com.mxerp.basedata.domain.unit;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 计量单位组领域服务 */
@NullMarked
public class UnitGroupDomainService {

  /** 单位组工厂 */
  private final UnitGroupFactory unitGroupFactory;

  /** 单位组仓储 */
  private final UnitGroupRepository unitGroupRepository;

  /**
   * 构造器注入
   *
   * @param unitGroupFactory 单位组工厂
   * @param unitGroupRepository 单位组仓储
   */
  public UnitGroupDomainService(
      UnitGroupFactory unitGroupFactory, UnitGroupRepository unitGroupRepository) {
    this.unitGroupFactory = unitGroupFactory;
    this.unitGroupRepository = unitGroupRepository;
  }

  /**
   * 导入生成：单位组存在则修改，不存在则创建
   *
   * @param code 代码
   * @param name 名称
   * @param disabled 是否禁用
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   * @return 单位组聚合
   */
  public UnitGroup importGenerate(
      String code, String name, boolean disabled, @Nullable Integer sequence, @Nullable String remark) {
    // 按代码查单位组
    UnitGroup model = unitGroupRepository.getByCode(code);

    // 不存在则创建，存在则修改
    if (model == null) {
      model = unitGroupFactory.create(code, name, disabled, sequence, remark);
    } else {
      modify(model, code, name, disabled, sequence, remark);
    }

    return model;
  }

  /**
   * 修改单位组
   *
   * @param id 单位组Id
   * @param code 代码
   * @param name 名称
   * @param disabled 是否禁用
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   * @return 单位组聚合
   * @throws DomainException 单位组不存在时抛出
   */
  public UnitGroup modify(
      long id,
      String code,
      String name,
      boolean disabled,
      @Nullable Integer sequence,
      @Nullable String remark) {
    UnitGroup model = unitGroupRepository.get(id);
    if (model == null) {
      throw new DomainException("单位组" + id + "不存在!");
    }
    modify(model, code, name, disabled, sequence, remark);
    return model;
  }

  /**
   * 修改单位组（使用已有聚合）
   *
   * @param model 单位组聚合
   * @param code 代码
   * @param name 名称
   * @param disabled 是否禁用
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   * @throws DomainException 代码与其它单位组冲突时抛出
   */
  public void modify(
      UnitGroup model,
      String code,
      String name,
      boolean disabled,
      @Nullable Integer sequence,
      @Nullable String remark) {
    if (unitGroupRepository.isExistByCodeExcludeId(code, model.getId())) {
      throw new DomainException("单位组代码" + code + "已存在!");
    }
    model.modify(code, name, disabled, sequence, remark);
  }
}
