#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<DiscordAccountLink> DiscordAccountLinks { get; set; } = null!;
	public virtual DbSet<DiscordHook> DiscordHooks { get; set; } = null!;
	public virtual DbSet<DiscordRemoteInvocation> DiscordRemoteInvocations { get; set; } = null!;
	public virtual DbSet<DiscordIntegrationSetting> DiscordIntegrationSettings { get; set; } = null!;

	private static void ConfigureDiscordIntegration(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<DiscordAccountLink>(entity =>
		{
			entity.ToTable("DiscordAccountLinks");
			entity.HasKey(x => x.DiscordUserId);
			entity.Property(x => x.DiscordUserId).HasMaxLength(20).UseCollation("ascii_bin").HasCharSet("ascii");
			entity.HasIndex(x => x.AccountId).IsUnique();
			entity.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Cascade);
		});
		modelBuilder.Entity<DiscordHook>(entity =>
		{
			entity.ToTable("DiscordHooks");
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Key).HasMaxLength(100);
			entity.HasIndex(x => x.Key).IsUnique();
			entity.Property(x => x.Category).HasMaxLength(100);
			entity.Property(x => x.Description).HasColumnType("text");
			entity.Property(x => x.Usage).HasMaxLength(1000);
			entity.Property(x => x.AllowedGuildIds).HasColumnType("text");
			entity.Property(x => x.AllowedChannelIds).HasColumnType("text");
			entity.Property(x => x.AllowedAccountIds).HasColumnType("text");
			entity.HasOne(x => x.Prog).WithMany().HasForeignKey(x => x.ProgId).OnDelete(DeleteBehavior.Restrict);
		});
		modelBuilder.Entity<DiscordRemoteInvocation>(entity =>
		{
			entity.ToTable("DiscordRemoteInvocations");
			entity.HasKey(x => x.Id);
			entity.Property(x => x.Id).HasMaxLength(36).UseCollation("ascii_bin").HasCharSet("ascii");
			entity.Property(x => x.DiscordUserId).HasMaxLength(20).UseCollation("ascii_bin").HasCharSet("ascii");
			entity.Property(x => x.Operation).HasMaxLength(80);
			entity.Property(x => x.Arguments).HasColumnType("text");
			entity.Property(x => x.Fingerprint).HasMaxLength(64);
			entity.Property(x => x.DefinitionFingerprint).HasMaxLength(64);
			entity.Property(x => x.Status).HasMaxLength(32);
			entity.Property(x => x.ResultText).HasColumnType("mediumtext");
			entity.Property(x => x.ConfirmationId).HasMaxLength(36);
			entity.HasIndex(x => new { x.DiscordUserId, x.CreatedUtc });
			entity.HasIndex(x => new { x.HookId, x.CreatedUtc });
			entity.HasIndex(x => x.CreatedUtc);
		});
		modelBuilder.Entity<DiscordIntegrationSetting>(entity =>
		{
			entity.ToTable("DiscordIntegrationSettings");
			entity.HasKey(x => x.Key);
			entity.Property(x => x.Key).HasMaxLength(100);
			entity.Property(x => x.Value).HasColumnType("text");
		});
	}
}
