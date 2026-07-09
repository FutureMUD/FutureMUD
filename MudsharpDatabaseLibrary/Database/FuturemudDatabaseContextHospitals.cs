using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

#nullable enable

namespace MudSharp.Database;

public partial class FuturemudDatabaseContext
{
	private static void ConfigureHospitals(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Hospital>(entity =>
		{
			entity.ToTable("Hospitals");
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.HasIndex(e => e.EconomicZoneId).HasDatabaseName("FK_Hospitals_EconomicZones_idx");
			entity.HasIndex(e => e.BankAccountId).HasDatabaseName("FK_Hospitals_BankAccounts_idx");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.Name)
			      .IsRequired()
			      .HasColumnType("varchar(200)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.EconomicZoneId).HasColumnType("bigint(20)");
			entity.Property(e => e.BankAccountId).HasColumnType("bigint(20)");
			entity.Property(e => e.IsTrading).HasColumnType("bit(1)").HasDefaultValue(true);
			entity.Property(e => e.DefaultMaximumDebt).HasColumnType("decimal(58,29)");

			entity.HasOne(d => d.EconomicZone)
			      .WithMany()
			      .HasForeignKey(d => d.EconomicZoneId)
			      .HasConstraintName("FK_Hospitals_EconomicZones");

			entity.HasOne(d => d.BankAccount)
			      .WithMany()
			      .HasForeignKey(d => d.BankAccountId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_Hospitals_BankAccounts");
		});

		modelBuilder.Entity<HospitalLocation>(entity =>
		{
			entity.ToTable("HospitalLocations");
			entity.HasKey(e => new { e.HospitalId, e.CellId, e.Role }).HasName("PRIMARY");

			entity.HasIndex(e => e.CellId).HasDatabaseName("FK_HospitalLocations_Cells_idx");
			entity.HasIndex(e => new { e.HospitalId, e.Role }).HasDatabaseName("IX_HospitalLocations_Hospital_Role");

			entity.Property(e => e.HospitalId).HasColumnType("bigint(20)");
			entity.Property(e => e.CellId).HasColumnType("bigint(20)");
			entity.Property(e => e.Role).HasColumnType("int(11)");

			entity.HasOne(d => d.Hospital)
			      .WithMany(p => p.Locations)
			      .HasForeignKey(d => d.HospitalId)
			      .HasConstraintName("FK_HospitalLocations_Hospitals");

			entity.HasOne(d => d.Cell)
			      .WithMany()
			      .HasForeignKey(d => d.CellId)
			      .HasConstraintName("FK_HospitalLocations_Cells");
		});

		modelBuilder.Entity<HospitalBloodStockPolicy>(entity =>
		{
			entity.ToTable("HospitalBloodStockPolicies");
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.HasIndex(e => e.HospitalId).HasDatabaseName("FK_HospitalBloodStockPolicies_Hospitals_idx");
			entity.HasIndex(e => e.BloodtypeId).HasDatabaseName("FK_HospitalBloodStockPolicies_Bloodtypes_idx");
			entity.HasIndex(e => new { e.HospitalId, e.BloodtypeId })
			      .HasDatabaseName("IX_HospitalBloodStockPolicies_Hospital_Bloodtype")
			      .IsUnique();

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.HospitalId).HasColumnType("bigint(20)");
			entity.Property(e => e.BloodtypeId).HasColumnType("bigint(20)");
			entity.Property(e => e.TargetLitres).HasColumnType("double");
			entity.Property(e => e.PricePerLitre).HasColumnType("decimal(58,29)");

			entity.HasOne(d => d.Hospital)
			      .WithMany(p => p.BloodStockPolicies)
			      .HasForeignKey(d => d.HospitalId)
			      .HasConstraintName("FK_HospitalBloodStockPolicies_Hospitals");

			entity.HasOne(d => d.Bloodtype)
			      .WithMany()
			      .HasForeignKey(d => d.BloodtypeId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_HospitalBloodStockPolicies_Bloodtypes");
		});
		modelBuilder.Entity<HospitalService>(entity =>
		{
			entity.ToTable("HospitalServices");
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.HasIndex(e => e.HospitalId).HasDatabaseName("FK_HospitalServices_Hospitals_idx");
			entity.HasIndex(e => e.SurgicalProcedureId).HasDatabaseName("FK_HospitalServices_SurgicalProcedures_idx");
			entity.HasIndex(e => e.ImplantPowerProcedureId).HasDatabaseName("FK_HospitalServices_ImplantPowerProcedure_idx");
			entity.HasIndex(e => e.ImplantInterfaceProcedureId).HasDatabaseName("FK_HospitalServices_ImplantInterfaceProcedure_idx");
			entity.HasIndex(e => e.AnesthesiaCannulationProcedureId).HasDatabaseName("FK_HospitalServices_AnesthesiaCannulationProcedure_idx");
			entity.HasIndex(e => e.AnesthesiaDrugId).HasDatabaseName("FK_HospitalServices_Drugs_Anesthesia_idx");
			entity.HasIndex(e => new { e.ImplantItemPrototypeId, e.ImplantItemPrototypeRevisionNumber }).HasDatabaseName("FK_HospitalServices_GameItemProtos_idx");
			entity.HasIndex(e => new { e.HospitalId, e.Name }).HasDatabaseName("IX_HospitalServices_Hospital_Name");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.HospitalId).HasColumnType("bigint(20)");
			entity.Property(e => e.Name)
			      .IsRequired()
			      .HasColumnType("varchar(200)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.Keywords)
			      .IsRequired()
			      .HasColumnType("varchar(500)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.Description)
			      .IsRequired()
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.ServiceType).HasColumnType("int(11)");
			entity.Property(e => e.Price).HasColumnType("decimal(58,29)");
			entity.Property(e => e.IsActive).HasColumnType("bit(1)").HasDefaultValue(true);
			entity.Property(e => e.AllowDebt).HasColumnType("bit(1)").HasDefaultValue(true);
			entity.Property(e => e.PreferOperatingTheatre).HasColumnType("bit(1)").HasDefaultValue(false);
			entity.Property(e => e.OfferingMode).HasColumnType("int(11)").HasDefaultValue(0);
			entity.Property(e => e.SortOrder).HasColumnType("int(11)");
			entity.Property(e => e.SurgicalProcedureId).HasColumnType("bigint(20)");
			entity.Property(e => e.ImplantItemPrototypeId).HasColumnType("bigint(20)");
			entity.Property(e => e.ImplantItemPrototypeRevisionNumber).HasColumnType("int(11)");
			entity.Property(e => e.ImplantPowerProcedureId).HasColumnType("bigint(20)");
			entity.Property(e => e.ImplantInterfaceProcedureId).HasColumnType("bigint(20)");
			entity.Property(e => e.AnesthesiaCannulationProcedureId).HasColumnType("bigint(20)");
			entity.Property(e => e.AnesthesiaDrugId).HasColumnType("bigint(20)");
			entity.Property(e => e.ProcedureParameters)
			      .IsRequired()
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.RequiredEquipmentJson)
			      .IsRequired(false)
			      .HasColumnType("mediumtext")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.BloodVolumeLitres).HasColumnType("double").HasDefaultValue(0.5);
			entity.Property(e => e.RequiresRecovery).HasColumnType("bit(1)").HasDefaultValue(false);
			entity.Property(e => e.AnesthesiaIntensity).HasColumnType("double").HasDefaultValue(1.25);

			entity.HasOne(d => d.Hospital)
			      .WithMany(p => p.Services)
			      .HasForeignKey(d => d.HospitalId)
			      .HasConstraintName("FK_HospitalServices_Hospitals");

			entity.HasOne(d => d.SurgicalProcedure)
			      .WithMany()
			      .HasForeignKey(d => d.SurgicalProcedureId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_HospitalServices_SurgicalProcedures");

			entity.HasOne(d => d.ImplantItemPrototype)
			      .WithMany()
			      .HasForeignKey(d => new { d.ImplantItemPrototypeId, d.ImplantItemPrototypeRevisionNumber })
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_HospitalServices_GameItemProtos");

			entity.HasOne(d => d.ImplantPowerProcedure)
			      .WithMany()
			      .HasForeignKey(d => d.ImplantPowerProcedureId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_HospitalServices_ImplantPowerProcedure");

			entity.HasOne(d => d.ImplantInterfaceProcedure)
			      .WithMany()
			      .HasForeignKey(d => d.ImplantInterfaceProcedureId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_HospitalServices_ImplantInterfaceProcedure");

			entity.HasOne(d => d.AnesthesiaCannulationProcedure)
			      .WithMany()
			      .HasForeignKey(d => d.AnesthesiaCannulationProcedureId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_HospitalServices_AnesthesiaCannulationProcedure");

			entity.HasOne(d => d.AnesthesiaDrug)
			      .WithMany()
			      .HasForeignKey(d => d.AnesthesiaDrugId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_HospitalServices_Drugs_Anesthesia");
		});

		modelBuilder.Entity<HospitalServiceRequest>(entity =>
		{
			entity.ToTable("HospitalServiceRequests");
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.HasIndex(e => e.HospitalId).HasDatabaseName("FK_HospitalServiceRequests_Hospitals_idx");
			entity.HasIndex(e => e.HospitalServiceId).HasDatabaseName("FK_HospitalServiceRequests_HospitalServices_idx");
			entity.HasIndex(e => e.RequesterId).HasDatabaseName("FK_HospitalServiceRequests_Characters_Requester_idx");
			entity.HasIndex(e => e.PatientId).HasDatabaseName("FK_HospitalServiceRequests_Characters_Patient_idx");
			entity.HasIndex(e => e.AssignedEmployeeId).HasDatabaseName("FK_HospitalServiceRequests_Characters_Employee_idx");
			entity.HasIndex(e => e.PreparedByEmployeeId).HasDatabaseName("FK_HospitalServiceRequests_Characters_PreparedBy_idx");
			entity.HasIndex(e => e.OperatingTheatreCellId).HasDatabaseName("FK_HospitalServiceRequests_Cells_Theatre_idx");
			entity.HasIndex(e => e.RecoveryRoomCellId).HasDatabaseName("FK_HospitalServiceRequests_Cells_Recovery_idx");
			entity.HasIndex(e => e.ReturnCellId).HasDatabaseName("FK_HospitalServiceRequests_Cells_Return_idx");
			entity.HasIndex(e => e.EmploymentTaskId).HasDatabaseName("IX_HospitalServiceRequests_EmploymentTaskId");
			entity.HasIndex(e => new { e.HospitalId, e.Status }).HasDatabaseName("IX_HospitalServiceRequests_Hospital_Status");

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.HospitalId).HasColumnType("bigint(20)");
			entity.Property(e => e.HospitalServiceId).HasColumnType("bigint(20)");
			entity.Property(e => e.RequesterId).HasColumnType("bigint(20)");
			entity.Property(e => e.RequesterName).RequiredString("mediumtext");
			entity.Property(e => e.PatientId).HasColumnType("bigint(20)");
			entity.Property(e => e.PatientName).RequiredString("mediumtext");
			entity.Property(e => e.Status).HasColumnType("int(11)");
			entity.Property(e => e.PaymentMethod).HasColumnType("int(11)");
			entity.Property(e => e.Price).HasColumnType("decimal(58,29)");
			entity.Property(e => e.AmountPaid).HasColumnType("decimal(58,29)");
			entity.Property(e => e.DebtCharged).HasColumnType("decimal(58,29)");
			entity.Property(e => e.EmploymentTaskId)
			      .HasColumnType("varchar(36)")
			      .HasCharSet("utf8")
			      .UseCollation("utf8_general_ci");
			entity.Property(e => e.AssignedEmployeeId).HasColumnType("bigint(20)");
			entity.Property(e => e.OperatingTheatreCellId).HasColumnType("bigint(20)");
			entity.Property(e => e.UsedInPlaceFallback).HasColumnType("bit(1)");
			entity.Property(e => e.SupplyPrepared).HasColumnType("bit(1)");
			entity.Property(e => e.PreparedByEmployeeId).HasColumnType("bigint(20)");
			entity.Property(e => e.PreparedAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.RecoveryRoomCellId).HasColumnType("bigint(20)");
			entity.Property(e => e.ReturnCellId).HasColumnType("bigint(20)");
			entity.Property(e => e.CreatedAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.LastUpdatedAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.CompletedAtUtc).HasColumnType("datetime(6)");
			entity.Property(e => e.OperationalNotes).RequiredString("mediumtext");
			entity.Property(e => e.ProcedureParameters).RequiredString("mediumtext");

			entity.HasOne(d => d.Hospital)
			      .WithMany(p => p.ServiceRequests)
			      .HasForeignKey(d => d.HospitalId)
			      .HasConstraintName("FK_HospitalServiceRequests_Hospitals");

			entity.HasOne(d => d.HospitalService)
			      .WithMany()
			      .HasForeignKey(d => d.HospitalServiceId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_HospitalServiceRequests_HospitalServices");

			entity.HasOne(d => d.Requester)
			      .WithMany()
			      .HasForeignKey(d => d.RequesterId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_HospitalServiceRequests_Characters_Requester");

			entity.HasOne(d => d.Patient)
			      .WithMany()
			      .HasForeignKey(d => d.PatientId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_HospitalServiceRequests_Characters_Patient");

			entity.HasOne(d => d.AssignedEmployee)
			      .WithMany()
			      .HasForeignKey(d => d.AssignedEmployeeId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_HospitalServiceRequests_Characters_Employee");

			entity.HasOne(d => d.PreparedByEmployee)
			      .WithMany()
			      .HasForeignKey(d => d.PreparedByEmployeeId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_HospitalServiceRequests_Characters_PreparedBy");

			entity.HasOne(d => d.OperatingTheatreCell)
			      .WithMany()
			      .HasForeignKey(d => d.OperatingTheatreCellId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_HospitalServiceRequests_Cells_Theatre");

			entity.HasOne(d => d.RecoveryRoomCell)
			      .WithMany()
			      .HasForeignKey(d => d.RecoveryRoomCellId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_HospitalServiceRequests_Cells_Recovery");

			entity.HasOne(d => d.ReturnCell)
			      .WithMany()
			      .HasForeignKey(d => d.ReturnCellId)
			      .OnDelete(DeleteBehavior.SetNull)
			      .HasConstraintName("FK_HospitalServiceRequests_Cells_Return");
		});

		modelBuilder.Entity<HospitalPatientDebtAccount>(entity =>
		{
			entity.ToTable("HospitalPatientDebtAccounts");
			entity.HasKey(e => e.Id).HasName("PRIMARY");

			entity.HasIndex(e => e.HospitalId).HasDatabaseName("FK_HospitalPatientDebtAccounts_Hospitals_idx");
			entity.HasIndex(e => e.PatientId).HasDatabaseName("FK_HospitalPatientDebtAccounts_Characters_idx");
			entity.HasIndex(e => new { e.HospitalId, e.PatientId })
			      .HasDatabaseName("IX_HospitalPatientDebtAccounts_Hospital_Patient")
			      .IsUnique();

			entity.Property(e => e.Id).HasColumnType("bigint(20)");
			entity.Property(e => e.HospitalId).HasColumnType("bigint(20)");
			entity.Property(e => e.PatientId).HasColumnType("bigint(20)");
			entity.Property(e => e.PatientName).RequiredString("mediumtext");
			entity.Property(e => e.Balance).HasColumnType("decimal(58,29)");
			entity.Property(e => e.MaximumDebt).HasColumnType("decimal(58,29)");
			entity.Property(e => e.IsSuspended).HasColumnType("bit(1)");
			entity.Property(e => e.LastUpdatedAtUtc).HasColumnType("datetime(6)");

			entity.HasOne(d => d.Hospital)
			      .WithMany(p => p.PatientDebtAccounts)
			      .HasForeignKey(d => d.HospitalId)
			      .HasConstraintName("FK_HospitalPatientDebtAccounts_Hospitals");

			entity.HasOne(d => d.Patient)
			      .WithMany()
			      .HasForeignKey(d => d.PatientId)
			      .OnDelete(DeleteBehavior.Restrict)
			      .HasConstraintName("FK_HospitalPatientDebtAccounts_Characters");
		});
	}
}
