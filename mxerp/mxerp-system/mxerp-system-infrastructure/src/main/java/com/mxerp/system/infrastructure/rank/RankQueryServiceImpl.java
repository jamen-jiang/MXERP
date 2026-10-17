package com.mxerp.system.infrastructure.rank;

import com.mxerp.system.application.rank.query.RankQueryService;
import com.mxerp.system.application.rank.query.RankResponse;
import com.mxerp.system.domain.rank.Rank;
import java.util.ArrayList;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.springframework.data.domain.Sort;
import org.springframework.stereotype.Service;

/** 职级查询服务实现 */
@NullMarked
@Service
public class RankQueryServiceImpl implements RankQueryService {

  /** 职级 JPA 接口 */
  private final RankJpa jpa;

  /**
   * 构造器注入
   *
   * @param jpa JPA 接口
   */
  public RankQueryServiceImpl(RankJpa jpa) {
    this.jpa = jpa;
  }

  /**
   * 按Id查询职级
   *
   * @param id 职级Id
   * @return 职级响应，不存在返回 null
   */
  @Override
  @Nullable
  public RankResponse getById(long id) {
    Rank model = jpa.findById(id).orElse(null);
    return model == null ? null : toResponse(model);
  }

  /**
   * 查询所有职级（按排序升序）
   *
   * @return 职级响应列表
   */
  @Override
  public List<RankResponse> getList() {
    Sort sort = Sort.by(Sort.Direction.ASC, "sort", "name");
    List<RankResponse> result = new ArrayList<>();
    for (Rank item : jpa.findAll(sort)) {
      result.add(toResponse(item));
    }
    return result;
  }

  /**
   * 聚合转响应
   *
   * @param model 职级聚合
   * @return 职级响应
   */
  private RankResponse toResponse(Rank model) {
    RankResponse response = new RankResponse();
    response.setId(model.getId());
    response.setLevel(model.getLevel());
    response.setParentId(model.getParentId());
    response.setName(model.getName());
    response.setSort(model.getSort());
    response.setRemark(model.getRemark());
    return response;
  }
}
