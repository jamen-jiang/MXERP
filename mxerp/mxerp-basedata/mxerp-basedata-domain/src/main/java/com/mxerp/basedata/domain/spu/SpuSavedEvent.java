package com.mxerp.basedata.domain.spu;

import com.mxerp.shared.domain.event.DomainEvent;
import java.util.List;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 产品保存事件（新增或修改） */
@NullMarked
@Getter
public class SpuSavedEvent extends DomainEvent {

  /** 附件Id集合（可空） */
  @Nullable private final List<Long> attachmentIds;

  /**
   * 构造器
   *
   * @param spu 产品
   * @param attachmentIds 附件Id集合（可空）
   */
  public SpuSavedEvent(Spu spu, @Nullable List<Long> attachmentIds) {
    super(spu);
    this.attachmentIds = attachmentIds;
  }
}
