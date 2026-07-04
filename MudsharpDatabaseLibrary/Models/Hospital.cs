using System;
using System.Collections.Generic;

#nullable enable

namespace MudSharp.Models;

public class Hospital
{
	public Hospital()
	{
		Locations = new HashSet<HospitalLocation>();
		Services = new HashSet<HospitalService>();
		ServiceRequests = new HashSet<HospitalServiceRequest>();
		PatientDebtAccounts = new HashSet<HospitalPatientDebtAccount>();
		BloodStockPolicies = new HashSet<HospitalBloodStockPolicy>();
	}

	public long Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public long EconomicZoneId { get; set; }
	public long? BankAccountId { get; set; }
	public bool IsTrading { get; set; }
	public decimal DefaultMaximumDebt { get; set; }

	public virtual EconomicZone EconomicZone { get; set; } = null!;
	public virtual BankAccount? BankAccount { get; set; }
	public virtual ICollection<HospitalLocation> Locations { get; set; }
	public virtual ICollection<HospitalService> Services { get; set; }
	public virtual ICollection<HospitalServiceRequest> ServiceRequests { get; set; }
	public virtual ICollection<HospitalPatientDebtAccount> PatientDebtAccounts { get; set; }
	public virtual ICollection<HospitalBloodStockPolicy> BloodStockPolicies { get; set; }
}

public class HospitalLocation
{
	public long HospitalId { get; set; }
	public long CellId { get; set; }
	public int Role { get; set; }

	public virtual Hospital Hospital { get; set; } = null!;
	public virtual Cell Cell { get; set; } = null!;
}

public class HospitalService
{
	public long Id { get; set; }
	public long HospitalId { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Keywords { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public int ServiceType { get; set; }
	public decimal Price { get; set; }
	public bool IsActive { get; set; }
	public bool AllowDebt { get; set; }
	public bool PreferOperatingTheatre { get; set; }
	public int SortOrder { get; set; }
	public long? SurgicalProcedureId { get; set; }
	public long? ImplantItemPrototypeId { get; set; }
	public int? ImplantItemPrototypeRevisionNumber { get; set; }
	public long? ImplantPowerProcedureId { get; set; }
	public long? ImplantInterfaceProcedureId { get; set; }
	public long? AnesthesiaCannulationProcedureId { get; set; }
	public string ProcedureParameters { get; set; } = string.Empty;
	public string? RequiredEquipmentJson { get; set; }
	public double BloodVolumeLitres { get; set; }
	public bool RequiresRecovery { get; set; }
	public long? AnesthesiaDrugId { get; set; }
	public double AnesthesiaIntensity { get; set; }

	public virtual Hospital Hospital { get; set; } = null!;
	public virtual SurgicalProcedure? SurgicalProcedure { get; set; }
	public virtual GameItemProto? ImplantItemPrototype { get; set; }
	public virtual SurgicalProcedure? ImplantPowerProcedure { get; set; }
	public virtual SurgicalProcedure? ImplantInterfaceProcedure { get; set; }
	public virtual SurgicalProcedure? AnesthesiaCannulationProcedure { get; set; }
	public virtual Drug? AnesthesiaDrug { get; set; }
}

public class HospitalServiceRequest
{
	public long Id { get; set; }
	public long HospitalId { get; set; }
	public long HospitalServiceId { get; set; }
	public long RequesterId { get; set; }
	public string RequesterName { get; set; } = string.Empty;
	public long PatientId { get; set; }
	public string PatientName { get; set; } = string.Empty;
	public int Status { get; set; }
	public int PaymentMethod { get; set; }
	public decimal Price { get; set; }
	public decimal AmountPaid { get; set; }
	public decimal DebtCharged { get; set; }
	public string? EmploymentTaskId { get; set; }
	public long? AssignedEmployeeId { get; set; }
	public long? OperatingTheatreCellId { get; set; }
	public bool UsedInPlaceFallback { get; set; }
	public bool SupplyPrepared { get; set; }
	public long? PreparedByEmployeeId { get; set; }
	public DateTime? PreparedAtUtc { get; set; }
	public long? RecoveryRoomCellId { get; set; }
	public long? ReturnCellId { get; set; }
	public DateTime CreatedAtUtc { get; set; }
	public DateTime LastUpdatedAtUtc { get; set; }
	public DateTime? CompletedAtUtc { get; set; }
	public string OperationalNotes { get; set; } = string.Empty;

	public virtual Hospital Hospital { get; set; } = null!;
	public virtual HospitalService HospitalService { get; set; } = null!;
	public virtual Character Requester { get; set; } = null!;
	public virtual Character Patient { get; set; } = null!;
	public virtual Character? AssignedEmployee { get; set; }
	public virtual Character? PreparedByEmployee { get; set; }
	public virtual Cell? OperatingTheatreCell { get; set; }
	public virtual Cell? RecoveryRoomCell { get; set; }
	public virtual Cell? ReturnCell { get; set; }
}

public class HospitalBloodStockPolicy
{
	public long Id { get; set; }
	public long HospitalId { get; set; }
	public long BloodtypeId { get; set; }
	public double TargetLitres { get; set; }
	public decimal PricePerLitre { get; set; }

	public virtual Hospital Hospital { get; set; } = null!;
	public virtual Bloodtype Bloodtype { get; set; } = null!;
}

public class HospitalPatientDebtAccount
{
	public long Id { get; set; }
	public long HospitalId { get; set; }
	public long PatientId { get; set; }
	public string PatientName { get; set; } = string.Empty;
	public decimal Balance { get; set; }
	public decimal MaximumDebt { get; set; }
	public bool IsSuspended { get; set; }
	public DateTime LastUpdatedAtUtc { get; set; }

	public virtual Hospital Hospital { get; set; } = null!;
	public virtual Character Patient { get; set; } = null!;
}
