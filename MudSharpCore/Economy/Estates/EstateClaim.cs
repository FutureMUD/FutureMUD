using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy.Estates;

public class EstateClaim : SaveableItem, IEstateClaim
{
	public EstateClaim(MudSharp.Models.EstateClaim claim, IEstate estate)
	{
		Gameworld = estate.Gameworld;
		_id = claim.Id;
		Estate = estate;
		_claimantReference = new FrameworkItemReference(claim.ClaimantId, claim.ClaimantType, Gameworld);
		if (claim.TargetId.HasValue && !string.IsNullOrWhiteSpace(claim.TargetType))
		{
			_targetReference = new FrameworkItemReference(claim.TargetId.Value, claim.TargetType, Gameworld);
		}

		_amount = claim.Amount;
		_reason = claim.Reason;
		_status = (ClaimStatus)claim.ClaimStatus;
		_statusReason = claim.StatusReason;
		IsSecured = claim.IsSecured;
		ClaimDate = new MudDateTime(claim.ClaimDate, Gameworld);
	}

	public EstateClaim(IEstate estate, IFrameworkItem claimant, decimal amount, string reason, ClaimStatus status,
		bool isSecured, IFrameworkItem targetItem = null)
	{
		Gameworld = estate.Gameworld;
		Estate = estate;
		_claimantReference = new FrameworkItemReference(claimant.Id, claimant.FrameworkItemType, Gameworld);
		_targetReference = targetItem == null
			? null
			: new FrameworkItemReference(targetItem.Id, targetItem.FrameworkItemType, Gameworld);
		_amount = amount;
		_reason = reason;
		_status = status;
		IsSecured = isSecured;
		ClaimDate = estate.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.EstateClaim
			{
				EstateId = estate.Id,
				ClaimantId = claimant.Id,
				ClaimantType = claimant.FrameworkItemType,
				TargetId = targetItem?.Id,
				TargetType = targetItem?.FrameworkItemType,
				Amount = amount,
				Reason = reason,
				ClaimStatus = (int)status,
				StatusReason = null,
				IsSecured = isSecured,
				ClaimDate = ClaimDate.GetDateTimeString()
			};
			FMDB.Context.EstateClaims.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "EstateClaim";

	public override void Save()
	{
		var dbitem = FMDB.Context.EstateClaims.Find(Id);
		dbitem.Amount = Amount;
		dbitem.Reason = Reason;
		dbitem.ClaimStatus = (int)Status;
		dbitem.StatusReason = StatusReason;
		Changed = false;
	}

	private readonly FrameworkItemReference _claimantReference;
	private readonly FrameworkItemReference _targetReference;
	private IFrameworkItem _claimant;
	private IFrameworkItem _target;

	public IEstate Estate { get; }
	public IFrameworkItem Claimant => _claimant ??= _claimantReference.GetItem;
	public IFrameworkItem TargetItem => _targetReference == null ? null : _target ??= _targetReference.GetItem;
	private decimal _amount;
	public decimal Amount
	{
		get => _amount;
		set
		{
			_amount = value;
			Changed = true;
		}
	}

	private string _reason;
	public string Reason
	{
		get => _reason;
		set
		{
			_reason = value;
			Changed = true;
		}
	}

	private ClaimStatus _status;
	public ClaimStatus Status
	{
		get => _status;
		set
		{
			_status = value;
			Changed = true;
		}
	}

	private string _statusReason;
	public string StatusReason
	{
		get => _statusReason;
		set
		{
			_statusReason = value;
			Changed = true;
		}
	}
	public bool IsSecured { get; }
	public MudDateTime ClaimDate { get; }
}
