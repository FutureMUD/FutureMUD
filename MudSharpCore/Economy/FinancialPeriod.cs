using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;

namespace MudSharp.Economy;

public class FinancialPeriod : LateInitialisingItem, IFinancialPeriod
{
	public FinancialPeriod(IEconomicZone zone, DateTime financialPeriodStart, DateTime financialPeriodEnd,
		MudDateTime financialPeriodStartMUD, MudDateTime financialPeriodEndMUD)
	{
		Gameworld = zone.Gameworld;
		Gameworld.SaveManager.AddInitialisation(this);
		EconomicZone = zone;
		FinancialPeriodStart = financialPeriodStart;
		FinancialPeriodEnd = financialPeriodEnd;
		FinancialPeriodStartMUD = financialPeriodStartMUD;
		FinancialPeriodEndMUD = financialPeriodEndMUD;
	}

	public FinancialPeriod(MudSharp.Models.FinancialPeriod period, IEconomicZone zone, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = period.Id;
		FinancialPeriodStart = period.PeriodStart;
		FinancialPeriodEnd = period.PeriodEnd;
		FinancialPeriodStartMUD = new MudDateTime(period.MudPeriodStart, gameworld);
		FinancialPeriodEndMUD = new MudDateTime(period.MudPeriodEnd, gameworld);
		EconomicZone = zone;
		IdInitialised = true;
	}

	public IEconomicZone EconomicZone { get; }
	public DateTime FinancialPeriodStart { get; }
	public DateTime FinancialPeriodEnd { get; set; }
	public MudDateTime FinancialPeriodStartMUD { get; }
	public MudDateTime FinancialPeriodEndMUD { get; }

	public int CompareTo(IFinancialPeriod other)
	{
		return FinancialPeriodEnd.CompareTo(other.FinancialPeriodEnd);
	}

	public bool InPeriod(DateTime compare)
	{
		return compare >= FinancialPeriodStart && (EconomicZone.CurrentFinancialPeriod == this || compare < FinancialPeriodEnd);
	}

	public bool InPeriod(MudDateTime compare)
	{
		return compare >= FinancialPeriodStartMUD && compare < FinancialPeriodEndMUD;
	}

	#region Overrides of Item

	public override string FrameworkItemType => "FinancialPeriod";

	#endregion

	#region Overrides of LateInitialisingItem

	/// <summary>Tells the object to perform whatever save action it needs to do</summary>
	public override void Save()
	{
		var dbitem = FMDB.Context.FinancialPeriods.Find(Id);
		dbitem.PeriodStart = FinancialPeriodStart;
		dbitem.PeriodEnd = FinancialPeriodEnd;
		dbitem.MudPeriodStart = FinancialPeriodStartMUD.GetDateTimeString();
		dbitem.MudPeriodEnd = FinancialPeriodEndMUD.GetDateTimeString();
		Changed = false;
	}

	public override object DatabaseInsert()
	{
		var dbitem = new Models.FinancialPeriod();
		FMDB.Context.FinancialPeriods.Add(dbitem);
		dbitem.EconomicZoneId = EconomicZone.Id;
		dbitem.PeriodStart = FinancialPeriodStart;
		dbitem.PeriodEnd = FinancialPeriodEnd;
		dbitem.MudPeriodStart = FinancialPeriodStartMUD.GetDateTimeString();
		dbitem.MudPeriodEnd = FinancialPeriodEndMUD.GetDateTimeString();

		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((MudSharp.Models.FinancialPeriod)dbitem).Id;
	}

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.FinancialPeriods.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.FinancialPeriods.Remove(dbitem);
					FMDB.Context.ShopFinancialPeriodResults.RemoveRange(dbitem.ShopFinancialPeriodResults);
					FMDB.Context.EconomicZoneRevenues.RemoveRange(dbitem.EconomicZoneRevenues);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	#endregion
}