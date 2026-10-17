package com.mxerp.system.adapter.controller;

import com.mxerp.shared.application.response.PageResponse;
import com.mxerp.system.application.department.command.AddDepartmentCommand;
import com.mxerp.system.application.department.command.ModifyDepartmentCommand;
import com.mxerp.system.application.department.command.RemoveDepartmentCommand;
import com.mxerp.system.application.department.query.DepartmentResponse;
import com.mxerp.system.application.department.query.GetDepartmentByIdQuery;
import com.mxerp.system.application.department.query.GetDepartmentsQuery;
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

/** 部门管理接口 */
@Tag(name = "部门管理", description = "部门增删改查")
@RestController
@RequestMapping("/Department")
public class DepartmentController {

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
  public DepartmentController(CommandGateway commandGateway, QueryGateway queryGateway) {
    this.commandGateway = commandGateway;
    this.queryGateway = queryGateway;
  }

  /**
   * 获取部门列表
   *
   * @param query 查询条件
   * @return 部门响应列表
   */
  @PreAuthorize("@ss.hasPermi('System_Department_View')")
  @Operation(summary = "获取部门列表")
  @GetMapping("/Query")
  public List<DepartmentResponse> query(@ParameterObject GetDepartmentsQuery query) {
    return queryGateway.queryMany(query, DepartmentResponse.class).join();
  }

  /**
   * 获取部门详情
   *
   * @param query 按Id查询条件
   * @return 部门响应
   */
  @PreAuthorize("@ss.hasPermi('System_Department_View')")
  @Operation(summary = "获取部门详情")
  @GetMapping("/Detail")
  public DepartmentResponse detail(@ParameterObject GetDepartmentByIdQuery query) {
    return queryGateway.query(query, DepartmentResponse.class).join();
  }

  /**
   * 添加部门
   *
   * @param command 添加部门命令
   * @return 新部门Id
   */
  @PreAuthorize("@ss.hasPermi('System_Department_Add')")
  @Operation(summary = "添加部门")
  @PostMapping("/Add")
  public Long add(@RequestBody AddDepartmentCommand command) {
    return commandGateway.sendAndWait(command, Long.class);
  }

  /**
   * 修改部门
   *
   * @param command 修改部门命令
   */
  @PreAuthorize("@ss.hasPermi('System_Department_Modify')")
  @Operation(summary = "修改部门")
  @PutMapping("/Modify")
  public void modify(@RequestBody ModifyDepartmentCommand command) {
    commandGateway.sendAndWait(command);
  }

  /**
   * 删除部门（含子孙）
   *
   * @param id 部门Id
   */
  @PreAuthorize("@ss.hasPermi('System_Department_Remove')")
  @Operation(summary = "删除部门")
  @DeleteMapping("/Remove")
  public void remove(@RequestParam Long id) {
    commandGateway.sendAndWait(new RemoveDepartmentCommand(id));
  }
}
