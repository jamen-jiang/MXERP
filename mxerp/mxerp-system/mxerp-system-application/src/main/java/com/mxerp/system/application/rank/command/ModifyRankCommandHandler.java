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

/** 修改职级命令处理器 */
@NullMarked
@Component
public class ModifyRankCommandHandler {

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
  public ModifyRankCommandHandler(
      RankDomainService rankDomainService, RankRepository rankRepository) {
    this.rankDomainService = rankDomainService;
    this.rankRepository = rankRepository;
  }

  /**
   * 处理修改职级命令
   *
   * @param command 修改职级命令
   */
  @CommandHandler
  @Transactional
  @DistributedLock(prefix = EntityConst.RANK, value = "#command.id")
  public void handle(ModifyRankCommand command) {
    // 1. 通过领域服务修改职级（内部算 level + 递归子职级）
    List<Rank> list =
        rankDomainService.modify(
            command.getId(),
            command.getParentId(),
            command.getName(),
            command.getSort(),
            command.getRemark());

    // 2. 保存主职级与所有子孙（level 被更新）
    for (Rank item : list) {
      rankRepository.save(item);
    }
  }
}
