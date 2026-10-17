package com.mxerp.shared.domain.event;


import com.mxerp.shared.domain.aggregate.OrderAggregateRoot;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 订单新增事件（用于生成流水号） */
@NullMarked
@Getter
public class OrderAddedEvent extends DomainEvent {

  /** 类型 */
  private final String type;

  /**
   * 构造器
   *
   * @param order 订单
   */
  public OrderAddedEvent(OrderAggregateRoot order) {
    super(order);
    this.type = order.getSerialNumberType();
  }
}
