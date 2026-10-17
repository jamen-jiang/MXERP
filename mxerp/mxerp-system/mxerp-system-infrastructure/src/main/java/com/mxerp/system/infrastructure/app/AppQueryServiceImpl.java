package com.mxerp.system.infrastructure.app;

import com.mxerp.system.application.app.query.AppQueryService;
import com.mxerp.system.application.app.query.AppResponse;
import com.mxerp.system.application.app.query.MetaResponse;
import com.mxerp.system.domain.app.App;
import com.mxerp.system.domain.app.MetaVO;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.springframework.data.domain.Sort;
import org.springframework.stereotype.Service;

/** 应用查询服务实现 */
@NullMarked
@Service
public class AppQueryServiceImpl implements AppQueryService {

  /** 应用 JPA 接口 */
  private final AppJpa jpa;

  /**
   * 构造器注入
   *
   * @param jpa JPA 接口
   */
  public AppQueryServiceImpl(AppJpa jpa) {
    this.jpa = jpa;
  }

  /**
   * 按Id查询应用
   *
   * @param id 应用Id
   * @return 应用响应，不存在返回 null
   */
  @Override
  @Nullable
  public AppResponse getById(long id) {
    App model = jpa.findById(id).orElse(null);
    return model == null ? null : toResponse(model);
  }

  /**
   * 按Id列表查询应用
   *
   * @param ids 应用Id列表（可空）
   * @return 应用响应列表
   */
  @Override
  public List<AppResponse> getListByIds(Collection<Long> ids) {
    if (ids == null || ids.isEmpty()) {
      return List.of();
    }
    Sort sort = Sort.by(Sort.Direction.ASC, "sort", "name");
    List<AppResponse> result = new ArrayList<>();
    for (App item : jpa.findAllById(ids)) {
      result.add(toResponse(item));
    }
    return result;
  }

  /**
   * 查询所有应用
   *
   * @return 应用响应列表
   */
  @Override
  public List<AppResponse> getList() {
    Sort sort = Sort.by(Sort.Direction.ASC, "sort", "name");
    List<AppResponse> result = new ArrayList<>();
    for (App item : jpa.findAll(sort)) {
      result.add(toResponse(item));
    }
    return result;
  }

  /**
   * 聚合转响应
   *
   * @param model 应用聚合
   * @return 应用响应
   */
  private AppResponse toResponse(App model) {
    AppResponse response = new AppResponse();
    response.setId(model.getId());
    response.setType(model.getType());
    response.setParentId(model.getParentId());
    response.setLevel(model.getLevel());
    response.setName(model.getName());
    response.setPath(model.getPath());
    response.setComponent(model.getComponent());
    response.setRedirect(model.getRedirect());
    response.setLicense(model.getLicense());
    response.setRemark(model.getRemark());
    response.setMeta(toMetaResponse(model.getMeta()));
    return response;
  }

  /**
   * 路由元数据转响应
   *
   * @param meta 路由元数据（可空）
   * @return 元数据响应，或 null
   */
  @Nullable
  private MetaResponse toMetaResponse(@Nullable MetaVO meta) {
    if (meta == null) {
      return null;
    }
    return new MetaResponse(
        meta.getTitle(),
        meta.getI18nKey(),
        meta.getIcon(),
        meta.getIsKeepAlive(),
        meta.getFixedIndexInTab(),
        meta.getIsMultiTab(),
        meta.getIsHide(),
        meta.getSort());
  }
}
