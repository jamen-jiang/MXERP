package com.mxerp.basedata.domain.brand;

import com.mxerp.shared.domain.exception.DomainException;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 品牌领域服务，负责跨聚合编排 */
@NullMarked
public class BrandDomainService {

  /** 品牌工厂 */
  private final BrandFactory brandFactory;

  /** 品牌仓储 */
  private final BrandRepository brandRepository;

  /**
   * 构造器注入
   *
   * @param brandFactory 品牌工厂
   * @param brandRepository 品牌仓储
   */
  public BrandDomainService(BrandFactory brandFactory, BrandRepository brandRepository) {
    this.brandFactory = brandFactory;
    this.brandRepository = brandRepository;
  }

  /**
   * 导入生成：品牌存在则修改，不存在则创建
   *
   * @param code 品牌代码
   * @param name 品牌名称
   * @param remark 备注（可空）
   * @return 品牌聚合
   */
  public Brand importGenerate(String code, String name, @Nullable String remark) {
    // 按代码查品牌
    Brand model = brandRepository.getByCode(code);

    // 不存在则创建，存在则修改
    if (model == null) {
      model = brandFactory.create(code, name, remark);
    } else {
      modify(model, code, name, remark);
    }

    return model;
  }

  /**
   * 修改品牌
   *
   * @param id 品牌Id
   * @param code 品牌代码
   * @param name 品牌名称
   * @param remark 备注（可空）
   * @return 品牌聚合
   */
  public Brand modify(long id, String code, String name, @Nullable String remark) {
    Brand model = brandRepository.get(id);
    if (model == null) {
      throw new DomainException("品牌" + id + "不存在!");
    }
    modify(model, code, name, remark);
    return model;
  }

  /**
   * 修改品牌
   *
   * @param model 品牌
   * @param code 品牌代码
   * @param name 品牌名称
   * @param remark 备注（可空）
   */
  public void modify(Brand model, String code, String name, @Nullable String remark) {
    // 校验品牌代码唯一
    if (brandRepository.isExistByCodeExcludeId(code, model.getId())) {
      throw new DomainException("品牌代码" + code + "已存在!");
    }
    model.modify(code, name, remark);
  }
}
