using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MoreLinq.Extensions;
using MudSharp.Database;

namespace MudSharp.Economy.Tax;

public class ValueAddedTax : SalesTaxBase
{
	public static void RegisterFactory()
	{
		TaxFactory.RegisterSalesTax("vat", (tax, zone) => new ValueAddedTax(tax, zone),
			(name, zone) => new ValueAddedTax(name, zone));
	}

	protected ValueAddedTax(Models.EconomicZoneTax tax, IEconomicZone economicZone) : base(tax, economicZone)
	{
		var definition = XElement.Parse(tax.Definition);
		Rate = decimal.Parse(definition.Element("Rate").Value);
		foreach (var element in definition.Element("ExemptTags").Elements())
		{
			ExemptTags.Add(Gameworld.Tags.Get(long.Parse(element.Value)));
		}
	}

	protected ValueAddedTax(string name, IEconomicZone economicZone) : base(name, economicZone,
		"<Tax><Rate>0.1</Rate><ExemptTags></ExemptTags></Tax>", "vat")
	{
		Rate = 0.1M;
	}

	public decimal Rate { get; protected set; }
	public List<ITag> ExemptTags { get; } = new();

	public override bool Applies(IMerchandise merchandise, ICharacter purchaser)
	{
		return merchandise.Item.Tags.All(x => !ExemptTags.Contains(x)) &&
		       (bool?)ApplicabilityProg?.Execute(purchaser, merchandise.Item.Id) != false;
	}

	public override decimal TaxValue(IMerchandise merchandise, ICharacter purchaser)
	{
		return merchandise.BasePrice * Rate;
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
		$"{base.HelpText}\n\trate <%> - the percentage tax rate of the sale value\n\ttag <tag(s)> - toggles item tags the item must possess for this tax to apply";

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
			actor.OutputHandler.Send("What percentage rate of the base sale price should this tax be?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(out var value))
		{
			actor.OutputHandler.Send("That is not a valid percentage.");
			return false;
		}

		Rate = (decimal)value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} sales tax will now be imposed at a rate of {value.ToString("P2", actor).ColourValue()} of the base sale price.");
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
		sb.AppendLine($"Value Added Tax {Name.ColourName()} (#{Id.ToString("N0", actor)})");
		sb.AppendLine(
			$"Filter Prog: {ApplicabilityProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Rate: {Rate.ToString("P", actor).ColourValue()}");
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