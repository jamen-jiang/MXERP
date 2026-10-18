package com.mxerp.production.domain.bom;

import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * BOM 行数据
 *
 * @param sequence 行号
 * @param spuId 子件SPU
 * @param skuId 子件SKU
 * @param quantity 单位用量（生产 1 个父件消耗的子件数量）
 * @param lossRate 损耗率（百分比，传 0 表示无损耗）
 * @param unitId 单位Id
 * @param issueMode 发料方式编码
 * @param issueTrigger 发料触发编码（手工领料 / 报工倒冲 / 入库倒冲）
 * @param warehouseId 发料仓库Id（可空，空表示下达时回落到工单的默认仓）
 * @param isKeyPart 是否关键件（缺料即停线）
 * @param remark 备注（可空）
 */
@NullMarked
public record BomLineData(
    Integer sequence,
    Long spuId,
    Long skuId,
    BigDecimal quantity,
    BigDecimal lossRate,
    Long unitId,
    String issueMode,
    String issueTrigger,
    @Nullable Long warehouseId,
    boolean isKeyPart,
    @Nullable String remark) {}
