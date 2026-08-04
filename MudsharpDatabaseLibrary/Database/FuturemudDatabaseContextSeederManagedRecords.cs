using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	private static void ConfigureSeederManagedRecords(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<SeederManagedRecord>(entity =>
		{
			entity.ToTable("SeederManagedRecords");
			entity.HasKey(x => x.Id);
			entity.HasIndex(x => new { x.Seeder, x.EntityType, x.StableKey })
				.IsUnique()
				.HasDatabaseName("UX_SeederManagedRecords_Identity");
			entity.Property(x => x.Id).HasColumnType("bigint(20)");
			entity.Property(x => x.Seeder).IsRequired().HasMaxLength(100);
			entity.Property(x => x.Module).IsRequired().HasMaxLength(100);
			entity.Property(x => x.EntityType).IsRequired().HasMaxLength(100);
			entity.Property(x => x.StableKey).IsRequired().HasMaxLength(500);
			entity.Property(x => x.LogicalId).HasColumnType("bigint(20)");
			entity.Property(x => x.RevisionNumber).HasColumnType("int(11)");
			entity.Property(x => x.AppliedFingerprint).IsRequired().HasMaxLength(64);
			entity.Property(x => x.ManifestVersion).IsRequired().HasMaxLength(50);
			entity.Property(x => x.AppliedAt).HasColumnType("datetime");
			entity.Property(x => x.Retired).HasColumnType("bit(1)");
		});
	}
}
