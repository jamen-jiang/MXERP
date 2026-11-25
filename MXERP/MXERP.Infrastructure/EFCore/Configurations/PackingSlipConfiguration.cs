using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MXERP.Domain.PackingSlips;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// 装箱单配置
/// </summary>
public sealed class PackingSlipConfiguration : IEntityTypeConfiguration<PackingSlip>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PackingSlip> builder)
    {
        builder.ToTable("PackingSlip");

        builder.Property(x => x.QrCode).HasMaxLength(50).IsRequired();
        builder.Property(x => x.CartonNo);
        builder.Property(x => x.OrderNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.CustomerOrderNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.StoreId);
        builder.Property(x => x.Orderer).HasMaxLength(50);
        builder.Property(x => x.Brand).HasMaxLength(100);
        builder.Property(x => x.Coating).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Uv).HasColumnName("UV");
        builder.Property(x => x.Hard);
        builder.Property(x => x.SalesLensCodeR).HasMaxLength(50).IsRequired();
        builder.Property(x => x.SalesLensNameR).HasMaxLength(50).IsRequired();
        builder.Property(x => x.IsRxR);
        builder.Property(x => x.DiameterR);
        builder.Property(x => x.QtyR);
        builder.Property(x => x.SphR).HasColumnName("SPHR").HasPrecision(18,2);
        builder.Property(x => x.CylR).HasColumnName("CYLR").HasPrecision(18,2);
        builder.Property(x => x.AxisR);
        builder.Property(x => x.AddR).HasPrecision(18,2);
        builder.Property(x => x.Prism1R).HasMaxLength(50);
        builder.Property(x => x.Base1R).HasMaxLength(50);
        builder.Property(x => x.Prism2R).HasMaxLength(50);
        builder.Property(x => x.Base2R).HasMaxLength(50);
        builder.Property(x => x.EtR).HasColumnName("ETR").HasMaxLength(50);
        builder.Property(x => x.BcR).HasColumnName("BCR").HasMaxLength(50);
        builder.Property(x => x.DecenterR).HasMaxLength(50);
        builder.Property(x => x.AngleR).HasMaxLength(50);
        builder.Property(x => x.DistanceR).HasMaxLength(50);
        builder.Property(x => x.ThicknessR).HasMaxLength(50);
        builder.Property(x => x.ColourR).HasMaxLength(50);
        builder.Property(x => x.IntermediateDistanceR).HasMaxLength(50);
        builder.Property(x => x.ReadingDistanceR).HasMaxLength(50);
        builder.Property(x => x.VertexDistanceR).HasMaxLength(50);
        builder.Property(x => x.WrapAngleR).HasMaxLength(50);
        builder.Property(x => x.TiltAngleR).HasMaxLength(50);
        builder.Property(x => x.PdR).HasColumnName("PDR").HasPrecision(18,4);
        builder.Property(x => x.EyePointR).HasMaxLength(50);
        builder.Property(x => x.FreamDataBoxAR).HasMaxLength(50);
        builder.Property(x => x.FreamDataBoxBR).HasMaxLength(50);
        builder.Property(x => x.FreamDataBoxEDR).HasMaxLength(50);
        builder.Property(x => x.DblR).HasColumnName("DBLR").HasPrecision(18,4);
        builder.Property(x => x.MirrorCoatingR).HasMaxLength(50);
        builder.Property(x => x.SalesLensCodeL).HasMaxLength(50).IsRequired();
        builder.Property(x => x.SalesLensNameL).HasMaxLength(50).IsRequired();
        builder.Property(x => x.IsRxL);
        builder.Property(x => x.DiameterL);
        builder.Property(x => x.QtyL);
        builder.Property(x => x.SphL).HasColumnName("SPHL").HasPrecision(18,2);
        builder.Property(x => x.CylL).HasColumnName("CYLL").HasPrecision(18,2);
        builder.Property(x => x.AxisL);
        builder.Property(x => x.AddL).HasPrecision(18,2);
        builder.Property(x => x.Prism1L).HasMaxLength(50);
        builder.Property(x => x.Base1L).HasMaxLength(50);
        builder.Property(x => x.Prism2L).HasMaxLength(50);
        builder.Property(x => x.Base2L).HasMaxLength(50);
        builder.Property(x => x.EtL).HasColumnName("ETL").HasMaxLength(50);
        builder.Property(x => x.BcL).HasColumnName("BCL").HasMaxLength(50);
        builder.Property(x => x.DecenterL).HasMaxLength(50);
        builder.Property(x => x.AngleL).HasMaxLength(50);
        builder.Property(x => x.DistanceL).HasMaxLength(50);
        builder.Property(x => x.ThicknessL).HasMaxLength(50);
        builder.Property(x => x.ColourL).HasMaxLength(50);
        builder.Property(x => x.IntermediateDistanceL).HasMaxLength(50);
        builder.Property(x => x.ReadingDistanceL).HasMaxLength(50);
        builder.Property(x => x.VertexDistanceL).HasMaxLength(50);
        builder.Property(x => x.WrapAngleL).HasMaxLength(50);
        builder.Property(x => x.TiltAngleL).HasMaxLength(50);
        builder.Property(x => x.PdL).HasColumnName("PDL").HasPrecision(18,4);
        builder.Property(x => x.EyePointL).HasMaxLength(50);
        builder.Property(x => x.FreamDataBoxAL).HasMaxLength(50);
        builder.Property(x => x.FreamDataBoxBL).HasMaxLength(50);
        builder.Property(x => x.FreamDataBoxEDL).HasMaxLength(50);
        builder.Property(x => x.DblL).HasColumnName("DBLL").HasPrecision(18,4);
        builder.Property(x => x.MirrorCoatingL).HasMaxLength(50);
        builder.Property(x => x.Remark).HasMaxLength(500);
        builder.Property(x => x.DeliveredOn);

        builder.HasIndex(x => x.QrCode).HasDatabaseName("IX_PackingSlip_QrCode");
        builder.HasIndex(x => x.OrderNumber).HasDatabaseName("IX_PackingSlip_OrderNumber");
        builder.HasIndex(x => x.CustomerOrderNumber).HasDatabaseName("IX_PackingSlip_CustomerOrderNumber");
        builder.HasIndex(x => x.StoreId).HasDatabaseName("IX_PackingSlip_StoreId");
        builder.HasIndex(x => x.SalesLensCodeR).HasDatabaseName("IX_PackingSlip_SalesLensCodeR");
        builder.HasIndex(x => x.SalesLensCodeL).HasDatabaseName("IX_PackingSlip_SalesLensCodeL");
    }
}
