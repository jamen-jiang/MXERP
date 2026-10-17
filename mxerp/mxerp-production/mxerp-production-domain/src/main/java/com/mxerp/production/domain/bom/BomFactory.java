package com.mxerp.production.domain.bom;

import com.mxerp.shared.domain.exception.DomainException;
import java.time.LocalDateTime;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** BOM 工厂，负责创建 BOM */
@NullMarked
public class BomFactory {

  /** BOM 仓储 */
  private final BomRepository bomRepository;

  /**
   * 构造器注入
   *
   * @param bomRepository BOM 仓储
   */
  public BomFactory(BomRepository bomRepository) {
    this.bomRepository = bomRepository;
  }

  /**
   * 创建 BOM
   *
   * @param skuId 父件SKU
   * @param version 版本号
   * @param status 状态
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @return BOM 聚合
   * @throws DomainException 父件SKU + 版本已存在时抛出
   */
  public Bom create(
      Long skuId,
      String version,
      String status,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    // 校验父件SKU + 版本唯一
    if (bomRepository.isExistBySkuIdAndVersionExcludeId(skuId, version)) {
      throw new DomainException("该父件的 BOM 版本" + version + "已存在!");
    }
    return new Bom(skuId, version, status, effectiveFrom, effectiveTo, remark);
  }
}
