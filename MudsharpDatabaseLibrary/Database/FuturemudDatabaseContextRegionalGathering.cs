#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<RegionalGatheringProfile> RegionalGatheringProfiles { get; set; } = null!;
	public virtual DbSet<RegionalGatheringPool> RegionalGatheringPools { get; set; } = null!;
	private static void ConfigureRegionalGathering(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<RegionalGatheringProfile>(entity =>
		{
			entity.ToTable("RegionalGatheringProfiles");
			entity.HasKey(x => x.Id).HasName("PRIMARY");
			entity.Property(x => x.Id).HasColumnType("bigint(20)");
			entity.Property(x => x.Name).HasMaxLength(200).IsRequired().HasCharSet("utf8mb4");
			entity.Property(x => x.Definition).HasColumnType("longtext").IsRequired().HasCharSet("utf8mb4");
			entity.Property(x => x.Revision).IsConcurrencyToken();
		});
		modelBuilder.Entity<RegionalGatheringPool>(entity =>
		{
			entity.ToTable("RegionalGatheringPools");
			entity.HasKey(x => new { x.OwnerKind, x.OwnerId }).HasName("PRIMARY");
			entity.Property(x => x.OwnerKind).HasMaxLength(10).IsRequired();
			entity.Property(x => x.OwnerId).HasColumnType("bigint(20)").ValueGeneratedNever();
			entity.Property(x => x.ProfileId).HasColumnType("bigint(20)");
			entity.Property(x => x.Revision).IsConcurrencyToken();
			// Keep ecological/payment evidence when a world region is removed. Resolution requires a live native owner.
			entity.HasOne<RegionalGatheringProfile>().WithMany().HasForeignKey(x => x.ProfileId).OnDelete(DeleteBehavior.Restrict);
		});
		modelBuilder.Entity<MagicGatheringParticipant>().HasIndex(x => x.SourceKey)
			.HasDatabaseName("IX_MagicGatheringParticipants_SharedSource");
	}
}
