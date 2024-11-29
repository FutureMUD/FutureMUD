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
		protected static void OnModelCreatingTwo(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ChargenScreenStoryboard>(entity => {
				entity.ToTable("ChargenScreenStoryboards");
				entity.Property(e => e.Id).HasColumnType("bigint(20)");
				entity.Property(e => e.Order).HasColumnType("int(11)");
				entity.Property(e => e.ChargenStage).HasColumnType("int(11)");
				entity.Property(e => e.ChargenType).HasColumnType("varchar(50)");
				entity.Property(e => e.NextStage).HasColumnType("int(11)");
				entity.Property(e => e.StageDefinition)
					.IsRequired()
					.IsRequired()
					.HasColumnType("mediumtext")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
			});

			modelBuilder.Entity<ChargenScreenStoryboardDependentStage>(entity => {
				entity.ToTable("ChargenScreenStoryboardDependentStages");
				entity
					.Property(e => e.OwnerId)
					.HasColumnType("bigint(20)");
				entity
					.Property(e => e.Dependency)
					.HasColumnType("int(11)");
				entity
					.HasKey(e => new { e.OwnerId, e.Dependency })
					.HasName("PRIMARY");
				entity
					.HasIndex(e => e.OwnerId)
					.HasDatabaseName("FK_ChargenScreenStoryboardDependentStages_Owner");
				entity
					.HasOne(e => e.Owner)
					.WithMany(e => e.DependentStages)
					.HasForeignKey(e => e.OwnerId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_ChargenScreenStoryboardDependentStages_Owner");
			});

			modelBuilder.Entity<CheckTemplateDifficulty>(entity =>
			{
				entity.HasKey(e => new { e.Difficulty, e.CheckTemplateId })
					.HasName("PRIMARY");

				entity.HasIndex(e => e.CheckTemplateId)
					.HasDatabaseName("FK_CheckTemplateDifficulties_CheckTemplates");

				entity.Property(e => e.Difficulty).HasColumnType("int(11)");

				entity.Property(e => e.CheckTemplateId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.CheckTemplate)
					.WithMany(p => p.CheckTemplateDifficulties)
					.HasForeignKey(d => d.CheckTemplateId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_CheckTemplateDifficulties_CheckTemplates");
			});

			modelBuilder.Entity<CheckTemplate>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.CanBranchIfTraitMissing)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.CheckMethod)
					.IsRequired()
					.HasColumnType("varchar(25)")
					.HasDefaultValueSql("'Standard'")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Definition)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.FailIfTraitMissingMode).HasColumnType("smallint(6)");

				entity.Property(e => e.ImproveTraits)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
			});

			modelBuilder.Entity<Check>(entity =>
			{
				entity.HasKey(e => e.Type)
					.HasName("PRIMARY")
					;

				entity.HasIndex(e => e.CheckTemplateId)
					.HasDatabaseName("FK_Checks_CheckTemplates");

				entity.HasIndex(e => e.TraitExpressionId)
					.HasDatabaseName("FK_Checks_TraitExpression");

				entity.Property(e => e.Type)
					.HasColumnType("int(11)")
					.ValueGeneratedNever()
					;

				entity.Property(e => e.CheckTemplateId).HasColumnType("bigint(20)");

				entity.Property(e => e.MaximumDifficultyForImprovement)
					.HasColumnType("int(11)")
					.HasDefaultValueSql("'10'");

				entity.Property(e => e.TraitExpressionId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.CheckTemplate)
					.WithMany(p => p.Checks)
					.HasForeignKey(d => d.CheckTemplateId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Checks_CheckTemplates");

				entity.HasOne(d => d.TraitExpression)
					.WithMany(p => p.Checks)
					.HasForeignKey(d => d.TraitExpressionId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Checks_TraitExpression");
			});

			modelBuilder.Entity<ClanMembership>(entity =>
			{
				entity.HasKey(e => new { e.ClanId, e.CharacterId })
					.HasName("PRIMARY");

				entity.HasIndex(e => e.CharacterId)
					.HasDatabaseName("FK_ClanMemberships_Characters");

				entity.HasIndex(e => e.ManagerId)
					.HasDatabaseName("FK_ClanMemberships_Manager");

				entity.Property(e => e.ClanId).HasColumnType("bigint(20)");

				entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

				entity.Property(e => e.ArchivedMembership).HasColumnType("bit(1)").HasDefaultValue(false);

				entity.Property(e => e.JoinDate)
					.IsRequired()
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.ManagerId).HasColumnType("bigint(20)");

				entity.Property(e => e.PaygradeId).HasColumnType("bigint(20)");

				entity.Property(e => e.PersonalName)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.RankId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.Character)
					.WithMany(p => p.ClanMembershipsCharacter)
					.HasForeignKey(d => d.CharacterId)
					.HasConstraintName("FK_ClanMemberships_Characters");

				entity.HasOne(d => d.Clan)
					.WithMany(p => p.ClanMemberships)
					.HasForeignKey(d => d.ClanId)
					.HasConstraintName("FK_ClanMemberships_Clans");

				entity.HasOne(d => d.Manager)
					.WithMany(p => p.ClanMembershipsManager)
					.HasForeignKey(d => d.ManagerId)
					.HasConstraintName("FK_ClanMemberships_Manager");
			});

			modelBuilder.Entity<ClanMembershipsAppointments>(entity =>
			{
				entity.HasKey(e => new { e.ClanId, e.CharacterId, e.AppointmentId })
					.HasName("PRIMARY");

				entity.ToTable("ClanMemberships_Appointments");

				entity.HasIndex(e => e.AppointmentId)
					.HasDatabaseName("FK_ClanMemberships_Appointments_Appointments");

				entity.Property(e => e.ClanId).HasColumnType("bigint(20)");

				entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

				entity.Property(e => e.AppointmentId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.Appointment)
					.WithMany(p => p.ClanMembershipsAppointments)
					.HasForeignKey(d => d.AppointmentId)
					.HasConstraintName("FK_ClanMemberships_Appointments_Appointments");

				entity.HasOne(d => d.ClanMembership)
					.WithMany(p => p.ClanMembershipsAppointments)
					.HasForeignKey(d => new { d.ClanId, d.CharacterId })
					.HasConstraintName("FK_ClanMemberships_Appointments_ClanMemberships");
			});

			modelBuilder.Entity<ClanMembershipBackpay>(entity =>
			{
				entity.HasKey(e => new { e.CurrencyId, e.ClanId, e.CharacterId })
					.HasName("PRIMARY");

				entity.ToTable("ClanMemberships_Backpay");

				entity.HasIndex(e => new { e.ClanId, e.CharacterId })
					.HasDatabaseName("FK_ClanMemberships_Backpay_ClanMemberships");

				entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");

				entity.Property(e => e.ClanId).HasColumnType("bigint(20)");

				entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

				entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");

				entity.HasOne(d => d.Currency)
					.WithMany(p => p.ClanMembershipsBackpay)
					.HasForeignKey(d => d.CurrencyId)
					.HasConstraintName("FK_ClanMemberships_Backpay_Currencies");

				entity.HasOne(d => d.C)
					.WithMany(p => p.ClanMembershipsBackpay)
					.HasForeignKey(d => new { d.ClanId, d.CharacterId })
					.HasConstraintName("FK_ClanMemberships_Backpay_ClanMemberships");
			});

			modelBuilder.Entity<Clan>(entity =>
			{
				entity.HasIndex(e => e.CalendarId)
					.HasDatabaseName("FK_Clans_Calendars");

				entity.HasIndex(e => e.OnPayProgId)
					.HasDatabaseName("FK_Clans_FutureProgs_idx");

				entity.HasIndex(e => e.PaymasterId)
					.HasDatabaseName("FK_Clans_Characters_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Alias)
					.IsRequired()
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.CalendarId).HasColumnType("bigint(20)");

				entity.Property(e => e.Description)
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.FullName)
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.IsTemplate)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.ShowFamousMembersInNotables)
				.HasColumnType("bit(1)")
				.HasDefaultValueSql("b'0'");

				entity.Property(e => e.MaximumPeriodsOfUncollectedBackPay).HasColumnType("int(11)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Sphere)
				.HasColumnType("varchar(100)")
				.HasCharSet("utf8")
				.UseCollation("utf8_general_ci");

				entity.Property(e => e.OnPayProgId).HasColumnType("bigint(20)");
				entity.Property(e => e.BankAccountId).HasColumnType("bigint(20)");
				entity.Property(e => e.DiscordChannelId).HasColumnType("decimal(20,0)");
				entity.Property(e => e.PayIntervalModifier).HasColumnType("int(11)");

				entity.Property(e => e.PayIntervalOther).HasColumnType("int(11)");

				entity.Property(e => e.PayIntervalReferenceDate)
					.IsRequired()
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PayIntervalReferenceTime)
					.IsRequired()
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PayIntervalType).HasColumnType("int(11)");

				entity.Property(e => e.PaymasterId).HasColumnType("bigint(20)");

				entity.Property(e => e.PaymasterItemProtoId).HasColumnType("bigint(20)");

				entity.Property(e => e.ShowClanMembersInWho)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.HasOne(d => d.Calendar)
					.WithMany(p => p.Clans)
					.HasForeignKey(d => d.CalendarId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Clans_Calendars");

				entity.HasOne(d => d.OnPayProg)
					.WithMany(p => p.Clans)
					.HasForeignKey(d => d.OnPayProgId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Clans_FutureProgs");

				entity.HasOne(d => d.Paymaster)
					.WithMany(p => p.Clans)
					.HasForeignKey(d => d.PaymasterId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Clans_Characters");

				entity.HasOne(d => d.BankAccount)
					.WithMany()
					.HasForeignKey(d => d.BankAccountId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Clans_BankAccounts");
			});

			modelBuilder.Entity<ClanAdministrationCell>(entity =>
			{
				entity.HasKey(e => new { e.ClanId, e.CellId })
					.HasName("PRIMARY");

				entity.ToTable("Clans_AdministrationCells");

				entity.HasIndex(e => e.CellId)
					.HasDatabaseName("FK_Clans_AdministrationCells_Cells_idx");

				entity.Property(e => e.ClanId).HasColumnType("bigint(20)");

				entity.Property(e => e.CellId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.Cell)
					.WithMany(p => p.ClansAdministrationCells)
					.HasForeignKey(d => d.CellId)
					.HasConstraintName("FK_Clans_AdministrationCells_Cells");

				entity.HasOne(d => d.Clan)
					.WithMany(p => p.ClansAdministrationCells)
					.HasForeignKey(d => d.ClanId)
					.HasConstraintName("FK_Clans_AdministrationCells_Clans");
			});

			modelBuilder.Entity<ClanTreasuryCell>(entity =>
			{
				entity.HasKey(e => new { e.ClanId, e.CellId })
					.HasName("PRIMARY");

				entity.ToTable("Clans_TreasuryCells");

				entity.HasIndex(e => e.CellId)
					.HasDatabaseName("FK_Clans_TreasuryCells_Cells_idx");

				entity.Property(e => e.ClanId).HasColumnType("bigint(20)");

				entity.Property(e => e.CellId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.Cell)
					.WithMany(p => p.ClansTreasuryCells)
					.HasForeignKey(d => d.CellId)
					.HasConstraintName("FK_Clans_TreasuryCells_Cells");

				entity.HasOne(d => d.Clan)
					.WithMany(p => p.ClansTreasuryCells)
					.HasForeignKey(d => d.ClanId)
					.HasConstraintName("FK_Clans_TreasuryCells_Clans");
			});

			modelBuilder.Entity<ClimateModel>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.MinimumMinutesBetweenFlavourEchoes).HasColumnType("int(11)");

				entity.Property(e => e.MinuteProcessingInterval).HasColumnType("int(11)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Type)
					.IsRequired()
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
			});

			modelBuilder.Entity<ClimateModelSeason>(entity => {
				entity.HasKey(e => new { e.ClimateModelId, e.SeasonId })
					.HasName("PRIMARY");

				entity.Property(e => e.ClimateModelId).HasColumnType("bigint(20)");
				entity.Property(e => e.SeasonId).HasColumnType("bigint(20)");
				entity.Property(e => e.IncrementalAdditionalChangeChanceFromStableWeather).HasColumnType("double");
				entity.Property(e => e.MaximumAdditionalChangeChanceFromStableWeather).HasColumnType("double");

				entity.HasOne(e => e.ClimateModel)
				.WithMany(x => x.ClimateModelSeasons)
				.HasForeignKey(e => e.ClimateModelId)
				.HasConstraintName("FK_ClimateModelSeasons_ClimateModels");

				entity.HasOne(e => e.Season)
				.WithMany(x => x.ClimateModelSeasons)
				.HasForeignKey(e => e.SeasonId)
				.HasConstraintName("FK_ClimateModelSeasons_Seasons");
			});

			modelBuilder.Entity<ClimateModelSeasonEvent>(entity => {
				entity.HasKey(e => new { e.ClimateModelId, e.SeasonId, e.WeatherEventId })
					.HasName("PRIMARY");
				entity.Property(e => e.ClimateModelId).HasColumnType("bigint(20)");
				entity.Property(e => e.SeasonId).HasColumnType("bigint(20)");
				entity.Property(e => e.WeatherEventId).HasColumnType("bigint(20)");
				entity.Property(e => e.ChangeChance).HasColumnType("double");

				entity.HasOne(e => e.ClimateModel)
				.WithMany()
				.HasForeignKey(e => e.ClimateModelId)
				.HasConstraintName("FK_ClimateModelSeasonEvents_ClimateModels");

				entity.HasOne(e => e.Season)
				.WithMany()
				.HasForeignKey(e => e.SeasonId)
				.HasConstraintName("FK_ClimateModelSeasonEvents_Seasons");

				entity.HasOne(e => e.WeatherEvent)
				.WithMany()
				.HasForeignKey(e => e.WeatherEventId)
				.HasConstraintName("FK_ClimateModelSeasonEvents_WeatherEvents");

				entity.Property(e => e.Transitions)
				   .IsRequired()
				   .HasColumnType("mediumtext")
				   .HasCharSet("utf8")
				   .UseCollation("utf8_general_ci");
			});

			modelBuilder.Entity<Clock>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Hours).HasColumnType("int(11)");

				entity.Property(e => e.Minutes).HasColumnType("int(11)");

				entity.Property(e => e.PrimaryTimezoneId).HasColumnType("bigint(20)");

				entity.Property(e => e.Seconds).HasColumnType("int(11)");
			});

			modelBuilder.Entity<Coin>(entity =>
			{
				entity.HasIndex(e => e.CurrencyId)
					.HasDatabaseName("FK_Coins_Currencies");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");

				entity.Property(e => e.UseForChange)
					  .HasColumnType("bit(1)")
					  .HasDefaultValue(true)
					  ;


				entity.Property(e => e.FullDescription)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.GeneralForm)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PluralWord)
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.ShortDescription)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Value).HasColumnType("decimal(58,29)");

				entity.HasOne(d => d.Currency)
					.WithMany(p => p.Coins)
					.HasForeignKey(d => d.CurrencyId)
					.HasConstraintName("FK_Coins_Currencies");
			});

			modelBuilder.Entity<Colour>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Basic).HasColumnType("int(11)");

				entity.Property(e => e.Blue).HasColumnType("int(11)");

				entity.Property(e => e.Fancy)
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Green).HasColumnType("int(11)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(50)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Red).HasColumnType("int(11)");
			});

			modelBuilder.Entity<CombatMessage>(entity =>
			{
				entity.HasIndex(e => e.ProgId)
					.HasDatabaseName("FK_CombatMessages_FutureProgs_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Chance).HasDefaultValueSql("'1'");

				entity.Property(e => e.FailureMessage)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.Message)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.Outcome).HasColumnType("int(11)");

				entity.Property(e => e.Priority).HasColumnType("int(11)");

				entity.Property(e => e.ProgId).HasColumnType("bigint(20)");
				entity.Property(e => e.AuxiliaryProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.Type).HasColumnType("int(11)");

				entity.Property(e => e.Verb).HasColumnType("int(11)");

				entity.HasOne(d => d.Prog)
					.WithMany(p => p.CombatMessages)
					.HasForeignKey(d => d.ProgId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_CombatMessages_FutureProgs");
				entity.HasOne(d => d.AuxiliaryProg)
					  .WithMany()
					  .HasForeignKey(d => d.AuxiliaryProgId)
					  .OnDelete(DeleteBehavior.SetNull)
					  .HasConstraintName("FK_CombatMessages_FutureProgs_Auxiliary");
			});

			modelBuilder.Entity<CombatMessagesCombatActions>(entity =>
			{
				entity.HasKey(e => new { e.CombatMessageId, e.CombatActionId })
					  .HasName("PRIMARY");

				entity.ToTable("CombatMessages_CombatActions");

				entity.HasIndex(e => e.CombatActionId)
					  .HasDatabaseName("FK_CombatMessages_CombatActions_WeaponAttacks_idx");

				entity.Property(e => e.CombatMessageId).HasColumnType("bigint(20)");

				entity.Property(e => e.CombatActionId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.CombatMessage)
					  .WithMany(p => p.CombatMessagesCombatActions)
					  .HasForeignKey(d => d.CombatMessageId)
					  .HasConstraintName("FK_CombatMessages_CombatActions_CombatMessages");

				entity.HasOne(d => d.CombatAction)
					  .WithMany(p => p.CombatMessagesCombatActions)
					  .HasForeignKey(d => d.CombatActionId)
					  .HasConstraintName("FK_CombatMessages_CombatActions_WeaponAttacks");
			});

			modelBuilder.Entity<CombatMessagesWeaponAttacks>(entity =>
			{
				entity.HasKey(e => new { e.CombatMessageId, e.WeaponAttackId })
					.HasName("PRIMARY");

				entity.ToTable("CombatMessages_WeaponAttacks");

				entity.HasIndex(e => e.WeaponAttackId)
					.HasDatabaseName("FK_CombatMessages_WeaponAttacks_WeaponAttacks_idx");

				entity.Property(e => e.CombatMessageId).HasColumnType("bigint(20)");

				entity.Property(e => e.WeaponAttackId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.CombatMessage)
					.WithMany(p => p.CombatMessagesWeaponAttacks)
					.HasForeignKey(d => d.CombatMessageId)
					.HasConstraintName("FK_CombatMessages_WeaponAttacks_CombatMessages");

				entity.HasOne(d => d.WeaponAttack)
					.WithMany(p => p.CombatMessagesWeaponAttacks)
					.HasForeignKey(d => d.WeaponAttackId)
					.HasConstraintName("FK_CombatMessages_WeaponAttacks_WeaponAttacks");
			});

			modelBuilder.Entity<CorpseModel>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Definition)
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.Description)
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.Name)
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.Type)
					.IsRequired()
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");
			});

			modelBuilder.Entity<CraftInput>(entity =>
			{
				entity.HasIndex(e => new { e.CraftId, e.CraftRevisionNumber })
					.HasDatabaseName("FK_CraftInputs_Crafts_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.CraftId).HasColumnType("bigint(20)");

				entity.Property(e => e.CraftRevisionNumber).HasColumnType("int(11)");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.InputType)
					.IsRequired()
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.OriginalAdditionTime).HasColumnType("datetime");

				entity.HasOne(d => d.Craft)
					.WithMany(p => p.CraftInputs)
					.HasForeignKey(d => new { d.CraftId, d.CraftRevisionNumber })
					.HasConstraintName("FK_CraftInputs_Crafts");
			});

			modelBuilder.Entity<CraftPhase>(entity =>
			{
				entity.HasKey(e => new { e.CraftPhaseId, e.CraftPhaseRevisionNumber, e.PhaseNumber })
					.HasName("PRIMARY");

				entity.Property(e => e.CraftPhaseId).HasColumnType("bigint(20)");

				entity.Property(e => e.CraftPhaseRevisionNumber).HasColumnType("int(11)");

				entity.Property(e => e.PhaseNumber).HasColumnType("int(11)");

				entity.Property(e => e.Echo)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.FailEcho)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.Craft)
					.WithMany(p => p.CraftPhases)
					.HasForeignKey(d => new { d.CraftPhaseId, d.CraftPhaseRevisionNumber })
					.HasConstraintName("FK_CraftPhases_Crafts");
			});

			modelBuilder.Entity<CraftProduct>(entity =>
			{
				entity.HasIndex(e => new { e.CraftId, e.CraftRevisionNumber })
					.HasDatabaseName("FK_CraftProducts_Crafts_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.CraftId).HasColumnType("bigint(20)");

				entity.Property(e => e.CraftRevisionNumber).HasColumnType("int(11)");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.IsFailProduct)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.MaterialDefiningInputIndex).HasColumnType("int(11)");

				entity.Property(e => e.OriginalAdditionTime).HasColumnType("datetime");

				entity.Property(e => e.ProductType)
					.IsRequired()
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.Craft)
					.WithMany(p => p.CraftProducts)
					.HasForeignKey(d => new { d.CraftId, d.CraftRevisionNumber })
					.HasConstraintName("FK_CraftProducts_Crafts");
			});

			modelBuilder.Entity<CraftTool>(entity =>
			{
				entity.HasIndex(e => new { e.CraftId, e.CraftRevisionNumber })
					.HasDatabaseName("FK_CraftTools_Crafts_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.CraftId).HasColumnType("bigint(20)");

				entity.Property(e => e.CraftRevisionNumber).HasColumnType("int(11)");

				entity.Property(e => e.UseToolDuration).HasColumnType("bit(1)").HasDefaultValue(true);

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.DesiredState).HasColumnType("int(11)");

				entity.Property(e => e.OriginalAdditionTime).HasColumnType("datetime");

				entity.Property(e => e.ToolType)
					.IsRequired()
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.Craft)
					.WithMany(p => p.CraftTools)
					.HasForeignKey(d => new { d.CraftId, d.CraftRevisionNumber })
					.HasConstraintName("FK_CraftTools_Crafts");
			});

			modelBuilder.Entity<Models.Craft>(entity =>
			{
				entity.HasKey(e => new { e.Id, e.RevisionNumber })
					.HasName("PRIMARY");

				entity.HasIndex(e => e.AppearInCraftsListProgId)
					.HasDatabaseName("FK_Crafts_FutureProgs_AppearInCraftsListProg_idx");

				entity.HasIndex(e => e.CanUseProgId)
					.HasDatabaseName("FK_Crafts_FutureProgs_CanUseProg_idx");

				entity.HasIndex(e => e.CheckTraitId)
					.HasDatabaseName("FK_Crafts_TraitDefinitions_idx");

				entity.HasIndex(e => e.EditableItemId)
					.HasDatabaseName("FK_Crafts_EditableItems_idx");

				entity.HasIndex(e => e.OnUseProgCancelId)
					.HasDatabaseName("FK_Crafts_FutureProgs_OnUseProgCancel_idx");

				entity.HasIndex(e => e.OnUseProgCompleteId)
					.HasDatabaseName("FK_Crafts_FutureProgs_OnUseProgComplete_idx");

				entity.HasIndex(e => e.OnUseProgStartId)
					.HasDatabaseName("FK_Crafts_FutureProgs_OnUseProgStart_idx");

				entity.HasIndex(e => e.WhyCannotUseProgId)
					.HasDatabaseName("FK_Crafts_FutureProgs_WhyCannotUseProg_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.RevisionNumber).HasColumnType("int(11)");

				entity.Property(e => e.ActionDescription)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.ActiveCraftItemSdesc)
					.IsRequired()
					.HasColumnName("ActiveCraftItemSDesc")
					.HasColumnType("varchar(200)")
					.HasDefaultValueSql("'a craft in progress'")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.AppearInCraftsListProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.Blurb)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.CanUseProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.Category)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.CheckDifficulty).HasColumnType("int(11)");

				entity.Property(e => e.CheckTraitId).HasColumnType("bigint(20)");

				entity.Property(e => e.EditableItemId).HasColumnType("bigint(20)");

				entity.Property(e => e.FailPhase).HasColumnType("int(11)");

				entity.Property(e => e.FailThreshold).HasColumnType("int(11)");

				entity.Property(e => e.FreeSkillChecks).HasColumnType("int(11)");

				entity.Property(e => e.Interruptable).HasColumnType("bit(1)");

				entity.Property(e => e.IsPracticalCheck).HasColumnType("bit(1)").HasDefaultValueSql("b'1'");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.OnUseProgCancelId).HasColumnType("bigint(20)");

				entity.Property(e => e.OnUseProgCompleteId).HasColumnType("bigint(20)");

				entity.Property(e => e.OnUseProgStartId).HasColumnType("bigint(20)");

				entity.Property(e => e.QualityFormula)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.WhyCannotUseProgId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.AppearInCraftsListProg)
					.WithMany(p => p.CraftsAppearInCraftsListProg)
					.HasForeignKey(d => d.AppearInCraftsListProgId)
					.HasConstraintName("FK_Crafts_FutureProgs_AppearInCraftsListProg");

				entity.HasOne(d => d.CanUseProg)
					.WithMany(p => p.CraftsCanUseProg)
					.HasForeignKey(d => d.CanUseProgId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Crafts_FutureProgs_CanUseProg");

				entity.HasOne(d => d.CheckTrait)
					.WithMany(p => p.Crafts)
					.HasForeignKey(d => d.CheckTraitId)
					.HasConstraintName("FK_Crafts_TraitDefinitions");

				entity.HasOne(d => d.EditableItem)
					.WithMany(p => p.Crafts)
					.HasForeignKey(d => d.EditableItemId)
					.HasConstraintName("FK_Crafts_EditableItems");

				entity.HasOne(d => d.OnUseProgCancel)
					.WithMany(p => p.CraftsOnUseProgCancel)
					.HasForeignKey(d => d.OnUseProgCancelId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Crafts_FutureProgs_OnUseProgCancel");

				entity.HasOne(d => d.OnUseProgComplete)
					.WithMany(p => p.CraftsOnUseProgComplete)
					.HasForeignKey(d => d.OnUseProgCompleteId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Crafts_FutureProgs_OnUseProgComplete");

				entity.HasOne(d => d.OnUseProgStart)
					.WithMany(p => p.CraftsOnUseProgStart)
					.HasForeignKey(d => d.OnUseProgStartId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Crafts_FutureProgs_OnUseProgStart");

				entity.HasOne(d => d.WhyCannotUseProg)
					.WithMany(p => p.CraftsWhyCannotUseProg)
					.HasForeignKey(d => d.WhyCannotUseProgId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Crafts_FutureProgs_WhyCannotUseProg");
			});

			modelBuilder.Entity<Crime>(entity =>
			{
				entity.HasIndex(e => e.AccuserId)
					.HasDatabaseName("FK_Crimes_Accuser_idx");

				entity.HasIndex(e => e.CriminalId)
					.HasDatabaseName("FK_Crimes_Criminal_idx");

				entity.HasIndex(e => e.LawId)
					.HasDatabaseName("FK_Crimes_Laws_idx");

				entity.HasIndex(e => e.LocationId)
					.HasDatabaseName("FK_Crimes_Location_idx");

				entity.HasIndex(e => e.VictimId)
					.HasDatabaseName("FK_Crimes_Victim_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.AccuserId).HasColumnType("bigint(20)");

				entity.Property(e => e.BailHasBeenPosted).HasColumnType("bit(1)").HasDefaultValue(false);
				entity.Property(e => e.HasBeenEnforced).HasColumnType("bit(1)").HasDefaultValue(false);
				entity.Property(e => e.ExecutionPunishment).HasColumnType("bit(1)").HasDefaultValue(false);
				entity.Property(e => e.FineHasBeenPaid).HasColumnType("bit(1)").HasDefaultValue(false);
				entity.Property(e => e.SentenceHasBeenServed).HasColumnType("bit(1)").HasDefaultValue(false);
				entity.Property(e => e.GoodBehaviourBond).HasColumnType("double").HasDefaultValue(0.0);

				entity.Property(e => e.WitnessIds).HasColumnType("varchar(1000)");

				entity.Property(e => e.ConvictionRecorded).HasColumnType("bit(1)");

				entity.Property(e => e.CriminalCharacteristics)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.CriminalFullDescription)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.CriminalId).HasColumnType("bigint(20)");

				entity.Property(e => e.CriminalShortDescription)
					.IsRequired()
					.HasColumnType("varchar(500)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.IsCriminalIdentityKnown).HasColumnType("bit(1)");

				entity.Property(e => e.IsFinalised).HasColumnType("bit(1)");

				entity.Property(e => e.IsKnownCrime).HasColumnType("bit(1)");

				entity.Property(e => e.IsStaleCrime).HasColumnType("bit(1)");

				entity.Property(e => e.LawId).HasColumnType("bigint(20)");

				entity.Property(e => e.LocationId).HasColumnType("bigint(20)");

				entity.Property(e => e.RealTimeOfCrime).HasColumnType("datetime");

				entity.Property(e => e.CalculatedBail).HasColumnType("decimal(58,29)").HasDefaultValue(0.0M);
				entity.Property(e => e.FineRecorded).HasColumnType("decimal(58,29)").HasDefaultValue(0.0M);
				entity.Property(e => e.CustodialSentenceLength).HasColumnType("double").HasDefaultValue(0.0);

				entity.Property(e => e.TimeOfCrime)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.TimeOfReport)
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.VictimId).HasColumnType("bigint(20)");

				entity.Property(e => e.ThirdPartyId).HasColumnType("bigint(20)");
				entity.Property(e => e.ThirdPartyIItemType).HasColumnType("varchar(100)");

				entity.HasOne(d => d.Accuser)
					.WithMany(p => p.CrimesAccuser)
					.HasForeignKey(d => d.AccuserId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Crimes_Accuser");

				entity.HasOne(d => d.Criminal)
					.WithMany(p => p.CrimesCriminal)
					.HasForeignKey(d => d.CriminalId)
					.HasConstraintName("FK_Crimes_Criminal");

				entity.HasOne(d => d.Law)
					.WithMany(p => p.Crimes)
					.HasForeignKey(d => d.LawId)
					.HasConstraintName("FK_Crimes_Laws");

				entity.HasOne(d => d.Location)
					.WithMany(p => p.Crimes)
					.HasForeignKey(d => d.LocationId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Crimes_Location");

				entity.HasOne(d => d.Victim)
					.WithMany(p => p.CrimesVictim)
					.HasForeignKey(d => d.VictimId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Crimes_Victim");
			});

			modelBuilder.Entity<CultureInfo>(entity =>
			{
				entity.Property(e => e.Id)
					.HasColumnType("varchar(50)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.DisplayName)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Order).HasColumnType("int(11)");
			});

			modelBuilder.Entity<Culture>(entity =>
			{
				entity.HasIndex(e => e.AvailabilityProgId)
					.HasDatabaseName("FK_Cultures_AvailabilityProg");
				
				entity.HasIndex(e => e.SkillStartingValueProgId)
					.HasDatabaseName("FK_Cultures_SkillStartingProg");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.AvailabilityProgId).HasColumnType("bigint(20)");

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

				entity.Property(e => e.PersonWordFemale)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PersonWordIndeterminate)
					.IsRequired()
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PersonWordMale)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PersonWordNeuter)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PrimaryCalendarId).HasColumnType("bigint(20)");

				entity.Property(e => e.SkillStartingValueProgId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.AvailabilityProg)
					.WithMany(p => p.CulturesAvailabilityProg)
					.HasForeignKey(d => d.AvailabilityProgId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Cultures_AvailabilityProg");

				entity.HasOne(d => d.SkillStartingValueProg)
					.WithMany(p => p.CulturesSkillStartingValueProg)
					.HasForeignKey(d => d.SkillStartingValueProgId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Cultures_SkillStartingProg");
			});

			modelBuilder.Entity<CulturesNameCultures>(entity =>
			{
				entity.HasKey(e => new { e.CultureId, e.NameCultureId, e.Gender })
					.HasName("PRIMARY");

				entity.Property(e => e.CultureId).HasColumnType("bigint(20)");
				entity.Property(e => e.NameCultureId).HasColumnType("bigint(20)");
				entity.Property(e => e.Gender).HasColumnType("smallint(6)");

				entity.HasOne(e => e.Culture)
					.WithMany(e => e.CulturesNameCultures)
					.HasForeignKey(e => e.CultureId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_CulturesNameCultures_Cultures");

				entity.HasOne(e => e.NameCulture)
					.WithMany(e => e.CulturesNameCultures)
					.HasForeignKey(e => e.NameCultureId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_CulturesNameCultures_NameCultures");
			});

			modelBuilder.Entity<CulturesChargenResources>(entity =>
			{
				entity.HasKey(e => new { e.CultureId, e.ChargenResourceId })
					.HasName("PRIMARY");

				entity.ToTable("Cultures_ChargenResources");

				entity.Property(e => e.CultureId).HasColumnType("bigint(20)");

				entity.Property(e => e.ChargenResourceId).HasColumnType("bigint(20)");

				entity.Property(e => e.RequirementOnly)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.Amount).HasColumnType("int(11)");

				//entity.HasOne(d => d.ChargenResource)
				//    .WithMany(p => p.CulturesChargenResources)
				//    .HasForeignKey(d => d.ChargenResourceId)
				//    .HasConstraintName("FK_Cultures_ChargenResources_ChargenResources");

				//entity.HasOne(d => d.Culture)
				//    .WithMany(p => p.CulturesChargenResources)
				//    .HasForeignKey(d => d.CultureId)
				//    .HasConstraintName("FK_Cultures_ChargenResources_Races");
			});

			modelBuilder.Entity<Currency>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");
				entity.Property(e => e.BaseCurrencyToGlobalBaseCurrencyConversion).HasColumnType("decimal(58,29)").HasDefaultValue(1.0M);
				
				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
			});

			modelBuilder.Entity<CurrencyDescriptionPatternElementSpecialValues>(entity =>
			{
				entity.HasKey(e => new { e.Value, e.CurrencyDescriptionPatternElementId })
					.HasName("PRIMARY");

				entity.HasIndex(e => e.CurrencyDescriptionPatternElementId)
					.HasDatabaseName("FK_CDPESV_CDPE");

				entity.Property(e => e.Value).HasColumnType("decimal(58,29)");

				entity.Property(e => e.CurrencyDescriptionPatternElementId).HasColumnType("bigint(20)");

				entity.Property(e => e.Text)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.CurrencyDescriptionPatternElement)
					.WithMany(p => p.CurrencyDescriptionPatternElementSpecialValues)
					.HasForeignKey(d => d.CurrencyDescriptionPatternElementId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_CDPESV_CDPE");
			});

			modelBuilder.Entity<CurrencyDescriptionPatternElement>(entity =>
			{
				entity.HasIndex(e => e.CurrencyDescriptionPatternId)
					.HasDatabaseName("FK_CDPE_CurrencyDescriptionPatterns");

				entity.HasIndex(e => e.CurrencyDivisionId)
					.HasDatabaseName("FK_CDPE_CurrencyDivisions");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.AlternatePattern)
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.CurrencyDescriptionPatternId).HasColumnType("bigint(20)");

				entity.Property(e => e.CurrencyDivisionId).HasColumnType("bigint(20)");

				entity.Property(e => e.Order).HasColumnType("int(11)");

				entity.Property(e => e.Pattern)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PluraliseWord)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.RoundingMode).HasColumnType("int(11)");

				entity.Property(e => e.ShowIfZero).HasColumnType("bit(1)");

				entity.Property(e => e.SpecialValuesOverrideFormat)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.HasOne(d => d.CurrencyDescriptionPattern)
					.WithMany(p => p.CurrencyDescriptionPatternElements)
					.HasForeignKey(d => d.CurrencyDescriptionPatternId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_CDPE_CurrencyDescriptionPatterns");

				entity.HasOne(d => d.CurrencyDivision)
					.WithMany(p => p.CurrencyDescriptionPatternElements)
					.HasForeignKey(d => d.CurrencyDivisionId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_CDPE_CurrencyDivisions");
			});

			modelBuilder.Entity<CurrencyDescriptionPattern>(entity =>
			{
				entity.HasIndex(e => e.CurrencyId)
					.HasDatabaseName("FK_CurrencyDescriptionPatterns_Currencies");

				entity.HasIndex(e => e.FutureProgId)
					.HasDatabaseName("FK_CurrencyDescriptionPatterns_FutureProgs");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");

				entity.Property(e => e.FutureProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.UseNaturalAggregationStyle).HasColumnType("bit(1)").HasDefaultValue(false);

				entity.Property(e => e.NegativePrefix)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Order).HasColumnType("int(11)");

				entity.Property(e => e.Type).HasColumnType("int(11)");

				entity.HasOne(d => d.Currency)
					.WithMany(p => p.CurrencyDescriptionPatterns)
					.HasForeignKey(d => d.CurrencyId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_CurrencyDescriptionPatterns_Currencies");

				entity.HasOne(d => d.FutureProg)
					.WithMany(p => p.CurrencyDescriptionPatterns)
					.HasForeignKey(d => d.FutureProgId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_CurrencyDescriptionPatterns_FutureProgs");
			});

			modelBuilder.Entity<CurrencyDivisionAbbreviation>(entity =>
			{
				entity.HasKey(e => new { e.Pattern, e.CurrencyDivisionId })
					.HasName("PRIMARY");

				entity.HasIndex(e => e.CurrencyDivisionId)
					.HasDatabaseName("FK_CurrencyDivisionAbbreviations_CurrencyDivisions");

				entity.Property(e => e.Pattern)
					.HasColumnType("varchar(150)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.CurrencyDivisionId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.CurrencyDivision)
					.WithMany(p => p.CurrencyDivisionAbbreviations)
					.HasForeignKey(d => d.CurrencyDivisionId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_CurrencyDivisionAbbreviations_CurrencyDivisions");
			});

			modelBuilder.Entity<CurrencyDivision>(entity =>
			{
				entity.HasIndex(e => e.CurrencyId)
					.HasDatabaseName("FK_CurrencyDivisions_Currencies");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.BaseUnitConversionRate).HasColumnType("decimal(58,29)");

				entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");

				entity.Property(e => e.IgnoreCase).HasColumnType("bit(1)").HasDefaultValue(true);

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.Currency)
					.WithMany(p => p.CurrencyDivisions)
					.HasForeignKey(d => d.CurrencyId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_CurrencyDivisions_Currencies");
			});

			modelBuilder.Entity<DamagePatterns>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Bonus).HasColumnType("int(11)");

				entity.Property(e => e.DamageType).HasColumnType("int(11)");

				entity.Property(e => e.Dice).HasColumnType("int(11)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Sides).HasColumnType("int(11)");
			});

			modelBuilder.Entity<DefaultHook>(entity =>
			{
				entity.HasKey(e => new { e.HookId, e.PerceivableType, e.FutureProgId })
					.HasName("PRIMARY");

				entity.HasIndex(e => e.FutureProgId)
					.HasDatabaseName("FK_DefaultHooks_Futureprogs_idx");

				entity.Property(e => e.HookId).HasColumnType("bigint(20)");

				entity.Property(e => e.PerceivableType)
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.FutureProgId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.FutureProg)
					.WithMany(p => p.DefaultHooks)
					.HasForeignKey(d => d.FutureProgId)
					.HasConstraintName("FK_DefaultHooks_Futureprogs");

				entity.HasOne(d => d.Hook)
					.WithMany(p => p.DefaultHooks)
					.HasForeignKey(d => d.HookId)
					.HasConstraintName("FK_DefaultHooks_Hooks");
			});

			modelBuilder.Entity<DisfigurementTemplate>(entity =>
			{
				entity.HasKey(e => new { e.Id, e.RevisionNumber })
					.HasName("PRIMARY");

				entity.HasIndex(e => e.EditableItemId)
					.HasDatabaseName("FK_DisfigurementTemplates_EditableItems_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.RevisionNumber).HasColumnType("int(11)");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.EditableItemId).HasColumnType("bigint(20)");

				entity.Property(e => e.FullDescription)
					.IsRequired()
					.HasColumnType("varchar(5000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(500)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.ShortDescription)
					.IsRequired()
					.HasColumnType("varchar(500)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Type)
					.IsRequired()
					.HasColumnType("varchar(50)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.EditableItem)
					.WithMany(p => p.DisfigurementTemplates)
					.HasForeignKey(d => d.EditableItemId)
					.HasConstraintName("FK_DisfigurementTemplates_EditableItems");
			});

			modelBuilder.Entity<Door>(entity =>
			{
				entity.HasIndex(e => e.LockedWith)
					.HasDatabaseName("FK_Doors_Locks");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.IsOpen).HasColumnType("bit(1)");

				entity.Property(e => e.LockedWith).HasColumnType("bigint(20)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Style).HasColumnType("int(11)");

				entity.HasOne(d => d.LockedWithNavigation)
					.WithMany(p => p.Doors)
					.HasForeignKey(d => d.LockedWith)
					.HasConstraintName("FK_Doors_Locks");
			});

			modelBuilder.Entity<Drawing>(entity =>
			{
				entity.HasIndex(e => e.AuthorId)
					.HasDatabaseName("FK_Drawings_Characters_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.AuthorId).HasColumnType("bigint(20)");

				entity.Property(e => e.DrawingSize).HasColumnType("int(11)");

				entity.Property(e => e.FullDescription)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.ImplementType).HasColumnType("int(11)");

				entity.Property(e => e.ShortDescription)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.Author)
					.WithMany(p => p.Drawings)
					.HasForeignKey(d => d.AuthorId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Drawings_Characters");
			});

			modelBuilder.Entity<DreamPhase>(entity =>
			{
				entity.HasKey(e => new { e.DreamId, e.PhaseId })
					.HasName("PRIMARY");

				entity.ToTable("Dream_Phases");

				entity.Property(e => e.DreamId).HasColumnType("bigint(20)");

				entity.Property(e => e.PhaseId).HasColumnType("int(11)");

				entity.Property(e => e.DreamerCommand)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.DreamerText)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.WaitSeconds)
					.HasColumnType("int(11)")
					.HasDefaultValueSql("'30'");

				entity.HasOne(d => d.Dream)
					.WithMany(p => p.DreamPhases)
					.HasForeignKey(d => d.DreamId)
					.HasConstraintName("FK_Dream_Phases_Dreams");
			});

			modelBuilder.Entity<Dream>(entity =>
			{
				entity.HasIndex(e => e.CanDreamProgId)
					.HasDatabaseName("FK_Dreams_FutureProgs_CanDream_idx");

				entity.HasIndex(e => e.OnDreamProgId)
					.HasDatabaseName("FK_Dreams_FutureProgs_OnDream_idx");

				entity.HasIndex(e => e.OnWakeDuringDreamingProgId)
					.HasDatabaseName("FK_Dreams_FutureProgs_OnWake_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.CanDreamProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.OnDreamProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.OnWakeDuringDreamingProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.OnlyOnce)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.Priority)
					.HasColumnType("int(11)")
					.HasDefaultValueSql("'100'");

				entity.HasOne(d => d.CanDreamProg)
					.WithMany(p => p.DreamsCanDreamProg)
					.HasForeignKey(d => d.CanDreamProgId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Dreams_FutureProgs_CanDream");

				entity.HasOne(d => d.OnDreamProg)
					.WithMany(p => p.DreamsOnDreamProg)
					.HasForeignKey(d => d.OnDreamProgId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Dreams_FutureProgs_OnDream");

				entity.HasOne(d => d.OnWakeDuringDreamingProg)
					.WithMany(p => p.DreamsOnWakeDuringDreamingProg)
					.HasForeignKey(d => d.OnWakeDuringDreamingProgId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Dreams_FutureProgs_OnWake");
			});

			modelBuilder.Entity<DreamsAlreadyDreamt>(entity =>
			{
				entity.HasKey(e => new { e.DreamId, e.CharacterId })
					.HasName("PRIMARY");

				entity.ToTable("Dreams_Already_Dreamt");

				entity.HasIndex(e => e.CharacterId)
					.HasDatabaseName("FK_Dreams_Dreamt_Characters_idx");

				entity.Property(e => e.DreamId).HasColumnType("bigint(20)");

				entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.Character)
					.WithMany(p => p.DreamsAlreadyDreamt)
					.HasForeignKey(d => d.CharacterId)
					.HasConstraintName("FK_Dreams_Dreamt_Characters");

				entity.HasOne(d => d.Dream)
					.WithMany(p => p.DreamsAlreadyDreamt)
					.HasForeignKey(d => d.DreamId)
					.HasConstraintName("FK_Dreams_Dreamt_Dreams");
			});

			modelBuilder.Entity<DreamsCharacters>(entity =>
			{
				entity.HasKey(e => new { e.DreamId, e.CharacterId })
					.HasName("PRIMARY");

				entity.ToTable("Dreams_Characters");

				entity.HasIndex(e => e.CharacterId)
					.HasDatabaseName("FK_Dreams_Characters_Characters_idx");

				entity.Property(e => e.DreamId).HasColumnType("bigint(20)");

				entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.Character)
					.WithMany(p => p.DreamsCharacters)
					.HasForeignKey(d => d.CharacterId)
					.HasConstraintName("FK_Dreams_Characters_Characters");

				entity.HasOne(d => d.Dream)
					.WithMany(p => p.DreamsCharacters)
					.HasForeignKey(d => d.DreamId)
					.HasConstraintName("FK_Dreams_Characters_Dreams");
			});

			modelBuilder.Entity<Drug>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.DrugVectors).HasColumnType("int(11)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");
			});

			modelBuilder.Entity<DrugIntensity>(entity =>
			{
				entity.HasKey(e => new { e.DrugId, e.DrugType })
					.HasName("PRIMARY");

				entity.Property(e => e.DrugId)
					.HasColumnType("bigint(20)")
					.ValueGeneratedOnAdd();

				entity.Property(e => e.DrugType).HasColumnType("int(11)");

				entity.Property(e => e.AdditionalEffects)
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.HasOne(d => d.Drug)
					.WithMany(p => p.DrugsIntensities)
					.HasForeignKey(d => d.DrugId)
					.HasConstraintName("FK_Drugs_DrugIntensities");
			});

			modelBuilder.Entity<Dub>(entity =>
			{
				entity.HasIndex(e => e.CharacterId)
					.HasDatabaseName("FK_Dubs_Characters");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

				entity.Property(e => e.IntroducedName)
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Keywords)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.LastDescription)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.LastUsage).HasColumnType("datetime");

				entity.Property(e => e.TargetId).HasColumnType("bigint(20)");

				entity.Property(e => e.TargetType)
					.IsRequired()
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.Character)
					.WithMany(p => p.Dubs)
					.HasForeignKey(d => d.CharacterId)
					.HasConstraintName("FK_Dubs_Characters");
			});

			modelBuilder.Entity<EconomicZoneRevenue>(entity =>
			{
				entity.HasKey(e => new { e.EconomicZoneId, e.FinancialPeriodId })
					.HasName("PRIMARY");

				entity.HasIndex(e => e.FinancialPeriodId)
					.HasDatabaseName("FK_EconomicZoneRevenues_FinancialPeriods_idx");

				entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");

				entity.Property(e => e.FinancialPeriodId).HasColumnType("bigint(20)");

				entity.Property(e => e.TotalTaxRevenue).HasColumnType("decimal(58,29)");

				entity.HasOne(d => d.EconomicZone)
					.WithMany(p => p.EconomicZoneRevenues)
					.HasForeignKey(d => d.EconomicZoneId)
					.HasConstraintName("FK_EconomicZoneRevenues");

				entity.HasOne(d => d.FinancialPeriod)
					.WithMany(p => p.EconomicZoneRevenues)
					.HasForeignKey(d => d.FinancialPeriodId)
					.HasConstraintName("FK_EconomicZoneRevenues_FinancialPeriods");
			});

			modelBuilder.Entity<EconomicZoneShopTax>(entity =>
			{
				entity.HasKey(e => new { e.EconomicZoneId, e.ShopId })
					.HasName("PRIMARY");

				entity.HasIndex(e => e.ShopId)
					.HasDatabaseName("FK_EconomicZoneShopTaxes_Shops_idx");

				entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");

				entity.Property(e => e.ShopId).HasColumnType("bigint(20)");

				entity.Property(e => e.OutstandingProfitTaxes).HasColumnType("decimal(58,29)");

				entity.Property(e => e.OutstandingSalesTaxes).HasColumnType("decimal(58,29)");

				entity.Property(e => e.TaxesInCredits).HasColumnType("decimal(58,29)");

				entity.HasOne(d => d.EconomicZone)
					.WithMany(p => p.EconomicZoneShopTaxes)
					.HasForeignKey(d => d.EconomicZoneId)
					.HasConstraintName("FK_EconomicZoneShopTaxes_EconomicZones");

				entity.HasOne(d => d.Shop)
					.WithMany(p => p.EconomicZoneShopTaxes)
					.HasForeignKey(d => d.ShopId)
					.HasConstraintName("FK_EconomicZoneShopTaxes_Shops");
			});

			modelBuilder.Entity<EconomicZoneTax>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");
				entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");
				entity.Property(e => e.MerchandiseFilterProgId).HasColumnType("bigint(20)");
				entity.Property(e => e.TaxType).HasColumnType("varchar(50)");
				entity.Property(e => e.Name).HasColumnType("varchar(200)");
				entity.Property(e => e.MerchantDescription).HasColumnType("varchar(200)");
				entity.Property(e => e.Definition).HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasIndex(e => e.EconomicZoneId).HasDatabaseName("FK_EconomicZoneTaxes_EconomicZones_idx");
				entity.HasIndex(e => e.MerchandiseFilterProgId).HasDatabaseName("FK_EconomicZoneTaxes_FutureProgs_idx");

				entity.HasOne(e => e.EconomicZone)
					.WithMany(e => e.EconomicZoneTaxes)
					.HasForeignKey(e => e.EconomicZoneId)
					.HasConstraintName("FK_EconomicZoneTaxes_EconomicZones");

				entity.HasOne(e => e.MerchandiseFilterProg)
					.WithMany()
					.HasForeignKey(e => e.MerchandiseFilterProgId)
					.HasConstraintName("FK_EconomicZoneTaxes_FutureProgs");
			});

			modelBuilder.Entity<EconomicZone>(entity =>
			{
				entity.HasIndex(e => e.CurrencyId)
					.HasDatabaseName("FK_EconomicZones_Currencies_idx");

				entity.HasIndex(e => e.CurrentFinancialPeriodId)
					.HasDatabaseName("FK_EconomicZones_FinancialPeriods_idx");

				entity.HasIndex(e => e.ReferenceCalendarId)
					.HasDatabaseName("FK_EconomicZones_Calendars_idx");

				entity.HasIndex(e => e.ReferenceClockId)
					.HasDatabaseName("FK_EconomicZones_Timezones_idx");

				entity.HasIndex(e => e.ControllingClanId).HasDatabaseName("FK_EconomicZones_ControllingClans_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");

				entity.Property(e => e.ControllingClanId).HasColumnType("bigint(20)");

				entity.Property(e => e.CurrentFinancialPeriodId).HasColumnType("bigint(20)");

				entity.Property(e => e.IntervalAmount)
					.HasColumnType("int(11)")
					.HasDefaultValueSql("'1'");

				entity.Property(e => e.IntervalModifier).HasColumnType("int(11)");

				entity.Property(e => e.IntervalType)
					.HasColumnType("int(11)")
					.HasDefaultValueSql("'2'");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.OutstandingTaxesOwed).HasColumnType("decimal(58,29)");

				entity.Property(e => e.TotalRevenueHeld).HasColumnType("decimal(58,29)").HasDefaultValueSql("0");

				entity.Property(e => e.PermitTaxableLosses)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'1'");

				entity.Property(e => e.PreviousFinancialPeriodsToKeep)
					.HasColumnType("int(11)")
					.HasDefaultValueSql("'50'");

				entity.Property(e => e.ReferenceCalendarId).HasColumnType("bigint(20)");

				entity.Property(e => e.ReferenceClockId).HasColumnType("bigint(20)");

				entity.Property(e => e.ReferenceTime)
					.IsRequired()
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.ZoneForTimePurposesId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.Currency)
					.WithMany(p => p.EconomicZones)
					.HasForeignKey(d => d.CurrencyId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_EconomicZones_Currencies");

				entity.HasOne(d => d.CurrentFinancialPeriod)
					.WithMany(p => p.EconomicZones)
					.HasForeignKey(d => d.CurrentFinancialPeriodId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_EconomicZones_FinancialPeriods");

				entity.HasOne(d => d.ReferenceCalendar)
					.WithMany(p => p.EconomicZones)
					.HasForeignKey(d => d.ReferenceCalendarId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_EconomicZones_Calendars");

				entity.HasOne(d => d.ReferenceClock)
					.WithMany(p => p.EconomicZones)
					.HasForeignKey(d => d.ReferenceClockId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_EconomicZones_Clocks");

				entity.HasOne(d => d.ReferenceClockNavigation)
					.WithMany(p => p.EconomicZones)
					.HasForeignKey(d => d.ReferenceClockId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_EconomicZones_Timezones");

				entity.HasOne(d => d.ControllingClan)
				.WithMany(d => d.EconomicZones)
				.HasForeignKey(d => d.ControllingClanId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_EconomicZones_ControllingClans");
			});

			modelBuilder.Entity<ConveyancingLocation>(entity =>
			{
				entity.ToTable("ConveyancingLocations");
				entity.HasKey(e => new { e.EconomicZoneId, e.CellId }).HasName("PRIMARY");
				entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");
				entity.Property(e => e.CellId).HasColumnType("bigint(20)");

				entity.HasOne(e => e.EconomicZone)
					.WithMany(e => e.ConveyancingLocations)
					.HasForeignKey(e => e.EconomicZoneId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_ConveyancingLocations_EconomicZones");

				entity.HasOne(e => e.Cell)
					.WithMany()
					.HasForeignKey(e => e.CellId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_ConveyancingLocations_Cells");
			});

			modelBuilder.Entity<JobFindingLocation>(entity =>
			{
				entity.ToTable("JobFindingLocations");
				entity.HasKey(e => new { e.EconomicZoneId, e.CellId }).HasName("PRIMARY");
				entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");
				entity.Property(e => e.CellId).HasColumnType("bigint(20)");

				entity.HasOne(e => e.EconomicZone)
					.WithMany(e => e.JobFindingLocations)
					.HasForeignKey(e => e.EconomicZoneId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_JobFindingLocations_EconomicZones");

				entity.HasOne(e => e.Cell)
					.WithMany()
					.HasForeignKey(e => e.CellId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_JobFindingLocations_Cells");
			});

			modelBuilder.Entity<EditableItem>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.BuilderAccountId).HasColumnType("bigint(20)");

				entity.Property(e => e.BuilderComment)
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.BuilderDate).HasColumnType("datetime");

				entity.Property(e => e.ObsoleteDate).HasColumnType("datetime");

				entity.Property(e => e.ReviewerAccountId).HasColumnType("bigint(20)");

				entity.Property(e => e.ReviewerComment)
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.ReviewerDate).HasColumnType("datetime");

				entity.Property(e => e.RevisionNumber).HasColumnType("int(11)");

				entity.Property(e => e.RevisionStatus).HasColumnType("int(11)");
			});

			modelBuilder.Entity<EmailTemplate>(entity =>
			{
				entity.HasKey(e => e.TemplateType)
					.HasName("PRIMARY");

				entity.Property(e => e.TemplateType).HasColumnType("int(11)").ValueGeneratedNever();

				entity.Property(e => e.Content)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.ReturnAddress)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Subject)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
			});

			modelBuilder.Entity<Election>(entity => {
				entity.ToTable("Elections");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");
				entity.Property(e => e.IsByElection).HasColumnType("bit(1)");
				entity.Property(e => e.AppointmentId).HasColumnType("bigint(20)");
				entity.Property(e => e.IsFinalised).HasColumnType("bit(1)");
				entity.Property(e => e.NumberOfAppointments).HasColumnType("int(11)");
				entity.Property(e => e.NominationStartDate).HasColumnType("varchar(100)");
				entity.Property(e => e.VotingStartDate).HasColumnType("varchar(100)");
				entity.Property(e => e.VotingEndDate).HasColumnType("varchar(100)");
				entity.Property(e => e.ResultsInEffectDate).HasColumnType("varchar(100)");
				entity.Property(e => e.ElectionStage).HasColumnType("int(11)");

				entity.HasIndex(e => e.AppointmentId).HasDatabaseName("FK_Elections_Appointments_idx");
				entity
					.HasOne(d => d.Appointment)
					.WithMany(d => d.Elections)
					.HasForeignKey(d => d.AppointmentId)
					.HasConstraintName("FK_Elections_Appointments");
			});

			modelBuilder.Entity<ElectionNominee>(entity => {
				entity.ToTable("ElectionsNominees");
				entity.Property(e => e.ElectionId).HasColumnType("bigint(20)");
				entity.Property(e => e.NomineeId).HasColumnType("bigint(20)");
				entity.Property(e => e.NomineeClanId).HasColumnType("bigint(20)");
				entity.HasKey(e => new { e.ElectionId, e.NomineeId }).HasName("PRIMARY");
				entity.HasIndex(e => e.ElectionId).HasDatabaseName("FK_ElectionsNominees_Elections_idx");
				entity.HasIndex(e => new { e.NomineeClanId, e.NomineeId }).HasDatabaseName("FK_ElectionsNominees_ClanMemberships_idx");
				entity.HasOne(d => d.Election).WithMany(d => d.ElectionNominees).HasForeignKey(d => d.ElectionId).HasConstraintName("FK_ElectionsNominees_Elections");
				entity.HasOne(d => d.Nominee).WithMany().HasForeignKey(d => new { d.NomineeClanId, d.NomineeId }).HasConstraintName("FK_ElectionsNominees_ClanMemberships");
			});

			modelBuilder.Entity<ElectionVote>(entity => {
				entity.ToTable("ElectionsVotes");
				entity.HasKey(e => new { e.ElectionId, e.NomineeId, e.VoterId }).HasName("PRIMARY");
				entity.Property(e => e.ElectionId).HasColumnType("bigint(20)");
				entity.Property(e => e.NomineeId).HasColumnType("bigint(20)");
				entity.Property(e => e.NomineeClanId).HasColumnType("bigint(20)");
				entity.Property(e => e.VoterId).HasColumnType("bigint(20)");
				entity.Property(e => e.VoterClanId).HasColumnType("bigint(20)");
				entity.Property(e => e.NumberOfVotes).HasColumnType("int(11)");
				entity.HasIndex(e => e.ElectionId).HasDatabaseName("FK_ElectionsVotes_Elections_idx");
				entity.HasIndex(e => new { e.NomineeClanId, e.NomineeId }).HasDatabaseName("FK_ElectionsVotes_Nominees_idx");
				entity.HasIndex(e => new { e.VoterClanId, e.VoterId }).HasDatabaseName("FK_ElectionsVotes_Voters_idx");
				entity.HasOne(d => d.Election).WithMany(d => d.ElectionVotes).HasForeignKey(d => d.ElectionId).HasConstraintName("FK_ElectionsVotes_Elections");
				entity.HasOne(d => d.Nominee).WithMany().HasForeignKey(d => new { d.NomineeClanId, d.NomineeId }).HasConstraintName("FK_ElectionsVotes_Nominees");
				entity.HasOne(d => d.Voter).WithMany().HasForeignKey(d => new { d.VoterClanId, d.VoterId }).HasConstraintName("FK_ElectionsVotes_Voters");
			});

			modelBuilder.Entity<EnforcementAuthority>(entity =>
			{
				entity.HasIndex(e => e.LegalAuthorityId)
					.HasDatabaseName("FK_EnforcementAuthorities_LegalAuthorities_idx");

				entity.HasIndex(e => e.FilterProgId)
					.HasDatabaseName("FK_EnforcementAuthorities_FutureProgs_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.CanAccuse).HasColumnType("bit(1)");

				entity.Property(e => e.CanConvict).HasColumnType("bit(1)");

				entity.Property(e => e.CanForgive).HasColumnType("bit(1)");

				entity.Property(e => e.LegalAuthorityId).HasColumnType("bigint(20)");
				entity.Property(e => e.FilterProgId)
					.IsRequired(false)
					.HasColumnType("bigint(20)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(250)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Priority).HasColumnType("int(11)");

				entity.HasOne(d => d.LegalAuthority)
					.WithMany(p => p.EnforcementAuthorities)
					.HasForeignKey(d => d.LegalAuthorityId)
					.HasConstraintName("FK_EnforcementAuthorities_LegalAuthorities");

				entity.HasOne(d => d.FilterProg)
				.WithMany(p => p.EnforcementAuthorities)
				.HasForeignKey(d => d.FilterProgId)
				.HasConstraintName("FK_EnforcementAuthorities_FutureProgs");
			});

			modelBuilder.Entity<EnforcementAuthoritiesAccusableClasses>(entity =>
			{
				entity.HasKey(e => new { e.EnforcementAuthorityId, e.LegalClassId })
					.HasName("PRIMARY");

				entity.ToTable("EnforcementAuthorities_AccusableClasses");

				entity.HasIndex(e => e.LegalClassId)
					.HasDatabaseName("FK_EnforcementAuthorities_AccusableClasses_LegalClasses_idx");

				entity.Property(e => e.EnforcementAuthorityId).HasColumnType("bigint(20)");

				entity.Property(e => e.LegalClassId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.EnforcementAuthority)
					.WithMany(p => p.EnforcementAuthoritiesAccusableClasses)
					.HasForeignKey(d => d.EnforcementAuthorityId)
					.HasConstraintName("FK_EnforcementAuthorities_AccusableClasses_Enforce");

				entity.HasOne(d => d.LegalClass)
					.WithMany(p => p.EnforcementAuthoritiesAccusableClasses)
					.HasForeignKey(d => d.LegalClassId)
					.HasConstraintName("FK_EnforcementAuthorities_AccusableClasses_LegalClasses");
			});

			modelBuilder.Entity<EnforcementAuthoritiesArrestableLegalClasses>(entity =>
			{
				entity.HasKey(e => new { e.EnforcementAuthorityId, e.LegalClassId });

				entity.HasIndex(e => e.LegalClassId)
					.HasDatabaseName("FK_EnforcementAuthoritiesArrestableLegalClasses_LegalClasses_idx");

				entity.HasIndex(e => e.EnforcementAuthorityId)
					.HasDatabaseName("FK_EnforcementAuthoritiesArrestableLegalClasses_Enforce_idx");

				entity.Property(e => e.EnforcementAuthorityId).HasColumnType("bigint(20)");

				entity.Property(e => e.LegalClassId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.EnforcementAuthority)
					.WithMany(p => p.EnforcementAuthoritiesArrestableLegalClasses)
					.HasForeignKey(d => d.EnforcementAuthorityId)
					.HasConstraintName("FK_EnforcementAuthoritiesArrestableLegalClasses_Enforce");

				entity.HasOne(d => d.LegalClass)
					.WithMany(p => p.EnforcementAuthoritiesArrestableClasses)
					.HasForeignKey(d => d.LegalClassId)
					.HasConstraintName("FK_EnforcementAuthoritiesArrestableLegalClasses_LegalClasses");
			});

			modelBuilder.Entity<EnforcementAuthorityParentAuthority>(entity =>
			{
				entity.HasKey(e => new { e.ParentId, e.ChildId })
					.HasName("PRIMARY");

				entity.ToTable("EnforcementAuthorities_ParentAuthorities");

				entity.HasIndex(e => e.ChildId)
					.HasDatabaseName("FK_EnforcementAuthorities_ParentAuthorities_Child_idx");

				entity.Property(e => e.ParentId).HasColumnType("bigint(20)");

				entity.Property(e => e.ChildId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.Child)
					.WithMany(p => p.EnforcementAuthoritiesParentAuthoritiesChild)
					.HasForeignKey(d => d.ChildId)
					.HasConstraintName("FK_EnforcementAuthorities_ParentAuthorities_Child");

				entity.HasOne(d => d.Parent)
					.WithMany(p => p.EnforcementAuthoritiesParentAuthoritiesParent)
					.HasForeignKey(d => d.ParentId)
					.HasConstraintName("FK_EnforcementAuthorities_ParentAuthorities_Parent");
			});

			modelBuilder.Entity<EntityDescriptionPattern>(entity =>
			{
				entity.HasIndex(e => e.ApplicabilityProgId)
					.HasDatabaseName("FK_EntityDescriptionPatterns_FutureProgs");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.ApplicabilityProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.Pattern)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.RelativeWeight).HasColumnType("int(11)");

				entity.Property(e => e.Type).HasColumnType("int(11)");

				entity.HasOne(d => d.ApplicabilityProg)
					.WithMany(p => p.EntityDescriptionPatterns)
					.HasForeignKey(d => d.ApplicabilityProgId)
					.HasConstraintName("FK_EntityDescriptionPatterns_FutureProgs");
			});

			modelBuilder.Entity<EntityDescriptionPatternsEntityDescriptions>(entity =>
			{
				entity.HasKey(e => new { e.PatternId, e.EntityDescriptionId })
					.HasName("PRIMARY");

				entity.ToTable("EntityDescriptionPatterns_EntityDescriptions");

				entity.HasIndex(e => e.EntityDescriptionId)
					.HasDatabaseName("FK_EDP_EntityDescriptions_EntityDescriptions");

				entity.Property(e => e.PatternId).HasColumnType("bigint(20)");

				entity.Property(e => e.EntityDescriptionId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.EntityDescription)
					.WithMany(p => p.EntityDescriptionPatternsEntityDescriptions)
					.HasForeignKey(d => d.EntityDescriptionId)
					.HasConstraintName("FK_EDP_EntityDescriptions_EntityDescriptions");

				entity.HasOne(d => d.Pattern)
					.WithMany(p => p.EntityDescriptionPatternsEntityDescriptions)
					.HasForeignKey(d => d.PatternId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_EDP_EntityDescriptions_EntityDescriptionPatterns");
			});

			modelBuilder.Entity<EntityDescriptions>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.DisplaySex).HasColumnType("smallint(6)");

				entity.Property(e => e.FullDescription)
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.ShortDescription)
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
			});

			modelBuilder.Entity<Ethnicity>(entity =>
			{
				entity.HasIndex(e => e.AvailabilityProgId)
					.HasDatabaseName("FK_Ethnicities_AvailabilityProg");

				entity.HasIndex(e => e.ParentRaceId)
					.HasDatabaseName("FK_Ethnicities_Races_idx");

				entity.HasIndex(e => e.PopulationBloodModelId)
					.HasDatabaseName("FK_Ethnicities_PopulationBloodModels_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.AvailabilityProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.ChargenBlurb)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.EthnicGroup)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.EthnicSubgroup)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.ParentRaceId).HasColumnType("bigint(20)");

				entity.Property(e => e.PopulationBloodModelId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.AvailabilityProg)
					.WithMany(p => p.Ethnicities)
					.HasForeignKey(d => d.AvailabilityProgId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Ethnicities_AvailabilityProg");

				entity.HasOne(d => d.ParentRace)
					.WithMany(p => p.Ethnicities)
					.HasForeignKey(d => d.ParentRaceId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_Ethnicities_Races");

				entity.HasOne(d => d.PopulationBloodModel)
					.WithMany(p => p.Ethnicities)
					.HasForeignKey(d => d.PopulationBloodModelId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Ethnicities_PopulationBloodModels");
			});

			modelBuilder.Entity<EthnicitiesCharacteristics>(entity =>
			{
				entity.HasKey(e => new { e.EthnicityId, e.CharacteristicDefinitionId, e.CharacteristicProfileId })
					.HasName("PRIMARY");

				entity.ToTable("Ethnicities_Characteristics");

				entity.HasIndex(e => e.CharacteristicDefinitionId)
					.HasDatabaseName("FK_Ethnicities_Characteristics_CharacteristicDefinitions");

				entity.HasIndex(e => e.CharacteristicProfileId)
					.HasDatabaseName("FK_Ethnicities_Characteristics_CharacteristicProfiles");

				entity.Property(e => e.EthnicityId).HasColumnType("bigint(20)");

				entity.Property(e => e.CharacteristicDefinitionId).HasColumnType("bigint(20)");

				entity.Property(e => e.CharacteristicProfileId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.CharacteristicDefinition)
					.WithMany(p => p.EthnicitiesCharacteristics)
					.HasForeignKey(d => d.CharacteristicDefinitionId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Ethnicities_Characteristics_CharacteristicDefinitions");

				entity.HasOne(d => d.CharacteristicProfile)
					.WithMany(p => p.EthnicitiesCharacteristics)
					.HasForeignKey(d => d.CharacteristicProfileId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Ethnicities_Characteristics_CharacteristicProfiles");

				entity.HasOne(d => d.Ethnicity)
					.WithMany(p => p.EthnicitiesCharacteristics)
					.HasForeignKey(d => d.EthnicityId)
					.HasConstraintName("FK_Ethnicities_Characteristics_Ethnicities");
			});

			modelBuilder.Entity<EthnicitiesChargenResources>(entity =>
			{
				entity.HasKey(e => new { e.EthnicityId, e.ChargenResourceId })
					.HasName("PRIMARY");

				entity.ToTable("Ethnicities_ChargenResources");

				entity.Property(e => e.EthnicityId).HasColumnType("bigint(20)");

				entity.Property(e => e.ChargenResourceId).HasColumnType("bigint(20)");

				entity.Property(e => e.RequirementOnly)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.Amount).HasColumnType("int(11)");

				//entity.HasOne(d => d.ChargenResource)
				//    .WithMany(p => p.EthnicitiesChargenResources)
				//    .HasForeignKey(d => d.ChargenResourceId)
				//    .HasConstraintName("FK_Ethnicities_ChargenResources_ChargenResources");

				//entity.HasOne(d => d.Ethnicity)
				//    .WithMany(p => p.EthnicitiesChargenResources)
				//    .HasForeignKey(d => d.EthnicityId)
				//    .HasConstraintName("FK_Ethnicities_ChargenResources_Ethnicities");
			});

			modelBuilder.Entity<Exit>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.AcceptsDoor).HasColumnType("bit(1)");

				entity.Property(e => e.BlockedLayers)
					.IsRequired()
					.HasColumnType("varchar(255)")
					.HasDefaultValueSql("''")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.CellId1).HasColumnType("bigint(20)");

				entity.Property(e => e.CellId2).HasColumnType("bigint(20)");

				entity.Property(e => e.ClimbDifficulty)
					.HasColumnType("int(11)")
					.HasDefaultValueSql("'5'");

				entity.Property(e => e.Direction1).HasColumnType("int(11)");

				entity.Property(e => e.Direction2).HasColumnType("int(11)");

				entity.Property(e => e.DoorId).HasColumnType("bigint(20)");

				entity.Property(e => e.DoorSize).HasColumnType("int(11)");

				entity.Property(e => e.FallCell).HasColumnType("bigint(20)");

				entity.Property(e => e.InboundDescription1)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.InboundDescription2)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.InboundTarget1)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.InboundTarget2)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.IsClimbExit)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.Keywords1)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Keywords2)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.MaximumSizeToEnter)
					.HasColumnType("int(11)")
					.HasDefaultValueSql("'12'");

				entity.Property(e => e.MaximumSizeToEnterUpright)
					.HasColumnType("int(11)")
					.HasDefaultValueSql("'12'");

				entity.Property(e => e.OutboundDescription1)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.OutboundDescription2)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.OutboundTarget1)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.OutboundTarget2)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PrimaryKeyword1)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PrimaryKeyword2)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Verb1)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Verb2)
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
			});

			modelBuilder.Entity<ExternalClanControl>(entity =>
			{
				entity.HasKey(e => new { e.VassalClanId, e.LiegeClanId, e.ControlledAppointmentId })
					.HasName("PRIMARY");

				entity.HasIndex(e => e.ControlledAppointmentId)
					.HasDatabaseName("FK_ECC_Appointments_Controlled");

				entity.HasIndex(e => e.ControllingAppointmentId)
					.HasDatabaseName("FK_ECC_Appointments_Controlling");

				entity.HasIndex(e => e.LiegeClanId)
					.HasDatabaseName("FK_ECC_Clans_Liege");

				entity.Property(e => e.VassalClanId).HasColumnType("bigint(20)");

				entity.Property(e => e.LiegeClanId).HasColumnType("bigint(20)");

				entity.Property(e => e.ControlledAppointmentId).HasColumnType("bigint(20)");

				entity.Property(e => e.ControllingAppointmentId).HasColumnType("bigint(20)");

				entity.Property(e => e.NumberOfAppointments).HasColumnType("int(11)");

				entity.HasOne(d => d.ControlledAppointment)
					.WithMany(p => p.ExternalClanControlsControlledAppointment)
					.HasForeignKey(d => d.ControlledAppointmentId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_ECC_Appointments_Controlled");

				entity.HasOne(d => d.ControllingAppointment)
					.WithMany(p => p.ExternalClanControlsControllingAppointment)
					.HasForeignKey(d => d.ControllingAppointmentId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_ECC_Appointments_Controlling");

				entity.HasOne(d => d.LiegeClan)
					.WithMany(p => p.ExternalClanControlsLiegeClan)
					.HasForeignKey(d => d.LiegeClanId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_ECC_Clans_Liege");

				entity.HasOne(d => d.VassalClan)
					.WithMany(p => p.ExternalClanControlsVassalClan)
					.HasForeignKey(d => d.VassalClanId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_ECC_Clans_Vassal");
			});

			modelBuilder.Entity<ExternalClanControlsAppointment>(entity =>
			{
				entity.HasKey(e => new { e.CharacterId, e.VassalClanId, e.LiegeClanId, e.ControlledAppointmentId })
					.HasName("PRIMARY");

				entity.ToTable("ExternalClanControls_Appointments");

				entity.HasIndex(e => new { e.VassalClanId, e.CharacterId })
					.HasDatabaseName("FK_ECC_Appointments_ClanMemberships");

				entity.HasIndex(e => new { e.VassalClanId, e.LiegeClanId, e.ControlledAppointmentId })
					.HasDatabaseName("FK_ECC_Appointments_ExternalClanControls");

				entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

				entity.Property(e => e.VassalClanId).HasColumnType("bigint(20)");

				entity.Property(e => e.LiegeClanId).HasColumnType("bigint(20)");

				entity.Property(e => e.ControlledAppointmentId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.ClanMemberships)
					.WithMany(p => p.ExternalClanControlsAppointments)
					.HasForeignKey(d => new { d.VassalClanId, d.CharacterId })
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_ECC_Appointments_ClanMemberships");

				entity.HasOne(d => d.ExternalClanControls)
					.WithMany(p => p.ExternalClanControlsAppointments)
					.HasForeignKey(d => new { d.VassalClanId, d.LiegeClanId, d.ControlledAppointmentId })
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_ECC_Appointments_ExternalClanControls");
			});

			modelBuilder.Entity<FinancialPeriod>(entity =>
			{
				entity.HasIndex(e => e.EconomicZoneId)
					.HasDatabaseName("FK_FinancialPeriods_EconomicZones_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");

				entity.Property(e => e.MudPeriodEnd)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.MudPeriodStart)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PeriodEnd).HasColumnType("datetime");

				entity.Property(e => e.PeriodStart).HasColumnType("datetime");

				entity.HasOne(d => d.EconomicZone)
					.WithMany(p => p.FinancialPeriods)
					.HasForeignKey(d => d.EconomicZoneId)
					.HasConstraintName("FK_FinancialPeriods_EconomicZones");
			});

			modelBuilder.Entity<ForagableProfile>(entity =>
			{
				entity.HasKey(e => new { e.Id, e.RevisionNumber })
					.HasName("PRIMARY");

				entity.HasIndex(e => e.EditableItemId)
					.HasDatabaseName("FK_ForagableProfiles_EditableItems_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.RevisionNumber).HasColumnType("int(11)");

				entity.Property(e => e.EditableItemId).HasColumnType("bigint(20)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.HasOne(d => d.EditableItem)
					.WithMany(p => p.ForagableProfiles)
					.HasForeignKey(d => d.EditableItemId)
					.HasConstraintName("FK_ForagableProfiles_EditableItems");
			});

			modelBuilder.Entity<ForagableProfilesForagables>(entity =>
			{
				entity.HasKey(e => new { e.ForagableProfileId, e.ForagableProfileRevisionNumber, e.ForagableId })
					.HasName("PRIMARY");

				entity.ToTable("ForagableProfiles_Foragables");

				entity.Property(e => e.ForagableProfileId).HasColumnType("bigint(20)");

				entity.Property(e => e.ForagableProfileRevisionNumber).HasColumnType("int(11)");

				entity.Property(e => e.ForagableId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.ForagableProfile)
					.WithMany(p => p.ForagableProfilesForagables)
					.HasForeignKey(d => new { d.ForagableProfileId, d.ForagableProfileRevisionNumber })
					.HasConstraintName("FK_ForagableProfiles_Foragables_ForagableProfiles");
			});

			modelBuilder.Entity<ForagableProfilesHourlyYieldGains>(entity =>
			{
				entity.HasKey(e => new { e.ForagableProfileId, e.ForagableProfileRevisionNumber, e.ForageType })
					.HasName("PRIMARY");

				entity.ToTable("ForagableProfiles_HourlyYieldGains");

				entity.Property(e => e.ForagableProfileId).HasColumnType("bigint(20)");

				entity.Property(e => e.ForagableProfileRevisionNumber).HasColumnType("int(11)");

				entity.Property(e => e.ForageType)
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.HasOne(d => d.ForagableProfile)
					.WithMany(p => p.ForagableProfilesHourlyYieldGains)
					.HasForeignKey(d => new { d.ForagableProfileId, d.ForagableProfileRevisionNumber })
					.HasConstraintName("FK_ForagableProfiles_HourlyYieldGains_ForagableProfiles");
			});

			modelBuilder.Entity<ForagableProfilesMaximumYields>(entity =>
			{
				entity.HasKey(e => new { e.ForagableProfileId, e.ForagableProfileRevisionNumber, e.ForageType })
					.HasName("PRIMARY");

				entity.ToTable("ForagableProfiles_MaximumYields");

				entity.Property(e => e.ForagableProfileId).HasColumnType("bigint(20)");

				entity.Property(e => e.ForagableProfileRevisionNumber).HasColumnType("int(11)");

				entity.Property(e => e.ForageType)
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.HasOne(d => d.ForagableProfile)
					.WithMany(p => p.ForagableProfilesMaximumYields)
					.HasForeignKey(d => new { d.ForagableProfileId, d.ForagableProfileRevisionNumber })
					.HasConstraintName("FK_ForagableProfiles_MaximumYields_ForagableProfiles");
			});

			modelBuilder.Entity<Foragable>(entity =>
			{
				entity.HasKey(e => new { e.Id, e.RevisionNumber })
					.HasName("PRIMARY");

				entity.HasIndex(e => e.EditableItemId)
					.HasDatabaseName("FK_Foragables_EditableItems");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.RevisionNumber).HasColumnType("int(11)");

				entity.Property(e => e.CanForageProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.EditableItemId).HasColumnType("bigint(20)");

				entity.Property(e => e.ForagableTypes)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.ForageDifficulty).HasColumnType("int(11)");

				entity.Property(e => e.ItemProtoId).HasColumnType("bigint(20)");

				entity.Property(e => e.MaximumOutcome).HasColumnType("int(11)");

				entity.Property(e => e.MinimumOutcome).HasColumnType("int(11)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.OnForageProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.QuantityDiceExpression)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.RelativeChance).HasColumnType("int(11)");

				entity.HasOne(d => d.EditableItem)
					.WithMany(p => p.Foragables)
					.HasForeignKey(d => d.EditableItemId)
					.HasConstraintName("FK_Foragables_EditableItems");
			});

			modelBuilder.Entity<Models.FutureProg>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.AcceptsAnyParameters)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.Category)
					.IsRequired()
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.FunctionComment)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.FunctionName)
					.IsRequired()
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.FunctionText)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_general_ci");

				entity.Property(e => e.Public)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.ReturnType).HasColumnType("bigint(20)");

				entity.Property(e => e.StaticType).HasColumnType("int(11)");

				entity.Property(e => e.Subcategory)
					.IsRequired()
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
			});

			modelBuilder.Entity<FutureProgsParameter>(entity =>
			{
				entity.HasKey(e => new { e.FutureProgId, e.ParameterIndex })
					.HasName("PRIMARY");

				entity.ToTable("FutureProgs_Parameters");

				entity.Property(e => e.FutureProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.ParameterIndex).HasColumnType("int(11)");

				entity.Property(e => e.ParameterName)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.ParameterType).HasColumnType("bigint(20)");

				entity.HasOne(d => d.FutureProg)
					.WithMany(p => p.FutureProgsParameters)
					.HasForeignKey(d => d.FutureProgId)
					.HasConstraintName("FK_FutureProgs_Parameters_FutureProgs");
			});

			modelBuilder.Entity<GameItemComponentProto>(entity =>
			{
				entity.HasKey(e => new { e.Id, e.RevisionNumber })
					.HasName("PRIMARY");

				entity.HasIndex(e => e.EditableItemId)
					.HasDatabaseName("FK_GameItemComponentProtos_EditableItems");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.RevisionNumber).HasColumnType("int(11)");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Description)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.EditableItemId).HasColumnType("bigint(20)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Type)
					.IsRequired()
					.HasColumnType("varchar(50)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.EditableItem)
					.WithMany(p => p.GameItemComponentProtos)
					.HasForeignKey(d => d.EditableItemId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_GameItemComponentProtos_EditableItems");
			});

			modelBuilder.Entity<GameItemComponent>(entity =>
			{
				entity.HasIndex(e => e.GameItemId)
					.HasDatabaseName("FK_GameItemComponents_GameItems");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.GameItemComponentProtoId).HasColumnType("bigint(20)");

				entity.Property(e => e.GameItemComponentProtoRevision).HasColumnType("int(11)");

				entity.Property(e => e.GameItemId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.GameItem)
					.WithMany(p => p.GameItemComponents)
					.HasForeignKey(d => d.GameItemId)
					.HasConstraintName("FK_GameItemComponents_GameItems");
			});

			modelBuilder.Entity<GameItemProto>(entity =>
			{
				entity.HasKey(e => new { e.Id, e.RevisionNumber })
					.HasName("PRIMARY");

				entity.HasIndex(e => e.EditableItemId)
					.HasDatabaseName("FK_GameItemProtos_EditableItems");

				entity.HasIndex(e => e.ItemGroupId)
					.HasDatabaseName("FK_GameItemProtos_ItemGroups_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.RevisionNumber).HasColumnType("int(11)");

				entity.Property(e => e.BaseItemQuality)
					.HasColumnType("int(11)")
					.HasDefaultValueSql("'5'");

				entity.Property(e => e.CustomColour)
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.EditableItemId).HasColumnType("bigint(20)");

				entity.Property(e => e.FullDescription)
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PermitPlayerSkins)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.CostInBaseCurrency)
					  .HasColumnType("decimal(58,29)")
					  .HasDefaultValueSql("'0'");

				entity.Property(e => e.HealthStrategyId).HasColumnType("bigint(20)");

				entity.Property(e => e.HighPriority)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.ItemGroupId).HasColumnType("bigint(20)");

				entity.Property(e => e.Keywords)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.LongDescription)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.MaterialId).HasColumnType("bigint(20)");

				entity.Property(e => e.MorphEmote)
					.IsRequired()
					.HasColumnType("varchar(1000)")
					.HasDefaultValueSql("'$0 $?1|morphs into $1|decays into nothing$.'")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.MorphGameItemProtoId).HasColumnType("bigint(20)");

				entity.Property(e => e.MorphTimeSeconds).HasColumnType("int(11)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.OnDestroyedGameItemProtoId).HasColumnType("bigint(20)");

				entity.Property(e => e.ReadOnly)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.ShortDescription)
					.HasColumnType("varchar(1000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Size).HasColumnType("int(11)");

				entity.HasOne(d => d.EditableItem)
					.WithMany(p => p.GameItemProtos)
					.HasForeignKey(d => d.EditableItemId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_GameItemProtos_EditableItems");

				entity.HasOne(d => d.ItemGroup)
					.WithMany(p => p.GameItemProtos)
					.HasForeignKey(d => d.ItemGroupId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_GameItemProtos_ItemGroups");
			});

			modelBuilder.Entity<GameItemProtoExtraDescription>(entity => {
				entity.HasKey(e => new { e.GameItemProtoId, e.GameItemProtoRevisionNumber, e.ApplicabilityProgId })
					.HasName("PRIMARY");

				entity.Property(e => e.GameItemProtoId).HasColumnType("bigint(20)");
				entity.Property(e => e.GameItemProtoRevisionNumber).HasColumnType("int(11)");
				entity.Property(e => e.ApplicabilityProgId).HasColumnType("bigint(20)");
				entity.Property(e => e.Priority).HasColumnType("int(20)");

				entity.Property(e => e.ShortDescription)
					.HasColumnType("varchar(500)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.FullDescription)
				.HasColumnType("varchar(2000)")
				.HasCharSet("utf8mb4")
				.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.FullDescriptionAddendum)
				.HasColumnType("varchar(500)")
				.HasCharSet("utf8mb4")
				.UseCollation("utf8mb4_unicode_ci");

				entity.HasOne(e => e.GameItemProto)
				.WithMany(e => e.ExtraDescriptions)
				.HasForeignKey(e => new { e.GameItemProtoId, e.GameItemProtoRevisionNumber })
				.HasConstraintName("FK_GameItemProtoExtraDescriptions_GameItemProtos");

				entity.HasOne(e => e.ApplicabilityProg)
				.WithMany()
				.HasForeignKey(e => e.ApplicabilityProgId)
				.HasConstraintName("FK_GameItemProtoExtraDescriptions_FutureProgs");
			});

			modelBuilder.Entity<GameItemProtosDefaultVariable>(entity =>
			{
				entity.HasKey(e => new { e.GameItemProtoId, e.GameItemProtoRevNum, e.VariableName })
					.HasName("PRIMARY");

				entity.ToTable("GameItemProtos_DefaultVariables");

				entity.Property(e => e.GameItemProtoId).HasColumnType("bigint(20)");

				entity.Property(e => e.GameItemProtoRevNum).HasColumnType("int(11)");

				entity.Property(e => e.VariableName)
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.VariableValue)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.HasOne(d => d.GameItemProto)
					.WithMany(p => p.GameItemProtosDefaultVariables)
					.HasForeignKey(d => new { d.GameItemProtoId, d.GameItemProtoRevNum })
					.HasConstraintName("FK_GameItemProtos_DefaultValues_GameItemProtos");
			});

			modelBuilder.Entity<GameItemProtosGameItemComponentProtos>(entity =>
			{
				entity.HasKey(e => new { e.GameItemProtoId, e.GameItemComponentProtoId, e.GameItemProtoRevision, e.GameItemComponentRevision })
					.HasName("PRIMARY");

				entity.ToTable("GameItemProtos_GameItemComponentProtos");

				entity.HasIndex(e => new { e.GameItemComponentProtoId, e.GameItemComponentRevision })
					.HasDatabaseName("FK_GIPGICP_GameItemComponentProtos");

				entity.HasIndex(e => new { e.GameItemProtoId, e.GameItemProtoRevision })
					.HasDatabaseName("FK_GIPGICP_GameItemProtos");

				entity.Property(e => e.GameItemProtoId).HasColumnType("bigint(20)");

				entity.Property(e => e.GameItemComponentProtoId).HasColumnType("bigint(20)");

				entity.Property(e => e.GameItemProtoRevision).HasColumnType("int(11)");

				entity.Property(e => e.GameItemComponentRevision).HasColumnType("int(11)");

				entity.HasOne(d => d.GameItemComponent)
					.WithMany(p => p.GameItemProtosGameItemComponentProtos)
					.HasForeignKey(d => new { d.GameItemComponentProtoId, d.GameItemComponentRevision })
					.HasConstraintName("FK_GIPGICP_GameItemComponentProtos");

				entity.HasOne(d => d.GameItemProto)
					.WithMany(p => p.GameItemProtosGameItemComponentProtos)
					.HasForeignKey(d => new { d.GameItemProtoId, d.GameItemProtoRevision })
					.HasConstraintName("FK_GIPGICP_GameItemProtos");
			});

			modelBuilder.Entity<GameItemProtosOnLoadProgs>(entity =>
			{
				entity.HasKey(e => new { e.GameItemProtoId, e.GameItemProtoRevisionNumber, e.FutureProgId })
					.HasName("PRIMARY");

				entity.ToTable("GameItemProtos_OnLoadProgs");

				entity.HasIndex(e => e.FutureProgId)
					.HasDatabaseName("FK_GameItemProtos_OnLoadProgs_FutureProgs_idx");

				entity.Property(e => e.GameItemProtoId).HasColumnType("bigint(20)");

				entity.Property(e => e.GameItemProtoRevisionNumber).HasColumnType("int(11)");

				entity.Property(e => e.FutureProgId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.FutureProg)
					.WithMany(p => p.GameItemProtosOnLoadProgs)
					.HasForeignKey(d => d.FutureProgId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_GameItemProtos_OnLoadProgs_FutureProgs");

				entity.HasOne(d => d.GameItemProto)
					.WithMany(p => p.GameItemProtosOnLoadProgs)
					.HasForeignKey(d => new { d.GameItemProtoId, d.GameItemProtoRevisionNumber })
					.HasConstraintName("FK_GameItemProtos_OnLoadProgs_GameItemProtos");
			});

			modelBuilder.Entity<GameItemProtosTags>(entity =>
			{
				entity.HasKey(e => new { e.GameItemProtoId, e.TagId, e.GameItemProtoRevisionNumber })
					.HasName("PRIMARY");

				entity.ToTable("GameItemProtos_Tags");

				entity.HasIndex(e => e.TagId)
					.HasDatabaseName("FK_GameItemProtos_Tags_Tags");

				entity.HasIndex(e => new { e.GameItemProtoId, e.GameItemProtoRevisionNumber })
					.HasDatabaseName("FK_GameItemProtos_Tags_GameItemProtos");

				entity.Property(e => e.GameItemProtoId).HasColumnType("bigint(20)");

				entity.Property(e => e.TagId).HasColumnType("bigint(20)");

				entity.Property(e => e.GameItemProtoRevisionNumber).HasColumnType("int(11)");

				entity.HasOne(d => d.Tag)
					.WithMany(p => p.GameItemProtosTags)
					.HasForeignKey(d => d.TagId)
					.HasConstraintName("FK_GameItemProtos_Tags_Tags");

				entity.HasOne(d => d.GameItemProto)
					.WithMany(p => p.GameItemProtosTags)
					.HasForeignKey(d => new { d.GameItemProtoId, d.GameItemProtoRevisionNumber })
					.HasConstraintName("FK_GameItemProtos_Tags_GameItemProtos");
			});

			modelBuilder.Entity<Models.GameItem>(entity =>
			{
				entity.HasIndex(e => e.ContainerId)
					.HasDatabaseName("FK_GameItems_GameItems_Containers_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Condition).HasDefaultValueSql("'1'");

				entity.Property(e => e.ContainerId).HasColumnType("bigint(20)");

				entity.Property(e => e.EffectData)
					.IsRequired()
					.HasColumnType("mediumtext")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.GameItemProtoId).HasColumnType("bigint(20)");

				entity.Property(e => e.GameItemProtoRevision).HasColumnType("int(11)");

				entity.Property(e => e.MaterialId).HasColumnType("bigint(20)");

				entity.Property(e => e.MorphTimeRemaining).HasColumnType("int(11)");

				entity.Property(e => e.PositionEmote)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PositionId)
					.HasColumnType("int(11)")
					.HasDefaultValueSql("'1'");

				entity.Property(e => e.PositionModifier).HasColumnType("int(11)");

				entity.Property(e => e.PositionTargetId).HasColumnType("bigint(20)");

				entity.Property(e => e.PositionTargetType)
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Quality).HasColumnType("int(11)");

				entity.Property(e => e.RoomLayer).HasColumnType("int(11)");

				entity.Property(e => e.Size).HasColumnType("int(11)");

				entity.Property(e => e.SkinId)
					.HasColumnType("bigint(20)")
					.IsRequired(false);

				entity.HasOne(d => d.Container)
					.WithMany(p => p.InverseContainer)
					.HasForeignKey(d => d.ContainerId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_GameItems_GameItems_Containers");
			});

			modelBuilder.Entity<Models.GameItemSkin>(entity =>
			{
				entity.ToTable("GameItemSkins");

				entity.HasKey(e => new { e.Id, e.RevisionNumber })
					.HasName("PRIMARY");

				entity.HasOne(d => d.EditableItem)
					.WithMany()
					.HasForeignKey(d => d.EditableItemId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_GameItemSkins_EditableItems");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.RevisionNumber).HasColumnType("int(11)");
				entity.Property(e => e.IsPublic).HasColumnType("bit(1)");
				entity.Property(e => e.Name)
					.HasColumnType("varchar(500)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");
				entity.Property(e => e.ItemProtoId).HasColumnType("bigint(20)");
				entity.Property(e => e.CanUseSkinProgId)
					.HasColumnType("bigint(20)")
					.IsRequired(false);
				entity.Property(e => e.Quality)
					.HasColumnType("int(11)")
					.IsRequired(false);
				entity.Property(e => e.ItemName)
					.HasColumnType("varchar(500)")
					.HasCharSet("utf8mb4")
					.IsRequired(false)
					.UseCollation("utf8mb4_unicode_ci");
				entity.Property(e => e.ShortDescription)
					.HasColumnType("varchar(500)")
					.HasCharSet("utf8mb4")
					.IsRequired(false)
					.UseCollation("utf8mb4_unicode_ci");
				entity.Property(e => e.LongDescription)
					.HasColumnType("varchar(500)")
					.HasCharSet("utf8mb4")
					.IsRequired(false)
					.UseCollation("utf8mb4_unicode_ci");
				entity.Property(e => e.FullDescription)
					.HasColumnType("varchar(2000)")
					.HasCharSet("utf8mb4")
					.IsRequired(false)
					.UseCollation("utf8mb4_unicode_ci");
			});

			modelBuilder.Entity<GameItemMagicResource>(entity =>
			{
				entity.HasKey(e => new { e.GameItemId, e.MagicResourceId })
					.HasName("PRIMARY");

				entity.ToTable("GameItems_MagicResources");

				entity.HasIndex(e => e.MagicResourceId)
					.HasDatabaseName("FK_GameItems_MagicResources_MagicResources_idx");

				entity.Property(e => e.GameItemId).HasColumnType("bigint(20)");

				entity.Property(e => e.MagicResourceId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.GameItem)
					.WithMany(p => p.GameItemsMagicResources)
					.HasForeignKey(d => d.GameItemId)
					.HasConstraintName("FK_GameItems_MagicResources_GameItems");

				entity.HasOne(d => d.MagicResource)
					.WithMany(p => p.GameItemsMagicResources)
					.HasForeignKey(d => d.MagicResourceId)
					.HasConstraintName("FK_GameItems_MagicResources_MagicResources");
			});

			modelBuilder.Entity<Gameitemeditingview>(entity =>
			{
				entity.HasNoKey();

				entity.ToTable("gameitemeditingview");

				entity.Property(e => e.Id).HasColumnType("tinyint(4)");

				entity.Property(e => e.MaterialId).HasColumnType("tinyint(4)");

				entity.Property(e => e.Name).HasColumnType("tinyint(4)");

				entity.Property(e => e.ProtoMaterial).HasColumnType("tinyint(4)");

				entity.Property(e => e.Quality).HasColumnType("tinyint(4)");

				entity.Property(e => e.Size).HasColumnType("tinyint(4)");
			});

			modelBuilder.Entity<Gas>(entity =>
			{
				entity.HasIndex(e => e.CountAsId)
					.HasDatabaseName("FK_Gases_Gases_idx");

				entity.HasIndex(e => e.PrecipitateId)
					.HasDatabaseName("FK_Gases_Liquids_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.BoilingPoint).HasDefaultValueSql("'5'");

				entity.Property(e => e.CountAsId).HasColumnType("bigint(20)");

				entity.Property(e => e.CountsAsQuality).HasColumnType("int(11)");

				entity.Property(e => e.Density).HasDefaultValueSql("'0.001205'");

				entity.Property(e => e.Description)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.DisplayColour)
					.IsRequired()
					.HasColumnType("varchar(40)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.DrugId).HasColumnType("bigint(20)");

				entity.Property(e => e.DrugGramsPerUnitVolume).HasDefaultValueSql("'0.0'");

				entity.Property(e => e.ElectricalConductivity).HasDefaultValueSql("'0.000005'");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.Organic)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.PrecipitateId).HasColumnType("bigint(20)");

				entity.Property(e => e.SmellText)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.SpecificHeatCapacity).HasDefaultValueSql("'1.005'");

				entity.Property(e => e.ThermalConductivity).HasDefaultValueSql("'0.0257'");

				entity.Property(e => e.VagueSmellText)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.Viscosity).HasDefaultValueSql("'15'");

				entity.HasOne(d => d.CountAs)
					.WithMany(p => p.InverseCountAs)
					.HasForeignKey(d => d.CountAsId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Gases_Gases");

				entity.HasOne(d => d.Precipitate)
					.WithMany(p => p.Gases)
					.HasForeignKey(d => d.PrecipitateId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Gases_Liquids");

				entity.HasOne(d => d.Drug)
					.WithMany()
					.HasForeignKey(d => d.DrugId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Gases_Drugs");
			});

			modelBuilder.Entity<GasesTags>(entity =>
			{
				entity.HasKey(e => new { e.GasId, e.TagId })
					.HasName("PRIMARY");

				entity.ToTable("Gases_Tags");

				entity.HasIndex(e => e.TagId)
					.HasDatabaseName("FK_Gases_Tags_Tags_idx");

				entity.Property(e => e.GasId).HasColumnType("bigint(20)");

				entity.Property(e => e.TagId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.Gas)
					.WithMany(p => p.GasesTags)
					.HasForeignKey(d => d.GasId)
					.HasConstraintName("FK_Gases_Tags_Gases");

				entity.HasOne(d => d.Tag)
					.WithMany(p => p.GasesTags)
					.HasForeignKey(d => d.TagId)
					.HasConstraintName("FK_Gases_Tags_Tags");
			});

			modelBuilder.Entity<Grid>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("mediumtext")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.GridType)
					.IsRequired()
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
			});

			modelBuilder.Entity<GroupAi>(entity =>
			{
				entity.ToTable("GroupAIs");

				entity.HasIndex(e => e.GroupAiTemplateId)
					.HasDatabaseName("FK_GroupAIs_GroupAITemplates_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Data)
					.IsRequired()
					.HasColumnType("mediumtext")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("mediumtext")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.GroupAiTemplateId)
					.HasColumnName("GroupAITemplateId")
					.HasColumnType("bigint(20)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.GroupAiTemplate)
					.WithMany(p => p.GroupAis)
					.HasForeignKey(d => d.GroupAiTemplateId)
					.HasConstraintName("FK_GroupAIs_GroupAITemplates");
			});

			modelBuilder.Entity<GroupAiTemplate>(entity =>
			{
				entity.ToTable("GroupAITemplates");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("mediumtext")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
			});

			modelBuilder.Entity<Guest>(entity =>
			{
				entity.HasKey(e => e.CharacterId)
					.HasName("PRIMARY");

				entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.Character)
					.WithOne(p => p.Guest)
					.HasForeignKey<Guest>(d => d.CharacterId)
					.HasConstraintName("FK_Guests_Characters");
			});

			modelBuilder.Entity<HealthStrategy>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_general_ci");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_general_ci");

				entity.Property(e => e.Type)
					.IsRequired()
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_general_ci");
			});

			modelBuilder.Entity<HearingProfile>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.SurveyDescription)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Type)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
			});

			modelBuilder.Entity<HeightWeightModel>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Bmimultiplier).HasColumnName("BMIMultiplier");

				entity.Property(e => e.MeanBmi).HasColumnName("MeanBMI");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(4000)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.StddevBmi).HasColumnName("StddevBMI");
			});

			modelBuilder.Entity<Helpfile>(entity =>
			{
				entity.HasIndex(e => e.RuleId)
					.HasDatabaseName("FK_Helpfiles_FutureProgs");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Category)
					.IsRequired()
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Keywords)
					.IsRequired()
					.HasColumnType("varchar(500)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.LastEditedBy)
					.IsRequired()
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.LastEditedDate).HasColumnType("datetime");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PublicText)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.RuleId).HasColumnType("bigint(20)");

				entity.Property(e => e.Subcategory)
					.IsRequired()
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.TagLine)
					.IsRequired()
					.HasColumnType("varchar(500)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.Rule)
					.WithMany(p => p.Helpfiles)
					.HasForeignKey(d => d.RuleId)
					.HasConstraintName("FK_Helpfiles_FutureProgs");
			});

			modelBuilder.Entity<HelpfilesExtraText>(entity =>
			{
				entity.HasKey(e => new { e.HelpfileId, e.DisplayOrder })
					.HasName("PRIMARY");

				entity.ToTable("Helpfiles_ExtraTexts");

				entity.HasIndex(e => e.RuleId)
					.HasDatabaseName("FK_Helpfiles_ExtraTexts_FutureProgs");

				entity.Property(e => e.HelpfileId).HasColumnType("bigint(20)");

				entity.Property(e => e.DisplayOrder).HasColumnType("int(11)");

				entity.Property(e => e.RuleId).HasColumnType("bigint(20)");

				entity.Property(e => e.Text)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.Helpfile)
					.WithMany(p => p.HelpfilesExtraTexts)
					.HasForeignKey(d => d.HelpfileId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Helpfiles_ExtraTexts_Helpfiles");

				entity.HasOne(d => d.Rule)
					.WithMany(p => p.HelpfilesExtraTexts)
					.HasForeignKey(d => d.RuleId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Helpfiles_ExtraTexts_FutureProgs");
			});

			modelBuilder.Entity<Hooks>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Category)
					.IsRequired()
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_general_ci");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.TargetEventType).HasColumnType("int(11)");

				entity.Property(e => e.Type)
					.IsRequired()
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
			});

			modelBuilder.Entity<HooksPerceivable>(entity =>
			{
				entity.ToTable("Hooks_Perceivables");

				entity.HasIndex(e => e.BodyId)
					.HasDatabaseName("FK_Hooks_Perceivables_Bodies_idx");

				entity.HasIndex(e => e.CellId)
					.HasDatabaseName("FK_Hooks_Perceivables_Cells_idx");

				entity.HasIndex(e => e.CharacterId)
					.HasDatabaseName("FK_Hooks_Perceivables_Characters_idx");

				entity.HasIndex(e => e.GameItemId)
					.HasDatabaseName("FK_Hooks_Perceivables_GameItems_idx");

				entity.HasIndex(e => e.HookId)
					.HasDatabaseName("FK_Hooks_Perceivables_Hooks_idx");

				entity.HasIndex(e => e.ShardId)
					.HasDatabaseName("FK_Hooks_Perceivables_Shards_idx");

				entity.HasIndex(e => e.ZoneId)
					.HasDatabaseName("FK_Hooks_Perceivables_Zones_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.BodyId).HasColumnType("bigint(20)");

				entity.Property(e => e.CellId).HasColumnType("bigint(20)");

				entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

				entity.Property(e => e.GameItemId).HasColumnType("bigint(20)");

				entity.Property(e => e.HookId).HasColumnType("bigint(20)");

				entity.Property(e => e.ShardId).HasColumnType("bigint(20)");

				entity.Property(e => e.ZoneId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.Body)
					.WithMany(p => p.HooksPerceivables)
					.HasForeignKey(d => d.BodyId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_Hooks_Perceivables_Bodies");

				entity.HasOne(d => d.Cell)
					.WithMany(p => p.HooksPerceivables)
					.HasForeignKey(d => d.CellId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_Hooks_Perceivables_Cells");

				entity.HasOne(d => d.Character)
					.WithMany(p => p.HooksPerceivables)
					.HasForeignKey(d => d.CharacterId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_Hooks_Perceivables_Characters");

				entity.HasOne(d => d.GameItem)
					.WithMany(p => p.HooksPerceivables)
					.HasForeignKey(d => d.GameItemId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_Hooks_Perceivables_GameItems");

				entity.HasOne(d => d.Hook)
					.WithMany(p => p.HooksPerceivables)
					.HasForeignKey(d => d.HookId)
					.HasConstraintName("FK_Hooks_Perceivables_Hooks");

				entity.HasOne(d => d.Shard)
					.WithMany(p => p.HooksPerceivables)
					.HasForeignKey(d => d.ShardId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_Hooks_Perceivables_Shards");

				entity.HasOne(d => d.Zone)
					.WithMany(p => p.HooksPerceivables)
					.HasForeignKey(d => d.ZoneId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_Hooks_Perceivables_Zones");
			});

			modelBuilder.Entity<Improver>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Definition)
					.IsRequired()
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
					.HasColumnType("varchar(50)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
			});

			modelBuilder.Entity<Infection>(entity =>
			{
				entity.HasIndex(e => e.BodypartId)
					.HasDatabaseName("FK_Infections_Bodyparts_idx");

				entity.HasIndex(e => e.OwnerId)
					.HasDatabaseName("FK_Infections_Bodies_idx");

				entity.HasIndex(e => e.WoundId)
					.HasDatabaseName("FK_Infections_Wounds_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.BodypartId).HasColumnType("bigint(20)");

				entity.Property(e => e.InfectionType).HasColumnType("int(11)");

				entity.Property(e => e.OwnerId).HasColumnType("bigint(20)");

				entity.Property(e => e.Virulence).HasColumnType("int(11)");

				entity.Property(e => e.WoundId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.Bodypart)
					.WithMany(p => p.Infections)
					.HasForeignKey(d => d.BodypartId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_Infections_Bodyparts");

				entity.HasOne(d => d.Owner)
					.WithMany(p => p.Infections)
					.HasForeignKey(d => d.OwnerId)
					.HasConstraintName("FK_Infections_Bodies");

				entity.HasOne(d => d.Wound)
					.WithMany(p => p.Infections)
					.HasForeignKey(d => d.WoundId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_Infections_Wounds");
			});

			modelBuilder.Entity<ItemGroupForm>(entity =>
			{
				entity.HasIndex(e => e.ItemGroupId)
					.HasDatabaseName("FK_ItemGroupForms_ItemGroups_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.ItemGroupId).HasColumnType("bigint(20)");

				entity.Property(e => e.Type)
					.IsRequired()
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.HasOne(d => d.ItemGroup)
					.WithMany(p => p.ItemGroupForms)
					.HasForeignKey(d => d.ItemGroupId)
					.HasConstraintName("FK_ItemGroupForms_ItemGroups");
			});

			modelBuilder.Entity<ItemGroup>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Keywords)
					.HasColumnType("varchar(1000)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_general_ci");

				entity.Property(e => e.Name)
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_general_ci");
			});

			modelBuilder.Entity<Knowledge>(entity =>
			{
				entity.ToTable("knowledges");

				entity.HasIndex(e => e.CanAcquireProgId)
					.HasDatabaseName("FK_KNOWLEDGES_FUTUREPROGS_ACQUIRE_idx");

				entity.HasIndex(e => e.CanLearnProgId)
					.HasDatabaseName("FK_KNOWLEDGES_FUTUREPROGS_LEARN_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.CanAcquireProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.CanLearnProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.Description)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.LearnDifficulty)
					.HasColumnType("int(11)")
					.HasDefaultValueSql("'7'");

				entity.Property(e => e.LearnableType).HasColumnType("int(11)");

				entity.Property(e => e.LearningSessionsRequired).HasColumnType("int(11)");

				entity.Property(e => e.LongDescription)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Subtype)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.TeachDifficulty)
					.HasColumnType("int(11)")
					.HasDefaultValueSql("'7'");

				entity.Property(e => e.Type)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.CanAcquireProg)
					.WithMany(p => p.KnowledgesCanAcquireProg)
					.HasForeignKey(d => d.CanAcquireProgId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_KNOWLEDGES_FUTUREPROGS_ACQUIRE");

				entity.HasOne(d => d.CanLearnProg)
					.WithMany(p => p.KnowledgesCanLearnProg)
					.HasForeignKey(d => d.CanLearnProgId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_KNOWLEDGES_FUTUREPROGS_LEARN");
			});

			modelBuilder.Entity<KnowledgesCosts>(entity =>
			{
				entity.HasKey(e => new { e.KnowledgeId, e.ChargenResourceId })
					.HasName("PRIMARY");
				entity.Property(e => e.KnowledgeId).HasColumnType("bigint(20)");
				entity.Property(e => e.ChargenResourceId).HasColumnType("bigint(20)");
				entity.Property(e => e.Cost).HasColumnType("int(11)");
				entity.HasIndex(e => e.KnowledgeId).HasDatabaseName("FK_KnowledgesCosts_Knowledges_idx");
				entity.HasIndex(e => e.ChargenResourceId).HasDatabaseName("FK_KnowledgesCosts_ChargenResources_idx");

				entity.HasOne(e => e.Knowledge)
					.WithMany(e => e.KnowledgesCosts)
					.HasForeignKey(e => e.KnowledgeId)
					.HasConstraintName("FK_KnowledgesCosts_Knowledges");

				entity.HasOne(e => e.ChargenResource)
					.WithMany(e => e.KnowledgesCosts)
					.HasForeignKey(e => e.ChargenResourceId)
					.HasConstraintName("FK_KnowledgesCosts_ChargenResources");
			});

			modelBuilder.Entity<LanguageDifficultyModels>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("longtext")
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

			modelBuilder.Entity<Language>(entity =>
			{
				entity.HasIndex(e => e.DefaultLearnerAccentId)
					.HasDatabaseName("FK_Languages_Accents_idx");

				entity.HasIndex(e => e.DifficultyModel)
					.HasDatabaseName("FK_Languages_LanguageDifficultyModels");

				entity.HasIndex(e => e.LinkedTraitId)
					.HasDatabaseName("FK_Languages_TraitDefinitions");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.DefaultLearnerAccentId).HasColumnType("bigint(20)");

				entity.Property(e => e.DifficultyModel).HasColumnType("bigint(20)");

				entity.Property(e => e.LinkedTraitId).HasColumnType("bigint(20)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.UnknownLanguageDescription)
					.IsRequired()
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.DefaultLearnerAccent)
					.WithMany(p => p.Languages)
					.HasForeignKey(d => d.DefaultLearnerAccentId)
					.HasConstraintName("FK_Languages_Accents");

				entity.HasOne(d => d.DifficultyModelNavigation)
					.WithMany(p => p.Languages)
					.HasForeignKey(d => d.DifficultyModel)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Languages_LanguageDifficultyModels");

				entity.HasOne(d => d.LinkedTrait)
					.WithMany(p => p.Languages)
					.HasForeignKey(d => d.LinkedTraitId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Languages_TraitDefinitions");
			});

			modelBuilder.Entity<Law>(entity =>
			{
				entity.HasIndex(e => e.LawAppliesProgId)
					.HasDatabaseName("FK_Laws_FutureProgs_idx");

				entity.HasIndex(e => e.LegalAuthorityId)
					.HasDatabaseName("FK_Laws_LegalAuthority_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.CanBeAppliedAutomatically).HasColumnType("bit(1)");

				entity.Property(e => e.DoNotAutomaticallyApplyRepeats).HasColumnType("bit(1)");

				entity.Property(e => e.CanBeArrested).HasColumnType("bit(1)");

				entity.Property(e => e.CanBeOfferedBail).HasColumnType("bit(1)");

				entity.Property(e => e.CrimeType).HasColumnType("int(11)");

				entity.Property(e => e.EnforcementPriority).HasColumnType("int(11)");

				entity.Property(e => e.EnforcementStrategy)
					.IsRequired()
					.HasColumnType("varchar(500)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PunishmentStrategy)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.LawAppliesProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.LegalAuthorityId).HasColumnType("bigint(20)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(250)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.LawAppliesProg)
					.WithMany(p => p.Laws)
					.HasForeignKey(d => d.LawAppliesProgId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Laws_FutureProgs");

				entity.HasOne(d => d.LegalAuthority)
					.WithMany(p => p.Laws)
					.HasForeignKey(d => d.LegalAuthorityId)
					.HasConstraintName("FK_Laws_LegalAuthority");
			});

			modelBuilder.Entity<LawsOffenderClasses>(entity =>
			{
				entity.HasKey(e => new { e.LawId, e.LegalClassId })
					.HasName("PRIMARY");

				entity.ToTable("Laws_OffenderClasses");

				entity.HasIndex(e => e.LegalClassId)
					.HasDatabaseName("FK_Laws_OffenderClasses_LegalClasses_idx");

				entity.Property(e => e.LawId).HasColumnType("bigint(20)");

				entity.Property(e => e.LegalClassId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.Law)
					.WithMany(p => p.LawsOffenderClasses)
					.HasForeignKey(d => d.LawId)
					.HasConstraintName("FK_Laws_OffenderClasses_Laws");

				entity.HasOne(d => d.LegalClass)
					.WithMany(p => p.LawsOffenderClasses)
					.HasForeignKey(d => d.LegalClassId)
					.HasConstraintName("FK_Laws_OffenderClasses_LegalClasses");
			});

			modelBuilder.Entity<LawsVictimClasses>(entity =>
			{
				entity.HasKey(e => new { e.LawId, e.LegalClassId })
					.HasName("PRIMARY");

				entity.ToTable("Laws_VictimClasses");

				entity.HasIndex(e => e.LegalClassId)
					.HasDatabaseName("FK_Laws_VictimClasses_LegalClasses_idx");

				entity.Property(e => e.LawId).HasColumnType("bigint(20)");

				entity.Property(e => e.LegalClassId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.Law)
					.WithMany(p => p.LawsVictimClasses)
					.HasForeignKey(d => d.LawId)
					.HasConstraintName("FK_Laws_VictimClasses_Laws");

				entity.HasOne(d => d.LegalClass)
					.WithMany(p => p.LawsVictimClasses)
					.HasForeignKey(d => d.LegalClassId)
					.HasConstraintName("FK_Laws_VictimClasses_LegalClasses");
			});

			modelBuilder.Entity<LegalAuthority>(entity =>
			{
				entity.HasIndex(e => e.CurrencyId)
					.HasDatabaseName("FK_LegalAuthorities_Currencies_idx");

				entity.HasIndex(e => e.PreparingLocationId)
					.HasDatabaseName("FK_LegalAuthorities_PreparingCells_idx");

				entity.HasIndex(e => e.MarshallingLocationId)
					.HasDatabaseName("FK_LegalAuthorities_MarshallingCells_idx");

				entity.HasIndex(e => e.EnforcerStowingLocationId)
					.HasDatabaseName("FK_LegalAuthorities_StowingCells_idx");

				entity.HasIndex(e => e.PrisonLocationId)
					.HasDatabaseName("FK_LegalAuthorities_PrisonCells_idx");

				entity.HasIndex(e => e.PrisonReleaseLocationId)
					.HasDatabaseName("FK_LegalAuthorities_PrisonReleaseCells_idx");

				entity.HasIndex(e => e.PrisonBelongingsLocationId)
					.HasDatabaseName("FK_LegalAuthorities_PrisonBelongingsCells_idx");

				entity.HasIndex(e => e.OnReleaseProgId)
					.HasDatabaseName("FK_LegalAuthorities_FutureprogsRelease_idx");

				entity.HasIndex(e => e.OnImprisonProgId)
					.HasDatabaseName("FK_LegalAuthorities_FutureprogsImprison_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(250)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PreparingLocationId)
					.HasColumnType("bigint(20)");
				entity.Property(e => e.MarshallingLocationId)
					.HasColumnType("bigint(20)");
				entity.Property(e => e.EnforcerStowingLocationId)
					.HasColumnType("bigint(20)");
				entity.Property(e => e.PrisonLocationId)
					.HasColumnType("bigint(20)");
				entity.Property(e => e.PrisonReleaseLocationId)
					.HasColumnType("bigint(20)");
				entity.Property(e => e.PrisonBelongingsLocationId)
					.HasColumnType("bigint(20)");
				entity.Property(e => e.CourtLocationId)
					.HasColumnType("bigint(20)");
				entity.Property(e => e.OnReleaseProgId)
					.HasColumnType("bigint(20)");
				entity.Property(e => e.OnImprisonProgId)
					.HasColumnType("bigint(20)");
				entity.Property(e => e.OnHoldProgId)
					.HasColumnType("bigint(20)");
				entity.Property(e => e.BailCalculationProgId)
					.HasColumnType("bigint(20)");
				entity.Property(e => e.BankAccountId)
					.HasColumnType("bigint(20)");
				entity.Property(e => e.GuardianDiscordChannel)
					.HasColumnType("decimal(20,0)");
				entity.Property(e => e.PlayersKnowTheirCrimes)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'")
					;

				entity.HasOne(d => d.Currency)
					.WithMany(p => p.LegalAuthorities)
					.HasForeignKey(d => d.CurrencyId)
					.HasConstraintName("FK_LegalAuthorities_Currencies");
				entity.HasOne(d => d.PreparingLocation)
					.WithMany()
					.HasForeignKey(d => d.PreparingLocationId)
					.HasConstraintName("FK_LegalAuthorities_PreparingCells");

				entity.HasOne(d => d.MarshallingLocation)
					.WithMany()
					.HasForeignKey(d => d.MarshallingLocationId)
					.HasConstraintName("FK_LegalAuthorities_MarshallingCells");

				entity.HasOne(d => d.EnforcerStowingLocation)
					.WithMany()
					.HasForeignKey(d => d.EnforcerStowingLocationId)
					.HasConstraintName("FK_LegalAuthorities_StowingCells");

				entity.HasOne(d => d.PrisonLocation)
					.WithMany()
					.HasForeignKey(d => d.PrisonLocationId)
					.HasConstraintName("FK_LegalAuthorities_PrisonCells");

				entity.HasOne(d => d.JailLocation)
					.WithMany()
					.HasForeignKey(d => d.JailLocationId)
					.HasConstraintName("FK_LegalAuthorities_PrisonJailCells");

				entity.HasOne(d => d.CourtLocation)
					.WithMany()
					.HasForeignKey(d => d.CourtLocationId)
					.HasConstraintName("FK_LegalAuthorities_CourtroomCell");

				entity.HasOne(d => d.PrisonReleaseLocation)
					.WithMany()
					.HasForeignKey(d => d.PrisonReleaseLocationId)
					.HasConstraintName("FK_LegalAuthorities_PrisonReleaseCells");

				entity.HasOne(d => d.PrisonBelongingsLocation)
					.WithMany()
					.HasForeignKey(d => d.PrisonBelongingsLocationId)
					.HasConstraintName("FK_LegalAuthorities_PrisonBelongingsCells");

				entity.HasOne(d => d.OnReleaseProg)
					.WithMany()
					.HasForeignKey(d => d.OnReleaseProgId)
					.HasConstraintName("FK_LegalAuthorities_FutureprogsRelease");

				entity.HasOne(d => d.OnImprisonProg)
					.WithMany()
					.HasForeignKey(d => d.OnImprisonProgId)
					.HasConstraintName("FK_LegalAuthorities_FutureprogsImprison");

				entity.HasOne(d => d.OnHoldProg)
					.WithMany()
					.HasForeignKey(d => d.OnHoldProgId)
					.HasConstraintName("FK_LegalAuthorities_FutureprogsHold");

				entity.HasOne(d => d.BailCalculationProg)
					.WithMany()
					.HasForeignKey(d => d.BailCalculationProgId)
					.HasConstraintName("FK_LegalAuthorities_FutureprogsBailCalc");
			});

			modelBuilder.Entity<LegalAuthorityFine>(entity =>
			{
				entity.ToTable("LegalAuthorityFines");
				entity.HasKey(e => new { e.LegalAuthorityId, e.CharacterId }).HasName("PRIMARY");

				entity.Property(e => e.LegalAuthorityId)
					.HasColumnType("bigint(20)");
				entity.Property(e => e.CharacterId)
					.HasColumnType("bigint(20)");
				entity.Property(e => e.FinesOwned).HasColumnType("decimal(58,29)");
				entity.Property(e => e.PaymentRequiredBy)
					.IsRequired()
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity
					.HasOne(e => e.LegalAuthority)
					.WithMany(e => e.Fines)
					.HasForeignKey(e => e.LegalAuthorityId)
					.HasConstraintName("FK_LegalAuthorityFines_LegalAuthorities");

				entity
					.HasOne(e => e.Character)
					.WithMany(e => e.Fines)
					.HasForeignKey(e => e.CharacterId)
					.HasConstraintName("FK_LegalAuthorityFines_Characters");
			});

			modelBuilder.Entity<LegalAuthorityCells>(entity =>
			{
				entity.HasKey(e => new { e.LegalAuthorityId, e.CellId }).HasName("PRIMARY");
				entity.Property(e => e.LegalAuthorityId).HasColumnType("bigint(20)");
				entity.Property(e => e.CellId).HasColumnType("bigint(20)");
				entity.HasIndex(e => e.LegalAuthorityId).HasDatabaseName("FK_LegalAuthoritiesCells_LegalAuthorities_idx");
				entity.HasIndex(e => e.CellId).HasDatabaseName("FK_LegalAuthoritiesCells_Cells_idx");
				entity
					.HasOne(e => e.LegalAuthority)
					.WithMany(e => e.LegalAuthorityCells)
					.HasForeignKey(e => e.LegalAuthorityId)
					.HasConstraintName("FK_LegalAuthoritiesCells_LegalAuthorities");
				entity
					.HasOne(e => e.Cell)
					.WithMany()
					.HasForeignKey(e => e.CellId)
					.HasConstraintName("FK_LegalAuthoritiesCells_Cells");
			});

			modelBuilder.Entity<LegalAuthorityJailCell>(entity =>
			{
				entity.HasKey(e => new { e.LegalAuthorityId, e.CellId }).HasName("PRIMARY");
				entity.Property(e => e.LegalAuthorityId).HasColumnType("bigint(20)");
				entity.Property(e => e.CellId).HasColumnType("bigint(20)");
				entity.HasIndex(e => e.LegalAuthorityId).HasDatabaseName("FK_LegalAuthoritiesCells_LegalAuthorities_Jail_idx");
				entity.HasIndex(e => e.CellId).HasDatabaseName("FK_LegalAuthoritiesCells_Cells_Jail_idx");
				entity
					.HasOne(e => e.LegalAuthority)
					.WithMany(e => e.LegalAuthorityJailCells)
					.HasForeignKey(e => e.LegalAuthorityId)
					.HasConstraintName("FK_LegalAuthoritiesCells_LegalAuthorities_Jail");
				entity
					.HasOne(e => e.Cell)
					.WithMany()
					.HasForeignKey(e => e.CellId)
					.HasConstraintName("FK_LegalAuthoritiesCells_Cells_Jail");
			});

			modelBuilder.Entity<LegalAuthoritiesZones>(entity =>
			{
				entity.HasKey(e => new { e.ZoneId, e.LegalAuthorityId })
					.HasName("PRIMARY");

				entity.ToTable("LegalAuthorities_Zones");

				entity.HasIndex(e => e.LegalAuthorityId)
					.HasDatabaseName("FK_LegalAuthorities_Zones_LegalAuthorities_idx");

				entity.Property(e => e.ZoneId).HasColumnType("bigint(20)");

				entity.Property(e => e.LegalAuthorityId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.LegalAuthority)
					.WithMany(p => p.LegalAuthoritiesZones)
					.HasForeignKey(d => d.LegalAuthorityId)
					.HasConstraintName("FK_LegalAuthorities_Zones_LegalAuthorities");

				entity.HasOne(d => d.Zone)
					.WithMany(p => p.LegalAuthoritiesZones)
					.HasForeignKey(d => d.ZoneId)
					.HasConstraintName("FK_LegalAuthorities_Zones_Zones");
			});

			modelBuilder.Entity<LegalClass>(entity =>
			{
				entity.HasIndex(e => e.LegalAuthorityId)
					.HasDatabaseName("FK_LegalClasses_LegalAuthorities_idx");

				entity.HasIndex(e => e.MembershipProgId)
					.HasDatabaseName("FK_LegalClasses_FutureProgs_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.CanBeDetainedUntilFinesPaid).HasColumnType("bit(1)");

				entity.Property(e => e.LegalAuthorityId).HasColumnType("bigint(20)");

				entity.Property(e => e.LegalClassPriority).HasColumnType("int(11)");

				entity.Property(e => e.MembershipProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(250)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.LegalAuthority)
					.WithMany(p => p.LegalClasses)
					.HasForeignKey(d => d.LegalAuthorityId)
					.HasConstraintName("FK_LegalClasses_LegalAuthorities");

				entity.HasOne(d => d.MembershipProg)
					.WithMany(p => p.LegalClasses)
					.HasForeignKey(d => d.MembershipProgId)
					.HasConstraintName("FK_LegalClasses_FutureProgs");
			});

			modelBuilder.Entity<Limb>(entity =>
			{
				entity.HasIndex(e => e.RootBodyId)
					.HasDatabaseName("FK_Limbs_BodyProtos_idx");

				entity.HasIndex(e => e.RootBodypartId)
					.HasDatabaseName("FK_Limbs_BodypartProto_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.LimbType).HasColumnType("int(11)");

				entity.Property(e => e.Name)
					.HasColumnType("text")
					.HasCharSet("utf8mb4")
					.UseCollation("utf8mb4_unicode_ci");

				entity.Property(e => e.RootBodyId).HasColumnType("bigint(20)");

				entity.Property(e => e.RootBodypartId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.RootBody)
					.WithMany(p => p.Limbs)
					.HasForeignKey(d => d.RootBodyId)
					.HasConstraintName("FK_Limbs_BodyProtos");

				entity.HasOne(d => d.RootBodypart)
					.WithMany(p => p.Limbs)
					.HasForeignKey(d => d.RootBodypartId)
					.OnDelete(DeleteBehavior.ClientSetNull)
					.HasConstraintName("FK_Limbs_BodypartProto");
			});

			modelBuilder.Entity<LimbBodypartProto>(entity =>
			{
				entity.HasKey(e => new { e.BodypartProtoId, e.LimbId })
					.HasName("PRIMARY");

				entity.ToTable("Limbs_BodypartProto");

				entity.HasIndex(e => e.LimbId)
					.HasDatabaseName("FK_Limbs_BodypartProto_Limbs_idx");

				entity.Property(e => e.BodypartProtoId).HasColumnType("bigint(20)");

				entity.Property(e => e.LimbId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.BodypartProto)
					.WithMany(p => p.LimbsBodypartProto)
					.HasForeignKey(d => d.BodypartProtoId)
					.HasConstraintName("FK_Limbs_BodypartProto_BodypartProto");

				entity.HasOne(d => d.Limb)
					.WithMany(p => p.LimbsBodypartProto)
					.HasForeignKey(d => d.LimbId)
					.HasConstraintName("FK_Limbs_BodypartProto_Limbs");
			});

			modelBuilder.Entity<LimbsSpinalPart>(entity =>
			{
				entity.HasKey(e => new { e.LimbId, e.BodypartProtoId })
					.HasName("PRIMARY");

				entity.ToTable("Limbs_SpinalParts");

				entity.HasIndex(e => e.BodypartProtoId)
					.HasDatabaseName("FK_Limbs_SpinalParts_BodypartProtos_idx");

				entity.Property(e => e.LimbId).HasColumnType("bigint(20)");

				entity.Property(e => e.BodypartProtoId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.BodypartProto)
					.WithMany(p => p.LimbsSpinalParts)
					.HasForeignKey(d => d.BodypartProtoId)
					.HasConstraintName("FK_Limbs_SpinalParts_BodypartProtos");

				entity.HasOne(d => d.Limb)
					.WithMany(p => p.LimbsSpinalParts)
					.HasForeignKey(d => d.LimbId)
					.HasConstraintName("FK_Limbs_SpinalParts_Limbs");
			});

			modelBuilder.Entity<LineOfCreditAccount>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");
				entity.Property(e => e.ShopId).HasColumnType("bigint(20)");
				entity.Property(e => e.AccountOwnerId).HasColumnType("bigint(20)");
				entity.Property(e => e.AccountOwnerName).HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
				entity.Property(e => e.IsSuspended).HasColumnType("bit(1)");
				entity.Property(e => e.AccountLimit).HasColumnType("decimal(58,29)");
				entity.Property(e => e.OutstandingBalance).HasColumnType("decimal(58,29)");

				entity.HasIndex(e => e.ShopId).HasDatabaseName("FK_LineOfCreditAccounts_Shops_idx");
				entity.HasIndex(e => e.AccountOwnerId).HasDatabaseName("FK_LineOfCreditAccounts_Characters_idx");

				entity.HasOne(d => d.Shop)
					.WithMany(d => d.LineOfCreditAccounts)
					.HasForeignKey(d => d.ShopId)
					.HasConstraintName("FK_LineOfCreditAccounts_Shops");

				entity.HasOne(d => d.AccountOwner)
					.WithMany()
					.HasForeignKey(d => d.AccountOwnerId)
					.HasConstraintName("FK_LineOfCreditAccounts_Characters");
			});

			modelBuilder.Entity<LineOfCreditAccountUser>(entity =>
			{
				entity.HasKey(e => new { e.LineOfCreditAccountId, e.AccountUserId }).HasName("PRIMARY");
				entity.Property(e => e.LineOfCreditAccountId).HasColumnType("bigint(20)");
				entity.Property(e => e.AccountUserId).HasColumnType("bigint(20)");
				entity.Property(e => e.AccountUserName).HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
				entity.Property(e => e.SpendingLimit).HasColumnType("decimal(58,29)");

				entity.HasIndex(e => e.LineOfCreditAccountId).HasDatabaseName("FK_LineOfCreditAccountUsers_LineOfCreditAccounts_idx");
				entity.HasIndex(e => e.AccountUserId).HasDatabaseName("FK_LineOfCreditAccountUsers_Characters_idx");

				entity.HasOne(d => d.LineOfCreditAccount)
					.WithMany(d => d.AccountUsers)
					.HasForeignKey(d => d.LineOfCreditAccountId)
					.HasConstraintName("FK_LineOfCreditAccountUsers_LineOfCreditAccounts");

				entity.HasOne(d => d.AccountUser)
					.WithMany()
					.HasForeignKey(d => d.AccountUserId)
					.HasConstraintName("FK_LineOfCreditAccountUsers_Characters");
			});

			modelBuilder.Entity<Liquid>(entity =>
			{
				entity.HasIndex(e => e.CountAsId)
					.HasDatabaseName("FK_Liquids_Liquids_CountasAs_idx");

				entity.HasIndex(e => e.DriedResidueId)
					.HasDatabaseName("FK_Liquids_Materials_idx");

				entity.HasIndex(e => e.DrugId)
					.HasDatabaseName("FK_Liquids_Drugs_idx");

				entity.HasIndex(e => e.SolventId)
					.HasDatabaseName("FK_Liquids_Liquids_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.BoilingPoint).HasDefaultValueSql("'373.15'");

				entity.Property(e => e.CountAsId).HasColumnType("bigint(20)");

				entity.Property(e => e.CountAsQuality).HasColumnType("int(11)");

				entity.Property(e => e.DampDescription)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.DampShortDescription)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Density).HasDefaultValueSql("'1'");

				entity.Property(e => e.Description)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.DisplayColour)
					.IsRequired()
					.HasColumnType("varchar(45)")
					.HasDefaultValueSql("'blue'")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.DraughtProgId).HasColumnType("bigint(20)");

				entity.Property(e => e.DrenchedDescription)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.DrenchedShortDescription)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.DriedResidueId).HasColumnType("bigint(20)");

				entity.Property(e => e.DrinkSatiatedHoursPerLitre).HasDefaultValueSql("'12'");

				entity.Property(e => e.DrugId).HasColumnType("bigint(20)");

				entity.Property(e => e.ElectricalConductivity).HasDefaultValueSql("'0.005'");

				entity.Property(e => e.FreezingPoint).HasDefaultValueSql("'273.15'");
				entity.Property(e => e.GasFormId).HasColumnType("bigint(20)");

				entity.Property(e => e.InjectionConsequence).HasColumnType("int(11)");

				entity.Property(e => e.LongDescription)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Organic)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.ResidueVolumePercentage).HasDefaultValueSql("'0.05'");
				entity.Property(e => e.RelativeEnthalpy).HasDefaultValueSql("'1.0'");

				entity.Property(e => e.SmellIntensity).HasDefaultValueSql("'10'");

				entity.Property(e => e.SmellText)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.SolventId).HasColumnType("bigint(20)");

				entity.Property(e => e.SolventVolumeRatio).HasDefaultValueSql("'1'");

				entity.Property(e => e.SpecificHeatCapacity).HasDefaultValueSql("'4181'");

				entity.Property(e => e.TasteIntensity).HasDefaultValueSql("'100'");

				entity.Property(e => e.TasteText)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.ThermalConductivity).HasDefaultValueSql("'0.609'");

				entity.Property(e => e.VagueSmellText)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.VagueTasteText)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Viscosity).HasDefaultValueSql("'1'");

				entity.Property(e => e.WaterLitresPerLitre).HasDefaultValueSql("'1'");

				entity.Property(e => e.WetDescription)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.WetShortDescription)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.CountAs)
					.WithMany(p => p.InverseCountAs)
					.HasForeignKey(d => d.CountAsId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Liquids_Liquids_CountasAs");

				entity.HasOne(d => d.GasForm)
					.WithMany()
					.HasForeignKey(d => d.GasFormId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Liquids_Gases");

				entity.HasOne(d => d.DriedResidue)
					.WithMany(p => p.Liquids)
					.HasForeignKey(d => d.DriedResidueId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Liquids_Materials");

				entity.HasOne(d => d.Drug)
					.WithMany(p => p.Liquids)
					.HasForeignKey(d => d.DrugId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Liquids_Drugs");

				entity.HasOne(d => d.Solvent)
					.WithMany(p => p.InverseSolvent)
					.HasForeignKey(d => d.SolventId)
					.OnDelete(DeleteBehavior.SetNull)
					.HasConstraintName("FK_Liquids_Liquids");
			});

			modelBuilder.Entity<LiquidsTags>(entity =>
			{
				entity.HasKey(e => new { e.LiquidId, e.TagId })
					.HasName("PRIMARY");

				entity.ToTable("Liquids_Tags");

				entity.HasIndex(e => e.TagId)
					.HasDatabaseName("FK_Liquids_Tags_Tags_idx");

				entity.Property(e => e.LiquidId).HasColumnType("bigint(20)");

				entity.Property(e => e.TagId).HasColumnType("bigint(20)");

				entity.HasOne(d => d.Liquid)
					.WithMany(p => p.LiquidsTags)
					.HasForeignKey(d => d.LiquidId)
					.HasConstraintName("FK_Liquids_Tags_Liquids");

				entity.HasOne(d => d.Tag)
					.WithMany(p => p.LiquidsTags)
					.HasForeignKey(d => d.TagId)
					.HasConstraintName("FK_Liquids_Tags_Tags");
			});

			modelBuilder.Entity<Lock>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(255)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Strength).HasColumnType("int(11)");

				entity.Property(e => e.Style).HasColumnType("int(11)");
			});

			modelBuilder.Entity<LoginIp>(entity =>
			{
				entity.HasKey(e => new { e.IpAddress, e.AccountId })
					.HasName("PRIMARY");

				entity.ToTable("LoginIPs");

				entity.HasIndex(e => e.AccountId)
					.HasDatabaseName("FK_LoginIPs_Accounts");

				entity.Property(e => e.IpAddress)
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.AccountId).HasColumnType("bigint(20)");

				entity.Property(e => e.AccountRegisteredOnThisIp)
					.HasColumnName("AccountRegisteredOnThisIP")
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'0'");

				entity.Property(e => e.FirstDate).HasColumnType("datetime");

				entity.HasOne(d => d.Account)
					.WithMany(p => p.LoginIps)
					.HasForeignKey(d => d.AccountId)
					.HasConstraintName("FK_LoginIPs_Accounts");
			});

			modelBuilder.Entity<MagicCapability>(entity =>
			{
				entity.HasIndex(e => e.MagicSchoolId)
					.HasDatabaseName("FK_MagicCapabilities_MagicSchools_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.CapabilityModel)
					.IsRequired()
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.MagicSchoolId).HasColumnType("bigint(20)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PowerLevel)
					.HasColumnType("int(11)")
					.HasDefaultValueSql("'1'");

				entity.HasOne(d => d.MagicSchool)
					.WithMany(p => p.MagicCapabilities)
					.HasForeignKey(d => d.MagicSchoolId)
					.HasConstraintName("FK_MagicCapabilities_MagicSchools");
			});

			modelBuilder.Entity<MagicGenerator>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Type)
					.IsRequired()
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
			});

			modelBuilder.Entity<MagicPower>(entity =>
			{
				entity.HasIndex(e => e.MagicSchoolId)
					.HasDatabaseName("FK_MagicPowers_MagicSchools_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Blurb)
					.IsRequired()
					.HasColumnType("varchar(500)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.MagicSchoolId).HasColumnType("bigint(20)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.PowerModel)
					.IsRequired()
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.ShowHelp)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.MagicSchool)
					.WithMany(p => p.MagicPowers)
					.HasForeignKey(d => d.MagicSchoolId)
					.HasConstraintName("FK_MagicPowers_MagicSchools");
			});

			modelBuilder.Entity<MagicResource>(entity =>
			{
				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.MagicResourceType).HasColumnType("int(11)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.ShortName)
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.Type)
					.IsRequired()
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.BottomColour)
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci")
					.HasDefaultValue("\x1B[35m")
					;

				entity.Property(e => e.MidColour)
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci")
					.HasDefaultValue("\x1B[1;35m")
					;

				entity.Property(e => e.TopColour)
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci")
					.HasDefaultValue("\x1B[0m\x1B[38;5;171m")
					;
			});

			modelBuilder.Entity<MagicSchool>(entity =>
			{
				entity.HasIndex(e => e.ParentSchoolId)
					.HasDatabaseName("FK_MagicSchools_MagicSchools_idx");

				entity.Property(e => e.Id).HasColumnType("bigint(20)");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(200)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.ParentSchoolId).HasColumnType("bigint(20)");

				entity.Property(e => e.PowerListColour)
					.IsRequired()
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.SchoolAdjective)
					.IsRequired()
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.Property(e => e.SchoolVerb)
					.IsRequired()
					.HasColumnType("varchar(45)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasOne(d => d.ParentSchool)
					.WithMany(p => p.InverseParentSchool)
					.HasForeignKey(d => d.ParentSchoolId)
					.OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_MagicSchools_MagicSchools");
			});

			modelBuilder.Entity<MagicSpell>(entity => {
				entity.Property(e => e.Id).HasColumnType("bigint(20)");
				entity.Property(e => e.SpellKnownProgId).HasColumnType("bigint(20)");
				entity.Property(e => e.MagicSchoolId).HasColumnType("bigint(20)");
				entity.Property(e => e.ExclusiveDelay).HasColumnType("double");
				entity.Property(e => e.NonExclusiveDelay).HasColumnType("double");
				entity.Property(e => e.CastingTraitDefinitionId).HasColumnType("bigint(20)");
				entity.Property(e => e.ResistingTraitDefinitionId).HasColumnType("bigint(20)");
				entity.Property(e => e.EffectDurationExpressionId).HasColumnType("bigint(20)");
				entity.Property(e => e.CastingDifficulty).HasColumnType("int(11)");
				entity.Property(e => e.ResistingDifficulty).HasColumnType("int(11)");
				entity.Property(e => e.CastingEmoteFlags).HasColumnType("int(11)");
				entity.Property(e => e.TargetEmoteFlags).HasColumnType("int(11)");
				entity.Property(e => e.MinimumSuccessThreshold).HasColumnType("int(11)").HasDefaultValueSql("4");
				entity.Property(e => e.Name)
					.IsRequired()
					.HasColumnType("varchar(100)")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
				entity.Property(e => e.Blurb)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
				entity.Property(e => e.Description)
					.IsRequired()
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
				entity.Property(e => e.Definition)
					.IsRequired()
					.HasColumnType("mediumtext")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
				entity.Property(e => e.CastingEmote)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
				entity.Property(e => e.FailCastingEmote)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
				entity.Property(e => e.TargetEmote)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");
				entity.Property(e => e.TargetResistedEmote)
					.HasColumnType("text")
					.HasCharSet("utf8")
					.UseCollation("utf8_general_ci");

				entity.HasIndex(e => e.MagicSchoolId).HasDatabaseName("FK_MagicSpells_MagicSchools_idx");
				entity.HasIndex(e => e.SpellKnownProgId).HasDatabaseName("FK_MagicSpells_Futureprogs_idx");
				entity.HasIndex(e => e.EffectDurationExpressionId).HasDatabaseName("FK_MagicSpells_TraitExpressions_idx");
				entity.HasIndex(e => e.CastingTraitDefinitionId)
					.HasDatabaseName("FK_MagicSpells_TraitDefinitions_Casting_idx");
				entity.HasIndex(e => e.ResistingTraitDefinitionId)
					.HasDatabaseName("FK_MagicSpells_TraitDefinitions_Resisting_idx");

				entity
					.HasOne(e => e.MagicSchool)
					.WithMany(e => e.MagicSpells)
					.HasForeignKey(e => e.MagicSchoolId)
					.HasConstraintName("FK_MagicSpells_MagicSchools");

				entity
					.HasOne(e => e.SpellKnownProg)
					.WithMany()
					.HasForeignKey(e => e.SpellKnownProgId)
					.HasConstraintName("FK_MagicSpells_Futureprogs");

				entity
					.HasOne(e => e.CastingTraitDefinition)
					.WithMany()
					.HasForeignKey(e => e.CastingTraitDefinitionId)
					.HasConstraintName("FK_MagicSpells_TraitDefinitions_Casting");

				entity
					.HasOne(e => e.ResistingTraitDefinition)
					.WithMany()
					.HasForeignKey(e => e.ResistingTraitDefinitionId)
					.HasConstraintName("FK_MagicSpells_TraitDefinitions_Resisting");

				entity
					.HasOne(e => e.EffectDurationExpression)
					.WithMany()
					.HasForeignKey(e => e.EffectDurationExpressionId)
					.HasConstraintName("FK_MagicSpells_TraitExpressions");
			});
		}

	}
}
