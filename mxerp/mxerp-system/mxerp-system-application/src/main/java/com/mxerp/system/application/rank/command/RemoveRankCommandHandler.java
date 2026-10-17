package com.mxerp.system.application.rank.command;

import com.mxerp.shared.application.consts.EntityConst;
import com.mxerp.shared.application.port.lock.DistributedLock;
import com.mxerp.system.domain.rank.Rank;
import com.mxerp.system.domain.rank.RankDomainService;
import com.mxerp.system.domain.rank.RankRepository;
import java.util.List;
import org.axonframework.messaging.commandhandling.annotation.CommandHandler;
import org.jspecify.annotations.NullMarked;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

/** 删除职级命令处理器 */
@NullMarked
@Component
public class RemoveRankCommandHandler {

  /** 职级领域服务 */
  private final RankDomainService rankDomainService;

  /** 职级仓储 */
  private final RankRepository rankRepository;

  /**
   * 构造器注入
   *
   * @param rankDomainService 职级领域服务
   * @param rankRepository 职级仓储
   */
  public RemoveRankCommandHandler(
      RankDomainService rankDomainService, RankRepository rankRepository) {
    this.rankDomainService = rankDomainService;
    this.rankRepository = rankRepository;
  }

  /**
   * 处理删除职级命令
   *
   * @param command 删除职级命令
   */
  @CommandHandler
  @Transactional
  @DistributedLock(prefix = EntityConst.RANK, value = "#command.id")
  public void handle(RemoveRankCommand command) {
    List<Rank> list = rankDomainService.removeWithChildren(command.getId());
    for (Rank item : list) {
      rankRepository.save(item);
    }
  }
}
