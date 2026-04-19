#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<ComputerMailDomain> ComputerMailDomains { get; set; }
	public virtual DbSet<ComputerMailAccount> ComputerMailAccounts { get; set; }
	public virtual DbSet<ComputerMailMessage> ComputerMailMessages { get; set; }
	public virtual DbSet<ComputerMailMailboxEntry> ComputerMailMailboxEntries { get; set; }

	private static void ConfigureComputerMail(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ComputerMailDomain>(entity =>
		{
			entity.ToTable("ComputerMailDomains");

			entity.HasKey(e => e.Id)
			      .HasName("PRIMARY");

			entity.HasIndex(e => e.DomainName)
			      .IsUnique()
			      .HasDatabaseName("IX_ComputerMailDomains_DomainName");

			entity.HasIndex(e => e.HostItemId)
			      .HasDatabaseName("FK_ComputerMailDomains_GameItems_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.HostItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.Enabled).HasColumnType("bit(1)");
			entity.Property(e => e.CreatedAtUtc).HasColumnType("datetime");
			entity.Property(e => e.DomainName)
			      .IsRequired()
			      .HasColumnType("varchar(255)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");

			entity.HasOne(d => d.HostItem)
			      .WithMany()
			      .HasForeignKey(d => d.HostItemId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_ComputerMailDomains_GameItems");
		});

		modelBuilder.Entity<ComputerMailAccount>(entity =>
		{
			entity.ToTable("ComputerMailAccounts");

			entity.HasKey(e => e.Id)
			      .HasName("PRIMARY");

			entity.HasIndex(e => e.ComputerMailDomainId)
			      .HasDatabaseName("FK_ComputerMailAccounts_ComputerMailDomains_idx");

			entity.HasIndex(e => new { e.ComputerMailDomainId, e.UserName })
			      .IsUnique()
			      .HasDatabaseName("IX_ComputerMailAccounts_Domain_UserName");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ComputerMailDomainId).HasColumnType("bigint(20)");
			entity.Property(e => e.PasswordSalt).HasColumnType("bigint(20)");
			entity.Property(e => e.IsEnabled).HasColumnType("bit(1)");
			entity.Property(e => e.CreatedAtUtc).HasColumnType("datetime");
			entity.Property(e => e.LastModifiedAtUtc).HasColumnType("datetime");
			entity.Property(e => e.UserName)
			      .IsRequired()
			      .HasColumnType("varchar(255)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.PasswordHash)
			      .IsRequired()
			      .HasColumnType("text")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");

			entity.HasOne(d => d.ComputerMailDomain)
			      .WithMany(p => p.Accounts)
			      .HasForeignKey(d => d.ComputerMailDomainId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_ComputerMailAccounts_ComputerMailDomains");
		});

		modelBuilder.Entity<ComputerMailMessage>(entity =>
		{
			entity.ToTable("ComputerMailMessages");

			entity.HasKey(e => e.Id)
			      .HasName("PRIMARY");

			entity.HasIndex(e => e.SentAtUtc)
			      .HasDatabaseName("IX_ComputerMailMessages_SentAtUtc");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.SentAtUtc).HasColumnType("datetime");
			entity.Property(e => e.SenderAddress)
			      .IsRequired()
			      .HasColumnType("varchar(255)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.RecipientAddress)
			      .IsRequired()
			      .HasColumnType("varchar(255)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.Subject)
			      .IsRequired()
			      .HasColumnType("varchar(500)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.Body)
			      .IsRequired()
			      .HasColumnType("longtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
		});

		modelBuilder.Entity<ComputerMailMailboxEntry>(entity =>
		{
			entity.ToTable("ComputerMailMailboxEntries");

			entity.HasKey(e => e.Id)
			      .HasName("PRIMARY");

			entity.HasIndex(e => e.ComputerMailAccountId)
			      .HasDatabaseName("FK_ComputerMailMailboxEntries_ComputerMailAccounts_idx");

			entity.HasIndex(e => e.ComputerMailMessageId)
			      .HasDatabaseName("FK_ComputerMailMailboxEntries_ComputerMailMessages_idx");

			entity.HasIndex(e => new { e.ComputerMailAccountId, e.IsDeleted, e.IsSentFolder, e.DeliveredAtUtc })
			      .HasDatabaseName("IX_ComputerMailMailboxEntries_Account_Folder_Delivered");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ComputerMailAccountId).HasColumnType("bigint(20)");
			entity.Property(e => e.ComputerMailMessageId).HasColumnType("bigint(20)");
			entity.Property(e => e.IsSentFolder).HasColumnType("bit(1)");
			entity.Property(e => e.IsRead).HasColumnType("bit(1)");
			entity.Property(e => e.IsDeleted).HasColumnType("bit(1)");
			entity.Property(e => e.DeliveredAtUtc).HasColumnType("datetime");

			entity.HasOne(d => d.ComputerMailAccount)
			      .WithMany(p => p.MailboxEntries)
			      .HasForeignKey(d => d.ComputerMailAccountId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_ComputerMailMailboxEntries_ComputerMailAccounts");

			entity.HasOne(d => d.ComputerMailMessage)
			      .WithMany(p => p.MailboxEntries)
			      .HasForeignKey(d => d.ComputerMailMessageId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_ComputerMailMailboxEntries_ComputerMailMessages");
		});
	}
}
