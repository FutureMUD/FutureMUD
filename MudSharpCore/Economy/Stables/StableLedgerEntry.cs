using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.TimeAndDate;

#nullable enable

namespace MudSharp.Economy.Stables;

public class StableLedgerEntry : FrameworkItem, IStableLedgerEntry
{
	public StableLedgerEntry(IStableStay stay, StableLedgerEntryType entryType, MudDateTime mudDateTime,
		ICharacter? actor, decimal amount, string note)
	{
		Stable = stay.Stable;
		Stay = stay;
		EntryType = entryType;
		MudDateTime = mudDateTime;
		ActorId = actor?.Id;
		ActorName = actor?.PersonalName.GetName(MudSharp.Character.Name.NameStyle.FullName);
		Amount = amount;
		Note = note;
	}

	public StableLedgerEntry(MudSharp.Models.StableStayLedgerEntry entry, IStableStay stay)
	{
		_id = entry.Id;
		Stable = stay.Stable;
		Stay = stay;
		EntryType = (StableLedgerEntryType)entry.EntryType;
		MudDateTime = new MudDateTime(entry.MudDateTime, Stable.Gameworld);
		ActorId = entry.ActorId;
		ActorName = entry.ActorName;
		Amount = entry.Amount;
		Note = entry.Note;
	}

	public IStable Stable { get; }
	public IStableStay Stay { get; }
	public StableLedgerEntryType EntryType { get; }
	public MudDateTime MudDateTime { get; }
	public long? ActorId { get; }
	public string? ActorName { get; }
	public decimal Amount { get; }
	public string Note { get; }
	public override string FrameworkItemType => "StableLedgerEntry";
}
