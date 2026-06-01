using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<MagicPortalNetwork> MagicPortalNetworks { get; set; }
	public virtual DbSet<MagicPortalEndpoint> MagicPortalEndpoints { get; set; }
	public virtual DbSet<MagicPortalLink> MagicPortalLinks { get; set; }

	private static void ConfigureMagicPortalTopology(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<MagicPortalNetwork>(entity =>
		{
			entity.ToTable("MagicPortalNetworks");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.MagicSchoolId).HasDatabaseName("FK_MagicPortalNetworks_MagicSchools_idx");
			entity.HasIndex(e => e.CreatedByCharacterId).HasDatabaseName("FK_MagicPortalNetworks_Characters_idx");
			entity.HasIndex(e => e.CreatedBySpellId).HasDatabaseName("FK_MagicPortalNetworks_MagicSpells_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.MagicSchoolId).HasColumnType("bigint(20)");
			entity.Property(e => e.IsActive).HasColumnType("bit(1)").HasDefaultValue(true);
			entity.Property(e => e.AllowCrossZone).HasColumnType("bit(1)");
			entity.Property(e => e.Verb).IsRequired().HasColumnType("varchar(100)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.OutboundKeyword).IsRequired().HasColumnType("varchar(100)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.InboundKeyword).IsRequired().HasColumnType("varchar(100)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.OutboundTarget).IsRequired().HasColumnType("varchar(500)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.InboundTarget).IsRequired().HasColumnType("varchar(500)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.OutboundDescription).IsRequired().HasColumnType("varchar(100)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.InboundDescription).IsRequired().HasColumnType("varchar(100)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.TimeMultiplier).HasColumnType("double").HasDefaultValue(1.0);
			entity.Property(e => e.CreatedByCharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.CreatedBySpellId).HasColumnType("bigint(20)");
			entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

			entity.HasOne(e => e.MagicSchool).WithMany().HasForeignKey(e => e.MagicSchoolId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_MagicPortalNetworks_MagicSchools");
			entity.HasOne(e => e.CreatedByCharacter).WithMany().HasForeignKey(e => e.CreatedByCharacterId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_MagicPortalNetworks_Characters");
			entity.HasOne(e => e.CreatedBySpell).WithMany().HasForeignKey(e => e.CreatedBySpellId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_MagicPortalNetworks_MagicSpells");
		});

		modelBuilder.Entity<MagicPortalEndpoint>(entity =>
		{
			entity.ToTable("MagicPortalEndpoints");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.MagicPortalNetworkId, e.Key }).IsUnique().HasDatabaseName("IX_MagicPortalEndpoints_Network_Key");
			entity.HasIndex(e => e.CellId).HasDatabaseName("FK_MagicPortalEndpoints_Cells_idx");
			entity.HasIndex(e => e.GameItemId).HasDatabaseName("FK_MagicPortalEndpoints_GameItems_idx");
			entity.HasIndex(e => e.CreatedByCharacterId).HasDatabaseName("FK_MagicPortalEndpoints_Characters_idx");
			entity.HasIndex(e => e.CreatedBySpellId).HasDatabaseName("FK_MagicPortalEndpoints_MagicSpells_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.MagicPortalNetworkId).HasColumnType("bigint(20)");
			entity.Property(e => e.Key).IsRequired().HasColumnType("varchar(100)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8").UseCollation("utf8_general_ci");
			entity.Property(e => e.AnchorType).HasColumnType("int(11)");
			entity.Property(e => e.CellId).HasColumnType("bigint(20)");
			entity.Property(e => e.GameItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.IsActive).HasColumnType("bit(1)").HasDefaultValue(true);
			entity.Property(e => e.CreatedByCharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.CreatedBySpellId).HasColumnType("bigint(20)");
			entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

			entity.HasOne(e => e.MagicPortalNetwork).WithMany(e => e.MagicPortalEndpoints).HasForeignKey(e => e.MagicPortalNetworkId).HasConstraintName("FK_MagicPortalEndpoints_MagicPortalNetworks");
			entity.HasOne(e => e.Cell).WithMany().HasForeignKey(e => e.CellId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_MagicPortalEndpoints_Cells");
			entity.HasOne(e => e.GameItem).WithMany().HasForeignKey(e => e.GameItemId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_MagicPortalEndpoints_GameItems");
			entity.HasOne(e => e.CreatedByCharacter).WithMany().HasForeignKey(e => e.CreatedByCharacterId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_MagicPortalEndpoints_Characters");
			entity.HasOne(e => e.CreatedBySpell).WithMany().HasForeignKey(e => e.CreatedBySpellId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_MagicPortalEndpoints_MagicSpells");
		});

		modelBuilder.Entity<MagicPortalLink>(entity =>
		{
			entity.ToTable("MagicPortalLinks");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.MagicPortalNetworkId, e.SourceEndpointId, e.DestinationEndpointId }).IsUnique().HasDatabaseName("IX_MagicPortalLinks_Network_Source_Destination");
			entity.HasIndex(e => e.SourceEndpointId).HasDatabaseName("FK_MagicPortalLinks_SourceEndpoints_idx");
			entity.HasIndex(e => e.DestinationEndpointId).HasDatabaseName("FK_MagicPortalLinks_DestinationEndpoints_idx");
			entity.HasIndex(e => e.CreatedByCharacterId).HasDatabaseName("FK_MagicPortalLinks_Characters_idx");
			entity.HasIndex(e => e.CreatedBySpellId).HasDatabaseName("FK_MagicPortalLinks_MagicSpells_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.MagicPortalNetworkId).HasColumnType("bigint(20)");
			entity.Property(e => e.SourceEndpointId).HasColumnType("bigint(20)");
			entity.Property(e => e.DestinationEndpointId).HasColumnType("bigint(20)");
			entity.Property(e => e.IsActive).HasColumnType("bit(1)").HasDefaultValue(true);
			entity.Property(e => e.CreatedByCharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.CreatedBySpellId).HasColumnType("bigint(20)");
			entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

			entity.HasOne(e => e.MagicPortalNetwork).WithMany(e => e.MagicPortalLinks).HasForeignKey(e => e.MagicPortalNetworkId).HasConstraintName("FK_MagicPortalLinks_MagicPortalNetworks");
			entity.HasOne(e => e.SourceEndpoint).WithMany(e => e.MagicPortalLinksAsSource).HasForeignKey(e => e.SourceEndpointId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_MagicPortalLinks_SourceEndpoints");
			entity.HasOne(e => e.DestinationEndpoint).WithMany(e => e.MagicPortalLinksAsDestination).HasForeignKey(e => e.DestinationEndpointId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_MagicPortalLinks_DestinationEndpoints");
			entity.HasOne(e => e.CreatedByCharacter).WithMany().HasForeignKey(e => e.CreatedByCharacterId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_MagicPortalLinks_Characters");
			entity.HasOne(e => e.CreatedBySpell).WithMany().HasForeignKey(e => e.CreatedBySpellId).OnDelete(DeleteBehavior.SetNull).HasConstraintName("FK_MagicPortalLinks_MagicSpells");
		});
	}
}
