#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<CharacterArchive> CharacterArchives { get; set; } = null!;

	private static void ConfigureCharacterArchives(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Models.Character>(entity =>
		{
			entity.Property(x => x.IsArchived).HasDefaultValue(false).IsConcurrencyToken();
			entity.ToTable("Characters", table => table.HasCheckConstraint("CK_Characters_BodyOrArchive",
				"`BodyId` IS NOT NULL OR `IsArchived` = 1"));
			entity.HasOne(x => x.Body).WithMany(x => x.Characters).HasForeignKey(x => x.BodyId)
				.OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Characters_Bodies");
		});
		modelBuilder.Entity<CharacterArchive>(entity =>
		{
			entity.ToTable("CharacterArchives");
			entity.HasKey(x => x.CharacterId);
			entity.Property(x => x.CharacterId).ValueGeneratedNever();
			entity.Property(x => x.DisplayName).HasMaxLength(1024);
			entity.Property(x => x.ShortDescription).HasMaxLength(4096);
			entity.Property(x => x.FullDescription).HasColumnType("longtext").HasMaxLength(65535);
			entity.Property(x => x.WoundHistory).HasColumnType("longtext").HasMaxLength(65535);
			entity.Property(x => x.Provenance).HasMaxLength(2048);
			entity.HasIndex(x => x.LifecycleId).IsUnique();
			entity.HasOne<Models.Character>().WithOne().HasForeignKey<CharacterArchive>(x => x.CharacterId)
				.OnDelete(DeleteBehavior.Restrict);
		});
	}
}
