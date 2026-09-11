using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

#nullable enable
namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<MagicalSubstance> MagicalSubstances { get; set; } = null!;
	private static void ConfigureMagicalSubstances(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<MagicalSubstance>(entity =>
		{
			entity.ToTable("MagicalSubstances");
			entity.HasKey(x => x.Id).HasName("PRIMARY");
			entity.Property(x => x.Id).HasColumnType("bigint(20)");
			entity.Property(x => x.Name).IsRequired().HasColumnType("varchar(200)").HasCharSet("utf8mb4").UseCollation("utf8mb4_unicode_ci");
			entity.Property(x => x.Definition).IsRequired().HasColumnType("longtext").HasCharSet("utf8mb4").UseCollation("utf8mb4_unicode_ci");
		});
	}
}
