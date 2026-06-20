using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MudSharp.Models;

#nullable enable

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	public virtual DbSet<EmploymentHostState> EmploymentHostStates { get; set; } = null!;
	public virtual DbSet<EmploymentContractRecord> EmploymentContracts { get; set; } = null!;
	public virtual DbSet<EmploymentJobOpeningRecord> EmploymentJobOpenings { get; set; } = null!;
	public virtual DbSet<EmploymentJobOpeningRequirement> EmploymentJobOpeningRequirements { get; set; } = null!;
	public virtual DbSet<EmploymentApplicationRecord> EmploymentApplications { get; set; } = null!;
	public virtual DbSet<EmploymentPayableRecord> EmploymentPayables { get; set; } = null!;
	public virtual DbSet<EmploymentActionPlanRecord> EmploymentActionPlans { get; set; } = null!;
	public virtual DbSet<EmploymentActionStepRecord> EmploymentActionSteps { get; set; } = null!;
	public virtual DbSet<EmploymentScheduledTaskRuleRecord> EmploymentScheduledTaskRules { get; set; } = null!;
	public virtual DbSet<EmploymentTaskConditionRecord> EmploymentTaskConditions { get; set; } = null!;
	public virtual DbSet<EmploymentConditionPredicateRecord> EmploymentConditionPredicates { get; set; } = null!;
	public virtual DbSet<EmploymentScheduledRuleTemplateRecord> EmploymentScheduledRuleTemplates { get; set; } = null!;
	public virtual DbSet<EmploymentActiveTaskRecord> EmploymentActiveTasks { get; set; } = null!;
	public virtual DbSet<EmploymentActiveTaskStepStateRecord> EmploymentActiveTaskStepStates { get; set; } = null!;
	public virtual DbSet<EmploymentManagerGoalRecord> EmploymentManagerGoals { get; set; } = null!;
	public virtual DbSet<EmploymentRegisterEntryRecord> EmploymentRegisterEntries { get; set; } = null!;
	public virtual DbSet<EmploymentLedgerEntryRecord> EmploymentLedgerEntries { get; set; } = null!;
	public virtual DbSet<Hotel> Hotels { get; set; } = null!;
	public virtual DbSet<HotelRoom> HotelRooms { get; set; } = null!;
	public virtual DbSet<HotelRoomKey> HotelRoomKeys { get; set; } = null!;
	public virtual DbSet<HotelRoomFurnishing> HotelRoomFurnishings { get; set; } = null!;
	public virtual DbSet<HotelRoomRental> HotelRoomRentals { get; set; } = null!;
	public virtual DbSet<HotelLostProperty> HotelLostProperties { get; set; } = null!;
	public virtual DbSet<HotelPatronBalance> HotelPatronBalances { get; set; } = null!;
	public virtual DbSet<HotelBannedPatron> HotelBannedPatrons { get; set; } = null!;

	private static void ConfigureEmployment(ModelBuilder modelBuilder)
	{
		ConfigureEmploymentHostState(modelBuilder);
		ConfigureEmploymentContracts(modelBuilder);
		ConfigureEmploymentJobOpenings(modelBuilder);
		ConfigureEmploymentApplications(modelBuilder);
		ConfigureEmploymentPayables(modelBuilder);
		ConfigureEmploymentActionPlans(modelBuilder);
		ConfigureEmploymentScheduledTaskRules(modelBuilder);
		ConfigureEmploymentActiveTasks(modelBuilder);
		ConfigureEmploymentManagerGoals(modelBuilder);
		ConfigureEmploymentAuditRows(modelBuilder);
		ConfigureHotels(modelBuilder);
	}

	private static void ConfigureEmploymentHostState(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EmploymentHostState>(entity =>
		{
			entity.ToTable("EmploymentHostStates");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.HostType, e.HostId })
			      .IsUnique()
			      .HasDatabaseName("IX_EmploymentHostStates_Host");
			entity.HasIndex(e => e.BoardId).HasDatabaseName("FK_EmploymentHostStates_Boards_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.HostType).RequiredString("varchar(50)");
			entity.Property(e => e.HostId).HasColumnType("bigint(20)");
			entity.Property(e => e.BoardId).HasColumnType("bigint(20)");
			entity.Property(e => e.CreatedAt).HasColumnType("datetime");
			entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");

			entity.HasOne(e => e.Board)
			      .WithMany()
			      .HasForeignKey(e => e.BoardId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_EmploymentHostStates_Boards");
		});
	}

	private static void ConfigureEmploymentContracts(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EmploymentContractRecord>(entity =>
		{
			entity.ToTable("EmploymentContracts");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.EmploymentHostStateId, e.RuntimeId })
			      .IsUnique()
			      .HasDatabaseName("IX_EmploymentContracts_Host_Runtime");
			entity.HasIndex(e => e.EmploymentHostStateId).HasDatabaseName("FK_EmploymentContracts_HostStates_idx");
			entity.HasIndex(e => e.EmployeeId).HasDatabaseName("IX_EmploymentContracts_Employee");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RuntimeId).HasColumnType("bigint(20)");
			entity.Property(e => e.EmploymentHostStateId).HasColumnType("bigint(20)");
			entity.Property(e => e.EmployeeId).HasColumnType("bigint(20)");
			entity.Property(e => e.Role).HasColumnType("int(11)");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.Authority).HasColumnType("bigint(20)");
			ConfigureCompensationColumns(entity);
			ConfigureScheduleColumns(entity);
			ConfigureDurationColumns(entity);
			ConfigurePaymentMethodColumns(entity);
			entity.Property(e => e.StartedAt).HasColumnType("datetime");
			entity.Property(e => e.EndsAt).HasColumnType("datetime");
			entity.Property(e => e.EndReason).HasColumnType("int(11)");
			entity.Property(e => e.OriginOpeningId).HasColumnType("bigint(20)");
			entity.Property(e => e.OriginApplicationId).HasColumnType("bigint(20)");

			entity.HasOne(e => e.EmploymentHostState)
			      .WithMany(e => e.Contracts)
			      .HasForeignKey(e => e.EmploymentHostStateId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentContracts_HostStates");
		});
	}

	private static void ConfigureEmploymentJobOpenings(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EmploymentJobOpeningRecord>(entity =>
		{
			entity.ToTable("EmploymentJobOpenings");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.EmploymentHostStateId, e.RuntimeId })
			      .IsUnique()
			      .HasDatabaseName("IX_EmploymentJobOpenings_Host_Runtime");
			entity.HasIndex(e => e.EmploymentHostStateId).HasDatabaseName("FK_EmploymentJobOpenings_HostStates_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RuntimeId).HasColumnType("bigint(20)");
			entity.Property(e => e.EmploymentHostStateId).HasColumnType("bigint(20)");
			entity.Property(e => e.Role).HasColumnType("int(11)");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.MaxPositions).HasColumnType("int(11)");
			entity.Property(e => e.NpcApplicationsOnly).HasColumnType("bit(1)");
			entity.Property(e => e.Authority).HasColumnType("bigint(20)");
			entity.Property(e => e.RevisionNumber).HasColumnType("int(11)").HasDefaultValue(1);
			ConfigureCompensationColumns(entity);
			ConfigureScheduleColumns(entity);
			ConfigureDurationColumns(entity);
			ConfigurePaymentMethodColumns(entity);

			entity.HasOne(e => e.EmploymentHostState)
			      .WithMany(e => e.JobOpenings)
			      .HasForeignKey(e => e.EmploymentHostStateId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentJobOpenings_HostStates");
		});

		modelBuilder.Entity<EmploymentJobOpeningRequirement>(entity =>
		{
			entity.ToTable("EmploymentJobOpeningRequirements");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.EmploymentJobOpeningId)
			      .HasDatabaseName("FK_EmploymentJobOpeningRequirements_Openings_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.EmploymentJobOpeningId).HasColumnType("bigint(20)");
			entity.Property(e => e.RequirementType).HasColumnType("int(11)");
			entity.Property(e => e.Name).RequiredString("varchar(200)");
			entity.Property(e => e.NumericValue).HasColumnType("double");
			entity.Property(e => e.Capability).HasColumnType("int(11)");

			entity.HasOne(e => e.EmploymentJobOpening)
			      .WithMany(e => e.Requirements)
			      .HasForeignKey(e => e.EmploymentJobOpeningId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentJobOpeningRequirements_Openings");
		});
	}

	private static void ConfigureEmploymentApplications(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EmploymentApplicationRecord>(entity =>
		{
			entity.ToTable("EmploymentApplications");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.EmploymentJobOpeningId, e.RuntimeId })
			      .IsUnique()
			      .HasDatabaseName("IX_EmploymentApplications_Opening_Runtime");
			entity.HasIndex(e => e.EmploymentJobOpeningId).HasDatabaseName("FK_EmploymentApplications_Openings_idx");
			entity.HasIndex(e => e.CandidateId).HasDatabaseName("IX_EmploymentApplications_Candidate");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RuntimeId).HasColumnType("bigint(20)");
			entity.Property(e => e.EmploymentJobOpeningId).HasColumnType("bigint(20)");
			entity.Property(e => e.CandidateId).HasColumnType("bigint(20)");
			entity.Property(e => e.AppliedAt).HasColumnType("datetime");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.DecisionReason).OptionalString("mediumtext");
			entity.Property(e => e.OfferedOpeningRevision).HasColumnType("int(11)").HasDefaultValue(1);
			entity.Property(e => e.CandidateProfileJson).OptionalString("mediumtext");

			entity.HasOne(e => e.EmploymentJobOpening)
			      .WithMany(e => e.Applications)
			      .HasForeignKey(e => e.EmploymentJobOpeningId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentApplications_Openings");
		});
	}

	private static void ConfigureEmploymentPayables(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EmploymentPayableRecord>(entity =>
		{
			entity.ToTable("EmploymentPayables");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.EmploymentHostStateId, e.RuntimeId })
			      .IsUnique()
			      .HasDatabaseName("IX_EmploymentPayables_Host_Runtime");
			entity.HasIndex(e => new { e.EmploymentHostStateId, e.ContractRuntimeId, e.PayPeriodStart, e.PayPeriodEnd })
			      .IsUnique()
			      .HasDatabaseName("IX_EmploymentPayables_Contract_Period");
			entity.HasIndex(e => e.EmploymentHostStateId).HasDatabaseName("FK_EmploymentPayables_HostStates_idx");
			entity.HasIndex(e => e.EmployeeId).HasDatabaseName("IX_EmploymentPayables_Employee");
			entity.HasIndex(e => e.CorrelationId).HasDatabaseName("IX_EmploymentPayables_Correlation");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RuntimeId).HasColumnType("bigint(20)");
			entity.Property(e => e.EmploymentHostStateId).HasColumnType("bigint(20)");
			entity.Property(e => e.CorrelationId).RequiredString("varchar(36)");
			entity.Property(e => e.ContractRuntimeId).HasColumnType("bigint(20)");
			entity.Property(e => e.EmployeeId).HasColumnType("bigint(20)");
			entity.Property(e => e.EmployeeName).RequiredString("varchar(200)");
			entity.Property(e => e.Role).HasColumnType("int(11)");
			entity.Property(e => e.AmountCurrencyId).HasColumnType("bigint(20)");
			entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");
			entity.Property(e => e.PayCadence).HasColumnType("int(11)");
			ConfigurePaymentMethodColumns(entity);
			entity.Property(e => e.PayPeriodStart).HasColumnType("datetime");
			entity.Property(e => e.PayPeriodEnd).HasColumnType("datetime");
			entity.Property(e => e.DueAt).HasColumnType("datetime");
			entity.Property(e => e.AccruedAt).HasColumnType("datetime");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.SettledAt).HasColumnType("datetime");
			entity.Property(e => e.ClaimedAt).HasColumnType("datetime");
			entity.Property(e => e.SettlementNote).OptionalString("mediumtext");

			entity.HasOne(e => e.EmploymentHostState)
			      .WithMany(e => e.Payables)
			      .HasForeignKey(e => e.EmploymentHostStateId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentPayables_HostStates");
		});
	}

	private static void ConfigureEmploymentActionPlans(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EmploymentActionPlanRecord>(entity =>
		{
			entity.ToTable("EmploymentActionPlans");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.EmploymentHostStateId).HasDatabaseName("FK_EmploymentActionPlans_HostStates_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.EmploymentHostStateId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).RequiredString("varchar(200)");

			entity.HasOne(e => e.EmploymentHostState)
			      .WithMany(e => e.ActionPlans)
			      .HasForeignKey(e => e.EmploymentHostStateId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentActionPlans_HostStates");
		});

		modelBuilder.Entity<EmploymentActionStepRecord>(entity =>
		{
			entity.ToTable("EmploymentActionSteps");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.EmploymentActionPlanId).HasDatabaseName("FK_EmploymentActionSteps_Plans_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.EmploymentActionPlanId).HasColumnType("bigint(20)");
			entity.Property(e => e.SortOrder).HasColumnType("int(11)");
			entity.Property(e => e.StepType).HasColumnType("int(11)");
			entity.Property(e => e.RequiredAuthority).HasColumnType("bigint(20)");
			entity.Property(e => e.RequiredCapabilities).RequiredString("varchar(500)");
			entity.Property(e => e.RequiresPaymentAuthorisation).HasColumnType("bit(1)");
			entity.Property(e => e.IsFinancialStep).HasColumnType("bit(1)");
			entity.Property(e => e.Description).OptionalString("mediumtext");
			entity.Property(e => e.AmountCurrencyId).HasColumnType("bigint(20)");
			entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");
			entity.Property(e => e.ExistingFinancialRecord).OptionalString("varchar(200)");
			entity.Property(e => e.DestinationCellId).HasColumnType("bigint(20)");
			entity.Property(e => e.ExecutionCellId).HasColumnType("bigint(20)");
			entity.Property(e => e.CommandName).OptionalString("varchar(100)");
			entity.Property(e => e.CommandArguments).OptionalString("mediumtext");
			entity.Property(e => e.AccountName).OptionalString("varchar(200)");
			entity.Property(e => e.BoardTitle).OptionalString("varchar(200)");
			entity.Property(e => e.BoardText).OptionalString("mediumtext");

			entity.HasOne(e => e.EmploymentActionPlan)
			      .WithMany(e => e.Steps)
			      .HasForeignKey(e => e.EmploymentActionPlanId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentActionSteps_Plans");
		});
	}

	private static void ConfigureEmploymentScheduledTaskRules(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EmploymentScheduledTaskRuleRecord>(entity =>
		{
			entity.ToTable("EmploymentScheduledTaskRules");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.EmploymentHostStateId)
			      .HasDatabaseName("FK_EmploymentScheduledTaskRules_HostStates_idx");
			entity.HasIndex(e => e.EmploymentActionPlanId)
			      .HasDatabaseName("FK_EmploymentScheduledTaskRules_Plans_idx");
			entity.HasIndex(e => e.PublicId)
			      .IsUnique()
			      .HasDatabaseName("IX_EmploymentScheduledTaskRules_PublicId");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.PublicId).RequiredString("varchar(36)");
			entity.Property(e => e.EmploymentHostStateId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).RequiredString("varchar(200)");
			entity.Property(e => e.IdempotencyKey).RequiredString("varchar(200)");
			entity.Property(e => e.EmploymentActionPlanId).HasColumnType("bigint(20)");
			entity.Property(e => e.ExpressionJson).HasColumnType("text");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.CooldownTicks).HasColumnType("bigint(20)");
			entity.Property(e => e.LastSpawnedAt).HasColumnType("datetime");

			entity.HasOne(e => e.EmploymentHostState)
			      .WithMany(e => e.ScheduledTaskRules)
			      .HasForeignKey(e => e.EmploymentHostStateId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentScheduledTaskRules_HostStates");

			entity.HasOne(e => e.EmploymentActionPlan)
			      .WithMany(e => e.ScheduledTaskRules)
			      .HasForeignKey(e => e.EmploymentActionPlanId)
			      .OnDelete(DeleteBehavior.Restrict)
		      .HasConstraintName("FK_EmploymentScheduledTaskRules_Plans");
		});

		ConfigureEmploymentConditionPredicates(modelBuilder);
		ConfigureEmploymentScheduledRuleTemplates(modelBuilder);
		ConfigureEmploymentTaskConditions(modelBuilder);
	}

	private static void ConfigureEmploymentConditionPredicates(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EmploymentConditionPredicateRecord>(entity =>
		{
			entity.ToTable("EmploymentConditionPredicates");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.EmploymentHostStateId)
			      .HasDatabaseName("FK_EmploymentConditionPredicates_HostStates_idx");
			entity.HasIndex(e => e.PublicId)
			      .IsUnique()
			      .HasDatabaseName("IX_EmploymentConditionPredicates_PublicId");
			entity.HasIndex(e => new { e.EmploymentHostStateId, e.Name })
			      .IsUnique()
			      .HasDatabaseName("IX_EmploymentConditionPredicates_Host_Name");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.PublicId).RequiredString("varchar(36)");
			entity.Property(e => e.EmploymentHostStateId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).RequiredString("varchar(200)");
			entity.Property(e => e.ExpressionJson).HasColumnType("text");

			entity.HasOne(e => e.EmploymentHostState)
			      .WithMany(e => e.ConditionPredicates)
			      .HasForeignKey(e => e.EmploymentHostStateId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentConditionPredicates_HostStates");
		});
	}

	private static void ConfigureEmploymentScheduledRuleTemplates(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EmploymentScheduledRuleTemplateRecord>(entity =>
		{
			entity.ToTable("EmploymentScheduledRuleTemplates");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.EmploymentHostStateId)
			      .HasDatabaseName("FK_EmploymentScheduledRuleTemplates_HostStates_idx");
			entity.HasIndex(e => e.EmploymentActionPlanId)
			      .HasDatabaseName("FK_EmploymentScheduledRuleTemplates_Plans_idx");
			entity.HasIndex(e => e.PublicId)
			      .IsUnique()
			      .HasDatabaseName("IX_EmploymentScheduledRuleTemplates_PublicId");
			entity.HasIndex(e => new { e.EmploymentHostStateId, e.Name })
			      .IsUnique()
			      .HasDatabaseName("IX_EmploymentScheduledRuleTemplates_Host_Name");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.PublicId).RequiredString("varchar(36)");
			entity.Property(e => e.EmploymentHostStateId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).RequiredString("varchar(200)");
			entity.Property(e => e.IdempotencyKeyPattern).RequiredString("varchar(200)");
			entity.Property(e => e.EmploymentActionPlanId).HasColumnType("bigint(20)");
			entity.Property(e => e.ExpressionJson).HasColumnType("text");
			entity.Property(e => e.CooldownTicks).HasColumnType("bigint(20)");

			entity.HasOne(e => e.EmploymentHostState)
			      .WithMany(e => e.ScheduledRuleTemplates)
			      .HasForeignKey(e => e.EmploymentHostStateId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentScheduledRuleTemplates_HostStates");

			entity.HasOne(e => e.EmploymentActionPlan)
			      .WithMany(e => e.ScheduledRuleTemplates)
			      .HasForeignKey(e => e.EmploymentActionPlanId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_EmploymentScheduledRuleTemplates_Plans");
		});
	}

	private static void ConfigureEmploymentTaskConditions(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EmploymentTaskConditionRecord>(entity =>
		{
			entity.ToTable("EmploymentTaskConditions");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.ScheduledTaskRuleId)
			      .HasDatabaseName("FK_EmploymentTaskConditions_ScheduledRules_idx");
			entity.HasIndex(e => e.ManagerGoalId)
			      .HasDatabaseName("FK_EmploymentTaskConditions_ManagerGoals_idx");
			entity.HasIndex(e => e.ConditionPredicateId)
			      .HasDatabaseName("FK_EmploymentTaskConditions_ConditionPredicates_idx");
			entity.HasIndex(e => e.ScheduledRuleTemplateId)
			      .HasDatabaseName("FK_EmploymentTaskConditions_ScheduledRuleTemplates_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.ScheduledTaskRuleId).HasColumnType("bigint(20)");
			entity.Property(e => e.ManagerGoalId).HasColumnType("bigint(20)");
			entity.Property(e => e.ConditionPredicateId).HasColumnType("bigint(20)");
			entity.Property(e => e.ScheduledRuleTemplateId).HasColumnType("bigint(20)");
			entity.Property(e => e.SortOrder).HasColumnType("int(11)");
			entity.Property(e => e.ConditionType).HasColumnType("int(11)");
			entity.Property(e => e.Key).OptionalString("varchar(200)");
			entity.Property(e => e.ThresholdInt).HasColumnType("int(11)");
			entity.Property(e => e.ThresholdDecimal).HasColumnType("decimal(58,29)");
			entity.Property(e => e.BoolValue).HasColumnType("bit(1)");
			entity.Property(e => e.EarliestTicks).HasColumnType("bigint(20)");
			entity.Property(e => e.LatestTicks).HasColumnType("bigint(20)");

			entity.HasOne(e => e.ScheduledTaskRule)
			      .WithMany(e => e.Conditions)
			      .HasForeignKey(e => e.ScheduledTaskRuleId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentTaskConditions_ScheduledRules");

			entity.HasOne(e => e.ManagerGoal)
			      .WithMany(e => e.Conditions)
			      .HasForeignKey(e => e.ManagerGoalId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentTaskConditions_ManagerGoals");

			entity.HasOne(e => e.ConditionPredicate)
			      .WithMany(e => e.Conditions)
			      .HasForeignKey(e => e.ConditionPredicateId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentTaskConditions_ConditionPredicates");

			entity.HasOne(e => e.ScheduledRuleTemplate)
			      .WithMany(e => e.Conditions)
			      .HasForeignKey(e => e.ScheduledRuleTemplateId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentTaskConditions_ScheduledRuleTemplates");
		});
	}

	private static void ConfigureEmploymentActiveTasks(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EmploymentActiveTaskRecord>(entity =>
		{
			entity.ToTable("EmploymentActiveTasks");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.EmploymentHostStateId).HasDatabaseName("FK_EmploymentActiveTasks_HostStates_idx");
			entity.HasIndex(e => e.EmploymentActionPlanId).HasDatabaseName("FK_EmploymentActiveTasks_Plans_idx");
			entity.HasIndex(e => e.PublicId).IsUnique().HasDatabaseName("IX_EmploymentActiveTasks_PublicId");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.PublicId).RequiredString("varchar(36)");
			entity.Property(e => e.EmploymentHostStateId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).RequiredString("varchar(200)");
			entity.Property(e => e.EmploymentActionPlanId).HasColumnType("bigint(20)");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.AssignedEmployeeId).HasColumnType("bigint(20)");
			entity.Property(e => e.BlockedReason).OptionalString("mediumtext");
			entity.Property(e => e.CorrelationId).RequiredString("varchar(36)");
			entity.Property(e => e.IdempotencyKey).RequiredString("varchar(200)");

			entity.HasOne(e => e.EmploymentHostState)
			      .WithMany(e => e.ActiveTasks)
			      .HasForeignKey(e => e.EmploymentHostStateId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentActiveTasks_HostStates");

			entity.HasOne(e => e.EmploymentActionPlan)
			      .WithMany(e => e.ActiveTasks)
			      .HasForeignKey(e => e.EmploymentActionPlanId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_EmploymentActiveTasks_Plans");
		});

		modelBuilder.Entity<EmploymentActiveTaskStepStateRecord>(entity =>
		{
			entity.ToTable("EmploymentActiveTaskStepStates");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.EmploymentActiveTaskId)
			      .HasDatabaseName("FK_EmploymentActiveTaskStepStates_Tasks_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.EmploymentActiveTaskId).HasColumnType("bigint(20)");
			entity.Property(e => e.SortOrder).HasColumnType("int(11)");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.OperationalPayload).HasColumnType("longtext");
			entity.Property(e => e.TransactionReference).HasColumnType("longtext");
			entity.Property(e => e.SelectedResources).HasColumnType("longtext");
			entity.Property(e => e.ReservationReference).HasColumnType("longtext");
			entity.Property(e => e.RouteResult).HasColumnType("longtext");
			entity.Property(e => e.CraftJobReference).HasColumnType("longtext");
			entity.Property(e => e.LoadedAssets).HasColumnType("longtext");
			entity.Property(e => e.FailureDiagnostic).HasColumnType("longtext");

			entity.HasOne(e => e.EmploymentActiveTask)
			      .WithMany(e => e.StepStates)
			      .HasForeignKey(e => e.EmploymentActiveTaskId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentActiveTaskStepStates_Tasks");
		});
	}

	private static void ConfigureEmploymentManagerGoals(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EmploymentManagerGoalRecord>(entity =>
		{
			entity.ToTable("EmploymentManagerGoals");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => new { e.EmploymentHostStateId, e.RuntimeId })
			      .IsUnique()
			      .HasDatabaseName("IX_EmploymentManagerGoals_Host_Runtime");
			entity.HasIndex(e => e.EmploymentHostStateId).HasDatabaseName("FK_EmploymentManagerGoals_HostStates_idx");
			entity.HasIndex(e => e.EmploymentActionPlanId).HasDatabaseName("FK_EmploymentManagerGoals_Plans_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.RuntimeId).HasColumnType("bigint(20)");
			entity.Property(e => e.EmploymentHostStateId).HasColumnType("bigint(20)");
			entity.Property(e => e.GoalType).HasColumnType("int(11)");
			entity.Property(e => e.RequiredAuthority).HasColumnType("bigint(20)");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.ConfigurationDescription).RequiredString("mediumtext");
			entity.Property(e => e.EmploymentActionPlanId).HasColumnType("bigint(20)");
			entity.Property(e => e.Priority).HasColumnType("int(11)");
			entity.Property(e => e.EvaluationCadenceTicks).HasColumnType("bigint(20)");
			entity.Property(e => e.LastEvaluatedAt).HasColumnType("datetime");
			entity.Property(e => e.LastEvaluationResult).OptionalString("mediumtext");
			entity.Property(e => e.CorrelationId).RequiredString("varchar(36)");

			entity.HasOne(e => e.EmploymentHostState)
			      .WithMany(e => e.ManagerGoals)
			      .HasForeignKey(e => e.EmploymentHostStateId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentManagerGoals_HostStates");

			entity.HasOne(e => e.EmploymentActionPlan)
			      .WithMany(e => e.ManagerGoals)
			      .HasForeignKey(e => e.EmploymentActionPlanId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_EmploymentManagerGoals_Plans");
		});
	}

	private static void ConfigureEmploymentAuditRows(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EmploymentRegisterEntryRecord>(entity =>
		{
			entity.ToTable("EmploymentRegisterEntries");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.EmploymentHostStateId).HasDatabaseName("FK_EmploymentRegisterEntries_HostStates_idx");
			entity.HasIndex(e => e.CorrelationId).HasDatabaseName("IX_EmploymentRegisterEntries_Correlation");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.EmploymentHostStateId).HasColumnType("bigint(20)");
			entity.Property(e => e.CorrelationId).RequiredString("varchar(36)");
			entity.Property(e => e.EntryType).HasColumnType("int(11)");
			entity.Property(e => e.ActorId).HasColumnType("bigint(20)");
			entity.Property(e => e.Description).RequiredString("mediumtext");
			entity.Property(e => e.RecordedAt).HasColumnType("datetime");

			entity.HasOne(e => e.EmploymentHostState)
			      .WithMany(e => e.RegisterEntries)
			      .HasForeignKey(e => e.EmploymentHostStateId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentRegisterEntries_HostStates");
		});

		modelBuilder.Entity<EmploymentLedgerEntryRecord>(entity =>
		{
			entity.ToTable("EmploymentLedgerEntries");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.EmploymentHostStateId).HasDatabaseName("FK_EmploymentLedgerEntries_HostStates_idx");
			entity.HasIndex(e => e.CorrelationId).HasDatabaseName("IX_EmploymentLedgerEntries_Correlation");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.EmploymentHostStateId).HasColumnType("bigint(20)");
			entity.Property(e => e.CorrelationId).RequiredString("varchar(36)");
			entity.Property(e => e.EntryType).HasColumnType("int(11)");
			entity.Property(e => e.ActorId).HasColumnType("bigint(20)");
			entity.Property(e => e.AmountCurrencyId).HasColumnType("bigint(20)");
			entity.Property(e => e.Amount).HasColumnType("decimal(58,29)");
			entity.Property(e => e.Description).RequiredString("mediumtext");
			entity.Property(e => e.RecordedAt).HasColumnType("datetime");

			entity.HasOne(e => e.EmploymentHostState)
			      .WithMany(e => e.LedgerEntries)
			      .HasForeignKey(e => e.EmploymentHostStateId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_EmploymentLedgerEntries_HostStates");
		});
	}

	private static void ConfigureHotels(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Hotel>(entity =>
		{
			entity.ToTable("Hotels");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.PropertyId).IsUnique().HasDatabaseName("IX_Hotels_PropertyId");
			entity.HasIndex(e => e.BankAccountId).HasDatabaseName("FK_Hotels_BankAccounts_idx");
			entity.HasIndex(e => e.CanRentProgId).HasDatabaseName("FK_Hotels_FutureProgs_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.PropertyId).HasColumnType("bigint(20)");
			entity.Property(e => e.BankAccountId).HasColumnType("bigint(20)");
			entity.Property(e => e.LicenseStatus).HasColumnType("int(11)");
			entity.Property(e => e.CanRentProgId).HasColumnType("bigint(20)");
			entity.Property(e => e.LostPropertyRetention).RequiredString("varchar(200)");
			entity.Property(e => e.OutstandingTaxes).HasColumnType("decimal(58,29)");
			entity.Property(e => e.CreatedAt).HasColumnType("datetime");
			entity.Property(e => e.LastUpdatedAt).HasColumnType("datetime");

			entity.HasOne(e => e.Property)
			      .WithOne(e => e.Hotel)
			      .HasForeignKey<Hotel>(e => e.PropertyId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_Hotels_Properties");

			entity.HasOne(e => e.BankAccount)
			      .WithMany()
			      .HasForeignKey(e => e.BankAccountId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_Hotels_BankAccounts");

			entity.HasOne(e => e.CanRentProg)
			      .WithMany()
			      .HasForeignKey(e => e.CanRentProgId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_Hotels_FutureProgs");
		});

		modelBuilder.Entity<HotelRoom>(entity =>
		{
			entity.ToTable("HotelRooms");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.HotelId).HasDatabaseName("FK_HotelRooms_Hotels_idx");
			entity.HasIndex(e => e.CellId).HasDatabaseName("FK_HotelRooms_Cells_idx");
			entity.HasIndex(e => new { e.HotelId, e.CellId })
			      .IsUnique()
			      .HasDatabaseName("IX_HotelRooms_Hotel_Cell");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.HotelId).HasColumnType("bigint(20)");
			entity.Property(e => e.CellId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name).RequiredString("varchar(200)");
			entity.Property(e => e.Listed).HasColumnType("bit(1)");
			entity.Property(e => e.PricePerDay).HasColumnType("decimal(58,29)");
			entity.Property(e => e.SecurityDeposit).HasColumnType("decimal(58,29)");
			entity.Property(e => e.MinimumDurationTicks).HasColumnType("bigint(20)");
			entity.Property(e => e.MaximumDurationTicks).HasColumnType("bigint(20)");

			entity.HasOne(e => e.Hotel)
			      .WithMany(e => e.Rooms)
			      .HasForeignKey(e => e.HotelId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_HotelRooms_Hotels");

			entity.HasOne(e => e.Cell)
			      .WithMany()
			      .HasForeignKey(e => e.CellId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_HotelRooms_Cells");
		});

		modelBuilder.Entity<HotelRoomKey>(entity =>
		{
			entity.ToTable("HotelRoomKeys");
			entity.HasKey(e => new { e.HotelRoomId, e.PropertyKeyId }).HasName("PRIMARY");
			entity.HasIndex(e => e.PropertyKeyId).HasDatabaseName("FK_HotelRoomKeys_PropertyKeys_idx");

			entity.Property(e => e.HotelRoomId).HasColumnType("bigint(20)");
			entity.Property(e => e.PropertyKeyId).HasColumnType("bigint(20)");

			entity.HasOne(e => e.HotelRoom)
			      .WithMany(e => e.Keys)
			      .HasForeignKey(e => e.HotelRoomId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_HotelRoomKeys_HotelRooms");

			entity.HasOne(e => e.PropertyKey)
			      .WithMany()
			      .HasForeignKey(e => e.PropertyKeyId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_HotelRoomKeys_PropertyKeys");
		});

		modelBuilder.Entity<HotelRoomFurnishing>(entity =>
		{
			entity.ToTable("HotelRoomFurnishings");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.HotelRoomId).HasDatabaseName("FK_HotelRoomFurnishings_HotelRooms_idx");
			entity.HasIndex(e => e.GameItemId).HasDatabaseName("IX_HotelRoomFurnishings_GameItem");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.HotelRoomId).HasColumnType("bigint(20)");
			entity.Property(e => e.GameItemId).HasColumnType("bigint(20)");
			entity.Property(e => e.Description).RequiredString("varchar(500)");
			entity.Property(e => e.ReplacementValue).HasColumnType("decimal(58,29)");
			entity.Property(e => e.OriginalCondition).HasColumnType("double");
			entity.Property(e => e.OriginalDamageCondition).HasColumnType("double");

			entity.HasOne(e => e.HotelRoom)
			      .WithMany(e => e.Furnishings)
			      .HasForeignKey(e => e.HotelRoomId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_HotelRoomFurnishings_HotelRooms");
		});

		modelBuilder.Entity<HotelRoomRental>(entity =>
		{
			entity.ToTable("HotelRoomRentals");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.HotelRoomId).IsUnique().HasDatabaseName("IX_HotelRoomRentals_Room");
			entity.HasIndex(e => e.GuestId).HasDatabaseName("IX_HotelRoomRentals_Guest");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.HotelRoomId).HasColumnType("bigint(20)");
			entity.Property(e => e.GuestId).HasColumnType("bigint(20)");
			entity.Property(e => e.StartTime).RequiredString("varchar(100)");
			entity.Property(e => e.EndTime).RequiredString("varchar(100)");
			entity.Property(e => e.RentalCharge).HasColumnType("decimal(58,29)");
			entity.Property(e => e.SecurityDeposit).HasColumnType("decimal(58,29)");
			entity.Property(e => e.TaxCharged).HasColumnType("decimal(58,29)");

			entity.HasOne(e => e.HotelRoom)
			      .WithOne(e => e.ActiveRental)
			      .HasForeignKey<HotelRoomRental>(e => e.HotelRoomId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_HotelRoomRentals_HotelRooms");
		});

		modelBuilder.Entity<HotelLostProperty>(entity =>
		{
			entity.ToTable("HotelLostProperties");
			entity.HasKey(e => e.Id).HasName("PRIMARY");
			entity.HasIndex(e => e.HotelId).HasDatabaseName("FK_HotelLostProperties_Hotels_idx");
			entity.HasIndex(e => e.HotelRoomId).HasDatabaseName("FK_HotelLostProperties_HotelRooms_idx");
			entity.HasIndex(e => e.OwnerId).HasDatabaseName("IX_HotelLostProperties_Owner");
			entity.HasIndex(e => e.BundleId).HasDatabaseName("IX_HotelLostProperties_Bundle");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.HotelId).HasColumnType("bigint(20)");
			entity.Property(e => e.HotelRoomId).HasColumnType("bigint(20)");
			entity.Property(e => e.OwnerId).HasColumnType("bigint(20)");
			entity.Property(e => e.BundleId).HasColumnType("bigint(20)");
			entity.Property(e => e.StoredUntil).RequiredString("varchar(100)");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.AuctionHouseId).HasColumnType("bigint(20)");
			entity.Property(e => e.ReservePrice).HasColumnType("decimal(58,29)");
			entity.Property(e => e.Description).RequiredString("varchar(500)");

			entity.HasOne(e => e.Hotel)
			      .WithMany(e => e.LostProperties)
			      .HasForeignKey(e => e.HotelId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_HotelLostProperties_Hotels");

			entity.HasOne(e => e.HotelRoom)
			      .WithMany()
			      .HasForeignKey(e => e.HotelRoomId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_HotelLostProperties_HotelRooms");
		});

		modelBuilder.Entity<HotelPatronBalance>(entity =>
		{
			entity.ToTable("HotelPatronBalances");
			entity.HasKey(e => new { e.HotelId, e.PatronId }).HasName("PRIMARY");
			entity.HasIndex(e => e.PatronId).HasDatabaseName("IX_HotelPatronBalances_Patron");

			entity.Property(e => e.HotelId).HasColumnType("bigint(20)");
			entity.Property(e => e.PatronId).HasColumnType("bigint(20)");
			entity.Property(e => e.Balance).HasColumnType("decimal(58,29)");

			entity.HasOne(e => e.Hotel)
			      .WithMany(e => e.PatronBalances)
			      .HasForeignKey(e => e.HotelId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_HotelPatronBalances_Hotels");
		});

		modelBuilder.Entity<HotelBannedPatron>(entity =>
		{
			entity.ToTable("HotelBannedPatrons");
			entity.HasKey(e => new { e.HotelId, e.PatronId }).HasName("PRIMARY");
			entity.HasIndex(e => e.PatronId).HasDatabaseName("IX_HotelBannedPatrons_Patron");

			entity.Property(e => e.HotelId).HasColumnType("bigint(20)");
			entity.Property(e => e.PatronId).HasColumnType("bigint(20)");

			entity.HasOne(e => e.Hotel)
			      .WithMany(e => e.BannedPatrons)
			      .HasForeignKey(e => e.HotelId)
			      .OnDelete(DeleteBehavior.Cascade)
			      .HasConstraintName("FK_HotelBannedPatrons_Hotels");
		});
	}

	private static void ConfigureCompensationColumns<T>(EntityTypeBuilder<T> entity) where T : class
	{
		entity.Property<long?>("FixedRateCurrencyId").HasColumnType("bigint(20)");
		entity.Property<decimal?>("FixedRateAmount").HasColumnType("decimal(58,29)");
		entity.Property<int>("MarketBindingType").HasColumnType("int(11)");
		entity.Property<decimal?>("MarketBindingValue").HasColumnType("decimal(58,29)");
		entity.Property<int>("PayCadence").HasColumnType("int(11)");
		entity.Property<long?>("MinimumEffectivePayCurrencyId").HasColumnType("bigint(20)");
		entity.Property<decimal?>("MinimumEffectivePayAmount").HasColumnType("decimal(58,29)");
		entity.Property<int>("EmployerPaymentSource").HasColumnType("int(11)");
	}

	private static void ConfigureScheduleColumns<T>(EntityTypeBuilder<T> entity) where T : class
	{
		entity.Property<string>("ScheduleDescription").RequiredString("varchar(500)");
		entity.Property<long?>("ScheduleStartTicks").HasColumnType("bigint(20)");
		entity.Property<long?>("ScheduleEndTicks").HasColumnType("bigint(20)");
	}

	private static void ConfigureDurationColumns<T>(EntityTypeBuilder<T> entity) where T : class
	{
		entity.Property<int>("DurationType").HasColumnType("int(11)");
		entity.Property<long?>("DurationTicks").HasColumnType("bigint(20)");
	}

	private static void ConfigurePaymentMethodColumns<T>(EntityTypeBuilder<T> entity) where T : class
	{
		entity.Property<int>("PaymentMethodKind").HasColumnType("int(11)");
		entity.Property<long?>("PaymentBankAccountId").HasColumnType("bigint(20)");
		entity.Property<long?>("PaymentItemId").HasColumnType("bigint(20)");
		entity.Property<string?>("PaymentItemType").OptionalString("varchar(100)");
		entity.Property<string?>("PaymentNotes").OptionalString("mediumtext");
	}
}

internal static class EmploymentModelBuilderExtensions
{
	public static PropertyBuilder<string> RequiredString(this PropertyBuilder<string> builder, string columnType)
	{
		return builder
		       .IsRequired()
		       .HasColumnType(columnType)
		       .HasCharSet("utf8")
		       .UseCollation("utf8_general_ci");
	}

	public static PropertyBuilder<string?> OptionalString(this PropertyBuilder<string?> builder, string columnType)
	{
		return builder
		       .HasColumnType(columnType)
		       .HasCharSet("utf8")
		       .UseCollation("utf8_general_ci");
	}
}
