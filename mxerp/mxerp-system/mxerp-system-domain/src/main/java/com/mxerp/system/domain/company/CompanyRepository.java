package com.mxerp.system.domain.company;

import com.mxerp.shared.domain.repository.Repository;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 公司仓储接口 */
@NullMarked
public interface CompanyRepository extends Repository<Company> {

  /**
   * 根据代码查询公司
   *
   * @param code 公司代码
   * @return 公司实体（可空），不存在返回 null
   */
  @Nullable Company getByCode(String code);

  /**
   * 根据Id列表批量查询公司
   *
   * @param ids 公司Id列表
   * @return 公司实体列表
   */
  List<Company> getListByIds(List<Long> ids);

  /**
   * 判断代码是否已存在
   *
   * @param code 公司代码
   * @param id 排除的Id（用于更新时校验，可为空）
   * @return 是否存在
   */
  boolean isExistByCodeExcludeId(String code, @Nullable Long id);

  /**
   * 判断代码是否已存在（新增时校验，无需排除Id）
   *
   * @param code 公司代码
   * @return 是否存在
   */
  default boolean isExistByCodeExcludeId(String code) {
    return isExistByCodeExcludeId(code, null);
  }
}
