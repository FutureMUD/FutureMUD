using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

#nullable enable
namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<MagicGatheringOperation> MagicGatheringOperations { get; set; } = null!;
	public virtual DbSet<MagicGatheringParticipant> MagicGatheringParticipants { get; set; } = null!;

	private static void ConfigureMagicGathering(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<MagicGatheringOperation>(entity =>
		{
			entity.ToTable("MagicGatheringOperations");
			entity.HasKey(x => x.Id).HasName("PRIMARY");
			entity.Property(x => x.Id).ValueGeneratedNever();
			entity.Property(x => x.OwnerId).HasColumnType("bigint(20)");
			entity.Property(x => x.ActorId).HasColumnType("bigint(20)");
			entity.Property(x => x.BodyId).HasColumnType("bigint(20)");
			entity.Property(x => x.MagicCapabilityId).HasColumnType("bigint(20)");
			entity.Property(x => x.CellId).HasColumnType("bigint(20)");
			entity.Property(x => x.SourceProfileId).HasColumnType("bigint(20)");
			entity.Property(x => x.SourceProfileRevision).HasColumnType("bigint(20)");
			entity.Property(x => x.SourceResourceId).HasColumnType("bigint(20)");
			entity.Property(x => x.DestinationResourceId).HasColumnType("bigint(20)");
			entity.Property(x => x.Kind).HasMaxLength(20).IsRequired();
			entity.Property(x => x.Status).HasMaxLength(40).IsRequired();
			entity.Property(x => x.Diagnostic).HasColumnType("text").HasCharSet("utf8mb4").IsRequired();
			entity.Property(x => x.LandDetailJson).HasColumnType("longtext").HasCharSet("utf8mb4");
			entity.Property(x => x.CreatedUtc).HasColumnType("datetime(6)");
			entity.Property(x => x.UpdatedUtc).HasColumnType("datetime(6)");
			entity.HasIndex(x => new { x.OwnerId, x.Status })
				.HasDatabaseName("IX_MagicGatheringOperations_OwnerId_Status");
			entity.HasIndex(x => new { x.CellId, x.SourceResourceId, x.Status })
				.HasDatabaseName("IX_MagicGatheringOperations_Cell_Source_Status");
			entity.HasIndex(x => new { x.MagicCapabilityId, x.MethodKey })
				.HasDatabaseName("IX_MagicGatheringOperations_Capability_Method");
		});
		modelBuilder.Entity<MagicGatheringParticipant>(entity =>
		{
			entity.ToTable("MagicGatheringParticipants");
			entity.HasKey(x => new { x.OperationId, x.SourceKey }).HasName("PRIMARY");
			entity.Property(x => x.OperationId).ValueGeneratedNever();
			entity.Property(x => x.CellId).HasColumnType("bigint(20)");
			entity.Property(x => x.SourceKey).HasMaxLength(150).IsRequired();
			entity.HasIndex(x => new { x.CellId, x.SourceKey })
				.HasDatabaseName("IX_MagicGatheringParticipants_Cell_Source");
		});
	}
}
