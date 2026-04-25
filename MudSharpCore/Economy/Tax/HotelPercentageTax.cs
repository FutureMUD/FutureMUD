using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.Models;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Economy.Tax;

public class HotelPercentageTax : HotelTaxBase
{
	public static void RegisterFactory()
	{
		TaxFactory.RegisterHotelTax("hotelpercent", (tax, zone) => new HotelPercentageTax(tax, zone),
			(name, zone) => new HotelPercentageTax(name, zone));
	}

	protected HotelPercentageTax(EconomicZoneTax tax, IEconomicZone economicZone) : base(tax, economicZone)
	{
		var definition = XElement.Parse(tax.Definition);
		Rate = decimal.Parse(definition.Element("Rate")?.Value ?? "0.1");
	}

	protected HotelPercentageTax(string name, IEconomicZone economicZone) : base(name, economicZone,
		"<Tax><Rate>0.1</Rate></Tax>", "hotelpercent")
	{
		Rate = 0.1M;
	}

	public decimal Rate { get; protected set; }

	public override decimal TaxValue(IProperty property, ICharacter patron, decimal rentalCharge)
	{
		return rentalCharge * Rate;
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
	#3rate <%>#0 - the percentage tax rate of the room rental charge";

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
			actor.OutputHandler.Send("What percentage rate of the room rental charge should this tax be?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentageDecimal(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send("That is not a valid percentage.");
			return false;
		}

		Rate = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} hotel tax will now be imposed at {value.ToString("P2", actor).ColourValue()} of the room rental charge.");
		return true;
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Hotel Percentage Tax [{Name}] (#{Id.ToString("N0", actor)})".GetLineWithTitleInner(actor, Telnet.FunctionYellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Filter Prog: {ApplicabilityProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Rate: {Rate.ToString("P", actor).ColourValue()}");
		sb.AppendLine($"Description: {MerchantDescription.ColourCommand()}");
		return sb.ToString();
	}
}
