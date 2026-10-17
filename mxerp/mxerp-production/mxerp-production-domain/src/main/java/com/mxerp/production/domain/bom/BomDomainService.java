package com.mxerp.production.domain.bom;

import com.mxerp.shared.domain.exception.DomainException;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.stream.Collectors;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** BOM 领域服务，负责跨聚合编排与用料行同步 */
@NullMarked
public class BomDomainService {

  /** BOM 工厂 */
  private final BomFactory bomFactory;

  /** BOM 仓储 */
  private final BomRepository bomRepository;

  /**
   * 构造器注入
   *
   * @param bomFactory BOM 工厂
   * @param bomRepository BOM 仓储
   */
  public BomDomainService(BomFactory bomFactory, BomRepository bomRepository) {
    this.bomFactory = bomFactory;
    this.bomRepository = bomRepository;
  }

  /**
   * 导入生成：BOM 存在则修改，不存在则创建
   *
   * @param skuId 父件SKU
   * @param version 版本号
   * @param status 状态
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @return BOM 聚合
   */
  public Bom importGenerate(
      Long skuId,
      String version,
      String status,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    // 按父件SKU + 版本查 BOM
    Bom model = bomRepository.getBySkuIdAndVersion(skuId, version);

    // 不存在则创建，存在则修改
    if (model == null) {
      model = bomFactory.create(skuId, version, status, effectiveFrom, effectiveTo, remark);
    } else {
      modify(model, skuId, version, status, effectiveFrom, effectiveTo, remark);
    }

    return model;
  }

  /**
   * 修改 BOM 表头
   *
   * @param id BOM Id
   * @param skuId 父件SKU
   * @param version 版本号
   * @param status 状态
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @return BOM 聚合
   * @throws DomainException BOM 不存在时抛出
   */
  public Bom modify(
      long id,
      Long skuId,
      String version,
      String status,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    Bom model = bomRepository.get(id);
    if (model == null) {
      throw new DomainException("BOM" + id + "不存在!");
    }
    modify(model, skuId, version, status, effectiveFrom, effectiveTo, remark);
    return model;
  }

  /**
   * 修改 BOM 表头
   *
   * @param model BOM 聚合
   * @param skuId 父件SKU
   * @param version 版本号
   * @param status 状态
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @throws DomainException 父件SKU + 版本与其它 BOM 冲突时抛出
   */
  public void modify(
      Bom model,
      Long skuId,
      String version,
      String status,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    // 校验父件SKU + 版本唯一
    if (bomRepository.isExistBySkuIdAndVersionExcludeId(skuId, version, model.getId())) {
      throw new DomainException("该父件的 BOM 版本" + version + "已存在!");
    }
    model.modify(skuId, version, status, effectiveFrom, effectiveTo, remark);
  }

  /**
   * 同步用料行（按行号匹配：删除不在输入中的、更新已存在的、添加新的）
   *
   * @param bom BOM 聚合
   * @param items 目标用料行列表（可空，null 视为清空）
   */
  public void syncItems(Bom bom, @Nullable List<BomItemData> items) {
    List<BomItemData> target = items == null ? List.of() : items;

    // 1. 删除未在输入中出现的行（按行号匹配）
    List<BomItem> toRemoves =
        bom.getBomItems().stream()
            .filter(
                i ->
                    target.stream()
                        .noneMatch(d -> Objects.equals(d.sequence(), i.getSequence())))
            .toList();
    for (BomItem item : toRemoves) {
      bom.deleteItem(item.getId());
    }

    // 2. 建立"行号 → 已存在行Id"的映射（只统计未删除的）
    Map<Integer, Long> existingDict =
        bom.getBomItems().stream()
            .filter(i -> !i.getIsDeleted())
            .collect(Collectors.toMap(BomItem::getSequence, BomItem::getId, (a, b) -> a));

    // 3. 遍历输入：存在则修改，不存在则添加
    for (BomItemData data : target) {
      Long id = existingDict.get(data.sequence());
      if (id != null) {
        // 存在：修改
        bom.modifyItem(
            id,
            data.sequence(),
            data.componentSkuId(),
            data.quantity(),
            data.lossRate(),
            data.unitId(),
            data.issueMode(),
            data.isKeyPart(),
            data.remark());
      } else {
        // 不存在：添加
        bom.addItem(
            data.sequence(),
            data.componentSkuId(),
            data.quantity(),
            data.lossRate(),
            data.unitId(),
            data.issueMode(),
            data.isKeyPart(),
            data.remark());
      }
    }
  }
}
