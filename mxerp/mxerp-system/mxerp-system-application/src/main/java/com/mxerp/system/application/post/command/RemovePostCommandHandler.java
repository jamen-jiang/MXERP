package com.mxerp.system.application.post.command;

import com.mxerp.shared.application.consts.EntityConst;
import com.mxerp.shared.application.exception.ApiException;
import com.mxerp.shared.application.port.lock.DistributedLock;
import com.mxerp.system.domain.post.Post;
import com.mxerp.system.domain.post.PostRepository;
import org.axonframework.messaging.commandhandling.annotation.CommandHandler;
import org.jspecify.annotations.NullMarked;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

/** 删除岗位命令处理器 */
@NullMarked
@Component
public class RemovePostCommandHandler {

  /** 岗位仓储 */
  private final PostRepository postRepository;

  /**
   * 构造器注入
   *
   * @param postRepository 岗位仓储
   */
  public RemovePostCommandHandler(PostRepository postRepository) {
    this.postRepository = postRepository;
  }

  /**
   * 处理删除岗位命令
   *
   * @param command 删除岗位命令
   */
  @CommandHandler
  @Transactional
  @DistributedLock(prefix = EntityConst.POST, value = "#command.id")
  public void handle(RemovePostCommand command) {
    Post model = postRepository.get(command.getId());
    if (model == null) {
      throw new ApiException("岗位" + command.getId() + "不存在!");
    }
    model.delete();
    postRepository.save(model);
  }
}
