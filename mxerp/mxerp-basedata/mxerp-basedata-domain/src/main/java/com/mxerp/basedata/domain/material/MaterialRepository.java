package com.mxerp.basedata.domain.material;

import com.mxerp.shared.domain.repository.Repository;
import java.util.Collection;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 物料仓储接口 */
@NullMarked
public interface MaterialRepository extends Repository<Material> {
  /**
   * 按Id集合批量查询
   *
   * @param ids 主键集合
   * @return 物料列表
   */
  List<Material> getByIds(Collection<Long> ids);

  /**
   * 按物料代码查询
   *
   * @param code 物料代码
   * @return 物料（可空），不存在返回 null
   */
  @Nullable Material getByCode(String code);

  /**
   * 判断物料代码是否存在
   *
   * @param code 物料代码
   * @param id 排除的主键Id（可空），修改时排除自身
   * @return true 表示已存在
   */
  boolean isExistByCodeExcludeId(String code, @Nullable Long id);

  /**
   * 判断物料代码是否存在
   *
   * @param code 物料代码
   * @return true 表示已存在
   */
  default boolean isExistByCodeExcludeId(String code) {
    return this.isExistByCodeExcludeId(code, null);
  }
}
