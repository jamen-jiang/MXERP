package com.mxerp.system.domain.privilege;

import com.mxerp.shared.domain.aggregate.DeleteAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.EnumUtils;
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

/** 权限 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Privilege",
    indexes = {
      @Index(name = "IX_Privilege_Master", columnList = "[Master]"),
      @Index(name = "IX_Privilege_MasterValue", columnList = "MasterValue"),
      @Index(name = "IX_Privilege_Access", columnList = "[Access]"),
      @Index(name = "IX_Privilege_AccessValue", columnList = "AccessValue")
    })
public class Privilege extends DeleteAggregateRoot {

  /** 权限对象 */
  @Nationalized
  @Column(name = "Master", nullable = false, length = 50)
  private String master;

  /** 权限对象值 */
  @Column(name = "MasterValue", nullable = false)
  private Long masterValue;

  /** 权限通道 */
  @Nationalized
  @Column(name = "Access", nullable = false, length = 50)
  private String access;

  /** 权限通道值 */
  @Column(name = "AccessValue", nullable = false)
  private Long accessValue;

  /** 操作 */
  @Column(name = "Operation", nullable = false)
  private Integer operation = 0;

  /**
   * 构造器（枚举）
   *
   * @param master 权限对象
   * @param masterValue 权限对象值
   * @param access 权限通道
   * @param accessValue 权限通道值
   */
  public Privilege(MasterEnum master, long masterValue, AccessEnum access, long accessValue) {
    this.master = master.getCode();
    this.masterValue = masterValue;
    this.access = access.getCode();
    this.accessValue = accessValue;
    this.operation = 0;
  }

  /**
   * 构造器（字符串）
   *
   * @param master 权限对象
   * @param masterValue 权限对象值
   * @param access 权限通道
   * @param accessValue 权限通道值
   * @throws DomainException 权限对象或权限通道不在枚举中时抛出
   */
  public Privilege(String master, long masterValue, String access, long accessValue) {
    this.master = validateMaster(master);
    this.masterValue = masterValue;
    this.access = validateAccess(access);
    this.accessValue = accessValue;
    this.operation = 0;
  }

  // ==================== 私有：校验 ====================

  /**
   * 校验权限对象
   *
   * @param master 权限对象
   * @return 校验并 trim 后的权限对象
   * @throws DomainException 权限对象为空或不在枚举范围内时抛出
   */
  private static String validateMaster(String master) {
    String trimmed = StringUtils.trimToNull(master);
    if (trimmed == null || EnumUtils.fromCode(MasterEnum.class, trimmed) == null) {
      throw new DomainException("权限对象" + master + "有误!");
    }
    return trimmed;
  }

  /**
   * 校验权限通道
   *
   * @param access 权限通道
   * @return 校验并 trim 后的权限通道
   * @throws DomainException 权限通道为空或不在枚举范围内时抛出
   */
  private static String validateAccess(String access) {
    String trimmed = StringUtils.trimToNull(access);
    if (trimmed == null || EnumUtils.fromCode(AccessEnum.class, trimmed) == null) {
      throw new DomainException("权限通道" + access + "有误!");
    }
    return trimmed;
  }
}
