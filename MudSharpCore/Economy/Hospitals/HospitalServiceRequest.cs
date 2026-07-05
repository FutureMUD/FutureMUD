using System;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using DbHospitalServiceRequest = MudSharp.Models.HospitalServiceRequest;

#nullable enable

namespace MudSharp.Economy.Hospitals;

public class HospitalServiceRequest : SaveableItem, IHospitalServiceRequest
{
	private ICharacter? _requester;
	private ICharacter? _patient;
	private HospitalServiceRequestStatus _status;
	private HospitalPaymentMethod _paymentMethod;
	private decimal _amountPaid;
	private decimal _debtCharged;
	private Guid? _employmentTaskId;
	private long? _assignedEmployeeId;
	private long? _operatingTheatreCellId;
	private bool _usedInPlaceFallback;
	private bool _supplyPrepared;
	private long? _preparedByEmployeeId;
	private DateTimeOffset? _preparedAt;
	private long? _recoveryRoomCellId;
	private long? _returnCellId;
	private DateTimeOffset _lastUpdatedAt;
	private DateTimeOffset? _completedAt;
	private string _operationalNotes;

	public HospitalServiceRequest(IHospital hospital, IHospitalService service, ICharacter requester, ICharacter patient,
		HospitalPaymentMethod paymentMethod)
	{
		Gameworld = hospital.Gameworld;
		Hospital = hospital;
		Service = service;
		RequesterId = CharacterInstanceIdentityComparer.IdentityId(requester);
		RequesterName = requester.PersonalName.GetName(NameStyle.FullName);
		PatientId = CharacterInstanceIdentityComparer.IdentityId(patient);
		PatientName = patient.PersonalName.GetName(NameStyle.FullName);
		_requester = requester;
		_patient = patient;
		_status = HospitalServiceRequestStatus.Queued;
		_paymentMethod = paymentMethod;
		Price = service.Price;
		_amountPaid = 0.0M;
		_debtCharged = 0.0M;
		_returnCellId = patient.Location?.Id;
		CreatedAt = DateTimeOffset.UtcNow;
		_lastUpdatedAt = CreatedAt;
		_operationalNotes = string.Empty;

		using (new FMDB())
		{
			var dbitem = new DbHospitalServiceRequest
			{
				HospitalId = hospital.Id,
				HospitalServiceId = service.Id,
				RequesterId = RequesterId,
				RequesterName = RequesterName,
				PatientId = PatientId,
				PatientName = PatientName,
				Status = (int)_status,
				PaymentMethod = (int)_paymentMethod,
				Price = Price,
				AmountPaid = 0.0M,
				DebtCharged = 0.0M,
				UsedInPlaceFallback = false,
				SupplyPrepared = false,
				ReturnCellId = _returnCellId,
				CreatedAtUtc = CreatedAt.UtcDateTime,
				LastUpdatedAtUtc = LastUpdatedAt.UtcDateTime,
				OperationalNotes = string.Empty
			};
			FMDB.Context.HospitalServiceRequests.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public HospitalServiceRequest(DbHospitalServiceRequest request, IHospital hospital)
	{
		Gameworld = hospital.Gameworld;
		Hospital = hospital;
		_id = request.Id;
		Service = hospital.Services.FirstOrDefault(x => x.Id == request.HospitalServiceId)!;
		RequesterId = request.RequesterId;
		RequesterName = request.RequesterName;
		PatientId = request.PatientId;
		PatientName = request.PatientName;
		_status = (HospitalServiceRequestStatus)request.Status;
		_paymentMethod = (HospitalPaymentMethod)request.PaymentMethod;
		Price = request.Price;
		_amountPaid = request.AmountPaid;
		_debtCharged = request.DebtCharged;
		_employmentTaskId = Guid.TryParse(request.EmploymentTaskId, out var parsedTaskId) ? parsedTaskId : null;
		_assignedEmployeeId = request.AssignedEmployeeId;
		_operatingTheatreCellId = request.OperatingTheatreCellId;
		_usedInPlaceFallback = request.UsedInPlaceFallback;
		_supplyPrepared = request.SupplyPrepared;
		_preparedByEmployeeId = request.PreparedByEmployeeId;
		_preparedAt = request.PreparedAtUtc.HasValue
			? new DateTimeOffset(DateTime.SpecifyKind(request.PreparedAtUtc.Value, DateTimeKind.Utc))
			: null;
		_recoveryRoomCellId = request.RecoveryRoomCellId;
		_returnCellId = request.ReturnCellId;
		CreatedAt = new DateTimeOffset(DateTime.SpecifyKind(request.CreatedAtUtc, DateTimeKind.Utc));
		_lastUpdatedAt = new DateTimeOffset(DateTime.SpecifyKind(request.LastUpdatedAtUtc, DateTimeKind.Utc));
		_completedAt = request.CompletedAtUtc.HasValue
			? new DateTimeOffset(DateTime.SpecifyKind(request.CompletedAtUtc.Value, DateTimeKind.Utc))
			: null;
		_operationalNotes = request.OperationalNotes;
	}

	public override string FrameworkItemType => "HospitalServiceRequest";
	public IHospital Hospital { get; }
	public IHospitalService Service { get; }
	public long RequesterId { get; }
	public string RequesterName { get; }
	public long PatientId { get; }
	public string PatientName { get; }
	public HospitalServiceRequestStatus Status => _status;
	public HospitalPaymentMethod PaymentMethod => _paymentMethod;
	public decimal Price { get; private set; }
	public decimal AmountPaid => _amountPaid;
	public decimal DebtCharged => _debtCharged;

	public Guid? EmploymentTaskId
	{
		get => _employmentTaskId;
		set
		{
			_employmentTaskId = value;
			Touch();
		}
	}

	public long? AssignedEmployeeId
	{
		get => _assignedEmployeeId;
		set
		{
			_assignedEmployeeId = value;
			Touch();
		}
	}

	public long? OperatingTheatreCellId
	{
		get => _operatingTheatreCellId;
		set
		{
			_operatingTheatreCellId = value;
			Touch();
		}
	}

	public bool UsedInPlaceFallback
	{
		get => _usedInPlaceFallback;
		set
		{
			_usedInPlaceFallback = value;
			Touch();
		}
	}

	public bool SupplyPrepared
	{
		get => _supplyPrepared;
		set
		{
			_supplyPrepared = value;
			Touch();
		}
	}

	public long? PreparedByEmployeeId
	{
		get => _preparedByEmployeeId;
		set
		{
			_preparedByEmployeeId = value;
			Touch();
		}
	}

	public DateTimeOffset? PreparedAt
	{
		get => _preparedAt;
		set
		{
			_preparedAt = value;
			Touch();
		}
	}

	public long? RecoveryRoomCellId
	{
		get => _recoveryRoomCellId;
		set
		{
			_recoveryRoomCellId = value;
			Touch();
		}
	}

	public long? ReturnCellId
	{
		get => _returnCellId;
		set
		{
			_returnCellId = value;
			Touch();
		}
	}

	public DateTimeOffset CreatedAt { get; }
	public DateTimeOffset LastUpdatedAt => _lastUpdatedAt;
	public DateTimeOffset? CompletedAt => _completedAt;

	public string OperationalNotes
	{
		get => _operationalNotes;
		set
		{
			_operationalNotes = value;
			Touch();
		}
	}

	public ICharacter? Requester => _requester ??= Gameworld.TryGetCharacter(RequesterId, true);
	public ICharacter? Patient => _patient ??= Gameworld.TryGetCharacter(PatientId, true);

	public void MarkStatus(HospitalServiceRequestStatus status, string note)
	{
		_status = status;
		if (!string.IsNullOrWhiteSpace(note))
		{
			_operationalNotes = string.IsNullOrWhiteSpace(OperationalNotes)
				? note
				: $"{OperationalNotes}\n{note}";
		}

		if (status is HospitalServiceRequestStatus.Completed or HospitalServiceRequestStatus.Failed or
		    HospitalServiceRequestStatus.Cancelled or HospitalServiceRequestStatus.Declined)
		{
			_completedAt = DateTimeOffset.UtcNow;
		}

		Touch();
	}

	public void MarkCharged(decimal amountPaid, decimal debtCharged, decimal? finalPrice = null)
	{
		if (finalPrice is not null)
		{
			Price = Math.Max(0.0M, finalPrice.Value);
		}

		_amountPaid = amountPaid;
		_debtCharged = debtCharged;
		Touch();
	}

	public void MarkSuppliesPrepared(ICharacter employee, string note)
	{
		_supplyPrepared = true;
		_preparedByEmployeeId = CharacterInstanceIdentityComparer.IdentityId(employee);
		_preparedAt = DateTimeOffset.UtcNow;
		MarkStatus(HospitalServiceRequestStatus.Assigned, note);
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.HospitalServiceRequests.Find(Id);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.Status = (int)Status;
		dbitem.PaymentMethod = (int)PaymentMethod;
		dbitem.Price = Price;
		dbitem.AmountPaid = AmountPaid;
		dbitem.DebtCharged = DebtCharged;
		dbitem.EmploymentTaskId = EmploymentTaskId?.ToString("D");
		dbitem.AssignedEmployeeId = AssignedEmployeeId;
		dbitem.OperatingTheatreCellId = OperatingTheatreCellId;
		dbitem.UsedInPlaceFallback = UsedInPlaceFallback;
		dbitem.SupplyPrepared = SupplyPrepared;
		dbitem.PreparedByEmployeeId = PreparedByEmployeeId;
		dbitem.PreparedAtUtc = PreparedAt?.UtcDateTime;
		dbitem.RecoveryRoomCellId = RecoveryRoomCellId;
		dbitem.ReturnCellId = ReturnCellId;
		dbitem.LastUpdatedAtUtc = LastUpdatedAt.UtcDateTime;
		dbitem.CompletedAtUtc = CompletedAt?.UtcDateTime;
		dbitem.OperationalNotes = OperationalNotes;
		Changed = false;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Hospital Request #{Id.ToString("N0", actor)} - {Service.Name.ColourName()}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Hospital: {Hospital.Name.ColourName()}");
		sb.AppendLine($"Requester: {RequesterName.ColourName()} (#{RequesterId.ToString("N0", actor)})");
		sb.AppendLine($"Patient: {PatientName.ColourName()} (#{PatientId.ToString("N0", actor)})");
		sb.AppendLine($"Status: {Status.DescribeEnum().ColourName()}");
		sb.AppendLine($"Payment: {PaymentMethod.DescribeEnum().ColourName()}");
		sb.AppendLine($"Price: {Hospital.Currency.Describe(Price, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Paid: {Hospital.Currency.Describe(AmountPaid, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Debt Charged: {Hospital.Currency.Describe(DebtCharged, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Task: {(EmploymentTaskId?.ToString("D").ColourValue() ?? "None".ColourError())}");
		sb.AppendLine($"Theatre: {(OperatingTheatreCellId?.ToString("N0", actor).ColourValue() ?? (UsedInPlaceFallback ? "in-place fallback".ColourCommand() : "None".ColourError()))}");
		sb.AppendLine($"Supplies: {(SupplyPrepared ? $"prepared by #{PreparedByEmployeeId?.ToString("N0", actor) ?? "?"} at {PreparedAt?.ToString("g", actor) ?? "?"}".ColourValue() : "Not prepared".ColourError())}");
		sb.AppendLine($"Recovery Room: {(RecoveryRoomCellId?.ToString("N0", actor).ColourValue() ?? "None".ColourError())}");
		sb.AppendLine($"Return/Lobby Cell: {(ReturnCellId?.ToString("N0", actor).ColourValue() ?? "None".ColourError())}");
		sb.AppendLine($"Created: {CreatedAt.ToString("g", actor).ColourValue()}");
		sb.AppendLine($"Updated: {LastUpdatedAt.ToString("g", actor).ColourValue()}");
		sb.AppendLine($"Completed: {(CompletedAt?.ToString("g", actor).ColourValue() ?? "No".ColourError())}");
		sb.AppendLine();
		sb.AppendLine("Notes:");
		sb.AppendLine(string.IsNullOrWhiteSpace(OperationalNotes) ? "\tNone.".ColourError() : OperationalNotes.Wrap(actor.InnerLineFormatLength));
		return sb.ToString();
	}

	private void Touch()
	{
		_lastUpdatedAt = DateTimeOffset.UtcNow;
		Changed = true;
	}
}
