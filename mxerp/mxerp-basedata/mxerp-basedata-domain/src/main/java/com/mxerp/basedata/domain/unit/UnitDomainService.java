package com.mxerp.basedata.domain.unit;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 单位领域服务，负责跨聚合编排 */
@NullMarked
public class UnitDomainService {

  /** 单位工厂 */
  private final UnitFactory unitFactory;

  /** 单位仓储 */
  private final UnitRepository unitRepository;

  /**
   * 构造器注入
   *
   * @param unitFactory 单位工厂
   * @param unitRepository 单位仓储
   */
  public UnitDomainService(UnitFactory unitFactory, UnitRepository unitRepository) {
    this.unitFactory = unitFactory;
    this.unitRepository = unitRepository;
  }

  /**
   * 导入生成：单位存在则修改，不存在则创建
   *
   * @param code 单位代码
   * @param name 单位名称
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @return 单位聚合
   */
  public Unit importGenerate(
      String code, String name, @Nullable Integer sort, @Nullable String remark) {
    // 按代码查单位
    Unit model = unitRepository.getByCode(code);

    // 不存在则创建，存在则修改
    if (model == null) {
      model = unitFactory.create(code, name, sort, remark);
    } else {
      modify(model, code, name, sort, remark);
    }

    return model;
  }

  /**
   * 修改单位
   *
   * @param id 单位Id
   * @param code 单位代码
   * @param name 单位名称
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @return 单位聚合
   */
  public Unit modify(
      long id, String code, String name, @Nullable Integer sort, @Nullable String remark) {
    Unit model = unitRepository.get(id);
    if (model == null) {
      throw new DomainException("单位" + id + "不存在!");
    }
    modify(model, code, name, sort, remark);
    return model;
  }

  /**
   * 修改单位
   *
   * @param model 单位
   * @param code 单位代码
   * @param name 单位名称
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   */
  public void modify(
      Unit model, String code, String name, @Nullable Integer sort, @Nullable String remark) {
    // 校验单位代码唯一
    if (unitRepository.isExistByCodeExcludeId(code, model.getId())) {
      throw new DomainException("单位代码" + code + "已存在!");
    }
    model.modify(code, name, sort, remark);
  }
}
