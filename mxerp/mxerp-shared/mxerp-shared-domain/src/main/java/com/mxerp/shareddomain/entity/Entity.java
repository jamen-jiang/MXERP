package com.mxerp.shareddomain.entity;

import org.jspecify.annotations.NullMarked;

/** 实体接口 */
@NullMarked
public interface Entity {
  /** 获取主键 */
  Long getId();
}
