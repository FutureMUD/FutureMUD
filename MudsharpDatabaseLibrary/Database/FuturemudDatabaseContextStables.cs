using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	private static void ConfigureStables(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Stable>(entity =>
		{
			entity.ToTable("Stables");
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.HasIndex(e => e.EconomicZoneId).HasDatabaseName("FK_Stables_EconomicZones_idx");
			entity.HasIndex(e => e.CellId).HasDatabaseName("FK_Stables_Cells_idx");
			entity.HasIndex(e => e.BankAccountId).HasDatabaseName("FK_Stables_BankAccounts_idx");
			entity.HasIndex(e => e.LodgeFeeProgId).HasDatabaseName("FK_Stables_FutureProgs_Lodge_idx");
			entity.HasIndex(e => e.DailyFeeProgId).HasDatabaseName("FK_Stables_FutureProgs_Daily_idx");
			entity.HasIndex(e => e.CanStableProgId).HasDatabaseName("FK_Stables_FutureProgs_Can_idx");
			entity.HasIndex(e => e.WhyCannotStableProgId).HasDatabaseName("FK_Stables_FutureProgs_Why_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.Name)
			      .IsRequired()
			      .HasColumnType("varchar(200)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");
			entity.Property(e => e.CellId).HasColumnType("bigint(20)");
			entity.Property(e => e.BankAccountId).HasColumnType("bigint(20)");
			entity.Property(e => e.IsTrading).HasColumnType("bit(1)").HasDefaultValue(true);
			entity.Property(e => e.LodgeFee).HasColumnType("decimal(58,29)");
			entity.Property(e => e.DailyFee).HasColumnType("decimal(58,29)");
			entity.Property(e => e.LodgeFeeProgId).HasColumnType("bigint(20)");
			entity.Property(e => e.DailyFeeProgId).HasColumnType("bigint(20)");
			entity.Property(e => e.CanStableProgId).HasColumnType("bigint(20)");
			entity.Property(e => e.WhyCannotStableProgId).HasColumnType("bigint(20)");
			entity.Property(e => e.EmployeeRecords)
			      .IsRequired()
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");

			entity.HasOne(d => d.EconomicZone)
			      .WithMany()
			      .HasForeignKey(d => d.EconomicZoneId)
			      .HasConstraintName("FK_Stables_EconomicZones");

			entity.HasOne(d => d.Cell)
			      .WithMany()
			      .HasForeignKey(d => d.CellId)
			      .HasConstraintName("FK_Stables_Cells");

			entity.HasOne(d => d.BankAccount)
			      .WithMany()
			      .HasForeignKey(d => d.BankAccountId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_Stables_BankAccounts");

			entity.HasOne(d => d.LodgeFeeProg)
			      .WithMany()
			      .HasForeignKey(d => d.LodgeFeeProgId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_Stables_FutureProgs_Lodge");

			entity.HasOne(d => d.DailyFeeProg)
			      .WithMany()
			      .HasForeignKey(d => d.DailyFeeProgId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_Stables_FutureProgs_Daily");

			entity.HasOne(d => d.CanStableProg)
			      .WithMany()
			      .HasForeignKey(d => d.CanStableProgId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_Stables_FutureProgs_Can");

			entity.HasOne(d => d.WhyCannotStableProg)
			      .WithMany()
			      .HasForeignKey(d => d.WhyCannotStableProgId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_Stables_FutureProgs_Why");
		});

		modelBuilder.Entity<StableStay>(entity =>
		{
			entity.ToTable("StableStays");
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.HasIndex(e => e.StableId).HasDatabaseName("FK_StableStays_Stables_idx");
			entity.HasIndex(e => e.MountId).HasDatabaseName("FK_StableStays_Characters_Mount_idx");
			entity.HasIndex(e => e.OriginalOwnerId).HasDatabaseName("FK_StableStays_Characters_Owner_idx");
			entity.HasIndex(e => e.TicketItemId).HasDatabaseName("FK_StableStays_GameItems_Ticket_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.StableId).HasColumnType("bigint(20)");
			entity.Property(e => e.MountId).HasColumnType("bigint(20)");
			entity.Property(e => e.OriginalOwnerId).HasColumnType("bigint(20)");
			entity.Property(e => e.OriginalOwnerName)
			      .IsRequired()
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.LodgedDateTime)
			      .IsRequired()
			      .HasColumnType("varchar(500)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.LastDailyFeeDateTime)
			      .IsRequired()
			      .HasColumnType("varchar(500)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.ClosedDateTime)
			      .HasColumnType("varchar(500)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.TicketItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.TicketToken)
			      .IsRequired()
			      .HasColumnType("varchar(200)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.AmountOwing).HasColumnType("decimal(58,29)");

			entity.HasOne(d => d.Stable)
			      .WithMany(p => p.Stays)
			      .HasForeignKey(d => d.StableId)
			      .HasConstraintName("FK_StableStays_Stables");

			entity.HasOne(d => d.Mount)
			      .WithMany()
			      .HasForeignKey(d => d.MountId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_StableStays_Characters_Mount");

			entity.HasOne(d => d.OriginalOwner)
			      .WithMany()
			      .HasForeignKey(d => d.OriginalOwnerId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_StableStays_Characters_Owner");

			entity.HasOne(d => d.TicketItem)
			      .WithMany()
			      .HasForeignKey(d => d.TicketItemId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_StableStays_GameItems_Ticket");
		});

		modelBuilder.Entity<StableStayLedgerEntry>(entity =>
		{
			entity.ToTable("StableStayLedgerEntries");
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.HasIndex(e => e.StableStayId).HasDatabaseName("FK_StableStayLedgerEntries_StableStays_idx");
			entity.HasIndex(e => e.ActorId).HasDatabaseName("FK_StableStayLedgerEntries_Characters_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.StableStayId).HasColumnType("bigint(20)");
			entity.Property(e => e.EntryType).HasColumnType("int(11)");
			entity.Property(e => e.MudDateTime)
			      .IsRequired()
			      .HasColumnType("varchar(500)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.ActorId).HasColumnType("bigint(20)");
			entity.Property(e => e.ActorName)
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");
			entity.Property(e => e.Note)
			      .IsRequired()
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");

			entity.HasOne(d => d.StableStay)
			      .WithMany(p => p.LedgerEntries)
			      .HasForeignKey(d => d.StableStayId)
			      .HasConstraintName("FK_StableStayLedgerEntries_StableStays");

			entity.HasOne(d => d.Actor)
			      .WithMany()
			      .HasForeignKey(d => d.ActorId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_StableStayLedgerEntries_Characters");
		});

		modelBuilder.Entity<StableAccount>(entity =>
		{
			entity.ToTable("StableAccounts");
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.HasIndex(e => e.StableId).HasDatabaseName("FK_StableAccounts_Stables_idx");
			entity.HasIndex(e => e.AccountOwnerId).HasDatabaseName("FK_StableAccounts_Characters_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.StableId).HasColumnType("bigint(20)");
			entity.Property(e => e.AccountName)
			      .IsRequired()
			      .HasColumnType("varchar(200)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.AccountOwnerId).HasColumnType("bigint(20)");
			entity.Property(e => e.AccountOwnerName)
			      .IsRequired()
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.Balance).HasColumnType("decimal(58,29)");
			entity.Property(e => e.CreditLimit).HasColumnType("decimal(58,29)");
			entity.Property(e => e.IsSuspended).HasColumnType("bit(1)");

			entity.HasOne(d => d.Stable)
			      .WithMany(p => p.StableAccounts)
			      .HasForeignKey(d => d.StableId)
			      .HasConstraintName("FK_StableAccounts_Stables");

			entity.HasOne(d => d.AccountOwner)
			      .WithMany()
			      .HasForeignKey(d => d.AccountOwnerId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_StableAccounts_Characters");
		});

		modelBuilder.Entity<StableAccountUser>(entity =>
		{
			entity.ToTable("StableAccountUsers");
			entity.HasKey(e => new { e.StableAccountId, e.AccountUserId })
			      .HasName("PRIMARY");

			entity.HasIndex(e => e.AccountUserId).HasDatabaseName("FK_StableAccountUsers_Characters_idx");

			entity.Property(e => e.StableAccountId).HasColumnType("bigint(20)");
			entity.Property(e => e.AccountUserId).HasColumnType("bigint(20)");
			entity.Property(e => e.AccountUserName)
			      .IsRequired()
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.SpendingLimit).HasColumnType("decimal(58,29)");

			entity.HasOne(d => d.StableAccount)
			      .WithMany(p => p.AccountUsers)
			      .HasForeignKey(d => d.StableAccountId)
			      .HasConstraintName("FK_StableAccountUsers_StableAccounts");

			entity.HasOne(d => d.AccountUser)
			      .WithMany()
			      .HasForeignKey(d => d.AccountUserId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_StableAccountUsers_Characters");
		});
	}
}
