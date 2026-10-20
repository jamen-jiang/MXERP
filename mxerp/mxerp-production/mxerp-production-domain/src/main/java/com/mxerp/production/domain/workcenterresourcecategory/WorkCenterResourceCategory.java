package com.mxerp.production.domain.workcenterresourcecategory;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 工作中心资源类别
 *
 * <p>由用户自己维护的字典：机器设备、人工、模具，车间里还会有什么类别由现场说了算，不该写死在代码里。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "WorkCenterResourceCategory",
    indexes = {
      @Index(name = "IX_WorkCenterResourceCategory_Code", columnList = "Code"),
      @Index(name = "IX_WorkCenterResourceCategory_Name", columnList = "Name"),
      @Index(name = "IX_WorkCenterResourceCategory_Disabled", columnList = "Disabled")
      // 注意：Code 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class WorkCenterResourceCategory extends UpdateAggregateRoot {

  /** 代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 50)
  private String code;

  /** 名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 100)
  private String name;

  /** 是否停用 */
  @Column(name = "Disabled", nullable = false)
  private Boolean disabled;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param code 代码
   * @param name 名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  WorkCenterResourceCategory(
      String code, String name, boolean disabled, @Nullable String remark) {
    apply(code, name, disabled, remark);
  }

  /**
   * 修改工作中心资源类别
   *
   * @param code 代码
   * @param name 名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   */
  void modify(String code, String name, boolean disabled, @Nullable String remark) {
    apply(code, name, disabled, remark);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * @param code 代码
   * @param name 名称
   * @param disabled 是否停用
   * @param remark 备注（可空）
   * @throws DomainException 代码或名称为空时抛出
   */
  private void apply(String code, String name, boolean disabled, @Nullable String remark) {
    String trimmedCode = StringUtils.trimToNull(code);
    String trimmedName = StringUtils.trimToNull(name);
    if (trimmedCode == null) {
      throw new DomainException("工作中心资源类别代码不能为空");
    }
    if (trimmedName == null) {
      throw new DomainException("工作中心资源类别名称不能为空");
    }

    this.code = trimmedCode;
    this.name = trimmedName;
    this.disabled = disabled;
    this.remark = remark;
  }
}
