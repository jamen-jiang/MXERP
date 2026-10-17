package com.mxerp.system.application.company.command;

import com.mxerp.shared.application.consts.EntityConst;
import com.mxerp.shared.application.port.lock.DistributedLock;
import com.mxerp.system.domain.company.Company;
import com.mxerp.system.domain.company.CompanyDomainService;
import com.mxerp.system.domain.company.CompanyRepository;
import org.axonframework.messaging.commandhandling.annotation.CommandHandler;
import org.jspecify.annotations.NullMarked;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

/** 修改公司命令处理器 */
@NullMarked
@Component
public class ModifyCompanyCommandHandler {

  /** 公司领域服务 */
  private final CompanyDomainService companyDomainService;

  /** 公司仓储 */
  private final CompanyRepository companyRepository;

  /**
   * 构造器注入
   *
   * @param companyDomainService 公司领域服务
   * @param companyRepository 公司仓储
   */
  public ModifyCompanyCommandHandler(
      CompanyDomainService companyDomainService, CompanyRepository companyRepository) {
    this.companyDomainService = companyDomainService;
    this.companyRepository = companyRepository;
  }

  /**
   * 处理修改公司命令
   *
   * @param command 修改公司命令
   */
  @CommandHandler
  @Transactional
  @DistributedLock(prefix = EntityConst.COMPANY, value = "#command.id")
  public void handle(ModifyCompanyCommand command) {
    // 1. 通过领域服务修改公司（内部校验代码唯一 + 存在性）
    Company model =
        companyDomainService.modify(
            command.getId(),
            command.getCode(),
            command.getName(),
            command.getTelephone(),
            command.getFax(),
            command.getEmail(),
            command.getAreaCode(),
            command.getPostCode(),
            command.getAddress(),
            command.getBillingAddress(),
            command.getSort(),
            command.getRemark());

    // 2. 保存
    companyRepository.save(model);
  }
}
