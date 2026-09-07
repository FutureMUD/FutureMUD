using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

#nullable enable
namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<ChargenSkillSelectionGroup> ChargenSkillSelectionGroups { get; set; } = null!;
	public virtual DbSet<ChargenSkillSelectionGroupMember> ChargenSkillSelectionGroupMembers { get; set; } = null!;

	private static void ConfigureSkillSelectionGroups(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ChargenSkillSelectionGroup>(entity =>
		{
			entity.ToTable("ChargenSkillSelectionGroups");
			entity.HasKey(x => x.Id);
			entity.Property(x => x.StableKey).HasMaxLength(191);
			entity.HasIndex(x => x.StableKey).IsUnique();
			entity.Property(x => x.Name).HasMaxLength(200);
			entity.Property(x => x.Description).HasColumnType("text");
			entity.Property(x => x.SeedBaseline).HasColumnType("longtext");
			entity.HasOne(x => x.EligibilityProg).WithMany().HasForeignKey(x => x.EligibilityProgId).OnDelete(DeleteBehavior.Restrict);
			entity.HasOne(x => x.MemberEligibilityProg).WithMany().HasForeignKey(x => x.MemberEligibilityProgId).OnDelete(DeleteBehavior.Restrict);
		});
		modelBuilder.Entity<ChargenSkillSelectionGroupMember>(entity =>
		{
			entity.ToTable("ChargenSkillSelectionGroupMembers");
			entity.HasKey(x => new { x.GroupId, x.TraitDefinitionId });
			entity.HasOne(x => x.Group).WithMany(x => x.Members).HasForeignKey(x => x.GroupId).OnDelete(DeleteBehavior.Restrict);
			entity.HasOne(x => x.TraitDefinition).WithMany().HasForeignKey(x => x.TraitDefinitionId).OnDelete(DeleteBehavior.Restrict);
		});
	}
}
