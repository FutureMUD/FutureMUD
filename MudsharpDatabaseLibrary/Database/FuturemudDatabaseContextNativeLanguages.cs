using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

#nullable enable
namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	private static void ConfigureNativeLanguages(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Character>().HasOne(x => x.NativeLanguage).WithMany()
			.HasForeignKey(x => x.NativeLanguageId).OnDelete(DeleteBehavior.SetNull);
		modelBuilder.Entity<Culture>().HasOne(x => x.NativeLanguage).WithMany()
			.HasForeignKey(x => x.NativeLanguageId).OnDelete(DeleteBehavior.SetNull);
		modelBuilder.Entity<Ethnicity>().HasOne(x => x.NativeLanguage).WithMany()
			.HasForeignKey(x => x.NativeLanguageId).OnDelete(DeleteBehavior.SetNull);
		modelBuilder.Entity<CharactersLanguages>().HasOne(x => x.AcquisitionAccent).WithMany()
			.HasForeignKey(x => x.AcquisitionAccentId).OnDelete(DeleteBehavior.SetNull);
		modelBuilder.Entity<Accent>().Property(x => x.Role).HasDefaultValue(0);
		modelBuilder.Entity<Accent>().HasMany(x => x.AssociatedLanguages).WithMany()
			.UsingEntity("AccentsAssociatedLanguages",
				right => right.HasOne(typeof(Language)).WithMany().HasForeignKey("LanguageId").OnDelete(DeleteBehavior.Cascade),
				left => left.HasOne(typeof(Accent)).WithMany().HasForeignKey("AccentId").OnDelete(DeleteBehavior.Cascade),
				join => join.HasKey("AccentId", "LanguageId"));
	}
}
