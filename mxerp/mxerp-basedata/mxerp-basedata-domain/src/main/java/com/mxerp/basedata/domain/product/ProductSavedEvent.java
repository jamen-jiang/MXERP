package com.mxerp.basedata.domain.product;

import com.mxerp.shared.domain.event.DomainEvent;
import java.util.List;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 产品保存事件（新增或修改） */
@NullMarked
@Getter
public class ProductSavedEvent extends DomainEvent {

  /** 附件Id集合（可空） */
  @Nullable private final List<Long> attachmentIds;

  /**
   * 构造器
   *
   * @param product 产品
   * @param attachmentIds 附件Id集合（可空）
   */
  public ProductSavedEvent(Product product, @Nullable List<Long> attachmentIds) {
    super(product);
    this.attachmentIds = attachmentIds;
  }
}
