package com.mxerp.production.domain.workorder;

import com.mxerp.production.domain.bom.IssueModeEnum;
import com.mxerp.production.domain.bom.IssueTriggerEnum;
import com.mxerp.shared.domain.entity.BaseDeleteEntity;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.EnumUtils;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import java.math.RoundingMode;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 工单用料
 *
 * <p>工单下达时由 BOM 展开生成：需求量按 BOM 行的发料方式算出来（按件乘工单数量、按批取整批量），
 * SPU、发料方式、发料触发与关键件标识一并快照，之后 BOM 改版不影响本单。物料类型不在这里抄：它记在 {@code Variant}
 * 主数据上，要按类别看料就按 SPU 分组。
 *
 * <p>倒冲（{@code issueTrigger} 是报工或入库）用的量不回去读 BOM：下达时把「含损耗的净单位用量」快照在
 * {@code unitQuantity} 上，报工或入库时拿它乘本次产出就是该领多少。手工加的料没有这一列，只能人领。
 *
 * <p>四个数量记录料的完整流转：要多少（required）、领多少（issued）、退多少（returned）、
 * 用多少（consumed）。差异即超领、退料或盘亏。
 *
 * <p>倒冲行的已领与已耗是同一笔数：它的领用量本来就是拿产出乘出来的，料出库即进了物料，没在机台上等着，
 * 所以发料那一下两个数一起加，这一行的「已领未耗」恒为零。这类料要收回来走红冲，不走退料——退一件根本没
 * 在线边停过的东西没有意义。手工领料的行才有「领了没用完」这口气，它的已耗由实耗那一笔填。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "WorkOrderMaterial",
    indexes = {
      @Index(name = "IX_WorkOrderMaterial_WorkOrderId", columnList = "WorkOrderId"),
      @Index(name = "IX_WorkOrderMaterial_MaterialId", columnList = "MaterialId"),
      @Index(name = "IX_WorkOrderMaterial_VariantId", columnList = "VariantId")
    })
public class WorkOrderMaterial extends BaseDeleteEntity {

  /** 工单Id */
  @Column(name = "WorkOrderId")
  private Long workOrderId;

  /** 来源BOM行Id（可空，手工加料时为空） */
  @Column(name = "BomLineId")
  @Nullable
  private Long bomLineId;

  /** 物料SPU */
  @Column(name = "MaterialId", nullable = false)
  private Long materialId;

  /** 物料SKU */
  @Column(name = "VariantId", nullable = false)
  private Long variantId;

  /** 需求数量 */
  @Column(name = "RequiredQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal requiredQuantity;

  /** 净单位用量（快照，含损耗：BOM 行的单位用量 × (1 + 损耗率 / 100)；只有倒冲行有值） */
  @Column(name = "UnitQuantity", precision = 18, scale = 2)
  @Nullable
  private BigDecimal unitQuantity;

  /** 已领数量 */
  @Column(name = "IssuedQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal issuedQuantity;

  /** 已退数量 */
  @Column(name = "ReturnedQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal returnedQuantity;

  /** 已耗数量 */
  @Column(name = "ConsumedQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal consumedQuantity;

  /** 单位Id */
  @Column(name = "UnitId", nullable = false)
  private Long unitId;

  /** 发料方式（快照） */
  @Nationalized
  @Column(name = "IssueMode", nullable = false, length = 50)
  private String issueMode;

  /** 发料触发（快照）：手工领料、报工倒冲、入库倒冲 */
  @Nationalized
  @Column(name = "IssueTrigger", nullable = false, length = 50)
  private String issueTrigger;

  /** 是否关键件（快照，缺料即停线） */
  @Column(name = "IsKeyPart", nullable = false)
  private Boolean isKeyPart;

  /** 发料仓库Id（可空） */
  @Column(name = "WarehouseId")
  @Nullable
  private Long warehouseId;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param bomLineId 来源BOM行Id（可空，手工加料时为空）
   * @param materialId 物料SPU
   * @param variantId 物料SKU
   * @param requiredQuantity 需求数量
   * @param unitQuantity 净单位用量（可空，手工加料时为空）
   * @param unitId 单位Id
   * @param issueMode 发料方式编码（快照）
   * @param issueTrigger 发料触发编码（快照）
   * @param isKeyPart 是否关键件（快照）
   * @param warehouseId 发料仓库Id（可空）
   * @param remark 备注（可空）
   * @throws DomainException SPU、物料、单位、发料方式、发料触发非法或需求数量为负时抛出
   */
  WorkOrderMaterial(
      @Nullable Long bomLineId,
      long materialId,
      long variantId,
      BigDecimal requiredQuantity,
      @Nullable BigDecimal unitQuantity,
      long unitId,
      String issueMode,
      String issueTrigger,
      boolean isKeyPart,
      @Nullable Long warehouseId,
      @Nullable String remark) {
    if (materialId <= 0) {
      throw new DomainException("物料SPU不能为空");
    }
    if (variantId <= 0) {
      throw new DomainException("物料不能为空");
    }
    if (requiredQuantity.signum() < 0) {
      throw new DomainException("需求数量不能为负数");
    }
    if (unitQuantity != null && unitQuantity.signum() < 0) {
      throw new DomainException("净单位用量不能为负数");
    }
    if (unitId <= 0) {
      throw new DomainException("单位不能为空");
    }
    String trimmedIssueMode = StringUtils.trimToNull(issueMode);
    if (trimmedIssueMode == null) {
      throw new DomainException("发料方式不能为空");
    }
    IssueModeEnum mode = EnumUtils.fromCode(IssueModeEnum.class, trimmedIssueMode);
    if (mode == null) {
      throw new DomainException("发料方式" + trimmedIssueMode + "有误!");
    }
    String trimmedIssueTrigger = resolveIssueTrigger(mode, issueTrigger, unitQuantity);
    this.bomLineId = bomLineId;
    this.materialId = materialId;
    this.variantId = variantId;
    this.requiredQuantity = requiredQuantity;
    this.unitQuantity = unitQuantity;
    this.issuedQuantity = BigDecimal.ZERO;
    this.returnedQuantity = BigDecimal.ZERO;
    this.consumedQuantity = BigDecimal.ZERO;
    this.unitId = unitId;
    this.issueMode = trimmedIssueMode;
    this.issueTrigger = trimmedIssueTrigger;
    this.isKeyPart = isKeyPart;
    this.warehouseId = warehouseId;
    this.remark = remark;
  }

  /**
   * 本行是否挂在这个触发上倒冲
   *
   * @param trigger 发料触发（报工倒冲或入库倒冲）
   * @return true 表示这一行该在这个动作上自动生成领料单
   */
  public boolean isBackflushAt(IssueTriggerEnum trigger) {
    return this.issueTrigger.equals(trigger.getCode());
  }

  /**
   * 本行是否倒冲行（发料触发不是手工领料）
   *
   * @return true 表示这一行由系统按产出量自动开单发料
   */
  public boolean isBackflush() {
    return !IssueTriggerEnum.MANUAL.getCode().equals(this.issueTrigger);
  }

  /**
   * 本次倒冲量：净单位用量 × 本次产出数量
   *
   * <p>产出口径由调用方决定（报工用良品加报废，入库用实收），这里只管乘回去。算出来是 0 说明这次没产出，
   * 调用方不该为它开单。
   *
   * @param outputQuantity 本次产出数量
   * @return 本次应领数量，保留 2 位小数
   * @throws DomainException 产出为负、或本行没有净单位用量时抛出
   */
  public BigDecimal backflushQuantity(BigDecimal outputQuantity) {
    if (outputQuantity.signum() < 0) {
      throw new DomainException("产出数量不能为负数");
    }
    if (this.unitQuantity == null) {
      throw new DomainException("物料" + this.variantId + "没有净单位用量，不能倒冲");
    }
    return this.unitQuantity.multiply(outputQuantity).setScale(2, RoundingMode.HALF_UP);
  }

  /**
   * 领料：累加已领数量
   *
   * <p>上限按净领料算（已领减已退再加本次不超过需求）：退回去的料等于把额度让出来，返工补领时不该被自己
   * 先前退的那一笔挡住。超领要放行就得回去把需求量调大，而不是在这里放宽校验。
   *
   * <p>倒冲行在这里顺带把已耗也加上——出库即进了物料，两个数本来就是一笔（见类注释）。
   *
   * @param quantity 本次领用数量
   * @throws DomainException 数量非正、或净领料累计超过需求数量时抛出
   */
  public void issue(BigDecimal quantity) {
    if (quantity.signum() <= 0) {
      throw new DomainException("领料数量必须大于0");
    }
    BigDecimal netIssued = this.issuedQuantity.subtract(this.returnedQuantity).add(quantity);
    if (netIssued.compareTo(this.requiredQuantity) > 0) {
      throw new DomainException(
          "物料"
              + this.variantId
              + "超领：需求"
              + this.requiredQuantity
              + "，净领料将达"
              + netIssued);
    }
    this.issuedQuantity = this.issuedQuantity.add(quantity);
    if (isBackflush()) {
      this.consumedQuantity = this.consumedQuantity.add(quantity);
    }
  }

  /**
   * 退料：累加已退数量
   *
   * <p>退料只能退「已领但还没耗用」的部分，否则等于凭空多出一批料。倒冲行的已领全部记成已耗，所以退不动，
   * 报错文案直接指路走红冲。
   *
   * @param quantity 本次退料数量
   * @throws DomainException 数量非正、本行是倒冲行、或退料超过已领未耗数量时抛出
   */
  public void returnMaterial(BigDecimal quantity) {
    if (quantity.signum() <= 0) {
      throw new DomainException("退料数量必须大于0");
    }
    if (isBackflush()) {
      throw new DomainException(
          "物料" + this.variantId + "是倒冲行，出库即算耗用，要收回去请红冲领料单");
    }
    if (quantity.compareTo(remaining()) > 0) {
      throw new DomainException("退料数量超过已领未耗数量");
    }
    this.returnedQuantity = this.returnedQuantity.add(quantity);
  }

  /**
   * 撤回领料：把已领数量减回去
   *
   * <p>只能撤「已领、还没退也没耗」的那部分：料一旦进了工序就追不回来，硬减会让已耗超过已领，净领料与
   * 需求量的对账也就散了。
   *
   * <p>倒冲行的已领与已耗是同一笔数加上去的，所以这里按已耗为上限、两个数一起减回去，否则红冲一张倒冲单
   * 会因为「已领未耗」恒为零而撤不动。
   *
   * @param quantity 本次撤回数量
   * @throws DomainException 数量非正、或撤回超过可撤回的数量时抛出
   */
  public void unissue(BigDecimal quantity) {
    if (quantity.signum() <= 0) {
      throw new DomainException("撤回领料数量必须大于0");
    }
    if (isBackflush()) {
      if (quantity.compareTo(this.consumedQuantity) > 0) {
        throw new DomainException(
            "物料" + this.variantId + "撤回数量超过已耗数量" + this.consumedQuantity);
      }
      this.issuedQuantity = this.issuedQuantity.subtract(quantity);
      this.consumedQuantity = this.consumedQuantity.subtract(quantity);
      return;
    }
    if (quantity.compareTo(remaining()) > 0) {
      throw new DomainException("物料" + this.variantId + "撤回数量超过已领未耗数量" + remaining());
    }
    this.issuedQuantity = this.issuedQuantity.subtract(quantity);
  }

  /**
   * 撤回退料：把已退数量减回去
   *
   * <p>退料单作废或红冲等于「这批料没退回仓库」，所以只减已退；已领数量当初照批时加过，退料没动它，这里也不动。
   * 减完要照净领料上限重查一遍：退料把额度让出去之后往往又补领过，这一减就把超领放回来了。
   *
   * @param quantity 本次撤回数量
   * @throws DomainException 数量非正、撤回超过已退数量、或撤回后净领料超过需求量时抛出
   */
  public void unreturn(BigDecimal quantity) {
    if (quantity.signum() <= 0) {
      throw new DomainException("撤回退料数量必须大于0");
    }
    BigDecimal left = this.returnedQuantity.subtract(quantity);
    if (left.signum() < 0) {
      throw new DomainException("物料" + this.variantId + "撤回数量超过已退数量" + this.returnedQuantity);
    }
    BigDecimal netIssued = this.issuedQuantity.subtract(left);
    if (netIssued.compareTo(this.requiredQuantity) > 0) {
      throw new DomainException(
          "物料"
              + this.variantId
              + "超领：需求"
              + this.requiredQuantity
              + "，撤回退料后净领料将达"
              + netIssued);
    }
    this.returnedQuantity = left;
  }

  /**
   * 耗用：累加已耗数量
   *
   * <p>只给手工领料的行填实耗用：倒冲行的已耗在发料那一下已经跟着已领一起记过了，这里再记一遍就是同一笔料
   * 算两次。
   *
   * @param quantity 本次耗用数量
   * @throws DomainException 数量为负、本行是倒冲行、或累计耗用超过已领未退数量时抛出
   */
  public void consume(BigDecimal quantity) {
    if (quantity.signum() < 0) {
      throw new DomainException("耗用数量不能为负数");
    }
    if (isBackflush()) {
      throw new DomainException(
          "物料" + this.variantId + "是倒冲行，耗用已随领料记过，不能再手工填");
    }
    BigDecimal next = this.consumedQuantity.add(quantity);
    if (next.compareTo(this.issuedQuantity.subtract(this.returnedQuantity)) > 0) {
      throw new DomainException("累计耗用数量超过已领未退数量");
    }
    this.consumedQuantity = next;
  }

  /**
   * 已领未耗数量
   *
   * @return 已领数量减已退数量再减已耗数量
   */
  public BigDecimal remaining() {
    return this.issuedQuantity.subtract(this.returnedQuantity).subtract(this.consumedQuantity);
  }

  // ==================== 私有：校验 ====================

  /**
   * 校验发料触发
   *
   * <p>倒冲是按「净单位用量 × 产出」开单的，所以只有从 BOM 展开来的按件行才配倒冲：手工加的料没有单位用量，
   * 按批的料整批领多少又只有配缸的人知道，系统反推不出来。
   *
   * @param mode 发料方式
   * @param issueTrigger 发料触发编码
   * @param unitQuantity 净单位用量（可空）
   * @return trim 后的发料触发编码
   * @throws DomainException 触发为空或非法、按批行选倒冲、或倒冲行没有净单位用量时抛出
   */
  private String resolveIssueTrigger(
      IssueModeEnum mode, String issueTrigger, @Nullable BigDecimal unitQuantity) {
    String trimmed = StringUtils.trimToNull(issueTrigger);
    if (trimmed == null) {
      throw new DomainException("发料触发不能为空");
    }
    IssueTriggerEnum trigger = EnumUtils.fromCode(IssueTriggerEnum.class, trimmed);
    if (trigger == null) {
      throw new DomainException("发料触发" + trimmed + "有误!");
    }
    if (trigger == IssueTriggerEnum.MANUAL) {
      return trigger.getCode();
    }
    if (mode == IssueModeEnum.BATCH) {
      throw new DomainException("按批发料的子件不能倒冲，请改为按件或改回手工领料");
    }
    if (unitQuantity == null) {
      throw new DomainException("倒冲的用料行必须有净单位用量");
    }
    return trigger.getCode();
  }
}
