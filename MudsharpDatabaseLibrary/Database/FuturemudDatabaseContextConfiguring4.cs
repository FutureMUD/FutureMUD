using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database
{
	public partial class FuturemudDatabaseContext
	{
        protected static void OnModelCreatingFour(ModelBuilder modelBuilder)
        {
	        modelBuilder.Entity<GPTMessage>(entity =>
	        {
		        entity.ToTable("GPTMessages");
		        entity.Property(e => e.Id).HasColumnType("bigint(20)");
		        entity.Property(e => e.GPTThreadId).HasColumnType("bigint(20)");
		        entity.Property(e => e.CharacterId).HasColumnType("bigint(20)").IsRequired(false);
		        entity.Property(e => e.Message)
		              .IsRequired()
		              .HasColumnType("text")
		              .HasCharSet("utf8")
		              .UseCollation("utf8_general_ci");
		        entity.Property(e => e.Response)
		              .IsRequired(false)
		              .HasColumnType("text")
		              .HasCharSet("utf8")
		              .UseCollation("utf8_general_ci");

		        entity.HasOne(e => e.GPTThread)
		              .WithMany(d => d.Messages)
		              .HasForeignKey(e => e.GPTThreadId)
		              .OnDelete(DeleteBehavior.Cascade)
		              .HasConstraintName("FK_GPTMessages_GPTThreads")
			        ;
		        entity.HasOne(e => e.Character)
		              .WithMany(d => d.GPTMessages)
		              .HasForeignKey(e => e.CharacterId)
		              .OnDelete(DeleteBehavior.Cascade)
		              .HasConstraintName("FK_GPTMessages_Characters")
			        ;
	        });

	        modelBuilder.Entity<GPTThread>(entity =>
		        {
			        entity.ToTable("GPTThreads");
			        entity.Property(e => e.Id).HasColumnType("bigint(20)");
			        entity.Property(e => e.Temperature).HasColumnType("double");
			        entity.Property(e => e.Name)
			              .IsRequired()
			              .HasColumnType("varchar(200)")
			              .HasCharSet("utf8")
			              .UseCollation("utf8_general_ci");
			        entity.Property(e => e.Model)
			              .IsRequired()
			              .HasColumnType("varchar(200)")
			              .HasCharSet("utf8")
			              .UseCollation("utf8_general_ci");
					entity.Property(e => e.Prompt)
			              .IsRequired()
			              .HasColumnType("text")
			              .HasCharSet("utf8")
			              .UseCollation("utf8_general_ci");
				});

            modelBuilder.Entity<ActiveJob>(entity =>
            {
                entity.ToTable("ActiveJobs");
                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.JobListingId).HasColumnType("bigint(20)");
                entity.Property(e => e.ActiveProjectId).HasColumnType("bigint(20)");
                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");
                entity.Property(e => e.IsJobComplete).HasColumnType("bit(1)");
                entity.Property(e => e.AlreadyHadClanPosition).HasColumnType("bit(1)");
                entity.Property(e => e.JobCommenced)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.JobDueToEnd)
                    .IsRequired(false)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.BackpayOwed)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.RevenueEarned)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(e => e.JobListing)
                    .WithMany(e => e.ActiveJobs)
                    .HasForeignKey(e => e.JobListingId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ActiveJobs_JobListings");

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.ActiveJobs)
                    .HasForeignKey(e => e.CharacterId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ActiveJobs_Characters");

                entity.HasOne(e => e.ActiveProject)
                    .WithMany()
                    .HasForeignKey(e => e.ActiveProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActiveJobs_ActiveProjects");
            });

            modelBuilder.Entity<JobListing>(entity =>
            {
                entity.ToTable("JobListings");
                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.IsReadyToBePosted).HasColumnType("bit(1)");
                entity.Property(e => e.IsArchived).HasColumnType("bit(1)");
                entity.Property(e => e.PosterId).HasColumnType("bigint(20)");
                entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");
                entity.Property(e => e.EligibilityProgId).HasColumnType("bigint(20)");
                entity.Property(e => e.ClanId).HasColumnType("bigint(20)").IsRequired(false);
                entity.Property(e => e.RankId).HasColumnType("bigint(20)").IsRequired(false);
                entity.Property(e => e.PaygradeId).HasColumnType("bigint(20)").IsRequired(false);
                entity.Property(e => e.AppointmentId).HasColumnType("bigint(20)").IsRequired(false);
                entity.Property(e => e.PersonalProjectId).HasColumnType("bigint(20)").IsRequired(false);
                entity.Property(e => e.PersonalProjectRevisionNumber).HasColumnType("int(11)").IsRequired(false);
                entity.Property(e => e.RequiredProjectId).HasColumnType("bigint(20)").IsRequired(false);
                entity.Property(e => e.RequiredProjectLabourId).HasColumnType("bigint(20)").IsRequired(false);
                entity.Property(e => e.BankAccountId).HasColumnType("bigint(20)").IsRequired(false);
                entity.Property(e => e.MaximumNumberOfSimultaneousEmployees).HasColumnType("int(11)");
                entity.Property(e => e.FullTimeEquivalentRatio).HasColumnType("double");
                entity.Property(e => e.PosterType)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.JobListingType)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.MaximumDuration)
                    .IsRequired(false)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.MoneyPaidIn)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.Definition)
                    .IsRequired(false)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.Description)
                    .IsRequired(false)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(e => e.EligibilityProg)
                    .WithMany()
                    .HasForeignKey(e => e.EligibilityProgId)
                    .HasConstraintName("FK_JobListings_FutureProgs");

                entity.HasOne(e => e.Clan)
                    .WithMany()
                    .HasForeignKey(e => e.ClanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobListings_Clans");
                entity.HasOne(e => e.BankAccount)
                    .WithMany()
                    .HasForeignKey(e => e.BankAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobListings_BankAccounts");
                entity.HasOne(e => e.EconomicZone)
                    .WithMany(e => e.JobListings)
                    .HasForeignKey(e => e.EconomicZoneId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_JobListings_EconomicZones");
                entity.HasOne(e => e.Rank)
                    .WithMany()
                    .HasForeignKey(e => e.RankId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobListings_Ranks");
                entity.HasOne(e => e.Paygrade)
                    .WithMany()
                    .HasForeignKey(e => e.PaygradeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobListings_Paygrades");
                entity.HasOne(e => e.Appointment)
                    .WithMany()
                    .HasForeignKey(e => e.AppointmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobListings_Appointments");
                entity.HasOne(e => e.PersonalProject)
                    .WithMany()
                    .HasForeignKey(e => new { e.PersonalProjectId, e.PersonalProjectRevisionNumber })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobListings_Projects");
                entity.HasOne(e => e.RequiredProject)
                    .WithMany()
                    .HasForeignKey(e => e.RequiredProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobListings_ActiveProjects");
                entity.HasOne(e => e.RequiredProjectLabour)
                    .WithMany()
                    .HasForeignKey(e => new { e.RequiredProjectId, e.RequiredProjectLabourId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobListings_ActiveProjectLabours");
            });

            modelBuilder.Entity<RegionalClimate>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ClimateModelId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<RegionalClimatesSeason>(entity =>
            {
                entity.HasKey(e => new { e.RegionalClimateId, e.SeasonId })
                    .HasName("PRIMARY");

                entity.ToTable("RegionalClimates_Seasons");

                entity.HasIndex(e => e.SeasonId)
                    .HasDatabaseName("FK_RegionalClimates_Seasons_Seasons_idx");

                entity.Property(e => e.RegionalClimateId).HasColumnType("bigint(20)");

                entity.Property(e => e.SeasonId).HasColumnType("bigint(20)");

                entity.Property(e => e.TemperatureInfo)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.RegionalClimate)
                    .WithMany(p => p.RegionalClimatesSeasons)
                    .HasForeignKey(d => d.RegionalClimateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RegionalClimates_Seasons_RegionalClimates");

                entity.HasOne(d => d.Season)
                    .WithMany(p => p.RegionalClimatesSeasons)
                    .HasForeignKey(d => d.SeasonId)
                    .HasConstraintName("FK_RegionalClimates_Seasons_Seasons");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasIndex(e => e.ZoneId)
                    .HasDatabaseName("FK_Rooms_Zones");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.X).HasColumnType("int(11)");

                entity.Property(e => e.Y).HasColumnType("int(11)");

                entity.Property(e => e.Z).HasColumnType("int(11)");

                entity.Property(e => e.ZoneId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Zone)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.ZoneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rooms_Zones");
            });

            modelBuilder.Entity<Script>(entity =>
            {
                entity.HasIndex(e => e.KnowledgeId)
                    .HasDatabaseName("FK_Scripts_Knowledges_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.DocumentLengthModifier).HasDefaultValueSql("'1'");

                entity.Property(e => e.InkUseModifier).HasDefaultValueSql("'1'");

                entity.Property(e => e.KnowledgeId).HasColumnType("bigint(20)");

                entity.Property(e => e.KnownScriptDescription)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.UnknownScriptDescription)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.Knowledge)
                    .WithMany(p => p.Scripts)
                    .HasForeignKey(d => d.KnowledgeId)
                    .HasConstraintName("FK_Scripts_Knowledges");
            });

            modelBuilder.Entity<ScriptsDesignedLanguage>(entity =>
            {
                entity.HasKey(e => new { e.ScriptId, e.LanguageId })
                    .HasName("PRIMARY");

                entity.ToTable("Scripts_DesignedLanguages");

                entity.HasIndex(e => e.LanguageId)
                    .HasDatabaseName("FK_Scripts_DesignedLanguages_Languages_idx");

                entity.Property(e => e.ScriptId).HasColumnType("bigint(20)");

                entity.Property(e => e.LanguageId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.ScriptsDesignedLanguages)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("FK_Scripts_DesignedLanguages_Languages");

                entity.HasOne(d => d.Script)
                    .WithMany(p => p.ScriptsDesignedLanguages)
                    .HasForeignKey(d => d.ScriptId)
                    .HasConstraintName("FK_Scripts_DesignedLanguages_Scripts");
            });

            modelBuilder.Entity<Season>(entity =>
            {
                entity.HasIndex(e => e.CelestialId)
                    .HasDatabaseName("FK_Seasons_Celestials_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CelestialDayOnset).HasColumnType("int(11)");

                entity.Property(e => e.CelestialId).HasColumnType("bigint(20)");

                entity.Property(e => e.SeasonGroup)
                      .IsRequired()
                      .HasColumnType("varchar(200)")
                      .HasCharSet("utf8")
                      .UseCollation("utf8_general_ci");

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.Celestial)
                    .WithMany(p => p.Seasons)
                    .HasForeignKey(d => d.CelestialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Seasons_Celestials");
            });

            modelBuilder.Entity<Shard>(entity =>
            {
                entity.HasIndex(e => e.SkyDescriptionTemplateId)
                    .HasDatabaseName("FK_Shards_SkyDescriptionTemplates");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.SkyDescriptionTemplateId).HasColumnType("bigint(20)");

                entity.Property(e => e.SphericalRadiusMetres).HasDefaultValueSql("'6371000'");

                entity.HasOne(d => d.SkyDescriptionTemplate)
                    .WithMany(p => p.Shards)
                    .HasForeignKey(d => d.SkyDescriptionTemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Shards_SkyDescriptionTemplates");
            });

            modelBuilder.Entity<ShardsCalendars>(entity =>
            {
                entity.HasKey(e => new { e.ShardId, e.CalendarId })
                    .HasName("PRIMARY");

                entity.ToTable("Shards_Calendars");

                entity.Property(e => e.ShardId).HasColumnType("bigint(20)");

                entity.Property(e => e.CalendarId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Shard)
                    .WithMany(p => p.ShardsCalendars)
                    .HasForeignKey(d => d.ShardId)
                    .HasConstraintName("FK_Shards_Calendars_Shards");
            });

            modelBuilder.Entity<ShardsCelestials>(entity =>
            {
                entity.HasKey(e => new { e.ShardId, e.CelestialId })
                    .HasName("PRIMARY");

                entity.ToTable("Shards_Celestials");

                entity.Property(e => e.ShardId).HasColumnType("bigint(20)");

                entity.Property(e => e.CelestialId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Shard)
                    .WithMany(p => p.ShardsCelestials)
                    .HasForeignKey(d => d.ShardId)
                    .HasConstraintName("FK_Shards_Celestials_Shards");
            });

            modelBuilder.Entity<ShardsClocks>(entity =>
            {
                entity.HasKey(e => new { e.ShardId, e.ClockId })
                    .HasName("PRIMARY");

                entity.ToTable("Shards_Clocks");

                entity.Property(e => e.ShardId).HasColumnType("bigint(20)");

                entity.Property(e => e.ClockId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Shard)
                    .WithMany(p => p.ShardsClocks)
                    .HasForeignKey(d => d.ShardId)
                    .HasConstraintName("FK_Shards_Clocks_Shards");
            });

            modelBuilder.Entity<ShieldType>(entity =>
            {
                entity.HasIndex(e => e.BlockTraitId)
                    .HasDatabaseName("FK_ShieldTypes_TraitDefinitions_idx");

                entity.HasIndex(e => e.EffectiveArmourTypeId)
                    .HasDatabaseName("FK_ShieldTypes_ArmourTypes_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BlockTraitId).HasColumnType("bigint(20)");

                entity.Property(e => e.EffectiveArmourTypeId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.HasOne(d => d.BlockTrait)
                    .WithMany(p => p.ShieldTypes)
                    .HasForeignKey(d => d.BlockTraitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShieldTypes_TraitDefinitions");

                entity.HasOne(d => d.EffectiveArmourType)
                    .WithMany(p => p.ShieldTypes)
                    .HasForeignKey(d => d.EffectiveArmourTypeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_ShieldTypes_ArmourTypes");
            });

            modelBuilder.Entity<ShopFinancialPeriodResult>(entity =>
            {
                entity.HasKey(e => new { e.EconomicZoneId, e.ShopId, e.FinancialPeriodId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.FinancialPeriodId)
                    .HasDatabaseName("FK_ShopFinancialPeriodResults_FinancialPeriods_idx");

                entity.HasIndex(e => e.ShopId)
                    .HasDatabaseName("FK_ShopFinancialPeriodResults_Shops_idx");

                entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");

                entity.Property(e => e.ShopId).HasColumnType("bigint(20)");

                entity.Property(e => e.FinancialPeriodId).HasColumnType("bigint(20)");

                entity.Property(e => e.GrossRevenue).HasColumnType("decimal(58,29)");

                entity.Property(e => e.NetRevenue).HasColumnType("decimal(58,29)");

                entity.Property(e => e.ProfitsTax).HasColumnType("decimal(58,29)");

                entity.Property(e => e.SalesTax).HasColumnType("decimal(58,29)");

                entity.HasOne(d => d.EconomicZone)
                    .WithMany(p => p.ShopFinancialPeriodResults)
                    .HasForeignKey(d => d.EconomicZoneId)
                    .HasConstraintName("FK_ShopFinancialPeriodResults_EconomicZones");

                entity.HasOne(d => d.FinancialPeriod)
                    .WithMany(p => p.ShopFinancialPeriodResults)
                    .HasForeignKey(d => d.FinancialPeriodId)
                    .HasConstraintName("FK_ShopFinancialPeriodResults_FinancialPeriods");

                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.ShopFinancialPeriodResults)
                    .HasForeignKey(d => d.ShopId)
                    .HasConstraintName("FK_ShopFinancialPeriodResults_Shops");
            });

            modelBuilder.Entity<ShopTransactionRecord>(entity =>
            {
                entity.HasIndex(e => e.CurrencyId)
                    .HasDatabaseName("FK_ShopTransactionRecords_Currencies_idx");

                entity.HasIndex(e => e.ShopId)
                    .HasDatabaseName("FK_ShopTransactionRecords_Shops_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");

                entity.Property(e => e.MudDateTime)
                    .IsRequired()
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PretaxValue).HasColumnType("decimal(58,29)");

                entity.Property(e => e.RealDateTime).HasColumnType("datetime");

                entity.Property(e => e.ShopId).HasColumnType("bigint(20)");

                entity.Property(e => e.Tax).HasColumnType("decimal(58,29)");

                entity.Property(e => e.ThirdPartyId).HasColumnType("bigint(20)");

                entity.Property(e => e.TransactionType).HasColumnType("int(11)");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.ShopTransactionRecords)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_ShopTransactionRecords_Currencies");

                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.ShopTransactionRecords)
                    .HasForeignKey(d => d.ShopId)
                    .HasConstraintName("FK_ShopTransactionRecords_Shops");
            });

            modelBuilder.Entity<Shop>(entity =>
            {
                entity.HasIndex(e => e.CanShopProgId)
                    .HasDatabaseName("FK_Shops_FutureProgs_Can_idx");

                entity.HasIndex(e => e.CurrencyId)
                    .HasDatabaseName("FK_Shops_Currencies_idx");

                entity.HasIndex(e => e.EconomicZoneId)
                    .HasDatabaseName("FK_Shops_EconomicZonesa_idx");

                entity.HasIndex(e => e.StockroomCellId)
                    .HasDatabaseName("FK_Shops_Cells_Stockroom_idx");

                entity.HasIndex(e => e.WhyCannotShopProgId)
                    .HasDatabaseName("FK_Shops_FutureProgs_WhyCant_idx");

                entity.HasIndex(e => e.WorkshopCellId)
                    .HasDatabaseName("FK_Shops_Cells_Workshop_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CanShopProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");

                entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");
                entity.Property(e => e.BankAccountId).HasColumnType("bigint(20)");
                entity.Property(e => e.CashBalance).HasColumnType("decimal(58, 29)");

				entity.Property(e => e.EmployeeRecords)
                    .IsRequired()
                    .HasColumnType("mediumtext")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.IsTrading).HasColumnType("bit(1)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.StockroomCellId).HasColumnType("bigint(20)");

                entity.Property(e => e.WhyCannotShopProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.WorkshopCellId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.BankAccount)
	                .WithMany()
	                .HasForeignKey(d => d.BankAccountId)
	                .OnDelete(DeleteBehavior.SetNull)
	                .HasConstraintName("FK_Shops_BankAccounts");

				entity.HasOne(d => d.CanShopProg)
                    .WithMany(p => p.ShopsCanShopProg)
                    .HasForeignKey(d => d.CanShopProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Shops_FutureProgs_Can");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Shops)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Shops_Currencies");

                entity.HasOne(d => d.EconomicZone)
                    .WithMany(p => p.Shops)
                    .HasForeignKey(d => d.EconomicZoneId)
                    .HasConstraintName("FK_Shops_EconomicZones");

                entity.HasOne(d => d.StockroomCell)
                    .WithMany(p => p.ShopsStockroomCell)
                    .HasForeignKey(d => d.StockroomCellId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Shops_Cells_Stockroom");

                entity.HasOne(d => d.WhyCannotShopProg)
                    .WithMany(p => p.ShopsWhyCannotShopProg)
                    .HasForeignKey(d => d.WhyCannotShopProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Shops_FutureProgs_WhyCant");

                entity.HasOne(d => d.WorkshopCell)
                    .WithMany(p => p.ShopsWorkshopCell)
                    .HasForeignKey(d => d.WorkshopCellId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Shops_Cells_Workshop");
            });

            modelBuilder.Entity<ShopsStoreroomCell>(entity =>
            {
                entity.HasKey(e => new { e.ShopId, e.CellId })
                    .HasName("PRIMARY");

                entity.ToTable("Shops_StoreroomCells");

                entity.HasIndex(e => e.CellId)
                    .HasDatabaseName("FK_Shops_StoreroomCells_Cells_idx");

                entity.Property(e => e.ShopId).HasColumnType("bigint(20)");

                entity.Property(e => e.CellId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Cell)
                    .WithMany(p => p.ShopsStoreroomCells)
                    .HasForeignKey(d => d.CellId)
                    .HasConstraintName("FK_Shops_StoreroomCells_Cells");

                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.ShopsStoreroomCells)
                    .HasForeignKey(d => d.ShopId)
                    .HasConstraintName("FK_Shops_StoreroomCells_Shops");
            });

            modelBuilder.Entity<ShopsTill>(entity =>
            {
                entity.HasKey(e => new { e.ShopId, e.GameItemId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.GameItemId)
                    .HasDatabaseName("FK_ShopTills_GameItems_idx");

                entity.Property(e => e.ShopId).HasColumnType("bigint(20)");

                entity.Property(e => e.GameItemId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.GameItem)
                    .WithMany(p => p.ShopsTills)
                    .HasForeignKey(d => d.GameItemId)
                    .HasConstraintName("FK_ShopTills_GameItems");

                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.ShopsTills)
                    .HasForeignKey(d => d.ShopId)
                    .HasConstraintName("FK_ShopTills_Shops");
            });

            modelBuilder.Entity<SkyDescriptionTemplate>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<SkyDescriptionTemplatesValue>(entity =>
            {
                entity.HasKey(e => new { e.SkyDescriptionTemplateId, e.LowerBound })
                    .HasName("PRIMARY");

                entity.ToTable("SkyDescriptionTemplates_Values");

                entity.Property(e => e.SkyDescriptionTemplateId).HasColumnType("bigint(20)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.SkyDescriptionTemplate)
                    .WithMany(p => p.SkyDescriptionTemplatesValues)
                    .HasForeignKey(d => d.SkyDescriptionTemplateId)
                    .HasConstraintName("FK_SkyDescriptionTemplates_Values_SkyDescriptionTemplates");
            });

            modelBuilder.Entity<Social>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.FutureProgId)
                    .HasDatabaseName("FK_Socials_FutureProgs");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DirectionTargetEcho)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.FutureProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.MultiTargetEcho)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.NoTargetEcho)
                    .IsRequired()
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.OneTargetEcho)
                    .IsRequired()
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.FutureProg)
                    .WithMany(p => p.Socials)
                    .HasForeignKey(d => d.FutureProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Socials_FutureProgs");
            });

            modelBuilder.Entity<StackDecorator>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("varchar(10000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<StaticConfiguration>(entity =>
            {
                entity.HasKey(e => e.SettingName)
                    .HasName("PRIMARY");

                entity.Property(e => e.SettingName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<StaticString>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<SurgicalProcedurePhase>(entity =>
            {
                entity.HasKey(e => new { e.SurgicalProcedureId, e.PhaseNumber })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.OnPhaseProgId)
                    .HasDatabaseName("FK_SurgicalProcedurePhases_FutureProgs_OnPhaseProg_idx");

                entity.Property(e => e.SurgicalProcedureId).HasColumnType("bigint(20)");

                entity.Property(e => e.PhaseNumber).HasColumnType("int(11)");

                entity.Property(e => e.InventoryActionPlan)
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.OnPhaseProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.PhaseEmote)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.PhaseSpecialEffects)
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.HasOne(d => d.OnPhaseProg)
                    .WithMany(p => p.SurgicalProcedurePhases)
                    .HasForeignKey(d => d.OnPhaseProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_SurgicalProcedurePhases_FutureProgs_OnPhaseProg");

                entity.HasOne(d => d.SurgicalProcedure)
                    .WithMany(p => p.SurgicalProcedurePhases)
                    .HasForeignKey(d => d.SurgicalProcedureId)
                    .HasConstraintName("FK_SurgicalProcedurePhases_SurgicalProcudres");
            });

            modelBuilder.Entity<SurgicalProcedure>(entity =>
            {
                entity.HasIndex(e => e.AbortProgId)
                    .HasDatabaseName("FK_SurgicalProcedures_FutureProgs_AbortProg_idx");

                entity.HasIndex(e => e.CompletionProgId)
                    .HasDatabaseName("FK_SurgicalProcedures_FutureProgs_CompletionProg_idx");

                entity.HasIndex(e => e.KnowledgeRequiredId)
                    .HasDatabaseName("FK_SurgicalProcedures_Knowledges_idx");

                entity.HasIndex(e => e.UsabilityProgId)
                    .HasDatabaseName("FK_SurgicalProcedures_FutureProgs_Usability_idx");

                entity.HasIndex(e => e.WhyCannotUseProgId)
                    .HasDatabaseName("FK_SurgicalProcedures_FutureProgs_WhyCannotUseProg_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AbortProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.Check).HasColumnType("int(11)");

                entity.Property(e => e.CompletionProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.Definition)
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.KnowledgeRequiredId).HasColumnType("bigint(20)");

                entity.Property(e => e.MedicalSchool)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.Procedure).HasColumnType("int(11)");

                entity.Property(e => e.ProcedureBeginEmote)
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ProcedureDescriptionEmote)
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ProcedureGerund)
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ProcedureName)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.UsabilityProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.WhyCannotUseProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.CheckTraitDefinitionId).HasColumnType("bigint(20)");

				entity.Property(e => e.TargetBodyTypeId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.TargetBodyType)
                .WithMany()
                .HasForeignKey(d => d.TargetBodyTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SurgicalProcedures_BodyProtos");

				entity.HasOne(d => d.AbortProg)
                    .WithMany(p => p.SurgicalProceduresAbortProg)
                    .HasForeignKey(d => d.AbortProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_SurgicalProcedures_FutureProgs_AbortProg");

                entity.HasOne(d => d.CompletionProg)
                    .WithMany(p => p.SurgicalProceduresCompletionProg)
                    .HasForeignKey(d => d.CompletionProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_SurgicalProcedures_FutureProgs_CompletionProg");

                entity.HasOne(d => d.KnowledgeRequired)
                    .WithMany(p => p.SurgicalProcedures)
                    .HasForeignKey(d => d.KnowledgeRequiredId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_SurgicalProcedures_Knowledges");

                entity.HasOne(d => d.UsabilityProg)
                    .WithMany(p => p.SurgicalProceduresUsabilityProg)
                    .HasForeignKey(d => d.UsabilityProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_SurgicalProcedures_FutureProgs_Usability");

                entity.HasOne(d => d.WhyCannotUseProg)
                    .WithMany(p => p.SurgicalProceduresWhyCannotUseProg)
                    .HasForeignKey(d => d.WhyCannotUseProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_SurgicalProcedures_FutureProgs_WhyCannotUseProg");

                entity.HasOne(d => d.CheckTraitDefinition)
                      .WithMany()
                      .HasForeignKey(d => d.CheckTraitDefinitionId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .HasConstraintName("FK_SurgicalProcedures_TraitDefinitions");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasIndex(e => e.ParentId)
                    .HasDatabaseName("FK_Tags_Parent_idx");

                entity.HasIndex(e => e.ShouldSeeProgId)
                    .HasDatabaseName("FK_Tags_Futureprogs_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ParentId).HasColumnType("bigint(20)");

                entity.Property(e => e.ShouldSeeProgId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Tags_Parent");

                entity.HasOne(d => d.ShouldSeeProg)
                    .WithMany(p => p.Tags)
                    .HasForeignKey(d => d.ShouldSeeProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Tags_Futureprogs");
            });

            modelBuilder.Entity<Terrain>(entity =>
            {
                entity.HasIndex(e => e.WeatherControllerId)
                    .HasDatabaseName("FK_Terrains_WeatherControllers_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AtmosphereId).HasColumnType("bigint(20)");

                entity.Property(e => e.AtmosphereType)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DefaultTerrain)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.ForagableProfileId).HasColumnType("bigint(20)");

                entity.Property(e => e.HideDifficulty).HasColumnType("int(11)");

                entity.Property(e => e.InfectionMultiplier).HasDefaultValueSql("'1'");

                entity.Property(e => e.InfectionType).HasColumnType("int(11)");

                entity.Property(e => e.DefaultCellOutdoorsType).HasColumnType("int(11)").HasDefaultValue("0");
                entity.Property(e => e.TerrainEditorText).HasColumnType("varchar(45)").IsRequired(false);

                entity.Property(e => e.InfectionVirulence)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'5'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.SpotDifficulty).HasColumnType("int(11)");

                entity.Property(e => e.TerrainBehaviourMode)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TerrainEditorColour)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasDefaultValueSql("'#FFFFFFFF'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TerrainANSIColour)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasDefaultValueSql("'7'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");


                entity.Property(e => e.WeatherControllerId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.WeatherController)
                    .WithMany(p => p.Terrains)
                    .HasForeignKey(d => d.WeatherControllerId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Terrains_WeatherControllers");
            });

            modelBuilder.Entity<TerrainsRangedCovers>(entity =>
            {
                entity.HasKey(e => new { e.TerrainId, e.RangedCoverId })
                    .HasName("PRIMARY");

                entity.ToTable("Terrains_RangedCovers");

                entity.HasIndex(e => e.RangedCoverId)
                    .HasDatabaseName("FK_Terrains_RangedCovers_RangedCovers_idx");

                entity.Property(e => e.TerrainId).HasColumnType("bigint(20)");

                entity.Property(e => e.RangedCoverId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.RangedCover)
                    .WithMany(p => p.TerrainsRangedCovers)
                    .HasForeignKey(d => d.RangedCoverId)
                    .HasConstraintName("FK_Terrains_RangedCovers_RangedCovers");

                entity.HasOne(d => d.Terrain)
                    .WithMany(p => p.TerrainsRangedCovers)
                    .HasForeignKey(d => d.TerrainId)
                    .HasConstraintName("FK_Terrains_RangedCovers_Terrains");
            });

            modelBuilder.Entity<Models.TimeZoneInfo>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Display)
                    .IsRequired()
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Order).HasColumnType("decimal(58,29)");
            });

            modelBuilder.Entity<Timezone>(entity =>
            {
                entity.HasIndex(e => e.ClockId)
                    .HasDatabaseName("FK_Timezones_Clocks");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ClockId).HasColumnType("bigint(20)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.OffsetHours).HasColumnType("int(11)");

                entity.Property(e => e.OffsetMinutes).HasColumnType("int(11)");

                entity.HasOne(d => d.Clock)
                    .WithMany(p => p.Timezones)
                    .HasForeignKey(d => d.ClockId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Timezones_Clocks");
            });

            modelBuilder.Entity<TraitDecorator>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Contents)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<TraitDefinition>(entity =>
            {
                entity.HasIndex(e => e.AvailabilityProgId)
                    .HasDatabaseName("FK_TraitDefinitions_AvailabilityProg");

                entity.HasIndex(e => e.ExpressionId)
                    .HasDatabaseName("FK_TraitDefinitions_TraitExpression");

                entity.HasIndex(e => e.LearnableProgId)
                    .HasDatabaseName("FK_TraitDefinitions_LearnableProg_idx");

                entity.HasIndex(e => e.TeachableProgId)
                    .HasDatabaseName("FK_TraitDefinitions_TeachableProg_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Alias)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.AvailabilityProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.BranchMultiplier).HasDefaultValueSql("'1'");

                entity.Property(e => e.ChargenBlurb)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DecoratorId).HasColumnType("bigint(20)");

                entity.Property(e => e.DerivedType).HasColumnType("int(11)");
                entity.Property(e => e.DisplayOrder).HasColumnType("int(11)").HasDefaultValueSql("1");
                entity.Property(e => e.ShowInAttributeCommand).HasColumnType("bit(1)").HasDefaultValueSql("b'1'");
                entity.Property(e => e.ShowInScoreCommand).HasColumnType("bit(1)").HasDefaultValueSql("b'1'");
                entity.Property(e => e.DisplayAsSubAttribute).HasColumnType("bit(1)").HasDefaultValueSql("b'0'");

                entity.Property(e => e.ExpressionId).HasColumnType("bigint(20)");

                entity.Property(e => e.Hidden)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.ImproverId).HasColumnType("bigint(20)");

                entity.Property(e => e.LearnDifficulty)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'7'");

                entity.Property(e => e.LearnableProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TeachDifficulty)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'7'");

                entity.Property(e => e.TeachableProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.TraitGroup)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Type).HasColumnType("int(11)");

                entity.Property(e => e.ValueExpression)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.AvailabilityProg)
                    .WithMany(p => p.TraitDefinitionsAvailabilityProg)
                    .HasForeignKey(d => d.AvailabilityProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_TraitDefinitions_AvailabilityProg");

                entity.HasOne(d => d.Expression)
                    .WithMany(p => p.TraitDefinitions)
                    .HasForeignKey(d => d.ExpressionId)
                    .HasConstraintName("FK_TraitDefinitions_TraitExpression");

                entity.HasOne(d => d.LearnableProg)
                    .WithMany(p => p.TraitDefinitionsLearnableProg)
                    .HasForeignKey(d => d.LearnableProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_TraitDefinitions_LearnableProg");

                entity.HasOne(d => d.TeachableProg)
                    .WithMany(p => p.TraitDefinitionsTeachableProg)
                    .HasForeignKey(d => d.TeachableProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_TraitDefinitions_TeachableProg");
            });

            modelBuilder.Entity<TraitDefinitionsChargenResources>(entity =>
            {
                entity.HasKey(e => new { e.TraitDefinitionId, e.ChargenResourceId, e.RequirementOnly })
                    .HasName("PRIMARY");

                entity.ToTable("TraitDefinitions_ChargenResources");

                entity.HasIndex(e => e.ChargenResourceId)
                    .HasDatabaseName("FK_TraitDefinitions_ChargenResources_ChargenResources");

                entity.Property(e => e.TraitDefinitionId).HasColumnType("bigint(20)");

                entity.Property(e => e.ChargenResourceId).HasColumnType("bigint(20)");

                entity.Property(e => e.RequirementOnly)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Amount).HasColumnType("int(11)");

                entity.HasOne(d => d.ChargenResource)
                    .WithMany(p => p.TraitDefinitionsChargenResources)
                    .HasForeignKey(d => d.ChargenResourceId)
                    .HasConstraintName("FK_TraitDefinitions_ChargenResources_ChargenResources");

                entity.HasOne(d => d.TraitDefinition)
                    .WithMany(p => p.TraitDefinitionsChargenResources)
                    .HasForeignKey(d => d.TraitDefinitionId)
                    .HasConstraintName("FK_TraitDefinitions_ChargenResources_Races");
            });

            modelBuilder.Entity<TraitExpression>(entity =>
            {
                entity.ToTable("TraitExpression");
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Expression)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasDefaultValueSql("'Unnamed Expression'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<TraitExpressionParameters>(entity =>
            {
                entity.HasKey(e => new { e.Parameter, e.TraitExpressionId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.TraitDefinitionId)
                    .HasDatabaseName("FK_TraitExpressionParameters_TraitDefinitions");

                entity.HasIndex(e => e.TraitExpressionId)
                    .HasDatabaseName("FK_TraitExpressionParameters_TraitExpression");

                entity.Property(e => e.Parameter)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TraitExpressionId).HasColumnType("bigint(20)");

                entity.Property(e => e.CanBranch)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.CanImprove)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.TraitDefinitionId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.TraitDefinition)
                    .WithMany(p => p.TraitExpressionParameters)
                    .HasForeignKey(d => d.TraitDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TraitExpressionParameters_TraitDefinitions");

                entity.HasOne(d => d.TraitExpression)
                    .WithMany(p => p.TraitExpressionParameters)
                    .HasForeignKey(d => d.TraitExpressionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TraitExpressionParameters_TraitExpression");
            });

            modelBuilder.Entity<Trait>(entity =>
            {
                entity.HasKey(e => new { e.BodyId, e.TraitDefinitionId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.TraitDefinitionId)
                    .HasDatabaseName("FK_Traits_TraitDefinitions");

                entity.Property(e => e.BodyId).HasColumnType("bigint(20)");

                entity.Property(e => e.TraitDefinitionId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Body)
                    .WithMany(p => p.Traits)
                    .HasForeignKey(d => d.BodyId)
                    .HasConstraintName("FK_Traits_Bodies");

                entity.HasOne(d => d.TraitDefinition)
                    .WithMany(p => p.Traits)
                    .HasForeignKey(d => d.TraitDefinitionId)
                    .HasConstraintName("FK_Traits_TraitDefinitions");
            });

            modelBuilder.Entity<UnitOfMeasure>(entity =>
            {
                entity.ToTable("UnitOfMeasure");
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Abbreviations)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DefaultUnitForSystem)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Describer).HasColumnType("bit(1)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PrimaryAbbreviation)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.SpaceBetween)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.System)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Type).HasColumnType("int(11)");
            });

            modelBuilder.Entity<VariableDefault>(entity =>
            {
                entity.HasKey(e => new { e.OwnerType, e.Property })
                    .HasName("PRIMARY");

                entity.Property(e => e.OwnerType).HasColumnType("bigint(20)");

                entity.Property(e => e.Property)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DefaultValue)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<VariableDefinition>(entity =>
            {
                entity.HasKey(e => new { e.OwnerType, e.Property })
                    .HasName("PRIMARY");

                entity.Property(e => e.OwnerType).HasColumnType("bigint(20)");

                entity.Property(e => e.Property)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ContainedType).HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<VariableValue>(entity =>
            {
                entity.HasKey(e => new { e.ReferenceType, e.ReferenceId, e.ReferenceProperty })
                    .HasName("PRIMARY");

                entity.Property(e => e.ReferenceType).HasColumnType("bigint(20)");

                entity.Property(e => e.ReferenceId).HasColumnType("bigint(20)");

                entity.Property(e => e.ReferenceProperty)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ValueDefinition)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ValueType).HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<WeaponAttack>(entity =>
            {
                entity.HasIndex(e => e.DamageExpressionId)
                    .HasDatabaseName("FK_WeaponAttacks_TraitExpression_Damage_idx");

                entity.HasIndex(e => e.FutureProgId)
                    .HasDatabaseName("FK_WeaponAttacks_FutureProgs_idx");

                entity.HasIndex(e => e.PainExpressionId)
                    .HasDatabaseName("FK_WeaponAttacks_TraitExpression_Pain_idx");

                entity.HasIndex(e => e.StunExpressionId)
                    .HasDatabaseName("FK_WeaponAttacks_TraitExpression_Stun_idx");

                entity.HasIndex(e => e.WeaponTypeId)
                    .HasDatabaseName("FK_WeaponAttacks_WeaponTypes_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AdditionalInfo)
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.Alignment).HasColumnType("int(11)");

                entity.Property(e => e.BaseAngleOfIncidence).HasDefaultValueSql("'1.5708'");

                entity.Property(e => e.BaseAttackerDifficulty)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'5'");

                entity.Property(e => e.BaseBlockDifficulty)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'5'");

                entity.Property(e => e.BaseDelay).HasDefaultValueSql("'1'");

                entity.Property(e => e.BaseDodgeDifficulty)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'5'");

                entity.Property(e => e.BaseParryDifficulty)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'5'");

                entity.Property(e => e.BodypartShapeId).HasColumnType("bigint(20)");

                entity.Property(e => e.DamageExpressionId).HasColumnType("bigint(20)");

                entity.Property(e => e.DamageType).HasColumnType("int(11)");

                entity.Property(e => e.ExertionLevel).HasColumnType("int(11)");

                entity.Property(e => e.FutureProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.HandednessOptions).HasColumnType("int(11)");

                entity.Property(e => e.RequiredPositionStateIds)
                    .IsRequired()
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci")
                    .HasDefaultValue("1 16 17 18");

                entity.Property(e => e.Intentions).HasColumnType("bigint(20)");

                entity.Property(e => e.MoveType).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.Orientation).HasColumnType("int(11)");

                entity.Property(e => e.PainExpressionId).HasColumnType("bigint(20)");

                entity.Property(e => e.RecoveryDifficultyFailure)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'5'");

                entity.Property(e => e.RecoveryDifficultySuccess)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'5'");

                entity.Property(e => e.StunExpressionId).HasColumnType("bigint(20)");

                entity.Property(e => e.Verb).HasColumnType("int(11)");

                entity.Property(e => e.WeaponTypeId).HasColumnType("bigint(20)");

                entity.Property(e => e.Weighting).HasDefaultValueSql("'1'");

                entity.HasOne(d => d.DamageExpression)
                    .WithMany(p => p.WeaponAttacksDamageExpression)
                    .HasForeignKey(d => d.DamageExpressionId)
                    .HasConstraintName("FK_WeaponAttacks_TraitExpression_Damage");

                entity.HasOne(d => d.FutureProg)
                    .WithMany(p => p.WeaponAttacks)
                    .HasForeignKey(d => d.FutureProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_WeaponAttacks_FutureProgs");

                entity.HasOne(d => d.PainExpression)
                    .WithMany(p => p.WeaponAttacksPainExpression)
                    .HasForeignKey(d => d.PainExpressionId)
                    .HasConstraintName("FK_WeaponAttacks_TraitExpression_Pain");

                entity.HasOne(d => d.StunExpression)
                    .WithMany(p => p.WeaponAttacksStunExpression)
                    .HasForeignKey(d => d.StunExpressionId)
                    .HasConstraintName("FK_WeaponAttacks_TraitExpression_Stun");

                entity.HasOne(d => d.WeaponType)
                    .WithMany(p => p.WeaponAttacks)
                    .HasForeignKey(d => d.WeaponTypeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_WeaponAttacks_WeaponTypes");
            });

            modelBuilder.Entity<WeaponType>(entity =>
            {
                entity.HasIndex(e => e.AttackTraitId)
                    .HasDatabaseName("FK_WeaponTypes_TraitDefinitions_Attack_idx");

                entity.HasIndex(e => e.ParryTraitId)
                    .HasDatabaseName("FK_WeaponTypes_TraitDefinitions_Parry_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AttackTraitId).HasColumnType("bigint(20)");

                entity.Property(e => e.Classification).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ParryBonus).HasColumnType("double");

                entity.Property(e => e.ParryTraitId).HasColumnType("bigint(20)");

                entity.Property(e => e.Reach)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.AttackTrait)
                    .WithMany(p => p.WeaponTypesAttackTrait)
                    .HasForeignKey(d => d.AttackTraitId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_WeaponTypes_TraitDefinitions_Attack");

                entity.HasOne(d => d.ParryTrait)
                    .WithMany(p => p.WeaponTypesParryTrait)
                    .HasForeignKey(d => d.ParryTraitId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_WeaponTypes_TraitDefinitions_Parry");
            });

            modelBuilder.Entity<WearProfile>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BodyPrototypeId).HasColumnType("bigint(20)");

                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.RequireContainerIsEmpty)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'Direct'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.WearAction1st)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("'put'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.WearAction3rd)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("'puts'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.WearAffix)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("'on'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.WearStringInventory)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("'worn on'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.WearlocProfiles)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<WearableSizeParameterRule>(entity =>
            {
                entity.ToTable("WearableSizeParameterRule");
                entity.HasIndex(e => e.TraitId)
                    .HasDatabaseName("FK_WearableSizeParameterRule_TraitDefinitions");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BodyProtoId).HasColumnType("bigint(20)");

                entity.Property(e => e.HeightLinearRatios)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.IgnoreTrait)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.TraitId).HasColumnType("bigint(20)");

                entity.Property(e => e.TraitVolumeRatios)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.WeightVolumeRatios)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.Trait)
                    .WithMany(p => p.WearableSizeParameterRule)
                    .HasForeignKey(d => d.TraitId)
                    .HasConstraintName("FK_WearableSizeParameterRule_TraitDefinitions");
            });

            modelBuilder.Entity<WearableSize>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BodyPrototypeId).HasColumnType("bigint(20)");

                entity.Property(e => e.OneSizeFitsAll).HasColumnType("bit(1)");
            });

            modelBuilder.Entity<WeatherController>(entity =>
            {
                entity.HasIndex(e => e.CelestialId)
                    .HasDatabaseName("FK_WeatherControllers_Celestials_idx");

                entity.HasIndex(e => e.CurrentSeasonId)
                    .HasDatabaseName("FK_WeatherControllers_Seasons_idx");

                entity.HasIndex(e => e.CurrentWeatherEventId)
                    .HasDatabaseName("FK_WeatherControllers_WeatherEvents_idx");

                entity.HasIndex(e => e.FeedClockId)
                    .HasDatabaseName("FK_WeatherControllers_Clocks_idx");

                entity.HasIndex(e => e.FeedClockTimeZoneId)
                    .HasDatabaseName("FK_WeatherControllers_TimeZones_idx");

                entity.HasIndex(e => e.RegionalClimateId)
                    .HasDatabaseName("FK_WeatherControllers_RegionalClimates_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CelestialId).HasColumnType("bigint(20)");

                entity.Property(e => e.ConsecutiveUnchangedPeriods).HasColumnType("int(11)");

                entity.Property(e => e.CurrentSeasonId).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrentWeatherEventId).HasColumnType("bigint(20)");

                entity.Property(e => e.FeedClockId).HasColumnType("bigint(20)");

                entity.Property(e => e.FeedClockTimeZoneId).HasColumnType("bigint(20)");

                entity.Property(e => e.HighestRecentPrecipitationLevel).HasColumnType("int(11)");

                entity.Property(e => e.MinutesCounter).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PeriodsSinceHighestPrecipitation).HasColumnType("int(11)");

                entity.Property(e => e.RegionalClimateId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Celestial)
                    .WithMany(p => p.WeatherControllers)
                    .HasForeignKey(d => d.CelestialId)
                    .HasConstraintName("FK_WeatherControllers_Celestials");

                entity.HasOne(d => d.CurrentSeason)
                    .WithMany(p => p.WeatherControllers)
                    .HasForeignKey(d => d.CurrentSeasonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WeatherControllers_Seasons");

                entity.HasOne(d => d.CurrentWeatherEvent)
                    .WithMany(p => p.WeatherControllers)
                    .HasForeignKey(d => d.CurrentWeatherEventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WeatherControllers_WeatherEvents");

                entity.HasOne(d => d.FeedClock)
                    .WithMany(p => p.WeatherControllers)
                    .HasForeignKey(d => d.FeedClockId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WeatherControllers_Clocks");

                entity.HasOne(d => d.FeedClockTimeZone)
                    .WithMany(p => p.WeatherControllers)
                    .HasForeignKey(d => d.FeedClockTimeZoneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WeatherControllers_TimeZones");

                entity.HasOne(d => d.RegionalClimate)
                    .WithMany(p => p.WeatherControllers)
                    .HasForeignKey(d => d.RegionalClimateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WeatherControllers_RegionalClimates");
            });

            modelBuilder.Entity<WeatherEvent>(entity =>
            {
                entity.HasIndex(e => e.CountsAsId)
                    .HasDatabaseName("FK_WeatherEvents_WeatherEvents_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AdditionalInfo)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CountsAsId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ObscuresViewOfSky).HasColumnType("bit(1)");

                entity.Property(e => e.PermittedAtAfternoon).HasColumnType("bit(1)");

                entity.Property(e => e.PermittedAtDawn).HasColumnType("bit(1)");

                entity.Property(e => e.PermittedAtDusk).HasColumnType("bit(1)");

                entity.Property(e => e.PermittedAtMorning).HasColumnType("bit(1)");

                entity.Property(e => e.PermittedAtNight).HasColumnType("bit(1)");

                entity.Property(e => e.Precipitation).HasColumnType("int(11)");

                entity.Property(e => e.WeatherDescription)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.WeatherEventType)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.WeatherRoomAddendum)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Wind).HasColumnType("int(11)");

                entity.HasOne(d => d.CountsAs)
                    .WithMany(p => p.InverseCountsAs)
                    .HasForeignKey(d => d.CountsAsId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_WeatherEvents_WeatherEvents");
            });

            modelBuilder.Entity<WitnessProfile>(entity =>
            {
                entity.HasIndex(e => e.IdentityKnownProgId)
                    .HasDatabaseName("FK_WitnessProfiles_IdentityProg_idx");

                entity.HasIndex(e => e.ReportingMultiplierProgId)
                    .HasDatabaseName("FK_WitnessProfiles_MultiplierProg_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.IdentityKnownProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ReportingMultiplierProgId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.IdentityKnownProg)
                    .WithMany(p => p.WitnessProfilesIdentityKnownProg)
                    .HasForeignKey(d => d.IdentityKnownProgId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WitnessProfiles_IdentityProg");

                entity.HasOne(d => d.ReportingMultiplierProg)
                    .WithMany(p => p.WitnessProfilesReportingMultiplierProg)
                    .HasForeignKey(d => d.ReportingMultiplierProgId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WitnessProfiles_MultiplierProg");
            });

            modelBuilder.Entity<WitnessProfilesCooperatingAuthorities>(entity =>
            {
                entity.HasKey(e => new { e.WitnessProfileId, e.LegalAuthorityId })
                    .HasName("PRIMARY");

                entity.ToTable("WitnessProfiles_CooperatingAuthorities");

                entity.HasIndex(e => e.LegalAuthorityId)
                    .HasDatabaseName("FK_WitnessProfiles_CooperatingAuthorities_LegalAuthorities_idx");

                entity.Property(e => e.WitnessProfileId).HasColumnType("bigint(20)");

                entity.Property(e => e.LegalAuthorityId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.LegalAuthority)
                    .WithMany(p => p.WitnessProfilesCooperatingAuthorities)
                    .HasForeignKey(d => d.LegalAuthorityId)
                    .HasConstraintName("FK_WitnessProfiles_CooperatingAuthorities_LegalAuthorities");

                entity.HasOne(d => d.WitnessProfile)
                    .WithMany(p => p.WitnessProfilesCooperatingAuthorities)
                    .HasForeignKey(d => d.WitnessProfileId)
                    .HasConstraintName("FK_WitnessProfiles_CooperatingAuthorities_WitnessProfiles");
            });

            modelBuilder.Entity<WitnessProfilesIgnoredCriminalClasses>(entity =>
            {
                entity.HasKey(e => new { e.WitnessProfileId, e.LegalClassId })
                    .HasName("PRIMARY");

                entity.ToTable("WitnessProfiles_IgnoredCriminalClasses");

                entity.HasIndex(e => e.LegalClassId)
                    .HasDatabaseName("FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses_idx");

                entity.Property(e => e.WitnessProfileId).HasColumnType("bigint(20)");

                entity.Property(e => e.LegalClassId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.LegalClass)
                    .WithMany(p => p.WitnessProfilesIgnoredCriminalClasses)
                    .HasForeignKey(d => d.LegalClassId)
                    .HasConstraintName("FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses");

                entity.HasOne(d => d.WitnessProfile)
                    .WithMany(p => p.WitnessProfilesIgnoredCriminalClasses)
                    .HasForeignKey(d => d.WitnessProfileId)
                    .HasConstraintName("FK_WitnessProfiles_IgnoredCriminalClasses_WitnessProfiles");
            });

            modelBuilder.Entity<WitnessProfilesIgnoredVictimClasses>(entity =>
            {
                entity.HasKey(e => new { e.WitnessProfileId, e.LegalClassId })
                    .HasName("PRIMARY");

                entity.ToTable("WitnessProfiles_IgnoredVictimClasses");

                entity.HasIndex(e => e.LegalClassId)
                    .HasDatabaseName("FK_WitnessProfiles_IgnoredVictimClasses_LegalClasses_idx");

                entity.Property(e => e.WitnessProfileId).HasColumnType("bigint(20)");

                entity.Property(e => e.LegalClassId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.LegalClass)
                    .WithMany(p => p.WitnessProfilesIgnoredVictimClasses)
                    .HasForeignKey(d => d.LegalClassId)
                    .HasConstraintName("FK_WitnessProfiles_IgnoredVictimClasses_LegalClasses");

                entity.HasOne(d => d.WitnessProfile)
                    .WithMany(p => p.WitnessProfilesIgnoredVictimClasses)
                    .HasForeignKey(d => d.WitnessProfileId)
                    .HasConstraintName("FK_WitnessProfiles_IgnoredVictimClasses_WitnessProfiles");
            });

            modelBuilder.Entity<Wound>(entity =>
            {
                entity.HasIndex(e => e.ActorOriginId)
                    .HasDatabaseName("FK_Wounds_Characters_idx");

                entity.HasIndex(e => e.BodyId)
                    .HasDatabaseName("FK_Wounds_Bodies_idx");

                entity.HasIndex(e => e.GameItemId)
                    .HasDatabaseName("FK_Wounds_GameItemOwner_idx");

                entity.HasIndex(e => e.LodgedItemId)
                    .HasDatabaseName("FK_Wounds_GameItems_idx");

                entity.HasIndex(e => e.ToolOriginId)
                    .HasDatabaseName("FK_Wounds_GameItems_Tool_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ActorOriginId).HasColumnType("bigint(20)");

                entity.Property(e => e.BodyId).HasColumnType("bigint(20)");

                entity.Property(e => e.BodypartProtoId).HasColumnType("bigint(20)");

                entity.Property(e => e.DamageType).HasColumnType("int(11)");

                entity.Property(e => e.ExtraInformation)
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.Property(e => e.GameItemId).HasColumnType("bigint(20)");

                entity.Property(e => e.Internal)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LodgedItemId).HasColumnType("bigint(20)");

                entity.Property(e => e.ToolOriginId).HasColumnType("bigint(20)");

                entity.Property(e => e.WoundType)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_general_ci");

                entity.HasOne(d => d.ActorOrigin)
                    .WithMany(p => p.Wounds)
                    .HasForeignKey(d => d.ActorOriginId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Wounds_Characters");

                entity.HasOne(d => d.Body)
                    .WithMany(p => p.Wounds)
                    .HasForeignKey(d => d.BodyId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Wounds_Bodies");

                entity.HasOne(d => d.GameItem)
                    .WithMany(p => p.WoundsGameItem)
                    .HasForeignKey(d => d.GameItemId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Wounds_GameItemOwner");

                entity.HasOne(d => d.LodgedItem)
                    .WithMany(p => p.WoundsLodgedItem)
                    .HasForeignKey(d => d.LodgedItemId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Wounds_GameItems");

                entity.HasOne(d => d.ToolOrigin)
                    .WithMany(p => p.WoundsToolOrigin)
                    .HasForeignKey(d => d.ToolOriginId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Wounds_GameItems_Tool");
            });

            modelBuilder.Entity<Writing>(entity =>
            {
                entity.HasIndex(e => e.AuthorId)
                    .HasDatabaseName("FK_Writings_Characters_Author_idx");

                entity.HasIndex(e => e.LanguageId)
                    .HasDatabaseName("FK_Writings_Languages_idx");

                entity.HasIndex(e => e.ScriptId)
                    .HasDatabaseName("FK_Writings_Scripts_idx");

                entity.HasIndex(e => e.TrueAuthorId)
                    .HasDatabaseName("FK_Writings_Characters_TrueAuthor_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AuthorId).HasColumnType("bigint(20)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ImplementType).HasColumnType("int(11)");

                entity.Property(e => e.LanguageId).HasColumnType("bigint(20)");

                entity.Property(e => e.ScriptId).HasColumnType("bigint(20)");

                entity.Property(e => e.Style).HasColumnType("int(11)");

                entity.Property(e => e.TrueAuthorId).HasColumnType("bigint(20)");

                entity.Property(e => e.WritingColour).HasColumnType("bigint(20)");

                entity.Property(e => e.WritingType)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.WritingsAuthor)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Writings_Characters_Author");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.Writings)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Writings_Languages");

                entity.HasOne(d => d.Script)
                    .WithMany(p => p.Writings)
                    .HasForeignKey(d => d.ScriptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Writings_Scripts");

                entity.HasOne(d => d.TrueAuthor)
                    .WithMany(p => p.WritingsTrueAuthor)
                    .HasForeignKey(d => d.TrueAuthorId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Writings_Characters_TrueAuthor");
            });

            modelBuilder.Entity<Zone>(entity =>
            {
                entity.HasIndex(e => e.DefaultCellId)
                    .HasDatabaseName("FK_Zones_Cells");

                entity.HasIndex(e => e.ShardId)
                    .HasDatabaseName("FK_Zones_Shards");

                entity.HasIndex(e => e.WeatherControllerId)
                    .HasDatabaseName("FK_Zones_WeatherControllers_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.DefaultCellId).HasColumnType("bigint(20)");

                entity.Property(e => e.ForagableProfileId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ShardId).HasColumnType("bigint(20)");

                entity.Property(e => e.WeatherControllerId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.DefaultCell)
                    .WithMany(p => p.Zones)
                    .HasForeignKey(d => d.DefaultCellId)
                    .HasConstraintName("FK_Zones_Cells");

                entity.HasOne(d => d.Shard)
                    .WithMany(p => p.Zones)
                    .HasForeignKey(d => d.ShardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Zones_Shards");

                entity.HasOne(d => d.WeatherController)
                    .WithMany(p => p.Zones)
                    .HasForeignKey(d => d.WeatherControllerId)
                    .HasConstraintName("FK_Zones_WeatherControllers");
            });

            modelBuilder.Entity<ZonesTimezones>(entity =>
            {
                entity.HasKey(e => new { e.ZoneId, e.ClockId, e.TimezoneId })
                    .HasName("PRIMARY");

                entity.ToTable("Zones_Timezones");

                entity.Property(e => e.ZoneId).HasColumnType("bigint(20)");

                entity.Property(e => e.ClockId).HasColumnType("bigint(20)");

                entity.Property(e => e.TimezoneId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Zone)
                    .WithMany(p => p.ZonesTimezones)
                    .HasForeignKey(d => d.ZoneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Zones_Timezones_Zones");
            });

            modelBuilder.Entity<WeeklyStatistic>(entity =>
            {
                entity.ToTable("WeeklyStatistics");
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.Start).HasColumnType("datetime");
                entity.Property(e => e.End).HasColumnType("datetime");
                entity.Property(e => e.TotalAccounts).HasColumnType("int(11)");
                entity.Property(e => e.ActiveAccounts).HasColumnType("int(11)");
                entity.Property(e => e.NewAccounts).HasColumnType("int(11)");
                entity.Property(e => e.ApplicationsSubmitted).HasColumnType("int(11)");
                entity.Property(e => e.ApplicationsApproved).HasColumnType("int(11)");
                entity.Property(e => e.PlayerDeaths).HasColumnType("int(11)");
                entity.Property(e => e.NonPlayerDeaths).HasColumnType("int(11)");
            });

            modelBuilder.Entity<PlayerActivitySnapshot>(entity =>
            {
                entity.ToTable("PlayerActivitySnapshots");
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.DateTime).HasColumnType("datetime");
                entity.Property(e => e.OnlinePlayers).HasColumnType("int(11)");
                entity.Property(e => e.OnlineAdmins).HasColumnType("int(11)");
                entity.Property(e => e.AvailableAdmins).HasColumnType("int(11)");
                entity.Property(e => e.IdlePlayers).HasColumnType("int(11)");
                entity.Property(e => e.UniquePCLocations).HasColumnType("int(11)");
                entity.Property(e => e.OnlineGuests).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Bank>(entity =>
            {
                entity.ToTable("Banks");
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");
                entity.Property(e => e.PrimaryCurrencyId).HasColumnType("bigint(20)");
                entity.Property(e => e.MaximumBankAccountsPerCustomer).HasColumnType("int(11)");

                entity
                    .HasOne(e => e.EconomicZone)
                    .WithMany()
                    .HasForeignKey(e => e.EconomicZoneId)
                    .HasConstraintName("FK_Banks_EconomicZones");

                entity
                    .HasOne(e => e.PrimaryCurrency)
                    .WithMany()
                    .HasForeignKey(e => e.PrimaryCurrencyId)
                    .HasConstraintName("FK_Banks_Currencies");
            });

            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.ToTable("BankAccounts");
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.AccountCreationDate)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CurrentBalance).HasColumnType("decimal(58,29)");
                entity.Property(e => e.CurrentMonthInterest).HasColumnType("decimal(58,29)");
                entity.Property(e => e.CurrentMonthFees).HasColumnType("decimal(58,29)");

                entity.Property(e => e.AccountStatus).HasColumnType("int(11)");
                entity.Property(e => e.AccountNumber).HasColumnType("int(11)");

                entity.Property(e => e.BankId).HasColumnType("bigint(20)");
                entity.Property(e => e.BankAccountTypeId).HasColumnType("bigint(20)");
                entity.Property(e => e.AccountOwnerCharacterId).HasColumnType("bigint(20)");
                entity.Property(e => e.AccountOwnerClanId).HasColumnType("bigint(20)");
                entity.Property(e => e.AccountOwnerShopId).HasColumnType("bigint(20)");
                entity.Property(e => e.NominatedBenefactorAccountId).HasColumnType("bigint(20)");

                entity
                    .HasOne(e => e.Bank)
                    .WithMany(e => e.BankAccounts)
                    .HasForeignKey(e => e.BankId)
                    .HasConstraintName("FK_BankAccounts_Banks")
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(e => e.BankAccountType)
                    .WithMany()
                    .HasForeignKey(e => e.BankAccountTypeId)
                    .HasConstraintName("FK_BankAccounts_BankAccountTypes")
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(e => e.AccountOwnerCharacter)
                    .WithMany()
                    .HasForeignKey(e => e.AccountOwnerCharacterId)
                    .HasConstraintName("FK_BankAccounts_Characters")
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(e => e.AccountOwnerClan)
                    .WithMany()
                    .HasForeignKey(e => e.AccountOwnerClanId)
                    .HasConstraintName("FK_BankAccounts_Clans")
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(e => e.AccountOwnerShop)
                    .WithMany()
                    .HasForeignKey(e => e.AccountOwnerShopId)
                    .HasConstraintName("FK_BankAccounts_Shops")
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(e => e.NominatedBenefactorAccount)
                    .WithMany()
                    .HasForeignKey(e => e.NominatedBenefactorAccountId)
                    .HasConstraintName("FK_BankAccounts_BankAccounts")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BankAccountType>(entity =>
            {
                entity.ToTable("BankAccountTypes");
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.CustomerDescription)
                    .IsRequired()
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.BankId).HasColumnType("bigint(20)");
                entity.Property(e => e.CanOpenAccountProgCharacterId).HasColumnType("bigint(20)");
                entity.Property(e => e.CanOpenAccountProgClanId).HasColumnType("bigint(20)");
                entity.Property(e => e.CanOpenAccountProgShopId).HasColumnType("bigint(20)");
                entity.Property(e => e.CanCloseAccountProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.MaximumOverdrawAmount).HasColumnType("decimal(58,29)");
                entity.Property(e => e.WithdrawalFleeFlat).HasColumnType("decimal(58,29)");
                entity.Property(e => e.WithdrawalFleeRate).HasColumnType("decimal(58,29)");
                entity.Property(e => e.DepositFeeFlat).HasColumnType("decimal(58,29)");
                entity.Property(e => e.DepositFeeRate).HasColumnType("decimal(58,29)");
                entity.Property(e => e.TransferFeeFlat).HasColumnType("decimal(58,29)");
                entity.Property(e => e.TransferFeeRate).HasColumnType("decimal(58,29)");
                entity.Property(e => e.TransferFeeOtherBankFlat).HasColumnType("decimal(58,29)");
                entity.Property(e => e.TransferFeeOtherBankRate).HasColumnType("decimal(58,29)");
                entity.Property(e => e.DailyFee).HasColumnType("decimal(58,29)");
                entity.Property(e => e.DailyInterestRate).HasColumnType("decimal(58,29)");
                entity.Property(e => e.OverdrawFeeFlat).HasColumnType("decimal(58,29)");
                entity.Property(e => e.OverdrawFeeRate).HasColumnType("decimal(58,29)");
                entity.Property(e => e.DailyOverdrawnFee).HasColumnType("decimal(58,29)");
                entity.Property(e => e.DailyOverdrawnInterestRate).HasColumnType("decimal(58,29)");

                entity
                    .HasOne(e => e.Bank)
                    .WithMany(e => e.BankAccountTypes)
                    .HasForeignKey(e => e.BankId)
                    .HasConstraintName("FK_BankAccountTypes_Banks")
                    .OnDelete(DeleteBehavior.Cascade); ;
                entity
                    .HasOne(e => e.CanOpenAccountProgCharacter)
                    .WithMany()
                    .HasForeignKey(e => e.CanOpenAccountProgCharacterId)
                    .HasConstraintName("FK_BankAccountTypes_CharacterProgs")
                    .OnDelete(DeleteBehavior.Cascade); ;
                entity
                    .HasOne(e => e.CanOpenAccountProgClan)
                    .WithMany()
                    .HasForeignKey(e => e.CanOpenAccountProgClanId)
                    .HasConstraintName("FK_BankAccountTypes_ClanProgs")
                    .OnDelete(DeleteBehavior.Cascade); ;
                entity
                    .HasOne(e => e.CanOpenAccountProgShop)
                    .WithMany()
                    .HasForeignKey(e => e.CanOpenAccountProgShopId)
                    .HasConstraintName("FK_BankAccountTypes_ShopProgs")
                    .OnDelete(DeleteBehavior.Cascade); ;
                entity
                    .HasOne(e => e.CanCloseAccountProg)
                    .WithMany()
                    .HasForeignKey(e => e.CanCloseAccountProgId)
                    .HasConstraintName("FK_BankAccountTypes_CanCloseProg")
                    .OnDelete(DeleteBehavior.Cascade); ;
            });

            modelBuilder.Entity<BankAccountTransaction>(entity =>
            {
                entity.ToTable("BankAccountTransactions");
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.BankAccountId).HasColumnType("bigint(20)");
                entity.Property(e => e.TransactionType).HasColumnType("int(11)");
                entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");
                entity.Property(e => e.AccountBalanceAfter).HasColumnType("decimal(58,29)");
                entity.Property(e => e.TransactionDescription)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.TransactionTime)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity
                    .HasOne(e => e.BankAccount)
                    .WithMany(e => e.BankAccountTransactions)
                    .HasForeignKey(e => e.BankAccountId)
                    .HasConstraintName("FK_BankAccountTransactions_BankAccounts")
                    .OnDelete(DeleteBehavior.Cascade); ;
            });

            modelBuilder.Entity<BankExchangeRate>(entity =>
            {
                entity.ToTable("BankExchangeRates");
                entity.HasKey(e =>
                    new { e.BankId, e.FromCurrencyId, e.ToCurrencyId }
                    ).HasName("PRIMARY");

                entity.Property(e => e.BankId).HasColumnType("bigint(20)");
                entity.Property(e => e.FromCurrencyId).HasColumnType("bigint(20)");
                entity.Property(e => e.ToCurrencyId).HasColumnType("bigint(20)");
                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(58,29)");

                entity
                    .HasOne(e => e.Bank)
                    .WithMany(e => e.BankExchangeRates)
                    .HasForeignKey(e => e.BankId)
                    .HasConstraintName("FK_BankExchangeRates_Banks")
                    .OnDelete(DeleteBehavior.Cascade); ;
                entity
                    .HasOne(e => e.FromCurrency)
                    .WithMany()
                    .HasForeignKey(e => e.FromCurrencyId)
                    .HasConstraintName("FK_BankExchangeRates_Currencies_From")
                    .OnDelete(DeleteBehavior.Cascade); ;
                entity
                    .HasOne(e => e.ToCurrency)
                    .WithMany()
                    .HasForeignKey(e => e.ToCurrencyId)
                    .HasConstraintName("FK_BankExchangeRates_Currencies_To")
                    .OnDelete(DeleteBehavior.Cascade); ;
            });

            modelBuilder.Entity<BankBranch>(entity =>
            {
                entity.ToTable("BankBranches");
                entity.HasKey(e =>
                    new { e.BankId, e.CellId }
                ).HasName("PRIMARY");

                entity.Property(e => e.BankId).HasColumnType("bigint(20)");
                entity.Property(e => e.CellId).HasColumnType("bigint(20)");

                entity
                    .HasOne(e => e.Bank)
                    .WithMany(e => e.BankBranches)
                    .HasForeignKey(e => e.BankId)
                    .HasConstraintName("FK_BankBranches_Banks")
                    .OnDelete(DeleteBehavior.Cascade); ;
                entity
                    .HasOne(e => e.Cell)
                    .WithMany()
                    .HasForeignKey(e => e.CellId)
                    .HasConstraintName("FK_BankBranches_Cells")
                    .OnDelete(DeleteBehavior.Cascade); ;
            });

            modelBuilder.Entity<BankCurrencyReserve>(entity =>
            {
                entity.ToTable("BankCurrencyReserves");
                entity.HasKey(e =>
                    new { e.BankId, e.CurrencyId }
                ).HasName("PRIMARY");

                entity.Property(e => e.BankId).HasColumnType("bigint(20)");
                entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");
                entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");

                entity
                    .HasOne(e => e.Bank)
                    .WithMany(e => e.BankCurrencyReserves)
                    .HasForeignKey(e => e.BankId)
                    .HasConstraintName("FK_BankCurrencyReserves_Banks")
                    .OnDelete(DeleteBehavior.Cascade); ;
                entity
                    .HasOne(e => e.Currency)
                    .WithMany()
                    .HasForeignKey(e => e.CurrencyId)
                    .HasConstraintName("FK_BankCurrencyReserves_Currencies")
                    .OnDelete(DeleteBehavior.Cascade); ;
            });

            modelBuilder.Entity<BankManager>(entity => {
                entity.ToTable("BankManagers");
                entity.HasKey(e => new { e.BankId, e.CharacterId }).HasName("PRIMARY");

                entity.Property(e => e.BankId).HasColumnType("bigint(20)");
                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

                entity
                    .HasOne(e => e.Bank)
                    .WithMany(e => e.BankManagers)
                    .HasForeignKey(e => e.BankId)
                    .HasConstraintName("FK_BankManagers_Banks")
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(e => e.Character)
                    .WithMany()
                    .HasForeignKey(e => e.CharacterId)
                    .HasConstraintName("FK_BankManagers_Characters")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BankManagerAuditLog>(entity => {
                entity.ToTable("BankManagerAuditLogs");
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.BankId).HasColumnType("bigint(20)");
                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");
                entity.Property(e => e.DateTime)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.Detail)
                    .IsRequired()
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity
                    .HasOne(e => e.Bank)
                    .WithMany(e => e.BankManagerAuditLogs)
                    .HasForeignKey(e => e.BankId)
                    .HasConstraintName("FK_BankManagerAuditLogs_Banks")
                    .OnDelete(DeleteBehavior.Cascade); ;

                entity
                    .HasOne(e => e.Character)
                    .WithMany()
                    .HasForeignKey(e => e.CharacterId)
                    .HasConstraintName("FK_BankManagerAuditLogs_Characters");
            });
        }

    }
}
