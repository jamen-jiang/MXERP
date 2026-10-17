package com.mxerp.basedata.domain.sku;

import com.mxerp.shared.domain.event.DomainEvent;
import java.util.TreeMap;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** SKU 删除事件 */
@NullMarked
@Getter
public class SkuDeletedEvent extends DomainEvent {

  /** 产品Id */
  private final Long spuId;

  /** 关键属性值 */
  @Nullable private final TreeMap<String, Object> KeyAttributes;

  /**
   * 构造器
   *
   * @param sku SKU
   */
  public SkuDeletedEvent(Sku sku) {
    super(sku);
    this.spuId = sku.getSpuId();
    this.KeyAttributes = sku.getKeyAttributes();
  }
}
