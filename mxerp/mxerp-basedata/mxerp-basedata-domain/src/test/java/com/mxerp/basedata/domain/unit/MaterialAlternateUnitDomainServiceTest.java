package com.mxerp.basedata.domain.unit;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.mockito.ArgumentMatchers.anyLong;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;

import com.mxerp.basedata.domain.material.Material;
import com.mxerp.basedata.domain.material.MaterialRepository;
import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.util.Collections;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

/** 物料替代单位（跨组折两次）的算法测试 */
@DisplayName("物料替代单位 MaterialAlternateUnit")
class MaterialAlternateUnitDomainServiceTest {

  /** 物料Id */
  private static final long MATERIAL_ID = 10L;

  /** 支：物料基本单位，数量组（1）基准 */
  private static final long PIECE_ID = 1L;

  /** 公斤：重量组（2）基准 */
  private static final long KILOGRAM_ID = 2L;

  /** 米：长度组（3）基准 */
  private static final long METRE_ID = 4L;

  /** 厘米：长度组（3），1 米 = 100 厘米 */
  private static final long CENTIMETRE_ID = 5L;

  /** 「1 公斤 = 0.5 支」这条换算关系上的系数 */
  private static final BigDecimal KG_TO_PIECE = new BigDecimal("0.5");

  private static Unit unit(
      long groupId, String code, String factor, String rounding, boolean base) {
    return new Unit(
        groupId,
        code,
        code,
        new BigDecimal(factor),
        base,
        new BigDecimal(rounding),
        false,
        null,
        null);
  }

  private final Unit piece = unit(1L, "PC", "1", "1", true);
  private final Unit kilogram = unit(2L, "KG", "1", "0.01", true);
  private final Unit metre = unit(3L, "M", "1", "0.001", true);
  private final Unit centimetre = unit(3L, "CM", "100", "0.1", false);

  private MaterialAlternateUnitDomainService service;

  @BeforeEach
  void setUp() {
    UnitRepository unitRepository = mock(UnitRepository.class);
    when(unitRepository.get(PIECE_ID)).thenReturn(piece);
    when(unitRepository.get(KILOGRAM_ID)).thenReturn(kilogram);
    when(unitRepository.get(METRE_ID)).thenReturn(metre);
    when(unitRepository.get(CENTIMETRE_ID)).thenReturn(centimetre);

    Material material = mock(Material.class);
    when(material.getUnitId()).thenReturn(PIECE_ID);
    MaterialRepository materialRepository = mock(MaterialRepository.class);
    when(materialRepository.get(MATERIAL_ID)).thenReturn(material);

    MaterialAlternateUnitRepository conversionRepository =
        mock(MaterialAlternateUnitRepository.class);
    // 只有公斤配了换算关系：1 公斤 = 0.5 支
    when(conversionRepository.getByMaterialIdAndUnitId(eq(MATERIAL_ID), anyLong()))
        .thenAnswer(
            invocation -> {
              long unitId = invocation.getArgument(1);
              return unitId == KILOGRAM_ID
                  ? new MaterialAlternateUnit(MATERIAL_ID, unitId, KG_TO_PIECE)
                  : null;
            });
    when(conversionRepository.isExistByMaterialIdAndUnitIdExcludeId(
            eq(MATERIAL_ID), eq(KILOGRAM_ID), eq(null)))
        .thenReturn(false);
    when(conversionRepository.getListByMaterialId(MATERIAL_ID)).thenReturn(Collections.emptyList());

    service =
        new MaterialAlternateUnitDomainService(
            new MaterialAlternateUnitFactory(conversionRepository),
            conversionRepository,
            materialRepository,
            unitRepository);
  }

  @Test
  @DisplayName("跨组：10 公斤按「1公斤=0.5支」折成 5 支")
  void crossGroupToMaterialBase() {
    BigDecimal result =
        service.convert(MATERIAL_ID, KILOGRAM_ID, PIECE_ID, new BigDecimal("10"));
    assertEquals(0, new BigDecimal("5").compareTo(result));
  }

  @Test
  @DisplayName("跨组反向：5 支折回 10 公斤，落数按公斤的步长")
  void crossGroupFromMaterialBase() {
    BigDecimal result =
        service.convert(MATERIAL_ID, PIECE_ID, KILOGRAM_ID, new BigDecimal("5"));
    assertEquals(0, new BigDecimal("10.00").compareTo(result));
  }

  @Test
  @DisplayName("同组仍走单位换算率：2.5 米 = 250 厘米")
  void sameGroupUsesUnitFactor() {
    BigDecimal result =
        service.convert(MATERIAL_ID, METRE_ID, CENTIMETRE_ID, new BigDecimal("2.5"));
    assertEquals(0, new BigDecimal("250.0").compareTo(result));
  }

  @Test
  @DisplayName("目标单位没配换算关系时报错，不去猜")
  void missingConversionRejected() {
    assertThrows(
        DomainException.class,
        () ->
            service.convert(MATERIAL_ID, KILOGRAM_ID, METRE_ID, BigDecimal.ONE));
  }

  @Test
  @DisplayName("建换算关系：与物料基本单位同组的单位不收")
  void rejectsSameGroupUnit() {
    assertThrows(
        DomainException.class, () -> service.create(MATERIAL_ID, PIECE_ID, BigDecimal.ONE));
  }

  @Test
  @DisplayName("建换算关系：跨组单位可以建，系数按物料基本单位计")
  void createsCrossGroupRow() {
    MaterialAlternateUnit created = service.create(MATERIAL_ID, KILOGRAM_ID, KG_TO_PIECE);
    assertEquals(0, KG_TO_PIECE.compareTo(created.getFactor()));
    assertEquals(Long.valueOf(KILOGRAM_ID), created.getUnitId());
    assertEquals(Long.valueOf(MATERIAL_ID), created.getMaterialId());
  }

  @Test
  @DisplayName("物料不存在时换算报错")
  void rejectsUnknownMaterial() {
    assertThrows(
        DomainException.class,
        () -> service.convert(99L, KILOGRAM_ID, PIECE_ID, BigDecimal.ONE));
  }
}
