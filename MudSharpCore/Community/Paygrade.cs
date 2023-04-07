using System;
using System.Collections.Generic;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Community;

public class Paygrade : SaveableItem, IPaygrade
{
	public Paygrade(MudSharp.Models.Paygrade paygrade, IClan clan)
	{
		_id = paygrade.Id;
		_name = paygrade.Name;
		Gameworld = clan.Gameworld;
		Clan = clan;
		Abbreviation = paygrade.Abbreviation;
		PayCurrency = Gameworld.Currencies.Get(paygrade.CurrencyId);
		PayAmount = paygrade.PayAmount;
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Paygrades.Find(Id);
			dbitem.Name = Name;
			dbitem.Abbreviation = Abbreviation;
			dbitem.PayAmount = PayAmount;
			dbitem.CurrencyId = PayCurrency.Id;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public override string FrameworkItemType => "Paygrade";

	#region IPaygrade Members

	public void SetName(string name)
	{
		_name = name;
	}

	public string Abbreviation { get; set; }

	public ICurrency PayCurrency { get; set; }

	public decimal PayAmount { get; set; }

	public IClan Clan { get; set; }

	#endregion

	#region IFutureProgVariable Members

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "abbreviation":
				return new TextVariable(Abbreviation);
			case "currency":
				return PayCurrency;
			case "amount":
				return new NumberVariable(PayAmount);
			default:
				throw new NotSupportedException("Invalid IFutureProgVariableType request in Paygrade.GetProperty");
		}
	}

	public FutureProgVariableTypes Type => FutureProgVariableTypes.ClanPaygrade;

	public object GetObject => this;

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "abbreviation", FutureProgVariableTypes.Text },
			{ "currency", FutureProgVariableTypes.Currency },
			{ "amount", FutureProgVariableTypes.Number }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The Id of the paygrade" },
			{ "name", "The name of the paygrade" },
			{ "abbreviation", "The abbreviation of the paygrade" },
			{ "currency", "The currency that pay is made out in" },
			{ "amount", "The amount in the currency's base pay to pay per pay period" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.ClanPaygrade, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}