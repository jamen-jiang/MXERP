package com.mxerp.system.adapter.controller;

import com.mxerp.system.application.company.command.AddCompanyCommand;
import com.mxerp.system.application.company.command.ModifyCompanyCommand;
import com.mxerp.system.application.company.query.CompanyResponse;
import com.mxerp.system.application.company.query.GetCompaniesQuery;
import com.mxerp.system.application.company.query.GetCompanyByIdQuery;
import com.mxerp.system.application.company.query.GetPageCompaniesQuery;
import com.mxerp.shared.application.response.PageResponse;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import java.util.List;
import org.axonframework.messaging.commandhandling.gateway.CommandGateway;
import org.axonframework.messaging.queryhandling.gateway.QueryGateway;
import org.springdoc.core.annotations.ParameterObject;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

/** 公司管理接口 */
@Tag(name = "公司管理", description = "公司增改查")
@RestController
@RequestMapping("/Company")
public class CompanyController {

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
  public CompanyController(CommandGateway commandGateway, QueryGateway queryGateway) {
    this.commandGateway = commandGateway;
    this.queryGateway = queryGateway;
  }

  /**
   * 获取公司列表基础数据（下拉框用）
   *
   * @param query 查询条件
   * @return 公司响应列表
   */
  @Operation(summary = "获取公司列表基础数据")
  @GetMapping("/GetBasicCompanies")
  public List<CompanyResponse> getBasicCompanies(@ParameterObject GetCompaniesQuery query) {
    return queryGateway.queryMany(query, CompanyResponse.class).join();
  }

  /**
   * 分页查询公司
   *
   * @param query 分页查询条件
   * @return 分页响应
   */
  @PreAuthorize("@ss.hasPermi('System_Company_View')")
  @Operation(summary = "分页查询公司")
  @GetMapping("/Query")
  @SuppressWarnings("unchecked")
  public PageResponse<CompanyResponse> query(@ParameterObject GetPageCompaniesQuery query) {
    return queryGateway.query(query, PageResponse.class).join();
  }

  /**
   * 获取公司详情（含联系人）
   *
   * @param query 按Id查询条件
   * @return 公司响应
   */
  @PreAuthorize("@ss.hasPermi('System_Company_View')")
  @Operation(summary = "获取公司详情")
  @GetMapping("/Detail")
  public CompanyResponse detail(@ParameterObject GetCompanyByIdQuery query) {
    return queryGateway.query(query, CompanyResponse.class).join();
  }

  /**
   * 添加公司
   *
   * @param command 添加公司命令
   * @return 新公司Id
   */
  @PreAuthorize("@ss.hasPermi('System_Company_Add')")
  @Operation(summary = "添加公司")
  @PostMapping("/Add")
  public Long add(@RequestBody AddCompanyCommand command) {
    return commandGateway.sendAndWait(command, Long.class);
  }

  /**
   * 修改公司
   *
   * @param command 修改公司命令
   */
  @PreAuthorize("@ss.hasPermi('System_Company_Modify')")
  @Operation(summary = "修改公司")
  @PutMapping("/Modify")
  public void modify(@RequestBody ModifyCompanyCommand command) {
    commandGateway.sendAndWait(command);
  }
}
