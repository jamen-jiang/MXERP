package com.mxerp.basedata.domain.variant;

import com.mxerp.shared.domain.event.DomainEvent;
import java.util.TreeMap;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** SKU 删除事件 */
@NullMarked
@Getter
public class VariantDeletedEvent extends DomainEvent {

  /** 产品Id */
  private final Long productId;

  /** 规格属性值（删除那一刻快照，代码 → 值） */
  @Nullable private final TreeMap<String, Object> productAttributes;

  /**
   * 构造器
   *
   * @param variant SKU
   */
  public VariantDeletedEvent(Variant variant) {
    super(variant);
    this.productId = variant.getProductId();
    this.productAttributes = variant.productAttributeValues();
  }
}
