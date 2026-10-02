#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<MagicSpellLifecycle> MagicSpellLifecycles { get; set; } = null!;
	public virtual DbSet<MagicSpellOwnedEntity> MagicSpellOwnedEntities { get; set; } = null!;

	private static void ConfigureSpellLifecycles(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<MagicSpellLifecycle>(entity =>
		{
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Id).ValueGeneratedNever();
			entity.Property(x => x.Family).HasMaxLength(128).HasCharSet("utf8mb4");
			entity.Property(x => x.Provenance).HasMaxLength(2048).HasCharSet("utf8mb4");
			entity.Property(x => x.Diagnostic).HasMaxLength(2048).HasCharSet("utf8mb4");
			entity.Property(x => x.Version).IsConcurrencyToken();
			entity.HasIndex(x => new { x.State, x.DeadlineUtc });
			entity.HasIndex(x => new { x.CreatorId, x.Family, x.State });
		});
		modelBuilder.Entity<MagicSpellOwnedEntity>(entity =>
		{
			entity.HasKey(x => new { x.Kind, x.EntityId });
			entity.Property(x => x.EntityId).ValueGeneratedNever();
			entity.HasOne(x => x.Lifecycle).WithMany(x => x.Entities)
				.HasForeignKey(x => x.LifecycleId).OnDelete(DeleteBehavior.Restrict);
		});
	}
}
