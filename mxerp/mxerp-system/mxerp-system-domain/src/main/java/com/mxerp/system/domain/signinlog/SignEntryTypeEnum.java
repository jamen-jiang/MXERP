package com.mxerp.system.domain.signinlog;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 登录入口类型 */
@NullMarked
@Getter
public enum SignEntryTypeEnum implements BaseEnum {

  /** 登录 */
  SIGN_IN("SignIn", "登录"),

  /** 刷新令牌 */
  REFRESH_TOKEN("RefreshToken", "刷新令牌"),

  /** 登出 */
  SIGN_OUT("SingOut", "登出");

  /** 编码 */
  private final String code;

  /** 名称 */
  private final String name;

  /**
   * 构造器
   *
   * @param code 编码
   * @param name 名称
   */
  SignEntryTypeEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
