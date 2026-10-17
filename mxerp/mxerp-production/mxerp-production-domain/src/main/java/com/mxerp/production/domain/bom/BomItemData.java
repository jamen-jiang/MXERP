package com.mxerp.production.domain.bom;

import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * BOM 行数据
 *
 * @param sequence 行号
 * @param componentSkuId 子件SKU
 * @param quantity 单位用量（生产 1 个父件消耗的子件数量）
 * @param lossRate 损耗率（百分比，传 0 表示无损耗）
 * @param unitId 单位Id
 * @param issueMode 发料方式编码
 * @param isKeyPart 是否关键件（缺料即停线）
 * @param remark 备注（可空）
 */
@NullMarked
public record BomItemData(
    Integer sequence,
    Long componentSkuId,
    BigDecimal quantity,
    BigDecimal lossRate,
    Long unitId,
    String issueMode,
    boolean isKeyPart,
    @Nullable String remark) {}
