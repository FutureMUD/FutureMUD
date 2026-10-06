#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<CharacterBodyRetirement> CharacterBodyRetirements { get; set; } = null!;

	private static void ConfigureBodyRetirements(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<CharacterBodyRetirement>(entity =>
		{
			entity.HasKey(x => x.BodyId);
			entity.Property(x => x.BodyId).ValueGeneratedNever();
			entity.HasOne(x => x.Body).WithMany().HasForeignKey(x => x.BodyId).OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(x => x.Character).WithMany().HasForeignKey(x => x.CharacterId).OnDelete(DeleteBehavior.Restrict);
		});
	}
}
