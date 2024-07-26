using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class SkillLearningMerit : CharacterMeritBase, ITraitLearningMerit
{
	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Skill Learning",
			(merit, gameworld) => new SkillLearningMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Skill Learning", (gameworld, name) => new SkillLearningMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Skill Learning", "Alters the rate that skills improve or branch at", new SkillLearningMerit().HelpText);
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add("Groups",
			from item in Groups
			select new XElement("Group", item)
		);
		root.Add(new XAttribute("branching", BranchingModifier));
		root.Add(new XAttribute("improving", LearningModifier));
		root.Add(new XAttribute("min_difficulty", (int)MinimumDifficultyForModifier));
		root.Add(new XAttribute("max_difficulty", (int)MaximumDifficultyForModifier));
		return root;
	}

	protected SkillLearningMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		var element = definition.Element("Groups");
		if (element != null)
		{
			foreach (var item in element.Elements("Group"))
			{
				Groups.Add(item.Value);
			}
		}

		BranchingModifier = double.Parse(definition.Attribute("branching")?.Value ?? "1.0");
		LearningModifier = double.Parse(definition.Attribute("improving")?.Value ?? "1.0");
		MinimumDifficultyForModifier = (Difficulty)int.Parse(definition.Attribute("min_difficulty")?.Value ??
		                                                     ((int)Difficulty.Automatic).ToString());
		MaximumDifficultyForModifier = (Difficulty)int.Parse(definition.Attribute("max_difficulty")?.Value ??
		                                                     ((int)Difficulty.Impossible).ToString());
	}

	protected SkillLearningMerit()
	{
	}

	protected SkillLearningMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Skill Learning", "@ learn|learns skills at a different rate")
	{
		BranchingModifier = 1.0;
		LearningModifier = 1.0;
		MinimumDifficultyForModifier = Difficulty.Automatic;
		MaximumDifficultyForModifier = Difficulty.Impossible;
		DoDatabaseInsert();
	}

	public List<string> Groups { get; } = new();
	public double BranchingModifier { get; set; }
	public double LearningModifier { get; set; }
	public Difficulty MinimumDifficultyForModifier { get; set; }
	public Difficulty MaximumDifficultyForModifier { get; set; }

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Branching Modifier: {BranchingModifier.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Learning Modifier: {LearningModifier.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Minimum Difficulty: {MinimumDifficultyForModifier.DescribeColoured()}");
		sb.AppendLine($"Maximum Difficulty: {MaximumDifficultyForModifier.DescribeColoured()}");
		sb.AppendLine($"Applies to Skill Groups: {(Groups.Count == 0 ? "All".ColourValue() : Groups.Select(x => x.TitleCase().ColourValue()).ListToString())}");
	}

	public double BranchingChanceModifier(IPerceivableHaveTraits ch, ITraitDefinition trait)
	{
		return !Groups.Any() || Groups.Any(x => x.EqualTo(trait.Group)) ? BranchingModifier : 1.0;
	}

	public double SkillLearningChanceModifier(IPerceivableHaveTraits ch, ITraitDefinition trait, Outcome outcome,
		Difficulty difficulty, TraitUseType useType)
	{
		return (!Groups.Any() || Groups.Any(x => x.EqualTo(trait.Group))) &&
		       difficulty >= MinimumDifficultyForModifier &&
		       difficulty <= MaximumDifficultyForModifier
			? LearningModifier
			: 1.0;
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3branching <%>#0 - sets the multiplier for skill branching
	#3learning <%>#0 - sets the multiplier for skill improvement
	#3min <difficulty>#0 - sets the minimum difficulty at which the bonuses apply
	#3max <difficulty>#0 - sets the maximum difficulty at which the bonuses apply
	#3group <which>#0 - toggles a skill group as being affected by the multipliers";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "branching":
			case "branch":
				return BuildingCommandBranching(actor, command);
			case "learning":
			case "learn":
			case "improving":
			case "improve":
				return BuildingCommandLearning(actor, command);
			case "min":
			case "minimum":
			case "mindifficulty":
			case "mindiff":
			case "minimumdifficulty":
				return BuildingCommandMinimumDifficulty(actor, command);
			case "max":
			case "maximum":
			case "maxdifficulty":
			case "maxdiff":
			case "maximumdifficulty":
				return BuildingCommandMaximumDifficulty(actor, command);
			case "group":
			case "groups":
			case "skillgroup":
				return BuildingCommandSkillGroup(actor, command);

		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandSkillGroup(ICharacter actor, StringStack command)
	{
		var groups = Gameworld.Traits.OfType<ISkillDefinition>().Select(x => x.Group.TitleCase()).Distinct().ToList();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which skill group do you want to toggle? The valid choices are {groups.ListToColouredString()}.");
			return false;
		}

		var group = groups.FirstOrDefault(x => x.EqualTo(command.SafeRemainingArgument))?.ToLowerInvariant();
		if (group is null)
		{
			actor.OutputHandler.Send($"There is no such skill group. Valid choices are {groups.ListToColouredString()}");
			return false;
		}

		Changed = true;
		if (Groups.Remove(group))
		{
			actor.OutputHandler.Send($"This merit will no longer apply to skills in the {group.TitleCase().ColourName()} skill group.");
			return true;
		}

		Groups.Add(group);
		actor.OutputHandler.Send($"This merit will now apply to skills in the {group.TitleCase().ColourName()} skill group.");
		return true;
	}

	private bool BuildingCommandMaximumDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the maximum difficult at which this merit provides a benefit? See {"show difficulties".MXPSend()} for options.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty difficulty))
		{
			actor.OutputHandler.Send($"That is not a valid difficulty. See {"show difficulties".MXPSend()} for options.");
			return false;
		}

		MaximumDifficultyForModifier = difficulty;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now only apply to learning/branch checks of difficulty equal to or less than {difficulty.DescribeColoured()}.");
		return true;
	}

	private bool BuildingCommandMinimumDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the minimum difficult at which this merit provides a benefit? See {"show difficulties".MXPSend()} for options.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty difficulty))
		{
			actor.OutputHandler.Send($"That is not a valid difficulty. See {"show difficulties".MXPSend()} for options.");
			return false;
		}

		MinimumDifficultyForModifier = difficulty;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now only apply to learning/branch checks of difficulty equal to or greater than {difficulty.DescribeColoured()}.");
		return true;
	}

	private bool BuildingCommandLearning(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the percentage modifier for learning rate for this merit?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		LearningModifier = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now provide multiply the base chance of learning a skill by {value.ToStringP2Colour(actor)}.");
		return true;
	}

	private bool BuildingCommandBranching(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the percentage modifier for branching rate for this merit?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		BranchingModifier = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now provide multiply the base chance of branching a skill by {value.ToStringP2Colour(actor)}.");
		return true;
	}
}