using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	protected static void ConfigureCharacterInstances(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<CharacterInstance>(entity =>
		{
			entity.HasKey(e => e.Id)
			      .HasName("PRIMARY");

			entity.HasIndex(e => e.AnchorInstanceId)
			      .HasDatabaseName("FK_CharacterInstances_AnchorInstance_idx");

			entity.HasIndex(e => e.BodyId)
			      .HasDatabaseName("FK_CharacterInstances_Bodies_idx");

			entity.HasIndex(e => e.CharacterId)
			      .HasDatabaseName("FK_CharacterInstances_Characters_idx");

			entity.HasIndex(e => e.EmbodiedBodyId)
			      .IsUnique()
			      .HasDatabaseName("UQ_CharacterInstances_EmbodiedBody");

			entity.HasIndex(e => e.PrimaryCharacterId)
			      .IsUnique()
			      .HasDatabaseName("UQ_CharacterInstances_PrimaryCharacter");

			entity.HasIndex(e => e.LocationId)
			      .HasDatabaseName("FK_CharacterInstances_Cells_idx");

			entity.HasIndex(e => new { e.LocationId, e.RoomLayer })
			      .HasDatabaseName("IX_CharacterInstances_Location_Layer");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.AnchorInstanceId).HasColumnType("bigint(20)");
			entity.Property(e => e.BodyId).HasColumnType("bigint(20)");
			entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.ControlPolicy).HasColumnType("int(11)");
			entity.Property(e => e.CreatedBySourceId).HasColumnType("bigint(20)");
			entity.Property(e => e.CreatedBySourceType).HasColumnType("int(11)");
			entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
			entity.Property(e => e.DeathPolicy).HasColumnType("int(11)");
			entity.Property(e => e.EmbodiedBodyId)
			      .HasColumnType("bigint(20)")
			      .HasComputedColumnSql("CASE WHEN `IsEmbodied` = b'1' THEN `BodyId` ELSE NULL END", stored: true);
			entity.Property(e => e.ExpiryDateTime).HasColumnType("datetime");
			entity.Property(e => e.InstanceKind).HasColumnType("int(11)");
			entity.Property(e => e.LocationId).HasColumnType("bigint(20)");
			entity.Property(e => e.PerceptionPolicy).HasColumnType("int(11)");
			entity.Property(e => e.PersistencePolicy).HasColumnType("int(11)");
			entity.Property(e => e.PositionId).HasColumnType("int(11)");
			entity.Property(e => e.PositionModifier).HasColumnType("int(11)");
			entity.Property(e => e.PositionTargetId).HasColumnType("bigint(20)");
			entity.Property(e => e.PrimaryCharacterId)
			      .HasColumnType("bigint(20)")
			      .HasComputedColumnSql("CASE WHEN `IsPrimary` = b'1' THEN `CharacterId` ELSE NULL END", stored: true);
			entity.Property(e => e.RoomLayer).HasColumnType("int(11)");
			entity.Property(e => e.State).HasColumnType("int(11)");
			entity.Property(e => e.Status).HasColumnType("int(11)");

			entity.Property(e => e.CreatedBySourceKey)
			      .HasColumnType("varchar(200)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");

			entity.Property(e => e.EffectData)
			      .IsRequired()
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");

			entity.Property(e => e.InstanceName)
			      .HasColumnType("varchar(100)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");

			entity.Property(e => e.IsControllable)
			      .HasColumnType("bit(1)")
			      .HasDefaultValueSql("b'1'");

			entity.Property(e => e.IsEmbodied)
			      .HasColumnType("bit(1)")
			      .HasDefaultValueSql("b'1'");

			entity.Property(e => e.IsPrimary)
			      .HasColumnType("bit(1)")
			      .HasDefaultValueSql("b'0'");

			entity.Property(e => e.PositionEmote)
			      .HasColumnType("text")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");

			entity.Property(e => e.PositionTargetType)
			      .HasColumnType("varchar(50)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");

			entity.HasOne(d => d.AnchorInstance)
			      .WithMany(p => p.AnchoredInstances)
			      .HasForeignKey(d => d.AnchorInstanceId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_CharacterInstances_AnchorInstance");

			entity.HasOne(d => d.Body)
			      .WithMany(p => p.CharacterInstances)
			      .HasForeignKey(d => d.BodyId)
			      .OnDelete(DeleteBehavior.ClientSetNull)
			      .HasConstraintName("FK_CharacterInstances_Bodies");

			entity.HasOne(d => d.Character)
			      .WithMany(p => p.CharacterInstances)
			      .HasForeignKey(d => d.CharacterId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_CharacterInstances_Characters");

			entity.HasOne(d => d.Location)
			      .WithMany(p => p.CharacterInstances)
			      .HasForeignKey(d => d.LocationId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_CharacterInstances_Cells");
		});
	}
}
