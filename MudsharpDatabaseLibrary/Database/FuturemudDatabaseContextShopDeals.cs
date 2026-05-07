using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database
{
    public partial class FuturemudDatabaseContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            ConfigureCharacterComputerWorkspace(modelBuilder);
            ConfigureComputerMail(modelBuilder);
            ConfigureStables(modelBuilder);
            ConfigureClanFinance(modelBuilder);
            ConfigureVirtualCash(modelBuilder);

            modelBuilder.Entity<ShopDeal>(entity =>
            {
                entity.ToTable("ShopDeals");

                entity.HasIndex(e => e.ShopId)
                      .HasDatabaseName("FK_ShopDeals_Shops_idx");

                entity.HasIndex(e => e.MerchandiseId)
                      .HasDatabaseName("FK_ShopDeals_Merchandises_idx");

                entity.HasIndex(e => e.TagId)
                      .HasDatabaseName("FK_ShopDeals_Tags_idx");

                entity.HasIndex(e => e.EligibilityProgId)
                      .HasDatabaseName("FK_ShopDeals_FutureProgs_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.ShopId).HasColumnType("bigint(20)");
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasColumnType("varchar(200)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.DealType).HasColumnType("int(11)");
                entity.Property(e => e.TargetType).HasColumnType("int(11)");
                entity.Property(e => e.MerchandiseId).HasColumnType("bigint(20)");
                entity.Property(e => e.TagId).HasColumnType("bigint(20)");
                entity.Property(e => e.PriceAdjustmentPercentage).HasColumnType("decimal(58,29)");
                entity.Property(e => e.MinimumQuantity).HasColumnType("int(11)");
                entity.Property(e => e.Applicability).HasColumnType("int(11)");
                entity.Property(e => e.EligibilityProgId).HasColumnType("bigint(20)");
                entity.Property(e => e.ExpiryDateTime)
                      .HasColumnType("varchar(500)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.IsCumulative)
                      .HasColumnType("bit(1)")
                      .HasDefaultValue(true);

                entity.HasOne(d => d.Shop)
                      .WithMany(p => p.ShopDeals)
                      .HasForeignKey(d => d.ShopId)
                      .HasConstraintName("FK_ShopDeals_Shops");

                entity.HasOne(d => d.Merchandise)
                      .WithMany(p => p.ShopDeals)
                      .HasForeignKey(d => d.MerchandiseId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_ShopDeals_Merchandises");

                entity.HasOne(d => d.Tag)
                      .WithMany(p => p.ShopDeals)
                      .HasForeignKey(d => d.TagId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_ShopDeals_Tags");

                entity.HasOne(d => d.EligibilityProg)
                      .WithMany(p => p.ShopDeals)
                      .HasForeignKey(d => d.EligibilityProgId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_ShopDeals_FutureProgs");
            });

            modelBuilder.Entity<MarketPopulation>(entity =>
            {
                entity.Property(e => e.IncomeFactor)
                      .HasDefaultValue(1.0m);

                entity.Property(e => e.Savings)
                      .HasDefaultValue(0.0m);

                entity.Property(e => e.SavingsCap)
                      .HasDefaultValue(0.0m);

                entity.Property(e => e.StressFlickerThreshold)
                      .HasDefaultValue(0.01m);
            });
        }

        private static void ConfigureVirtualCash(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VirtualCashBalance>(entity =>
            {
                entity.ToTable("VirtualCashBalances");
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasIndex(e => new { e.OwnerType, e.OwnerId, e.CurrencyId })
                      .IsUnique()
                      .HasDatabaseName("IX_VirtualCashBalances_Owner_Currency");
                entity.HasIndex(e => e.CurrencyId)
                      .HasDatabaseName("FK_VirtualCashBalances_Currencies_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.OwnerType)
                      .IsRequired()
                      .HasColumnType("varchar(100)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.OwnerId).HasColumnType("bigint(20)");
                entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");
                entity.Property(e => e.Balance).HasColumnType("decimal(58,29)");

                entity.HasOne(d => d.Currency)
                      .WithMany()
                      .HasForeignKey(d => d.CurrencyId)
                      .HasConstraintName("FK_VirtualCashBalances_Currencies");
            });

            modelBuilder.Entity<VirtualCashLedgerEntry>(entity =>
            {
                entity.ToTable("VirtualCashLedgerEntries");
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasIndex(e => new { e.OwnerType, e.OwnerId, e.RealDateTime })
                      .HasDatabaseName("IX_VirtualCashLedgerEntries_Owner_Date");
                entity.HasIndex(e => e.CurrencyId)
                      .HasDatabaseName("FK_VirtualCashLedgerEntries_Currencies_idx");
                entity.HasIndex(e => e.LinkedBankAccountId)
                      .HasDatabaseName("FK_VirtualCashLedgerEntries_BankAccounts_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.OwnerType)
                      .IsRequired()
                      .HasColumnType("varchar(100)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.OwnerId).HasColumnType("bigint(20)");
                entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");
                entity.Property(e => e.RealDateTime).HasColumnType("datetime");
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
                entity.Property(e => e.CounterpartyId).HasColumnType("bigint(20)");
                entity.Property(e => e.CounterpartyType)
                      .HasColumnType("varchar(100)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.CounterpartyName)
                      .HasColumnType("mediumtext")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");
                entity.Property(e => e.BalanceAfter).HasColumnType("decimal(58,29)");
                entity.Property(e => e.SourceKind)
                      .IsRequired()
                      .HasColumnType("varchar(100)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.DestinationKind)
                      .IsRequired()
                      .HasColumnType("varchar(100)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.LinkedBankAccountId).HasColumnType("bigint(20)");
                entity.Property(e => e.ReferenceType)
                      .HasColumnType("varchar(100)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.ReferenceId).HasColumnType("bigint(20)");
                entity.Property(e => e.Reference)
                      .HasColumnType("mediumtext")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.Reason)
                      .IsRequired()
                      .HasColumnType("mediumtext")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.Currency)
                      .WithMany()
                      .HasForeignKey(d => d.CurrencyId)
                      .HasConstraintName("FK_VirtualCashLedgerEntries_Currencies");
                entity.HasOne(d => d.LinkedBankAccount)
                      .WithMany()
                      .HasForeignKey(d => d.LinkedBankAccountId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_VirtualCashLedgerEntries_BankAccounts");
            });
        }

        private static void ConfigureClanFinance(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClanBudget>(entity =>
            {
                entity.ToTable("ClanBudgets");
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasIndex(e => e.ClanId)
                      .HasDatabaseName("FK_ClanBudgets_Clans_idx");
                entity.HasIndex(e => e.AppointmentId)
                      .HasDatabaseName("FK_ClanBudgets_Appointments_idx");
                entity.HasIndex(e => e.BankAccountId)
                      .HasDatabaseName("FK_ClanBudgets_BankAccounts_idx");
                entity.HasIndex(e => e.CurrencyId)
                      .HasDatabaseName("FK_ClanBudgets_Currencies_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.ClanId).HasColumnType("bigint(20)");
                entity.Property(e => e.AppointmentId).HasColumnType("bigint(20)");
                entity.Property(e => e.BankAccountId).HasColumnType("bigint(20)");
                entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasColumnType("varchar(200)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.AmountPerPeriod).HasColumnType("decimal(58,29)");
                entity.Property(e => e.CurrentPeriodDrawdown).HasColumnType("decimal(58,29)");
                entity.Property(e => e.PeriodIntervalType).HasColumnType("int(11)");
                entity.Property(e => e.PeriodIntervalModifier).HasColumnType("int(11)");
                entity.Property(e => e.PeriodIntervalOther).HasColumnType("int(11)");
                entity.Property(e => e.PeriodIntervalOtherSecondary).HasColumnType("int(11)");
                entity.Property(e => e.PeriodIntervalFallback).HasColumnType("int(11)");
                entity.Property(e => e.CurrentPeriodStart)
                      .IsRequired()
                      .HasColumnType("varchar(255)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.CurrentPeriodEnd)
                      .IsRequired()
                      .HasColumnType("varchar(255)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.IsActive)
                      .HasColumnType("bit(1)")
                      .HasDefaultValue(true);

                entity.HasOne(e => e.Clan)
                      .WithMany(e => e.ClanBudgets)
                      .HasForeignKey(e => e.ClanId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_ClanBudgets_Clans");
                entity.HasOne(e => e.Appointment)
                      .WithMany(e => e.ClanBudgets)
                      .HasForeignKey(e => e.AppointmentId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_ClanBudgets_Appointments");
                entity.HasOne(e => e.BankAccount)
                      .WithMany(e => e.ClanBudgets)
                      .HasForeignKey(e => e.BankAccountId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_ClanBudgets_BankAccounts");
                entity.HasOne(e => e.Currency)
                      .WithMany(e => e.ClanBudgets)
                      .HasForeignKey(e => e.CurrencyId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_ClanBudgets_Currencies");
            });

            modelBuilder.Entity<ClanBudgetTransaction>(entity =>
            {
                entity.ToTable("ClanBudgetTransactions");
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasIndex(e => e.ClanBudgetId)
                      .HasDatabaseName("FK_ClanBudgetTransactions_Budgets_idx");
                entity.HasIndex(e => e.ActorId)
                      .HasDatabaseName("FK_ClanBudgetTransactions_Actors_idx");
                entity.HasIndex(e => e.BankAccountId)
                      .HasDatabaseName("FK_ClanBudgetTransactions_BankAccounts_idx");
                entity.HasIndex(e => e.CurrencyId)
                      .HasDatabaseName("FK_ClanBudgetTransactions_Currencies_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.ClanBudgetId).HasColumnType("bigint(20)");
                entity.Property(e => e.ActorId).HasColumnType("bigint(20)");
                entity.Property(e => e.BankAccountId).HasColumnType("bigint(20)");
                entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");
                entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");
                entity.Property(e => e.BankBalanceAfter).HasColumnType("decimal(58,29)");
                entity.Property(e => e.TransactionTime)
                      .IsRequired()
                      .HasColumnType("varchar(255)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.PeriodStart)
                      .IsRequired()
                      .HasColumnType("varchar(255)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.PeriodEnd)
                      .IsRequired()
                      .HasColumnType("varchar(255)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.Reason)
                      .IsRequired()
                      .HasColumnType("varchar(1000)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");

                entity.HasOne(e => e.ClanBudget)
                      .WithMany(e => e.ClanBudgetTransactions)
                      .HasForeignKey(e => e.ClanBudgetId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_ClanBudgetTransactions_Budgets");
                entity.HasOne(e => e.Actor)
                      .WithMany()
                      .HasForeignKey(e => e.ActorId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_ClanBudgetTransactions_Characters");
                entity.HasOne(e => e.BankAccount)
                      .WithMany(e => e.ClanBudgetTransactions)
                      .HasForeignKey(e => e.BankAccountId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_ClanBudgetTransactions_BankAccounts");
                entity.HasOne(e => e.Currency)
                      .WithMany(e => e.ClanBudgetTransactions)
                      .HasForeignKey(e => e.CurrencyId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_ClanBudgetTransactions_Currencies");
            });

            modelBuilder.Entity<ClanPayrollHistory>(entity =>
            {
                entity.ToTable("ClanPayrollHistories");
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasIndex(e => e.ClanId)
                      .HasDatabaseName("FK_ClanPayrollHistories_Clans_idx");
                entity.HasIndex(e => e.CharacterId)
                      .HasDatabaseName("FK_ClanPayrollHistories_Characters_idx");
                entity.HasIndex(e => e.RankId)
                      .HasDatabaseName("FK_ClanPayrollHistories_Ranks_idx");
                entity.HasIndex(e => e.PaygradeId)
                      .HasDatabaseName("FK_ClanPayrollHistories_Paygrades_idx");
                entity.HasIndex(e => e.AppointmentId)
                      .HasDatabaseName("FK_ClanPayrollHistories_Appointments_idx");
                entity.HasIndex(e => e.ActorId)
                      .HasDatabaseName("FK_ClanPayrollHistories_Actors_idx");
                entity.HasIndex(e => e.CurrencyId)
                      .HasDatabaseName("FK_ClanPayrollHistories_Currencies_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.ClanId).HasColumnType("bigint(20)");
                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");
                entity.Property(e => e.RankId).HasColumnType("bigint(20)");
                entity.Property(e => e.PaygradeId).HasColumnType("bigint(20)");
                entity.Property(e => e.AppointmentId).HasColumnType("bigint(20)");
                entity.Property(e => e.ActorId).HasColumnType("bigint(20)");
                entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");
                entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");
                entity.Property(e => e.EntryType).HasColumnType("int(11)");
                entity.Property(e => e.DateTime)
                      .IsRequired()
                      .HasColumnType("varchar(255)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");
                entity.Property(e => e.Description)
                      .IsRequired()
                      .HasColumnType("varchar(1000)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");

                entity.HasOne(e => e.Clan)
                      .WithMany(e => e.ClanPayrollHistories)
                      .HasForeignKey(e => e.ClanId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_ClanPayrollHistories_Clans");
                entity.HasOne(e => e.Character)
                      .WithMany()
                      .HasForeignKey(e => e.CharacterId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_ClanPayrollHistories_Characters");
                entity.HasOne(e => e.Rank)
                      .WithMany()
                      .HasForeignKey(e => e.RankId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_ClanPayrollHistories_Ranks");
                entity.HasOne(e => e.Paygrade)
                      .WithMany()
                      .HasForeignKey(e => e.PaygradeId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_ClanPayrollHistories_Paygrades");
                entity.HasOne(e => e.Appointment)
                      .WithMany()
                      .HasForeignKey(e => e.AppointmentId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_ClanPayrollHistories_Appointments");
                entity.HasOne(e => e.Actor)
                      .WithMany()
                      .HasForeignKey(e => e.ActorId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_ClanPayrollHistories_Actors");
                entity.HasOne(e => e.Currency)
                      .WithMany(e => e.ClanPayrollHistories)
                      .HasForeignKey(e => e.CurrencyId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_ClanPayrollHistories_Currencies");
            });
        }
    }
}
