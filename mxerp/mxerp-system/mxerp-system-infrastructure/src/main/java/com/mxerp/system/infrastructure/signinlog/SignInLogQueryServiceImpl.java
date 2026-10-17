package com.mxerp.system.infrastructure.signinlog;

import com.mxerp.shared.application.response.PageResponse;
import com.mxerp.system.application.signinlog.query.GetPageSignInLogsQuery;
import com.mxerp.system.application.signinlog.query.SignInLogQueryService;
import com.mxerp.system.application.signinlog.query.SignInLogResponse;
import com.mxerp.system.domain.signinlog.SignInLog;
import jakarta.persistence.criteria.Predicate;
import java.util.ArrayList;
import java.util.List;
import org.apache.commons.lang3.StringUtils;
import org.jspecify.annotations.NullMarked;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Sort;
import org.springframework.data.jpa.domain.Specification;
import org.springframework.stereotype.Service;

/** 登录日志查询服务实现（JPA Specification） */
@NullMarked
@Service
public class SignInLogQueryServiceImpl implements SignInLogQueryService {

  /** 登录日志 JPA 接口 */
  private final SignInLogJpa jpa;

  /**
   * 构造器注入
   *
   * @param jpa JPA 接口
   */
  public SignInLogQueryServiceImpl(SignInLogJpa jpa) {
    this.jpa = jpa;
  }

  /**
   * 分页查询登录日志（全字段，含错误信息 / 异常信息）
   *
   * @param query 分页查询条件
   * @return 分页响应
   */
  @Override
  public PageResponse<SignInLogResponse> getPage(GetPageSignInLogsQuery query) {
    Specification<SignInLog> spec = buildSpec(query);
    Sort sort = Sort.by(Sort.Direction.DESC, "startedOn", "id");
    PageRequest pageable =
        PageRequest.of(Math.max(query.getPageIndex() - 1, 0), query.getPageSize(), sort);

    Page<SignInLog> page = jpa.findAll(spec, pageable);
    List<SignInLogResponse> list = new ArrayList<>(page.getContent().size());
    for (SignInLog item : page.getContent()) {
      list.add(toResponse(item));
    }
    return new PageResponse<>(
        (int) page.getTotalElements(), query.getPageSize(), query.getPageIndex(), list);
  }

  /**
   * 构造查询条件：类型 / 账号 / 名称精确或模糊匹配，时间区间闭区间，成功标志精确匹配
   *
   * @param query 分页查询条件
   * @return 查询条件
   */
  private Specification<SignInLog> buildSpec(GetPageSignInLogsQuery query) {
    return (root, criteriaQuery, cb) -> {
      List<Predicate> predicates = new ArrayList<>();

      // 类型：精确匹配
      if (StringUtils.isNotBlank(query.getType())) {
        predicates.add(cb.equal(root.get("type"), query.getType().trim()));
      }
      // 账号：模糊匹配
      if (StringUtils.isNotBlank(query.getUserCode())) {
        predicates.add(cb.like(root.get("userCode"), "%" + query.getUserCode().trim() + "%"));
      }
      // 名称：模糊匹配
      if (StringUtils.isNotBlank(query.getUserName())) {
        predicates.add(cb.like(root.get("userName"), "%" + query.getUserName().trim() + "%"));
      }
      // 开始时间下界
      if (query.getStartedOn() != null) {
        predicates.add(cb.greaterThanOrEqualTo(root.get("startedOn"), query.getStartedOn()));
      }
      // 结束时间上界
      if (query.getEndedOn() != null) {
        predicates.add(cb.lessThanOrEqualTo(root.get("endedOn"), query.getEndedOn()));
      }
      // 成功标志
      if (query.getIsSuccess() != null) {
        predicates.add(cb.equal(root.get("isSuccess"), query.getIsSuccess()));
      }
      return cb.and(predicates.toArray(new Predicate[0]));
    };
  }

  /**
   * 聚合转响应
   *
   * @param model 登录日志聚合
   * @return 登录日志响应
   */
  private SignInLogResponse toResponse(SignInLog model) {
    SignInLogResponse response = new SignInLogResponse();
    response.setId(model.getId());
    response.setType(model.getType());
    response.setUserId(model.getUserId());
    response.setUserCode(model.getUserCode());
    response.setUserName(model.getUserName());
    response.setStartedOn(model.getStartedOn());
    response.setEndedOn(model.getEndedOn());
    response.setElapsedMilliseconds(model.getElapsedMilliseconds());
    response.setIp(model.getIp());
    response.setCity(model.getCity());
    response.setUserAgent(model.getUserAgent());
    response.setBrowser(model.getBrowser());
    response.setOs(model.getOs());
    response.setIsSuccess(model.getIsSuccess());
    response.setErrorMessage(model.getErrorMessage());
    response.setException(model.getException());
    return response;
  }
}
