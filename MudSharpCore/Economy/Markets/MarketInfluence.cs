using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Economy.Markets;

public class MarketInfluence : SaveableItem, IMarketInfluence
{
	/// <inheritdoc />
	public sealed override string FrameworkItemType => "MarketInfluence";

	public MarketInfluence(IMarket market, IMarketInfluenceTemplate template, string name, MudDateTime appliesFrom,
		[CanBeNull] MudDateTime appliesUntil)
	{
		Gameworld = market.Gameworld;
		Market = market;
		MarketInfluenceTemplate = template;
		_name = name;
		Description = template.Description;
		AppliesFrom = appliesFrom;
		AppliesUntil = appliesUntil;
		_marketImpacts.AddRange(template.MarketImpacts);
		CharacterKnowsAboutInfluenceProg = template.CharacterKnowsAboutInfluenceProg;
	}

	/// <inheritdoc />
	public override void Save()
	{
		throw new System.NotImplementedException();
	}

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		throw new System.NotImplementedException();
	}

	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Market Influence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine($"Market: {Market.Name.ColourValue()} (#{Market.Id.ToString("N0", actor).ColourValue()})");
		sb.AppendLine($"Template: {MarketInfluenceTemplate?.Name.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine($"Character Knows Prog: {CharacterKnowsAboutInfluenceProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Applies From: {AppliesFrom.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()}");
		sb.AppendLine($"Applies Until: {AppliesUntil?.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue() ?? "Forever".Colour(Telnet.Magenta)}");
		sb.AppendLine();
		sb.AppendLine("Description:");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		sb.AppendLine("Impacts:");
		sb.AppendLine();
		foreach (var impact in _marketImpacts)
		{
			sb.AppendLine($"\t{impact.MarketCategory.Name.ColourName()}: Supply {impact.SupplyImpact.ToBonusString()}, Demand {impact.DemandImpact.ToBonusString()}");
		}
		return sb.ToString();
	}

	/// <inheritdoc />
	public IMarket Market { get; set; }

	/// <inheritdoc />
	public IMarketInfluenceTemplate MarketInfluenceTemplate { get; set; }

	/// <inheritdoc />
	public string Description { get; set; }

	/// <inheritdoc />
	public MudDateTime AppliesFrom { get; set; }

	/// <inheritdoc />
	public MudDateTime AppliesUntil { get; set; }

	private readonly List<MarketImpact> _marketImpacts = new();

	/// <inheritdoc />
	public IEnumerable<MarketImpact> MarketImpacts => _marketImpacts;

	public IFutureProg CharacterKnowsAboutInfluenceProg { get; set; }

	/// <inheritdoc />
	public bool CharacterKnowsAboutInfluence(ICharacter character)
	{
		return CharacterKnowsAboutInfluenceProg?.ExecuteBool(character) != false;
	}

	/// <inheritdoc />
	public bool Applies(IMarketCategory category, MudDateTime currentDateTime)
	{
		return AppliesFrom <= currentDateTime &&
		       (AppliesUntil is null || AppliesUntil >= currentDateTime) &&
		       _marketImpacts.Any(x => x.MarketCategory == category);
	}

	/// <inheritdoc />
	public string TextForMarketShow(ICharacter actor)
	{
		var now = Market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		
		if (AppliesUntil is null)
		{
			if (AppliesFrom <= now)
			{
				return $"\t#{Id.ToString("N0", actor)}) {Name.ColourName()} - Impacts {_marketImpacts.Count.ToString("N0", actor).ColourValue()} {"category".Pluralise(_marketImpacts.Count != 1)} - Active Until Revoked";
			}

			return $"\t#{Id.ToString("N0", actor)}) {Name.ColourName()} - Impacts {_marketImpacts.Count.ToString("N0", actor).ColourValue()} {"category".Pluralise(_marketImpacts.Count != 1)} - Begins {AppliesFrom.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()} Until Revoked";
		}

		if (AppliesFrom <= now)
		{
			return $"\t#{Id.ToString("N0", actor)}) {Name.ColourName()} - Impacts {_marketImpacts.Count.ToString("N0", actor).ColourValue()} {"category".Pluralise(_marketImpacts.Count != 1)} - Active Until {AppliesUntil.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()}";
		}

		return $"\t#{Id.ToString("N0", actor)}) {Name.ColourName()} - Impacts {_marketImpacts.Count.ToString("N0", actor).ColourValue()} {"category".Pluralise(_marketImpacts.Count != 1)} - Begins {AppliesFrom.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()} Until {AppliesUntil.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()}";
	}
}