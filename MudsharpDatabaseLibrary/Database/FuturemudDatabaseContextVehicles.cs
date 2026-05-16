using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<VehicleProto> VehicleProtos { get; set; }
	public virtual DbSet<VehicleCompartmentProto> VehicleCompartmentProtos { get; set; }
	public virtual DbSet<VehicleOccupantSlotProto> VehicleOccupantSlotProtos { get; set; }
	public virtual DbSet<VehicleControlStationProto> VehicleControlStationProtos { get; set; }
	public virtual DbSet<VehicleMovementProfileProto> VehicleMovementProfileProtos { get; set; }
	public virtual DbSet<VehicleAccessPointProto> VehicleAccessPointProtos { get; set; }
	public virtual DbSet<VehicleCargoSpaceProto> VehicleCargoSpaceProtos { get; set; }
	public virtual DbSet<VehicleInstallationPointProto> VehicleInstallationPointProtos { get; set; }
	public virtual DbSet<VehicleTowPointProto> VehicleTowPointProtos { get; set; }
	public virtual DbSet<VehicleDamageZoneProto> VehicleDamageZoneProtos { get; set; }
	public virtual DbSet<VehicleDamageZoneEffectProto> VehicleDamageZoneEffectProtos { get; set; }
	public virtual DbSet<Vehicle> Vehicles { get; set; }
	public virtual DbSet<VehicleCompartment> VehicleCompartments { get; set; }
	public virtual DbSet<VehicleOccupancy> VehicleOccupancies { get; set; }
	public virtual DbSet<VehicleAccessState> VehicleAccessStates { get; set; }
	public virtual DbSet<VehicleAccessPoint> VehicleAccessPoints { get; set; }
	public virtual DbSet<VehicleAccessPointLock> VehicleAccessPointLocks { get; set; }
	public virtual DbSet<VehicleCargoSpace> VehicleCargoSpaces { get; set; }
	public virtual DbSet<VehicleInstallation> VehicleInstallations { get; set; }
	public virtual DbSet<VehicleTowLink> VehicleTowLinks { get; set; }
	public virtual DbSet<VehicleDamageZone> VehicleDamageZones { get; set; }

	private static void ConfigureVehicles(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<VehicleProto>(entity =>
		{
			entity.ToTable("VehicleProtos");
			entity.HasKey(e => new { e.Id, e.RevisionNumber }).HasName("PRIMARY");
			entity.HasIndex(e => e.EditableItemId).HasDatabaseName("FK_VehicleProtos_EditableItems_idx");
			entity.HasIndex(e => new { e.ExteriorItemProtoId, e.ExteriorItemProtoRevision }).HasDatabaseName("FK_VehicleProtos_GameItemProtos_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RevisionNumber).HasColumnType("int(11)");
			entity.Property(e => e.EditableItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name)
			      .IsRequired()
			      .HasColumnType("varchar(200)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.Description)
			      .IsRequired()
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.VehicleScale).HasColumnType("int(11)");
			entity.Property(e => e.ExteriorItemProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.ExteriorItemProtoRevision).HasColumnType("int(11)");

			entity.HasOne(d => d.EditableItem)
			      .WithMany()
			      .HasForeignKey(d => d.EditableItemId)
			      .HasConstraintName("FK_VehicleProtos_EditableItems");

			entity.HasOne(d => d.ExteriorItemProto)
			      .WithMany()
			      .HasForeignKey(d => new { d.ExteriorItemProtoId, d.ExteriorItemProtoRevision })
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_VehicleProtos_GameItemProtos");
		});

		modelBuilder.Entity<VehicleCompartmentProto>(entity =>
		{
			entity.ToTable("VehicleCompartmentProtos");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.VehicleProtoId, e.VehicleProtoRevision }).HasDatabaseName("FK_VehicleCompartmentProtos_VehicleProtos_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoRevision).HasColumnType("int(11)");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.Description).IsRequired().HasColumnType("mediumtext").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.DisplayOrder).HasColumnType("int(11)");

			entity.HasOne(d => d.VehicleProto)
			      .WithMany(p => p.Compartments)
			      .HasForeignKey(d => new { d.VehicleProtoId, d.VehicleProtoRevision })
			      .HasConstraintName("FK_VehicleCompartmentProtos_VehicleProtos");
		});

		modelBuilder.Entity<VehicleOccupantSlotProto>(entity =>
		{
			entity.ToTable("VehicleOccupantSlotProtos");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.VehicleProtoId, e.VehicleProtoRevision }).HasDatabaseName("FK_VehicleOccupantSlotProtos_VehicleProtos_idx");
			entity.HasIndex(e => e.VehicleCompartmentProtoId).HasDatabaseName("FK_VehicleOccupantSlotProtos_Compartments_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoRevision).HasColumnType("int(11)");
			entity.Property(e => e.VehicleCompartmentProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.SlotType).HasColumnType("int(11)");
			entity.Property(e => e.Capacity).HasColumnType("int(11)");
			entity.Property(e => e.RequiredForMovement).HasColumnType("bit(1)");

			entity.HasOne(d => d.VehicleProto)
			      .WithMany(p => p.OccupantSlots)
			      .HasForeignKey(d => new { d.VehicleProtoId, d.VehicleProtoRevision })
			      .HasConstraintName("FK_VehicleOccupantSlotProtos_VehicleProtos");

			entity.HasOne(d => d.VehicleCompartmentProto)
			      .WithMany()
			      .HasForeignKey(d => d.VehicleCompartmentProtoId)
			      .HasConstraintName("FK_VehicleOccupantSlotProtos_Compartments");
		});

		modelBuilder.Entity<VehicleControlStationProto>(entity =>
		{
			entity.ToTable("VehicleControlStationProtos");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.VehicleProtoId, e.VehicleProtoRevision }).HasDatabaseName("FK_VehicleControlStationProtos_VehicleProtos_idx");
			entity.HasIndex(e => e.VehicleOccupantSlotProtoId).HasDatabaseName("FK_VehicleControlStationProtos_Slots_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoRevision).HasColumnType("int(11)");
			entity.Property(e => e.VehicleOccupantSlotProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.IsPrimary).HasColumnType("bit(1)");

			entity.HasOne(d => d.VehicleProto)
			      .WithMany(p => p.ControlStations)
			      .HasForeignKey(d => new { d.VehicleProtoId, d.VehicleProtoRevision })
			      .HasConstraintName("FK_VehicleControlStationProtos_VehicleProtos");

			entity.HasOne(d => d.VehicleOccupantSlotProto)
			      .WithMany()
			      .HasForeignKey(d => d.VehicleOccupantSlotProtoId)
			      .HasConstraintName("FK_VehicleControlStationProtos_Slots");
		});

		modelBuilder.Entity<VehicleMovementProfileProto>(entity =>
		{
			entity.ToTable("VehicleMovementProfileProtos");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.VehicleProtoId, e.VehicleProtoRevision }).HasDatabaseName("FK_VehicleMovementProfileProtos_VehicleProtos_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoRevision).HasColumnType("int(11)");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.MovementType).HasColumnType("int(11)");
			entity.Property(e => e.IsDefault).HasColumnType("bit(1)");
			entity.Property(e => e.RequiredPowerSpikeInWatts).HasColumnType("double");
			entity.Property(e => e.FuelLiquidId).HasColumnType("bigint(20)");
			entity.Property(e => e.FuelVolumePerMove).HasColumnType("double");
			entity.Property(e => e.RequiredInstalledRole).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.RequiresTowLinksClosed).HasColumnType("bit(1)");
			entity.Property(e => e.RequiresAccessPointsClosed).HasColumnType("bit(1)");

			entity.HasOne(d => d.VehicleProto)
			      .WithMany(p => p.MovementProfiles)
			      .HasForeignKey(d => new { d.VehicleProtoId, d.VehicleProtoRevision })
			      .HasConstraintName("FK_VehicleMovementProfileProtos_VehicleProtos");
		});

		modelBuilder.Entity<VehicleAccessPointProto>(entity =>
		{
			entity.ToTable("VehicleAccessPointProtos");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.VehicleProtoId, e.VehicleProtoRevision }).HasDatabaseName("FK_VehicleAccessPointProtos_VehicleProtos_idx");
			entity.HasIndex(e => e.VehicleCompartmentProtoId).HasDatabaseName("FK_VehicleAccessPointProtos_Compartments_idx");
			entity.HasIndex(e => new { e.ProjectionItemProtoId, e.ProjectionItemProtoRevision }).HasDatabaseName("FK_VehicleAccessPointProtos_ItemProtos_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoRevision).HasColumnType("int(11)");
			entity.Property(e => e.VehicleCompartmentProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.Description).IsRequired().HasColumnType("mediumtext").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.AccessPointType).HasColumnType("int(11)");
			entity.Property(e => e.ProjectionItemProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.ProjectionItemProtoRevision).HasColumnType("int(11)");
			entity.Property(e => e.StartsOpen).HasColumnType("bit(1)");
			entity.Property(e => e.MustBeClosedForMovement).HasColumnType("bit(1)");
			entity.Property(e => e.DisplayOrder).HasColumnType("int(11)");

			entity.HasOne(d => d.VehicleProto)
			      .WithMany(p => p.AccessPoints)
			      .HasForeignKey(d => new { d.VehicleProtoId, d.VehicleProtoRevision })
			      .HasConstraintName("FK_VehicleAccessPointProtos_VehicleProtos");

			entity.HasOne(d => d.VehicleCompartmentProto)
			      .WithMany()
			      .HasForeignKey(d => d.VehicleCompartmentProtoId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_VehicleAccessPointProtos_Compartments");

			entity.HasOne(d => d.ProjectionItemProto)
			      .WithMany()
			      .HasForeignKey(d => new { d.ProjectionItemProtoId, d.ProjectionItemProtoRevision })
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_VehicleAccessPointProtos_ItemProtos");
		});

		modelBuilder.Entity<VehicleCargoSpaceProto>(entity =>
		{
			entity.ToTable("VehicleCargoSpaceProtos");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.VehicleProtoId, e.VehicleProtoRevision }).HasDatabaseName("FK_VehicleCargoSpaceProtos_VehicleProtos_idx");
			entity.HasIndex(e => e.VehicleCompartmentProtoId).HasDatabaseName("FK_VehicleCargoSpaceProtos_Compartments_idx");
			entity.HasIndex(e => e.RequiredAccessPointProtoId).HasDatabaseName("FK_VehicleCargoSpaceProtos_AccessPoints_idx");
			entity.HasIndex(e => new { e.ProjectionItemProtoId, e.ProjectionItemProtoRevision }).HasDatabaseName("FK_VehicleCargoSpaceProtos_ItemProtos_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoRevision).HasColumnType("int(11)");
			entity.Property(e => e.VehicleCompartmentProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.RequiredAccessPointProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.Description).IsRequired().HasColumnType("mediumtext").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.ProjectionItemProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.ProjectionItemProtoRevision).HasColumnType("int(11)");
			entity.Property(e => e.DisplayOrder).HasColumnType("int(11)");

			entity.HasOne(d => d.VehicleProto)
			      .WithMany(p => p.CargoSpaces)
			      .HasForeignKey(d => new { d.VehicleProtoId, d.VehicleProtoRevision })
			      .HasConstraintName("FK_VehicleCargoSpaceProtos_VehicleProtos");

			entity.HasOne(d => d.VehicleCompartmentProto)
			      .WithMany()
			      .HasForeignKey(d => d.VehicleCompartmentProtoId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_VehicleCargoSpaceProtos_Compartments");

			entity.HasOne(d => d.RequiredAccessPointProto)
			      .WithMany()
			      .HasForeignKey(d => d.RequiredAccessPointProtoId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_VehicleCargoSpaceProtos_AccessPoints");

			entity.HasOne(d => d.ProjectionItemProto)
			      .WithMany()
			      .HasForeignKey(d => new { d.ProjectionItemProtoId, d.ProjectionItemProtoRevision })
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_VehicleCargoSpaceProtos_ItemProtos");
		});

		modelBuilder.Entity<VehicleInstallationPointProto>(entity =>
		{
			entity.ToTable("VehicleInstallationPointProtos");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.VehicleProtoId, e.VehicleProtoRevision }).HasDatabaseName("FK_VehicleInstallationPointProtos_VehicleProtos_idx");
			entity.HasIndex(e => e.RequiredAccessPointProtoId).HasDatabaseName("FK_VehicleInstallationPointProtos_AccessPoints_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoRevision).HasColumnType("int(11)");
			entity.Property(e => e.RequiredAccessPointProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.Description).IsRequired().HasColumnType("mediumtext").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.MountType).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.RequiredRole).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.RequiredForMovement).HasColumnType("bit(1)");
			entity.Property(e => e.DisplayOrder).HasColumnType("int(11)");

			entity.HasOne(d => d.VehicleProto)
			      .WithMany(p => p.InstallationPoints)
			      .HasForeignKey(d => new { d.VehicleProtoId, d.VehicleProtoRevision })
			      .HasConstraintName("FK_VehicleInstallationPointProtos_VehicleProtos");

			entity.HasOne(d => d.RequiredAccessPointProto)
			      .WithMany()
			      .HasForeignKey(d => d.RequiredAccessPointProtoId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_VehicleInstallationPointProtos_AccessPoints");
		});

		modelBuilder.Entity<VehicleTowPointProto>(entity =>
		{
			entity.ToTable("VehicleTowPointProtos");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.VehicleProtoId, e.VehicleProtoRevision }).HasDatabaseName("FK_VehicleTowPointProtos_VehicleProtos_idx");
			entity.HasIndex(e => e.RequiredAccessPointProtoId).HasDatabaseName("FK_VehicleTowPointProtos_AccessPoints_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoRevision).HasColumnType("int(11)");
			entity.Property(e => e.RequiredAccessPointProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.Description).IsRequired().HasColumnType("mediumtext").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.TowType).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.CanTow).HasColumnType("bit(1)");
			entity.Property(e => e.CanBeTowed).HasColumnType("bit(1)");
			entity.Property(e => e.MaximumTowedWeight).HasColumnType("double");
			entity.Property(e => e.DisplayOrder).HasColumnType("int(11)");

			entity.HasOne(d => d.VehicleProto)
			      .WithMany(p => p.TowPoints)
			      .HasForeignKey(d => new { d.VehicleProtoId, d.VehicleProtoRevision })
			      .HasConstraintName("FK_VehicleTowPointProtos_VehicleProtos");

			entity.HasOne(d => d.RequiredAccessPointProto)
			      .WithMany()
			      .HasForeignKey(d => d.RequiredAccessPointProtoId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_VehicleTowPointProtos_AccessPoints");
		});

		modelBuilder.Entity<VehicleDamageZoneProto>(entity =>
		{
			entity.ToTable("VehicleDamageZoneProtos");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.VehicleProtoId, e.VehicleProtoRevision }).HasDatabaseName("FK_VehicleDamageZoneProtos_VehicleProtos_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoRevision).HasColumnType("int(11)");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.Description).IsRequired().HasColumnType("mediumtext").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.MaximumDamage).HasColumnType("double");
			entity.Property(e => e.HitWeight).HasColumnType("double");
			entity.Property(e => e.DisabledThreshold).HasColumnType("double");
			entity.Property(e => e.DestroyedThreshold).HasColumnType("double");
			entity.Property(e => e.DisablesMovement).HasColumnType("bit(1)");
			entity.Property(e => e.DisplayOrder).HasColumnType("int(11)");

			entity.HasOne(d => d.VehicleProto)
			      .WithMany(p => p.DamageZones)
			      .HasForeignKey(d => new { d.VehicleProtoId, d.VehicleProtoRevision })
			      .HasConstraintName("FK_VehicleDamageZoneProtos_VehicleProtos");
		});

		modelBuilder.Entity<VehicleDamageZoneEffectProto>(entity =>
		{
			entity.ToTable("VehicleDamageZoneEffectProtos");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.VehicleDamageZoneProtoId).HasDatabaseName("FK_VehicleDamageZoneEffectProtos_DamageZones_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleDamageZoneProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.TargetType).HasColumnType("int(11)");
			entity.Property(e => e.TargetProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.MinimumStatus).HasColumnType("int(11)");

			entity.HasOne(d => d.VehicleDamageZoneProto)
			      .WithMany(p => p.Effects)
			      .HasForeignKey(d => d.VehicleDamageZoneProtoId)
			      .HasConstraintName("FK_VehicleDamageZoneEffectProtos_DamageZones");
		});

		modelBuilder.Entity<Vehicle>(entity =>
		{
			entity.ToTable("Vehicles");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.VehicleProtoId, e.VehicleProtoRevision }).HasDatabaseName("FK_Vehicles_VehicleProtos_idx");
			entity.HasIndex(e => e.ExteriorItemId).IsUnique().HasDatabaseName("FK_Vehicles_GameItems_Exterior_idx");
			entity.HasIndex(e => e.CurrentCellId).HasDatabaseName("FK_Vehicles_Cells_Current_idx");
			entity.HasIndex(e => e.DestinationCellId).HasDatabaseName("FK_Vehicles_Cells_Destination_idx");
			entity.HasIndex(e => e.CurrentExitId).HasDatabaseName("FK_Vehicles_Exits_idx");
			entity.HasIndex(e => e.MovementProfileProtoId).HasDatabaseName("FK_Vehicles_MovementProfileProtos_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoRevision).HasColumnType("int(11)");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.ExteriorItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.LocationType).HasColumnType("int(11)");
			entity.Property(e => e.CurrentCellId).HasColumnType("bigint(20)");
			entity.Property(e => e.CurrentRoomLayer).HasColumnType("int(11)");
			entity.Property(e => e.MovementStatus).HasColumnType("int(11)");
			entity.Property(e => e.CurrentExitId).HasColumnType("bigint(20)");
			entity.Property(e => e.DestinationCellId).HasColumnType("bigint(20)");
			entity.Property(e => e.MovementProfileProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
			entity.Property(e => e.LastMovementDateTime).HasColumnType("datetime");

			entity.HasOne(d => d.VehicleProto)
			      .WithMany(p => p.Vehicles)
			      .HasForeignKey(d => new { d.VehicleProtoId, d.VehicleProtoRevision })
			      .HasConstraintName("FK_Vehicles_VehicleProtos");

			entity.HasOne(d => d.ExteriorItem)
			      .WithMany()
			      .HasForeignKey(d => d.ExteriorItemId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_Vehicles_GameItems_Exterior");

			entity.HasOne(d => d.CurrentCell)
			      .WithMany()
			      .HasForeignKey(d => d.CurrentCellId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_Vehicles_Cells_Current");

			entity.HasOne(d => d.DestinationCell)
			      .WithMany()
			      .HasForeignKey(d => d.DestinationCellId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_Vehicles_Cells_Destination");

			entity.HasOne(d => d.CurrentExit)
			      .WithMany()
			      .HasForeignKey(d => d.CurrentExitId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_Vehicles_Exits");

			entity.HasOne(d => d.MovementProfileProto)
			      .WithMany()
			      .HasForeignKey(d => d.MovementProfileProtoId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_Vehicles_MovementProfileProtos");
		});

		modelBuilder.Entity<VehicleCompartment>(entity =>
		{
			entity.ToTable("VehicleCompartments");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.VehicleId).HasDatabaseName("FK_VehicleCompartments_Vehicles_idx");
			entity.HasIndex(e => e.VehicleCompartmentProtoId).HasDatabaseName("FK_VehicleCompartments_Protos_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleCompartmentProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");

			entity.HasOne(d => d.Vehicle)
			      .WithMany(p => p.Compartments)
			      .HasForeignKey(d => d.VehicleId)
			      .HasConstraintName("FK_VehicleCompartments_Vehicles");

			entity.HasOne(d => d.VehicleCompartmentProto)
			      .WithMany()
			      .HasForeignKey(d => d.VehicleCompartmentProtoId)
			      .HasConstraintName("FK_VehicleCompartments_Protos");
		});

		modelBuilder.Entity<VehicleOccupancy>(entity =>
		{
			entity.ToTable("VehicleOccupancies");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.VehicleId).HasDatabaseName("FK_VehicleOccupancies_Vehicles_idx");
			entity.HasIndex(e => e.CharacterId).HasDatabaseName("FK_VehicleOccupancies_Characters_idx");
			entity.HasIndex(e => e.VehicleOccupantSlotProtoId).HasDatabaseName("FK_VehicleOccupancies_Slots_idx");
			entity.HasIndex(e => new { e.VehicleId, e.CharacterId }).IsUnique().HasDatabaseName("IX_VehicleOccupancies_Vehicle_Character");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleId).HasColumnType("bigint(20)");
			entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleOccupantSlotProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.IsController).HasColumnType("bit(1)");

			entity.HasOne(d => d.Vehicle)
			      .WithMany(p => p.Occupancies)
			      .HasForeignKey(d => d.VehicleId)
			      .HasConstraintName("FK_VehicleOccupancies_Vehicles");

			entity.HasOne(d => d.Character)
			      .WithMany()
			      .HasForeignKey(d => d.CharacterId)
			      .HasConstraintName("FK_VehicleOccupancies_Characters");

			entity.HasOne(d => d.VehicleOccupantSlotProto)
			      .WithMany()
			      .HasForeignKey(d => d.VehicleOccupantSlotProtoId)
			      .HasConstraintName("FK_VehicleOccupancies_Slots");
		});

		modelBuilder.Entity<VehicleAccessState>(entity =>
		{
			entity.ToTable("VehicleAccessStates");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.VehicleId).HasDatabaseName("FK_VehicleAccessStates_Vehicles_idx");
			entity.HasIndex(e => e.CharacterId).HasDatabaseName("FK_VehicleAccessStates_Characters_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleId).HasColumnType("bigint(20)");
			entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.AccessTag).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.AccessLevel).HasColumnType("int(11)");

			entity.HasOne(d => d.Vehicle)
			      .WithMany(p => p.AccessStates)
			      .HasForeignKey(d => d.VehicleId)
			      .HasConstraintName("FK_VehicleAccessStates_Vehicles");

			entity.HasOne(d => d.Character)
			      .WithMany()
			      .HasForeignKey(d => d.CharacterId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_VehicleAccessStates_Characters");
		});

		modelBuilder.Entity<VehicleAccessPoint>(entity =>
		{
			entity.ToTable("VehicleAccessPoints");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.VehicleId).HasDatabaseName("FK_VehicleAccessPoints_Vehicles_idx");
			entity.HasIndex(e => e.VehicleAccessPointProtoId).HasDatabaseName("FK_VehicleAccessPoints_Protos_idx");
			entity.HasIndex(e => e.ProjectionItemId).IsUnique().HasDatabaseName("FK_VehicleAccessPoints_GameItems_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleAccessPointProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.ProjectionItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.IsOpen).HasColumnType("bit(1)");
			entity.Property(e => e.IsDisabled).HasColumnType("bit(1)");

			entity.HasOne(d => d.Vehicle)
			      .WithMany(p => p.AccessPoints)
			      .HasForeignKey(d => d.VehicleId)
			      .HasConstraintName("FK_VehicleAccessPoints_Vehicles");

			entity.HasOne(d => d.VehicleAccessPointProto)
			      .WithMany()
			      .HasForeignKey(d => d.VehicleAccessPointProtoId)
			      .HasConstraintName("FK_VehicleAccessPoints_Protos");

			entity.HasOne(d => d.ProjectionItem)
			      .WithMany()
			      .HasForeignKey(d => d.ProjectionItemId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_VehicleAccessPoints_GameItems");
		});

		modelBuilder.Entity<VehicleAccessPointLock>(entity =>
		{
			entity.ToTable("VehicleAccessPointLocks");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.VehicleAccessPointId).HasDatabaseName("FK_VehicleAccessPointLocks_AccessPoints_idx");
			entity.HasIndex(e => e.LockItemId).IsUnique().HasDatabaseName("FK_VehicleAccessPointLocks_GameItems_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleAccessPointId).HasColumnType("bigint(20)");
			entity.Property(e => e.LockItemId).HasColumnType("bigint(20)");

			entity.HasOne(d => d.VehicleAccessPoint)
			      .WithMany(p => p.Locks)
			      .HasForeignKey(d => d.VehicleAccessPointId)
			      .HasConstraintName("FK_VehicleAccessPointLocks_AccessPoints");

			entity.HasOne(d => d.LockItem)
			      .WithMany()
			      .HasForeignKey(d => d.LockItemId)
			      .HasConstraintName("FK_VehicleAccessPointLocks_GameItems");
		});

		modelBuilder.Entity<VehicleCargoSpace>(entity =>
		{
			entity.ToTable("VehicleCargoSpaces");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.VehicleId).HasDatabaseName("FK_VehicleCargoSpaces_Vehicles_idx");
			entity.HasIndex(e => e.VehicleCargoSpaceProtoId).HasDatabaseName("FK_VehicleCargoSpaces_Protos_idx");
			entity.HasIndex(e => e.ProjectionItemId).IsUnique().HasDatabaseName("FK_VehicleCargoSpaces_GameItems_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleCargoSpaceProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.ProjectionItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.IsDisabled).HasColumnType("bit(1)");

			entity.HasOne(d => d.Vehicle)
			      .WithMany(p => p.CargoSpaces)
			      .HasForeignKey(d => d.VehicleId)
			      .HasConstraintName("FK_VehicleCargoSpaces_Vehicles");

			entity.HasOne(d => d.VehicleCargoSpaceProto)
			      .WithMany()
			      .HasForeignKey(d => d.VehicleCargoSpaceProtoId)
			      .HasConstraintName("FK_VehicleCargoSpaces_Protos");

			entity.HasOne(d => d.ProjectionItem)
			      .WithMany()
			      .HasForeignKey(d => d.ProjectionItemId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_VehicleCargoSpaces_GameItems");
		});

		modelBuilder.Entity<VehicleInstallation>(entity =>
		{
			entity.ToTable("VehicleInstallations");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.VehicleId).HasDatabaseName("FK_VehicleInstallations_Vehicles_idx");
			entity.HasIndex(e => e.VehicleInstallationPointProtoId).HasDatabaseName("FK_VehicleInstallations_Protos_idx");
			entity.HasIndex(e => e.InstalledItemId).IsUnique().HasDatabaseName("FK_VehicleInstallations_GameItems_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleInstallationPointProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.InstalledItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.IsDisabled).HasColumnType("bit(1)");

			entity.HasOne(d => d.Vehicle)
			      .WithMany(p => p.Installations)
			      .HasForeignKey(d => d.VehicleId)
			      .HasConstraintName("FK_VehicleInstallations_Vehicles");

			entity.HasOne(d => d.VehicleInstallationPointProto)
			      .WithMany()
			      .HasForeignKey(d => d.VehicleInstallationPointProtoId)
			      .HasConstraintName("FK_VehicleInstallations_Protos");

			entity.HasOne(d => d.InstalledItem)
			      .WithMany()
			      .HasForeignKey(d => d.InstalledItemId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_VehicleInstallations_GameItems");
		});

		modelBuilder.Entity<VehicleTowLink>(entity =>
		{
			entity.ToTable("VehicleTowLinks");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.SourceVehicleId).HasDatabaseName("FK_VehicleTowLinks_SourceVehicles_idx");
			entity.HasIndex(e => e.TargetVehicleId).HasDatabaseName("FK_VehicleTowLinks_TargetVehicles_idx");
			entity.HasIndex(e => e.SourceTowPointProtoId).HasDatabaseName("FK_VehicleTowLinks_SourceTowPointProtos_idx");
			entity.HasIndex(e => e.TargetTowPointProtoId).HasDatabaseName("FK_VehicleTowLinks_TargetTowPointProtos_idx");
			entity.HasIndex(e => e.HitchItemId).HasDatabaseName("FK_VehicleTowLinks_GameItems_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.SourceVehicleId).HasColumnType("bigint(20)");
			entity.Property(e => e.TargetVehicleId).HasColumnType("bigint(20)");
			entity.Property(e => e.SourceTowPointProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.TargetTowPointProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.HitchItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.IsDisabled).HasColumnType("bit(1)");
			entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

			entity.HasOne(d => d.SourceVehicle)
			      .WithMany(p => p.SourceTowLinks)
			      .HasForeignKey(d => d.SourceVehicleId)
			      .HasConstraintName("FK_VehicleTowLinks_SourceVehicles");

			entity.HasOne(d => d.TargetVehicle)
			      .WithMany(p => p.TargetTowLinks)
			      .HasForeignKey(d => d.TargetVehicleId)
			      .HasConstraintName("FK_VehicleTowLinks_TargetVehicles");

			entity.HasOne(d => d.SourceTowPointProto)
			      .WithMany()
			      .HasForeignKey(d => d.SourceTowPointProtoId)
			      .HasConstraintName("FK_VehicleTowLinks_SourceTowPointProtos");

			entity.HasOne(d => d.TargetTowPointProto)
			      .WithMany()
			      .HasForeignKey(d => d.TargetTowPointProtoId)
			      .HasConstraintName("FK_VehicleTowLinks_TargetTowPointProtos");

			entity.HasOne(d => d.HitchItem)
			      .WithMany()
			      .HasForeignKey(d => d.HitchItemId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_VehicleTowLinks_GameItems");
		});

		modelBuilder.Entity<VehicleDamageZone>(entity =>
		{
			entity.ToTable("VehicleDamageZones");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.VehicleId).HasDatabaseName("FK_VehicleDamageZones_Vehicles_idx");
			entity.HasIndex(e => e.VehicleDamageZoneProtoId).HasDatabaseName("FK_VehicleDamageZones_Protos_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleDamageZoneProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.CurrentDamage).HasColumnType("double");
			entity.Property(e => e.Status).HasColumnType("int(11)");

			entity.HasOne(d => d.Vehicle)
			      .WithMany(p => p.DamageZones)
			      .HasForeignKey(d => d.VehicleId)
			      .HasConstraintName("FK_VehicleDamageZones_Vehicles");

			entity.HasOne(d => d.VehicleDamageZoneProto)
			      .WithMany()
			      .HasForeignKey(d => d.VehicleDamageZoneProtoId)
			      .HasConstraintName("FK_VehicleDamageZones_Protos");
		});
	}
}
