package com.mxerp.basedata.domain.brand;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 产品品牌工厂，负责创建品牌 */
@NullMarked
public class BrandFactory {

  /** 品牌仓储 */
  private final BrandRepository brandRepository;

  /**
   * 构造器注入
   *
   * @param brandRepository 品牌仓储
   */
  public BrandFactory(BrandRepository brandRepository) {
    this.brandRepository = brandRepository;
  }

  /**
   * 创建品牌
   *
   * @param code 品牌代码
   * @param name 品牌名称
   * @param remark 备注（可空）
   * @return 品牌聚合
   * @throws DomainException 品牌代码已存在时抛出
   */
  public Brand create(String code, String name, @Nullable String remark) {
    // 校验品牌代码唯一
    if (brandRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("品牌代码" + code + "已存在!");
    }
    return new Brand(code, name, remark);
  }
}
