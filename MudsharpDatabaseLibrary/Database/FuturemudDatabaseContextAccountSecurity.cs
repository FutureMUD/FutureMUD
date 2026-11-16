#nullable enable
using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	private static void ConfigureAccountSecurity(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Account>(entity =>
		{
			entity.Property(x => x.CredentialVersion).HasDefaultValue(0L).IsConcurrencyToken();
			entity.Property(x => x.Password).IsConcurrencyToken();
			entity.Property(x => x.RecoveryCode).IsConcurrencyToken();
			entity.Property(x => x.RecoveryExpiresUtc).HasColumnType("datetime(6)");
			entity.Property(x => x.RecoveryIssuedUtc).HasColumnType("datetime(6)");
			entity.Property(x => x.LoginAttemptWindowUtc).HasColumnType("datetime(6)");
			entity.Property(x => x.LoginBlockedUntilUtc).HasColumnType("datetime(6)");
		});
	}
}
