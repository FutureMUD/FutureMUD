using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Health;
using DbHospital = MudSharp.Models.Hospital;
using DbHospitalLocation = MudSharp.Models.HospitalLocation;

#nullable enable

namespace MudSharp.Economy.Hospitals;

public partial class Hospital : SavableKeywordedItem, IHospital
{
	private sealed record HospitalLocationAssignment(ICell Cell, HospitalLocationRole Role);

	private readonly List<HospitalLocationAssignment> _locations = new();
	private readonly List<IHospitalService> _services = new();
	private readonly List<IHospitalServiceRequest> _serviceRequests = new();
	private readonly List<IHospitalPatientDebtAccount> _patientDebtAccounts = new();
	private readonly List<IHospitalBloodStockPolicy> _bloodStockPolicies = new();
	private IEconomicZone _economicZone;
	private long? _bankAccountId;
	private IBankAccount? _bankAccount;
	private bool _isTrading;
	private decimal _defaultMaximumDebt;

	public Hospital(IEconomicZone zone, IBankAccount? bankAccount, string name)
	{
		Gameworld = zone.Gameworld;
		_name = name;
		SetKeywordsFromSDesc(name);
		_economicZone = zone;
		_bankAccountId = bankAccount?.Id;
		_bankAccount = bankAccount;
		_isTrading = true;
		_defaultMaximumDebt = 0.0M;

		using (new FMDB())
		{
			var dbitem = new DbHospital
			{
				Name = name,
				EconomicZoneId = zone.Id,
				BankAccountId = bankAccount?.Id,
				IsTrading = true,
				DefaultMaximumDebt = 0.0M
			};
			FMDB.Context.Hospitals.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		EnsureAutomaticCombinedServices();
	}

	public Hospital(DbHospital hospital, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = hospital.Id;
		_name = hospital.Name;
		SetKeywordsFromSDesc(hospital.Name);
		_economicZone = gameworld.EconomicZones.Get(hospital.EconomicZoneId)!;
		_bankAccountId = hospital.BankAccountId;
		_isTrading = hospital.IsTrading;
		_defaultMaximumDebt = hospital.DefaultMaximumDebt;

		foreach (var location in hospital.Locations.OrderBy(x => x.Role).ThenBy(x => x.CellId))
		{
			var cell = gameworld.Cells.Get(location.CellId);
			if (cell is null)
			{
				continue;
			}

			_locations.Add(new HospitalLocationAssignment(cell, (HospitalLocationRole)location.Role));
		}

		foreach (var service in hospital.Services.OrderBy(x => x.SortOrder).ThenBy(x => x.Name))
		{
			_services.Add(new HospitalService(service, this));
		}

		foreach (var account in hospital.PatientDebtAccounts.OrderBy(x => x.PatientName))
		{
			_patientDebtAccounts.Add(new HospitalPatientDebtAccount(account, this));
		}

		foreach (var policy in hospital.BloodStockPolicies.OrderBy(x => x.BloodtypeId))
		{
			if (gameworld.Bloodtypes.Get(policy.BloodtypeId) is null)
			{
				continue;
			}

			_bloodStockPolicies.Add(new HospitalBloodStockPolicy(policy, this));
		}

		foreach (var request in hospital.ServiceRequests.OrderBy(x => x.Id))
		{
			var loaded = new HospitalServiceRequest(request, this);
			if (loaded.Service is not null)
			{
				_serviceRequests.Add(loaded);
			}
		}

		EnsureAutomaticCombinedServices();
	}

	public override string FrameworkItemType => "Hospital";

	public IEconomicZone EconomicZone
	{
		get => _economicZone;
		set
		{
			if (_economicZone == value)
			{
				return;
			}

			_economicZone = value;
			_bankAccount = null;
			_bankAccountId = null;
			Changed = true;
		}
	}

	public ICurrency Currency => EconomicZone.Currency;

	public IBankAccount? BankAccount
	{
		get
		{
			if (_bankAccount is null && _bankAccountId is not null)
			{
				_bankAccount = Gameworld.BankAccounts.Get(_bankAccountId.Value);
			}

			return _bankAccount;
		}
		set
		{
			_bankAccount = value;
			_bankAccountId = value?.Id;
			Changed = true;
		}
	}

	public decimal CashBalance => VirtualCashLedger.Balance(this, Currency);
	public decimal AvailableFunds => VirtualCashLedger.AvailableFunds(this, Currency, BankAccount);

	public bool IsTrading
	{
		get => _isTrading;
		set
		{
			if (_isTrading == value)
			{
				return;
			}

			_isTrading = value;
			Changed = true;
		}
	}

	public bool IsReadyToDoBusiness => IsTrading &&
	                                   ActiveServices.Any(HospitalMedicalServiceRunner.CanBeRequestedStandalone) &&
	                                   WaitingRooms.Any();

	public decimal DefaultMaximumDebt
	{
		get => _defaultMaximumDebt;
		set
		{
			_defaultMaximumDebt = Math.Max(0.0M, value);
			Changed = true;
		}
	}

	public IEnumerable<ICell> WaitingRooms => _locations
		.Where(x => x.Role == HospitalLocationRole.WaitingRoom)
		.Select(x => x.Cell)
		.DistinctBy(x => x.Id);

	public IEnumerable<ICell> OperatingTheatres => _locations
		.Where(x => x.Role == HospitalLocationRole.OperatingTheatre)
		.Select(x => x.Cell)
		.DistinctBy(x => x.Id);

	public IEnumerable<ICell> SupplyRooms => _locations
		.Where(x => x.Role == HospitalLocationRole.SupplyArea)
		.Select(x => x.Cell)
		.DistinctBy(x => x.Id);

	public IEnumerable<ICell> RecoveryRooms => _locations
		.Where(x => x.Role == HospitalLocationRole.RecoveryRoom)
		.Select(x => x.Cell)
		.DistinctBy(x => x.Id);

	public IEnumerable<ICell> StaffRooms => _locations
		.Where(x => x.Role == HospitalLocationRole.StaffRoom)
		.Select(x => x.Cell)
		.DistinctBy(x => x.Id);

	public IEnumerable<ICell> Locations => _locations
		.Select(x => x.Cell)
		.DistinctBy(x => x.Id);

	public IEnumerable<IHospitalService> Services => _services;
	public IEnumerable<IHospitalService> ActiveServices => _services.Where(x => x.IsActive);
	public IEnumerable<IHospitalServiceRequest> ServiceRequests => _serviceRequests;
	public IEnumerable<IHospitalServiceRequest> ActiveServiceRequests => _serviceRequests.Where(x =>
		x.Status is HospitalServiceRequestStatus.Queued or HospitalServiceRequestStatus.Assigned or
			HospitalServiceRequestStatus.InProgress);
	public IEnumerable<IHospitalPatientDebtAccount> PatientDebtAccounts => _patientDebtAccounts;
	public IEnumerable<IHospitalBloodStockPolicy> BloodStockPolicies => _bloodStockPolicies;

	public override void Save()
	{
		var context = FMDB.Context;
		var dbitem = context.Hospitals.Find(Id);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.Name = Name;
		dbitem.EconomicZoneId = EconomicZone.Id;
		dbitem.BankAccountId = BankAccount?.Id;
		dbitem.IsTrading = IsTrading;
		dbitem.DefaultMaximumDebt = DefaultMaximumDebt;

		var existingLocations = context.HospitalLocations.Where(x => x.HospitalId == Id).ToList();
		context.HospitalLocations.RemoveRange(existingLocations);
		foreach (var location in _locations.DistinctBy(x => (x.Cell.Id, x.Role)))
		{
			context.HospitalLocations.Add(new DbHospitalLocation
			{
				HospitalId = Id,
				CellId = location.Cell.Id,
				Role = (int)location.Role
			});
		}

		Changed = false;
	}

	public bool IsEmployee(ICharacter actor)
	{
		return this.HasActiveEmploymentContract(actor);
	}

	public bool IsManager(ICharacter actor)
	{
		return this.HasManagerEmploymentAccess(actor);
	}

	public bool IsProprietor(ICharacter actor)
	{
		return this.HasProprietorEmploymentAccess(actor);
	}

	public bool HasLocationRole(ICell cell, HospitalLocationRole role)
	{
		return _locations.Any(x => x.Cell.Id == cell.Id && x.Role == role);
	}

	public IEnumerable<HospitalLocationRole> LocationRoles(ICell cell)
	{
		return _locations
			.Where(x => x.Cell.Id == cell.Id)
			.Select(x => x.Role)
			.Distinct();
	}

	public void AddLocation(ICell cell, HospitalLocationRole role)
	{
		if (_locations.Any(x => x.Cell.Id == cell.Id && x.Role == role))
		{
			return;
		}

		_locations.Add(new HospitalLocationAssignment(cell, role));
		Changed = true;
	}

	public void RemoveLocation(ICell cell, HospitalLocationRole role)
	{
		if (_locations.RemoveAll(x => x.Cell.Id == cell.Id && x.Role == role) > 0)
		{
			Changed = true;
		}
	}

	public IHospitalService? ServiceByIdOrName(string text)
	{
		if (long.TryParse(text.TrimStart('#'), out var id))
		{
			return _services.FirstOrDefault(x => x.Id == id);
		}

		return _services.FirstOrDefault(x => x.Name.EqualTo(text)) ??
		       _services.FirstOrDefault(x => x.Keywords.Any(y => y.EqualTo(text))) ??
		       _services.FirstOrDefault(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
	}

	public void AddService(IHospitalService service)
	{
		if (_services.Any(x => x.Id == service.Id))
		{
			return;
		}

		_services.Add(service);
	}

	public void RemoveService(IHospitalService service)
	{
		_services.Remove(service);
	}

	public IHospitalPatientDebtAccount? DebtAccountFor(ICharacter patient, bool createIfMissing)
	{
		var patientId = CharacterInstanceIdentityComparer.IdentityId(patient);
		var existing = DebtAccountFor(patientId);
		if (existing is not null || !createIfMissing)
		{
			return existing;
		}

		var account = new HospitalPatientDebtAccount(this, patient, DefaultMaximumDebt);
		_patientDebtAccounts.Add(account);
		return account;
	}

	public IHospitalPatientDebtAccount? DebtAccountFor(long patientId)
	{
		return _patientDebtAccounts.FirstOrDefault(x => x.PatientId == patientId);
	}

	public IHospitalBloodStockPolicy? BloodStockPolicyFor(IBloodtype bloodtype, bool createIfMissing)
	{
		var existing = _bloodStockPolicies.FirstOrDefault(x => x.Bloodtype.Id == bloodtype.Id);
		if (existing is not null || !createIfMissing)
		{
			return existing;
		}

		var policy = new HospitalBloodStockPolicy(this, bloodtype);
		_bloodStockPolicies.Add(policy);
		return policy;
	}

	public void RemoveBloodStockPolicy(IHospitalBloodStockPolicy policy)
	{
		if (_bloodStockPolicies.RemoveAll(x => x.Id == policy.Id) == 0)
		{
			return;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.HospitalBloodStockPolicies.Find(policy.Id);
			if (dbitem is not null)
			{
				FMDB.Context.HospitalBloodStockPolicies.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}
	}

	public void AddServiceRequest(IHospitalServiceRequest request)
	{
		if (_serviceRequests.Any(x => x.Id == request.Id))
		{
			return;
		}

		_serviceRequests.Add(request);
	}

	public IHospitalServiceRequest? RequestById(string text)
	{
		return long.TryParse(text.TrimStart('#'), out var id)
			? _serviceRequests.FirstOrDefault(x => x.Id == id)
			: null;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Hospital #{Id.ToString("N0", actor)} - {Name.TitleCase().ColourName()}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Economic Zone: {EconomicZone.Name.ColourName()}");
		sb.AppendLine($"Bank Account: {(BankAccount is null ? "None".ColourError() : BankAccount.AccountReference.ColourValue())}");
		sb.AppendLine($"Virtual Cash: {Currency.Describe(CashBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Available Funds: {Currency.Describe(AvailableFunds, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Open: {IsTrading.ToColouredString()}");
		sb.AppendLine($"Ready: {IsReadyToDoBusiness.ToColouredString()}");
		sb.AppendLine($"Default Maximum Debt: {Currency.Describe(DefaultMaximumDebt, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Waiting Rooms: {WaitingRooms.Select(x => x.GetFriendlyReference(actor)).DefaultIfEmpty("none".ColourError()).ListToString()}");
		sb.AppendLine($"Operating Theatres: {OperatingTheatres.Select(x => x.GetFriendlyReference(actor)).DefaultIfEmpty("none".ColourError()).ListToString()}");
		sb.AppendLine($"Supply Rooms: {SupplyRooms.Select(x => x.GetFriendlyReference(actor)).DefaultIfEmpty("none".ColourError()).ListToString()}");
		sb.AppendLine($"Recovery Rooms: {RecoveryRooms.Select(x => x.GetFriendlyReference(actor)).DefaultIfEmpty("none".ColourError()).ListToString()}");
		sb.AppendLine($"Staff Rooms: {StaffRooms.Select(x => x.GetFriendlyReference(actor)).DefaultIfEmpty("none".ColourError()).ListToString()}");
		sb.AppendLine($"Services: {Services.Count().ToString("N0", actor).ColourValue()} ({ActiveServices.Count().ToString("N0", actor).ColourValue()} active)");
		sb.AppendLine($"Active Requests: {ActiveServiceRequests.Count().ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Debt Accounts: {PatientDebtAccounts.Count().ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Blood Stock Policies: {BloodStockPolicies.Count().ToString("N0", actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Employees:");
		sb.AppendLine(this.ActiveEmploymentContractsTable(actor));
		return sb.ToString();
	}

	public string ShowToNonEmployee(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(Name.GetLineWithTitleInner(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Open: {IsTrading.ToColouredString()}");
		sb.AppendLine($"Ready: {IsReadyToDoBusiness.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Available Services:");
		sb.AppendLine(StringUtilities.GetTextTable(
			from service in ActiveServices
				.Where(HospitalMedicalServiceRunner.CanBeRequestedStandalone)
				.OrderBy(x => x.SortOrder)
				.ThenBy(x => x.Name)
			select new List<string>
			{
				service.Id.ToString("N0", actor),
				service.Name,
				service.ServiceType.DescribeEnum(),
				HospitalServiceBilling.DescribePrice(this, service, actor),
				service.AllowDebt.ToColouredString(),
				HospitalServiceAvailability.Evaluate(this, service, actor).DescribeColoured()
			},
			new List<string> { "#", "Service", "Type", "Price", "Debt", "Status" },
			actor,
			Telnet.Cyan));

		var account = DebtAccountFor(CharacterInstanceIdentityComparer.IdentityId(actor));
		if (account is not null)
		{
			sb.AppendLine();
			sb.AppendLine($"Your Balance: {Currency.Describe(account.Balance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
			sb.AppendLine($"Your Available Credit: {Currency.Describe(account.AvailableCredit, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		}

		return sb.ToString();
	}

	private void EnsureAutomaticCombinedServices()
	{
		EnsureAutomaticCombinedService(
			HospitalServiceType.Stabilisation,
			"Stabilisation",
			"Emergency stabilisation billed from the concrete treatments actually performed.");
		EnsureAutomaticCombinedService(
			HospitalServiceType.FullTreatment,
			"Full Treatment",
			"Comprehensive treatment billed from the concrete treatments actually performed.");
	}

	private void EnsureAutomaticCombinedService(HospitalServiceType serviceType, string name, string description)
	{
		if (_services.Any(x => x.ServiceType == serviceType))
		{
			return;
		}

		var service = new HospitalService(this, name, serviceType, 0.0M)
		{
			Description = description,
			AllowDebt = true,
			PreferOperatingTheatre = true,
			RequiresRecovery = true
		};
		_services.Add(service);
	}

	public void Delete()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Hospitals
			                     .Include(x => x.Locations)
			                     .Include(x => x.Services)
			                     .Include(x => x.ServiceRequests)
			                     .Include(x => x.PatientDebtAccounts)
				                     .Include(x => x.BloodStockPolicies)
			                     .FirstOrDefault(x => x.Id == Id);
			if (dbitem is not null)
			{
				FMDB.Context.Hospitals.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}
	}
}
