package com.mxerp.basedata.domain.unit;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 计量单位组工厂，负责创建单位组 */
@NullMarked
public class UnitGroupFactory {

  /** 单位组仓储 */
  private final UnitGroupRepository unitGroupRepository;

  /**
   * 构造器注入
   *
   * @param unitGroupRepository 单位组仓储
   */
  public UnitGroupFactory(UnitGroupRepository unitGroupRepository) {
    this.unitGroupRepository = unitGroupRepository;
  }

  /**
   * 创建单位组
   *
   * @param code 代码
   * @param name 名称
   * @param disabled 是否禁用
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @return 单位组聚合
   * @throws DomainException 单位组代码已存在时抛出
   */
  public UnitGroup create(
      String code, String name, boolean disabled, @Nullable Integer sort, @Nullable String remark) {
    // 校验单位组代码唯一
    if (unitGroupRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("单位组代码" + code + "已存在!");
    }
    return new UnitGroup(code, name, disabled, sort, remark);
  }
}
