package com.mxerp.system.application.department.command;

import com.mxerp.shared.application.cqrs.Command;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 添加部门命令 */
@NullMarked
@Getter
@Schema(description = "添加部门命令")
public class AddDepartmentCommand extends Command {

  /** 公司Id */
  @Schema(description = "公司Id", requiredMode = Schema.RequiredMode.REQUIRED)
  private final Long companyId;

  /** 父Id（可空） */
  @Schema(description = "父Id")
  @Nullable
  private final Long parentId;

  /** 组织机构代码 */
  @Schema(description = "组织机构代码", requiredMode = Schema.RequiredMode.REQUIRED)
  private final String code;

  /** 组织机构名称 */
  @Schema(description = "组织机构名称", requiredMode = Schema.RequiredMode.REQUIRED)
  private final String name;

  /** 电话号码（可空） */
  @Schema(description = "电话号码")
  @Nullable
  private final String telephone;

  /** 传真（可空） */
  @Schema(description = "传真")
  @Nullable
  private final String fax;

  /** 邮箱（可空） */
  @Schema(description = "邮箱")
  @Nullable
  private final String email;

  /** 排序（可空） */
  @Schema(description = "排序")
  @Nullable
  private final Integer sort;

  /** 地址（可空） */
  @Schema(description = "地址")
  @Nullable
  private final String address;

  /** 备注（可空） */
  @Schema(description = "备注")
  @Nullable
  private final String remark;

  /**
   * 构造器
   *
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
  public AddDepartmentCommand(
      long companyId,
      @Nullable Long parentId,
      String code,
      String name,
      @Nullable String telephone,
      @Nullable String fax,
      @Nullable String email,
      @Nullable Integer sort,
      @Nullable String address,
      @Nullable String remark) {
    this.companyId = companyId;
    this.parentId = parentId;
    this.code = code;
    this.name = name;
    this.telephone = telephone;
    this.fax = fax;
    this.email = email;
    this.sort = sort;
    this.address = address;
    this.remark = remark;
  }
}
