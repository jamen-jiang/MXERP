package com.mxerp.system.application.rank.command;

import com.mxerp.system.domain.rank.Rank;
import com.mxerp.system.domain.rank.RankFactory;
import com.mxerp.system.domain.rank.RankRepository;
import org.axonframework.messaging.commandhandling.annotation.CommandHandler;
import org.jspecify.annotations.NullMarked;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

/** 添加职级命令处理器 */
@NullMarked
@Component
public class AddRankCommandHandler {

  /** 职级工厂 */
  private final RankFactory rankFactory;

  /** 职级仓储 */
  private final RankRepository rankRepository;

  /**
   * 构造器注入
   *
   * @param rankFactory 职级工厂
   * @param rankRepository 职级仓储
   */
  public AddRankCommandHandler(RankFactory rankFactory, RankRepository rankRepository) {
    this.rankFactory = rankFactory;
    this.rankRepository = rankRepository;
  }

  /**
   * 处理添加职级命令
   *
   * @param command 添加职级命令
   * @return 新职级Id
   */
  @CommandHandler
  @Transactional
  public Long handle(AddRankCommand command) {
    Rank model =
        rankFactory.create(
            command.getParentId(), command.getName(), command.getSort(), command.getRemark());
    rankRepository.save(model);
    return model.getId();
  }
}
