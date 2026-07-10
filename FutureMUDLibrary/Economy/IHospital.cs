using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.Health;

#nullable enable

namespace MudSharp.Economy;

public enum HospitalServiceType
{
	Binding,
	WoundCleaning,
	WoundClosing,
	WoundTending,
	BoneRelocation,
	BoneSetting,
	SurgicalProcedure,
	ImplantProcedure,
	BloodDonation,
	BloodTransfusion,
	Stabilisation,
	FullTreatment,
	Triage,
	DetailedExamination,
	ExploratorySurgery,
	TraumaControl,
	OrganStabilisation,
	OrganExtraction,
	OrganTransplant,
	Amputation,
	Replantation,
	SurgicalBoneSetting,
	Cannulation,
	Decannulation,
	InvasiveProcedureFinalisation,
	InstallImplant,
	RemoveImplant,
	ConfigureImplantPower,
	ConfigureImplantInterface,
	InstallProsthetic
}

public enum HospitalServiceOfferingMode
{
	StandaloneAndCombined,
	StandaloneOnly,
	CombinedOnly
}

public enum HospitalServiceRequestStatus
{
	PendingConsent,
	Queued,
	Assigned,
	InProgress,
	Completed,
	Failed,
	Cancelled,
	Declined
}

public enum HospitalPaymentMethod
{
	Cash,
	BankPaymentItem,
	Debt,
	Waived,
	DonorPayout
}

public enum HospitalLocationRole
{
	WaitingRoom,
	OperatingTheatre,
	SupplyArea,
	RecoveryRoom,
	StaffRoom
}

public enum HospitalServiceSupplyItemType
{
	ReusableTool,
	Consumable
}

public sealed record HospitalServiceEquipmentRequirement(
	int Quantity,
	EmploymentItemSelector Selector,
	HospitalServiceSupplyItemType ItemType = HospitalServiceSupplyItemType.ReusableTool);

public interface IHospitalService : IFrameworkItem, ISaveable, IKeywordedItem
{
	IHospital Hospital { get; }
	HospitalServiceType ServiceType { get; set; }
	string Description { get; set; }
	decimal Price { get; set; }
	bool IsActive { get; set; }
	bool AllowDebt { get; set; }
	bool PreferOperatingTheatre { get; set; }
	HospitalServiceOfferingMode OfferingMode { get; set; }
	int SortOrder { get; set; }
	ISurgicalProcedure? SurgicalProcedure { get; set; }
	IGameItemProto? ImplantItemPrototype { get; set; }
	ISurgicalProcedure? ImplantPowerProcedure { get; set; }
	ISurgicalProcedure? ImplantInterfaceProcedure { get; set; }
	ISurgicalProcedure? AnesthesiaCannulationProcedure { get; set; }
	string ProcedureParameters { get; set; }
	IReadOnlyList<HospitalServiceEquipmentRequirement> RequiredEquipment { get; }
	double BloodVolumeLitres { get; set; }
	bool RequiresRecovery { get; set; }
	IDrug? AnesthesiaDrug { get; set; }
	double AnesthesiaIntensity { get; set; }
	void AddRequiredEquipment(HospitalServiceEquipmentRequirement requirement);
	void RemoveRequiredEquipmentAt(int index);
	void ClearRequiredEquipment();
	void Rename(string name);
	string Show(ICharacter actor);
	void Delete();
}

public interface IHospitalPatientDebtAccount : IFrameworkItem, ISaveable
{
	IHospital Hospital { get; }
	long PatientId { get; }
	string PatientName { get; set; }
	decimal Balance { get; }
	decimal MaximumDebt { get; set; }
	bool IsSuspended { get; set; }
	DateTimeOffset LastUpdatedAt { get; }
	decimal AvailableCredit { get; }
	bool CanCharge(decimal amount, out string reason);
	void Charge(decimal amount, string reason);
	void Pay(decimal amount, string reason);
	void Forgive(decimal amount, string reason);
	string Show(ICharacter actor);
}

public interface IHospitalBloodStockPolicy : IFrameworkItem, ISaveable
{
	IHospital Hospital { get; }
	IBloodtype Bloodtype { get; }
	double TargetLitres { get; set; }
	decimal PricePerLitre { get; set; }
	string Show(ICharacter actor);
}

public interface IHospitalServiceRequest : IFrameworkItem, ISaveable
{
	IHospital Hospital { get; }
	IHospitalService Service { get; }
	long RequesterId { get; }
	string RequesterName { get; }
	long PatientId { get; }
	string PatientName { get; }
	HospitalServiceRequestStatus Status { get; }
	HospitalPaymentMethod PaymentMethod { get; }
	decimal Price { get; }
	decimal AmountPaid { get; }
	decimal DebtCharged { get; }
	Guid? EmploymentTaskId { get; set; }
	long? AssignedEmployeeId { get; set; }
	long? OperatingTheatreCellId { get; set; }
	bool UsedInPlaceFallback { get; set; }
	bool SupplyPrepared { get; set; }
	long? PreparedByEmployeeId { get; set; }
	DateTimeOffset? PreparedAt { get; set; }
	long? RecoveryRoomCellId { get; set; }
	long? ReturnCellId { get; set; }
	DateTimeOffset CreatedAt { get; }
	DateTimeOffset LastUpdatedAt { get; }
	DateTimeOffset? CompletedAt { get; }
	string OperationalNotes { get; set; }
	string ProcedureParameters { get; set; }
	ICharacter? Requester { get; }
	ICharacter? Patient { get; }
	void MarkStatus(HospitalServiceRequestStatus status, string note);
	void MarkCharged(decimal amountPaid, decimal debtCharged, decimal? finalPrice = null);
	void MarkSuppliesPrepared(ICharacter employee, string note);
	string Show(ICharacter actor);
}

public interface IHospital : IFrameworkItem, ISaveable, IKeywordedItem, IEmploymentHost
{
	IEconomicZone EconomicZone { get; set; }
	ICurrency Currency { get; }
	IBankAccount? BankAccount { get; set; }
	decimal CashBalance { get; }
	decimal AvailableFunds { get; }
	bool IsTrading { get; set; }
	bool IsReadyToDoBusiness { get; }
	decimal DefaultMaximumDebt { get; set; }
	IEnumerable<ICell> WaitingRooms { get; }
	IEnumerable<ICell> OperatingTheatres { get; }
	IEnumerable<ICell> SupplyRooms { get; }
	IEnumerable<ICell> RecoveryRooms { get; }
	IEnumerable<ICell> StaffRooms { get; }
	IEnumerable<ICell> Locations { get; }
	IEnumerable<IHospitalService> Services { get; }
	IEnumerable<IHospitalService> ActiveServices { get; }
	IEnumerable<IHospitalServiceRequest> ServiceRequests { get; }
	IEnumerable<IHospitalServiceRequest> ActiveServiceRequests { get; }
	IEnumerable<IHospitalPatientDebtAccount> PatientDebtAccounts { get; }
	IEnumerable<IHospitalBloodStockPolicy> BloodStockPolicies { get; }
	bool IsEmployee(ICharacter actor);
	bool IsManager(ICharacter actor);
	bool IsProprietor(ICharacter actor);
	bool HasLocationRole(ICell cell, HospitalLocationRole role);
	IEnumerable<HospitalLocationRole> LocationRoles(ICell cell);
	void AddLocation(ICell cell, HospitalLocationRole role);
	void RemoveLocation(ICell cell, HospitalLocationRole role);
	IHospitalService? ServiceByIdOrName(string text);
	void AddService(IHospitalService service);
	void RemoveService(IHospitalService service);
	IHospitalPatientDebtAccount? DebtAccountFor(ICharacter patient, bool createIfMissing);
	IHospitalPatientDebtAccount? DebtAccountFor(long patientId);
	IHospitalBloodStockPolicy? BloodStockPolicyFor(IBloodtype bloodtype, bool createIfMissing);
	void RemoveBloodStockPolicy(IHospitalBloodStockPolicy policy);
	void AddServiceRequest(IHospitalServiceRequest request);
	IHospitalServiceRequest? RequestById(string text);
	string Show(ICharacter actor);
	string ShowToNonEmployee(ICharacter actor);
	void Delete();
}
