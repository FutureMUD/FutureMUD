using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy.Estates;

public class EstatePayout : SaveableItem, IEstatePayout
{
	public EstatePayout(MudSharp.Models.EstatePayout payout, IEstate estate)
	{
		Gameworld = estate.Gameworld;
		_id = payout.Id;
		Estate = estate;
		_recipientReference = new FrameworkItemReference(payout.RecipientId, payout.RecipientType, Gameworld);
		Amount = payout.Amount;
		Reason = payout.Reason;
		CreatedDate = new MudDateTime(payout.CreatedDate, Gameworld);
		_collectedDate = string.IsNullOrWhiteSpace(payout.CollectedDate)
			? null
			: new MudDateTime(payout.CollectedDate, Gameworld);
	}

	public EstatePayout(IEstate estate, IFrameworkItem recipient, decimal amount, string reason)
	{
		Gameworld = estate.Gameworld;
		Estate = estate;
		_recipientReference = new FrameworkItemReference(recipient.Id, recipient.FrameworkItemType, Gameworld);
		Amount = amount;
		Reason = reason;
		CreatedDate = estate.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.EstatePayout
			{
				EstateId = estate.Id,
				RecipientId = recipient.Id,
				RecipientType = recipient.FrameworkItemType,
				Amount = amount,
				Reason = reason,
				CreatedDate = CreatedDate.GetDateTimeString(),
				CollectedDate = null
			};
			FMDB.Context.EstatePayouts.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "EstatePayout";

	public override void Save()
	{
		var dbitem = FMDB.Context.EstatePayouts.Find(Id);
		dbitem.CollectedDate = CollectedDate?.GetDateTimeString();
		Changed = false;
	}

	private readonly FrameworkItemReference _recipientReference;
	private IFrameworkItem _recipient;
	private MudDateTime _collectedDate;

	public IEstate Estate { get; }
	public IFrameworkItem Recipient => _recipient ??= _recipientReference.GetItem;
	public decimal Amount { get; }
	public string Reason { get; }
	public MudDateTime CreatedDate { get; }
	public MudDateTime CollectedDate
	{
		get => _collectedDate;
		set
		{
			_collectedDate = value;
			Changed = true;
		}
	}

	public bool IsCollected => CollectedDate != null;
}
