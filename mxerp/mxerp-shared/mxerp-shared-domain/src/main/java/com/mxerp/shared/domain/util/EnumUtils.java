package com.mxerp.shared.domain.util;

import com.mxerp.shared.domain.enums.BaseEnum;
import org.jspecify.annotations.NonNull;
import org.jspecify.annotations.Nullable;

/** 枚举工具 */
public final class EnumUtils {

  private EnumUtils() {}

  /**
   * 按编码查找枚举
   *
   * @param enumClass 枚举类型
   * @param code 编码
   * @param <E> 枚举类型
   * @return 枚举（可空），不存在返回 null
   */
  @Nullable
  public static <E extends Enum<E> & BaseEnum> E fromCode(
      @NonNull Class<E> enumClass, @NonNull String code) {
    for (E e : enumClass.getEnumConstants()) {
      if (e.getCode().equals(code)) {
        return e;
      }
    }
    return null;
  }
}
