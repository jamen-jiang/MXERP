package com.mxerp.inventory.domain.warehouse;

import com.mxerp.shared.domain.exception.DomainException;
import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.stream.Collectors;
import org.apache.commons.lang3.StringUtils;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 仓库领域服务，负责跨聚合编排与库位行同步 */
@NullMarked
public class WarehouseDomainService {

  /** 仓库工厂 */
  private final WarehouseFactory warehouseFactory;

  /** 仓库仓储 */
  private final WarehouseRepository warehouseRepository;

  /**
   * 构造器注入
   *
   * @param warehouseFactory 仓库工厂
   * @param warehouseRepository 仓库仓储
   */
  public WarehouseDomainService(
      WarehouseFactory warehouseFactory, WarehouseRepository warehouseRepository) {
    this.warehouseFactory = warehouseFactory;
    this.warehouseRepository = warehouseRepository;
  }

  /**
   * 导入生成：仓库存在则修改，不存在则创建
   *
   * @param code 代码
   * @param name 名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @return 仓库聚合
   */
  public Warehouse importGenerate(
      String code, String name, boolean disabled, @Nullable String remark) {
    // 按代码查仓库
    Warehouse model = warehouseRepository.getByCode(code);

    // 不存在则创建，存在则修改
    if (model == null) {
      model = warehouseFactory.create(code, name, disabled, remark);
    } else {
      modify(model, code, name, disabled, remark);
    }

    return model;
  }

  /**
   * 修改仓库
   *
   * @param id 仓库Id
   * @param code 代码
   * @param name 名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @return 仓库聚合
   * @throws DomainException 仓库不存在时抛出
   */
  public Warehouse modify(
      long id, String code, String name, boolean disabled, @Nullable String remark) {
    Warehouse model = warehouseRepository.get(id);
    if (model == null) {
      throw new DomainException("仓库" + id + "不存在!");
    }
    modify(model, code, name, disabled, remark);
    return model;
  }

  /**
   * 修改仓库
   *
   * @param model 仓库聚合
   * @param code 代码
   * @param name 名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @throws DomainException 仓库代码与其它仓库冲突时抛出
   */
  public void modify(
      Warehouse model, String code, String name, boolean disabled, @Nullable String remark) {
    // 校验仓库代码唯一
    if (warehouseRepository.isExistByCodeExcludeId(code, model.getId())) {
      throw new DomainException("仓库代码" + code + "已存在!");
    }
    model.modify(code, name, disabled, remark);
  }

  /**
   * 同步库位行（按库位代码匹配：删除不在输入中的、更新已存在的、添加新的）
   *
   * <p>用代码而不是 Id 做匹配，是因为库位主要来自 Excel 导入，表里没有本系统 Id。代价是改代码等于
   * 删旧增新，旧库位会被逻辑删除，历史上落在它上面的数量不迁移。
   *
   * @param warehouse 仓库聚合
   * @param locations 目标库位列表（可空，null 视为清空）
   */
  public void syncLocations(Warehouse warehouse, @Nullable List<WarehouseLocationData> locations) {
    List<WarehouseLocationData> target = locations == null ? List.of() : locations;

    // 1. 删除未在输入中出现的库位（按代码匹配）
    List<WarehouseLocation> toRemoves =
        warehouse.getLocations().stream()
            .filter(l -> target.stream().noneMatch(d -> Objects.equals(d.code(), l.getCode())))
            .toList();
    for (WarehouseLocation location : toRemoves) {
      warehouse.deleteLocation(location.getId());
    }

    // 2. 建立「代码 → 已存在库位Id」的映射（只统计未删除的）
    Map<String, Long> existingDict =
        warehouse.getLocations().stream()
            .filter(l -> !l.getIsDeleted())
            .collect(
                Collectors.toMap(
                    WarehouseLocation::getCode, WarehouseLocation::getId, (a, b) -> a));

    // 3. 遍历输入：存在则修改，不存在则添加
    for (WarehouseLocationData data : target) {
      Long id = existingDict.get(StringUtils.trimToNull(data.code()));
      if (id != null) {
        // 存在：修改
        warehouse.modifyLocation(id, data.code(), data.name(), data.disabled(), data.remark());
      } else {
        // 不存在：添加
        warehouse.addLocation(data.code(), data.name(), data.disabled(), data.remark());
      }
    }
  }
}
