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

  /** 建一个组内单位：换算率 = factor（1 基准 = factor 本单位），步长 = rounding */
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
    Unit centimetre = unit(1L, "CM", "100", "0.1", false);

    BigDecimal result = metre.convertTo(centimetre, new BigDecimal("2.5"));
    assertEquals(0, new BigDecimal("250.0").compareTo(result));
  }

  @Test
  @DisplayName("组内换算：厘米 → 米折回基准再折出")
  void centimetreToMetre() {
    Unit metre = unit(1L, "M2", "1", "0.001", true);
    Unit centimetre = unit(1L, "CM2", "100", "0.1", false);

    BigDecimal result = centimetre.convertTo(metre, new BigDecimal("250"));
    assertEquals(0, new BigDecimal("2.500").compareTo(result));
  }

  @Test
  @DisplayName("除不尽的换算率不会把整件算多：1件=1/12箱，12箱折回144件")
  void truncatedFactorDoesNotInflate() {
    Unit piece = unit(2L, "PC", "1", "1", true);
    // 1 件 = 1/12 箱 → 箱的 factor 存成 12 位小数，末位有误差
    Unit box = unit(2L, "BOX", "0.083333333333", "1", false);

    BigDecimal result = box.convertTo(piece, new BigDecimal("12"));
    assertEquals(0, new BigDecimal("144").compareTo(result));
  }

  @Test
  @DisplayName("落数方向固定进位：目标步长为 1 时 144.2 件进成 145 件")
  void roundsUpToTargetRounding() {
    Unit piece = unit(3L, "PC3", "1", "1", true);

    assertEquals(0, new BigDecimal("145").compareTo(piece.roundQuantity(new BigDecimal("144.2"))));
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
  void negativeQuantityRejected() {
    Unit metre = unit(9L, "M9", "1", "0.001", true);
    Unit centimetre = unit(9L, "CM9", "100", "0.1", false);

    assertThrows(
        DomainException.class, () -> metre.convertTo(centimetre, new BigDecimal("-1")));
  }
}
