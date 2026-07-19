using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.Health;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class IncenseBurnerGameItemComponentProto : GameItemComponentProto, IIncenseBurnerPrototype
{
	protected IncenseBurnerGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "IncenseBurner")
	{
		MaximumFuelWeight = 1000.0;
		SecondsPerUnitWeight = 60.0;
		ScentRange = 1;
		DrugRange = 0;
		DrugPulseSeconds = 10;
		LingeringMultiplier = 5.0;
		SourceScentDescription = "A curl of fragrant smoke hangs in the air.";
		DistantScentDescription = "A faint trace of fragrant smoke drifts in from nearby.";
		ScentDifficulty = Difficulty.Normal;
		Changed = true;
	}

	protected IncenseBurnerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "IncenseBurner";
	public ITag? FuelTag { get; protected set; }
	public double MaximumFuelWeight { get; protected set; }
	public double SecondsPerUnitWeight { get; protected set; }
	public int ScentRange { get; protected set; }
	public int DrugRange { get; protected set; }
	public int DrugPulseSeconds { get; protected set; }
	public double LingeringMultiplier { get; protected set; }
	public string SourceScentDescription { get; protected set; } = string.Empty;
	public string DistantScentDescription { get; protected set; } = string.Empty;
	public Difficulty ScentDifficulty { get; protected set; }
	public IDrug? Drug { get; protected set; }
	public double GramsPerPulse { get; protected set; }

	protected override void LoadFromXml(XElement root)
	{
		FuelTag = Gameworld.Tags.Get(long.Parse(root.Element("FuelTag")?.Value ?? "0"));
		MaximumFuelWeight = double.Parse(root.Element("MaximumFuelWeight")?.Value ?? "1000.0");
		SecondsPerUnitWeight = double.Parse(root.Element("SecondsPerUnitWeight")?.Value ?? "60.0");
		ScentRange = int.Parse(root.Element("ScentRange")?.Value ?? "1");
		DrugRange = int.Parse(root.Element("DrugRange")?.Value ?? "0");
		DrugPulseSeconds = int.Parse(root.Element("DrugPulseSeconds")?.Value ?? "10");
		LingeringMultiplier = double.Parse(root.Element("LingeringMultiplier")?.Value ?? "5.0");
		SourceScentDescription = root.Element("SourceScentDescription")?.Value ?? string.Empty;
		DistantScentDescription = root.Element("DistantScentDescription")?.Value ?? string.Empty;
		ScentDifficulty = (Difficulty)int.Parse(root.Element("ScentDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		Drug = Gameworld.Drugs.Get(long.Parse(root.Element("Drug")?.Value ?? "0"));
		GramsPerPulse = double.Parse(root.Element("GramsPerPulse")?.Value ?? "0.0");
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("FuelTag", FuelTag?.Id ?? 0),
			new XElement("MaximumFuelWeight", MaximumFuelWeight),
			new XElement("SecondsPerUnitWeight", SecondsPerUnitWeight),
			new XElement("ScentRange", ScentRange),
			new XElement("DrugRange", DrugRange),
			new XElement("DrugPulseSeconds", DrugPulseSeconds),
			new XElement("LingeringMultiplier", LingeringMultiplier),
			new XElement("SourceScentDescription", new XCData(SourceScentDescription)),
			new XElement("DistantScentDescription", new XCData(DistantScentDescription)),
			new XElement("ScentDifficulty", (int)ScentDifficulty),
			new XElement("Drug", Drug?.Id ?? 0),
			new XElement("GramsPerPulse", GramsPerPulse)).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new IncenseBurnerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new IncenseBurnerGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new IncenseBurnerGameItemComponentProto(proto, gameworld));
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("incenseburner", true,
			(gameworld, account) => new IncenseBurnerGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("incense burner", false,
			(gameworld, account) => new IncenseBurnerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("IncenseBurner",
			(proto, gameworld) => new IncenseBurnerGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"IncenseBurner",
			$"A lightable container that burns tagged fuel into ambient scent",
			BuildingHelpText);
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\t#3name <name>#0 - sets the name of the component\n\t#3desc <desc>#0 - sets the description of the component\n\t#3tag <tag>|clear#0 - sets the required fuel tag\n\t#3capacity <weight>#0 - sets maximum fuel weight\n\t#3rate <seconds>#0 - sets burn seconds per unit weight\n\t#3range <rooms>#0 - sets scent spread range\n\t#3linger <multiplier>#0 - sets scent lingering multiplier\n\t#3source <text>|clear#0 - sets scent text in the source cell\n\t#3distant <text>|clear#0 - sets scent text in other cells\n\t#3difficulty <difficulty>#0 - sets scent tracking difficulty\n\t#3drug <drug>|clear#0 - sets optional inhaled drug\n\t#3dose <weight>#0 - sets drug dose per pulse\n\t#3pulse <seconds>#0 - sets drug pulse interval\n\t#3drugrange <rooms>#0 - sets drug dosing range";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "tag":
			case "fueltag":
				return BuildingCommandFuelTag(actor, command);
			case "capacity":
			case "maxfuel":
				return BuildingCommandCapacity(actor, command);
			case "rate":
			case "secondsperweight":
				return BuildingCommandRate(actor, command);
			case "range":
				return BuildingCommandRange(actor, command);
			case "drugrange":
				return BuildingCommandDrugRange(actor, command);
			case "pulse":
				return BuildingCommandPulse(actor, command);
			case "linger":
				return BuildingCommandLinger(actor, command);
			case "source":
			case "sourcedesc":
				return BuildingCommandSource(actor, command);
			case "distant":
			case "distantdesc":
				return BuildingCommandDistant(actor, command);
			case "difficulty":
				return BuildingCommandDifficulty(actor, command);
			case "drug":
				return BuildingCommandDrug(actor, command);
			case "dose":
			case "grams":
				return BuildingCommandDose(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Incense Burner Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})\n\nFuel Tag: {FuelTag?.Name.ColourName() ?? "Any".ColourValue()}\nCapacity: {MaximumFuelWeight.ToString("N2", actor).ColourValue()} weight units\nBurn Rate: {SecondsPerUnitWeight.ToString("N2", actor).ColourValue()} seconds per unit weight\nScent Range: {ScentRange.ToString("N0", actor).ColourValue()} rooms\nScent Difficulty: {ScentDifficulty.DescribeColoured()}\nSource Scent: {SourceScentDescription.ColourCommand()}\nDistant Scent: {DistantScentDescription.ColourCommand()}\nDrug: {Drug?.Name.ColourName() ?? "None".ColourError()}\nDose: {GramsPerPulse.ToString("N4", actor).ColourValue()} grams every {DrugPulseSeconds.ToString("N0", actor).ColourValue()} seconds to {DrugRange.ToString("N0", actor).ColourValue()} rooms";
	}

	private bool BuildingCommandFuelTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which item tag should count as incense fuel? Use {"clear".ColourCommand()} to accept any item.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			FuelTag = null;
			Changed = true;
			actor.OutputHandler.Send("This incense burner will now accept any item as fuel.");
			return true;
		}

		var tag = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Tags.Get(value)
			: Gameworld.Tags.GetByName(command.SafeRemainingArgument);
		if (tag is null)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return false;
		}

		FuelTag = tag;
		Changed = true;
		actor.OutputHandler.Send($"This incense burner now accepts fuel tagged {tag.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What maximum fuel weight should it hold?");
			return false;
		}

		var value = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, out var success);
		if (!success || value <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a positive weight.");
			return false;
		}

		MaximumFuelWeight = value;
		Changed = true;
		actor.OutputHandler.Send($"This incense burner now holds {Gameworld.UnitManager.DescribeExact(MaximumFuelWeight, UnitType.Mass, actor).ColourValue()} of fuel.");
		return true;
	}

	private bool BuildingCommandRate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var value) || value <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a positive number of seconds per unit weight.");
			return false;
		}

		SecondsPerUnitWeight = value;
		Changed = true;
		actor.OutputHandler.Send($"Fuel will now burn for {SecondsPerUnitWeight.ToString("N2", actor).ColourValue()} seconds per unit weight.");
		return true;
	}

	private bool BuildingCommandRange(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value < 0)
		{
			actor.OutputHandler.Send("You must enter a non-negative number of rooms.");
			return false;
		}

		ScentRange = value;
		Changed = true;
		actor.OutputHandler.Send($"Scent will now spread {ScentRange.ToString("N0", actor).ColourValue()} rooms.");
		return true;
	}

	private bool BuildingCommandDrugRange(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value < 0)
		{
			actor.OutputHandler.Send("You must enter a non-negative number of rooms.");
			return false;
		}

		DrugRange = value;
		Changed = true;
		actor.OutputHandler.Send($"Drug dosing will now reach {DrugRange.ToString("N0", actor).ColourValue()} rooms.");
		return true;
	}

	private bool BuildingCommandPulse(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a positive number of seconds.");
			return false;
		}

		DrugPulseSeconds = value;
		Changed = true;
		actor.OutputHandler.Send($"Drug dosing will now pulse every {DrugPulseSeconds.ToString("N0", actor).ColourValue()} seconds.");
		return true;
	}

	private bool BuildingCommandLinger(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var value) || value <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a positive lingering multiplier.");
			return false;
		}

		LingeringMultiplier = value;
		Changed = true;
		actor.OutputHandler.Send($"Scent effects will now linger {LingeringMultiplier.ToString("N2", actor).ColourValue()} times the refreshed burn seconds.");
		return true;
	}

	private bool BuildingCommandSource(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What text should appear in the source cell? Use {"clear".ColourCommand()} to clear it.");
			return false;
		}

		SourceScentDescription = command.SafeRemainingArgument.EqualTo("clear")
			? string.Empty
			: command.SafeRemainingArgument.ProperSentences().Trim().SubstituteANSIColour();
		Changed = true;
		actor.OutputHandler.Send($"The source scent text is now: {SourceScentDescription.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandDistant(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What text should appear in distant cells? Use {"clear".ColourCommand()} to clear it.");
			return false;
		}

		DistantScentDescription = command.SafeRemainingArgument.EqualTo("clear")
			? string.Empty
			: command.SafeRemainingArgument.ProperSentences().Trim().SubstituteANSIColour();
		Changed = true;
		actor.OutputHandler.Send($"The distant scent text is now: {DistantScentDescription.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var value))
		{
			actor.OutputHandler.Send("You must enter a valid difficulty.");
			return false;
		}

		ScentDifficulty = value;
		Changed = true;
		actor.OutputHandler.Send($"Scent tracking difficulty is now {ScentDifficulty.DescribeColoured()}.");
		return true;
	}

	private bool BuildingCommandDrug(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which inhaled drug should this incense deliver? Use {"clear".ColourCommand()} to clear it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			Drug = null;
			Changed = true;
			actor.OutputHandler.Send("This incense burner no longer delivers any drug.");
			return true;
		}

		var drug = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Drugs.Get(value)
			: Gameworld.Drugs.GetByName(command.SafeRemainingArgument);
		if (drug is null)
		{
			actor.OutputHandler.Send("There is no such drug.");
			return false;
		}

		if (!drug.DrugVectors.HasFlag(DrugVector.Inhaled))
		{
			actor.OutputHandler.Send($"The drug {drug.Name.ColourName()} cannot be used because it does not support the inhaled vector.");
			return false;
		}

		Drug = drug;
		Changed = true;
		actor.OutputHandler.Send($"This incense burner now delivers {drug.Name.ColourName()} while burning.");
		return true;
	}

	private bool BuildingCommandDose(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much drug should each pulse deliver?");
			return false;
		}

		var value = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, out var success) *
		            Gameworld.UnitManager.BaseWeightToKilograms * 1000.0;
		if (!success || value <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a positive drug weight.");
			return false;
		}

		GramsPerPulse = value;
		Changed = true;
		actor.OutputHandler.Send($"Each drug pulse will now deliver {GramsPerPulse.ToString("N4", actor).ColourValue()} grams.");
		return true;
	}
}
