#nullable enable
using System.Globalization;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp.Economy.Tax;

/// <summary>A zone rate withheld from interest actually received by a bank.</summary>
public sealed class BankInterestTax : ProfitTaxBase
{
	public static void RegisterFactory() =>
		TaxFactory.RegisterProfitTax("bankinterest", (row, zone) => new BankInterestTax(row, zone),
			(name, zone) => new BankInterestTax(name, zone));

	private BankInterestTax(EconomicZoneTax row, IEconomicZone zone) : base(row, zone)
	{
		Rate = (decimal?)XElement.Parse(row.Definition).Element("Rate") ?? 0;
	}

	private BankInterestTax(string name, IEconomicZone zone) : base(name, zone, "<Tax><Rate>0</Rate></Tax>", "bankinterest") { }

	public decimal Rate { get; private set; }
	protected override string SubtypeHelp => "RATE <percentage> sets the rate withheld on bank interest receipts.";
	public override decimal TaxValue(IShop shop, decimal grossProfit, decimal netProfit) => 0;
	public override string Show(ICharacter actor) => $"{Name.ColourName()} withholds {Rate.ToString("P2", actor)} of realised bank interest in {EconomicZone.Name.ColourName()}.";
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.PeekSpeech().EqualTo("prog"))
		{
			actor.OutputHandler.Send("Bank interest withholding applies to banks in this economic zone.");
			return false;
		}
		if (!command.PeekSpeech().EqualTo("rate")) return base.BuildingCommand(actor, command);
		command.PopSpeech();
		if (!command.SafeRemainingArgument.TryParsePercentageDecimal(actor.Account.Culture, out var rate) ||
		    rate < 0 || rate + EconomicZone.ProfitTaxes.OfType<BankInterestTax>().Where(x => x != this).Sum(x => x.Rate) > 1)
		{
			actor.OutputHandler.Send("Combined bank interest withholding must be between 0% and 100%.");
			return false;
		}
		Rate = rate;
		Changed = true;
		actor.OutputHandler.Send(Show(actor));
		return true;
	}

	public override void Save()
	{
		var row = FMDB.Context.EconomicZoneTaxes.Find(Id)!;
		row.Name = Name;
		row.MerchantDescription = MerchantDescription;
		row.Definition = new XElement("Tax", new XElement("Rate", Rate)).ToString();
		Changed = false;
	}
}
