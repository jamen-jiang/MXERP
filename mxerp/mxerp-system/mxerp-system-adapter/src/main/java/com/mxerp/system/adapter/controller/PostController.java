package com.mxerp.system.adapter.controller;

import com.mxerp.shared.application.response.PageResponse;
import com.mxerp.system.application.post.command.AddPostCommand;
import com.mxerp.system.application.post.command.ModifyPostCommand;
import com.mxerp.system.application.post.command.RemovePostCommand;
import com.mxerp.system.application.post.query.GetPagePostsQuery;
import com.mxerp.system.application.post.query.GetPostByIdQuery;
import com.mxerp.system.application.post.query.GetPostsQuery;
import com.mxerp.system.application.post.query.PostResponse;
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

/** 岗位管理接口 */
@Tag(name = "岗位管理", description = "岗位增删改查")
@RestController
@RequestMapping("/Post")
public class PostController {

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
  public PostController(CommandGateway commandGateway, QueryGateway queryGateway) {
    this.commandGateway = commandGateway;
    this.queryGateway = queryGateway;
  }

  /**
   * 获取岗位列表
   *
   * @return 岗位响应列表
   */
  @Operation(summary = "获取岗位列表")
  @GetMapping("/Query")
  public List<PostResponse> query() {
    return queryGateway.queryMany(new GetPostsQuery(), PostResponse.class).join();
  }

  /**
   * 分页查询岗位
   *
   * @param query 分页查询条件
   * @return 分页响应
   */
  @PreAuthorize("@ss.hasPermi('System_Post_View')")
  @Operation(summary = "分页查询岗位")
  @GetMapping("/Page")
  @SuppressWarnings("unchecked")
  public PageResponse<PostResponse> page(@ParameterObject GetPagePostsQuery query) {
    return queryGateway.query(query, PageResponse.class).join();
  }

  /**
   * 获取岗位详情
   *
   * @param query 按Id查询条件
   * @return 岗位响应
   */
  @PreAuthorize("@ss.hasPermi('System_Post_View')")
  @Operation(summary = "获取岗位详情")
  @GetMapping("/Detail")
  public PostResponse detail(@ParameterObject GetPostByIdQuery query) {
    return queryGateway.query(query, PostResponse.class).join();
  }

  /**
   * 添加岗位
   *
   * @param command 添加岗位命令
   * @return 新岗位Id
   */
  @PreAuthorize("@ss.hasPermi('System_Post_Add')")
  @Operation(summary = "添加岗位")
  @PostMapping("/Add")
  public Long add(@RequestBody AddPostCommand command) {
    return commandGateway.sendAndWait(command, Long.class);
  }

  /**
   * 修改岗位
   *
   * @param command 修改岗位命令
   */
  @PreAuthorize("@ss.hasPermi('System_Post_Modify')")
  @Operation(summary = "修改岗位")
  @PutMapping("/Modify")
  public void modify(@RequestBody ModifyPostCommand command) {
    commandGateway.sendAndWait(command);
  }

  /**
   * 删除岗位
   *
   * @param id 岗位Id
   */
  @PreAuthorize("@ss.hasPermi('System_Post_Remove')")
  @Operation(summary = "删除岗位")
  @DeleteMapping("/Remove")
  public void remove(@RequestParam Long id) {
    commandGateway.sendAndWait(new RemovePostCommand(id));
  }
}
