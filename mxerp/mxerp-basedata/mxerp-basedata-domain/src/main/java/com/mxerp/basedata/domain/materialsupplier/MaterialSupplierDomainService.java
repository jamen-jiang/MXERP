package com.mxerp.basedata.domain.materialsupplier;

import com.mxerp.basedata.domain.material.MaterialRepository;
import com.mxerp.basedata.domain.supplier.SupplierRepository;
import com.mxerp.basedata.domain.unit.UnitRepository;
import com.mxerp.basedata.domain.variant.Variant;
import com.mxerp.basedata.domain.variant.VariantRepository;
import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 物料供应商领域服务，负责跨聚合编排
 *
 * <p>这里没有 {@code importGenerate}：同一供应商同一物料的报价天生有多张卡，按业务键查不到唯一一条，
 * 批量导入只能走「新建一张卡」。
 */
@NullMarked
public class MaterialSupplierDomainService {

  /** 物料供应商工厂 */
  private final MaterialSupplierFactory materialSupplierFactory;

  /** 物料供应商仓储 */
  private final MaterialSupplierRepository materialSupplierRepository;

  /** 供应商仓储 */
  private final SupplierRepository supplierRepository;

  /** 物料仓储 */
  private final MaterialRepository materialRepository;

  /** SKU 仓储 */
  private final VariantRepository variantRepository;

  /** 单位仓储：报价单位必须存在 */
  private final UnitRepository unitRepository;

  /**
   * 构造器注入
   *
   * @param materialSupplierFactory 物料供应商工厂
   * @param materialSupplierRepository 物料供应商仓储
   * @param supplierRepository 供应商仓储
   * @param materialRepository 物料仓储
   * @param variantRepository SKU 仓储
   * @param unitRepository 单位仓储
   */
  public MaterialSupplierDomainService(
      MaterialSupplierFactory materialSupplierFactory,
      MaterialSupplierRepository materialSupplierRepository,
      SupplierRepository supplierRepository,
      MaterialRepository materialRepository,
      VariantRepository variantRepository,
      UnitRepository unitRepository) {
    this.materialSupplierFactory = materialSupplierFactory;
    this.materialSupplierRepository = materialSupplierRepository;
    this.supplierRepository = supplierRepository;
    this.materialRepository = materialRepository;
    this.variantRepository = variantRepository;
    this.unitRepository = unitRepository;
  }

  /**
   * 新建供应商卡
   *
   * <p>新卡在生效点接替同键的旧卡：旧卡的失效时间收口到新卡生效点，其它区间交叠的直接拒绝。
   *
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空，个别度数的特别价）
   * @param supplierId 供应商Id
   * @param supplierProductCode 供应商产品代码（可空）
   * @param supplierProductName 供应商产品名称（可空）
   * @param price 采购价格
   * @param discount 折扣（%，可空）
   * @param currencyId 币种Id
   * @param unitId 单位Id（可空，空表示按物料基本单位报价）
   * @param moq 起订量（可空）
   * @param leadTime 货期（天，可空）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   * @return 供应商卡聚合
   * @throws DomainException 供应商、物料、SKU 不存在、SKU 不属于该物料、或生效区间重叠时抛出
   */
  public MaterialSupplier create(
      long materialId,
      @Nullable Long variantId,
      long supplierId,
      @Nullable String supplierProductCode,
      @Nullable String supplierProductName,
      BigDecimal price,
      @Nullable BigDecimal discount,
      long currencyId,
      @Nullable Long unitId,
      @Nullable BigDecimal moq,
      @Nullable Integer leadTime,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable Integer sequence,
      @Nullable String remark) {
    validateReferences(supplierId, materialId, variantId);
    if (unitId != null && unitRepository.get(unitId) == null) {
      throw new DomainException("单位" + unitId + "不存在!");
    }
    handOver(supplierId, materialId, variantId, effectiveFrom, effectiveTo);
    return materialSupplierFactory.create(
        materialId,
        variantId,
        supplierId,
        supplierProductCode,
        supplierProductName,
        price,
        discount,
        currencyId,
        unitId,
        moq,
        leadTime,
        effectiveFrom,
        effectiveTo,
        sequence,
        remark);
  }

  /**
   * 修改供应商卡
   *
   * <p>供应商、物料、SKU 与价格不可改；改有效期时，同键的其它卡不许与新区间重叠。
   *
   * @param id 供应商卡Id
   * @param supplierProductCode 供应商产品代码（可空）
   * @param supplierProductName 供应商产品名称（可空）
   * @param discount 折扣（%，可空）
   * @param moq 起订量（可空）
   * @param leadTime 货期（天，可空）
   * @param effectiveFrom 生效时间
   * @param effectiveTo 失效时间（可空）
   * @param sequence 排序（可空）
   * @param remark 备注（可空）
   * @return 供应商卡聚合
   * @throws DomainException 供应商卡不存在、或生效区间与同键的其它卡重叠时抛出
   */
  public MaterialSupplier modify(
      long id,
      @Nullable String supplierProductCode,
      @Nullable String supplierProductName,
      @Nullable BigDecimal discount,
      @Nullable BigDecimal moq,
      @Nullable Integer leadTime,
      LocalDateTime effectiveFrom,
      @Nullable LocalDateTime effectiveTo,
      @Nullable Integer sequence,
      @Nullable String remark) {
    MaterialSupplier model = getExist(id);
    checkNoOverlap(
        model.getSupplierId(),
        model.getMaterialId(),
        model.getVariantId(),
        effectiveFrom,
        effectiveTo,
        id);
    model.modify(
        supplierProductCode,
        supplierProductName,
        discount,
        moq,
        leadTime,
        effectiveFrom,
        effectiveTo,
        sequence,
        remark);
    return model;
  }

  // ==================== 私有：取数与校验 ====================

  /**
   * 新卡接替旧卡：覆盖新卡生效点的旧卡收口，其它交叠直接拒绝
   *
   * @param supplierId 供应商Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空）
   * @param from 新卡生效时间
   * @param to 新卡失效时间（可空）
   * @throws DomainException 区间交叠且不能收口时抛出
   */
  private void handOver(
      long supplierId,
      long materialId,
      @Nullable Long variantId,
      LocalDateTime from,
      @Nullable LocalDateTime to) {
    List<MaterialSupplier> cards =
        materialSupplierRepository.getListBySupplierIdAndMaterialIdAndVariantId(
            supplierId, materialId, variantId);

    List<MaterialSupplier> predecessors = new ArrayList<>();
    for (MaterialSupplier card : cards) {
      if (card.getEffectiveFrom().isBefore(from) && card.covers(from)) {
        predecessors.add(card);
        continue;
      }
      if (card.overlaps(from, to)) {
        throw new DomainException("该供应商这个物料的新价格有效期与已有价格重叠!");
      }
    }

    for (MaterialSupplier predecessor : predecessors) {
      predecessor.closeAt(from);
    }
  }

  /**
   * 校验修改后的区间不与同键的其它卡重叠
   *
   * @param supplierId 供应商Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空）
   * @param from 生效时间
   * @param to 失效时间（可空）
   * @param excludeId 排除的供应商卡Id
   * @throws DomainException 区间重叠时抛出
   */
  private void checkNoOverlap(
      long supplierId,
      long materialId,
      @Nullable Long variantId,
      LocalDateTime from,
      @Nullable LocalDateTime to,
      long excludeId) {
    List<MaterialSupplier> cards =
        materialSupplierRepository.getListBySupplierIdAndMaterialIdAndVariantId(
            supplierId, materialId, variantId);
    for (MaterialSupplier card : cards) {
      if (!Objects.equals(card.getId(), excludeId) && card.overlaps(from, to)) {
        throw new DomainException("该供应商卡的有效期与同一供应商同一物料的其它价格重叠!");
      }
    }
  }

  /**
   * 校验供应商、物料、SKU 存在，且 SKU 属于该物料
   *
   * @param supplierId 供应商Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空）
   * @throws DomainException 引用不存在或 SKU 不属于该物料时抛出
   */
  private void validateReferences(long supplierId, long materialId, @Nullable Long variantId) {
    if (supplierRepository.get(supplierId) == null) {
      throw new DomainException("供应商" + supplierId + "不存在!");
    }
    if (materialRepository.get(materialId) == null) {
      throw new DomainException("物料" + materialId + "不存在!");
    }
    if (variantId != null) {
      Variant variant = variantRepository.get(variantId);
      if (variant == null) {
        throw new DomainException("SKU" + variantId + "不存在!");
      }
      if (!variant.getMaterialId().equals(materialId)) {
        throw new DomainException("SKU" + variantId + "不属于物料" + materialId + "!");
      }
    }
  }

  /**
   * 取供应商卡，不存在则抛异常
   *
   * @param id 供应商卡Id
   * @return 供应商卡聚合
   * @throws DomainException 供应商卡不存在时抛出
   */
  private MaterialSupplier getExist(long id) {
    MaterialSupplier model = materialSupplierRepository.get(id);
    if (model == null) {
      throw new DomainException("供应商卡" + id + "不存在!");
    }
    return model;
  }
}
