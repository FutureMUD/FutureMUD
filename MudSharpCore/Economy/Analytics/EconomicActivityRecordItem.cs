#nullable enable

using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Economy.Analytics;

internal sealed class EconomicActivityRecordItem : LateInitialisingItem
{
	private readonly Models.EconomicActivityRecord _record;

	public EconomicActivityRecordItem(IFuturemud gameworld, Models.EconomicActivityRecord record)
	{
		Gameworld = gameworld;
		_record = record;
		Gameworld.SaveManager.AddInitialisation(this);
	}

	public Models.EconomicActivityRecord Record => _record;

	public override void Save()
	{
		throw new ApplicationException("Economic activity records are immutable after creation.");
	}

	public override object DatabaseInsert()
	{
		FMDB.Context.EconomicActivityRecords.Add(_record);
		return _record;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((Models.EconomicActivityRecord)dbitem).Id;
	}

	public override string FrameworkItemType => "EconomicActivityRecord";
}
