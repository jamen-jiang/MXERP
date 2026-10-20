package com.mxerp.production.domain.bom;

import com.mxerp.shared.domain.exception.DomainException;
import java.time.LocalDateTime;
import java.util.ArrayList;
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
   * <p>导入只维护草稿版本：状态是发布/作废推进出来的，不接受 Excel 里填的状态，否则一份表就能把已发布
   * 配方退回草稿、把冻结的版本改开。
   *
   * @param materialId 父件SPU
   * @param variantId 父件SKU
   * @param version 版本号
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @return BOM 聚合
   */
  public Bom importGenerate(
      Long materialId,
      Long variantId,
      String version,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    // 按父件SKU + 版本查 BOM
    Bom model = bomRepository.getByVariantIdAndVersion(variantId, version);

    // 不存在则创建，存在则修改
    if (model == null) {
      model = bomFactory.create(materialId, variantId, version, effectiveFrom, effectiveTo, remark);
    } else {
      modify(model, materialId, variantId, version, effectiveFrom, effectiveTo, remark);
    }

    return model;
  }

  /**
   * 修改 BOM 表头
   *
   * @param id BOM Id
   * @param materialId 父件SPU
   * @param variantId 父件SKU
   * @param version 版本号
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @return BOM 聚合
   * @throws DomainException BOM 不存在时抛出
   */
  public Bom modify(
      long id,
      Long materialId,
      Long variantId,
      String version,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    Bom model = getExist(id);
    modify(model, materialId, variantId, version, effectiveFrom, effectiveTo, remark);
    return model;
  }

  /**
   * 修改 BOM 表头
   *
   * @param model BOM 聚合
   * @param materialId 父件SPU
   * @param variantId 父件SKU
   * @param version 版本号
   * @param effectiveFrom 生效时间（可空）
   * @param effectiveTo 失效时间（可空）
   * @param remark 备注（可空）
   * @throws DomainException 父件SKU + 版本与其它 BOM 冲突、或版本已不是草稿时抛出
   */
  public void modify(
      Bom model,
      Long materialId,
      Long variantId,
      String version,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    // 校验父件SKU + 版本唯一
    if (bomRepository.isExistByVariantIdAndVersionExcludeId(variantId, version, model.getId())) {
      throw new DomainException("该父件的 BOM 版本" + version + "已存在!");
    }
    model.modify(materialId, variantId, version, effectiveFrom, effectiveTo, remark);
  }

  // ==================== 版本与生效链 ====================

  /**
   * 发布版本，并按生效日期接力：生效更早的已发布版本自动收口到本版生效点
   *
   * <p>规则与 D365 的 BOM version 一致：同一父件任一时刻至多一个生效版本。所以发布前先跟其它已发布版本
   * 比区间——本版落在更晚的未来版本之内要拒绝；本版生效时间比已有版本晚的，那一版到点到期。
   *
   * @param id BOM Id
   * @return BOM 聚合
   * @throws DomainException BOM 不存在、生效区间与同父件的已发布版本重叠时抛出
   */
  public Bom publish(long id) {
    Bom model = getExist(id);
    LocalDateTime effectiveFrom = model.getEffectiveFrom();
    if (effectiveFrom == null) {
      throw new DomainException("BOM 版本" + model.getVersion() + "没有生效时间，不能发布");
    }

    List<Bom> released =
        bomRepository.getListByVariantId(model.getVariantId()).stream()
            .filter(b -> !Objects.equals(b.getId(), model.getId()))
            .filter(Bom::isReleased)
            .toList();

    // 1. 先全量校验：生效更早且覆盖本版起点的那一版由本版接棒，其余版本区间不许与本版交叠
    List<Bom> predecessors = new ArrayList<>();
    for (Bom other : released) {
      LocalDateTime otherFrom = other.getEffectiveFrom();
      if (otherFrom != null
          && otherFrom.isBefore(effectiveFrom)
          && other.isEffectiveAt(effectiveFrom)) {
        predecessors.add(other);
        continue;
      }
      if (other.overlaps(effectiveFrom, model.getEffectiveTo())) {
        throw new DomainException(
            "BOM 版本"
                + model.getVersion()
                + "的生效区间与版本"
                + other.getVersion()
                + "重叠!");
      }
    }

    // 2. 前一版收口到本版生效点，区间既不留缝也不重叠
    for (Bom predecessor : predecessors) {
      predecessor.closeAt(effectiveFrom);
    }

    model.publish();
    return model;
  }

  /**
   * 作废版本
   *
   * @param id BOM Id
   * @return BOM 聚合
   * @throws DomainException BOM 不存在、或不是已发布状态时抛出
   */
  public Bom obsolete(long id) {
    Bom model = getExist(id);
    model.obsolete();
    return model;
  }

  /**
   * 升版：把已发布版本的用料整单复制成一个新草稿
   *
   * <p>已发布版本冻结以后，改配方只能开新版本，复制是这一步唯一的省事办法：镜片一行子件常有十几列，
   * 手工重录既慢又容易抄错损耗率。新草稿沿用父件与全部行，只换版本号和生效区间，改完再走发布。
   *
   * @param id 源 BOM Id（一般是被替换的那一版）
   * @param version 新版本号
   * @param effectiveFrom 新版本生效时间（可空，发布前必填）
   * @param effectiveTo 新版本失效时间（可空）
   * @param remark 备注（可空）
   * @return 新草稿 BOM（由调用方落库）
   * @throws DomainException 源 BOM 不存在、源版本还是草稿、或新版本号已存在时抛出
   */
  public Bom revise(
      long id,
      String version,
      @Nullable LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable String remark) {
    Bom source = getExist(id);
    if (source.isDraft()) {
      throw new DomainException("BOM 版本" + source.getVersion() + "还是草稿，直接改就行，不必升版");
    }

    Bom model =
        bomFactory.create(
            source.getMaterialId(),
            source.getVariantId(),
            version,
            effectiveFrom,
            effectiveTo,
            remark);
    for (BomLine line : source.getBomLines()) {
      model.addLine(
          line.getSequence(),
          line.getMaterialId(),
          line.getVariantId(),
          line.getQuantity(),
          line.getLossRate(),
          line.getUnitId(),
          line.getIssueMode(),
          line.getIssueTrigger(),
          line.getWarehouseId(),
          line.getIsKeyPart(),
          line.getRemark());
    }
    return model;
  }

  // ==================== 私有 ====================

  /**
   * 取 BOM，不存在则抛异常
   *
   * @param id BOM Id
   * @return BOM 聚合
   * @throws DomainException 不存在时抛出
   */
  private Bom getExist(long id) {
    Bom model = bomRepository.get(id);
    if (model == null) {
      throw new DomainException("BOM" + id + "不存在!");
    }
    return model;
  }

  /**
   * 同步用料行（按行号匹配：删除不在输入中的、更新已存在的、添加新的）
   *
   * @param bom BOM 聚合
   * @param lines 目标用料行列表（可空，null 视为清空）
   * @throws DomainException BOM 已不是草稿时抛出
   */
  public void syncLines(Bom bom, @Nullable List<BomLineData> lines) {
    List<BomLineData> target = lines == null ? List.of() : lines;

    // 1. 删除未在输入中出现的行（按行号匹配）
    List<BomLine> toRemoves =
        bom.getBomLines().stream()
            .filter(
                i ->
                    target.stream()
                        .noneMatch(d -> Objects.equals(d.sequence(), i.getSequence())))
            .toList();
    for (BomLine line : toRemoves) {
      bom.deleteLine(line.getId());
    }

    // 2. 建立"行号 → 已存在行Id"的映射（只统计未删除的）
    Map<Integer, Long> existingDict =
        bom.getBomLines().stream()
            .filter(i -> !i.getIsDeleted())
            .collect(Collectors.toMap(BomLine::getSequence, BomLine::getId, (a, b) -> a));

    // 3. 遍历输入：存在则修改，不存在则添加
    for (BomLineData data : target) {
      Long id = existingDict.get(data.sequence());
      if (id != null) {
        // 存在：修改
        bom.modifyLine(
            id,
            data.sequence(),
            data.materialId(),
            data.variantId(),
            data.quantity(),
            data.lossRate(),
            data.unitId(),
            data.issueMode(),
            data.issueTrigger(),
            data.warehouseId(),
            data.isKeyPart(),
            data.remark());
      } else {
        // 不存在：添加
        bom.addLine(
            data.sequence(),
            data.materialId(),
            data.variantId(),
            data.quantity(),
            data.lossRate(),
            data.unitId(),
            data.issueMode(),
            data.issueTrigger(),
            data.warehouseId(),
            data.isKeyPart(),
            data.remark());
      }
    }
  }
}
