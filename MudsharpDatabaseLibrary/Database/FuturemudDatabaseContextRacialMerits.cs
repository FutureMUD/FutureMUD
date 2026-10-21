#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<RacesMerits> RacesMerits { get; set; } = null!;

	private static void ConfigureRacialMerits(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<RacesMerits>(entity =>
		{
			entity.HasKey(x => new { x.RaceId, x.MeritId }).HasName("PRIMARY");
			entity.ToTable("Races_Merits");
			entity.HasIndex(x => x.MeritId).HasDatabaseName("FK_Races_Merits_Merits");
			entity.Property(x => x.RaceId).HasColumnType("bigint(20)");
			entity.Property(x => x.MeritId).HasColumnType("bigint(20)");
			entity.HasOne(x => x.Race)
				.WithMany(x => x.RacesMerits)
				.HasForeignKey(x => x.RaceId)
				.HasConstraintName("FK_Races_Merits_Races")
				.OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(x => x.Merit)
				.WithMany(x => x.RacesMerits)
				.HasForeignKey(x => x.MeritId)
				.HasConstraintName("FK_Races_Merits_Merits")
				.OnDelete(DeleteBehavior.Cascade);
		});
	}
}
