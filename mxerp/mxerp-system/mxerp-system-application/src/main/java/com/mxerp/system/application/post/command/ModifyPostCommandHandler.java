package com.mxerp.system.application.post.command;

import com.mxerp.shared.application.consts.EntityConst;
import com.mxerp.shared.application.port.lock.DistributedLock;
import com.mxerp.system.domain.post.Post;
import com.mxerp.system.domain.post.PostDomainService;
import com.mxerp.system.domain.post.PostRepository;
import java.util.List;
import org.axonframework.messaging.commandhandling.annotation.CommandHandler;
import org.jspecify.annotations.NullMarked;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

/** 修改岗位命令处理器 */
@NullMarked
@Component
public class ModifyPostCommandHandler {

  /** 岗位领域服务 */
  private final PostDomainService postDomainService;

  /** 岗位仓储 */
  private final PostRepository postRepository;

  /**
   * 构造器注入
   *
   * @param postDomainService 岗位领域服务
   * @param postRepository 岗位仓储
   */
  public ModifyPostCommandHandler(
      PostDomainService postDomainService, PostRepository postRepository) {
    this.postDomainService = postDomainService;
    this.postRepository = postRepository;
  }

  /**
   * 处理修改岗位命令
   *
   * @param command 修改岗位命令
   */
  @CommandHandler
  @Transactional
  @DistributedLock(prefix = EntityConst.POST, value = "#command.id")
  public void handle(ModifyPostCommand command) {
    List<Post> models =
        postDomainService.modify(
            command.getId(),
            command.getName(),
            command.getDepartmentId(),
            command.getParentId(),
            command.isDisabled(),
            command.getRemark());
    for (Post model : models) {
      postRepository.save(model);
    }
  }
}
