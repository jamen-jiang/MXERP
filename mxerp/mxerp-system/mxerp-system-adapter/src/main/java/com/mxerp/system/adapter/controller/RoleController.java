package com.mxerp.system.adapter.controller;

import com.mxerp.shared.application.response.PageResponse;
import com.mxerp.system.application.role.command.AddRoleCommand;
import com.mxerp.system.application.role.command.ModifyRoleCommand;
import com.mxerp.system.application.role.command.RemoveRoleCommand;
import com.mxerp.system.application.role.query.GetPageRolesQuery;
import com.mxerp.system.application.role.query.GetRoleByIdQuery;
import com.mxerp.system.application.role.query.GetRolesQuery;
import com.mxerp.system.application.role.query.RoleResponse;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import java.util.List;
import org.axonframework.messaging.commandhandling.gateway.CommandGateway;
import org.axonframework.messaging.queryhandling.gateway.QueryGateway;
import org.springdoc.core.annotations.ParameterObject;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

/** 角色管理接口 */
@Tag(name = "角色管理", description = "角色增删改查")
@RestController
@RequestMapping("/Role")
public class RoleController {

  /** 命令网关 */
  private final CommandGateway commandGateway;

  /** 查询网关 */
  private final QueryGateway queryGateway;

  /**
   * 构造器注入
   *
   * @param commandGateway 命令网关
   * @param queryGateway 查询网关
   */
  public RoleController(CommandGateway commandGateway, QueryGateway queryGateway) {
    this.commandGateway = commandGateway;
    this.queryGateway = queryGateway;
  }

  /**
   * 获取角色列表
   *
   * @return 角色响应列表
   */
  @Operation(summary = "获取角色列表")
  @GetMapping("/Query")
  public List<RoleResponse> query() {
    return queryGateway.queryMany(new GetRolesQuery(), RoleResponse.class).join();
  }

  /**
   * 分页查询角色
   *
   * @param query 分页查询条件
   * @return 分页响应
   */
  @PreAuthorize("@ss.hasPermi('System_Role_View')")
  @Operation(summary = "分页查询角色")
  @GetMapping("/Page")
  @SuppressWarnings("unchecked")
  public PageResponse<RoleResponse> page(@ParameterObject GetPageRolesQuery query) {
    return queryGateway.query(query, PageResponse.class).join();
  }

  /**
   * 获取角色详情
   *
   * @param query 按Id查询条件
   * @return 角色响应
   */
  @PreAuthorize("@ss.hasPermi('System_Role_View')")
  @Operation(summary = "获取角色详情")
  @GetMapping("/Detail")
  public RoleResponse detail(@ParameterObject GetRoleByIdQuery query) {
    return queryGateway.query(query, RoleResponse.class).join();
  }

  /**
   * 添加角色
   *
   * @param command 添加角色命令
   * @return 新角色Id
   */
  @PreAuthorize("@ss.hasPermi('System_Role_Add')")
  @Operation(summary = "添加角色")
  @PostMapping("/Add")
  public Long add(@RequestBody AddRoleCommand command) {
    return commandGateway.sendAndWait(command, Long.class);
  }

  /**
   * 修改角色
   *
   * @param command 修改角色命令
   */
  @PreAuthorize("@ss.hasPermi('System_Role_Modify')")
  @Operation(summary = "修改角色")
  @PutMapping("/Modify")
  public void modify(@RequestBody ModifyRoleCommand command) {
    commandGateway.sendAndWait(command);
  }

  /**
   * 删除角色
   *
   * @param id 角色Id
   */
  @PreAuthorize("@ss.hasPermi('System_Role_Remove')")
  @Operation(summary = "删除角色")
  @DeleteMapping("/Remove")
  public void remove(@RequestParam Long id) {
    commandGateway.sendAndWait(new RemoveRoleCommand(id));
  }
}
