package com.mxerp.basedata.domain.spu;

import com.mxerp.shared.domain.event.DomainEvent;
import org.jspecify.annotations.NullMarked;

/** 产品删除事件 */
@NullMarked
public class SpuDeletedEvent extends DomainEvent {

  /**
   * 构造器
   *
   * @param spu 产品
   */
  public SpuDeletedEvent(Spu spu) {
    super(spu);
  }
}
