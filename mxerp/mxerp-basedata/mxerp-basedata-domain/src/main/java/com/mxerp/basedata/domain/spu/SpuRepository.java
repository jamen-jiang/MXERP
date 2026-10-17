package com.mxerp.basedata.domain.spu;

import com.mxerp.shared.domain.repository.Repository;
import java.util.Collection;
import java.util.List;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 产品仓储接口 */
@NullMarked
public interface SpuRepository extends Repository<Spu> {
  /**
   * 按Id集合批量查询
   *
   * @param ids 主键集合
   * @return 产品列表
   */
  List<Spu> getByIds(Collection<Long> ids);

  /**
   * 按产品类型 + 产品代码查询
   *
   * @param productType 产品类型
   * @param code 产品代码
   * @return 产品（可空），不存在返回 null
   */
  @Nullable Spu getByProductTypeAndCode(String productType, String code);

  /**
   * 判断产品代码是否存在
   *
   * @param code 产品代码
   * @param id 排除的主键Id（可空），修改时排除自身
   * @return true 表示已存在
   */
  boolean isExistByCodeExcludeId(String code, @Nullable Long id);

  /**
   * 判断产品代码是否存在
   *
   * @param code 产品代码
   * @return true 表示已存在
   */
  default boolean isExistByCodeExcludeId(String code) {
    return this.isExistByCodeExcludeId(code, null);
  }
}
