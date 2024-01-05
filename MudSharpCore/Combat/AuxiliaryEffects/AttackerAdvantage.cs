using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using OpenAI_API.Moderation;

namespace MudSharp.Combat.AuxiliaryEffects;
#nullable enable
internal class AttackerAdvantage : IAuxiliaryEffect
{
	public required IFuturemud Gameworld { get; set; }
	public required double DefenseBonusPerDegree { get; set; }
	public required double OffenseBonusPerDegree { get; set; }
	public required ITraitDefinition DefenseTrait { get; set; }
	public required Difficulty DefenseDifficulty { get; set; }
	public required bool AllowNegatives { get; set; }
	public required bool AllowPositives { get; set; }

	[SetsRequiredMembers]
	public AttackerAdvantage(XElement root, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		DefenseBonusPerDegree = double.Parse(root.Attribute("defensebonusperdegree")!.Value);
		OffenseBonusPerDegree = double.Parse(root.Attribute("offensebonusperdegree")!.Value);
		AllowNegatives = bool.Parse(root.Attribute("allownegatives")!.Value);
		AllowPositives = bool.Parse(root.Attribute("allowpositives")!.Value);
		DefenseTrait = gameworld.Traits.Get(long.Parse(root.Attribute("defensetrait")!.Value)) ?? throw new ApplicationException($"Missing DefenseTrait for AttackerAdvantage: {root.Attribute("defensetrait")!.Value}");
		DefenseDifficulty = (Difficulty)(int.Parse(root.Attribute("defensedifficulty")!.Value));
	}

	public XElement Save()
	{
		return new XElement("Effect",
			new XAttribute("type", "attackeradvantage"),
			new XAttribute("defensebonusperdegree", DefenseBonusPerDegree),
			new XAttribute("offensebonusperdegree", OffenseBonusPerDegree),
			new XAttribute("defensetrait", DefenseTrait.Id),
			new XAttribute("defensedifficulty", (int)DefenseDifficulty),
			new XAttribute("allownegatives", AllowNegatives),
			new XAttribute("allowpositives", AllowPositives)
		);
	}

	public string DescribeForShow(ICharacter actor)
	{
		return $"Attacker Advantage | vs {DefenseTrait.Name.ColourValue()}@{DefenseDifficulty.DescribeColoured()} | Off: [{(AllowNegatives ? OffenseBonusPerDegree.InvertSign() : 0.0).ToBonusString(actor)}/{(AllowPositives ? OffenseBonusPerDegree : 0.0).ToBonusString(actor)}] Def: [{(AllowNegatives ? DefenseBonusPerDegree.InvertSign() : 0.0).ToBonusString(actor)}/{(AllowPositives ? DefenseBonusPerDegree : 0.0).ToBonusString(actor)}]";
	}

	/// <inheritdoc />
	public static void RegisterTypeHelp()
	{
		AuxiliaryCombatAction.RegisterBuilderParser("attackeradvantage", (_, actor, ss) =>
		{
			var gameworld = actor.Gameworld;
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send($"Which trait should be used to defend against this effect?");
				return null;
			}

			var trait = gameworld.Traits.GetByIdOrName(ss.PopSpeech());
			if (trait is null)
			{
				actor.OutputHandler.Send("There is no such trait.");
				return null;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("How difficult should the defense against this effect be?");
				return null;
			}

			if (!ss.PopSpeech().TryParseEnum<Difficulty>(out var difficulty))
			{
				actor.OutputHandler.Send($"That is not a valid difficulty. The valid difficulties are {Enum.GetValues<Difficulty>().Select(x => x.DescribeColoured()).ListToString()}.");
				return null;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send($"What amount of offensive bonus do you want to apply to the attacker per degree of failure on defense? Each {gameworld.GetStaticDouble("CheckBonusPerDifficultyLevel").ToString("N2", actor).ColourValue()} bonus represents one degree of difficulty.");
				return null;
			}

			if (!double.TryParse(ss.PopSpeech(), out var offense))
			{
				actor.OutputHandler.Send($"The value {ss.Last.ColourCommand()} is not a valid number.");
				return null;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send($"What amount of defense bonus do you want to apply to the attacker per degree of failure on defense? Each {gameworld.GetStaticDouble("CheckBonusPerDifficultyLevel").ToString("N2", actor).ColourValue()} bonus represents one degree of difficulty.");
				return null;
			}

			if (!double.TryParse(ss.PopSpeech(), out var defense))
			{
				actor.OutputHandler.Send($"The value {ss.Last.ColourCommand()} is not a valid number.");
				return null;
			}

			return new AttackerAdvantage(
				new XElement("Effect",
					new XAttribute("type", "attackeradvantage"),
					new XAttribute("defensebonusperdegree", defense.Abs()),
					new XAttribute("offensebonusperdegree", offense.Abs()),
					new XAttribute("defensetrait", trait.Id),
					new XAttribute("defensedifficulty", (int)difficulty),
					new XAttribute("allownegatives", true),
					new XAttribute("allowpositives", true)
				), 
				gameworld);
		}, 
			$@"The Attacker Advantage effect type applies a bonus or penalty to the person using the move based on how successful an opposed check is.

The syntax to create this type is as follows:

	#3auxiliary set add attackeradvantage <defense trait> <difficulty> <offense bonus> <defense bonus>#0

Use negative values for offense/defense bonus to indicate penalties.

{BuildingHelpText}", 
			true);
	}

	private const string BuildingHelpText = @"You can use the following syntax to edit this effect:

	#3trait <which>#0 - sets the trait used to defend against this move
	#3difficulty <diff>#0 - sets the difficulty of defending
	#3allowpositive#0 - toggles allowing positive values of bonus on success
	#3allownegative#0 - toggles allowing negative values of bonus on failure
	#3defense <bonus>#0 - sets the bonus per degree for defense
	#3offense <bonus>#0 - sets the bonus per degree for offense";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "bonusdefense":
			case "bonusdefence":
			case "bonusdef":
			case "defbonus":
			case "defensebonus":
			case "defensivebonus":
			case "defencebonus":
			case "def":
			case "defense":
			case "defence":
				return BuildingCommandBonusDefense(actor, command);
			case "bonusoffense":
			case "bonusoffence":
			case "bonusoff":
			case "offbonus":
			case "offensebonus":
			case "offencebonus":
			case "offensivebonus":
			case "off":
			case "offense":
			case "offence":
				return BuildingCommandBonusOffense(actor, command);
			case "trait":
			case "skill":
				return BuildingCommandTrait(actor, command);
			case "difficulty":
			case "diff":
				return BuildingCommandDifficulty(actor, command);
			case "allownegatives":
			case "allownegative":
			case "negative":
			case "allowneg":
				return BuildingCommandAllowNegatives(actor);
			case "allowpositives":
			case "allowpositive":
			case "allowpos":
			case "positive":
				return BuildingCommandAllowPositives(actor);
			default:
				actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandAllowPositives(ICharacter actor)
	{
		AllowPositives = !AllowPositives;
		actor.OutputHandler.Send($"This effect will {AllowPositives.NowNoLonger()} apply positive modifiers when the attacker is successful.");
		return true;
	}

	private bool BuildingCommandAllowNegatives(ICharacter actor)
	{
		AllowNegatives = !AllowNegatives;
		actor.OutputHandler.Send($"This effect will {AllowNegatives.NowNoLonger()} apply negative modifiers when the attacker is unsuccessful.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What difficulty should the defense test against this effect be? The valid values are {Enum.GetValues<Difficulty>().Select(x => x.DescribeColoured()).ListToString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var difficulty))
		{
			actor.OutputHandler.Send($"That is not a valid difficulty. The valid values are {Enum.GetValues<Difficulty>().Select(x => x.DescribeColoured()).ListToString()}.");
			return false;
		}

		DefenseDifficulty = difficulty;
		actor.OutputHandler.Send($"The defense test against this effect will now be at {difficulty.DescribeColoured()} difficulty.");
		return true;
	}

	private bool BuildingCommandTrait(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait should be used for the defense test against this effect?");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (trait is null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		DefenseTrait = trait;
		actor.OutputHandler.Send($"The defense test against this effect will now use the {trait.Name.ColourValue()} trait.");
		return true;
	}

	private bool BuildingCommandBonusOffense(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much offensive advantage should be applied per degree or success on the attacker's check?");
			return false;
		}

		if (double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("That is not a valid number.");
			return false;
		}

		OffenseBonusPerDegree = value;
		var sb = new StringBuilder();
		sb.AppendLine($"The offensive advantage per degree of success applied will now be {value.ToBonusString(actor)}.\n\nFor a normal difficulty check, this leads to the following bonuses:\n\n");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Total.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? OffenseBonusPerDegree * -5.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Major.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? OffenseBonusPerDegree * -4.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Moderate.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? OffenseBonusPerDegree * -3.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Minor.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? OffenseBonusPerDegree * -2.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Marginal.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? OffenseBonusPerDegree * -1.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{"Tie".Colour(Telnet.Yellow)}: {Difficulty.Normal.DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Marginal.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? OffenseBonusPerDegree * 1.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Minor.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? OffenseBonusPerDegree * 2.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Moderate.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? OffenseBonusPerDegree * 3.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Major.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? OffenseBonusPerDegree * 4.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Total.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? OffenseBonusPerDegree * 5.0 : 0.0).DescribeColoured()}");
		actor.OutputHandler.Send(sb.ToString());
		return true;
	}

	private bool BuildingCommandBonusDefense(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much defensive advantage should be applied per degree or success on the attacker's check?");
			return false;
		}

		if (double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("That is not a valid number.");
			return false;
		}

		DefenseBonusPerDegree = value;
		var sb = new StringBuilder();
		sb.AppendLine($"The defensive advantage per degree of success applied will now be {value.ToBonusString(actor)}.\n\nFor a normal difficulty check, this leads to the following bonuses:\n\n");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Total.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? DefenseBonusPerDegree * -5.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Major.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? DefenseBonusPerDegree * -4.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Moderate.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? DefenseBonusPerDegree * -3.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Minor.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? DefenseBonusPerDegree * -2.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Marginal.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? DefenseBonusPerDegree * -1.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{"Tie".Colour(Telnet.Yellow)}: {Difficulty.Normal.DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Marginal.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? DefenseBonusPerDegree * 1.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Minor.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? DefenseBonusPerDegree * 2.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Moderate.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? DefenseBonusPerDegree * 3.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Major.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? DefenseBonusPerDegree * 4.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Total.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? DefenseBonusPerDegree * 5.0 : 0.0).DescribeColoured()}");
		actor.OutputHandler.Send(sb.ToString());
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Attacker Advantage Effect".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Allow Positives: {AllowPositives.ToColouredString()}");
		sb.AppendLine($"Allow Negatives: {AllowNegatives.ToColouredString()}");
		sb.AppendLine($"Defense Trait: {DefenseTrait.Name.ColourValue()} @ {DefenseDifficulty.DescribeColoured()}");
		sb.AppendLine($"Offense Bonus Per Degree: {OffenseBonusPerDegree.ToBonusString(actor)}");
		sb.AppendLine($"Defense Bonus Per Degree: {DefenseBonusPerDegree.ToBonusString(actor)}");
		sb.AppendLine();
		sb.AppendLine("Offense Results:");
		sb.AppendLine();
		sb.AppendLine($"\t{OpposedOutcomeDegree.Total.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? OffenseBonusPerDegree * -5.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Major.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? OffenseBonusPerDegree * -4.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Moderate.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? OffenseBonusPerDegree * -3.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Minor.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? OffenseBonusPerDegree * -2.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Marginal.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? OffenseBonusPerDegree * -1.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{"Tie".Colour(Telnet.Yellow)}: {Difficulty.Normal.DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Marginal.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? OffenseBonusPerDegree * 1.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Minor.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? OffenseBonusPerDegree * 2.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Moderate.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? OffenseBonusPerDegree * 3.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Major.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? OffenseBonusPerDegree * 4.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Total.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? OffenseBonusPerDegree * 5.0 : 0.0).DescribeColoured()}");
		sb.AppendLine();
		sb.AppendLine("Defense Results:");
		sb.AppendLine();
		sb.AppendLine($"\t{OpposedOutcomeDegree.Total.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? DefenseBonusPerDegree * -5.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Major.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? DefenseBonusPerDegree * -4.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Moderate.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? DefenseBonusPerDegree * -3.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Minor.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? DefenseBonusPerDegree * -2.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Marginal.DescribeColour()} {"Opponent".Colour(Telnet.Red)}: {Difficulty.Normal.ApplyBonus(AllowNegatives ? DefenseBonusPerDegree * -1.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{"Tie".Colour(Telnet.Yellow)}: {Difficulty.Normal.DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Marginal.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? DefenseBonusPerDegree * 1.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Minor.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? DefenseBonusPerDegree * 2.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Moderate.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? DefenseBonusPerDegree * 3.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Major.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? DefenseBonusPerDegree * 4.0 : 0.0).DescribeColoured()}");
		sb.AppendLine($"\t{OpposedOutcomeDegree.Total.DescribeColour()} {"Attacker".Colour(Telnet.Green)}: {Difficulty.Normal.ApplyBonus(AllowPositives ? DefenseBonusPerDegree * 5.0 : 0.0).DescribeColoured()}");
		return sb.ToString();
	}

	public void ApplyEffect(ICharacter attacker, IPerceiver? target, CheckOutcome outcome)
	{
		if (target is not ICharacter tch)
		{
			return;
		}

		var defenseCheck = Gameworld.GetCheck(CheckType.CombatMoveCheck);
		var defenderOutcome = defenseCheck.Check(tch, DefenseDifficulty, DefenseTrait, attacker);
		var opposed = new OpposedOutcome(outcome, defenderOutcome);
		switch (opposed.Outcome)
		{
			case OpposedOutcomeDirection.Proponent:
				if (!AllowPositives)
				{
					return;
				}
				attacker.OffensiveAdvantage += OffenseBonusPerDegree * (int)opposed.Degree;
				attacker.DefensiveAdvantage += DefenseBonusPerDegree * (int)opposed.Degree;
				break;
			case OpposedOutcomeDirection.Opponent:
				if (!AllowNegatives)
				{
					return;
				}
				attacker.OffensiveAdvantage += OffenseBonusPerDegree * -1.0 * (int)opposed.Degree;
				attacker.DefensiveAdvantage += DefenseBonusPerDegree * -1.0 * (int)opposed.Degree;
				break;
			case OpposedOutcomeDirection.Stalemate:
				return;
		}
	}
}
