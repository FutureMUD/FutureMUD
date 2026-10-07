using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

#nullable enable

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<AreasRooms> AreasRooms { get; set; } = null!;
	public virtual DbSet<RoomSpatialMigrationLedger> RoomSpatialMigrationLedgers { get; set; } = null!;
	public virtual DbSet<RoomSpatialAreaMigrationLedger> RoomSpatialAreaMigrationLedgers { get; set; } = null!;
	public virtual DbSet<RoomSpatialContractionLedger> RoomSpatialContractionLedgers { get; set; } = null!;
	public virtual DbSet<RoomSpatialAreaContractionLedger> RoomSpatialAreaContractionLedgers { get; set; } = null!;

	private static void ConfigureRoomSpatialOwnership(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Room>(entity =>
		{
			entity.Property(x => x.ZoneId).HasColumnType("bigint(20)");
			entity.Property(x => x.X).HasColumnType("int(11)");
			entity.Property(x => x.Y).HasColumnType("int(11)");
			entity.Property(x => x.Z).HasColumnType("int(11)");
			entity.HasIndex(x => x.ZoneId).HasDatabaseName("IX_Rooms_ZoneId");
			entity.HasOne(x => x.Zone).WithMany(x => x.OwnedRooms).HasForeignKey(x => x.ZoneId)
				.OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Rooms_OwningZone");
		});
		modelBuilder.Entity<AreasRooms>(entity =>
		{
			entity.ToTable("Areas_Rooms");
			entity.HasKey(x => new { x.AreaId, x.RoomId });
			entity.Property(x => x.AreaId).HasColumnType("bigint(20)");
			entity.Property(x => x.RoomId).HasColumnType("bigint(20)");
			entity.HasOne(x => x.Area).WithMany(x => x.AreasRooms).HasForeignKey(x => x.AreaId)
				.HasConstraintName("FK_Areas_Rooms_Areas");
			entity.HasOne(x => x.Room).WithMany(x => x.AreasRooms).HasForeignKey(x => x.RoomId)
				.HasConstraintName("FK_Areas_Rooms_Rooms");
		});
		modelBuilder.Entity<RoomSpatialMigrationLedger>(entity =>
		{
			entity.ToTable("RoomSpatialMigrationLedger");
			entity.HasKey(x => x.LegacyRoomId);
			entity.Property(x => x.LegacyRoomId).HasColumnType("bigint(20)").ValueGeneratedNever();
			entity.Property(x => x.RoomId).HasColumnType("bigint(20)");
			entity.Property(x => x.ZoneId).HasColumnType("bigint(20)");
			entity.Property(x => x.X).HasColumnType("int(11)");
			entity.Property(x => x.Y).HasColumnType("int(11)");
			entity.Property(x => x.Z).HasColumnType("int(11)");
			entity.Property(x => x.Warning).HasMaxLength(255).HasCharSet("utf8mb4")
				.UseCollation("utf8mb4_general_ci");
		});
		modelBuilder.Entity<RoomSpatialAreaMigrationLedger>(entity =>
		{
			entity.ToTable("RoomSpatialAreaMigrationLedger");
			entity.HasKey(x => new { x.AreaId, x.LegacyRoomId });
			entity.Property(x => x.AreaId).HasColumnType("bigint(20)");
			entity.Property(x => x.LegacyRoomId).HasColumnType("bigint(20)");
			entity.Property(x => x.RoomId).HasColumnType("bigint(20)");
		});
		modelBuilder.Entity<RoomSpatialContractionLedger>(entity =>
		{
			entity.ToTable("RoomSpatialContractionLedger");
			entity.HasKey(x => x.LegacyRoomId);
			entity.Property(x => x.LegacyRoomId).HasColumnType("bigint(20)").ValueGeneratedNever();
			entity.Property(x => x.RoomId).HasColumnType("bigint(20)");
			entity.Property(x => x.ZoneId).HasColumnType("bigint(20)");
			entity.Property(x => x.X).HasColumnType("int(11)");
			entity.Property(x => x.Y).HasColumnType("int(11)");
			entity.Property(x => x.Z).HasColumnType("int(11)");
			entity.Property(x => x.Warning).HasMaxLength(255).HasCharSet("utf8mb4").UseCollation("utf8mb4_general_ci");
		});
		modelBuilder.Entity<RoomSpatialAreaContractionLedger>(entity =>
		{
			entity.ToTable("RoomSpatialAreaContractionLedger");
			entity.HasKey(x => new { x.AreaId, x.LegacyRoomId });
			entity.Property(x => x.AreaId).HasColumnType("bigint(20)");
			entity.Property(x => x.LegacyRoomId).HasColumnType("bigint(20)");
			entity.Property(x => x.RoomId).HasColumnType("bigint(20)");
		});
	}
}
