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

/** 修改应用命令处理器 */
@NullMarked
@Component
public class ModifyAppCommandHandler {

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
  public ModifyAppCommandHandler(AppDomainService appDomainService, AppRepository appRepository) {
    this.appDomainService = appDomainService;
    this.appRepository = appRepository;
  }

  /**
   * 处理修改应用命令
   *
   * @param command 修改命令
   */
  @CommandHandler
  @Transactional
  @DistributedLock(prefix = EntityConst.APP, value = "#command.id")
  public void handle(ModifyAppCommand command) {
    // 1. 通过领域服务修改应用（内部算 parentPath + 递归子应用）
    List<App> list =
        appDomainService.modify(
            command.getId(),
            command.getParentId(),
            command.getName(),
            command.getPath(),
            command.getComponent(),
            command.getRedirect(),
            command.getLicense(),
            command.getRemark(),
            command.getMeta().getTitle(),
            command.getMeta().getI18nKey(),
            command.getMeta().getIcon(),
            command.getMeta().getIsKeepAlive(),
            command.getMeta().getFixedIndexInTab(),
            command.getMeta().getIsMultiTab(),
            command.getMeta().getIsHide(),
            command.getMeta().getSequence());

    // 2. 保存主应用与所有子孙（parentPath 被更新）
    for (App item : list) {
      appRepository.save(item);
    }
  }
}
