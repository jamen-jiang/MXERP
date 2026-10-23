package com.mxerp.basedata.domain.unit;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

/** 单位换算与落数的纯算法测试 */
@DisplayName("单位 Unit")
class UnitTest {

  /** 建一个组内单位：换算率 = factor（1 本单位 = factor 基准单位），步长 = rounding */
  private static Unit unit(
      long groupId, String code, String factor, String rounding, boolean isBase) {
    return new Unit(
        groupId,
        code,
        code,
        new BigDecimal(factor),
        isBase,
        new BigDecimal(rounding),
        false,
        null,
        null);
  }

  @Test
  @DisplayName("组内换算：米 → 厘米按 factor 折出")
  void metreToCentimetre() {
    Unit metre = unit(1L, "M", "1", "0.001", true);
    Unit centimetre = unit(1L, "CM", "0.01", "0.1", false);

    BigDecimal result = metre.convertTo(centimetre, new BigDecimal("2.5"));
    assertEquals(0, new BigDecimal("250.0").compareTo(result));
  }

  @Test
  @DisplayName("组内换算：厘米 → 米折回基准再折出")
  void centimetreToMetre() {
    Unit metre = unit(1L, "M2", "1", "0.001", true);
    Unit centimetre = unit(1L, "CM2", "0.01", "0.1", false);

    BigDecimal result = centimetre.convertTo(metre, new BigDecimal("250"));
    assertEquals(0, new BigDecimal("2.500").compareTo(result));
  }

  @Test
  @DisplayName("除不尽的换算率不会把数量算多：3根=1公斤，3根折成 1.00 公斤")
  void truncatedFactorDoesNotInflate() {
    Unit kilogram = unit(2L, "KG", "1", "0.01", true);
    // 1 根 = 1/3 公斤 → 只能存近似值，3 根折出来是 0.999999999999
    Unit rod = unit(2L, "ROD", "0.333333333333", "0.01", false);

    BigDecimal result = rod.convertTo(kilogram, new BigDecimal("3"));
    assertEquals(0, new BigDecimal("1.00").compareTo(result));
  }

  @Test
  @DisplayName("落数方向固定进位：目标步长为 1 时 144.2 件进成 145 件")
  void roundsUpToTargetRounding() {
    Unit piece = unit(3L, "PC3", "1", "1", true);

    assertEquals(0, new BigDecimal("145").compareTo(piece.roundQty(new BigDecimal("144.2"))));
  }

  @Test
  @DisplayName("参数校验：基准单位的换算率必须是 1")
  void baseFactorMustBeOne() {
    assertThrows(
        DomainException.class, () -> unit(4L, "BAD", "10", "1", true));
  }

  @Test
  @DisplayName("参数校验：换算率和步长都必须大于零")
  void factorAndRoundingMustBePositive() {
    assertThrows(DomainException.class, () -> unit(5L, "BAD2", "0", "1", false));
    assertThrows(DomainException.class, () -> unit(6L, "BAD3", "1", "0", false));
  }

  @Test
  @DisplayName("跨组换算直接报错")
  void crossGroupRejected() {
    Unit metre = unit(7L, "M7", "1", "0.001", true);
    Unit kilogram = unit(8L, "KG8", "1", "0.001", true);

    assertThrows(
        DomainException.class, () -> metre.convertTo(kilogram, BigDecimal.ONE));
  }

  @Test
  @DisplayName("数量为负报错")
  void negativeQtyRejected() {
    Unit metre = unit(9L, "M9", "1", "0.001", true);
    Unit centimetre = unit(9L, "CM9", "0.01", "0.1", false);

    assertThrows(
        DomainException.class, () -> metre.convertTo(centimetre, new BigDecimal("-1")));
  }
}
