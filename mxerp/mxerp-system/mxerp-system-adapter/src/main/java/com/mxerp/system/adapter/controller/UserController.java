package com.mxerp.system.adapter.controller;

import com.mxerp.shared.application.response.PageResponse;
import com.mxerp.system.application.user.command.AddUserCommand;
import com.mxerp.system.application.user.command.ModifyUserCommand;
import com.mxerp.system.application.user.command.RemoveUserCommand;
import com.mxerp.system.application.user.query.GetPageUsersQuery;
import com.mxerp.system.application.user.query.GetUserByIdQuery;
import com.mxerp.system.application.user.query.UserResponse;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
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

/** 用户管理接口 */
@Tag(name = "用户管理", description = "用户增删改查")
@RestController
@RequestMapping("/User")
public class UserController {

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
  public UserController(CommandGateway commandGateway, QueryGateway queryGateway) {
    this.commandGateway = commandGateway;
    this.queryGateway = queryGateway;
  }

  /**
   * 分页查询用户
   *
   * @param query 分页查询条件
   * @return 分页响应
   */
  @PreAuthorize("@ss.hasPermi('System_User_View')")
  @Operation(summary = "分页查询用户")
  @GetMapping("/Page")
  @SuppressWarnings("unchecked")
  public PageResponse<UserResponse> page(@ParameterObject GetPageUsersQuery query) {
    return queryGateway.query(query, PageResponse.class).join();
  }

  /**
   * 获取用户详情（含角色列表 + 岗位列表）
   *
   * @param query 按Id查询条件
   * @return 用户响应
   */
  @PreAuthorize("@ss.hasPermi('System_User_View')")
  @Operation(summary = "获取用户详情")
  @GetMapping("/Detail")
  public UserResponse detail(@ParameterObject GetUserByIdQuery query) {
    return queryGateway.query(query, UserResponse.class).join();
  }

  /**
   * 添加用户
   *
   * @param command 添加用户命令
   * @return 新用户Id
   */
  @PreAuthorize("@ss.hasPermi('System_User_Add')")
  @Operation(summary = "添加用户")
  @PostMapping("/Add")
  public Long add(@RequestBody AddUserCommand command) {
    return commandGateway.sendAndWait(command, Long.class);
  }

  /**
   * 修改用户
   *
   * @param command 修改用户命令
   */
  @PreAuthorize("@ss.hasPermi('System_User_Modify')")
  @Operation(summary = "修改用户")
  @PutMapping("/Modify")
  public void modify(@RequestBody ModifyUserCommand command) {
    commandGateway.sendAndWait(command);
  }

  /**
   * 删除用户
   *
   * @param id 用户Id
   */
  @PreAuthorize("@ss.hasPermi('System_User_Remove')")
  @Operation(summary = "删除用户")
  @DeleteMapping("/Remove")
  public void remove(@RequestParam Long id) {
    commandGateway.sendAndWait(new RemoveUserCommand(id));
  }
}
