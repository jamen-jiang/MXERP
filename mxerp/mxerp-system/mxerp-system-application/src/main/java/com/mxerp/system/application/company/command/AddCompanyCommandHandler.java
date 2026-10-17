package com.mxerp.system.application.company.command;

import com.mxerp.system.application.company.payload.CompanyContactPayload;
import com.mxerp.system.domain.company.CompanyContactData;
import com.mxerp.system.domain.company.Company;
import com.mxerp.system.domain.company.CompanyFactory;
import com.mxerp.system.domain.company.CompanyRepository;
import com.mxerp.system.domain.company.CompanyDomainService;
import java.util.ArrayList;
import java.util.List;
import org.axonframework.messaging.commandhandling.annotation.CommandHandler;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.springframework.stereotype.Component;
import org.springframework.transaction.annotation.Transactional;

/** 添加公司命令处理器 */
@NullMarked
@Component
public class AddCompanyCommandHandler {

  /** 公司工厂 */
  private final CompanyFactory companyFactory;

  /** 公司领域服务 */
  private final CompanyDomainService companyDomainService;

  /** 公司仓储 */
  private final CompanyRepository companyRepository;

  /**
   * 构造器注入
   *
   * @param companyFactory 公司工厂
   * @param companyDomainService 公司领域服务
   * @param companyRepository 公司仓储
   */
  public AddCompanyCommandHandler(
      CompanyFactory companyFactory,
      CompanyDomainService companyDomainService,
      CompanyRepository companyRepository) {
    this.companyFactory = companyFactory;
    this.companyDomainService = companyDomainService;
    this.companyRepository = companyRepository;
  }

  /**
   * 处理添加公司命令
   *
   * @param command 添加公司命令
   * @return 新公司Id
   */
  @CommandHandler
  @Transactional
  public Long handle(AddCompanyCommand command) {
    // 1. 创建公司聚合
    Company model =
        companyFactory.create(
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

    // 2. 同步联系人
    companyDomainService.syncCompanyContacts(model, toContactDataList(command.getCompanyContacts()));

    // 3. 保存
    companyRepository.save(model);
    return model.getId();
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
