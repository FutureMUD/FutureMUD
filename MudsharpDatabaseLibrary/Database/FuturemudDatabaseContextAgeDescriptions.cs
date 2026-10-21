#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	private static void ConfigureAgeDescriptions(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Race>(entity =>
		{
			entity.Property(x => x.AgeDescriptionProgId).HasColumnType("bigint(20)");
			entity.HasOne(x => x.AgeDescriptionProg).WithMany()
				.HasForeignKey(x => x.AgeDescriptionProgId)
				.HasConstraintName("FK_Races_AgeDescriptionProg")
				.OnDelete(DeleteBehavior.SetNull);
		});
	}
}
