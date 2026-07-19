using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework.Save;
using DbHospitalPatientDebtAccount = MudSharp.Models.HospitalPatientDebtAccount;

#nullable enable

namespace MudSharp.Economy.Hospitals;

public class HospitalPatientDebtAccount : SaveableItem, IHospitalPatientDebtAccount
{
	private string _patientName;
	private decimal _balance;
	private decimal _maximumDebt;
	private bool _isSuspended;
	private DateTimeOffset _lastUpdatedAt;

	public HospitalPatientDebtAccount(IHospital hospital, ICharacter patient, decimal maximumDebt)
	{
		Gameworld = hospital.Gameworld;
		Hospital = hospital;
		PatientId = CharacterInstanceIdentityComparer.IdentityId(patient);
		_patientName = patient.PersonalName.GetName(NameStyle.FullName);
		_balance = 0.0M;
		_maximumDebt = Math.Max(0.0M, maximumDebt);
		_isSuspended = false;
		_lastUpdatedAt = DateTimeOffset.UtcNow;

		using (new FMDB())
		{
			var dbitem = new DbHospitalPatientDebtAccount
			{
				HospitalId = hospital.Id,
				PatientId = PatientId,
				PatientName = _patientName,
				Balance = 0.0M,
				MaximumDebt = _maximumDebt,
				IsSuspended = false,
				LastUpdatedAtUtc = _lastUpdatedAt.UtcDateTime
			};
			FMDB.Context.HospitalPatientDebtAccounts.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public HospitalPatientDebtAccount(DbHospitalPatientDebtAccount account, IHospital hospital)
	{
		Gameworld = hospital.Gameworld;
		Hospital = hospital;
		_id = account.Id;
		PatientId = account.PatientId;
		_patientName = account.PatientName;
		_balance = account.Balance;
		_maximumDebt = account.MaximumDebt;
		_isSuspended = account.IsSuspended;
		_lastUpdatedAt = new DateTimeOffset(DateTime.SpecifyKind(account.LastUpdatedAtUtc, DateTimeKind.Utc));
	}

	public override string FrameworkItemType => "HospitalPatientDebtAccount";
	public IHospital Hospital { get; }
	public long PatientId { get; }

	public string PatientName
	{
		get => _patientName;
		set
		{
			_patientName = value;
			Changed = true;
		}
	}

	public decimal Balance => _balance;

	public decimal MaximumDebt
	{
		get => _maximumDebt;
		set
		{
			_maximumDebt = Math.Max(0.0M, value);
			Touch();
		}
	}

	public bool IsSuspended
	{
		get => _isSuspended;
		set
		{
			_isSuspended = value;
			Touch();
		}
	}

	public DateTimeOffset LastUpdatedAt => _lastUpdatedAt;
	public decimal AvailableCredit => Math.Max(0.0M, MaximumDebt - Balance);

	public bool CanCharge(decimal amount, out string reason)
	{
		if (amount <= 0.0M)
		{
			reason = string.Empty;
			return true;
		}

		if (IsSuspended)
		{
			reason = $"The hospital debt account for {PatientName} is suspended.";
			return false;
		}

		if (Balance + amount > MaximumDebt)
		{
			reason = $"That would put {PatientName} above the hospital's debt limit of {Hospital.Currency.Describe(MaximumDebt, CurrencyDescriptionPatternType.ShortDecimal)}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public void Charge(decimal amount, string reason)
	{
		if (amount <= 0.0M)
		{
			return;
		}

		_balance += amount;
		Touch();
	}

	public void Pay(decimal amount, string reason)
	{
		if (amount <= 0.0M)
		{
			return;
		}

		_balance -= amount;
		Touch();
	}

	public void Forgive(decimal amount, string reason)
	{
		if (amount <= 0.0M || Balance <= 0.0M)
		{
			return;
		}

		_balance = Math.Max(0.0M, Balance - amount);
		Touch();
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.HospitalPatientDebtAccounts.Find(Id);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.PatientName = PatientName;
		dbitem.Balance = Balance;
		dbitem.MaximumDebt = MaximumDebt;
		dbitem.IsSuspended = IsSuspended;
		dbitem.LastUpdatedAtUtc = LastUpdatedAt.UtcDateTime;
		Changed = false;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Hospital Debt #{Id.ToString("N0", actor)} - {PatientName.ColourName()}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Hospital: {Hospital.Name.ColourName()}");
		sb.AppendLine($"Balance: {Hospital.Currency.Describe(Balance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		if (Balance < 0.0M)
		{
			sb.AppendLine($"Prepaid Credit: {Hospital.Currency.Describe(-Balance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		}

		sb.AppendLine($"Maximum Debt: {Hospital.Currency.Describe(MaximumDebt, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Available Credit: {Hospital.Currency.Describe(AvailableCredit, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Suspended: {IsSuspended.ToColouredString()}");
		sb.AppendLine($"Last Updated: {LastUpdatedAt.ToString("g", actor).ColourValue()}");
		return sb.ToString();
	}

	private void Touch()
	{
		_lastUpdatedAt = DateTimeOffset.UtcNow;
		Changed = true;
	}
}
