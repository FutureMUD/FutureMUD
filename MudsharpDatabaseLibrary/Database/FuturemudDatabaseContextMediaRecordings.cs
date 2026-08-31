#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<MediaRecording> MediaRecordings { get; set; }
	public virtual DbSet<MediaRecordingChunk> MediaRecordingChunks { get; set; }
	public virtual DbSet<MediaSceneSnapshot> MediaSceneSnapshots { get; set; }
	public virtual DbSet<MediaRecordingFrame> MediaRecordingFrames { get; set; }
	public virtual DbSet<MediaRecordingReference> MediaRecordingReferences { get; set; }

	private static void ConfigureMediaRecordings(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<MediaRecording>(entity =>
		{
			entity.ToTable("MediaRecordings");
			entity.HasKey(x => x.Id).HasName("PRIMARY");
			entity.Property(x => x.Id).HasColumnType("bigint(20)").ValueGeneratedOnAdd();
			entity.Property(x => x.SchemaVersion).HasColumnType("int(11)").HasDefaultValue(1);
			entity.Property(x => x.Capabilities).HasColumnType("int(11)");
			entity.Property(x => x.Status).HasColumnType("int(11)");
			entity.Property(x => x.Name).IsRequired().HasMaxLength(255).HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(x => x.CreatedAtUtc).HasColumnType("datetime");
			entity.Property(x => x.FinalisedAtUtc).HasColumnType("datetime");
			entity.Property(x => x.DurationMilliseconds).HasColumnType("bigint(20)");
			entity.Property(x => x.LogicalSizeInBytes).HasColumnType("bigint(20)");
			entity.HasIndex(x => new { x.Status, x.CreatedAtUtc }).HasDatabaseName("IX_MediaRecordings_Status_Created");
		});

		modelBuilder.Entity<MediaRecordingChunk>(entity =>
		{
			entity.ToTable("MediaRecordingChunks");
			entity.HasKey(x => x.Id).HasName("PRIMARY");
			entity.Property(x => x.Id).HasColumnType("bigint(20)").ValueGeneratedOnAdd();
			entity.Property(x => x.MediaRecordingId).HasColumnType("bigint(20)");
			entity.Property(x => x.Sequence).HasColumnType("int(11)");
			entity.Property(x => x.OffsetMilliseconds).HasColumnType("bigint(20)");
			entity.Property(x => x.DurationMilliseconds).HasColumnType("bigint(20)");
			entity.Property(x => x.UncompressedSizeBytes).HasColumnType("int(11)");
			entity.Property(x => x.Payload).IsRequired().HasColumnType("longblob");
			entity.Property(x => x.CreatedAtUtc).HasColumnType("datetime");
			entity.HasIndex(x => new { x.MediaRecordingId, x.Sequence }).IsUnique()
				.HasDatabaseName("UX_MediaRecordingChunks_Recording_Sequence");
			entity.HasOne(x => x.MediaRecording).WithMany(x => x.Chunks).HasForeignKey(x => x.MediaRecordingId)
				.OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_MediaRecordingChunks_MediaRecordings");
		});

		modelBuilder.Entity<MediaSceneSnapshot>(entity =>
		{
			entity.ToTable("MediaSceneSnapshots");
			entity.HasKey(x => x.Id).HasName("PRIMARY");
			entity.Property(x => x.Id).HasColumnType("bigint(20)").ValueGeneratedOnAdd();
			entity.Property(x => x.ContentHash).IsRequired().HasMaxLength(64).IsFixedLength().HasCharSet("ascii")
				.UseCollation("ascii_general_ci");
			entity.Property(x => x.UncompressedSizeBytes).HasColumnType("int(11)");
			entity.Property(x => x.Payload).IsRequired().HasColumnType("longblob");
			entity.Property(x => x.CreatedAtUtc).HasColumnType("datetime");
			entity.HasIndex(x => x.ContentHash).IsUnique().HasDatabaseName("UX_MediaSceneSnapshots_ContentHash");
		});

		modelBuilder.Entity<MediaRecordingFrame>(entity =>
		{
			entity.ToTable("MediaRecordingFrames");
			entity.HasKey(x => x.Id).HasName("PRIMARY");
			entity.Property(x => x.Id).HasColumnType("bigint(20)").ValueGeneratedOnAdd();
			entity.Property(x => x.MediaRecordingId).HasColumnType("bigint(20)");
			entity.Property(x => x.MediaSceneSnapshotId).HasColumnType("bigint(20)");
			entity.Property(x => x.StartOffsetMilliseconds).HasColumnType("bigint(20)");
			entity.Property(x => x.EndOffsetMilliseconds).HasColumnType("bigint(20)");
			entity.HasIndex(x => new { x.MediaRecordingId, x.StartOffsetMilliseconds })
				.HasDatabaseName("IX_MediaRecordingFrames_Recording_Offset");
			entity.HasIndex(x => x.MediaSceneSnapshotId).HasDatabaseName("FK_MediaRecordingFrames_MediaSceneSnapshots_idx");
			entity.HasOne(x => x.MediaRecording).WithMany(x => x.Frames).HasForeignKey(x => x.MediaRecordingId)
				.OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_MediaRecordingFrames_MediaRecordings");
			entity.HasOne(x => x.MediaSceneSnapshot).WithMany(x => x.Frames).HasForeignKey(x => x.MediaSceneSnapshotId)
				.OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_MediaRecordingFrames_MediaSceneSnapshots");
		});

		modelBuilder.Entity<MediaRecordingReference>(entity =>
		{
			entity.ToTable("MediaRecordingReferences");
			entity.HasKey(x => x.Id).HasName("PRIMARY");
			entity.Property(x => x.Id).HasColumnType("bigint(20)").ValueGeneratedOnAdd();
			entity.Property(x => x.GameItemComponentId).HasColumnType("bigint(20)");
			entity.Property(x => x.MediaRecordingId).HasColumnType("bigint(20)");
			entity.Property(x => x.Name).IsRequired().HasMaxLength(255).HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(x => x.PubliclyAccessible).HasColumnType("bit(1)");
			entity.Property(x => x.CreatedAtUtc).HasColumnType("datetime");
			entity.Property(x => x.LastModifiedAtUtc).HasColumnType("datetime");
			entity.HasIndex(x => new { x.GameItemComponentId, x.Name }).IsUnique()
				.HasDatabaseName("UX_MediaRecordingReferences_Component_Name");
			entity.HasIndex(x => x.MediaRecordingId).HasDatabaseName("FK_MediaRecordingReferences_MediaRecordings_idx");
			entity.HasOne(x => x.GameItemComponent).WithMany().HasForeignKey(x => x.GameItemComponentId)
				.OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_MediaRecordingReferences_GameItemComponents");
			entity.HasOne(x => x.MediaRecording).WithMany(x => x.References).HasForeignKey(x => x.MediaRecordingId)
				.OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_MediaRecordingReferences_MediaRecordings");
		});
	}
}
