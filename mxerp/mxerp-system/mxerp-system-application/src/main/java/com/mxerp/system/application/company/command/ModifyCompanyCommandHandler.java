package com.mxerp.system.application.company.command;

import com.mxerp.shared.application.consts.EntityConst;
import com.mxerp.shared.application.port.lock.DistributedLock;
import com.mxerp.system.application.company.payload.CompanyContactPayload;
import com.mxerp.system.application.company.payload.CompanyContactPayload;
import com.mxerp.system.domain.company.Company;
import com.mxerp.system.domain.company.CompanyContactData;
import com.mxerp.system.domain.company.CompanyDomainService;
import com.mxerp.system.domain.company.CompanyRepository;
import java.util.ArrayList;
import java.util.List;
import org.axonframework.messaging.commandhandling.annotation.CommandHandler;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
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
            command.getRegion(),
            command.getGroup(),
            command.getCode(),
            command.getName(),
            command.getTelephone(),
            command.getFax(),
            command.getEmail(),
            command.getAreaCode(),
            command.getPostCode(),
            command.getCurrencyId(),
            command.getAddress(),
            command.getBillingAddress(),
            command.getSort(),
            command.getRemark());

    // 2. 同步加成与联系人
    model.modify(command.getMarkUp());
    companyDomainService.syncCompanyContacts(model, toContactDataList(command.getCompanyContacts()));

    // 3. 保存
    companyRepository.save(model);
  }

  /**
   * 入参转领域数据
   *
   * @param payloads 联系人入参列表（可空）
   * @return 联系人数据列表（可空）
   */
  @Nullable
  private List<CompanyContactData> toContactDataList(
      @Nullable List<CompanyContactPayload> payloads) {
    if (payloads == null) {
      return null;
    }
    List<CompanyContactData> list = new ArrayList<>(payloads.size());
    for (CompanyContactPayload item : payloads) {
      list.add(
          new CompanyContactData(
              item.getName(), item.getTelephone(), item.getArea(), item.getAddress()));
    }
    return list;
  }
}
