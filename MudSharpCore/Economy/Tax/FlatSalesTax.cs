using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;

namespace MudSharp.Economy.Tax;

public class FlatSalesTax : SalesTaxBase
{
	public static void RegisterFactory()
	{
		TaxFactory.RegisterSalesTax("flat", (tax, zone) => new FlatSalesTax(tax, zone),
			(name, zone) => new FlatSalesTax(name, zone));
	}

	protected FlatSalesTax(Models.EconomicZoneTax tax, IEconomicZone economicZone) : base(tax, economicZone)
	{
		var definition = XElement.Parse(tax.Definition);
		Rate = decimal.Parse(definition.Element("Rate").Value);
		foreach (var element in definition.Element("ExemptTags").Elements())
		{
			ExemptTags.Add(Gameworld.Tags.Get(long.Parse(element.Value)));
		}
	}

	protected FlatSalesTax(string name, IEconomicZone economicZone) : base(name, economicZone,
		"<Tax><Rate>1.0</Rate><ExemptTags></ExemptTags></Tax>", "flat")
	{
		Rate = 1M;
	}

	public decimal Rate { get; protected set; }
	public List<ITag> ExemptTags { get; } = new();

	public override bool Applies(IMerchandise merchandise, ICharacter purchaser)
	{
		return merchandise.Item.Tags.All(x => !ExemptTags.Contains(x)) &&
		       ApplicabilityProg?.ExecuteBool(false, purchaser, merchandise.Item.Id) != false
			;
	}

	public override decimal TaxValue(IMerchandise merchandise, ICharacter purchaser)
	{
		return Rate;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.EconomicZoneTaxes.Find(Id);
		dbitem.Name = Name;
		dbitem.MerchantDescription = MerchantDescription;
		dbitem.MerchandiseFilterProgId = ApplicabilityProg?.Id;
		dbitem.Definition = new XElement("Tax", new XElement("Rate", Rate),
			new XElement("ExemptTags", from tag in ExemptTags select new XElement("Tag", tag.Id))).ToString();
		Changed = false;
	}

	#region Implementation of IEditableItem

	protected override string HelpText =>
		$@"{base.HelpText}
	#3rate <amount>#0 - the flat tax added to the sale value
	#3tag <tag(s)>#0 - toggles item tags the item must possess for this tax to apply";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "rate":
				return BuildingCommandRate(actor, command);
			case "tag":
				return BuildingCommandTag(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandRate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What flat tax per transaction should this tax be?");
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
			$"The {Name.ColourName()} sales tax will now be imposed at a value of {EconomicZone.Currency.Describe(value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} per transaction.");
		return true;
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which tag(s) do you want to toggle as exempt from this tax?");
			return false;
		}

		var tags = new List<ITag>();
		while (!command.IsFinished)
		{
			var matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
			if (matchedtags.Count == 0)
			{
				actor.OutputHandler.Send("There is no such tag.");
				return false;
			}

			if (matchedtags.Count > 1)
			{
				actor.OutputHandler.Send(
					$"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
				return false;
			}

			tags.Add(matchedtags.Single());
		}

		var sb = new StringBuilder();
		foreach (var tag in tags)
		{
			if (ExemptTags.Contains(tag))
			{
				ExemptTags.Remove(tag);
				sb.AppendLine(
					$"The {Name.ColourName()} tax no longer exempts items with the {tag.FullName.ColourName()} tag.");
			}
			else
			{
				ExemptTags.Add(tag);
				sb.AppendLine(
					$"The {Name.ColourName()} tax now exempts items with the {tag.FullName.ColourName()} tag.");
			}
		}

		Changed = true;
		actor.OutputHandler.Send(sb.ToString());
		return true;
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Flat Sales Tax [{Name}] (#{Id.ToString("N0", actor)})".GetLineWithTitleInner(actor, Telnet.FunctionYellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(
			$"Filter Prog: {ApplicabilityProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Rate: {EconomicZone.Currency.Describe(Rate, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Description: {MerchantDescription.ColourCommand()}");
		sb.AppendLine($"Exempt Tags:");
		foreach (var tag in ExemptTags)
		{
			sb.AppendLine($"\t{tag.FullName.ColourName()} [#{tag.Id.ToString("N0", actor)}]");
		}

		return sb.ToString();
	}

	#endregion
}