using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	protected static void OnModelCreatingSix(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Arena>(entity =>
		{
			entity.ToTable("Arenas");

			entity.HasIndex(e => e.EconomicZoneId)
				.HasDatabaseName("FK_Arenas_EconomicZones");

			entity.HasIndex(e => e.CurrencyId)
				.HasDatabaseName("FK_Arenas_Currencies");

			entity.HasIndex(e => e.BankAccountId)
				.HasDatabaseName("FK_Arenas_BankAccounts");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.Name)
				.IsRequired()
				.HasColumnType("varchar(200)")
				.HasCharSet("utf8")
				.UseCollation("utf8_general_ci");
			entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");
			entity.Property(e => e.CurrencyId).HasColumnType("bigint(20)");
			entity.Property(e => e.BankAccountId).HasColumnType("bigint(20)");
			entity.Property(e => e.VirtualBalance).HasColumnType("decimal(58,29)");
			entity.Property(e => e.CreatedAt).HasColumnType("datetime");
			entity.Property(e => e.IsDeleted)
				.HasColumnType("bit(1)")
				.HasDefaultValueSql("b'0'");
			entity.Property(e => e.SignupEcho)
				.HasColumnType("varchar(4000)")
				.HasCharSet("utf8")
				.UseCollation("utf8_general_ci");

			entity.HasOne(d => d.EconomicZone)
				.WithMany()
				.HasForeignKey(d => d.EconomicZoneId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Arenas_EconomicZones");

			entity.HasOne(d => d.Currency)
				.WithMany()
				.HasForeignKey(d => d.CurrencyId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Arenas_Currencies");

			entity.HasOne(d => d.BankAccount)
				.WithMany()
				.HasForeignKey(d => d.BankAccountId)
				.OnDelete(DeleteBehavior.SetNull)
				.HasConstraintName("FK_Arenas_BankAccounts");
		});

		modelBuilder.Entity<ArenaManager>(entity =>
		{
			entity.ToTable("ArenaManagers");

			entity.HasIndex(e => e.ArenaId)
				.HasDatabaseName("FK_ArenaManagers_Arenas");

			entity.HasIndex(e => e.CharacterId)
				.HasDatabaseName("FK_ArenaManagers_Characters");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaId).HasColumnType("bigint(20)");
			entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.CreatedAt).HasColumnType("datetime");

			entity.HasOne(d => d.Arena)
				.WithMany(p => p.ArenaManagers)
				.HasForeignKey(d => d.ArenaId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaManagers_Arenas");

			entity.HasOne(d => d.Character)
				.WithMany()
				.HasForeignKey(d => d.CharacterId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaManagers_Characters");
		});

		modelBuilder.Entity<ArenaCell>(entity =>
		{
			entity.ToTable("ArenaCells");

			entity.HasIndex(e => e.ArenaId)
				.HasDatabaseName("FK_ArenaCells_Arenas");

			entity.HasIndex(e => e.CellId)
				.HasDatabaseName("FK_ArenaCells_Cells");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaId).HasColumnType("bigint(20)");
			entity.Property(e => e.CellId).HasColumnType("bigint(20)");
			entity.Property(e => e.Role).HasColumnType("int(11)");

			entity.HasOne(d => d.Arena)
				.WithMany(p => p.ArenaCells)
				.HasForeignKey(d => d.ArenaId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaCells_Arenas");

			entity.HasOne(d => d.Cell)
				.WithMany()
				.HasForeignKey(d => d.CellId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaCells_Cells");
		});

		modelBuilder.Entity<ArenaCombatantClass>(entity =>
		{
			entity.ToTable("ArenaCombatantClasses");

			entity.HasIndex(e => e.ArenaId)
				.HasDatabaseName("FK_ArenaCombatantClasses_Arenas");

			entity.HasIndex(e => e.EligibilityProgId)
				.HasDatabaseName("FK_ArenaCombatantClasses_EligibilityProg");

			entity.HasIndex(e => e.AdminNpcLoaderProgId)
				.HasDatabaseName("FK_ArenaCombatantClasses_AdminNpcLoaderProg");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name)
				.IsRequired()
				.HasColumnType("varchar(200)")
				.HasCharSet("utf8")
				.UseCollation("utf8_general_ci");
			entity.Property(e => e.Description)
				.IsRequired()
				.HasColumnType("varchar(4000)")
				.HasCharSet("utf8")
				.UseCollation("utf8_general_ci");
			entity.Property(e => e.EligibilityProgId).HasColumnType("bigint(20)");
			entity.Property(e => e.AdminNpcLoaderProgId).HasColumnType("bigint(20)");
			entity.Property(e => e.ResurrectNpcOnDeath)
				.HasColumnType("bit(1)")
				.HasDefaultValueSql("b'0'");
			entity.Property(e => e.DefaultStageNameTemplate)
				.HasColumnType("varchar(200)")
				.HasCharSet("utf8")
				.UseCollation("utf8_general_ci");
			entity.Property(e => e.DefaultSignatureColour)
				.HasColumnType("varchar(50)")
				.HasCharSet("utf8")
				.UseCollation("utf8_general_ci");

			entity.HasOne(d => d.Arena)
				.WithMany(p => p.ArenaCombatantClasses)
				.HasForeignKey(d => d.ArenaId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaCombatantClasses_Arenas");

			entity.HasOne(d => d.EligibilityProg)
				.WithMany()
				.HasForeignKey(d => d.EligibilityProgId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_ArenaCombatantClasses_EligibilityProg");

			entity.HasOne(d => d.AdminNpcLoaderProg)
				.WithMany()
				.HasForeignKey(d => d.AdminNpcLoaderProgId)
				.OnDelete(DeleteBehavior.SetNull)
				.HasConstraintName("FK_ArenaCombatantClasses_AdminNpcLoaderProg");
		});

		modelBuilder.Entity<ArenaEventType>(entity =>
		{
			entity.ToTable("ArenaEventTypes");

			entity.HasIndex(e => e.ArenaId)
				.HasDatabaseName("FK_ArenaEventTypes_Arenas");

			entity.HasIndex(e => e.IntroProgId)
				.HasDatabaseName("FK_ArenaEventTypes_IntroProg");

			entity.HasIndex(e => e.ScoringProgId)
				.HasDatabaseName("FK_ArenaEventTypes_ScoringProg");

			entity.HasIndex(e => e.ResolutionOverrideProgId)
				.HasDatabaseName("FK_ArenaEventTypes_ResolutionProg");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name)
				.IsRequired()
				.HasColumnType("varchar(200)")
				.HasCharSet("utf8")
				.UseCollation("utf8_general_ci");
			entity.Property(e => e.BringYourOwn)
				.HasColumnType("bit(1)")
				.HasDefaultValueSql("b'0'");
			entity.Property(e => e.RegistrationDurationSeconds).HasColumnType("int(11)");
			entity.Property(e => e.PreparationDurationSeconds).HasColumnType("int(11)");
			entity.Property(e => e.TimeLimitSeconds).HasColumnType("int(11)");
			entity.Property(e => e.AutoScheduleIntervalSeconds).HasColumnType("int(11)");
			entity.Property(e => e.AutoScheduleReferenceTime).HasColumnType("datetime");
			entity.Property(e => e.BettingModel).HasColumnType("int(11)");
			entity.Property(e => e.EliminationMode).HasColumnType("int(11)");
			entity.Property(e => e.AllowSurrender)
				.HasColumnType("bit(1)")
				.HasDefaultValueSql("b'1'");
			entity.Property(e => e.AppearanceFee).HasColumnType("decimal(58,29)");
			entity.Property(e => e.VictoryFee).HasColumnType("decimal(58,29)");
			entity.Property(e => e.IntroProgId).HasColumnType("bigint(20)");
			entity.Property(e => e.ScoringProgId).HasColumnType("bigint(20)");
			entity.Property(e => e.ResolutionOverrideProgId).HasColumnType("bigint(20)");

			entity.HasOne(d => d.Arena)
				.WithMany(p => p.ArenaEventTypes)
				.HasForeignKey(d => d.ArenaId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaEventTypes_Arenas");

			entity.HasOne(d => d.IntroProg)
				.WithMany()
				.HasForeignKey(d => d.IntroProgId)
				.OnDelete(DeleteBehavior.SetNull)
				.HasConstraintName("FK_ArenaEventTypes_IntroProg");

			entity.HasOne(d => d.ScoringProg)
				.WithMany()
				.HasForeignKey(d => d.ScoringProgId)
				.OnDelete(DeleteBehavior.SetNull)
				.HasConstraintName("FK_ArenaEventTypes_ScoringProg");

			entity.HasOne(d => d.ResolutionOverrideProg)
				.WithMany()
				.HasForeignKey(d => d.ResolutionOverrideProgId)
				.OnDelete(DeleteBehavior.SetNull)
				.HasConstraintName("FK_ArenaEventTypes_ResolutionProg");
		});

		modelBuilder.Entity<ArenaEventTypeSide>(entity =>
		{
			entity.ToTable("ArenaEventTypeSides");

			entity.HasIndex(e => e.ArenaEventTypeId)
				.HasDatabaseName("FK_ArenaEventTypeSides_EventTypes");

			entity.HasIndex(e => e.OutfitProgId)
				.HasDatabaseName("FK_ArenaEventTypeSides_OutfitProg");

			entity.HasIndex(e => e.NpcLoaderProgId)
				.HasDatabaseName("FK_ArenaEventTypeSides_NpcLoaderProg");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaEventTypeId).HasColumnType("bigint(20)");
			entity.Property(e => e.Index).HasColumnType("int(11)");
			entity.Property(e => e.Capacity).HasColumnType("int(11)");
			entity.Property(e => e.Policy).HasColumnType("int(11)");
			entity.Property(e => e.AllowNpcSignup)
				.HasColumnType("bit(1)")
				.HasDefaultValueSql("b'0'");
			entity.Property(e => e.AutoFillNpc)
				.HasColumnType("bit(1)")
				.HasDefaultValueSql("b'0'");
			entity.Property(e => e.OutfitProgId).HasColumnType("bigint(20)");
			entity.Property(e => e.NpcLoaderProgId).HasColumnType("bigint(20)");

			entity.HasOne(d => d.ArenaEventType)
				.WithMany(p => p.ArenaEventTypeSides)
				.HasForeignKey(d => d.ArenaEventTypeId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaEventTypeSides_EventTypes");

			entity.HasOne(d => d.OutfitProg)
				.WithMany()
				.HasForeignKey(d => d.OutfitProgId)
				.OnDelete(DeleteBehavior.SetNull)
				.HasConstraintName("FK_ArenaEventTypeSides_OutfitProg");

			entity.HasOne(d => d.NpcLoaderProg)
				.WithMany()
				.HasForeignKey(d => d.NpcLoaderProgId)
				.OnDelete(DeleteBehavior.SetNull)
				.HasConstraintName("FK_ArenaEventTypeSides_NpcLoaderProg");
		});

		modelBuilder.Entity<ArenaEventTypeSideAllowedClass>(entity =>
		{
			entity.ToTable("ArenaEventTypeSideAllowedClasses");

			entity.HasKey(e => new { e.ArenaEventTypeSideId, e.ArenaCombatantClassId })
				.HasName("PK_ArenaEventTypeSideAllowedClasses");

			entity.Property(e => e.ArenaEventTypeSideId).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaCombatantClassId).HasColumnType("bigint(20)");

			entity.HasOne(d => d.ArenaEventTypeSide)
				.WithMany(p => p.ArenaEventTypeSideAllowedClasses)
				.HasForeignKey(d => d.ArenaEventTypeSideId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaEventTypeSideAllowedClasses_Sides");

			entity.HasOne(d => d.ArenaCombatantClass)
				.WithMany(p => p.ArenaEventTypeSideAllowedClasses)
				.HasForeignKey(d => d.ArenaCombatantClassId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaEventTypeSideAllowedClasses_Classes");
		});

		modelBuilder.Entity<ArenaEvent>(entity =>
		{
			entity.ToTable("ArenaEvents");

			entity.HasIndex(e => e.ArenaId)
				.HasDatabaseName("FK_ArenaEvents_Arenas");

			entity.HasIndex(e => e.ArenaEventTypeId)
				.HasDatabaseName("FK_ArenaEvents_EventTypes");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaId).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaEventTypeId).HasColumnType("bigint(20)");
			entity.Property(e => e.State).HasColumnType("int(11)");
			entity.Property(e => e.BringYourOwn)
				.HasColumnType("bit(1)")
				.HasDefaultValueSql("b'0'");
			entity.Property(e => e.RegistrationDurationSeconds).HasColumnType("int(11)");
			entity.Property(e => e.PreparationDurationSeconds).HasColumnType("int(11)");
			entity.Property(e => e.TimeLimitSeconds).HasColumnType("int(11)");
			entity.Property(e => e.BettingModel).HasColumnType("int(11)");
			entity.Property(e => e.AppearanceFee).HasColumnType("decimal(58,29)");
			entity.Property(e => e.VictoryFee).HasColumnType("decimal(58,29)");
			entity.Property(e => e.CreatedAt).HasColumnType("datetime");
			entity.Property(e => e.ScheduledAt).HasColumnType("datetime");
			entity.Property(e => e.RegistrationOpensAt).HasColumnType("datetime");
			entity.Property(e => e.StartedAt).HasColumnType("datetime");
			entity.Property(e => e.ResolvedAt).HasColumnType("datetime");
			entity.Property(e => e.CompletedAt).HasColumnType("datetime");
			entity.Property(e => e.AbortedAt).HasColumnType("datetime");
			entity.Property(e => e.CancellationReason)
				.HasColumnType("varchar(4000)")
				.HasCharSet("utf8")
				.UseCollation("utf8_general_ci");

			entity.HasOne(d => d.Arena)
				.WithMany(p => p.ArenaEvents)
				.HasForeignKey(d => d.ArenaId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaEvents_Arenas");

			entity.HasOne(d => d.ArenaEventType)
				.WithMany(p => p.ArenaEvents)
				.HasForeignKey(d => d.ArenaEventTypeId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_ArenaEvents_EventTypes");
		});

		modelBuilder.Entity<ArenaEventSide>(entity =>
		{
			entity.ToTable("ArenaEventSides");

			entity.HasIndex(e => e.ArenaEventId)
				.HasDatabaseName("FK_ArenaEventSides_ArenaEvents");

			entity.HasIndex(e => e.OutfitProgId)
				.HasDatabaseName("FK_ArenaEventSides_OutfitProg");

			entity.HasIndex(e => e.NpcLoaderProgId)
				.HasDatabaseName("FK_ArenaEventSides_NpcLoaderProg");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaEventId).HasColumnType("bigint(20)");
			entity.Property(e => e.SideIndex).HasColumnType("int(11)");
			entity.Property(e => e.Capacity).HasColumnType("int(11)");
			entity.Property(e => e.Policy).HasColumnType("int(11)");
			entity.Property(e => e.AllowNpcSignup)
				.HasColumnType("bit(1)")
				.HasDefaultValueSql("b'0'");
			entity.Property(e => e.AutoFillNpc)
				.HasColumnType("bit(1)")
				.HasDefaultValueSql("b'0'");
			entity.Property(e => e.OutfitProgId).HasColumnType("bigint(20)");
			entity.Property(e => e.NpcLoaderProgId).HasColumnType("bigint(20)");

			entity.HasOne(d => d.ArenaEvent)
				.WithMany(p => p.ArenaEventSides)
				.HasForeignKey(d => d.ArenaEventId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaEventSides_ArenaEvents");

			entity.HasOne(d => d.OutfitProg)
				.WithMany()
				.HasForeignKey(d => d.OutfitProgId)
				.OnDelete(DeleteBehavior.SetNull)
				.HasConstraintName("FK_ArenaEventSides_OutfitProg");

			entity.HasOne(d => d.NpcLoaderProg)
				.WithMany()
				.HasForeignKey(d => d.NpcLoaderProgId)
				.OnDelete(DeleteBehavior.SetNull)
				.HasConstraintName("FK_ArenaEventSides_NpcLoaderProg");
		});

		modelBuilder.Entity<ArenaReservation>(entity =>
		{
			entity.ToTable("ArenaReservations");

			entity.HasIndex(e => e.ArenaEventId)
				.HasDatabaseName("FK_ArenaReservations_ArenaEvents");

			entity.HasIndex(e => e.CharacterId)
				.HasDatabaseName("FK_ArenaReservations_Characters");

			entity.HasIndex(e => e.ClanId)
				.HasDatabaseName("FK_ArenaReservations_Clans");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaEventId).HasColumnType("bigint(20)");
			entity.Property(e => e.SideIndex).HasColumnType("int(11)");
			entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.ClanId).HasColumnType("bigint(20)");
			entity.Property(e => e.ReservedAt).HasColumnType("datetime");
			entity.Property(e => e.ExpiresAt).HasColumnType("datetime");

			entity.HasOne(d => d.ArenaEvent)
				.WithMany(p => p.ArenaReservations)
				.HasForeignKey(d => d.ArenaEventId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaReservations_ArenaEvents");

			entity.HasOne(d => d.Character)
				.WithMany()
				.HasForeignKey(d => d.CharacterId)
				.OnDelete(DeleteBehavior.SetNull)
				.HasConstraintName("FK_ArenaReservations_Characters");

			entity.HasOne(d => d.Clan)
				.WithMany()
				.HasForeignKey(d => d.ClanId)
				.OnDelete(DeleteBehavior.SetNull)
				.HasConstraintName("FK_ArenaReservations_Clans");
		});

		modelBuilder.Entity<ArenaSignup>(entity =>
		{
			entity.ToTable("ArenaSignups");

			entity.HasIndex(e => e.ArenaEventId)
				.HasDatabaseName("FK_ArenaSignups_ArenaEvents");

			entity.HasIndex(e => e.CharacterId)
				.HasDatabaseName("FK_ArenaSignups_Characters");

			entity.HasIndex(e => e.CombatantClassId)
				.HasDatabaseName("FK_ArenaSignups_CombatantClasses");

			entity.HasIndex(e => e.ArenaReservationId)
				.HasDatabaseName("FK_ArenaSignups_Reservations");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaEventId).HasColumnType("bigint(20)");
			entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.CombatantClassId).HasColumnType("bigint(20)");
			entity.Property(e => e.SideIndex).HasColumnType("int(11)");
			entity.Property(e => e.IsNpc)
				.HasColumnType("bit(1)")
				.HasDefaultValueSql("b'0'");
			entity.Property(e => e.StageName)
				.HasColumnType("varchar(200)")
				.HasCharSet("utf8")
				.UseCollation("utf8_general_ci");
			entity.Property(e => e.SignatureColour)
				.HasColumnType("varchar(50)")
				.HasCharSet("utf8")
				.UseCollation("utf8_general_ci");
			entity.Property(e => e.StartingRating).HasColumnType("decimal(58,29)");
			entity.Property(e => e.SignedUpAt).HasColumnType("datetime");
			entity.Property(e => e.ArenaReservationId).HasColumnType("bigint(20)");

			entity.HasOne(d => d.ArenaEvent)
				.WithMany(p => p.ArenaSignups)
				.HasForeignKey(d => d.ArenaEventId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaSignups_ArenaEvents");

			entity.HasOne(d => d.Character)
				.WithMany()
				.HasForeignKey(d => d.CharacterId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaSignups_Characters");

			entity.HasOne(d => d.CombatantClass)
				.WithMany(p => p.ArenaSignups)
				.HasForeignKey(d => d.CombatantClassId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaSignups_CombatantClasses");

			entity.HasOne(d => d.ArenaReservation)
				.WithMany(p => p.ArenaSignups)
				.HasForeignKey(d => d.ArenaReservationId)
				.OnDelete(DeleteBehavior.SetNull)
				.HasConstraintName("FK_ArenaSignups_Reservations");
		});

		modelBuilder.Entity<ArenaElimination>(entity =>
		{
			entity.ToTable("ArenaEliminations");

			entity.HasIndex(e => e.ArenaEventId)
				.HasDatabaseName("FK_ArenaEliminations_ArenaEvents");

			entity.HasIndex(e => e.ArenaSignupId)
				.HasDatabaseName("FK_ArenaEliminations_Signups");

			entity.HasIndex(e => e.CharacterId)
				.HasDatabaseName("FK_ArenaEliminations_Characters");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaEventId).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaSignupId).HasColumnType("bigint(20)");
			entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.Reason).HasColumnType("int(11)");
			entity.Property(e => e.OccurredAt).HasColumnType("datetime");

			entity.HasOne(d => d.ArenaEvent)
				.WithMany(p => p.ArenaEliminations)
				.HasForeignKey(d => d.ArenaEventId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaEliminations_ArenaEvents");

			entity.HasOne(d => d.ArenaSignup)
				.WithMany(p => p.ArenaEliminations)
				.HasForeignKey(d => d.ArenaSignupId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaEliminations_Signups");

			entity.HasOne(d => d.Character)
				.WithMany()
				.HasForeignKey(d => d.CharacterId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaEliminations_Characters");
		});

		modelBuilder.Entity<ArenaRating>(entity =>
		{
			entity.ToTable("ArenaRatings");

			entity.HasIndex(e => e.ArenaId)
				.HasDatabaseName("FK_ArenaRatings_Arenas");

			entity.HasIndex(e => e.CharacterId)
				.HasDatabaseName("FK_ArenaRatings_Characters");

			entity.HasIndex(e => e.CombatantClassId)
				.HasDatabaseName("FK_ArenaRatings_CombatantClasses");

			entity.HasIndex(e => new { e.ArenaId, e.CharacterId, e.CombatantClassId })
				.IsUnique()
				.HasDatabaseName("UX_ArenaRatings_UniqueParticipant");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaId).HasColumnType("bigint(20)");
			entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.CombatantClassId).HasColumnType("bigint(20)");
			entity.Property(e => e.Rating).HasColumnType("decimal(58,29)");
			entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");

			entity.HasOne(d => d.Arena)
				.WithMany(p => p.ArenaRatings)
				.HasForeignKey(d => d.ArenaId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaRatings_Arenas");

			entity.HasOne(d => d.Character)
				.WithMany()
				.HasForeignKey(d => d.CharacterId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaRatings_Characters");

			entity.HasOne(d => d.CombatantClass)
				.WithMany(p => p.ArenaRatings)
				.HasForeignKey(d => d.CombatantClassId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaRatings_CombatantClasses");
		});

		modelBuilder.Entity<ArenaBet>(entity =>
		{
			entity.ToTable("ArenaBets");

			entity.HasIndex(e => e.ArenaEventId)
				.HasDatabaseName("FK_ArenaBets_ArenaEvents");

			entity.HasIndex(e => e.CharacterId)
				.HasDatabaseName("FK_ArenaBets_Characters");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaEventId).HasColumnType("bigint(20)");
			entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.SideIndex).HasColumnType("int(11)");
			entity.Property(e => e.Stake).HasColumnType("decimal(58,29)");
			entity.Property(e => e.FixedDecimalOdds).HasColumnType("decimal(58,29)");
			entity.Property(e => e.ModelSnapshot)
				.HasColumnType("text")
				.HasCharSet("utf8")
				.UseCollation("utf8_general_ci");
			entity.Property(e => e.IsCancelled)
				.HasColumnType("bit(1)")
				.HasDefaultValueSql("b'0'");
			entity.Property(e => e.PlacedAt).HasColumnType("datetime");
			entity.Property(e => e.CancelledAt).HasColumnType("datetime");

			entity.HasOne(d => d.ArenaEvent)
				.WithMany(p => p.ArenaBets)
				.HasForeignKey(d => d.ArenaEventId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaBets_ArenaEvents");

			entity.HasOne(d => d.Character)
				.WithMany()
				.HasForeignKey(d => d.CharacterId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaBets_Characters");
		});

		modelBuilder.Entity<ArenaBetPool>(entity =>
		{
			entity.ToTable("ArenaBetPools");

			entity.HasIndex(e => e.ArenaEventId)
				.HasDatabaseName("FK_ArenaBetPools_ArenaEvents");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaEventId).HasColumnType("bigint(20)");
			entity.Property(e => e.SideIndex).HasColumnType("int(11)");
			entity.Property(e => e.TotalStake).HasColumnType("decimal(58,29)");
			entity.Property(e => e.TakeRate).HasColumnType("decimal(58,29)");

			entity.HasOne(d => d.ArenaEvent)
				.WithMany(p => p.ArenaBetPools)
				.HasForeignKey(d => d.ArenaEventId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaBetPools_ArenaEvents");
		});

		modelBuilder.Entity<ArenaBetPayout>(entity =>
		{
			entity.ToTable("ArenaBetPayouts");

			entity.HasIndex(e => e.ArenaEventId)
				.HasDatabaseName("FK_ArenaBetPayouts_ArenaEvents");

			entity.HasIndex(e => e.CharacterId)
				.HasDatabaseName("FK_ArenaBetPayouts_Characters");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaEventId).HasColumnType("bigint(20)");
			entity.Property(e => e.CharacterId).HasColumnType("bigint(20)");
			entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");
			entity.Property(e => e.IsBlocked)
				.HasColumnType("bit(1)")
				.HasDefaultValueSql("b'0'");
			entity.Property(e => e.CreatedAt).HasColumnType("datetime");
			entity.Property(e => e.CollectedAt).HasColumnType("datetime");

			entity.HasOne(d => d.ArenaEvent)
				.WithMany(p => p.ArenaBetPayouts)
				.HasForeignKey(d => d.ArenaEventId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaBetPayouts_ArenaEvents");

			entity.HasOne(d => d.Character)
				.WithMany()
				.HasForeignKey(d => d.CharacterId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaBetPayouts_Characters");
		});

		modelBuilder.Entity<ArenaFinanceSnapshot>(entity =>
		{
			entity.ToTable("ArenaFinanceSnapshots");

			entity.HasIndex(e => e.ArenaId)
				.HasDatabaseName("FK_ArenaFinanceSnapshots_Arenas");

			entity.HasIndex(e => e.ArenaEventId)
				.HasDatabaseName("FK_ArenaFinanceSnapshots_ArenaEvents");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaId).HasColumnType("bigint(20)");
			entity.Property(e => e.ArenaEventId).HasColumnType("bigint(20)");
			entity.Property(e => e.Period)
				.HasColumnType("varchar(100)")
				.HasCharSet("utf8")
				.UseCollation("utf8_general_ci");
			entity.Property(e => e.Revenue).HasColumnType("decimal(58,29)");
			entity.Property(e => e.Costs).HasColumnType("decimal(58,29)");
			entity.Property(e => e.TaxWithheld).HasColumnType("decimal(58,29)");
			entity.Property(e => e.Profit).HasColumnType("decimal(58,29)");
			entity.Property(e => e.CreatedAt).HasColumnType("datetime");

			entity.HasOne(d => d.Arena)
				.WithMany(p => p.ArenaFinanceSnapshots)
				.HasForeignKey(d => d.ArenaId)
				.OnDelete(DeleteBehavior.Cascade)
				.HasConstraintName("FK_ArenaFinanceSnapshots_Arenas");

			entity.HasOne(d => d.ArenaEvent)
				.WithMany(p => p.ArenaFinanceSnapshots)
				.HasForeignKey(d => d.ArenaEventId)
				.OnDelete(DeleteBehavior.SetNull)
				.HasConstraintName("FK_ArenaFinanceSnapshots_ArenaEvents");
		});
	}
}
