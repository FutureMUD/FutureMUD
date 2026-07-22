#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<RouteCell> RouteCells { get; set; } = null!;
	public virtual DbSet<RouteCellLandmark> RouteCellLandmarks { get; set; } = null!;
	public virtual DbSet<RouteExitAnchor> RouteExitAnchors { get; set; } = null!;
	public virtual DbSet<ActiveRouteMotion> ActiveRouteMotions { get; set; } = null!;
	public virtual DbSet<RouteMotionResourceLedger> RouteMotionResourceLedgers { get; set; } = null!;

	private static void ConfigureRouteCells(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<RouteCell>(entity =>
		{
			entity.ToTable("RouteCells", table =>
			{
				table.HasCheckConstraint("CK_RouteCells_Length", "`LengthMetres` > 0");
				table.HasCheckConstraint("CK_RouteCells_DefaultPosition",
					"`DefaultPositionMetres` >= 0 AND `DefaultPositionMetres` <= `LengthMetres`");
				table.HasCheckConstraint("CK_RouteCells_RoomEquivalent", "`MetresPerRoomEquivalent` > 0");
				table.HasCheckConstraint("CK_RouteCells_TopologyVersion", "`TopologyVersion` >= 1");
			});

			entity.HasKey(e => e.CellId).HasName("PRIMARY");

			entity.Property(e => e.CellId).HasColumnType("bigint(20)");
			entity.Property(e => e.LengthMetres).HasColumnType("decimal(18,3)");
			entity.Property(e => e.DefaultPositionMetres).HasColumnType("decimal(18,3)");
			entity.Property(e => e.MetresPerRoomEquivalent).HasColumnType("decimal(18,3)");
			entity.Property(e => e.TopologyVersion)
			      .HasColumnType("bigint(20)")
			      .HasDefaultValue(1L);
			entity.Property(e => e.PositiveDirectionName)
			      .IsRequired()
			      .HasColumnType("varchar(100)")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.NegativeDirectionName)
			      .IsRequired()
			      .HasColumnType("varchar(100)")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");

			entity.HasOne(d => d.Cell)
			      .WithOne(p => p.RouteCell)
			      .HasForeignKey<RouteCell>(d => d.CellId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_RouteCells_Cells");
		});

		modelBuilder.Entity<RouteCellLandmark>(entity =>
		{
			entity.ToTable("RouteCellLandmarks", table =>
			{
				table.HasCheckConstraint("CK_RouteCellLandmarks_Position", "`PositionMetres` >= 0");
			});

			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.RouteCellId, e.PositionMetres })
			      .HasDatabaseName("IX_RouteCellLandmarks_RouteCell_Position");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RouteCellId).HasColumnType("bigint(20)");
			entity.Property(e => e.PositionMetres).HasColumnType("decimal(18,3)");
			entity.Property(e => e.DisplayOrder).HasColumnType("int(11)");
			entity.Property(e => e.Name)
			      .IsRequired()
			      .HasColumnType("varchar(200)")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.Keywords)
			      .IsRequired()
			      .HasColumnType("varchar(500)")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.Description)
			      .IsRequired()
			      .HasColumnType("text")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");

			entity.HasOne(d => d.RouteCell)
			      .WithMany(p => p.Landmarks)
			      .HasForeignKey(d => d.RouteCellId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_RouteCellLandmarks_RouteCells");
		});

		modelBuilder.Entity<RouteExitAnchor>(entity =>
		{
			entity.ToTable("RouteExitAnchors", table =>
			{
				table.HasCheckConstraint("CK_RouteExitAnchors_Band",
					"`MinimumPositionMetres` >= 0 AND `MaximumPositionMetres` >= `MinimumPositionMetres`");
				table.HasCheckConstraint("CK_RouteExitAnchors_Arrival",
					"`ArrivalPositionMetres` >= `MinimumPositionMetres` AND `ArrivalPositionMetres` <= `MaximumPositionMetres`");
			});

			entity.HasKey(e => new { e.ExitId, e.RouteCellId }).HasName("PRIMARY");
			entity.HasIndex(e => new { e.RouteCellId, e.MinimumPositionMetres, e.MaximumPositionMetres })
			      .HasDatabaseName("IX_RouteExitAnchors_RouteCell_Band");

			entity.Property(e => e.ExitId).HasColumnType("bigint(20)");
			entity.Property(e => e.RouteCellId).HasColumnType("bigint(20)");
			entity.Property(e => e.MinimumPositionMetres).HasColumnType("decimal(18,3)");
			entity.Property(e => e.MaximumPositionMetres).HasColumnType("decimal(18,3)");
			entity.Property(e => e.ArrivalPositionMetres).HasColumnType("decimal(18,3)");

			entity.HasOne(d => d.Exit)
			      .WithMany(p => p.RouteExitAnchors)
			      .HasForeignKey(d => d.ExitId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_RouteExitAnchors_Exits");

			entity.HasOne(d => d.RouteCell)
			      .WithMany(p => p.ExitAnchors)
			      .HasForeignKey(d => d.RouteCellId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_RouteExitAnchors_RouteCells");
		});

		modelBuilder.Entity<ActiveRouteMotion>(entity =>
		{
			entity.ToTable("ActiveRouteMotions", table =>
			{
				table.HasCheckConstraint("CK_ActiveRouteMotions_Checkpoint", "`CheckpointPositionMetres` >= 0");
				table.HasCheckConstraint("CK_ActiveRouteMotions_TargetBand",
					"`TargetMinimumPositionMetres` >= 0 AND `TargetMaximumPositionMetres` >= `TargetMinimumPositionMetres`");
				table.HasCheckConstraint("CK_ActiveRouteMotions_Direction", "`Direction` IN (-1, 1)");
				table.HasCheckConstraint("CK_ActiveRouteMotions_Speed", "`SpeedMetresPerSecond` > 0");
				table.HasCheckConstraint("CK_ActiveRouteMotions_RemainingDuration", "`RemainingDurationMilliseconds` >= 0");
				table.HasCheckConstraint("CK_ActiveRouteMotions_Sequence", "`CheckpointSequence` >= 0");
				table.HasCheckConstraint("CK_ActiveRouteMotions_TopologyVersion", "`TopologyVersion` >= 1");
			});

			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.MoverType, e.MoverId })
			      .IsUnique()
			      .HasDatabaseName("UX_ActiveRouteMotions_Mover");
			entity.HasIndex(e => new { e.RouteCellId, e.RoomLayer, e.Status })
			      .HasDatabaseName("IX_ActiveRouteMotions_RouteCell_Layer_Status");
			entity.HasIndex(e => e.OperationId)
			      .IsUnique()
			      .HasDatabaseName("UX_ActiveRouteMotions_Operation");
			entity.HasIndex(e => e.SelectedExitId)
			      .HasDatabaseName("FK_ActiveRouteMotions_Exits_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.MoverType).HasColumnType("int(11)");
			entity.Property(e => e.MoverId).HasColumnType("bigint(20)");
			entity.Property(e => e.RouteCellId).HasColumnType("bigint(20)");
			entity.Property(e => e.RoomLayer).HasColumnType("int(11)");
			entity.Property(e => e.CheckpointPositionMetres).HasColumnType("decimal(18,3)");
			entity.Property(e => e.TargetMinimumPositionMetres).HasColumnType("decimal(18,3)");
			entity.Property(e => e.TargetMaximumPositionMetres).HasColumnType("decimal(18,3)");
			entity.Property(e => e.Direction).HasColumnType("int(11)");
			entity.Property(e => e.SpeedMetresPerSecond).HasColumnType("decimal(18,6)");
			entity.Property(e => e.RemainingDurationMilliseconds).HasColumnType("bigint(20)");
			entity.Property(e => e.TopologyVersion).HasColumnType("bigint(20)");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.CheckpointSequence).HasColumnType("bigint(20)");
			entity.Property(e => e.SelectedExitId).HasColumnType("bigint(20)");
			entity.Property(e => e.CreatedDateTime).HasColumnType("datetime(6)");
			entity.Property(e => e.LastCheckpointDateTime).HasColumnType("datetime(6)");
			entity.Property(e => e.OperationId)
			      .IsRequired()
			      .HasColumnType("varchar(64)")
			      .HasCharSet("ascii")
			      .UseCollation("ascii_general_ci");
			entity.Property(e => e.StateData)
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");

			entity.HasOne(d => d.RouteCell)
			      .WithMany(p => p.ActiveMotions)
			      .HasForeignKey(d => d.RouteCellId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_ActiveRouteMotions_RouteCells");

			entity.HasOne(d => d.SelectedExit)
			      .WithMany()
			      .HasForeignKey(d => d.SelectedExitId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_ActiveRouteMotions_Exits");
		});

		modelBuilder.Entity<RouteMotionResourceLedger>(entity =>
		{
			entity.ToTable("RouteMotionResourceLedgers", table =>
			{
				table.HasCheckConstraint("CK_RouteMotionResourceLedgers_Sequence", "`CheckpointSequence` >= 0");
				table.HasCheckConstraint("CK_RouteMotionResourceLedgers_Amounts",
					"`ReservedAmount` >= 0 AND `ConsumedAmount` >= 0 AND `ConsumedAmount` <= `ReservedAmount`");
			});

			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.IdempotencyKey)
			      .IsUnique()
			      .HasDatabaseName("UX_RouteMotionResourceLedgers_Idempotency");
			entity.HasIndex(e => new { e.ActiveRouteMotionId, e.CheckpointSequence })
			      .HasDatabaseName("IX_RouteMotionResourceLedgers_Motion_Sequence");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ActiveRouteMotionId).HasColumnType("bigint(20)");
			entity.Property(e => e.CheckpointSequence).HasColumnType("bigint(20)");
			entity.Property(e => e.ResourceOwnerType).HasColumnType("int(11)");
			entity.Property(e => e.ResourceOwnerId).HasColumnType("bigint(20)");
			entity.Property(e => e.ResourceType).HasColumnType("int(11)");
			entity.Property(e => e.ResourceReferenceId).HasColumnType("bigint(20)");
			entity.Property(e => e.ReservedAmount).HasColumnType("decimal(18,6)");
			entity.Property(e => e.ConsumedAmount).HasColumnType("decimal(18,6)");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.CreatedDateTime).HasColumnType("datetime(6)");
			entity.Property(e => e.CommittedDateTime).HasColumnType("datetime(6)");
			entity.Property(e => e.IdempotencyKey)
			      .IsRequired()
			      .HasColumnType("varchar(200)")
			      .HasCharSet("ascii")
			      .UseCollation("ascii_general_ci");
			entity.Property(e => e.ResourceKey)
			      .IsRequired()
			      .HasColumnType("varchar(200)")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");

			entity.HasOne(d => d.ActiveRouteMotion)
			      .WithMany(p => p.ResourceLedger)
			      .HasForeignKey(d => d.ActiveRouteMotionId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_RouteMotionResourceLedgers_ActiveRouteMotions");
		});

		modelBuilder.Entity<Models.Character>(entity =>
		{
			entity.Property(e => e.RoutePosition).HasColumnType("decimal(18,3)");
			entity.HasIndex(e => new { e.Location, e.RoomLayer, e.RoutePosition })
			      .HasDatabaseName("IX_Characters_Location_Layer_RoutePosition");
		});

		modelBuilder.Entity<CharacterInstance>(entity =>
		{
			entity.Property(e => e.RoutePosition).HasColumnType("decimal(18,3)");
			entity.HasIndex(e => new { e.LocationId, e.RoomLayer, e.RoutePosition })
			      .HasDatabaseName("IX_CharacterInstances_Location_Layer_RoutePosition");
		});

		modelBuilder.Entity<Models.GameItem>(entity =>
		{
			entity.Property(e => e.RoutePosition).HasColumnType("decimal(18,3)");
			entity.HasIndex(e => e.RoutePosition)
			      .HasDatabaseName("IX_GameItems_RoutePosition");
		});

		modelBuilder.Entity<Vehicle>(entity =>
		{
			entity.Property(e => e.CurrentRoutePosition).HasColumnType("decimal(18,3)");
			entity.HasIndex(e => new { e.CurrentCellId, e.CurrentRoomLayer, e.CurrentRoutePosition })
			      .HasDatabaseName("IX_Vehicles_Cell_Layer_RoutePosition");
		});

		modelBuilder.Entity<Track>(entity =>
		{
			entity.Property(e => e.RoutePosition).HasColumnType("decimal(18,3)");
			entity.Property(e => e.RouteDirection).HasColumnType("int(11)");
			entity.HasIndex(e => new { e.CellId, e.RoomLayer, e.RoutePosition })
			      .HasDatabaseName("IX_Tracks_Cell_Layer_RoutePosition");
		});
	}
}
