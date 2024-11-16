using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Economy.Tax;

public class GrossProfitTax : ProfitTaxBase
{
	public static void RegisterFactory()
	{
		TaxFactory.RegisterProfitTax("gross", (tax, zone) => new GrossProfitTax(tax, zone),
			(name, zone) => new GrossProfitTax(name, zone));
	}

	/// <inheritdoc />
	private GrossProfitTax(EconomicZoneTax tax, IEconomicZone zone) : base(tax, zone)
	{
		var root = XElement.Parse(tax.Definition);
		TaxRate = decimal.Parse(root.Element("TaxRate").Value);
	}

	/// <inheritdoc />
	private GrossProfitTax(string name, IEconomicZone zone) : base(name, zone, "<Tax><TaxRate>0.01</TaxRate></Tax>", "gross")
	{
		TaxRate = 0.01M;
	}

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.EconomicZoneTaxes.Find(Id);
		dbitem.Name = Name;
		dbitem.MerchantDescription = MerchantDescription;
		dbitem.MerchandiseFilterProgId = ApplicabilityProg?.Id;
		dbitem.Definition = new XElement("Tax", 
			new XElement("TaxRate", TaxRate)
			).ToString();
		Changed = false;
	}

	public decimal TaxRate { get; private set; }

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Gross Profit Tax [{Name}] (#{Id.ToString("N0", actor)})".GetLineWithTitleInner(actor, Telnet.FunctionYellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Filter Prog: {ApplicabilityProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Tax Rate: {TaxRate.ToString("P", actor).ColourValue()}");
		sb.AppendLine($"Description: {MerchantDescription.ColourCommand()}");

		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => @"#3rate <%>#0 - sets the tax rate";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "rate":
				return BuildingCommandRate(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandRate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a percentage.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentageDecimal(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		TaxRate = value;
		Changed = true;
		actor.OutputHandler.Send($"This tax now has a tax rate of {value.ToStringP2Colour(actor)}.");
		return true;
	}

	/// <inheritdoc />
	public override decimal TaxValue(IShop shop, decimal grossProfit, decimal netProfit)
	{
		return grossProfit * TaxRate;
	}
}
