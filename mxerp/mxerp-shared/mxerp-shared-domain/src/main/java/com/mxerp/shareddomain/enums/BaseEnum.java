package com.mxerp.shareddomain.enums;

import org.jspecify.annotations.NullMarked;

/** 枚举基础接口 */
@NullMarked
public interface BaseEnum {

  /**
   * 获取编码
   *
   * @return 编码
   */
  String getCode();

  /**
   * 获取名称
   *
   * @return 名称
   */
  String getName();
}
