#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.NPC;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Magic.Powers;

public sealed class DangerSensePower : PsionicSustainedSelfPowerBase
{
	public override string PowerType => "Danger Sense";
	public override string DatabaseType => "dangersense";
	protected override string DefaultBeginVerb => "dangersense";
	protected override string DefaultEndVerb => "enddangersense";
	protected override CheckType ActivationCheckType => CheckType.DangerSenseNearbyThreat;

	public uint ThreatRange { get; private set; }
	public bool RespectDoors { get; private set; }
	public bool RespectCorners { get; private set; }
	public bool IncludeCurrentLocation { get; private set; }
	public bool OnlyNPCs { get; private set; }
	public Difficulty ThreatDifficulty { get; private set; }
	public Difficulty DefenseDifficulty { get; private set; }
	public double DefenseBonus { get; private set; }
	public TimeSpan DefenseDuration { get; private set; }
	public TimeSpan ThreatWarningInterval { get; private set; }
	public string ThreatEcho { get; private set; }
	public string DefenseEcho { get; private set; }
	public double SustainTickMultiplier => 1.0 / 6.0;

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("dangersense", (power, gameworld) => new DangerSensePower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("dangersense", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new DangerSensePower(gameworld, school, name, trait));
	}

	private DangerSensePower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) :
		base(gameworld, school, name, trait)
	{
		Blurb = "Sense nearby danger and gain defensive flashes of warning";
		_showHelpText =
			$"Use {school.SchoolVerb.ToUpperInvariant()} DANGERSENSE to sustain danger sense and {school.SchoolVerb.ToUpperInvariant()} ENDDANGERSENSE to end it.";
		ThreatRange = 1;
		RespectDoors = true;
		RespectCorners = false;
		IncludeCurrentLocation = true;
		OnlyNPCs = true;
		ThreatDifficulty = Difficulty.Normal;
		DefenseDifficulty = Difficulty.Normal;
		DefenseBonus = Gameworld.GetStaticDouble("CheckBonusPerDifficultyLevel");
		DefenseDuration = TimeSpan.FromSeconds(15);
		ThreatWarningInterval = TimeSpan.FromSeconds(30);
		ConcentrationPointsToSustain = 0.0;
		SustainPenalty = 0.0;
		BeginEmote = "Your awareness spreads into a restless sense of danger.";
		EndEmote = "Your sense of danger folds back into ordinary awareness.";
		FailEmote = "You reach for danger, but your awareness stays dull.";
		ThreatEcho = "A prickling warning crawls across your thoughts: danger is nearby.";
		DefenseEcho = "A flash of warning sharpens your reactions.";
		DoDatabaseInsert();
	}

	private DangerSensePower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		ThreatRange = uint.Parse(root.Element("ThreatRange")?.Value ?? "1");
		RespectDoors = bool.Parse(root.Element("RespectDoors")?.Value ?? "true");
		RespectCorners = bool.Parse(root.Element("RespectCorners")?.Value ?? "false");
		IncludeCurrentLocation = bool.Parse(root.Element("IncludeCurrentLocation")?.Value ?? "true");
		OnlyNPCs = bool.Parse(root.Element("OnlyNPCs")?.Value ?? "true");
		ThreatDifficulty = (Difficulty)int.Parse(root.Element("ThreatDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		DefenseDifficulty = (Difficulty)int.Parse(root.Element("DefenseDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		DefenseBonus = double.Parse(root.Element("DefenseBonus")?.Value ??
		                            Gameworld.GetStaticDouble("CheckBonusPerDifficultyLevel").ToString());
		DefenseDuration = TimeSpan.FromSeconds(double.Parse(root.Element("DefenseDurationSeconds")?.Value ?? "15"));
		ThreatWarningInterval = TimeSpan.FromSeconds(double.Parse(root.Element("ThreatWarningIntervalSeconds")?.Value ?? "30"));
		ThreatEcho = root.Element("ThreatEcho")?.Value ??
		             "A prickling warning crawls across your thoughts: danger is nearby.";
		DefenseEcho = root.Element("DefenseEcho")?.Value ?? "A flash of warning sharpens your reactions.";
	}

	protected override XElement SaveDefinition()
	{
		return SaveSustainedSelfDefinition(
			new XElement("ThreatRange", ThreatRange),
			new XElement("RespectDoors", RespectDoors),
			new XElement("RespectCorners", RespectCorners),
			new XElement("IncludeCurrentLocation", IncludeCurrentLocation),
			new XElement("OnlyNPCs", OnlyNPCs),
			new XElement("ThreatDifficulty", (int)ThreatDifficulty),
			new XElement("DefenseDifficulty", (int)DefenseDifficulty),
			new XElement("DefenseBonus", DefenseBonus),
			new XElement("DefenseDurationSeconds", DefenseDuration.TotalSeconds),
			new XElement("ThreatWarningIntervalSeconds", ThreatWarningInterval.TotalSeconds),
			new XElement("ThreatEcho", new XCData(ThreatEcho)),
			new XElement("DefenseEcho", new XCData(DefenseEcho))
		);
	}

	protected override IEffect CreateEffect(ICharacter actor)
	{
		return new MagicDangerSenseEffect(actor, this);
	}

	protected override IEnumerable<IEffect> ActiveEffects(ICharacter actor)
	{
		return actor.EffectsOfType<MagicDangerSenseEffect>().Where(x => x.Power == this);
	}

	public bool CheckNearbyThreats(ICharacter actor)
	{
		var cells = actor.Location.CellsInVicinity(ThreatRange, RespectDoors, RespectCorners);
		if (!IncludeCurrentLocation)
		{
			cells = cells.Except(actor.Location);
		}

		var threats = cells
		              .SelectMany(x => x.Characters)
		              .Where(x => x != actor)
		              .Where(x => !OnlyNPCs || x is INPC)
		              .Where(x => !actor.IsAlly(x))
		              .Where(x => x.CombatTarget == actor || actor.CombatTarget == x || x.Combat is not null ||
		                          x.CanEngage(actor))
		              .Distinct()
		              .ToList();
		if (!threats.Any())
		{
			return false;
		}

		var outcome = Gameworld.GetCheck(CheckType.DangerSenseNearbyThreat)
		                       .Check(actor, ThreatDifficulty, SkillCheckTrait, threats.First());
		if (outcome < MinimumSuccessThreshold)
		{
			return false;
		}

		actor.OutputHandler.Send(new EmoteOutput(new Emote(ThreatEcho, actor, actor)));
		return true;
	}

	public void RefreshDefensiveEdge(ICharacter actor)
	{
		if (actor.Combat is null)
		{
			return;
		}

		var outcome = Gameworld.GetCheck(CheckType.DangerSenseDefense)
		                       .Check(actor, DefenseDifficulty, SkillCheckTrait, actor.CombatTarget as ICharacter);
		if (outcome < MinimumSuccessThreshold)
		{
			return;
		}

		var existing = actor.EffectsOfType<DangerSenseDefensiveEdge>().FirstOrDefault(x => x.Power == this);
		if (existing is null)
		{
			actor.AddEffect(new DangerSenseDefensiveEdge(actor, this, DefenseBonus), DefenseDuration);
		}
		else
		{
			actor.Reschedule(existing, DefenseDuration);
		}

		if (!string.IsNullOrWhiteSpace(DefenseEcho))
		{
			actor.OutputHandler.Send(new EmoteOutput(new Emote(DefenseEcho, actor, actor)));
		}
	}

	protected override void ShowSubtypeDetails(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Threat Range: {ThreatRange.ToString("N0", actor).ColourValue()} rooms");
		sb.AppendLine($"Respect Doors: {RespectDoors.ToColouredString()}");
		sb.AppendLine($"Respect Corners: {RespectCorners.ToColouredString()}");
		sb.AppendLine($"Include Current Location: {IncludeCurrentLocation.ToColouredString()}");
		sb.AppendLine($"Only NPCs: {OnlyNPCs.ToColouredString()}");
		sb.AppendLine($"Threat Difficulty: {ThreatDifficulty.DescribeColoured()}");
		sb.AppendLine($"Defense Difficulty: {DefenseDifficulty.DescribeColoured()}");
		sb.AppendLine($"Defense Bonus: {DefenseBonus.ToBonusString(actor)}");
		sb.AppendLine($"Defense Duration: {DefenseDuration.Describe(actor).ColourValue()}");
		sb.AppendLine($"Warning Interval: {ThreatWarningInterval.Describe(actor).ColourValue()}");
		sb.AppendLine($"Threat Echo: {ThreatEcho.ColourCommand()}");
		sb.AppendLine($"Defense Echo: {DefenseEcho.ColourCommand()}");
	}

	protected override string SubtypeHelpText => $@"{base.SubtypeHelpText}
	#3range <rooms>#0 - sets nearby threat scan range
	#3doors#0 - toggles whether closed doors block threat scans
	#3corners#0 - toggles corner-aware threat scans
	#3current#0 - toggles including the current location
	#3npcs#0 - toggles whether only NPCs count as danger
	#3threatdifficulty <difficulty>#0 - sets nearby threat check difficulty
	#3defensedifficulty <difficulty>#0 - sets defense check difficulty
	#3defensebonus <number>#0 - sets defensive check bonus
	#3defenseduration <seconds>#0 - sets how long the defensive edge lasts
	#3warninginterval <seconds>#0 - sets warning spam protection interval
	#3threatecho <emote>#0 - sets nearby danger warning echo
	#3defenseecho <emote|none>#0 - sets defensive edge echo";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "range":
				return BuildingCommandRange(actor, command);
			case "doors":
				RespectDoors = !RespectDoors;
				Changed = true;
				actor.OutputHandler.Send($"Danger sense will {RespectDoors.NowNoLonger()} respect closed doors.");
				return true;
			case "corners":
				RespectCorners = !RespectCorners;
				Changed = true;
				actor.OutputHandler.Send($"Danger sense will {RespectCorners.NowNoLonger()} respect corners.");
				return true;
			case "current":
				IncludeCurrentLocation = !IncludeCurrentLocation;
				Changed = true;
				actor.OutputHandler.Send($"Danger sense will {IncludeCurrentLocation.NowNoLonger()} include the current location.");
				return true;
			case "npcs":
				OnlyNPCs = !OnlyNPCs;
				Changed = true;
				actor.OutputHandler.Send($"Danger sense will {OnlyNPCs.NowNoLonger()} only report NPCs.");
				return true;
			case "threatdifficulty":
			case "threatdiff":
				return BuildingCommandDifficulty(actor, command, true);
			case "defensedifficulty":
			case "defensediff":
				return BuildingCommandDifficulty(actor, command, false);
			case "defensebonus":
			case "bonus":
				return BuildingCommandDefenseBonus(actor, command);
			case "defenseduration":
				return BuildingCommandDuration(actor, command, true);
			case "warninginterval":
			case "interval":
				return BuildingCommandDuration(actor, command, false);
			case "threatecho":
				return BuildingCommandEcho(actor, command, true);
			case "defenseecho":
				return BuildingCommandEcho(actor, command, false);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandRange(ICharacter actor, StringStack command)
	{
		if (!uint.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("You must enter a whole number of rooms.");
			return false;
		}

		ThreatRange = value;
		Changed = true;
		actor.OutputHandler.Send($"Danger sense now scans {ThreatRange.ToString("N0", actor).ColourValue()} rooms away.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command, bool threat)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty value))
		{
			actor.OutputHandler.Send($"Valid difficulties are {Enum.GetValues<Difficulty>().Select(x => x.DescribeColoured()).ListToString()}.");
			return false;
		}

		if (threat)
		{
			ThreatDifficulty = value;
		}
		else
		{
			DefenseDifficulty = value;
		}

		Changed = true;
		actor.OutputHandler.Send($"{(threat ? "Threat" : "Defense")} checks now use {value.DescribeColoured()} difficulty.");
		return true;
	}

	private bool BuildingCommandDefenseBonus(ICharacter actor, StringStack command)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must enter a positive defense bonus.");
			return false;
		}

		DefenseBonus = value;
		Changed = true;
		actor.OutputHandler.Send($"Danger sense now grants {DefenseBonus.ToBonusString(actor)} to defensive combat checks.");
		return true;
	}

	private bool BuildingCommandDuration(ICharacter actor, StringStack command, bool defense)
	{
		if (!double.TryParse(command.SafeRemainingArgument, out var seconds) || seconds <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a positive number of seconds.");
			return false;
		}

		if (defense)
		{
			DefenseDuration = TimeSpan.FromSeconds(seconds);
		}
		else
		{
			ThreatWarningInterval = TimeSpan.FromSeconds(seconds);
		}

		Changed = true;
		actor.OutputHandler.Send($"{(defense ? "Defense edge" : "Threat warning")} duration is now {TimeSpan.FromSeconds(seconds).Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEcho(ICharacter actor, StringStack command, bool threat)
	{
		if (!threat && (command.IsFinished || command.SafeRemainingArgument.EqualToAny("none", "clear", "delete")))
		{
			DefenseEcho = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("Danger sense no longer echoes when it refreshes defensive awareness.");
			return true;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote should be used?");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		if (threat)
		{
			ThreatEcho = command.SafeRemainingArgument;
		}
		else
		{
			DefenseEcho = command.SafeRemainingArgument;
		}

		Changed = true;
		actor.OutputHandler.Send($"The {(threat ? "threat" : "defense")} echo is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}
}
