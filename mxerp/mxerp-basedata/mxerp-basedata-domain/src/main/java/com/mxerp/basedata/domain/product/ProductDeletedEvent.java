package com.mxerp.basedata.domain.product;

import com.mxerp.shared.domain.event.DomainEvent;
import org.jspecify.annotations.NullMarked;

/** 产品删除事件 */
@NullMarked
public class ProductDeletedEvent extends DomainEvent {

  /**
   * 构造器
   *
   * @param product 产品
   */
  public ProductDeletedEvent(Product product) {
    super(product);
  }
}
