package com.mxerp.production.domain.productionrun;

import com.mxerp.shared.domain.repository.Repository;
import java.time.LocalDateTime;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 炉号单仓储接口 */
@NullMarked
public interface ProductionRunRepository extends Repository<ProductionRun> {

  /**
   * 按炉号查询
   *
   * @param number 炉号
   * @return 炉号单（可空），不存在返回 {@code null}
   */
  @Nullable ProductionRun getByNumber(String number);

  /**
   * 查某道工单工序当前被哪一炉占着
   *
   * <p>一道工单工序至多进一个未作废的炉号单，这条规则的正主是数据库的过滤唯一索引，这里的应用层查询只是让报错
   * 提前到用户操作上、并告诉他冲突在哪一炉。排除正在编辑的那一单，否则会把自己的行当成别人的占用。
   *
   * @param workOrderOperationId 工单工序Id
   * @param excludeRunId 要排除的炉号单Id（可空，新增时传 {@code null}）
   * @return 冲突的行；没有被别的炉占着时返回 {@code null}
   */
  @Nullable
  ProductionRunLine getLiveLineByWorkOrderOperationId(
      Long workOrderOperationId, @Nullable Long excludeRunId);

  /**
   * 查一张工单当前进有哪些炉
   *
   * <p>取消工单、改数量之前要先问这一句：单上的工序被哪几炉记着账，改法就不是「直接改」而是先把那一行提出去。
   * 作废、红冲的炉号单里的行已软删，天然不在结果里。
   *
   * @param workOrderId 工单Id
   * @return 炉号单行列表
   */
  List<ProductionRunLine> getLiveLinesByWorkOrderId(Long workOrderId);

  /**
   * 按设备与状态查炉号单
   *
   * <p>「这台设备这个月开了几炉、哪几炉还没入账」就是这一句；状态用列表是因为炉次统计常要一次问几档。
   *
   * @param workCenterId 工作中心Id
   * @param statusCodes 状态编码列表
   * @param from 炉次时间下限（含，可空表示不限）
   * @param to 炉次时间上限（含，可空表示不限）
   * @return 炉号单列表
   */
  List<ProductionRun> getListByWorkCenterAndStatus(
      Long workCenterId,
      List<String> statusCodes,
      @Nullable LocalDateTime from,
      @Nullable LocalDateTime to);

  /**
   * 判断炉号是否存在
   *
   * @param number 炉号
   * @param id 排除的炉号单Id（可空，新增时传 {@code null}）
   * @return true 表示已存在
   */
  boolean isExistByNumberExcludeId(String number, @Nullable Long id);

  /**
   * 判断炉号是否存在
   *
   * @param number 炉号
   * @return true 表示已存在
   */
  default boolean isExistByNumberExcludeId(String number) {
    return isExistByNumberExcludeId(number, null);
  }
}
