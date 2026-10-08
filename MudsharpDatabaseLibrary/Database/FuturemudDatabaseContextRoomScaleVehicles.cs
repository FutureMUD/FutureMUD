#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<VehicleCompartmentLinkProto> VehicleCompartmentLinkProtos { get; set; } = null!;
	public virtual DbSet<VehicleDocking> VehicleDockings { get; set; } = null!;

	private static void ConfigureRoomScaleVehicleInteriors(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<VehicleCompartmentProto>(entity =>
		{
			entity.HasIndex(e => e.InteriorTerrainId)
			      .HasDatabaseName("FK_VehicleCompartmentProtos_Terrains_idx");

			entity.Property(e => e.InteriorTerrainId).HasColumnType("bigint(20)");
			entity.Property(e => e.InteriorOutdoorsType)
			      .HasColumnType("int(11)")
			      .HasDefaultValue(0);

			entity.HasOne(d => d.InteriorTerrain)
			      .WithMany()
			      .HasForeignKey(d => d.InteriorTerrainId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleCompartmentProtos_Terrains");
		});

		modelBuilder.Entity<VehicleCompartmentLinkProto>(entity =>
		{
			entity.ToTable("VehicleCompartmentLinkProtos");
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.HasIndex(e => new { e.VehicleProtoId, e.VehicleProtoRevision })
			      .HasDatabaseName("FK_VehicleCompartmentLinkProtos_VehicleProtos_idx");
			entity.HasIndex(e => e.SourceVehicleCompartmentProtoId)
			      .HasDatabaseName("FK_VehicleCompartmentLinkProtos_Source_idx");
			entity.HasIndex(e => e.DestinationVehicleCompartmentProtoId)
			      .HasDatabaseName("FK_VehicleCompartmentLinkProtos_Destination_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleProtoRevision).HasColumnType("int(11)");
			entity.Property(e => e.SourceVehicleCompartmentProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.DestinationVehicleCompartmentProtoId).HasColumnType("bigint(20)");
			entity.Property(e => e.OutboundDirection)
			      .IsRequired()
			      .HasColumnType("varchar(100)")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.InboundDirection)
			      .IsRequired()
			      .HasColumnType("varchar(100)")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.OutboundDescription)
			      .IsRequired()
			      .HasColumnType("varchar(500)")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.InboundDescription)
			      .IsRequired()
			      .HasColumnType("varchar(500)")
			      .HasCharSet("utf8mb4")
			      .UseCollation("utf8mb4_unicode_ci");

			entity.HasOne(d => d.VehicleProto)
			      .WithMany(p => p.CompartmentLinks)
			      .HasForeignKey(d => new { d.VehicleProtoId, d.VehicleProtoRevision })
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_VehicleCompartmentLinkProtos_VehicleProtos");

			entity.HasOne(d => d.SourceVehicleCompartmentProto)
			      .WithMany(p => p.SourceLinks)
			      .HasForeignKey(d => d.SourceVehicleCompartmentProtoId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleCompartmentLinkProtos_Source");

			entity.HasOne(d => d.DestinationVehicleCompartmentProto)
			      .WithMany(p => p.DestinationLinks)
			      .HasForeignKey(d => d.DestinationVehicleCompartmentProtoId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleCompartmentLinkProtos_Destination");
		});

		modelBuilder.Entity<VehicleCompartment>(entity =>
		{
			entity.HasIndex(e => e.InteriorRoomId)
			      .IsUnique()
			      .HasDatabaseName("UX_VehicleCompartments_InteriorRoom");

			entity.Property(e => e.InteriorRoomId).HasColumnType("bigint(20)");

			entity.HasOne(d => d.InteriorRoom)
			      .WithMany()
			      .HasForeignKey(d => d.InteriorRoomId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleCompartments_InteriorRooms");
		});

		modelBuilder.Entity<Room>(entity =>
		{
			entity.ToTable("Rooms", table =>
			{
				table.HasCheckConstraint("CK_Rooms_HostedVehicleOwnership",
					"(`HostedVehicleId` IS NULL AND `HostedVehicleCompartmentId` IS NULL) OR " +
					"(`HostedVehicleId` IS NOT NULL AND `HostedVehicleCompartmentId` IS NOT NULL)");
			});

			entity.HasIndex(e => e.HostedVehicleId)
			      .HasDatabaseName("FK_Rooms_HostedVehicles_idx");
			entity.HasIndex(e => e.HostedVehicleCompartmentId)
			      .IsUnique()
			      .HasDatabaseName("UX_Rooms_HostedVehicleCompartments");

			entity.Property(e => e.HostedVehicleId).HasColumnType("bigint(20)");
			entity.Property(e => e.HostedVehicleCompartmentId).HasColumnType("bigint(20)");

			entity.HasOne(d => d.HostedVehicle)
			      .WithMany(p => p.HostedRooms)
			      .HasForeignKey(d => d.HostedVehicleId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_Rooms_HostedVehicles");

			entity.HasOne(d => d.HostedVehicleCompartment)
			      .WithMany()
			      .HasForeignKey(d => d.HostedVehicleCompartmentId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_Rooms_HostedVehicleCompartments");
		});

		modelBuilder.Entity<VehicleDocking>(entity =>
		{
			entity.ToTable("VehicleDockings");
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.HasIndex(e => e.VehicleAccessPointId)
			      .IsUnique()
			      .HasDatabaseName("UX_VehicleDockings_AccessPoint");
			entity.HasIndex(e => new { e.VehicleId, e.State })
			      .HasDatabaseName("IX_VehicleDockings_Vehicle_State");
			entity.HasIndex(e => e.VehicleCompartmentId)
			      .HasDatabaseName("FK_VehicleDockings_Compartments_idx");
			entity.HasIndex(e => new { e.ExteriorRoomId, e.ExteriorRoomLayer })
			      .HasDatabaseName("IX_VehicleDockings_ExteriorRoom_Layer");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleAccessPointId).HasColumnType("bigint(20)");
			entity.Property(e => e.VehicleCompartmentId).HasColumnType("bigint(20)");
			entity.Property(e => e.ExteriorRoomId).HasColumnType("bigint(20)");
			entity.Property(e => e.ExteriorRoomLayer).HasColumnType("int(11)");
			entity.Property(e => e.State)
			      .HasColumnType("int(11)")
			      .HasDefaultValue(0);

			entity.HasOne(d => d.Vehicle)
			      .WithMany(p => p.Dockings)
			      .HasForeignKey(d => d.VehicleId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleDockings_Vehicles");

			entity.HasOne(d => d.VehicleAccessPoint)
			      .WithOne(p => p.Docking)
			      .HasForeignKey<VehicleDocking>(d => d.VehicleAccessPointId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleDockings_AccessPoints");

			entity.HasOne(d => d.VehicleCompartment)
			      .WithMany(p => p.Dockings)
			      .HasForeignKey(d => d.VehicleCompartmentId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleDockings_Compartments");

			entity.HasOne(d => d.ExteriorRoom)
			      .WithMany(p => p.VehicleDockings)
			      .HasForeignKey(d => d.ExteriorRoomId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_VehicleDockings_ExteriorRooms");
		});
	}
}
