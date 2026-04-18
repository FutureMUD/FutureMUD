using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database
{
    public partial class FuturemudDatabaseContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShopDeal>(entity =>
            {
                entity.ToTable("ShopDeals");

                entity.HasIndex(e => e.ShopId)
                      .HasDatabaseName("FK_ShopDeals_Shops_idx");

                entity.HasIndex(e => e.MerchandiseId)
                      .HasDatabaseName("FK_ShopDeals_Merchandises_idx");

                entity.HasIndex(e => e.TagId)
                      .HasDatabaseName("FK_ShopDeals_Tags_idx");

                entity.HasIndex(e => e.EligibilityProgId)
                      .HasDatabaseName("FK_ShopDeals_FutureProgs_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.ShopId).HasColumnType("bigint(20)");
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasColumnType("varchar(200)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.DealType).HasColumnType("int(11)");
                entity.Property(e => e.TargetType).HasColumnType("int(11)");
                entity.Property(e => e.MerchandiseId).HasColumnType("bigint(20)");
                entity.Property(e => e.TagId).HasColumnType("bigint(20)");
                entity.Property(e => e.PriceAdjustmentPercentage).HasColumnType("decimal(58,29)");
                entity.Property(e => e.MinimumQuantity).HasColumnType("int(11)");
                entity.Property(e => e.Applicability).HasColumnType("int(11)");
                entity.Property(e => e.EligibilityProgId).HasColumnType("bigint(20)");
                entity.Property(e => e.ExpiryDateTime)
                      .HasColumnType("varchar(500)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.IsCumulative)
                      .HasColumnType("bit(1)")
                      .HasDefaultValue(true);

                entity.HasOne(d => d.Shop)
                      .WithMany(p => p.ShopDeals)
                      .HasForeignKey(d => d.ShopId)
                      .HasConstraintName("FK_ShopDeals_Shops");

                entity.HasOne(d => d.Merchandise)
                      .WithMany(p => p.ShopDeals)
                      .HasForeignKey(d => d.MerchandiseId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_ShopDeals_Merchandises");

                entity.HasOne(d => d.Tag)
                      .WithMany(p => p.ShopDeals)
                      .HasForeignKey(d => d.TagId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_ShopDeals_Tags");

                entity.HasOne(d => d.EligibilityProg)
                      .WithMany(p => p.ShopDeals)
                      .HasForeignKey(d => d.EligibilityProgId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_ShopDeals_FutureProgs");
            });

            modelBuilder.Entity<MarketPopulation>(entity =>
            {
                entity.Property(e => e.IncomeFactor)
                      .HasDefaultValue(1.0m);

                entity.Property(e => e.Savings)
                      .HasDefaultValue(0.0m);

                entity.Property(e => e.SavingsCap)
                      .HasDefaultValue(0.0m);

                entity.Property(e => e.StressFlickerThreshold)
                      .HasDefaultValue(0.01m);
            });
        }
    }
}
