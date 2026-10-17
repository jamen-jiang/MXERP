package com.mxerp.basedata.domain.unit;

import com.mxerp.shared.domain.exception.DomainException;
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
   * @param code 代码
   * @param name 名称
   * @param sort 排序（可空）
   * @param remark 备注（可空）
   * @return 单位聚合
   * @throws DomainException 单位代码已存在时抛出
   */
  public Unit create(String code, String name, @Nullable Integer sort, @Nullable String remark) {
    // 校验单位代码唯一
    if (unitRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("单位代码" + code + "已存在!");
    }
    return new Unit(code, name, sort, remark);
  }
}
