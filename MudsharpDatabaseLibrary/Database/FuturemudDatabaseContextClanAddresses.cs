#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<RankFormOfAddress> RankFormsOfAddress { get; set; } = null!;
	public virtual DbSet<AppointmentFormOfAddress> AppointmentFormsOfAddress { get; set; } = null!;

	private static void ConfigureClanAddresses(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Clan>().Property(x => x.ShowFormsOfAddressInLook).HasDefaultValue(true);
		modelBuilder.Entity<Appointment>().Property(x => x.AddressPriority).HasDefaultValue(0);
		modelBuilder.Entity<RankFormOfAddress>(entity =>
		{
			entity.ToTable("RankFormsOfAddress");
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Id).ValueGeneratedNever();
			entity.Property(x => x.Text).HasMaxLength(200);
			entity.Property(x => x.TemplateKey).HasMaxLength(100);
			entity.HasOne(x => x.Rank).WithMany(x => x.FormsOfAddress).HasForeignKey(x => x.RankId).OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(x => x.FutureProg).WithMany().HasForeignKey(x => x.FutureProgId).OnDelete(DeleteBehavior.Restrict);
			entity.HasIndex(x => new { x.RankId, x.Order });
		});
		modelBuilder.Entity<AppointmentFormOfAddress>(entity =>
		{
			entity.ToTable("AppointmentFormsOfAddress");
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Id).ValueGeneratedNever();
			entity.Property(x => x.Text).HasMaxLength(200);
			entity.Property(x => x.TemplateKey).HasMaxLength(100);
			entity.HasOne(x => x.Appointment).WithMany(x => x.FormsOfAddress).HasForeignKey(x => x.AppointmentId).OnDelete(DeleteBehavior.Cascade);
			entity.HasOne(x => x.FutureProg).WithMany().HasForeignKey(x => x.FutureProgId).OnDelete(DeleteBehavior.Restrict);
			entity.HasIndex(x => new { x.AppointmentId, x.Order });
		});
	}
}
