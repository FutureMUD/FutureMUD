using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	private static void ConfigureWritingCollections(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<WritingCollection>(entity =>
		{
			entity.ToTable("WritingCollections");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.Name)
			      .IsUnique()
			      .HasDatabaseName("IX_WritingCollections_Name");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.Name)
			      .IsRequired()
			      .HasColumnType("varchar(200)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.Description)
			      .IsRequired()
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.DefaultTitle)
			      .IsRequired()
			      .HasColumnType("varchar(500)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
		});

		modelBuilder.Entity<WritingCollectionEntry>(entity =>
		{
			entity.ToTable("WritingCollectionEntries");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.WritingCollectionId)
			      .HasDatabaseName("FK_WritingCollectionEntries_Collections_idx");
			entity.HasIndex(e => new { e.WritingCollectionId, e.PageNumber, e.DisplayOrder })
			      .HasDatabaseName("IX_WritingCollectionEntries_Page_Order");
			entity.HasIndex(e => e.WritingId)
			      .HasDatabaseName("FK_WritingCollectionEntries_Writings_idx");
			entity.HasIndex(e => e.DrawingId)
			      .HasDatabaseName("FK_WritingCollectionEntries_Drawings_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.WritingCollectionId).HasColumnType("bigint(20)");
			entity.Property(e => e.PageNumber).HasColumnType("int(11)");
			entity.Property(e => e.DisplayOrder).HasColumnType("int(11)");
			entity.Property(e => e.WritingId).HasColumnType("bigint(20)");
			entity.Property(e => e.DrawingId).HasColumnType("bigint(20)");

			entity.HasOne(e => e.WritingCollection)
			      .WithMany(e => e.WritingCollectionEntries)
			      .HasForeignKey(e => e.WritingCollectionId)
			      .HasConstraintName("FK_WritingCollectionEntries_Collections")
			      .OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(e => e.Writing)
			      .WithMany()
			      .HasForeignKey(e => e.WritingId)
			      .HasConstraintName("FK_WritingCollectionEntries_Writings")
			      .OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(e => e.Drawing)
			      .WithMany()
			      .HasForeignKey(e => e.DrawingId)
			      .HasConstraintName("FK_WritingCollectionEntries_Drawings")
			      .OnDelete(DeleteBehavior.Cascade);
		});
	}
}