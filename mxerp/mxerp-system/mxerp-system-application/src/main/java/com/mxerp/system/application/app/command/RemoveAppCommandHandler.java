package com.mxerp.system.application.app.command;

import com.mxerp.shared.application.consts.EntityConst;
import com.mxerp.shared.application.port.lock.DistributedLock;
import com.mxerp.system.domain.app.App;
import com.mxerp.system.domain.app.AppDomainService;
import com.mxerp.system.domain.app.AppRepository;
import java.util.List;
import org.axonframework.messaging.commandhandling.annotation.CommandHandler;
import org.jspecify.annotations.NullMarked;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

/** 删除应用命令处理器 */
@NullMarked
@Component
public class RemoveAppCommandHandler {

  /** 应用领域服务 */
  private final AppDomainService appDomainService;

  /** 应用仓储 */
  private final AppRepository appRepository;

  /**
   * 构造器注入
   *
   * @param appDomainService 应用领域服务
   * @param appRepository 应用仓储
   */
  public RemoveAppCommandHandler(AppDomainService appDomainService, AppRepository appRepository) {
    this.appDomainService = appDomainService;
    this.appRepository = appRepository;
  }

  /**
   * 处理删除应用命令
   *
   * @param command 删除应用命令
   */
  @CommandHandler
  @Transactional
  @DistributedLock(prefix = EntityConst.APP, value = "#command.id")
  public void handle(RemoveAppCommand command) {
    List<App> list = appDomainService.removeWithChildren(command.getId());
    for (App item : list) {
      appRepository.save(item);
    }
  }
}
