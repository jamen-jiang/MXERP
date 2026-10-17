package com.mxerp.system.application.department.command;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 修改部门命令 */
@NullMarked
@Getter
@Schema(description = "修改部门命令")
public class ModifyDepartmentCommand extends AddDepartmentCommand {

  /** 部门Id */
  @Schema(description = "部门Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private final Long id;

  /**
   * 构造器
   *
   * @param id 部门Id
   * @param companyId 公司Id
   * @param parentId 父Id（可空）
   * @param code 组织机构代码
   * @param name 组织机构名称
   * @param telephone 电话号码（可空）
   * @param fax 传真（可空）
   * @param email 邮箱（可空）
   * @param sort 排序（可空）
   * @param address 地址（可空）
   * @param remark 备注（可空）
   */
  public ModifyDepartmentCommand(
      Long id,
      Long companyId,
      @Nullable Long parentId,
      String code,
      String name,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Integer sort,
      @Nullable String address,
      @Nullable String remark) {
    super(companyId, parentId, code, name, telephone, fax, email, sort, address, remark);
    this.id = id;
  }
}
