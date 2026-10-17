package com.mxerp.system.domain.privilege;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 权限对象 */
@NullMarked
@Getter
public enum MasterEnum implements BaseEnum {

  /** 角色 */
  ROLE("Role", "角色"),

  /** 用户 */
  USER("User", "用户"),

  /** 部门 */
  DEPARTMENT("Department", "部门"),

  /** 岗位 */
  POST("Post", "岗位");

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
  MasterEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
