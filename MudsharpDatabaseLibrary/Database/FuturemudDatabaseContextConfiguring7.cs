#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	private static void OnModelCreatingSeven(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<TrapTemplate>(entity =>
		{
			entity.HasKey(e => new { e.Id, e.RevisionNumber })
				.HasName("PRIMARY");

			entity.HasIndex(e => e.EditableItemId)
				.HasDatabaseName("FK_TrapTemplates_EditableItems_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RevisionNumber).HasColumnType("int(11)");
			entity.Property(e => e.EditableItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name)
				.IsRequired()
				.HasColumnType("varchar(200)")
				.HasCharSet("utf8mb4")
				.UseCollation("utf8mb4_unicode_ci");
			entity.Property(e => e.Definition)
				.IsRequired()
				.HasColumnType("mediumtext")
				.HasCharSet("utf8mb4")
				.UseCollation("utf8mb4_unicode_ci");

			entity.HasOne(d => d.EditableItem)
				.WithMany(p => p.TrapTemplates)
				.HasForeignKey(d => d.EditableItemId)
				.HasConstraintName("FK_TrapTemplates_EditableItems");
		});
	}
}
