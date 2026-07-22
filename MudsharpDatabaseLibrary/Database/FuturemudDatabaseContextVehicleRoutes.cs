#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<VehicleRoute> VehicleRoutes { get; set; } = null!;
	public virtual DbSet<VehicleRouteStop> VehicleRouteStops { get; set; } = null!;
	public virtual DbSet<VehicleRoutePlatformBinding> VehicleRoutePlatformBindings { get; set; } = null!;
	public virtual DbSet<VehicleRouteTopologyPin> VehicleRouteTopologyPins { get; set; } = null!;
	public virtual DbSet<VehicleRouteLeg> VehicleRouteLegs { get; set; } = null!;
	public virtual DbSet<VehicleRouteStep> VehicleRouteSteps { get; set; } = null!;
	public virtual DbSet<VehicleService> VehicleServices { get; set; } = null!;
	public virtual DbSet<VehicleServiceSchedule> VehicleServiceSchedules { get; set; } = null!;
	public virtual DbSet<VehicleJourney> VehicleJourneys { get; set; } = null!;
	public virtual DbSet<VehicleJourneyEvent> VehicleJourneyEvents { get; set; } = null!;

	private static void ConfigureVehicleRoutesAndServices(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<VehicleRoute>(entity =>
		{
			entity.ToTable("VehicleRoutes");
			entity.HasKey(e => new { e.Id, e.RevisionNumber }).HasName("PRIMARY");
			entity.HasIndex(e => e.EditableItemId)
			      .HasDatabaseName("FK_VehicleRoutes_EditableItems_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RevisionNumber).HasColumnType("int(11)");
			entity.Property(e => e.EditableItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name)
			      .IsRequired()
			      .HasColumnType("varchar(200)")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.Description)
			      .IsRequired()
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");

			entity.HasOne(d => d.EditableItem)
			      .WithMany()
			      .HasForeignKey(d => d.EditableItemId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_VehicleRoutes_EditableItems");
		});

		modelBuilder.Entity<VehicleRouteStop>(entity =>
		{
			entity.ToTable("VehicleRouteStops", table =>
			{
				table.HasCheckConstraint("CK_VehicleRouteStops_Sequence", "`Sequence` >= 0");
				table.HasCheckConstraint("CK_VehicleRouteStops_RoutePosition",
					"`RoutePositionMetres` IS NULL OR `RoutePositionMetres` >= 0");
				table.HasCheckConstraint("CK_VehicleRouteStops_Dwell", "`DwellDurationMilliseconds` >= 0");
			});

			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasAlternateKey(e => new { e.Id, e.VehicleRouteId, e.VehicleRouteRevision })
			      .HasName("AK_VehicleRouteStops_Id_Route");
			entity.HasIndex(e => new { e.VehicleRouteId, e.VehicleRouteRevision, e.Sequence })
			      .IsUnique()
			      .HasDatabaseName("UX_VehicleRouteStops_Route_Sequence");
			entity.HasIndex(e => e.CellId)
			      .HasDatabaseName("FK_VehicleRouteStops_Cells_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleRouteId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleRouteRevision).HasColumnType("int(11)");
			entity.Property(e => e.Name)
			      .IsRequired()
			      .HasColumnType("varchar(200)")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.Sequence).HasColumnType("int(11)");
			entity.Property(e => e.CellId).HasColumnType("bigint(20)");
			entity.Property(e => e.RoomLayer).HasColumnType("int(11)");
			entity.Property(e => e.RoutePositionMetres).HasColumnType("decimal(18,3)");
			entity.Property(e => e.DwellDurationMilliseconds)
			      .HasColumnType("bigint(20)")
			      .HasDefaultValue(0L);

			entity.HasOne(d => d.VehicleRoute)
			      .WithMany(p => p.Stops)
			      .HasForeignKey(d => new { d.VehicleRouteId, d.VehicleRouteRevision })
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_VehicleRouteStops_VehicleRoutes");

			entity.HasOne(d => d.Cell)
			      .WithMany()
			      .HasForeignKey(d => d.CellId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleRouteStops_Cells");
		});

		modelBuilder.Entity<VehicleRoutePlatformBinding>(entity =>
		{
			entity.ToTable("VehicleRoutePlatformBindings", table =>
			{
				table.HasCheckConstraint("CK_VehicleRoutePlatformBindings_Tolerance",
					"`DockingToleranceMetres` >= 0");
			});

			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new
				{
					e.VehicleRouteStopId,
					e.PlatformCellId,
					e.VehicleAccessPointProtoId
				})
			      .IsUnique()
			      .HasDatabaseName("UX_VehicleRoutePlatformBindings_Stop_Platform_AccessPoint");
			entity.HasIndex(e => e.PlatformCellId)
			      .HasDatabaseName("FK_VehicleRoutePlatformBindings_Cells_idx");
			entity.HasIndex(e => e.VehicleAccessPointProtoId)
			      .HasDatabaseName("FK_VehicleRoutePlatformBindings_AccessPointProtos_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleRouteStopId).HasColumnType("bigint(20)");
			entity.Property(e => e.PlatformCellId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleAccessPointProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.DockingToleranceMetres)
			      .HasColumnType("decimal(18,3)")
			      .HasDefaultValue(2.0m);

			entity.HasOne(d => d.VehicleRouteStop)
			      .WithMany(p => p.PlatformBindings)
			      .HasForeignKey(d => d.VehicleRouteStopId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_VehicleRoutePlatformBindings_VehicleRouteStops");

			entity.HasOne(d => d.PlatformCell)
			      .WithMany()
			      .HasForeignKey(d => d.PlatformCellId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleRoutePlatformBindings_Cells");

			entity.HasOne(d => d.VehicleAccessPointProto)
			      .WithMany()
			      .HasForeignKey(d => d.VehicleAccessPointProtoId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleRoutePlatformBindings_AccessPointProtos");
		});

		modelBuilder.Entity<VehicleRouteTopologyPin>(entity =>
		{
			entity.ToTable("VehicleRouteTopologyPins", table =>
			{
				table.HasCheckConstraint("CK_VehicleRouteTopologyPins_Version", "`TopologyVersion` >= 1");
			});

			entity.HasKey(e => new { e.VehicleRouteId, e.VehicleRouteRevision, e.RouteCellId })
			      .HasName("PRIMARY");
			entity.HasIndex(e => e.RouteCellId)
			      .HasDatabaseName("FK_VehicleRouteTopologyPins_RouteCells_idx");

			entity.Property(e => e.VehicleRouteId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleRouteRevision).HasColumnType("int(11)");
			entity.Property(e => e.RouteCellId).HasColumnType("bigint(20)");
			entity.Property(e => e.TopologyVersion).HasColumnType("bigint(20)");

			entity.HasOne(d => d.VehicleRoute)
			      .WithMany(p => p.TopologyPins)
			      .HasForeignKey(d => new { d.VehicleRouteId, d.VehicleRouteRevision })
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_VehicleRouteTopologyPins_VehicleRoutes");

			entity.HasOne(d => d.RouteCell)
			      .WithMany()
			      .HasForeignKey(d => d.RouteCellId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleRouteTopologyPins_RouteCells");
		});

		modelBuilder.Entity<VehicleRouteLeg>(entity =>
		{
			entity.ToTable("VehicleRouteLegs", table =>
			{
				table.HasCheckConstraint("CK_VehicleRouteLegs_Sequence", "`Sequence` >= 0");
				table.HasCheckConstraint("CK_VehicleRouteLegs_Distance", "`RouteDistanceMetres` >= 0");
				table.HasCheckConstraint("CK_VehicleRouteLegs_RoomEquivalentCost", "`RoomEquivalentCost` >= 0");
				table.HasCheckConstraint("CK_VehicleRouteLegs_DistinctStops", "`OriginStopId` <> `DestinationStopId`");
			});

			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.VehicleRouteId, e.VehicleRouteRevision, e.Sequence })
			      .IsUnique()
			      .HasDatabaseName("UX_VehicleRouteLegs_Route_Sequence");
			entity.HasIndex(e => new { e.OriginStopId, e.VehicleRouteId, e.VehicleRouteRevision })
			      .HasDatabaseName("FK_VehicleRouteLegs_OriginStops_idx");
			entity.HasIndex(e => new { e.DestinationStopId, e.VehicleRouteId, e.VehicleRouteRevision })
			      .HasDatabaseName("FK_VehicleRouteLegs_DestinationStops_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleRouteId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleRouteRevision).HasColumnType("int(11)");
			entity.Property(e => e.Sequence).HasColumnType("int(11)");
			entity.Property(e => e.OriginStopId).HasColumnType("bigint(20)");
			entity.Property(e => e.DestinationStopId).HasColumnType("bigint(20)");
			entity.Property(e => e.RouteDistanceMetres).HasColumnType("decimal(18,3)");
			entity.Property(e => e.RoomEquivalentCost).HasColumnType("decimal(18,6)");

			entity.HasOne(d => d.VehicleRoute)
			      .WithMany(p => p.Legs)
			      .HasForeignKey(d => new { d.VehicleRouteId, d.VehicleRouteRevision })
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_VehicleRouteLegs_VehicleRoutes");

			entity.HasOne(d => d.OriginStop)
			      .WithMany(p => p.OriginLegs)
			      .HasForeignKey(d => new { d.OriginStopId, d.VehicleRouteId, d.VehicleRouteRevision })
			      .HasPrincipalKey(p => new { p.Id, p.VehicleRouteId, p.VehicleRouteRevision })
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_VehicleRouteLegs_OriginStops");

			entity.HasOne(d => d.DestinationStop)
			      .WithMany(p => p.DestinationLegs)
			      .HasForeignKey(d => new { d.DestinationStopId, d.VehicleRouteId, d.VehicleRouteRevision })
			      .HasPrincipalKey(p => new { p.Id, p.VehicleRouteId, p.VehicleRouteRevision })
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_VehicleRouteLegs_DestinationStops");
		});

		modelBuilder.Entity<VehicleRouteStep>(entity =>
		{
			entity.ToTable("VehicleRouteSteps", table =>
			{
				table.HasCheckConstraint("CK_VehicleRouteSteps_Sequence", "`Sequence` >= 0");
				table.HasCheckConstraint("CK_VehicleRouteSteps_Positions",
					"(`OriginRoutePositionMetres` IS NULL OR `OriginRoutePositionMetres` >= 0) AND " +
					"(`DestinationRoutePositionMetres` IS NULL OR `DestinationRoutePositionMetres` >= 0) AND " +
					"((`OriginRoutePositionMetres` IS NULL AND `PinnedTopologyVersion` IS NULL) OR " +
					"(`OriginRoutePositionMetres` IS NOT NULL AND `PinnedTopologyVersion` IS NOT NULL AND `PinnedTopologyVersion` >= 1)) AND " +
					"((`DestinationRoutePositionMetres` IS NULL AND `DestinationTopologyVersion` IS NULL) OR " +
					"(`DestinationRoutePositionMetres` IS NOT NULL AND `DestinationTopologyVersion` IS NOT NULL AND `DestinationTopologyVersion` >= 1))");
				table.HasCheckConstraint("CK_VehicleRouteSteps_RoomEquivalentCost", "`RoomEquivalentCost` >= 0");
				table.HasCheckConstraint("CK_VehicleRouteSteps_TypedPayload",
					"(`StepType` = 0 AND `ExitId` IS NULL AND `Direction` IS NOT NULL AND `Direction` IN (-1, 1) AND " +
					"`PinnedTopologyVersion` IS NOT NULL AND `DestinationTopologyVersion` = `PinnedTopologyVersion` AND " +
					"`DistanceMetres` IS NOT NULL AND `DistanceMetres` >= 0 AND " +
					"`OriginRoutePositionMetres` IS NOT NULL AND `DestinationRoutePositionMetres` IS NOT NULL AND " +
					"`OriginCellId` = `DestinationCellId` AND `OriginRoomLayer` = `DestinationRoomLayer`) OR " +
					"(`StepType` = 1 AND `ExitId` IS NOT NULL AND `Direction` IS NULL AND " +
					"`DistanceMetres` IS NULL)");
			});

			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.VehicleRouteLegId, e.Sequence })
			      .IsUnique()
			      .HasDatabaseName("UX_VehicleRouteSteps_Leg_Sequence");
			entity.HasIndex(e => e.OriginCellId)
			      .HasDatabaseName("FK_VehicleRouteSteps_OriginCells_idx");
			entity.HasIndex(e => e.DestinationCellId)
			      .HasDatabaseName("FK_VehicleRouteSteps_DestinationCells_idx");
			entity.HasIndex(e => e.ExitId)
			      .HasDatabaseName("FK_VehicleRouteSteps_Exits_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleRouteLegId).HasColumnType("bigint(20)");
			entity.Property(e => e.Sequence).HasColumnType("int(11)");
			entity.Property(e => e.StepType).HasColumnType("int(11)");
			entity.Property(e => e.OriginCellId).HasColumnType("bigint(20)");
			entity.Property(e => e.OriginRoomLayer).HasColumnType("int(11)");
			entity.Property(e => e.OriginRoutePositionMetres).HasColumnType("decimal(18,3)");
			entity.Property(e => e.DestinationCellId).HasColumnType("bigint(20)");
			entity.Property(e => e.DestinationRoomLayer).HasColumnType("int(11)");
			entity.Property(e => e.DestinationRoutePositionMetres).HasColumnType("decimal(18,3)");
			entity.Property(e => e.DistanceMetres).HasColumnType("decimal(18,3)");
			entity.Property(e => e.RoomEquivalentCost).HasColumnType("decimal(18,6)");
			entity.Property(e => e.Direction).HasColumnType("int(11)");
			entity.Property(e => e.PinnedTopologyVersion).HasColumnType("bigint(20)");
			entity.Property(e => e.DestinationTopologyVersion).HasColumnType("bigint(20)");
			entity.Property(e => e.ExitId).HasColumnType("bigint(20)");

			entity.HasOne(d => d.VehicleRouteLeg)
			      .WithMany(p => p.Steps)
			      .HasForeignKey(d => d.VehicleRouteLegId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_VehicleRouteSteps_VehicleRouteLegs");

			entity.HasOne(d => d.OriginCell)
			      .WithMany()
			      .HasForeignKey(d => d.OriginCellId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleRouteSteps_OriginCells");

			entity.HasOne(d => d.DestinationCell)
			      .WithMany()
			      .HasForeignKey(d => d.DestinationCellId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleRouteSteps_DestinationCells");

			entity.HasOne(d => d.Exit)
			      .WithMany()
			      .HasForeignKey(d => d.ExitId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleRouteSteps_Exits");
		});

		modelBuilder.Entity<VehicleService>(entity =>
		{
			entity.ToTable("VehicleServices", table =>
			{
				table.HasCheckConstraint("CK_VehicleServices_OperatorMode", "`OperatorMode` IN (0, 1)");
				table.HasCheckConstraint("CK_VehicleServices_RetryInterval", "`RetryIntervalMilliseconds` > 0");
				table.HasCheckConstraint("CK_VehicleServices_MaximumHold", "`MaximumHoldMilliseconds` >= 0");
			});

			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.VehicleRouteId, e.VehicleRouteRevision })
			      .HasDatabaseName("FK_VehicleServices_VehicleRoutes_idx");
			entity.HasIndex(e => new { e.VehicleId, e.Enabled })
			      .HasDatabaseName("IX_VehicleServices_Vehicle_Enabled");
			entity.HasIndex(e => e.Name)
			      .HasDatabaseName("IX_VehicleServices_Name");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.Name)
			      .IsRequired()
			      .HasColumnType("varchar(200)")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.Keywords)
			      .IsRequired()
			      .HasColumnType("varchar(1000)")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.VehicleRouteId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleRouteRevision).HasColumnType("int(11)");
			entity.Property(e => e.VehicleId).HasColumnType("bigint(20)");
			entity.Property(e => e.OperatorMode).HasColumnType("int(11)");
			entity.Property(e => e.RetryIntervalMilliseconds)
			      .HasColumnType("bigint(20)")
			      .HasDefaultValue(30000L);
			entity.Property(e => e.MaximumHoldMilliseconds)
			      .HasColumnType("bigint(20)")
			      .HasDefaultValue(900000L);
			entity.Property(e => e.Enabled)
			      .HasColumnType("bit(1)")
			      .HasDefaultValue(true);

			entity.HasOne(d => d.VehicleRoute)
			      .WithMany(p => p.Services)
			      .HasForeignKey(d => new { d.VehicleRouteId, d.VehicleRouteRevision })
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleServices_VehicleRoutes");

			entity.HasOne(d => d.Vehicle)
			      .WithMany()
			      .HasForeignKey(d => d.VehicleId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleServices_Vehicles");
		});

		modelBuilder.Entity<VehicleServiceSchedule>(entity =>
		{
			entity.ToTable("VehicleServiceSchedules", table =>
			{
				table.HasCheckConstraint("CK_VehicleServiceSchedules_RecurrenceInterval",
					"`RecurrenceIntervalAmount` > 0");
			});

			entity.HasKey(e => e.VehicleServiceId).HasName("PRIMARY");
			entity.Property(e => e.VehicleServiceId)
			      .ValueGeneratedNever()
			      .HasColumnType("bigint(20)");
			entity.Property(e => e.ReferenceDeparture)
			      .IsRequired()
			      .HasColumnType("varchar(500)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.NextDeparture)
			      .IsRequired()
			      .HasColumnType("varchar(500)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.RecurrenceType).HasColumnType("int(11)");
			entity.Property(e => e.RecurrenceIntervalAmount).HasColumnType("int(11)");
			entity.Property(e => e.RecurrenceModifier).HasColumnType("int(11)");
			entity.Property(e => e.RecurrenceSecondaryModifier).HasColumnType("int(11)");
			entity.Property(e => e.RecurrenceFallbackMode).HasColumnType("int(11)");

			entity.HasOne(d => d.VehicleService)
			      .WithOne(p => p.Schedule)
			      .HasForeignKey<VehicleServiceSchedule>(d => d.VehicleServiceId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_VehicleServiceSchedules_VehicleServices");
		});

		modelBuilder.Entity<VehicleJourney>(entity =>
		{
			entity.ToTable("VehicleJourneys", table =>
			{
				table.HasCheckConstraint("CK_VehicleJourneys_State", "`State` BETWEEN 0 AND 8");
				table.HasCheckConstraint("CK_VehicleJourneys_Delay", "`DelayMilliseconds` >= 0");
			});

			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.OperationId)
			      .IsUnique()
			      .HasDatabaseName("UX_VehicleJourneys_Operation");
			entity.HasIndex(e => e.VehicleServiceId)
			      .HasDatabaseName("FK_VehicleJourneys_VehicleServices_idx");
			entity.HasIndex(e => new { e.VehicleServiceId, e.ScheduledDeparture })
			      .IsUnique()
			      .HasDatabaseName("UX_VehicleJourneys_Service_ScheduledDeparture");
			entity.HasIndex(e => new { e.VehicleRouteId, e.VehicleRouteRevision })
			      .HasDatabaseName("FK_VehicleJourneys_VehicleRoutes_idx");
			entity.HasIndex(e => e.VehicleId)
			      .HasDatabaseName("FK_VehicleJourneys_Vehicles_idx");
			entity.HasIndex(e => new { e.CurrentStopId, e.VehicleRouteId, e.VehicleRouteRevision })
			      .HasDatabaseName("FK_VehicleJourneys_CurrentStops_idx");
			entity.HasIndex(e => new { e.NextStopId, e.VehicleRouteId, e.VehicleRouteRevision })
			      .HasDatabaseName("FK_VehicleJourneys_NextStops_idx");
			entity.HasIndex(e => new { e.VehicleServiceId, e.State })
			      .HasDatabaseName("IX_VehicleJourneys_Service_State");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.OperationId)
			      .IsRequired()
			      .HasColumnType("varchar(64)")
			      .HasCharSet("ascii")
			      .UseCollation("ascii_general_ci");
			entity.Property(e => e.VehicleServiceId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleRouteId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleRouteRevision).HasColumnType("int(11)");
			entity.Property(e => e.VehicleId).HasColumnType("bigint(20)");
			entity.Property(e => e.State)
			      .HasColumnType("int(11)")
			      .HasDefaultValue(0);
			entity.Property(e => e.CurrentStopId).HasColumnType("bigint(20)");
			entity.Property(e => e.NextStopId).HasColumnType("bigint(20)");
			entity.Property(e => e.ScheduledDeparture)
			      .IsRequired()
			      .HasColumnType("varchar(500)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.ExpectedDeparture)
			      .IsRequired()
			      .HasColumnType("varchar(500)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.DelayMilliseconds)
			      .HasColumnType("bigint(20)")
			      .HasDefaultValue(0L);
			entity.Property(e => e.LastCheckpointUtc).HasColumnType("datetime(6)");

			entity.HasOne(d => d.VehicleService)
			      .WithMany(p => p.Journeys)
			      .HasForeignKey(d => d.VehicleServiceId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleJourneys_VehicleServices");

			entity.HasOne(d => d.VehicleRoute)
			      .WithMany(p => p.Journeys)
			      .HasForeignKey(d => new { d.VehicleRouteId, d.VehicleRouteRevision })
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleJourneys_VehicleRoutes");

			entity.HasOne(d => d.Vehicle)
			      .WithMany()
			      .HasForeignKey(d => d.VehicleId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleJourneys_Vehicles");

			entity.HasOne(d => d.CurrentStop)
			      .WithMany(p => p.CurrentJourneys)
			      .HasForeignKey(d => new { d.CurrentStopId, d.VehicleRouteId, d.VehicleRouteRevision })
			      .HasPrincipalKey(p => new { p.Id, p.VehicleRouteId, p.VehicleRouteRevision })
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleJourneys_CurrentStops");

			entity.HasOne(d => d.NextStop)
			      .WithMany(p => p.NextJourneys)
			      .HasForeignKey(d => new { d.NextStopId, d.VehicleRouteId, d.VehicleRouteRevision })
			      .HasPrincipalKey(p => new { p.Id, p.VehicleRouteId, p.VehicleRouteRevision })
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleJourneys_NextStops");
		});

		modelBuilder.Entity<VehicleJourneyEvent>(entity =>
		{
			entity.ToTable("VehicleJourneyEvents", table =>
			{
				table.HasCheckConstraint("CK_VehicleJourneyEvents_Sequence", "`Sequence` >= 0");
				table.HasCheckConstraint("CK_VehicleJourneyEvents_EventType", "`EventType` BETWEEN 0 AND 11");
				table.HasCheckConstraint("CK_VehicleJourneyEvents_State", "`State` BETWEEN 0 AND 8");
			});

			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.VehicleJourneyId, e.Sequence })
			      .IsUnique()
			      .HasDatabaseName("UX_VehicleJourneyEvents_Journey_Sequence");
			entity.HasIndex(e => e.IdempotencyKey)
			      .IsUnique()
			      .HasDatabaseName("UX_VehicleJourneyEvents_Idempotency");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleJourneyId).HasColumnType("bigint(20)");
			entity.Property(e => e.Sequence).HasColumnType("bigint(20)");
			entity.Property(e => e.IdempotencyKey)
			      .IsRequired()
			      .HasColumnType("varchar(128)")
			      .HasCharSet("ascii")
			      .UseCollation("ascii_general_ci");
			entity.Property(e => e.EventType).HasColumnType("int(11)");
			entity.Property(e => e.State).HasColumnType("int(11)");
			entity.Property(e => e.OccurredAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.WorldTime)
			      .HasColumnType("varchar(500)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.Message)
			      .IsRequired()
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");

			entity.HasOne(d => d.VehicleJourney)
			      .WithMany(p => p.Events)
			      .HasForeignKey(d => d.VehicleJourneyId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_VehicleJourneyEvents_VehicleJourneys");
		});

		modelBuilder.Entity<VehicleDocking>(entity =>
		{
			entity.HasIndex(e => e.VehicleRouteStopId)
			      .HasDatabaseName("FK_VehicleDockings_VehicleRouteStops_idx");

			entity.Property(e => e.VehicleRouteStopId).HasColumnType("bigint(20)");

			entity.HasOne(d => d.VehicleRouteStop)
			      .WithMany(p => p.Dockings)
			      .HasForeignKey(d => d.VehicleRouteStopId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleDockings_VehicleRouteStops");
		});
	}
}
