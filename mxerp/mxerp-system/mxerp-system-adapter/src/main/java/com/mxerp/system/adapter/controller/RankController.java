package com.mxerp.system.adapter.controller;

import com.mxerp.system.application.rank.command.AddRankCommand;
import com.mxerp.system.application.rank.command.ModifyRankCommand;
import com.mxerp.system.application.rank.command.RemoveRankCommand;
import com.mxerp.system.application.rank.query.GetRankByIdQuery;
import com.mxerp.system.application.rank.query.GetRanksQuery;
import com.mxerp.system.application.rank.query.RankResponse;
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

/** 职级管理接口 */
@Tag(name = "职级管理", description = "职级增删改查")
@RestController
@RequestMapping("/Rank")
public class RankController {

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
  public RankController(CommandGateway commandGateway, QueryGateway queryGateway) {
    this.commandGateway = commandGateway;
    this.queryGateway = queryGateway;
  }

  /**
   * 获取职级列表
   *
   * @return 职级响应列表
   */
  @Operation(summary = "获取职级列表")
  @GetMapping("/Query")
  public List<RankResponse> query() {
    return queryGateway.queryMany(new GetRanksQuery(), RankResponse.class).join();
  }

  /**
   * 获取职级详情
   *
   * @param query 按Id查询条件
   * @return 职级响应
   */
  @Operation(summary = "获取职级详情")
  @GetMapping("/Detail")
  public RankResponse detail(@ParameterObject GetRankByIdQuery query) {
    return queryGateway.query(query, RankResponse.class).join();
  }

  /**
   * 添加职级
   *
   * @param command 添加职级命令
   * @return 新职级Id
   */
  @PreAuthorize("@ss.hasPermi('System_Rank_Add')")
  @Operation(summary = "添加职级")
  @PostMapping("/Add")
  public Long add(@RequestBody AddRankCommand command) {
    return commandGateway.sendAndWait(command, Long.class);
  }

  /**
   * 修改职级
   *
   * @param command 修改职级命令
   */
  @PreAuthorize("@ss.hasPermi('System_Rank_Modify')")
  @Operation(summary = "修改职级")
  @PutMapping("/Modify")
  public void modify(@RequestBody ModifyRankCommand command) {
    commandGateway.sendAndWait(command);
  }

  /**
   * 删除职级（含子孙）
   *
   * @param id 职级Id
   */
  @PreAuthorize("@ss.hasPermi('System_Rank_Remove')")
  @Operation(summary = "删除职级")
  @DeleteMapping("/Remove")
  public void remove(@RequestParam Long id) {
    commandGateway.sendAndWait(new RemoveRankCommand(id));
  }
}
