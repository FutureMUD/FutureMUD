#nullable enable
using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<EmploymentTaskGrantRecord> EmploymentTaskGrants { get; set; } = null!;
	public virtual DbSet<EmploymentTaskGrantLimitRecord> EmploymentTaskGrantLimits { get; set; } = null!;
	public virtual DbSet<EmploymentTaskGrantScopeRecord> EmploymentTaskGrantScopes { get; set; } = null!;

	private static void ConfigureEmploymentTaskGrants(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EmploymentActiveTaskRecord>(entity =>
		{
			entity.Property(e => e.SourceRuleId).OptionalString("varchar(36)");
			entity.Property(e => e.CreatedByPrincipal).OptionalString("mediumtext");
			entity.Property(e => e.AuthorisedByPrincipal).OptionalString("mediumtext");
		});
		modelBuilder.Entity<EmploymentTaskGrantRecord>(entity =>
		{
			entity.ToTable("EmploymentTaskGrants");
			entity.HasKey(e => e.EmploymentActiveTaskId);
			entity.Property(e => e.EmploymentActiveTaskId).ValueGeneratedNever();
			entity.Property(e => e.PublicId).RequiredString("varchar(36)");
			entity.Property(e => e.TaskPublicId).RequiredString("varchar(36)");
			entity.Property(e => e.CorrelationId).RequiredString("varchar(36)");
			entity.Property(e => e.IssuerPrincipal).RequiredString("mediumtext");
			entity.Property(e => e.RevokedByPrincipal).OptionalString("mediumtext");
			entity.Property(e => e.RevocationReason).OptionalString("mediumtext");
			entity.HasIndex(e => e.PublicId).IsUnique();
			entity.HasOne(e => e.Task).WithOne(e => e.AuthorisationGrant)
				.HasForeignKey<EmploymentTaskGrantRecord>(e => e.EmploymentActiveTaskId).OnDelete(DeleteBehavior.Cascade);
		});
		modelBuilder.Entity<EmploymentTaskGrantLimitRecord>(entity =>
		{
			entity.ToTable("EmploymentTaskGrantLimits");
			entity.HasKey(e => new { e.EmploymentActiveTaskId, e.CurrencyId });
			entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");
			entity.HasOne(e => e.Grant).WithMany(e => e.Limits)
				.HasForeignKey(e => e.EmploymentActiveTaskId).OnDelete(DeleteBehavior.Cascade);
		});
		modelBuilder.Entity<EmploymentTaskGrantScopeRecord>(entity =>
		{
			entity.ToTable("EmploymentTaskGrantScopes");
			entity.HasKey(e => new { e.EmploymentActiveTaskId, e.Kind, e.Scope });
			entity.Property(e => e.Scope).RequiredString("varchar(255)");
			entity.HasOne(e => e.Grant).WithMany(e => e.Scopes)
				.HasForeignKey(e => e.EmploymentActiveTaskId).OnDelete(DeleteBehavior.Cascade);
		});
	}
}
