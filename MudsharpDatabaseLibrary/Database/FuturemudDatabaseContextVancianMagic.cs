using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

#nullable enable
namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<CharacterMagicCapabilityState> CharacterMagicCapabilityStates { get; set; } = null!;
	public virtual DbSet<VancianMagicOperation> VancianMagicOperations { get; set; } = null!;

	private static void ConfigureVancianMagic(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<MagicSpell>().Property(x => x.SpellLevel).HasDefaultValue(0);
		modelBuilder.Entity<MagicSpell>().Property(x => x.ScrollInscriptionAllowed).HasDefaultValue(false);
		modelBuilder.Entity<CharacterMagicCapabilityState>(entity =>
		{
			entity.HasKey(x => new { x.CharacterId, x.MagicCapabilityId });
			entity.Property(x => x.StateVersion).IsConcurrencyToken();
			entity.Property(x => x.Definition).HasColumnType("longtext").HasCharSet("utf8mb4");
			entity.HasOne(x => x.Character).WithMany().HasForeignKey(x => x.CharacterId).OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(x => x.MagicCapability).WithMany().HasForeignKey(x => x.MagicCapabilityId).OnDelete(DeleteBehavior.Cascade);
		});
		modelBuilder.Entity<VancianMagicOperation>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Id).ValueGeneratedNever();
			entity.Property(x => x.Kind).HasMaxLength(40);
			entity.Property(x => x.Status).HasMaxLength(40);
			entity.Property(x => x.Definition).HasColumnType("longtext").HasCharSet("utf8mb4");
			entity.Property(x => x.Diagnostic).HasColumnType("text").HasCharSet("utf8mb4");
			entity.HasIndex(x => new { x.CharacterId, x.MagicCapabilityId });
			entity.HasIndex(x => x.SourceItemId);
			entity.HasIndex(x => x.DestinationItemId);
		});
	}
}
