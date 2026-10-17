package com.mxerp.system.application.app.command;

import com.mxerp.system.domain.app.App;
import com.mxerp.system.domain.app.AppFactory;
import com.mxerp.system.domain.app.AppRepository;
import org.axonframework.messaging.commandhandling.annotation.CommandHandler;
import org.jspecify.annotations.NullMarked;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

/** 创建应用命令处理器 */
@NullMarked
@Component
public class AddAppCommandHandler {

  /** 应用工厂 */
  private final AppFactory appFactory;

  /** 应用仓储 */
  private final AppRepository appRepository;

  /**
   * 构造器注入
   *
   * @param appFactory 应用工厂
   * @param appRepository 应用仓储
   */
  public AddAppCommandHandler(AppFactory appFactory, AppRepository appRepository) {
    this.appFactory = appFactory;
    this.appRepository = appRepository;
  }

  /**
   * 处理创建应用命令
   *
   * @param command 创建命令
   * @return 新应用Id
   */
  @CommandHandler
  @Transactional
  public Long handle(AddAppCommand command) {
    App model =
        appFactory.create(
            command.getType(),
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
            command.getMeta().getSort());
    appRepository.save(model);
    return model.getId();
  }
}
