using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	private static void ConfigureEconomyAnalytics(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EconomicActivityRecord>(entity =>
		{
			entity.ToTable("EconomicActivityRecords");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.RealDateTime).HasDatabaseName("IX_EconomicActivityRecords_RealDateTime");
			entity.HasIndex(e => new { e.EconomicZoneId, e.RealDateTime })
				.HasDatabaseName("IX_EconomicActivityRecords_Zone_RealDateTime");
			entity.HasIndex(e => new { e.EconomicZoneId, e.FinancialPeriodId })
				.HasDatabaseName("IX_EconomicActivityRecords_Zone_FinancialPeriod");
			entity.HasIndex(e => new { e.MudCalendarId, e.MudYear, e.MudMonth, e.MudDay })
				.HasDatabaseName("IX_EconomicActivityRecords_MudDate");
			entity.HasIndex(e => e.CurrencyId).HasDatabaseName("FK_EconomicActivityRecords_Currencies_idx");
			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RealDateTime).HasColumnType("datetime");
			entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");
			entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");
			entity.Property(e => e.FinancialPeriodId).HasColumnType("bigint(20)");
			entity.Property(e => e.MudCalendarId).HasColumnType("bigint(20)");
			entity.Property(e => e.MudYear).HasColumnType("int(11)");
			entity.Property(e => e.MudMonth).HasColumnType("int(11)");
			entity.Property(e => e.MudWeek).HasColumnType("int(11)");
			entity.Property(e => e.MudDay).HasColumnType("int(11)");
			entity.Property(e => e.MudDateTime).HasColumnType("varchar(500)")
				.HasCharSet("utf8mb4").UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.ActivityType).HasColumnType("int(11)");
			entity.Property(e => e.VolumeClassification).HasColumnType("int(11)");
			entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");
			entity.Property(e => e.GlobalBaseValue).HasColumnType("decimal(58,29)");
			entity.Property(e => e.SourceId).HasColumnType("bigint(20)");
			entity.Property(e => e.SourceType).HasColumnType("varchar(100)")
				.HasCharSet("utf8mb4").UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.SourceControlBucket).HasColumnType("int(11)");
			entity.Property(e => e.DestinationId).HasColumnType("bigint(20)");
			entity.Property(e => e.DestinationType).HasColumnType("varchar(100)")
				.HasCharSet("utf8mb4").UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.DestinationControlBucket).HasColumnType("int(11)");
			entity.Property(e => e.ReferenceId).HasColumnType("bigint(20)");
			entity.Property(e => e.ReferenceType).HasColumnType("varchar(100)")
				.HasCharSet("utf8mb4").UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.ReferenceText).HasColumnType("varchar(500)")
				.HasCharSet("utf8mb4").UseCollation("utf8mb4_unicode_ci");
			entity.HasOne(e => e.Currency).WithMany().HasForeignKey(e => e.CurrencyId)
				.OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_EconomicActivityRecords_Currencies");
			entity.HasOne(e => e.EconomicZone).WithMany().HasForeignKey(e => e.EconomicZoneId)
				.OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_EconomicActivityRecords_EconomicZones");
			entity.HasOne(e => e.FinancialPeriod).WithMany().HasForeignKey(e => e.FinancialPeriodId)
				.OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_EconomicActivityRecords_FinancialPeriods");
		});

		modelBuilder.Entity<EconomySnapshot>(entity =>
		{
			entity.ToTable("EconomySnapshots");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.RealDateTime).HasDatabaseName("IX_EconomySnapshots_RealDateTime");
			entity.HasIndex(e => new { e.EconomicZoneId, e.RealDateTime })
				.HasDatabaseName("IX_EconomySnapshots_Zone_RealDateTime");
			entity.HasIndex(e => new { e.EconomicZoneId, e.FinancialPeriodId, e.Reason })
				.IsUnique().HasDatabaseName("UX_EconomySnapshots_Zone_Period_Reason");
			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RealDateTime).HasColumnType("datetime");
			entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");
			entity.Property(e => e.FinancialPeriodId).HasColumnType("bigint(20)");
			entity.Property(e => e.MudDateTime).HasColumnType("varchar(500)")
				.HasCharSet("utf8mb4").UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.Reason).HasColumnType("int(11)");
			entity.HasOne(e => e.EconomicZone).WithMany().HasForeignKey(e => e.EconomicZoneId)
				.OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_EconomySnapshots_EconomicZones");
			entity.HasOne(e => e.FinancialPeriod).WithMany().HasForeignKey(e => e.FinancialPeriodId)
				.OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_EconomySnapshots_FinancialPeriods");
		});

		modelBuilder.Entity<EconomySnapshotEntry>(entity =>
		{
			entity.ToTable("EconomySnapshotEntries");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.EconomySnapshotId).HasDatabaseName("FK_EconomySnapshotEntries_Snapshots_idx");
			entity.HasIndex(e => new { e.CurrencyId, e.Metric, e.ControlBucket })
				.HasDatabaseName("IX_EconomySnapshotEntries_Currency_Metric_Control");
			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.EconomySnapshotId).HasColumnType("bigint(20)");
			entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");
			entity.Property(e => e.Metric).HasColumnType("int(11)");
			entity.Property(e => e.ControlBucket).HasColumnType("int(11)");
			entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");
			entity.Property(e => e.GlobalBaseValue).HasColumnType("decimal(58,29)");
			entity.Property(e => e.EntityCount).HasColumnType("int(11)");
			entity.HasOne(e => e.EconomySnapshot).WithMany(e => e.Entries)
				.HasForeignKey(e => e.EconomySnapshotId).OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_EconomySnapshotEntries_Snapshots");
			entity.HasOne(e => e.Currency).WithMany().HasForeignKey(e => e.CurrencyId)
				.OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_EconomySnapshotEntries_Currencies");
		});
	}
}
