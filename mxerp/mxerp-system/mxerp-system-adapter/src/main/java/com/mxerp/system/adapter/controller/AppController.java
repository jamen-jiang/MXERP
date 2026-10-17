package com.mxerp.system.adapter.controller;

import com.mxerp.system.application.app.command.AddAppCommand;
import com.mxerp.system.application.app.command.ModifyAppCommand;
import com.mxerp.system.application.app.command.RemoveAppCommand;
import com.mxerp.system.application.app.query.AppResponse;
import com.mxerp.system.application.app.query.GetAppByIdQuery;
import com.mxerp.system.application.app.query.GetAppsQuery;
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

/** 应用（菜单/权限资源）管理接口 */
@Tag(name = "应用管理", description = "应用增删改查与当前用户权限")
@RestController
@RequestMapping("/App")
public class AppController {

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
  public AppController(CommandGateway commandGateway, QueryGateway queryGateway) {
    this.commandGateway = commandGateway;
    this.queryGateway = queryGateway;
  }

  /**
   * 获取应用树（当前登录用户可见的菜单）
   *
   * @param query 查询条件
   * @return 应用响应列表
   */
  @Operation(summary = "获取当前用户应用树")
  @GetMapping("/GetCurrentUserApps")
  public List<AppResponse> getCurrentUserApps(@ParameterObject GetAppsQuery query) {
    return queryGateway.queryMany(query, AppResponse.class).join();
  }

  /**
   * 获取应用详情
   *
   * @param query 按Id查询条件
   * @return 应用响应
   */
  @PreAuthorize("@ss.hasPermi('System_App_View')")
  @Operation(summary = "获取应用详情")
  @GetMapping("/Detail")
  public AppResponse detail(@ParameterObject GetAppByIdQuery query) {
    return queryGateway.query(query, AppResponse.class).join();
  }

  /**
   * 添加应用
   *
   * @param command 添加应用命令
   * @return 新应用Id
   */
  @PreAuthorize("@ss.hasPermi('System_App_Add')")
  @Operation(summary = "添加应用")
  @PostMapping("/Add")
  public Long add(@RequestBody AddAppCommand command) {
    return commandGateway.sendAndWait(command, Long.class);
  }

  /**
   * 修改应用
   *
   * @param command 修改应用命令
   */
  @PreAuthorize("@ss.hasPermi('System_App_Modify')")
  @Operation(summary = "修改应用")
  @PutMapping("/Modify")
  public void modify(@RequestBody ModifyAppCommand command) {
    commandGateway.sendAndWait(command);
  }

  /**
   * 删除应用（含子孙）
   *
   * @param id 应用Id
   */
  @PreAuthorize("@ss.hasPermi('System_App_Remove')")
  @Operation(summary = "删除应用")
  @DeleteMapping("/Remove")
  public void remove(@RequestParam Long id) {
    commandGateway.sendAndWait(new RemoveAppCommand(id));
  }
}
