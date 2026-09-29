using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

#nullable enable
namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<CharacterAcquiredSpell> CharacterAcquiredSpells { get; set; } = null!;
	public virtual DbSet<CharacterMagicSkillOpportunity> CharacterMagicSkillOpportunities { get; set; } = null!;
	public virtual DbSet<CharacterCastingEnrolment> CharacterCastingEnrolments { get; set; } = null!;
	public virtual DbSet<MagicCastingOperation> MagicCastingOperations { get; set; } = null!;

	private static void ConfigureMagicCasting(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<CharacterAcquiredSpell>(entity =>
		{
			entity.HasKey(x => new { x.CharacterId, x.MagicSpellId });
			entity.Property(x => x.StateVersion).IsConcurrencyToken();
			entity.Property(x => x.Provenance).HasColumnType("text").HasCharSet("utf8mb4");
			entity.HasOne(x => x.Character).WithMany().HasForeignKey(x => x.CharacterId).OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(x => x.MagicSpell).WithMany().HasForeignKey(x => x.MagicSpellId).OnDelete(DeleteBehavior.Restrict);
		});
		modelBuilder.Entity<CharacterMagicSkillOpportunity>(entity =>
		{
			entity.HasKey(x => new { x.CharacterId, x.TraitDefinitionId });
			entity.Property(x => x.StateVersion).IsConcurrencyToken();
			entity.HasOne(x => x.Character).WithMany().HasForeignKey(x => x.CharacterId).OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(x => x.TraitDefinition).WithMany().HasForeignKey(x => x.TraitDefinitionId).OnDelete(DeleteBehavior.Restrict);
		});
		modelBuilder.Entity<CharacterCastingEnrolment>(entity =>
		{
			entity.HasKey(x => new { x.CharacterId, x.CapabilityIdentity });
			entity.HasOne(x => x.Character).WithMany().HasForeignKey(x => x.CharacterId).OnDelete(DeleteBehavior.Cascade);
		});
		modelBuilder.Entity<MagicCastingOperation>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Id).ValueGeneratedNever();
			entity.Property(x => x.Stage).HasMaxLength(40);
			entity.Property(x => x.Definition).HasColumnType("longtext").HasCharSet("utf8mb4");
			entity.Property(x => x.Diagnostic).HasColumnType("text").HasCharSet("utf8mb4");
			entity.HasIndex(x => new { x.CharacterId, x.Stage });
		});
	}
}
