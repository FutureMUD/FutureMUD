using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

#nullable enable

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	private static void ConfigureEnvironmentalMagic(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<LandRejuvenationTreatment>(entity =>
		{
			entity.ToTable("LandRejuvenationTreatments");
			entity.HasKey(x => x.Id).HasName("PRIMARY");
			entity.Property(x => x.Id).ValueGeneratedNever();
			entity.Property(x => x.CellId).HasColumnType("bigint(20)");
			entity.Property(x => x.Revision).HasColumnType("bigint(20)").IsConcurrencyToken();
			entity.Property(x => x.Status).HasMaxLength(40).IsRequired();
			entity.Property(x => x.Checkpoint).HasColumnType("longtext").HasCharSet("utf8mb4")
				.UseCollation("utf8mb4_unicode_ci").IsRequired();
			entity.HasIndex(x => new { x.CellId, x.Status }).HasDatabaseName("IX_LandRejuvenationTreatments_CellId_Status");
			// Retain terminal/pending evidence after deletion of its originating spell, caster or cell.
		});

		modelBuilder.Entity<Cell>(entity =>
		{
			entity.Property(x => x.EnvironmentalMagicBindingMode)
				.HasColumnType("int(11)")
				.HasDefaultValue(0);
			entity.Property(x => x.EnvironmentalMagicProfileId).HasColumnType("bigint(20)");
			entity.HasIndex(x => x.EnvironmentalMagicProfileId)
				.HasDatabaseName("IX_Cells_EnvironmentalMagicProfileId");
		});

		modelBuilder.Entity<Terrain>(entity =>
		{
			entity.Property(x => x.EnvironmentalMagicProfileId).HasColumnType("bigint(20)");
			entity.HasIndex(x => x.EnvironmentalMagicProfileId)
				.HasDatabaseName("IX_Terrains_EnvironmentalMagicProfileId");
		});

		modelBuilder.Entity<CellEnvironmentalState>(entity =>
		{
			entity.ToTable("CellEnvironmentalStates", table =>
			{
				table.HasCheckConstraint("CK_CellEnvironmentalStates_Versions", "`SchemaVersion` >= 1 AND `Revision` >= 0");
				table.HasCheckConstraint("CK_CellEnvironmentalStates_ScarDamage", "`ScarDamage` >= 0");
				table.HasCheckConstraint("CK_CellEnvironmentalStates_Pressure", "`RecentPressure` >= 0 AND `PressureHalfLifeSeconds` > 0");
			});
			entity.HasKey(x => x.CellId).HasName("PRIMARY");
			entity.Property(x => x.CellId).HasColumnType("bigint(20)").ValueGeneratedNever();
			entity.Property(x => x.SchemaVersion).HasColumnType("int(11)").HasDefaultValue(1);
			entity.Property(x => x.Revision).HasColumnType("bigint(20)").HasDefaultValue(0L).IsConcurrencyToken();
			entity.Property(x => x.ScarDamage).HasDefaultValue(0.0);
			entity.Property(x => x.RecentPressure).HasDefaultValue(0.0);
			entity.Property(x => x.PressureHalfLifeSeconds).HasDefaultValue(3600.0);
			entity.Property(x => x.PressureProfileId).HasColumnType("bigint(20)");
			entity.Property(x => x.PressureDecayAnchor).HasDefaultValue(0.0);
			entity.Property(x => x.LastDefileUtc).HasColumnType("datetime(6)");
			entity.Property(x => x.PressureReferenceUtc).HasColumnType("datetime(6)");
			entity.HasOne(x => x.Cell)
				.WithOne(x => x.EnvironmentalState)
				.HasForeignKey<CellEnvironmentalState>(x => x.CellId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_CellEnvironmentalStates_Cells");
		});

		modelBuilder.Entity<EnvironmentalMagicOperation>(entity =>
		{
			entity.ToTable("EnvironmentalMagicOperations");
			entity.HasKey(x => x.Id).HasName("PRIMARY");
			entity.Property(x => x.Id).ValueGeneratedNever();
			entity.Property(x => x.CellId).HasColumnType("bigint(20)");
			entity.Property(x => x.ActorId).HasColumnType("bigint(20)");
			entity.Property(x => x.Kind).HasMaxLength(40).IsRequired();
			entity.Property(x => x.Status).HasMaxLength(40).IsRequired();
			entity.Property(x => x.AtUtc).HasColumnType("datetime(6)");
			entity.Property(x => x.Attribution)
				.HasColumnType("text")
				.HasCharSet("utf8mb4")
				.UseCollation("utf8mb4_unicode_ci")
				.IsRequired();
			entity.Property(x => x.Diagnostic)
				.HasColumnType("text")
				.HasCharSet("utf8mb4")
				.UseCollation("utf8mb4_unicode_ci")
				.IsRequired();
			entity.HasIndex(x => new { x.CellId, x.AtUtc })
				.HasDatabaseName("IX_EnvironmentalMagicOperations_CellId_AtUtc");
			// Keep identity and diagnostic evidence after deletion of the cell, actor, or profile.
			// The operation key must continue to reject replay even when those objects no longer exist.
		});
	}
}
