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
        protected static void OnModelCreatingOne(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accent>(entity =>
            {
                entity.HasIndex(e => e.LanguageId)
                    .HasDatabaseName("FK_Accents_Languages");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ChargenAvailabilityProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Difficulty).HasColumnType("int(11)");

                entity.Property(e => e.Group)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LanguageId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Suffix)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.VagueSuffix)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.Accents)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Accents_Languages");
            });

            modelBuilder.Entity<AccountNote>(entity =>
            {
                entity.HasIndex(e => e.AccountId)
                    .HasDatabaseName("FK_AccountNotes_Accounts");

                entity.HasIndex(e => e.AuthorId)
                    .HasDatabaseName("FK_AccountNotes_Author");

                entity.HasIndex(e => e.CharacterId)
                .HasDatabaseName("FK_AccountNotes_Characters_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AccountId).HasColumnType("bigint(20)");

                entity.Property(e => e.AuthorId).HasColumnType("bigint(20)");

                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

                entity.Property(e => e.IsJournalEntry).HasColumnType("bit(1)").HasDefaultValueSql("b'0'");

                entity.Property(e => e.InGameTimeStamp)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountNotesAccount)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_AccountNotes_Accounts");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.AccountNotesAuthor)
                    .HasForeignKey(d => d.AuthorId)
                    .HasConstraintName("FK_AccountNotes_Author");

                entity.HasOne(d => d.Character)
                .WithMany()
                .HasForeignKey(d => d.CharacterId)
                .HasConstraintName("FK_AccountNotes_Characters");
            });

            modelBuilder.Entity<Models.Account>(entity =>
            {
                entity.HasIndex(e => e.AuthorityGroupId)
                    .HasDatabaseName("FK_Accounts_AuthorityGroups");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AccessStatus).HasColumnType("int(11)");

                entity.Property(e => e.ActiveCharactersAllowed)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.AppendNewlinesBetweenMultipleEchoesPerPrompt)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.AuthorityGroupId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CharacterNameOverlaySetting).HasColumnType("int(11)");

                entity.Property(e => e.CodedRoomDescriptionAdditionsOnNewLine)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.ActLawfully)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.HasBeenActiveInWeek)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

				entity.Property(e => e.HintsEnabled)
					.HasColumnType("bit(1)")
					.HasDefaultValueSql("b'1'");

				entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.CultureName)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Email)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.FormatLength)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'110'");

                entity.Property(e => e.InnerFormatLength)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'80'");

                entity.Property(e => e.IsRegistered)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastLoginIp)
                    .HasColumnName("LastLoginIP")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastLoginTime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PageLength)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'22'");

                entity.Property(e => e.Password)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.PromptType).HasColumnType("int(11)");

                entity.Property(e => e.RecoveryCode)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.RegistrationCode)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Salt).HasColumnType("bigint(20)");

                entity.Property(e => e.TabRoomDescriptions)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.TimeZoneId)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.UnitPreference)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.UseMccp)
                    .HasColumnName("UseMCCP")
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.UseMsp)
                    .HasColumnName("UseMSP")
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.UseMxp)
                    .HasColumnName("UseMXP")
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.UseUnicode)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.HasOne(d => d.AuthorityGroup)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.AuthorityGroupId)
                    .HasConstraintName("FK_Accounts_AuthorityGroups");
            });

            modelBuilder.Entity<AccountsChargenResources>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.ChargenResourceId })
                    .HasName("PRIMARY");

                entity.ToTable("Accounts_ChargenResources");

                entity.HasIndex(e => e.ChargenResourceId)
                    .HasDatabaseName("FK_Accounts_ChargenResources_ChargenResources");

                entity.Property(e => e.AccountId).HasColumnType("bigint(20)");

                entity.Property(e => e.ChargenResourceId).HasColumnType("bigint(20)");

                entity.Property(e => e.Amount).HasColumnType("int(11)");

                entity.Property(e => e.LastAwardDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountsChargenResources)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Accounts_ChargenResources_Accounts");

                entity.HasOne(d => d.ChargenResource)
                    .WithMany(p => p.AccountsChargenResources)
                    .HasForeignKey(d => d.ChargenResourceId)
                    .HasConstraintName("FK_Accounts_ChargenResources_ChargenResources");
            });

            modelBuilder.Entity<ActiveProjectLabour>(entity =>
            {
                entity.HasKey(e => new { e.ActiveProjectId, e.ProjectLabourRequirementsId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ProjectLabourRequirementsId)
                    .HasDatabaseName("FK_ActiveProjectLabours_ProjectLabourRequirements_idx");

                entity.Property(e => e.ActiveProjectId).HasColumnType("bigint(20)");

                entity.Property(e => e.ProjectLabourRequirementsId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.ActiveProject)
                    .WithMany(p => p.ActiveProjectLabours)
                    .HasForeignKey(d => d.ActiveProjectId)
                    .HasConstraintName("FK_ActiveProjectLabours_ActiveProjects");

                entity.HasOne(d => d.ProjectLabourRequirements)
                    .WithMany(p => p.ActiveProjectLabours)
                    .HasForeignKey(d => d.ProjectLabourRequirementsId)
                    .HasConstraintName("FK_ActiveProjectLabours_ProjectLabourRequirements");
            });

            modelBuilder.Entity<ActiveProjectMaterial>(entity =>
            {
                entity.HasKey(e => new { e.ActiveProjectId, e.ProjectMaterialRequirementsId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ProjectMaterialRequirementsId)
                    .HasDatabaseName("FK_ActiveProjectMaterials_ProjectMaterialRequirements_idx");

                entity.Property(e => e.ActiveProjectId).HasColumnType("bigint(20)");

                entity.Property(e => e.ProjectMaterialRequirementsId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.ActiveProject)
                    .WithMany(p => p.ActiveProjectMaterials)
                    .HasForeignKey(d => d.ActiveProjectId)
                    .HasConstraintName("FK_ActiveProjectMaterials_ActiveProjects");

                entity.HasOne(d => d.ProjectMaterialRequirements)
                    .WithMany(p => p.ActiveProjectMaterials)
                    .HasForeignKey(d => d.ProjectMaterialRequirementsId)
                    .HasConstraintName("FK_ActiveProjectMaterials_ProjectMaterialRequirements");
            });

            modelBuilder.Entity<ActiveProject>(entity =>
            {
                entity.HasIndex(e => e.CellId)
                    .HasDatabaseName("FK_ActiveProjects_Cells_idx");

                entity.HasIndex(e => e.CharacterId)
                    .HasDatabaseName("FK_ActiveProjects_Characters_idx");

                entity.HasIndex(e => e.CurrentPhaseId)
                    .HasDatabaseName("FK_ActiveProjects_ProjectPhases_idx");

                entity.HasIndex(e => new { e.ProjectId, e.ProjectRevisionNumber })
                    .HasDatabaseName("FK_ActiveProjects_Projects_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CellId).HasColumnType("bigint(20)");

                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrentPhaseId).HasColumnType("bigint(20)");

                entity.Property(e => e.ProjectId).HasColumnType("bigint(20)");

                entity.Property(e => e.ProjectRevisionNumber).HasColumnType("int(11)");

                entity.HasOne(d => d.Cell)
                    .WithMany(p => p.ActiveProjects)
                    .HasForeignKey(d => d.CellId)
                    .HasConstraintName("FK_ActiveProjects_Cells");

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.ActiveProjects)
                    .HasForeignKey(d => d.CharacterId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ActiveProjects_Characters");

                entity.HasOne(d => d.CurrentPhase)
                    .WithMany(p => p.ActiveProjects)
                    .HasForeignKey(d => d.CurrentPhaseId)
                    .OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_ActiveProjects_ProjectPhases");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ActiveProjects)
                    .HasForeignKey(d => new { d.ProjectId, d.ProjectRevisionNumber })
                    .OnDelete(DeleteBehavior.Cascade)
					.HasConstraintName("FK_ActiveProjects_Projects");
            });

            modelBuilder.Entity<Ally>(entity =>
            {
                entity.HasKey(e => new { e.CharacterId, e.AllyId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.AllyId)
                    .HasDatabaseName("FK_Allies_Characters_Target_idx");

                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

                entity.Property(e => e.AllyId).HasColumnType("bigint(20)");

                entity.Property(e => e.Trusted)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.HasOne(d => d.AllyCharacter)
                    .WithMany(p => p.AlliesAlly)
                    .HasForeignKey(d => d.AllyId)
                    .HasConstraintName("FK_Allies_Characters_Target");

                entity.HasOne(d => d.OwnerCharacter)
                    .WithMany(p => p.AlliesCharacter)
                    .HasForeignKey(d => d.CharacterId)
                    .HasConstraintName("FK_Allies_Characters_Owner");
            });

            modelBuilder.Entity<AmmunitionTypes>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BaseBlockDifficulty).HasColumnType("int(11)");

                entity.Property(e => e.BaseDodgeDifficulty).HasColumnType("int(11)");

                entity.Property(e => e.DamageExpression)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.DamageType).HasColumnType("int(11)");

                entity.Property(e => e.Loudness).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.PainExpression)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.RangedWeaponTypes)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.SpecificType)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.StunExpression)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasIndex(e => e.ClanId)
                    .HasDatabaseName("FK_Appointments_Clans");

                entity.HasIndex(e => e.MinimumRankId)
                    .HasDatabaseName("FK_Appointments_Ranks");

                entity.HasIndex(e => e.MinimumRankToAppointId)
                    .HasDatabaseName("FK_Appointments_Ranks_2");

                entity.HasIndex(e => e.ParentAppointmentId)
                    .HasDatabaseName("FK_Appointments_ParentAppointment");

                entity.HasIndex(e => e.PaygradeId)
                    .HasDatabaseName("FK_Appointments_Paygrades");

                entity.HasIndex(e => e.NumberOfVotesProgId).HasDatabaseName("FK_Appointments_NumberOfVotesProg_idx");
                entity.HasIndex(e => e.CanNominateProgId).HasDatabaseName("FK_Appointments_CanNominateProg_idx");
                entity.HasIndex(e => e.WhyCantNominateProgId).HasDatabaseName("FK_Appointments_WhyCantNominateProg_idx");

                entity.HasIndex(e => new { e.InsigniaGameItemId, e.InsigniaGameItemRevnum })
                    .HasDatabaseName("FK_Appointments_GameItemProtos");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ClanId).HasColumnType("bigint(20)");

                entity.Property(e => e.InsigniaGameItemId).HasColumnType("bigint(20)");

                entity.Property(e => e.InsigniaGameItemRevnum).HasColumnType("int(11)");

                entity.Property(e => e.FameType).HasColumnType("int(11)").HasDefaultValueSql("0");

                entity.Property(e => e.IsAppointedByElection).HasColumnType("bit(1)").HasDefaultValue(false);
                entity.Property(e => e.ElectionTermMinutes).HasColumnType("double");
                entity.Property(e => e.ElectionLeadTimeMinutes).HasColumnType("double");
                entity.Property(e => e.NominationPeriodMinutes).HasColumnType("double");
                entity.Property(e => e.VotingPeriodMinutes).HasColumnType("double");
                entity.Property(e => e.MaximumConsecutiveTerms).HasColumnType("int(11)");
                entity.Property(e => e.MaximumTotalTerms).HasColumnType("int(11)");
                entity.Property(e => e.IsSecretBallot).HasColumnType("bit(1)");
                entity.Property(e => e.CanNominateProgId).HasColumnType("bigint(20)");
                entity.Property(e => e.WhyCantNominateProgId).HasColumnType("bigint(20)");
                entity.Property(e => e.NumberOfVotesProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.MaximumSimultaneousHolders)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.MinimumRankId).HasColumnType("bigint(20)");

                entity.Property(e => e.MinimumRankToAppointId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ParentAppointmentId).HasColumnType("bigint(20)");

                entity.Property(e => e.PaygradeId).HasColumnType("bigint(20)");

                entity.Property(e => e.Privileges).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Clan)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.ClanId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Appointments_Clans");

                entity.HasOne(d => d.MinimumRank)
                    .WithMany(p => p.AppointmentsMinimumRank)
                    .HasForeignKey(d => d.MinimumRankId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Appointments_Ranks");

                entity.HasOne(d => d.MinimumRankToAppoint)
                    .WithMany(p => p.AppointmentsMinimumRankToAppoint)
                    .HasForeignKey(d => d.MinimumRankToAppointId)
                    .HasConstraintName("FK_Appointments_Ranks_2");

                entity.HasOne(d => d.ParentAppointment)
                    .WithMany(p => p.InverseParentAppointment)
                    .HasForeignKey(d => d.ParentAppointmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Appointments_ParentAppointment");

                entity.HasOne(d => d.Paygrade)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.PaygradeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Appointments_Paygrades");

                entity.HasOne(d => d.InsigniaGameItem)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => new { d.InsigniaGameItemId, d.InsigniaGameItemRevnum })
                    .HasConstraintName("FK_Appointments_GameItemProtos");

                entity.HasOne(d => d.CanNominateProg)
                    .WithMany()
                    .HasForeignKey((d => d.CanNominateProgId))
                    .HasConstraintName("FK_Appointments_CanNominateProg");

                entity.HasOne(d => d.WhyCantNominateProg)
                    .WithMany()
                    .HasForeignKey((d => d.WhyCantNominateProgId))
                    .HasConstraintName("FK_Appointments_WhyCantNominateProg");

                entity.HasOne(d => d.NumberOfVotesProg)
                    .WithMany()
                    .HasForeignKey(d => d.NumberOfVotesProgId)
                    .HasConstraintName("FK_Appointments_NumberOfVotesProg");
            });

            modelBuilder.Entity<AppointmentsAbbreviations>(entity =>
            {
                entity.HasKey(e => new { e.Abbreviation, e.AppointmentId })
                    .HasName("PRIMARY");

                entity.ToTable("Appointments_Abbreviations");

                entity.HasIndex(e => e.AppointmentId)
                    .HasDatabaseName("FK_Appointments_Abbreviations_Appointments");

                entity.HasIndex(e => e.FutureProgId)
                    .HasDatabaseName("FK_Appointments_Abbreviations_FutureProgs");

                entity.Property(e => e.Abbreviation)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.AppointmentId).HasColumnType("bigint(20)");

                entity.Property(e => e.FutureProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.Order).HasColumnType("int(11)");

                entity.HasOne(d => d.Appointment)
                    .WithMany(p => p.AppointmentsAbbreviations)
                    .HasForeignKey(d => d.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Appointments_Abbreviations_Appointments");

                entity.HasOne(d => d.FutureProg)
                    .WithMany(p => p.AppointmentsAbbreviations)
                    .HasForeignKey(d => d.FutureProgId)
                    .HasConstraintName("FK_Appointments_Abbreviations_FutureProgs");
            });

            modelBuilder.Entity<AppointmentsTitles>(entity =>
            {
                entity.HasKey(e => new { e.Title, e.AppointmentId })
                    .HasName("PRIMARY");

                entity.ToTable("Appointments_Titles");

                entity.HasIndex(e => e.AppointmentId)
                    .HasDatabaseName("FK_Appointments_Titles_Appointments");

                entity.HasIndex(e => e.FutureProgId)
                    .HasDatabaseName("FK_Appointments_Titles_FutureProgs");

                entity.Property(e => e.Title)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.AppointmentId).HasColumnType("bigint(20)");

                entity.Property(e => e.FutureProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.Order).HasColumnType("int(11)");

                entity.HasOne(d => d.Appointment)
                    .WithMany(p => p.AppointmentsTitles)
                    .HasForeignKey(d => d.AppointmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Appointments_Titles_Appointments");

                entity.HasOne(d => d.FutureProg)
                    .WithMany(p => p.AppointmentsTitles)
                    .HasForeignKey(d => d.FutureProgId)
                    .HasConstraintName("FK_Appointments_Titles_FutureProgs");
            });

            modelBuilder.Entity<Areas>(entity =>
            {
                entity.HasIndex(e => e.WeatherControllerId)
                    .HasDatabaseName("FK_Areas_WeatherControllers_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.WeatherControllerId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.WeatherController)
                    .WithMany(p => p.Areas)
                    .HasForeignKey(d => d.WeatherControllerId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Areas_WeatherControllers");
            });

            modelBuilder.Entity<AreasRooms>(entity =>
            {
                entity.HasKey(e => new { e.AreaId, e.RoomId })
                    .HasName("PRIMARY");

                entity.ToTable("Areas_Rooms");

                entity.HasIndex(e => e.RoomId)
                    .HasDatabaseName("FK_Areas_Rooms_Rooms_idx");

                entity.Property(e => e.AreaId).HasColumnType("bigint(20)");

                entity.Property(e => e.RoomId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.AreasRooms)
                    .HasForeignKey(d => d.AreaId)
                    .HasConstraintName("FK_Areas_Rooms_Areas");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.AreasRooms)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK_Areas_Rooms_Rooms");
            });

            modelBuilder.Entity<ArmourType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BaseDifficultyDegrees).HasColumnType("int(11)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.MinimumPenetrationDegree).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.StackedDifficultyDegrees).HasColumnType("int(11)");
            });

            modelBuilder.Entity<ArtificialIntelligence>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("mediumtext")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
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

            modelBuilder.Entity<AuctionHouse>(entity =>
            {
                entity.ToTable("AuctionHouses");
                entity.Property(e => e.Id).HasColumnType("bigint(20)");
                entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");
                entity.Property(e => e.AuctionHouseCellId).HasColumnType("bigint(20)");
                entity.Property(e => e.ProfitsBankAccountId).HasColumnType("bigint(20)");
                entity.Property(e => e.DefaultListingTime).HasColumnType("double");
                entity.Property(e => e.AuctionListingFeeFlat).HasColumnType("decimal(58,29)");
                entity.Property(e => e.AuctionListingFeeRate).HasColumnType("decimal(58,29)");
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("mediumtext")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.EconomicZone)
                    .WithMany()
                    .HasForeignKey(d => d.EconomicZoneId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_AuctionHouses_EconomicZones");
                entity.HasOne(d => d.AuctionHouseCell)
                    .WithMany()
                    .HasForeignKey(d => d.AuctionHouseCellId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_AuctionHouses_Cells");
                entity.HasOne(d => d.ProfitsBankAccount)
                    .WithMany()
                    .HasForeignKey(d => d.ProfitsBankAccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_AuctionHouses_BankAccounts");
            });

            modelBuilder.Entity<AuthorityGroup>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AccountsLevel).HasColumnType("int(11)");

                entity.Property(e => e.AuthorityLevel).HasColumnType("int(11)");

                entity.Property(e => e.CharacterApprovalLevel).HasColumnType("int(11)");

                entity.Property(e => e.CharacterApprovalRisk).HasColumnType("int(11)");

                entity.Property(e => e.CharactersLevel).HasColumnType("int(11)");

                entity.Property(e => e.InformationLevel).HasColumnType("int(11)");

                entity.Property(e => e.ItemsLevel).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PlanesLevel).HasColumnType("int(11)");

                entity.Property(e => e.RoomsLevel).HasColumnType("int(11)");
            });

            modelBuilder.Entity<AutobuilderAreaTemplate>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TemplateType)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<AutobuilderRoomTemplate>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TemplateType)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Ban>(entity =>
            {
                entity.HasIndex(e => e.BannerAccountId)
                    .HasDatabaseName("FK_Bans_Accounts");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BannerAccountId).HasColumnType("bigint(20)");

                entity.Property(e => e.Expiry).HasColumnType("datetime");

                entity.Property(e => e.IpMask)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.BannerAccount)
                    .WithMany(p => p.Bans)
                    .HasForeignKey(d => d.BannerAccountId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Bans_Accounts");
            });

            modelBuilder.Entity<BloodModel>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<BloodModelsBloodtypes>(entity =>
            {
                entity.HasKey(e => new { e.BloodModelId, e.BloodtypeId })
                    .HasName("PRIMARY");

                entity.ToTable("BloodModels_Bloodtypes");

                entity.HasIndex(e => e.BloodtypeId)
                    .HasDatabaseName("FK_BloodModels_Bloodtypes_Bloodtypes_idx");

                entity.Property(e => e.BloodModelId).HasColumnType("bigint(20)");

                entity.Property(e => e.BloodtypeId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.BloodModel)
                    .WithMany(p => p.BloodModelsBloodtypes)
                    .HasForeignKey(d => d.BloodModelId)
                    .HasConstraintName("FK_BloodModels_Bloodtypes_BloodModels");

                entity.HasOne(d => d.Bloodtype)
                    .WithMany(p => p.BloodModelsBloodtypes)
                    .HasForeignKey(d => d.BloodtypeId)
                    .HasConstraintName("FK_BloodModels_Bloodtypes_Bloodtypes");
            });

            modelBuilder.Entity<BloodtypeAntigen>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Bloodtype>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<BloodtypesBloodtypeAntigens>(entity =>
            {
                entity.HasKey(e => new { e.BloodtypeId, e.BloodtypeAntigenId })
                    .HasName("PRIMARY");

                entity.ToTable("Bloodtypes_BloodtypeAntigens");

                entity.HasIndex(e => e.BloodtypeAntigenId)
                    .HasDatabaseName("FK_Bloodtypes_BloodtypeAntigens_BloodtypeAntigens_idx");

                entity.Property(e => e.BloodtypeId).HasColumnType("bigint(20)");

                entity.Property(e => e.BloodtypeAntigenId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.BloodtypeAntigen)
                    .WithMany(p => p.BloodtypesBloodtypeAntigens)
                    .HasForeignKey(d => d.BloodtypeAntigenId)
                    .HasConstraintName("FK_Bloodtypes_BloodtypeAntigens_BloodtypeAntigens");

                entity.HasOne(d => d.Bloodtype)
                    .WithMany(p => p.BloodtypesBloodtypeAntigens)
                    .HasForeignKey(d => d.BloodtypeId)
                    .HasConstraintName("FK_Bloodtypes_BloodtypeAntigens_Bloodtypes");
            });

            modelBuilder.Entity<BoardPost>(entity =>
            {
                entity.HasIndex(e => e.AuthorId)
                    .HasDatabaseName("FK_BoardsPosts_Accounts_idx");

                entity.HasIndex(e => e.BoardId)
                    .HasDatabaseName("FK_BoardPosts_Boards_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AuthorId).HasColumnType("bigint(20)");

                entity.Property(e => e.BoardId).HasColumnType("bigint(20)");

                entity.Property(e => e.AuthorIsCharacter)
                    .HasColumnType("bit(1)")
                    .HasDefaultValue(false);

                entity.Property(e => e.InGameDateTime)
                    .IsRequired(false)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PostTime).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.AuthorName)
                    .IsRequired(false)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.AuthorShortDescription)
                    .IsRequired(false)
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.AuthorFullDescription)
                    .IsRequired(false)
                    .HasColumnType("varchar(8000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.Board)
                    .WithMany(p => p.BoardPosts)
                    .HasForeignKey(d => d.BoardId)
                    .HasConstraintName("FK_BoardPosts_Boards");
            });

            modelBuilder.Entity<Board>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ShowOnLogin).HasColumnType("bit(1)");

                entity.Property(e => e.CalendarId)
                    .HasColumnType("bigint(20)");

                entity
                    .HasOne(e => e.Calendar)
                    .WithMany()
                    .HasForeignKey(e => e.CalendarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Boards_Calendars");
            });

            modelBuilder.Entity<Models.Body>(entity =>
            {
                entity.HasIndex(e => e.BloodtypeId)
                    .HasDatabaseName("FK_Bodies_Bloodtypes_idx");

                entity.HasIndex(e => e.EthnicityId)
                    .HasDatabaseName("FK_Bodies_Ethnicities_idx");

                entity.HasIndex(e => e.FullDescriptionPatternId)
                    .HasDatabaseName("FK_Bodies_EntityDescriptionPatterns_Full_idx");

                entity.HasIndex(e => e.RaceId)
                    .HasDatabaseName("FK_Bodies_Races");

                entity.HasIndex(e => e.ShortDescriptionPatternId)
                    .HasDatabaseName("FK_Bodies_EntityDescriptionPatterns_Short_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BloodtypeId).HasColumnType("bigint(20)");

                entity.Property(e => e.BodyPrototypeId)
                    .HasColumnName("BodyPrototypeID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.CurrentBloodVolume).HasDefaultValueSql("'-1'");

                entity.Property(e => e.CurrentSpeed).HasColumnType("bigint(20)");

                entity.Property(e => e.EffectData)
                    .IsRequired()
                    .HasColumnType("mediumtext")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.EthnicityId).HasColumnType("bigint(20)");

                entity.Property(e => e.FullDescription)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.FullDescriptionPatternId).HasColumnType("bigint(20)");

                entity.Property(e => e.Gender).HasColumnType("smallint(6)");

                entity.Property(e => e.HeldBreathLength).HasColumnType("int(11)");

                entity.Property(e => e.Position).HasColumnType("bigint(20)");

                entity.Property(e => e.RaceId).HasColumnType("bigint(20)");

                entity.Property(e => e.Scars)
                    .HasColumnType("mediumtext")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ShortDescription)
                    .HasColumnType("varchar(1000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ShortDescriptionPatternId).HasColumnType("bigint(20)");

                entity.Property(e => e.Tattoos)
                    .HasColumnType("mediumtext")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.Bloodtype)
                    .WithMany(p => p.Bodies)
                    .HasForeignKey(d => d.BloodtypeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Bodies_Bloodtypes");

                entity.HasOne(d => d.Ethnicity)
                    .WithMany(p => p.Bodies)
                    .HasForeignKey(d => d.EthnicityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bodies_Ethnicities");

                entity.HasOne(d => d.FullDescriptionPattern)
                    .WithMany(p => p.BodiesFullDescriptionPattern)
                    .HasForeignKey(d => d.FullDescriptionPatternId)
                    .HasConstraintName("FK_Bodies_EntityDescriptionPatterns_Full");

                entity.HasOne(d => d.Race)
                    .WithMany(p => p.Bodies)
                    .HasForeignKey(d => d.RaceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Bodies_Races");

                entity.HasOne(d => d.ShortDescriptionPattern)
                    .WithMany(p => p.BodiesShortDescriptionPattern)
                    .HasForeignKey(d => d.ShortDescriptionPatternId)
                    .HasConstraintName("FK_Bodies_EntityDescriptionPatterns_Short");
            });

            modelBuilder.Entity<BodyDrugDose>(entity =>
            {
                entity.HasKey(e => new { e.BodyId, e.DrugId, e.Active })
                    .HasName("PRIMARY");

                entity.ToTable("Bodies_DrugDoses");

                entity.HasIndex(e => e.DrugId)
                    .HasDatabaseName("FK_Bodies_DrugDoses_Drugs_idx");

                entity.Property(e => e.BodyId).HasColumnType("bigint(20)");

                entity.Property(e => e.DrugId).HasColumnType("bigint(20)");

                entity.Property(e => e.Active).HasColumnType("bit(1)");

                entity.Property(e => e.OriginalVector).HasColumnType("int(11)");

                entity.HasOne(d => d.Body)
                    .WithMany(p => p.BodiesDrugDoses)
                    .HasForeignKey(d => d.BodyId)
                    .HasConstraintName("FK_Bodies_DrugDoses_Bodies");

                entity.HasOne(d => d.Drug)
                    .WithMany(p => p.BodiesDrugDoses)
                    .HasForeignKey(d => d.DrugId)
                    .HasConstraintName("FK_Bodies_DrugDoses_Drugs");
            });

            modelBuilder.Entity<BodiesGameItems>(entity =>
            {
                entity.HasKey(e => new { e.BodyId, e.GameItemId })
                    .HasName("PRIMARY");

                entity.ToTable("Bodies_GameItems");

                entity.HasIndex(e => e.GameItemId)
                    .HasDatabaseName("FK_Bodies_GameItems_GameItems");

                entity.Property(e => e.BodyId).HasColumnType("bigint(20)");

                entity.Property(e => e.GameItemId).HasColumnType("bigint(20)");

                entity.Property(e => e.EquippedOrder).HasColumnType("int(11)");

                entity.Property(e => e.WearProfile).HasColumnType("bigint(20)");

                entity.Property(e => e.Wielded).HasColumnType("int(11)");

                entity.HasOne(d => d.Body)
                    .WithMany(p => p.BodiesGameItems)
                    .HasForeignKey(d => d.BodyId)
                    .HasConstraintName("FK_Bodies_GameItems_Bodies");

                entity.HasOne(d => d.GameItem)
                    .WithMany(p => p.BodiesGameItems)
                    .HasForeignKey(d => d.GameItemId)
                    .HasConstraintName("FK_Bodies_GameItems_GameItems");
            });

            modelBuilder.Entity<BodiesImplants>(entity =>
            {
                entity.HasKey(e => new { e.BodyId, e.ImplantId })
                    .HasName("PRIMARY");

                entity.ToTable("Bodies_Implants");

                entity.HasIndex(e => e.ImplantId)
                    .HasDatabaseName("FK_Bodies_Implants_GameItems_idx");

                entity.Property(e => e.BodyId).HasColumnType("bigint(20)");

                entity.Property(e => e.ImplantId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Body)
                    .WithMany(p => p.BodiesImplants)
                    .HasForeignKey(d => d.BodyId)
                    .HasConstraintName("FK_Bodies_Implants_Bodies");

                entity.HasOne(d => d.Implant)
                    .WithMany(p => p.BodiesImplants)
                    .HasForeignKey(d => d.ImplantId)
                    .HasConstraintName("FK_Bodies_Implants_GameItems");
            });

            modelBuilder.Entity<BodiesProsthetics>(entity =>
            {
                entity.HasKey(e => new { e.BodyId, e.ProstheticId })
                    .HasName("PRIMARY");

                entity.ToTable("Bodies_Prosthetics");

                entity.HasIndex(e => e.ProstheticId)
                    .HasDatabaseName("FK_Bodies_Prosthetics_GameItems_idx");

                entity.Property(e => e.BodyId).HasColumnType("bigint(20)");

                entity.Property(e => e.ProstheticId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Body)
                    .WithMany(p => p.BodiesProsthetics)
                    .HasForeignKey(d => d.BodyId)
                    .HasConstraintName("FK_Bodies_Prosthetics_Bodies");

                entity.HasOne(d => d.Prosthetic)
                    .WithMany(p => p.BodiesProsthetics)
                    .HasForeignKey(d => d.ProstheticId)
                    .HasConstraintName("FK_Bodies_Prosthetics_GameItems");
            });

            modelBuilder.Entity<BodiesSeveredParts>(entity =>
            {
                entity.HasKey(e => new { e.BodiesId, e.BodypartProtoId })
                    .HasName("PRIMARY");

                entity.ToTable("Bodies_SeveredParts");

                entity.HasIndex(e => e.BodypartProtoId)
                    .HasDatabaseName("FK_Bodies_SeveredParts_BodypartProtos_idx");

                entity.Property(e => e.BodiesId).HasColumnType("bigint(20)");

                entity.Property(e => e.BodypartProtoId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Bodies)
                    .WithMany(p => p.BodiesSeveredParts)
                    .HasForeignKey(d => d.BodiesId)
                    .HasConstraintName("FK_Bodies_SeveredParts_Bodies");

                entity.HasOne(d => d.BodypartProto)
                    .WithMany(p => p.BodiesSeveredParts)
                    .HasForeignKey(d => d.BodypartProtoId)
                    .HasConstraintName("FK_Bodies_SeveredParts_BodypartProtos");
            });

            modelBuilder.Entity<BodyProtosPositions>(entity => {
                entity.HasKey(e => new { e.BodyProtoId, e.Position });

                entity.Property(e => e.BodyProtoId).HasColumnType("bigint(20)");
                entity.Property(e => e.Position).HasColumnType("int(11)");

                entity.HasIndex(e => e.BodyProtoId)
                    .HasDatabaseName("FK_BodyProtosPositions_BodyProtos_idx");

                entity.HasOne(d => d.BodyProto)
                .WithMany(p => p.BodyProtosPositions)
                .HasForeignKey(d => d.BodyProtoId)
                .HasConstraintName("FK_BodyProtosPositions_BodyProtos");
            });

            modelBuilder.Entity<BodyProto>(entity =>
            {
                entity.HasIndex(e => e.CountsAsId)
                    .HasDatabaseName("FK_BodyPrototype_BodyPrototype_idx");

                entity.HasIndex(e => e.DefaultSmashingBodypartId)
                    .HasDatabaseName("FK_BodyPrototype_Bodyparts_idx");

                entity.HasIndex(e => e.WearSizeParameterId)
                    .HasDatabaseName("FK_BodyPrototype_WearableSizeParameterRule");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ConsiderString)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CountsAsId).HasColumnType("bigint(20)");

                entity.Property(e => e.DefaultSmashingBodypartId).HasColumnType("bigint(20)");

                entity.Property(e => e.LegDescriptionPlural)
                    .IsRequired()
                    .HasColumnType("varchar(1000)")
                    .HasDefaultValueSql("'legs'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LegDescriptionSingular)
                    .IsRequired()
                    .HasColumnType("varchar(1000)")
                    .HasDefaultValueSql("'leg'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.MinimumLegsToStand)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'2'");

                entity.Property(e => e.MinimumWingsToFly)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'2'");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.StaminaRecoveryProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.WearSizeParameterId).HasColumnType("bigint(20)");

                entity.Property(e => e.WielderDescriptionPlural)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasDefaultValueSql("'hands'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.WielderDescriptionSingle)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasDefaultValueSql("'hand'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.CountsAs)
                    .WithMany(p => p.InverseCountsAs)
                    .HasForeignKey(d => d.CountsAsId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_BodyPrototype_BodyPrototype");

                entity.HasOne(d => d.DefaultSmashingBodypart)
                    .WithMany(p => p.BodyProtos)
                    .HasForeignKey(d => d.DefaultSmashingBodypartId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_BodyPrototype_Bodyparts");

                entity.HasOne(d => d.WearSizeParameter)
                    .WithMany(p => p.BodyProtos)
                    .HasForeignKey(d => d.WearSizeParameterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BodyPrototype_WearableSizeParameterRule");
            });

            modelBuilder.Entity<BodyProtosAdditionalBodyparts>(entity =>
            {
                entity.HasKey(e => new { e.BodyProtoId, e.BodypartId, e.Usage })
                    .HasName("PRIMARY");

                entity.ToTable("BodyProtos_AdditionalBodyparts");

                entity.HasIndex(e => e.BodypartId)
                    .HasDatabaseName("FK_BodyProtos_AdditionalBodyparts_BodypartProto_idx");
                entity.HasIndex(e => e.BodyProtoId)
                    .HasDatabaseName("FK_BodyProtos_AdditionalBodyparts_BodyProtos_idx");

                entity.Property(e => e.BodyProtoId).HasColumnType("bigint(20)");

                entity.Property(e => e.BodypartId).HasColumnType("bigint(20)");

                entity.Property(e => e.Usage)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.BodyProto)
                    .WithMany(p => p.BodyProtosAdditionalBodyparts)
                    .HasForeignKey(d => d.BodyProtoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BodyProtos_AdditionalBodyparts_BodyProtos");

                entity.HasOne(d => d.Bodypart)
                    .WithMany(p => p.BodyProtosAdditionalBodyparts)
                    .HasForeignKey(d => d.BodypartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BodyProtos_AdditionalBodyparts_BodypartProto");
            });

            modelBuilder.Entity<BodypartGroupDescriber>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Comment)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DescribedAs)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<BodypartGroupDescribersBodyProtos>(entity =>
            {
                entity.HasKey(e => new { e.BodypartGroupDescriberId, e.BodyProtoId })
                    .HasName("PRIMARY");

                entity.ToTable("BodypartGroupDescribers_BodyProtos");

                entity.HasIndex(e => e.BodyProtoId)
                    .HasDatabaseName("FK_BGD_BodyProtos_BodyProtos");

                entity.Property(e => e.BodypartGroupDescriberId).HasColumnType("bigint(20)");

                entity.Property(e => e.BodyProtoId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.BodyProto)
                    .WithMany(p => p.BodypartGroupDescribersBodyProtos)
                    .HasForeignKey(d => d.BodyProtoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BGD_BodyProtos_BodyProtos");

                entity.HasOne(d => d.BodypartGroupDescriber)
                    .WithMany(p => p.BodypartGroupDescribersBodyProtos)
                    .HasForeignKey(d => d.BodypartGroupDescriberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BGD_BodyProtos_BodypartGroupDescribers");
            });

            modelBuilder.Entity<BodypartGroupDescribersBodypartProtos>(entity =>
            {
                entity.HasKey(e => new { e.BodypartGroupDescriberId, e.BodypartProtoId })
                    .HasName("PRIMARY");

                entity.ToTable("BodypartGroupDescribers_BodypartProtos");

                entity.HasIndex(e => e.BodypartProtoId)
                    .HasDatabaseName("FK_BGD_BodypartProtos_BodypartProto");

                entity.Property(e => e.BodypartGroupDescriberId).HasColumnType("bigint(20)");

                entity.Property(e => e.BodypartProtoId).HasColumnType("bigint(20)");

                entity.Property(e => e.Mandatory)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.HasOne(d => d.BodypartGroupDescriber)
                    .WithMany(p => p.BodypartGroupDescribersBodypartProtos)
                    .HasForeignKey(d => d.BodypartGroupDescriberId)
                    .HasConstraintName("FK_BGD_BodypartProtos_BodypartGroupDescribers");

                entity.HasOne(d => d.BodypartProto)
                    .WithMany(p => p.BodypartGroupDescribersBodypartProtos)
                    .HasForeignKey(d => d.BodypartProtoId)
                    .HasConstraintName("FK_BGD_BodypartProtos_BodypartProto");
            });

            modelBuilder.Entity<BodypartGroupDescribersShapeCount>(entity =>
            {
                entity.HasKey(e => new { e.BodypartGroupDescriptionRuleId, e.TargetId })
                    .HasName("PRIMARY");

                entity.ToTable("BodypartGroupDescribers_ShapeCount");

                entity.HasIndex(e => e.TargetId)
                    .HasDatabaseName("FK_BGD_ShapeCount_BodypartShape");

                entity.Property(e => e.BodypartGroupDescriptionRuleId).HasColumnType("bigint(20)");

                entity.Property(e => e.TargetId).HasColumnType("bigint(20)");

                entity.Property(e => e.MaxCount).HasColumnType("int(11)");

                entity.Property(e => e.MinCount).HasColumnType("int(11)");

                entity.HasOne(d => d.BodypartGroupDescriptionRule)
                    .WithMany(p => p.BodypartGroupDescribersShapeCount)
                    .HasForeignKey(d => d.BodypartGroupDescriptionRuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BGD_ShapeCount_BodypartGroupDescribers");

                entity.HasOne(d => d.Target)
                    .WithMany(p => p.BodypartGroupDescribersShapeCount)
                    .HasForeignKey(d => d.TargetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BGD_ShapeCount_BodypartShape");
            });

            modelBuilder.Entity<BodypartInternalInfos>(entity =>
            {
                entity.HasKey(e => new { e.BodypartProtoId, e.InternalPartId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.InternalPartId)
                    .HasDatabaseName("FK_BodypartInternalInfos_BodypartProtos_Internal_idx");
                entity.HasIndex(e => e.BodypartProtoId)
                    .HasDatabaseName("FK_BodypartInternalInfos_BodypartProtos_idx");

                entity.Property(e => e.BodypartProtoId).HasColumnType("bigint(20)");

                entity.Property(e => e.InternalPartId).HasColumnType("bigint(20)");

                entity.Property(e => e.HitChance).HasDefaultValueSql("'5'");

                entity.Property(e => e.IsPrimaryOrganLocation)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.ProximityGroup)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.BodypartProto)
                    .WithMany(p => p.BodypartInternalInfosBodypartProto)
                    .HasForeignKey(d => d.BodypartProtoId)
                    .HasConstraintName("FK_BodypartInternalInfos_BodypartProtos");

                entity.HasOne(d => d.InternalPart)
                    .WithMany(p => p.BodypartInternalInfosInternalPart)
                    .HasForeignKey(d => d.InternalPartId)
                    .HasConstraintName("FK_BodypartInternalInfos_BodypartProtos_Internal");
            });

            modelBuilder.Entity<BodypartProto>(entity =>
            {
                entity.ToTable("BodypartProto");
                entity.HasIndex(e => e.ArmourTypeId)
                    .HasDatabaseName("FK_BodypartProto_ArmourTypes_idx");

                entity.HasIndex(e => e.BodyId)
                    .HasDatabaseName("FK_BodypartProto_BodyPrototype");

                entity.HasIndex(e => e.BodypartShapeId)
                    .HasDatabaseName("FK_BodypartProto_BodypartShape");

                entity.HasIndex(e => e.CountAsId)
                    .HasDatabaseName("FK_BodypartProto_BodypartProto_idx");

                entity.HasIndex(e => e.DefaultMaterialId)
                    .HasDatabaseName("FK_BodypartProto_Materials_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Alignment).HasColumnType("int(11)");

                entity.Property(e => e.ArmourTypeId).HasColumnType("bigint(20)");

                entity.Property(e => e.BleedModifier).HasDefaultValueSql("'0.1'");

                entity.Property(e => e.BodyId).HasColumnType("bigint(20)");

                entity.Property(e => e.BodypartShapeId).HasColumnType("bigint(20)");

                entity.Property(e => e.BodypartType).HasColumnType("int(11)");

                entity.Property(e => e.CountAsId).HasColumnType("bigint(20)");

                entity.Property(e => e.DamageModifier).HasDefaultValueSql("'1'");

                entity.Property(e => e.DefaultMaterialId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DisplayOrder).HasColumnType("int(11)");

                entity.Property(e => e.HypoxiaDamagePerTick).HasDefaultValueSql("'0.2'");

                entity.Property(e => e.IsCore)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.IsOrgan).HasColumnType("int(11)");

                entity.Property(e => e.IsVital)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Location).HasColumnType("int(11)");

                entity.Property(e => e.MaxLife)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'100'");

                entity.Property(e => e.MaxSingleSize).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PainModifier).HasDefaultValueSql("'1'");

                entity.Property(e => e.RelativeHitChance)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'100'");

                entity.Property(e => e.RelativeInfectability).HasDefaultValueSql("'1'");

                entity.Property(e => e.SeveredThreshold)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'100'");

                entity.Property(e => e.Significant).HasColumnType("bit(1)");

                entity.Property(e => e.Size)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'5'");

                entity.Property(e => e.StunModifier).HasDefaultValueSql("'1'");

                entity.Property(e => e.Unary).HasColumnType("bit(1)");

                entity.HasOne(d => d.ArmourType)
                    .WithMany(p => p.BodypartProto)
                    .HasForeignKey(d => d.ArmourTypeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_BodypartProto_ArmourTypes");

                entity.HasOne(d => d.Body)
                    .WithMany(p => p.BodypartProtos)
                    .HasForeignKey(d => d.BodyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BodypartProto_BodyPrototype");

                entity.HasOne(d => d.BodypartShape)
                    .WithMany(p => p.BodypartProto)
                    .HasForeignKey(d => d.BodypartShapeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BodypartProto_BodypartShape");

                entity.HasOne(d => d.CountAs)
                    .WithMany(p => p.InverseCountAs)
                    .HasForeignKey(d => d.CountAsId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_BodypartProto_BodypartProto");

                entity.HasOne(d => d.DefaultMaterial)
                    .WithMany(p => p.BodypartProto)
                    .HasForeignKey(d => d.DefaultMaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BodypartProto_Materials");
            });

            modelBuilder.Entity<BodypartProtoAlignmentHits>(entity =>
            {
                entity.ToTable("BodypartProto_AlignmentHits");

                entity.HasIndex(e => e.BodypartProtoId)
                    .HasDatabaseName("FK_BodypartProto_AlignmentHits_BodypartProto");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Alignment).HasColumnType("int(11)");

                entity.Property(e => e.BodypartProtoId).HasColumnType("bigint(20)");

                entity.Property(e => e.HitChance).HasColumnType("int(11)");

                entity.HasOne(d => d.BodypartProto)
                    .WithMany(p => p.BodypartProtoAlignmentHits)
                    .HasForeignKey(d => d.BodypartProtoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BodypartProto_AlignmentHits_BodypartProto");
            });

            modelBuilder.Entity<BodypartProtoBodypartProtoUpstream>(entity =>
            {
                entity.HasKey(e => new { e.Child, e.Parent })
                    .HasName("PRIMARY");

                entity.ToTable("BodypartProto_BodypartProto_Upstream");

                entity.HasIndex(e => e.Parent)
                    .HasDatabaseName("FKParent");

                entity.Property(e => e.Child).HasColumnType("bigint(20)");

                entity.Property(e => e.Parent).HasColumnType("bigint(20)");

                entity.HasOne(d => d.ChildNavigation)
                    .WithMany(p => p.BodypartProtoBodypartProtoUpstreamChildNavigation)
                    .HasForeignKey(d => d.Child)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKChild");

                entity.HasOne(d => d.ParentNavigation)
                    .WithMany(p => p.BodypartProtoBodypartProtoUpstreamParentNavigation)
                    .HasForeignKey(d => d.Parent)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FKParent");
            });

            modelBuilder.Entity<BodypartProtoOrientationHits>(entity =>
            {
                entity.ToTable("BodypartProto_OrientationHits");

                entity.HasIndex(e => e.BodypartProtoId)
                    .HasDatabaseName("FK_BodypartProto_OrientationHits_BodypartProto");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BodypartProtoId).HasColumnType("bigint(20)");

                entity.Property(e => e.HitChance).HasColumnType("int(11)");

                entity.Property(e => e.Orientation).HasColumnType("int(11)");

                entity.HasOne(d => d.BodypartProto)
                    .WithMany(p => p.BodypartProtoOrientationHits)
                    .HasForeignKey(d => d.BodypartProtoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BodypartProto_OrientationHits_BodypartProto");
            });

            modelBuilder.Entity<BodypartShape>(entity =>
            {
                entity.ToTable("BodypartShape");
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Bodypartshapecountview>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("bodypartshapecountview");

                entity.Property(e => e.BodypartGroupDescriptionRuleId).HasColumnType("tinyint(4)");

                entity.Property(e => e.DescribedAs).HasColumnType("tinyint(4)");

                entity.Property(e => e.MaxCount).HasColumnType("tinyint(4)");

                entity.Property(e => e.MinCount).HasColumnType("tinyint(4)");

                entity.Property(e => e.Name).HasColumnType("tinyint(4)");

                entity.Property(e => e.TargetId).HasColumnType("tinyint(4)");
            });

            modelBuilder.Entity<BoneOrganCoverage>(entity =>
            {
                entity.HasKey(e => new { e.BoneId, e.OrganId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.OrganId)
                    .HasDatabaseName("FK_BoneOrganCoverages_BodypartProto_Organ_idx");

                entity.Property(e => e.BoneId).HasColumnType("bigint(20)");

                entity.Property(e => e.OrganId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Bone)
                    .WithMany(p => p.BoneOrganCoveragesBone)
                    .HasForeignKey(d => d.BoneId)
                    .HasConstraintName("FK_BoneOrganCoverages_BodypartProto_Bone");

                entity.HasOne(d => d.Organ)
                    .WithMany(p => p.BoneOrganCoveragesOrgan)
                    .HasForeignKey(d => d.OrganId)
                    .HasConstraintName("FK_BoneOrganCoverages_BodypartProto_Organ");
            });

            modelBuilder.Entity<ButcheryProductItems>(entity =>
            {
                entity.HasIndex(e => e.ButcheryProductId)
                    .HasDatabaseName("FK_ButcheryProductItems_ButcheryProducts_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ButcheryProductId).HasColumnType("bigint(20)");

                entity.Property(e => e.ButcheryProductItemscol)
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DamageThreshold).HasDefaultValueSql("'10'");

                entity.Property(e => e.DamagedProtoId).HasColumnType("bigint(20)");

                entity.Property(e => e.DamagedQuantity).HasColumnType("int(11)");

                entity.Property(e => e.NormalProtoId).HasColumnType("bigint(20)");

                entity.Property(e => e.NormalQuantity).HasColumnType("int(11)");

                entity.HasOne(d => d.ButcheryProduct)
                    .WithMany(p => p.ButcheryProductItems)
                    .HasForeignKey(d => d.ButcheryProductId)
                    .HasConstraintName("FK_ButcheryProductItems_ButcheryProducts");
            });

            modelBuilder.Entity<ButcheryProducts>(entity =>
            {
                entity.HasIndex(e => e.CanProduceProgId)
                    .HasDatabaseName("FK_ButcheryProducts_FutureProgs_idx");

                entity.HasIndex(e => e.TargetBodyId)
                    .HasDatabaseName("FK_ButcheryProducts_BodyProtos_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CanProduceProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.IsPelt).HasColumnType("bit(1)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Subcategory)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TargetBodyId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.CanProduceProg)
                    .WithMany(p => p.ButcheryProducts)
                    .HasForeignKey(d => d.CanProduceProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_ButcheryProducts_FutureProgs");

                entity.HasOne(d => d.TargetBody)
                    .WithMany(p => p.ButcheryProducts)
                    .HasForeignKey(d => d.TargetBodyId)
                    .HasConstraintName("FK_ButcheryProducts_BodyProtos");
            });

            modelBuilder.Entity<ButcheryProductsBodypartProtos>(entity =>
            {
                entity.HasKey(e => new { e.ButcheryProductId, e.BodypartProtoId })
                    .HasName("PRIMARY");

                entity.ToTable("ButcheryProducts_BodypartProtos");

                entity.HasIndex(e => e.BodypartProtoId)
                    .HasDatabaseName("FK_ButcheryProducts_BodypartProtos_BodypartProtos_idx");

                entity.Property(e => e.ButcheryProductId).HasColumnType("bigint(20)");

                entity.Property(e => e.BodypartProtoId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.BodypartProto)
                    .WithMany(p => p.ButcheryProductsBodypartProtos)
                    .HasForeignKey(d => d.BodypartProtoId)
                    .HasConstraintName("FK_ButcheryProducts_BodypartProtos_BodypartProtos");

                entity.HasOne(d => d.ButcheryProduct)
                    .WithMany(p => p.ButcheryProductsBodypartProtos)
                    .HasForeignKey(d => d.ButcheryProductId)
                    .HasConstraintName("FK_ButcheryProducts_BodypartProtos_ButcheryProducts");
            });

            modelBuilder.Entity<Calendar>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Date)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.FeedClockId).HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<Models.Celestial>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CelestialYear).HasColumnType("int(11)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CelestialType)
                    .IsRequired()
                    .HasColumnType("varchar(30)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci")
                    .HasDefaultValue("OldSun");

                entity.Property(e => e.FeedClockId).HasColumnType("bigint(20)");

                entity.Property(e => e.LastYearBump).HasColumnType("int(11)");

                entity.Property(e => e.Minutes).HasColumnType("int(11)");
            });

            modelBuilder.Entity<CellOverlayPackage>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.RevisionNumber })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.EditableItemId)
                    .HasDatabaseName("FK_CellOverlayPackages_EditableItems");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.RevisionNumber).HasColumnType("int(11)");

                entity.Property(e => e.EditableItemId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.EditableItem)
                    .WithMany(p => p.CellOverlayPackages)
                    .HasForeignKey(d => d.EditableItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CellOverlayPackages_EditableItems");
            });

            modelBuilder.Entity<CellOverlay>(entity =>
            {
                entity.HasIndex(e => e.CellId)
                    .HasDatabaseName("FK_CellOverlays_Cells");

                entity.HasIndex(e => e.HearingProfileId)
                    .HasDatabaseName("FK_CellOverlays_HearingProfiles");

                entity.HasIndex(e => e.TerrainId)
                    .HasDatabaseName("FK_CellOverlays_Terrains");

                entity.HasIndex(e => new { e.CellOverlayPackageId, e.CellOverlayPackageRevisionNumber })
                    .HasDatabaseName("FK_CellOverlays_CellOverlayPackages");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AmbientLightFactor).HasDefaultValueSql("'1'");

                entity.Property(e => e.AtmosphereId).HasColumnType("bigint(20)");

                entity.Property(e => e.AtmosphereType)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasDefaultValueSql("'gas'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CellDescription)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CellId).HasColumnType("bigint(20)");

                entity.Property(e => e.CellName)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CellOverlayPackageId).HasColumnType("bigint(20)");

                entity.Property(e => e.CellOverlayPackageRevisionNumber).HasColumnType("int(11)");

                entity.Property(e => e.HearingProfileId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.OutdoorsType).HasColumnType("int(11)");

                entity.Property(e => e.SafeQuit).HasColumnType("bit(1)").HasDefaultValueSql("b'1'");

                entity.Property(e => e.TerrainId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Cell)
                    .WithMany(p => p.CellOverlays)
                    .HasForeignKey(d => d.CellId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CellOverlays_Cells");

                entity.HasOne(d => d.HearingProfile)
                    .WithMany(p => p.CellOverlays)
                    .HasForeignKey(d => d.HearingProfileId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_CellOverlays_HearingProfiles");

                entity.HasOne(d => d.Terrain)
                    .WithMany(p => p.CellOverlays)
                    .HasForeignKey(d => d.TerrainId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_CellOverlays_Terrains");

                entity.HasOne(d => d.CellOverlayPackage)
                    .WithMany(p => p.CellOverlays)
                    .HasForeignKey(d => new { d.CellOverlayPackageId, d.CellOverlayPackageRevisionNumber })
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CellOverlays_CellOverlayPackages");
            });

            modelBuilder.Entity<CellOverlayExit>(entity =>
            {
                entity.HasKey(e => new { e.CellOverlayId, e.ExitId })
                    .HasName("PRIMARY");

                entity.ToTable("CellOverlays_Exits");

                entity.HasIndex(e => e.ExitId)
                    .HasDatabaseName("FK_CellOverlays_Exits_Exits");

                entity.Property(e => e.CellOverlayId).HasColumnType("bigint(20)");

                entity.Property(e => e.ExitId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.CellOverlay)
                    .WithMany(p => p.CellOverlaysExits)
                    .HasForeignKey(d => d.CellOverlayId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CellOverlays_Exits_CellOverlays");

                entity.HasOne(d => d.Exit)
                    .WithMany(p => p.CellOverlaysExits)
                    .HasForeignKey(d => d.ExitId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CellOverlays_Exits_Exits");
            });

            modelBuilder.Entity<Cell>(entity =>
            {
                entity.HasIndex(e => e.CurrentOverlayId)
                    .HasDatabaseName("FK_Cells_CellOverlays");

                entity.HasIndex(e => e.RoomId)
                    .HasDatabaseName("FK_Cells_Rooms");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrentOverlayId).HasColumnType("bigint(20)");

                entity.Property(e => e.EffectData)
                    .IsRequired()
                    .HasColumnType("mediumtext")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ForagableProfileId).HasColumnType("bigint(20)");

                entity.Property(e => e.RoomId).HasColumnType("bigint(20)");

                entity.Property(e => e.Temporary)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.HasOne(d => d.CurrentOverlay)
                    .WithMany(p => p.Cells)
                    .HasForeignKey(d => d.CurrentOverlayId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Cells_CellOverlays");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Cells)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Cells_Rooms");
            });

            modelBuilder.Entity<CellsForagableYield>(entity =>
            {
                entity.HasKey(e => new { e.CellId, e.ForagableType })
                    .HasName("PRIMARY");

                entity.ToTable("Cells_ForagableYields");

                entity.Property(e => e.CellId).HasColumnType("bigint(20)");

                entity.Property(e => e.ForagableType)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.HasOne(d => d.Cell)
                    .WithMany(p => p.CellsForagableYields)
                    .HasForeignKey(d => d.CellId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Cells_ForagableYields_Cells");
            });

            modelBuilder.Entity<CellsGameItems>(entity =>
            {
                entity.HasKey(e => new { e.CellId, e.GameItemId })
                    .HasName("PRIMARY");

                entity.ToTable("Cells_GameItems");

                entity.HasIndex(e => e.GameItemId)
                    .HasDatabaseName("FK_Cells_GameItems_GameItems");

                entity.Property(e => e.CellId).HasColumnType("bigint(20)");

                entity.Property(e => e.GameItemId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Cell)
                    .WithMany(p => p.CellsGameItems)
                    .HasForeignKey(d => d.CellId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Cells_GameItems_Cells");

                entity.HasOne(d => d.GameItem)
                    .WithMany(p => p.CellsGameItems)
                    .HasForeignKey(d => d.GameItemId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Cells_GameItems_GameItems");
            });

            modelBuilder.Entity<CellMagicResource>(entity =>
            {
                entity.HasKey(e => new { e.CellId, e.MagicResourceId })
                    .HasName("PRIMARY");

                entity.ToTable("Cells_MagicResources");

                entity.HasIndex(e => e.MagicResourceId)
                    .HasDatabaseName("FK_Cells_MagicResources_MagicResources_idx");

                entity.Property(e => e.CellId).HasColumnType("bigint(20)");

                entity.Property(e => e.MagicResourceId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Cell)
                    .WithMany(p => p.CellsMagicResources)
                    .HasForeignKey(d => d.CellId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Cells_MagicResources_Cells");

                entity.HasOne(d => d.MagicResource)
                    .WithMany(p => p.CellsMagicResources)
                    .HasForeignKey(d => d.MagicResourceId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Cells_MagicResources_MagicResources");
            });

            modelBuilder.Entity<CellsRangedCovers>(entity =>
            {
                entity.HasKey(e => new { e.CellId, e.RangedCoverId })
                    .HasName("PRIMARY");

                entity.ToTable("Cells_RangedCovers");

                entity.HasIndex(e => e.RangedCoverId)
                    .HasDatabaseName("FK_Cells_RangedCovers_RangedCovers_idx");

                entity.Property(e => e.CellId).HasColumnType("bigint(20)");

                entity.Property(e => e.RangedCoverId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Cell)
                    .WithMany(p => p.CellsRangedCovers)
                    .HasForeignKey(d => d.CellId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Cells_RangedCovers_Cells");

                entity.HasOne(d => d.RangedCover)
                    .WithMany(p => p.CellsRangedCovers)
                    .HasForeignKey(d => d.RangedCoverId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Cells_RangedCovers_RangedCovers");
            });

            modelBuilder.Entity<CellsTags>(entity =>
            {
                entity.HasKey(e => new { e.CellId, e.TagId })
                    .HasName("PRIMARY");

                entity.ToTable("Cells_Tags");

                entity.HasIndex(e => e.TagId)
                    .HasDatabaseName("FK_Cells_Tags_Tags_idx");

                entity.Property(e => e.CellId).HasColumnType("bigint(20)");

                entity.Property(e => e.TagId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Cell)
                    .WithMany(p => p.CellsTags)
                    .HasForeignKey(d => d.CellId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Cells_Tags_Cells");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.CellsTags)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Cells_Tags_Tags");
            });

            modelBuilder.Entity<ChannelCommandWord>(entity =>
            {
                entity.HasKey(e => e.Word)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ChannelId)
                    .HasDatabaseName("FK_ChannelCommandWords_Channels");

                entity.Property(e => e.Word)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ChannelId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Channel)
                    .WithMany(p => p.ChannelCommandWords)
                    .HasForeignKey(d => d.ChannelId)
                    .HasConstraintName("FK_ChannelCommandWords_Channels");
            });

            modelBuilder.Entity<ChannelIgnorer>(entity =>
            {
                entity.HasKey(e => new { e.ChannelId, e.AccountId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.AccountId)
                    .HasDatabaseName("FK_ChannelIgnorers_Accounts");

                entity.Property(e => e.ChannelId).HasColumnType("bigint(20)");

                entity.Property(e => e.AccountId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.ChannelIgnorers)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_ChannelIgnorers_Accounts");

                entity.HasOne(d => d.Channel)
                    .WithMany(p => p.ChannelIgnorers)
                    .HasForeignKey(d => d.ChannelId)
                    .HasConstraintName("FK_ChannelIgnorers_Channels");
            });

            modelBuilder.Entity<Channel>(entity =>
            {
                entity.HasIndex(e => e.ChannelListenerProgId)
                    .HasDatabaseName("FK_Channels_FutureProgs_Listener");

                entity.HasIndex(e => e.ChannelSpeakerProgId)
                    .HasDatabaseName("FK_Channels_FutureProgs_Speaker");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AddToGuideCommandTree)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.AddToPlayerCommandTree)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.AnnounceChannelJoiners).HasColumnType("bit(1)");

                entity.Property(e => e.AnnounceMissedListeners).HasColumnType("bit(1)");

                entity.Property(e => e.ChannelColour)
                    .IsRequired()
                    .HasColumnType("char(10)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ChannelListenerProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.ChannelName)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ChannelSpeakerProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.Mode).HasColumnType("int(11)");

                entity.HasOne(d => d.ChannelListenerProg)
                    .WithMany(p => p.ChannelsChannelListenerProg)
                    .HasForeignKey(d => d.ChannelListenerProgId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Channels_FutureProgs_Listener");

                entity.HasOne(d => d.ChannelSpeakerProg)
                    .WithMany(p => p.ChannelsChannelSpeakerProg)
                    .HasForeignKey(d => d.ChannelSpeakerProgId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Channels_FutureProgs_Speaker");
            });

            modelBuilder.Entity<CharacterCombatSetting>(entity =>
            {
                entity.HasIndex(e => e.AvailabilityProgId)
                    .HasDatabaseName("FK_CharacterCombatSettings_FutureProgs_idx");

                entity.HasIndex(e => e.CharacterOwnerId)
                    .HasDatabaseName("FK_CharacterCombatSettings_Characters_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AttackCriticallyInjured)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.AttackUnarmedOrHelpless)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.AutomaticallyMoveTowardsTarget)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.AvailabilityProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.CharacterOwnerId).HasColumnType("bigint(20)");

                entity.Property(e => e.ClassificationsAllowed)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.DefaultPreferredDefenseType).HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.FallbackToUnarmedIfNoWeapon)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.ForbiddenIntentions).HasColumnType("bigint(20)");

                entity.Property(e => e.GlobalTemplate)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.GrappleResponse).HasColumnType("int(11)");

                entity.Property(e => e.InventoryManagement).HasColumnType("int(11)");

                entity.Property(e => e.ManualPositionManagement)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.MeleeAttackOrderPreference)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasDefaultValueSql("'0 1 2 3 4'")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.MoveToMeleeIfCannotEngageInRangedCombat)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.MovementManagement).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.PreferFavouriteWeapon)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.PreferNonContactClinchBreaking)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'");

                entity.Property(e => e.PreferShieldUse)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.PreferToFightArmed)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.PreferredIntentions).HasColumnType("bigint(20)");

                entity.Property(e => e.PreferredMeleeMode).HasColumnType("int(11)");

                entity.Property(e => e.PreferredRangedMode)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.PreferredWeaponSetup).HasColumnType("int(11)");

                entity.Property(e => e.PursuitMode)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.RangedManagement).HasColumnType("int(11)");

                entity.Property(e => e.RequiredIntentions).HasColumnType("bigint(20)");

                entity.Property(e => e.RequiredMinimumAim).HasDefaultValueSql("'0.5'");

                entity.Property(e => e.SkirmishToOtherLocations)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.HasOne(d => d.AvailabilityProg)
                    .WithMany(p => p.CharacterCombatSettings)
                    .HasForeignKey(d => d.AvailabilityProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_CharacterCombatSettings_FutureProgs");

                entity.HasOne(d => d.CharacterOwner)
                    .WithMany(p => p.CharacterCombatSettings)
                    .HasForeignKey(d => d.CharacterOwnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CharacterCombatSettings_Characters");
            });

            modelBuilder.Entity<CharacterIntroTemplate>(entity =>
            {
                entity.HasIndex(e => e.AppliesToCharacterProgId)
                    .HasDatabaseName("FK_CharacterIntroTemplates_FutureProgs_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AppliesToCharacterProgId).HasColumnType("bigint(20)");

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

                entity.Property(e => e.Order).HasColumnType("int(11)");

                entity.Property(e => e.ResolutionPriority)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.AppliesToCharacterProg)
                    .WithMany(p => p.CharacterIntroTemplates)
                    .HasForeignKey(d => d.AppliesToCharacterProgId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CharacterIntroTemplates_FutureProgs");
            });

            modelBuilder.Entity<CharacterKnowledge>(entity =>
            {
                entity.HasIndex(e => e.CharacterId)
                    .HasDatabaseName("FK_CHARACTERKNOWLEDGES_CHARACTERS");

                entity.HasIndex(e => e.KnowledgeId)
                    .HasDatabaseName("FK_CHARACTERKNOWLEDGES_KNOWLEDGES_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

                entity.Property(e => e.HowAcquired)
                    .IsRequired()
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.KnowledgeId).HasColumnType("bigint(20)");

                entity.Property(e => e.TimesTaught).HasColumnType("int(11)");

                entity.Property(e => e.WhenAcquired).HasColumnType("datetime");

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.CharacterKnowledges)
                    .HasForeignKey(d => d.CharacterId)
                    .HasConstraintName("FK_CHARACTERKNOWLEDGES_CHARACTERS");

                entity.HasOne(d => d.Knowledge)
                    .WithMany(p => p.CharacterKnowledges)
                    .HasForeignKey(d => d.KnowledgeId)
                    .HasConstraintName("FK_CHARACTERKNOWLEDGES_KNOWLEDGES");
            });

            modelBuilder.Entity<CharacterLog>(entity =>
            {
                entity.ToTable("CharacterLog");
                entity.HasIndex(e => e.AccountId)
                    .HasDatabaseName("FK_CharacterLog_Accounts_idx");

                entity.HasIndex(e => e.CellId)
                    .HasDatabaseName("FK_CharacterLog_Cells_idx");

                entity.HasIndex(e => e.CharacterId)
                    .HasDatabaseName("FK_CharacterLog_Characters_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AccountId).HasColumnType("bigint(20)");

                entity.Property(e => e.CellId).HasColumnType("bigint(20)");

                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

                entity.Property(e => e.Command)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.IsPlayerCharacter).HasColumnType("bit(1)");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.CharacterLog)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CharacterLog_Accounts");

                entity.HasOne(d => d.Cell)
                    .WithMany(p => p.CharacterLog)
                    .HasForeignKey(d => d.CellId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CharacterLog_Cells");

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.CharacterLog)
                    .HasForeignKey(d => d.CharacterId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CharacterLog_Characters");
            });

            modelBuilder.Entity<CharacteristicDefinition>(entity =>
            {
                entity.HasIndex(e => e.ParentId)
                    .HasDatabaseName("FK_CharacteristicDefinitions_Parent");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ChargenDisplayType).HasColumnType("int(11)");

                entity.Property(e => e.Definition)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasColumnType("varchar(45)")
                    .HasDefaultValueSql("'standard'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ParentId).HasColumnType("bigint(20)");

                entity.Property(e => e.Pattern)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Type).HasColumnType("int(11)");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_CharacteristicDefinitions_Parent");
            });

            modelBuilder.Entity<CharacteristicProfile>(entity =>
            {
                entity.HasIndex(e => e.TargetDefinitionId)
                    .HasDatabaseName("FK_CharacteristicProfiles_CharacteristicDefinitions");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TargetDefinitionId).HasColumnType("bigint(20)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.TargetDefinition)
                    .WithMany(p => p.CharacteristicProfiles)
                    .HasForeignKey(d => d.TargetDefinitionId)
                    .HasConstraintName("FK_CharacteristicProfiles_CharacteristicDefinitions");
            });

            modelBuilder.Entity<CharacteristicValue>(entity =>
            {
                entity.HasIndex(e => e.DefinitionId)
                    .HasDatabaseName("FK_CharacteristicValues_CharacteristicDefinitions");

                entity.HasIndex(e => e.FutureProgId)
                    .HasDatabaseName("FK_CharacteristicValues_FutureProgs");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AdditionalValue)
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Default)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.DefinitionId).HasColumnType("bigint(20)");

                entity.Property(e => e.FutureProgId).HasColumnType("bigint(20)");
                entity.Property(e => e.OngoingValidityProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Pluralisation).HasColumnType("int(11)");

                entity.Property(e => e.Value)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.HasOne(d => d.Definition)
                    .WithMany(p => p.CharacteristicValues)
                    .HasForeignKey(d => d.DefinitionId)
                    .HasConstraintName("FK_CharacteristicValues_CharacteristicDefinitions");

                entity.HasOne(d => d.FutureProg)
                    .WithMany(p => p.CharacteristicValues)
                    .HasForeignKey(d => d.FutureProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_CharacteristicValues_FutureProgs");

                entity.HasOne(d => d.OngoingValidityProg)
                   .WithMany()
                   .HasForeignKey(d => d.OngoingValidityProgId)
                   .OnDelete(DeleteBehavior.SetNull)
                   .HasConstraintName("FK_CharacteristicValues_FutureProgs_Ongoing");
            });

            modelBuilder.Entity<Characteristic>(entity =>
            {
                entity.HasKey(e => new { e.BodyId, e.Type })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.CharacteristicId)
                    .HasDatabaseName("FK_Characteristics_CharacteristicValues");

                entity.Property(e => e.BodyId).HasColumnType("bigint(20)");

                entity.Property(e => e.Type).HasColumnType("int(11)");

                entity.Property(e => e.CharacteristicId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Body)
                    .WithMany(p => p.Characteristics)
                    .HasForeignKey(d => d.BodyId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Characteristics_Bodies");

                entity.HasOne(d => d.CharacteristicValue)
                    .WithMany(p => p.Characteristics)
                    .HasForeignKey(d => d.CharacteristicId)
                    .HasConstraintName("FK_Characteristics_CharacteristicValues");
            });

            modelBuilder.Entity<Models.Character>(entity =>
            {
                entity.HasIndex(e => e.AccountId)
                    .HasDatabaseName("FK_Characters_Accounts");

                entity.HasIndex(e => e.BodyId)
                    .HasDatabaseName("FK_Characters_Bodies");

                entity.HasIndex(e => e.ChargenId)
                    .HasDatabaseName("FK_Characters_Chargens_idx");

                entity.HasIndex(e => e.CultureId)
                    .HasDatabaseName("FK_Characters_Cultures");

                entity.HasIndex(e => e.CurrencyId)
                    .HasDatabaseName("FK_Characters_Currencies");

                entity.HasIndex(e => e.CurrentAccentId)
                    .HasDatabaseName("FK_Characters_Accents_idx");

                entity.HasIndex(e => e.CurrentLanguageId)
                    .HasDatabaseName("FK_Characters_Languages_idx");

                entity.HasIndex(e => e.CurrentProjectId)
                    .HasDatabaseName("FK_Characters_ActiveProjects_idx");

                entity.HasIndex(e => e.CurrentProjectLabourId)
                    .HasDatabaseName("FK_Characters_ProjectLabourRequirements_idx");

                entity.HasIndex(e => e.CurrentScriptId)
                    .HasDatabaseName("FK_Characters_Scripts_idx");

                entity.HasIndex(e => e.CurrentWritingLanguageId)
                    .HasDatabaseName("FK_Characters_Languages_Written_idx");

                entity.HasIndex(e => e.Location)
                    .HasDatabaseName("FK_Characters_Cells");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AccountId).HasColumnType("bigint(20)");

                entity.Property(e => e.BirthdayCalendarId).HasColumnType("bigint(20)");

                entity.Property(e => e.BirthdayDate)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.BodyId).HasColumnType("bigint(20)");

                entity.Property(e => e.ChargenId).HasColumnType("bigint(20)");

                entity.Property(e => e.CombatBrief)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.CreationTime).HasColumnType("datetime");

                entity.Property(e => e.CultureId).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrentAccentId).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrentCombatSettingId).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrentLanguageId).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrentProjectId).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrentProjectLabourId).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrentScriptId).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrentWritingLanguageId).HasColumnType("bigint(20)");

                entity.Property(e => e.DeathTime).HasColumnType("datetime");

                entity.Property(e => e.DominantHandAlignment)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'3'");

                entity.Property(e => e.EffectData)
                    .IsRequired()
                    .HasColumnType("mediumtext")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Gender).HasColumnType("smallint(6)");

                entity.Property(e => e.IntroductionMessage)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.IsAdminAvatar)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.LastLoginTime).HasColumnType("datetime");

                entity.Property(e => e.LastLogoutTime).HasColumnType("datetime");

                entity.Property(e => e.Location).HasColumnType("bigint(20)");

                entity.Property(e => e.LongTermPlan)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.NameInfo)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.NeedsModel)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'NoNeeds'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.NoMercy)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Outfits)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

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

                entity.Property(e => e.PreferredDefenseType).HasColumnType("int(11)");

                entity.Property(e => e.RoomBrief)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.RoomLayer).HasColumnType("int(11)");

                entity.Property(e => e.ShortTermPlan)
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ShownIntroductionMessage)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.State).HasColumnType("int(11)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.TotalMinutesPlayed).HasColumnType("int(11)");

                entity.Property(e => e.WritingStyle)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'8256'");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Characters)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Characters_Accounts");

                entity.HasOne(d => d.Body)
                    .WithMany(p => p.Characters)
                    .HasForeignKey(d => d.BodyId)
                    .HasConstraintName("FK_Characters_Bodies");

                entity.HasOne(d => d.Chargen)
                    .WithMany(p => p.Characters)
                    .HasForeignKey(d => d.ChargenId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Characters_Chargens");

                entity.HasOne(d => d.Culture)
                    .WithMany(p => p.Characters)
                    .HasForeignKey(d => d.CultureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Characters_Cultures");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Characters)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_Characters_Currencies");

                entity.HasOne(d => d.CurrentAccent)
                    .WithMany(p => p.Characters)
                    .HasForeignKey(d => d.CurrentAccentId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Characters_Accents");

                entity.HasOne(d => d.CurrentLanguage)
                    .WithMany(p => p.CharactersCurrentLanguage)
                    .HasForeignKey(d => d.CurrentLanguageId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Characters_Languages_Spoken");

                entity.HasOne(d => d.CurrentProject)
                    .WithMany(p => p.Characters)
                    .HasForeignKey(d => d.CurrentProjectId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Characters_ActiveProjects");

                entity.HasOne(d => d.CurrentProjectLabour)
                    .WithMany(p => p.Characters)
                    .HasForeignKey(d => d.CurrentProjectLabourId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Characters_ProjectLabourRequirements");

                entity.HasOne(d => d.CurrentScript)
                    .WithMany(p => p.Characters)
                    .HasForeignKey(d => d.CurrentScriptId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Characters_Scripts");

                entity.HasOne(d => d.CurrentWritingLanguage)
                    .WithMany(p => p.CharactersCurrentWritingLanguage)
                    .HasForeignKey(d => d.CurrentWritingLanguageId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Characters_Languages_Written");

                entity.HasOne(d => d.LocationNavigation)
                    .WithMany(p => p.Characters)
                    .HasForeignKey(d => d.Location)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Characters_Cells");
            });

            modelBuilder.Entity<CharacterAccent>(entity =>
            {
                entity.HasKey(e => new { e.CharacterId, e.AccentId })
                    .HasName("PRIMARY");

                entity.ToTable("Characters_Accents");

                entity.HasIndex(e => e.AccentId)
                    .HasDatabaseName("FK_Characters_Accents_Accents_idx");

                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

                entity.Property(e => e.AccentId).HasColumnType("bigint(20)");

                entity.Property(e => e.Familiarity).HasColumnType("int(11)");

                entity.Property(e => e.IsPreferred)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.HasOne(d => d.Accent)
                    .WithMany(p => p.CharactersAccents)
                    .HasForeignKey(d => d.AccentId)
                    .HasConstraintName("FK_Characters_Accents_Accents");

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.CharactersAccents)
                    .HasForeignKey(d => d.CharacterId)
                    .HasConstraintName("FK_Characters_Accents_Characters");
            });

            modelBuilder.Entity<CharactersChargenRoles>(entity =>
            {
                entity.HasKey(e => new { e.CharacterId, e.ChargenRoleId })
                    .HasName("PRIMARY");

                entity.ToTable("Characters_ChargenRoles");

                entity.HasIndex(e => e.ChargenRoleId)
                    .HasDatabaseName("FK_Characters_ChargenRoles_ChargenRoles");

                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

                entity.Property(e => e.ChargenRoleId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.CharactersChargenRoles)
                    .HasForeignKey(d => d.CharacterId)
                    .HasConstraintName("FK_Characters_ChargenRoles_Characters");

                entity.HasOne(d => d.ChargenRole)
                    .WithMany(p => p.CharactersChargenRoles)
                    .HasForeignKey(d => d.ChargenRoleId)
                    .HasConstraintName("FK_Characters_ChargenRoles_ChargenRoles");
            });

            modelBuilder.Entity<CharactersLanguages>(entity =>
            {
                entity.HasKey(e => new { e.CharacterId, e.LanguageId })
                    .HasName("PRIMARY");

                entity.ToTable("Characters_Languages");

                entity.HasIndex(e => e.LanguageId)
                    .HasDatabaseName("FK_Characters_Languages_Languages_idx");

                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

                entity.Property(e => e.LanguageId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.CharactersLanguages)
                    .HasForeignKey(d => d.CharacterId)
                    .HasConstraintName("FK_Characters_Languages_Characters");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.CharactersLanguages)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("FK_Characters_Languages_Languages");
            });

            modelBuilder.Entity<CharactersMagicResources>(entity =>
            {
                entity.HasKey(e => new { e.CharacterId, e.MagicResourceId })
                    .HasName("PRIMARY");

                entity.ToTable("Characters_MagicResources");

                entity.HasIndex(e => e.MagicResourceId)
                    .HasDatabaseName("FK_Characters_MagicResources_MagicResources_idx");

                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

                entity.Property(e => e.MagicResourceId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.CharactersMagicResources)
                    .HasForeignKey(d => d.CharacterId)
                    .HasConstraintName("FK_Characters_MagicResources_Characters");

                entity.HasOne(d => d.MagicResource)
                    .WithMany(p => p.CharactersMagicResources)
                    .HasForeignKey(d => d.MagicResourceId)
                    .HasConstraintName("FK_Characters_MagicResources_MagicResources");
            });

            modelBuilder.Entity<CharactersScripts>(entity =>
            {
                entity.HasKey(e => new { e.CharacterId, e.ScriptId })
                    .HasName("PRIMARY");

                entity.ToTable("Characters_Scripts");

                entity.HasIndex(e => e.ScriptId)
                    .HasDatabaseName("FK_Characters_Scripts_Scripts_idx");

                entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");

                entity.Property(e => e.ScriptId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.CharactersScripts)
                    .HasForeignKey(d => d.CharacterId)
                    .HasConstraintName("FK_Characters_Scripts_Characters");

                entity.HasOne(d => d.Script)
                    .WithMany(p => p.CharactersScripts)
                    .HasForeignKey(d => d.ScriptId)
                    .HasConstraintName("FK_Characters_Scripts_Scripts");
            });

            modelBuilder.Entity<ChargenAdvice>(entity =>
            {
                entity.HasIndex(e => e.ShouldShowAdviceProgId)
                    .HasDatabaseName("FK_ChargenAdvices_FutureProgs_idx");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AdviceText)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.AdviceTitle)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ChargenStage).HasColumnType("int(11)");

                entity.Property(e => e.ShouldShowAdviceProgId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.ShouldShowAdviceProg)
                    .WithMany(p => p.ChargenAdvices)
                    .HasForeignKey(d => d.ShouldShowAdviceProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_ChargenAdvices_FutureProgs");
            });

            modelBuilder.Entity<ChargenAdvicesChargenRoles>(entity =>
            {
                entity.HasKey(e => new { e.ChargenAdviceId, e.ChargenRoleId })
                    .HasName("PRIMARY");

                entity.ToTable("ChargenAdvices_ChargenRoles");

                entity.HasIndex(e => e.ChargenRoleId)
                    .HasDatabaseName("FK_ChargenAdvices_ChargenRoles_ChargenRoles_idx");

                entity.Property(e => e.ChargenAdviceId).HasColumnType("bigint(20)");

                entity.Property(e => e.ChargenRoleId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.ChargenAdvice)
                    .WithMany(p => p.ChargenAdvicesChargenRoles)
                    .HasForeignKey(d => d.ChargenAdviceId)
                    .HasConstraintName("FK_ChargenAdvices_ChargenRoles_ChargenAdvices");

                entity.HasOne(d => d.ChargenRole)
                    .WithMany(p => p.ChargenAdvicesChargenRoles)
                    .HasForeignKey(d => d.ChargenRoleId)
                    .HasConstraintName("FK_ChargenAdvices_ChargenRoles_ChargenRoles");
            });

            modelBuilder.Entity<ChargenAdvicesCultures>(entity =>
            {
                entity.HasKey(e => new { e.ChargenAdviceId, e.CultureId })
                    .HasName("PRIMARY");

                entity.ToTable("ChargenAdvices_Cultures");

                entity.HasIndex(e => e.CultureId)
                    .HasDatabaseName("FK_ChargenAdvices_Cultures_Cultures_idx");

                entity.Property(e => e.ChargenAdviceId).HasColumnType("bigint(20)");

                entity.Property(e => e.CultureId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.ChargenAdvice)
                    .WithMany(p => p.ChargenAdvicesCultures)
                    .HasForeignKey(d => d.ChargenAdviceId)
                    .HasConstraintName("FK_ChargenAdvices_Cultures_ChargenAdvices");

                entity.HasOne(d => d.Culture)
                    .WithMany(p => p.ChargenAdvicesCultures)
                    .HasForeignKey(d => d.CultureId)
                    .HasConstraintName("FK_ChargenAdvices_Cultures_Cultures");
            });

            modelBuilder.Entity<ChargenAdvicesEthnicities>(entity =>
            {
                entity.HasKey(e => new { e.ChargenAdviceId, e.EthnicityId })
                    .HasName("PRIMARY");

                entity.ToTable("ChargenAdvices_Ethnicities");

                entity.HasIndex(e => e.EthnicityId)
                    .HasDatabaseName("FK_ChargenAdvices_Ethnicities_Ethnicities_idx");

                entity.Property(e => e.ChargenAdviceId).HasColumnType("bigint(20)");

                entity.Property(e => e.EthnicityId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.ChargenAdvice)
                    .WithMany(p => p.ChargenAdvicesEthnicities)
                    .HasForeignKey(d => d.ChargenAdviceId)
                    .HasConstraintName("FK_ChargenAdvices_Ethnicities_ChargenAdvices");

                entity.HasOne(d => d.Ethnicity)
                    .WithMany(p => p.ChargenAdvicesEthnicities)
                    .HasForeignKey(d => d.EthnicityId)
                    .HasConstraintName("FK_ChargenAdvices_Ethnicities_Ethnicities");
            });

            modelBuilder.Entity<ChargenAdvicesRaces>(entity =>
            {
                entity.HasKey(e => new { e.ChargenAdviceId, e.RaceId })
                    .HasName("PRIMARY");

                entity.ToTable("ChargenAdvices_Races");

                entity.HasIndex(e => e.RaceId)
                    .HasDatabaseName("FK_ChargenAdvices_Races_Races_idx");

                entity.Property(e => e.ChargenAdviceId).HasColumnType("bigint(20)");

                entity.Property(e => e.RaceId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.ChargenAdvice)
                    .WithMany(p => p.ChargenAdvicesRaces)
                    .HasForeignKey(d => d.ChargenAdviceId)
                    .HasConstraintName("FK_ChargenAdvices_Races_ChargenAdvices");

                entity.HasOne(d => d.Race)
                    .WithMany(p => p.ChargenAdvicesRaces)
                    .HasForeignKey(d => d.RaceId)
                    .HasConstraintName("FK_ChargenAdvices_Races_Races");
            });

            modelBuilder.Entity<ChargenResource>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Alias)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.MaximumNumberAwardedPerAward).HasColumnType("double");

                entity.Property(e => e.MaximumResourceFormula)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.MaximumResourceId).HasColumnType("bigint(20)");

                entity.Property(e => e.MinimumTimeBetweenAwards).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PermissionLevelRequiredToAward).HasColumnType("int(11)");

                entity.Property(e => e.PermissionLevelRequiredToCircumventMinimumTime).HasColumnType("int(11)");

                entity.Property(e => e.PluralName)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ShowToPlayerInScore).HasColumnType("bit(1)");

                entity.Property(e => e.TextDisplayedToPlayerOnAward)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TextDisplayedToPlayerOnDeduct)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<ChargenRole>(entity =>
            {
                entity.HasIndex(e => e.AvailabilityProgId)
                    .HasDatabaseName("FK_ChargenRoles_FutureProgs");

                entity.HasIndex(e => e.PosterId)
                    .HasDatabaseName("FK_ChargenRoles_Accounts");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AvailabilityProgId).HasColumnType("bigint(20)");

                entity.Property(e => e.ChargenBlurb)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Expired)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.MaximumNumberAlive).HasColumnType("int(11)");

                entity.Property(e => e.MaximumNumberTotal).HasColumnType("int(11)");

                entity.Property(e => e.MinimumAuthorityToApprove).HasColumnType("int(11)");

                entity.Property(e => e.MinimumAuthorityToView).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(4000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PosterId).HasColumnType("bigint(20)");

                entity.Property(e => e.Type).HasColumnType("int(11)");

                entity.HasOne(d => d.AvailabilityProg)
                    .WithMany(p => p.ChargenRoles)
                    .HasForeignKey(d => d.AvailabilityProgId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_ChargenRoles_FutureProgs");

                entity.HasOne(d => d.Poster)
                    .WithMany(p => p.ChargenRoles)
                    .HasForeignKey(d => d.PosterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChargenRoles_Accounts");
            });

            modelBuilder.Entity<ChargenRolesApprovers>(entity =>
            {
                entity.HasKey(e => new { e.ChargenRoleId, e.ApproverId })
                    .HasName("PRIMARY");

                entity.ToTable("ChargenRoles_Approvers");

                entity.HasIndex(e => e.ApproverId)
                    .HasDatabaseName("FK_ChargenRoles_Approvers_Accounts");

                entity.Property(e => e.ChargenRoleId).HasColumnType("bigint(20)");

                entity.Property(e => e.ApproverId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Approver)
                    .WithMany(p => p.ChargenRolesApprovers)
                    .HasForeignKey(d => d.ApproverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChargenRoles_Approvers_Accounts");

                entity.HasOne(d => d.ChargenRole)
                    .WithMany(p => p.ChargenRolesApprovers)
                    .HasForeignKey(d => d.ChargenRoleId)
                    .HasConstraintName("FK_ChargenRoles_Approvers_ChargenRoles");
            });

            modelBuilder.Entity<ChargenRolesClanMemberships>(entity =>
            {
                entity.HasKey(e => new { e.ChargenRoleId, e.ClanId })
                    .HasName("PRIMARY");

                entity.ToTable("ChargenRoles_ClanMemberships");

                entity.HasIndex(e => e.ClanId)
                    .HasDatabaseName("FK_ChargenRoles_ClanMemberships_Clans");

                entity.Property(e => e.ChargenRoleId).HasColumnType("bigint(20)");

                entity.Property(e => e.ClanId).HasColumnType("bigint(20)");

                entity.Property(e => e.PaygradeId).HasColumnType("bigint(20)");

                entity.Property(e => e.RankId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.ChargenRole)
                    .WithMany(p => p.ChargenRolesClanMemberships)
                    .HasForeignKey(d => d.ChargenRoleId)
                    .HasConstraintName("FK_ChargenRoles_ClanMemberships_ChargenRoles");

                entity.HasOne(d => d.Clan)
                    .WithMany(p => p.ChargenRolesClanMemberships)
                    .HasForeignKey(d => d.ClanId)
                    .HasConstraintName("FK_ChargenRoles_ClanMemberships_Clans");
            });

            modelBuilder.Entity<ChargenRolesClanMembershipsAppointments>(entity =>
            {
                entity.HasKey(e => new { e.ChargenRoleId, e.ClanId, e.AppointmentId })
                    .HasName("PRIMARY");

                entity.ToTable("ChargenRoles_ClanMemberships_Appointments");

                entity.Property(e => e.ChargenRoleId).HasColumnType("bigint(20)");

                entity.Property(e => e.ClanId).HasColumnType("bigint(20)");

                entity.Property(e => e.AppointmentId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.ChargenRolesClanMembership)
                    .WithMany(p => p.ChargenRolesClanMembershipsAppointments)
                    .HasForeignKey(d => new { d.ChargenRoleId, d.ClanId })
                    .HasConstraintName("FK_CRCMA_ChargenRoles_ClanMemberships");
            });

            modelBuilder.Entity<ChargenRolesCost>(entity =>
            {
                entity.HasKey(e => new { e.ChargenRoleId, e.ChargenResourceId, e.RequirementOnly })
                    .HasName("PRIMARY");

                entity.ToTable("ChargenRoles_Costs");

                entity.HasIndex(e => e.ChargenResourceId)
                    .HasDatabaseName("FK_ChargenRoles_Costs_ChargenResources");

                entity.Property(e => e.ChargenRoleId).HasColumnType("bigint(20)");

                entity.Property(e => e.ChargenResourceId).HasColumnType("bigint(20)");

                entity.Property(e => e.RequirementOnly)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.Amount).HasColumnType("int(11)");

                entity.HasOne(d => d.ChargenResource)
                    .WithMany(p => p.ChargenRolesCosts)
                    .HasForeignKey(d => d.ChargenResourceId)
                    .HasConstraintName("FK_ChargenRoles_Costs_ChargenResources");

                entity.HasOne(d => d.ChargenRole)
                    .WithMany(p => p.ChargenRolesCosts)
                    .HasForeignKey(d => d.ChargenRoleId)
                    .HasConstraintName("FK_ChargenRoles_Costs_ChargenRoles");
            });

            modelBuilder.Entity<ChargenRolesCurrency>(entity =>
            {
                entity.HasKey(e => new { e.ChargenRoleId, e.CurrencyId })
                    .HasName("PRIMARY");

                entity.ToTable("ChargenRoles_Currencies");

                entity.HasIndex(e => e.CurrencyId)
                    .HasDatabaseName("FK_ChargenRoles_Currencies_Currencies");

                entity.Property(e => e.ChargenRoleId).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");

                entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");

                entity.HasOne(d => d.ChargenRole)
                    .WithMany(p => p.ChargenRolesCurrencies)
                    .HasForeignKey(d => d.ChargenRoleId)
                    .HasConstraintName("FK_ChargenRoles_Currencies_ChargenRoles");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.ChargenRolesCurrencies)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_ChargenRoles_Currencies_Currencies");
            });

            modelBuilder.Entity<ChargenRolesMerit>(entity =>
            {
                entity.HasKey(e => new { e.ChargenRoleId, e.MeritId })
                    .HasName("PRIMARY");

                entity.ToTable("ChargenRoles_Merits");

                entity.HasIndex(e => e.MeritId)
                    .HasDatabaseName("FK_ChargenRoles_Merits_Merits_idx");

                entity.Property(e => e.ChargenRoleId).HasColumnType("bigint(20)");

                entity.Property(e => e.MeritId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.ChargenRole)
                    .WithMany(p => p.ChargenRolesMerits)
                    .HasForeignKey(d => d.ChargenRoleId)
                    .HasConstraintName("FK_ChargenRoles_Merits_ChargenRoles");

                entity.HasOne(d => d.Merit)
                    .WithMany(p => p.ChargenRolesMerits)
                    .HasForeignKey(d => d.MeritId)
                    .HasConstraintName("FK_ChargenRoles_Merits_Merits");
            });

            modelBuilder.Entity<ChargenRolesTrait>(entity =>
            {
                entity.HasKey(e => new { e.ChargenRoleId, e.TraitId })
                    .HasName("PRIMARY");

                entity.ToTable("ChargenRoles_Traits");

                entity.HasIndex(e => e.TraitId)
                    .HasDatabaseName("FK_ChargenRoles_Traits_Currencies");

                entity.Property(e => e.ChargenRoleId).HasColumnType("bigint(20)");

                entity.Property(e => e.TraitId).HasColumnType("bigint(20)");

                entity.Property(e => e.GiveIfDoesntHave)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.HasOne(d => d.ChargenRole)
                    .WithMany(p => p.ChargenRolesTraits)
                    .HasForeignKey(d => d.ChargenRoleId)
                    .HasConstraintName("FK_ChargenRoles_Traits_ChargenRoles");

                entity.HasOne(d => d.Trait)
                    .WithMany(p => p.ChargenRolesTraits)
                    .HasForeignKey(d => d.TraitId)
                    .HasConstraintName("FK_ChargenRoles_Traits_Currencies");
            });

            modelBuilder.Entity<Chargen>(entity =>
            {
                entity.HasIndex(e => e.AccountId)
                    .HasDatabaseName("FK_Chargens_Accounts");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AccountId).HasColumnType("bigint(20)");

                entity.Property(e => e.ApprovalTime).HasColumnType("datetime");

                entity.Property(e => e.ApprovedById).HasColumnType("bigint(20)");

                entity.Property(e => e.Definition)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.MinimumApprovalAuthority).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(12000)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.SubmitTime).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Chargens)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Chargens_Accounts");
            });
        }

    }
}
