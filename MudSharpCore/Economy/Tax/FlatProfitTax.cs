﻿using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Economy.Tax;

public class FlatProfitTax : ProfitTaxBase
{
	public static void RegisterFactory()
	{
		TaxFactory.RegisterProfitTax("flat", (tax, zone) => new FlatProfitTax(tax, zone),
			(name, zone) => new FlatProfitTax(name, zone));
	}

	/// <inheritdoc />
	private FlatProfitTax(EconomicZoneTax tax, IEconomicZone zone) : base(tax, zone)
	{
		var root = XElement.Parse(tax.Definition);
		TaxRate = decimal.Parse(root.Element("TaxRate").Value);
	}

	/// <inheritdoc />
	private FlatProfitTax(string name, IEconomicZone zone) : base(name, zone, "<Tax><TaxRate>1</TaxRate></Tax>", "flat")
	{
		TaxRate = 1M;
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
		sb.AppendLine($"Flat Profit Tax [{Name}] (#{Id.ToString("N0", actor)})".GetLineWithTitleInner(actor, Telnet.FunctionYellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Filter Prog: {ApplicabilityProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Tax Rate: {EconomicZone.Currency.Describe(TaxRate, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Description: {MerchantDescription.ColourCommand()}");

		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => @"#3rate <amount>#0 - sets the tax rate as a flat amount";

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
			actor.OutputHandler.Send("You must specify a currency amount for the tax.");
			return false;
		}

		if (!EconomicZone.Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid amount of {EconomicZone.Currency.Name.ColourValue()}.");
			return false;
		}

		TaxRate = value;
		Changed = true;
		actor.OutputHandler.Send($"This tax now has a tax rate of {EconomicZone.Currency.Describe(value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}.");
		return true;
	}

	/// <inheritdoc />
	public override decimal TaxValue(IShop shop, decimal grossProfit, decimal netProfit)
	{
		return TaxRate;
	}
}