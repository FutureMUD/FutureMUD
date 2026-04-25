using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.Models;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Economy.Tax;

public class HotelFlatTax : HotelTaxBase
{
	public static void RegisterFactory()
	{
		TaxFactory.RegisterHotelTax("hotelflat", (tax, zone) => new HotelFlatTax(tax, zone),
			(name, zone) => new HotelFlatTax(name, zone));
	}

	protected HotelFlatTax(EconomicZoneTax tax, IEconomicZone economicZone) : base(tax, economicZone)
	{
		var definition = XElement.Parse(tax.Definition);
		Rate = decimal.Parse(definition.Element("Rate")?.Value ?? "0.0");
	}

	protected HotelFlatTax(string name, IEconomicZone economicZone) : base(name, economicZone,
		"<Tax><Rate>1.0</Rate></Tax>", "hotelflat")
	{
		Rate = 1.0M;
	}

	public decimal Rate { get; protected set; }

	public override decimal TaxValue(IProperty property, ICharacter patron, decimal rentalCharge)
	{
		return Rate;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.EconomicZoneTaxes.Find(Id);
		dbitem.Name = Name;
		dbitem.MerchantDescription = MerchantDescription;
		dbitem.MerchandiseFilterProgId = ApplicabilityProg?.Id;
		dbitem.Definition = new XElement("Tax", new XElement("Rate", Rate)).ToString();
		Changed = false;
	}

	protected override string HelpText =>
		$@"{base.HelpText}
	#3rate <amount>#0 - the flat tax added to each hotel room rental";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "rate":
				return BuildingCommandRate(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandRate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What flat tax per hotel stay should this tax be?");
			return false;
		}

		if (!EconomicZone.Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"That is not a valid amount of {EconomicZone.Currency.Name.ColourValue()}.");
			return false;
		}

		Rate = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} hotel tax will now be imposed at {EconomicZone.Currency.Describe(value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} per stay.");
		return true;
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Hotel Flat Tax [{Name}] (#{Id.ToString("N0", actor)})".GetLineWithTitleInner(actor, Telnet.FunctionYellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Filter Prog: {ApplicabilityProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Rate: {EconomicZone.Currency.Describe(Rate, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Description: {MerchantDescription.ColourCommand()}");
		return sb.ToString();
	}
}
