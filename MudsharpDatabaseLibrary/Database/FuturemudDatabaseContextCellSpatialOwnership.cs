using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

#nullable enable

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<AreasCells> AreasCells { get; set; } = null!;
	public virtual DbSet<CellRoomMigrationLedger> CellRoomMigrationLedgers { get; set; } = null!;
	public virtual DbSet<CellRoomAreaMigrationLedger> CellRoomAreaMigrationLedgers { get; set; } = null!;
	public virtual DbSet<CellRoomContractionLedger> CellRoomContractionLedgers { get; set; } = null!;
	public virtual DbSet<CellRoomAreaContractionLedger> CellRoomAreaContractionLedgers { get; set; } = null!;

	private static void ConfigureCellSpatialOwnership(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Cell>(entity =>
		{
			entity.Property(x => x.ZoneId).HasColumnType("bigint(20)");
			entity.Property(x => x.X).HasColumnType("int(11)");
			entity.Property(x => x.Y).HasColumnType("int(11)");
			entity.Property(x => x.Z).HasColumnType("int(11)");
			entity.HasIndex(x => x.ZoneId).HasDatabaseName("IX_Cells_ZoneId");
			entity.HasOne(x => x.Zone).WithMany(x => x.OwnedCells).HasForeignKey(x => x.ZoneId)
				.OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Cells_OwningZone");
		});
		modelBuilder.Entity<AreasCells>(entity =>
		{
			entity.ToTable("Areas_Cells");
			entity.HasKey(x => new { x.AreaId, x.CellId });
			entity.Property(x => x.AreaId).HasColumnType("bigint(20)");
			entity.Property(x => x.CellId).HasColumnType("bigint(20)");
			entity.HasOne(x => x.Area).WithMany(x => x.AreasCells).HasForeignKey(x => x.AreaId)
				.HasConstraintName("FK_Areas_Cells_Areas");
			entity.HasOne(x => x.Cell).WithMany(x => x.AreasCells).HasForeignKey(x => x.CellId)
				.HasConstraintName("FK_Areas_Cells_Cells");
		});
		modelBuilder.Entity<CellRoomMigrationLedger>(entity =>
		{
			entity.ToTable("CellRoomMigrationLedger");
			entity.HasKey(x => x.RoomId);
			entity.Property(x => x.RoomId).HasColumnType("bigint(20)").ValueGeneratedNever();
			entity.Property(x => x.CellId).HasColumnType("bigint(20)");
			entity.Property(x => x.ZoneId).HasColumnType("bigint(20)");
			entity.Property(x => x.X).HasColumnType("int(11)");
			entity.Property(x => x.Y).HasColumnType("int(11)");
			entity.Property(x => x.Z).HasColumnType("int(11)");
			entity.Property(x => x.Warning).HasMaxLength(255).HasCharSet("utf8mb4")
				.UseCollation("utf8mb4_general_ci");
		});
		modelBuilder.Entity<CellRoomAreaMigrationLedger>(entity =>
		{
			entity.ToTable("CellRoomAreaMigrationLedger");
			entity.HasKey(x => new { x.AreaId, x.RoomId });
			entity.Property(x => x.AreaId).HasColumnType("bigint(20)");
			entity.Property(x => x.RoomId).HasColumnType("bigint(20)");
			entity.Property(x => x.CellId).HasColumnType("bigint(20)");
		});
		modelBuilder.Entity<CellRoomContractionLedger>(entity =>
		{
			entity.ToTable("CellRoomContractionLedger");
			entity.HasKey(x => x.RoomId);
			entity.Property(x => x.RoomId).HasColumnType("bigint(20)").ValueGeneratedNever();
			entity.Property(x => x.CellId).HasColumnType("bigint(20)");
			entity.Property(x => x.ZoneId).HasColumnType("bigint(20)");
			entity.Property(x => x.X).HasColumnType("int(11)");
			entity.Property(x => x.Y).HasColumnType("int(11)");
			entity.Property(x => x.Z).HasColumnType("int(11)");
			entity.Property(x => x.Warning).HasMaxLength(255).HasCharSet("utf8mb4").UseCollation("utf8mb4_general_ci");
		});
		modelBuilder.Entity<CellRoomAreaContractionLedger>(entity =>
		{
			entity.ToTable("CellRoomAreaContractionLedger");
			entity.HasKey(x => new { x.AreaId, x.RoomId });
			entity.Property(x => x.AreaId).HasColumnType("bigint(20)");
			entity.Property(x => x.RoomId).HasColumnType("bigint(20)");
			entity.Property(x => x.CellId).HasColumnType("bigint(20)");
		});
	}
}
