using MudSharp.Health;
using System.Globalization;

namespace MudSharp.Combat;

public class FireProfile : IFireProfile
{
    private readonly IFuturemud _gameworld;
    private readonly List<ITag> _extinguishTags = new();

    public FireProfile(IFuturemud gameworld)
    {
        _gameworld = gameworld;
        Name = "Fire";
        DamageType = DamageType.Burning;
        DamagePerTick = 1.0;
		TickFrequency = TimeSpan.FromSeconds(10);
        MinimumOxidation = 0.1;
        SpreadChance = 0.1;
    }

    public FireProfile(XElement root, IFuturemud gameworld)
    {
        _gameworld = gameworld;
        Name = root.Element("Name")?.Value ?? "Fire";
        DamageType = (DamageType)int.Parse(root.Element("DamageType")?.Value ?? $"{(int)DamageType.Burning}");
        DamagePerTick = double.Parse(root.Element("DamagePerTick")?.Value ?? "0", CultureInfo.InvariantCulture);
        PainPerTick = double.Parse(root.Element("PainPerTick")?.Value ?? "0", CultureInfo.InvariantCulture);
        StunPerTick = double.Parse(root.Element("StunPerTick")?.Value ?? "0", CultureInfo.InvariantCulture);
        ThermalLoadPerTick = double.Parse(root.Element("ThermalLoadPerTick")?.Value ?? "0", CultureInfo.InvariantCulture);
        SpreadChance = double.Parse(root.Element("SpreadChance")?.Value ?? "0", CultureInfo.InvariantCulture);
        MinimumOxidation = double.Parse(root.Element("MinimumOxidation")?.Value ?? "0", CultureInfo.InvariantCulture);
        SelfOxidising = bool.Parse(root.Element("SelfOxidising")?.Value ?? "false");
		TickFrequency = TimeSpan.FromSeconds(Math.Max(0.1,
			double.Parse(root.Element("TickFrequencySeconds")?.Value ?? "10", CultureInfo.InvariantCulture)));
        foreach (XElement tag in root.Element("ExtinguishTags")?.Elements("Tag") ?? Enumerable.Empty<XElement>())
        {
            if (!long.TryParse(tag.Value, out long id))
            {
                continue;
            }

            ITag loadedTag = _gameworld.Tags.Get(id);
            if (loadedTag is not null)
            {
                _extinguishTags.Add(loadedTag);
            }
        }
    }

    public string Name { get; set; }
    public DamageType DamageType { get; set; }
    public double DamagePerTick { get; set; }
    public double PainPerTick { get; set; }
    public double StunPerTick { get; set; }
    public double ThermalLoadPerTick { get; set; }
    public double SpreadChance { get; set; }
    public double MinimumOxidation { get; set; }
    public bool SelfOxidising { get; set; }
	public TimeSpan TickFrequency { get; set; }
	public IEnumerable<ITag> ExtinguishTags => _extinguishTags;

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What name should this fire profile have?");
					return false;
				}

				Name = command.SafeRemainingArgument.TitleCase();
				actor.OutputHandler.Send($"This fire profile is now named {Name.ColourName()}.");
				return true;
			case "type":
			case "damagetype":
				if (!command.SafeRemainingArgument.TryParseEnum<DamageType>(out DamageType damageType))
				{
					actor.OutputHandler.Send("That is not a valid damage type.");
					return false;
				}

				DamageType = damageType;
				actor.OutputHandler.Send($"This fire profile now inflicts {DamageType.DescribeEnum().ColourValue()} damage.");
				return true;
			case "damage":
				return BuildingCommandAmount(actor, command, "damage", value => DamagePerTick = value);
			case "pain":
				return BuildingCommandAmount(actor, command, "pain", value => PainPerTick = value);
			case "stun":
				return BuildingCommandAmount(actor, command, "stun", value => StunPerTick = value);
			case "thermal":
			case "heat":
				return BuildingCommandAmount(actor, command, "thermal load", value => ThermalLoadPerTick = value);
			case "spread":
				if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out double spread) ||
					spread < 0.0 || spread > 1.0)
				{
					actor.OutputHandler.Send("You must enter a percentage from 0% to 100%.");
					return false;
				}

				SpreadChance = spread;
				actor.OutputHandler.Send($"This fire profile now has a {SpreadChance.ToString("P2", actor).ColourValue()} spread chance per tick.");
				return true;
			case "oxidation":
			case "minimumoxidation":
				if (!double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, actor.Account.Culture,
						out double oxidation) || oxidation < 0.0)
				{
					actor.OutputHandler.Send("You must enter a non-negative oxidation factor.");
					return false;
				}

				MinimumOxidation = oxidation;
				actor.OutputHandler.Send($"This fire profile now needs an oxidation factor of at least {MinimumOxidation.ToString("N3", actor).ColourValue()}.");
				return true;
			case "selfoxidising":
			case "selfoxidizing":
			case "selfoxidise":
			case "selfoxidize":
				SelfOxidising = !SelfOxidising;
				actor.OutputHandler.Send($"This fire profile is {SelfOxidising.NowNoLonger()} self-oxidising.");
				return true;
			case "interval":
			case "frequency":
			case "tick":
				if (!TimeSpan.TryParse(command.SafeRemainingArgument, actor, out TimeSpan interval) ||
					interval <= TimeSpan.Zero)
				{
					actor.OutputHandler.Send("You must enter a positive tick interval.");
					return false;
				}

				TickFrequency = interval;
				actor.OutputHandler.Send($"This fire profile now ticks every {TickFrequency.Describe(actor).ColourValue()}.");
				return true;
			case "extinguish":
			case "extinguishtag":
				return BuildingCommandExtinguishTag(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandAmount(ICharacter actor, StringStack command, string name, Action<double> setter)
	{
		if (!double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, actor.Account.Culture,
				out double value) || value < 0.0)
		{
			actor.OutputHandler.Send($"You must enter a non-negative {name} amount.");
			return false;
		}

		setter(value);
		actor.OutputHandler.Send($"This fire profile now applies {value.ToString("N3", actor).ColourValue()} {name} per tick.");
		return true;
	}

	private bool BuildingCommandExtinguishTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which liquid tag should toggle as an extinguishing agent?");
			return false;
		}

		ITag tag = _gameworld.Tags.GetByIdOrName(command.SafeRemainingArgument);
		if (tag is null)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return false;
		}

		if (_extinguishTags.Remove(tag))
		{
			actor.OutputHandler.Send($"Liquids tagged {tag.FullName.ColourName()} will no longer extinguish this fire.");
			return true;
		}

		_extinguishTags.Add(tag);
		actor.OutputHandler.Send($"Liquids tagged {tag.FullName.ColourName()} will now extinguish this fire.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		StringBuilder sb = new();
		sb.AppendLine($"Fire Profile: {Name.ColourName()}");
		sb.AppendLine($"Damage Type: {DamageType.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Damage/Pain/Stun: {DamagePerTick.ToString("N3", actor).ColourValue()} / {PainPerTick.ToString("N3", actor).ColourValue()} / {StunPerTick.ToString("N3", actor).ColourValue()}");
		sb.AppendLine($"Thermal Load: {ThermalLoadPerTick.ToString("N3", actor).ColourValue()}");
		sb.AppendLine($"Tick Frequency: {TickFrequency.Describe(actor).ColourValue()}");
		sb.AppendLine($"Spread Chance: {SpreadChance.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Minimum Oxidation: {MinimumOxidation.ToString("N3", actor).ColourValue()}");
		sb.AppendLine($"Self-Oxidising: {SelfOxidising.ToColouredString()}");
		sb.AppendLine($"Extinguishing Tags: {(_extinguishTags.Any() ? _extinguishTags.Select(x => x.FullName.ColourName()).ListToString() : "None".ColourError())}");
		return sb.ToString();
	}

	public const string HelpText = @"You can edit the fire profile with the following options:

	#3fire name <name>#0 - sets the profile name
	#3fire type <damage type>#0 - sets the damage type
	#3fire damage <amount>#0 - sets damage per tick
	#3fire pain <amount>#0 - sets pain per tick
	#3fire stun <amount>#0 - sets stun per tick
	#3fire thermal <amount>#0 - sets thermal load per tick
	#3fire spread <percentage>#0 - sets the per-tick spread chance
	#3fire oxidation <factor>#0 - sets the minimum atmospheric oxidation
	#3fire selfoxidising#0 - toggles self-oxidising fire
	#3fire interval <timespan>#0 - sets the tick interval
	#3fire extinguish <tag>#0 - toggles a liquid tag that extinguishes the fire";

    public XElement SaveToXml()
    {
        return new XElement("FireProfile",
            new XElement("Name", new XCData(Name)),
            new XElement("DamageType", (int)DamageType),
            new XElement("DamagePerTick", DamagePerTick),
            new XElement("PainPerTick", PainPerTick),
            new XElement("StunPerTick", StunPerTick),
            new XElement("ThermalLoadPerTick", ThermalLoadPerTick),
            new XElement("SpreadChance", SpreadChance),
            new XElement("MinimumOxidation", MinimumOxidation),
            new XElement("SelfOxidising", SelfOxidising),
            new XElement("TickFrequencySeconds", TickFrequency.TotalSeconds),
            new XElement("ExtinguishTags", _extinguishTags.Select(x => new XElement("Tag", x.Id)))
        );
    }
}
