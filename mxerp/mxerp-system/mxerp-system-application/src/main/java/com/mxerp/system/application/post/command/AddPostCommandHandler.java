package com.mxerp.system.application.post.command;

import com.mxerp.system.domain.post.Post;
import com.mxerp.system.domain.post.PostFactory;
import com.mxerp.system.domain.post.PostRepository;
import org.axonframework.messaging.commandhandling.annotation.CommandHandler;
import org.jspecify.annotations.NullMarked;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

/** 添加岗位命令处理器 */
@NullMarked
@Component
public class AddPostCommandHandler {

  /** 岗位工厂 */
  private final PostFactory postFactory;

  /** 岗位仓储 */
  private final PostRepository postRepository;

  /**
   * 构造器注入
   *
   * @param postFactory 岗位工厂
   * @param postRepository 岗位仓储
   */
  public AddPostCommandHandler(PostFactory postFactory, PostRepository postRepository) {
    this.postFactory = postFactory;
    this.postRepository = postRepository;
  }

  /**
   * 处理添加岗位命令
   *
   * @param command 添加岗位命令
   * @return 新岗位Id
   */
  @CommandHandler
  @Transactional
  public Long handle(AddPostCommand command) {
    Post model =
        postFactory.create(
            command.getName(),
            command.getDepartmentId(),
            command.getRankId(),
            command.getRemark(),
            command.getRoleIds());
    postRepository.save(model);
    return model.getId();
  }
}
