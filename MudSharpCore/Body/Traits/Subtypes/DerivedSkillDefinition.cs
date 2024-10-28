using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits.Subtypes;

public class DerivedSkillDefinition : DerivedTraitDefinition, ISkillDefinition
{
	public DerivedSkillDefinition(Models.TraitDefinition trait, IFuturemud game) : base(trait, game)
	{
	}

	#region Overrides of SaveableItem

	public override void Save()
	{
		var dbitem = FMDB.Context.TraitDefinitions.Find(Id);
		dbitem.Name = Name;
		dbitem.TraitGroup = Group;
		dbitem.Type = (int)TraitType;
		dbitem.DerivedType = 0;
		dbitem.Alias = string.Empty;
		dbitem.AvailabilityProgId = AvailabilityProg.Id;
		dbitem.TeachableProgId = TeachableProg.Id;
		dbitem.LearnableProgId = LearnableProg.Id;
		dbitem.TeachDifficulty = (int)TeachDifficulty;
		dbitem.LearnDifficulty = (int)LearnDifficulty;
		dbitem.ChargenBlurb = string.Empty;
		dbitem.Hidden = Hidden;
		dbitem.BranchMultiplier = BranchMultiplier;
		dbitem.DecoratorId = Decorator.Id;
		Changed = false;
	}

	#endregion

	#region Overrides of TraitDefinition

	public override ITrait LoadTrait(Models.Trait trait, IHaveTraits owner)
	{
		return new DerivedSkill(this, trait, owner);
	}

	/// <inheritdoc />
	protected override ITrait NewTraitBeforeInsert(IHaveTraits owner, double value)
	{
		return new DerivedSkill(this, owner);
	}

	#endregion

	#region Chargen

	private readonly List<ChargenResourceCost> _costs = new();

	public override bool ChargenAvailable(ICharacterTemplate template)
	{
		return
			_costs.Where(x => x.RequirementOnly).All(x =>
				template.Account.AccountResources[x.Resource] >= x.Amount) &&
			((bool?)AvailabilityProg?.Execute(template) ?? true);
	}

	public override int ResourceCost(IChargenResource resource)
	{
		return _costs.FirstOrDefault(x => !x.RequirementOnly && x.Resource == resource)?.Amount ?? 0;
	}

	public override int ResourceRequirement(IChargenResource resource)
	{
		return _costs.FirstOrDefault(x => x.RequirementOnly && x.Resource == resource)?.Amount ?? 0;
	}

	public override bool HasResourceCosts => _costs.Any(x => !x.RequirementOnly && x.Amount != 0);

	public override IEnumerable<(IChargenResource resource, int cost)> ResourceCosts =>
		_costs.Where(x => !x.RequirementOnly).Select(x => (x.Resource, x.Amount));

	#endregion

	public IFutureProg AvailabilityProg { get; protected set; }

	public override TraitType TraitType => TraitType.DerivedSkill;

	#region Implementation of ISkillDefinition

	public IFutureProg TeachableProg { get; set; }

	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Skill Definition #{Id.ToString("N0", voyeur)} - {Name.ColourName()}");
		sb.AppendLine($"Group: {Group.TitleCase().ColourValue()}");
		sb.AppendLine($"Cap Expression: N/A");
		sb.AppendLine($"Improver: N/A");
		sb.AppendLine($"Decorator: {Decorator.Name.ColourValue()}");
		sb.AppendLine($"Branch Multiplier: {BranchMultiplier.ToString("P2", voyeur).ColourValue()}");
		sb.AppendLine($"Chargen Prog: {AvailabilityProg.MXPClickableFunctionNameWithId()}");
		sb.AppendLine($"Teach Prog: {TeachableProg.MXPClickableFunctionNameWithId()}");
		sb.AppendLine($"Learn Prog: {LearnableProg.MXPClickableFunctionNameWithId()}");
		sb.AppendLine($"Teach Difficulty: {TeachDifficulty.Describe().ColourValue()}");
		sb.AppendLine($"Learn Difficulty: {LearnDifficulty.Describe().ColourValue()}");
		sb.AppendLine($"Hidden: {Hidden.ToColouredString()}");
		if (ResourceCosts.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Chargen Resource Costs:");
			sb.AppendLine();
			foreach (var cost in ResourceCosts)
			{
				sb.AppendLine(
					$"\t{cost.cost.ToString("N0", voyeur)} {(cost.cost == 1 ? cost.resource.Name : cost.resource.PluralName)}"
						.ColourValue());
			}
		}

		return sb.ToString();
	}

	public bool CanTeach(ICharacter character)
	{
		return (bool?)TeachableProg?.Execute(character, this) ?? true;
	}

	public IFutureProg LearnableProg { get; set; }

	public bool CanLearn(ICharacter character)
	{
		return (bool?)LearnableProg?.Execute(character, this) ?? true;
	}

	public Difficulty TeachDifficulty { get; set; }
	public Difficulty LearnDifficulty { get; set; }

	protected override string SubtypeHelpText => @"#3branch <multiplier%>#0 - sets the branch multiplier for a skill
	#3chargen <prog>#0 - sets a prog to determine chargen availability
	#3teachable <prog>#0 - sets a prog to determine teachability
	#3learnable <prog>#0 - sets a prog to determine learnability
	#3teach <difficulty>#0 - sets the difficulty of teaching the skill
	#3learn <difficulty>#0 - sets the difficulty of learning the skill
	#3improver <which>#0 - sets the trait improver
	#3expression <expression>#0 - changes the cap expression for a skill

Note: Most often you will want to use the #3TRAITEXPRESSION#0 command to edit the existing trait expression rather than changing to a new one";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "branch":
				return BuildingCommandBranch(actor, command);
			case "chargen":
				return BuildingCommandChargen(actor, command);
			case "teachable":
				return BuildingCommandTeachable(actor, command);
			case "learnable":
				return BuildingCommandLearnable(actor, command);
			case "teach":
				return BuildingCommandTeach(actor, command);
			case "learn":
				return BuildingCommandLearn(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandLearn(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What difficulty should be imposed on the roll to learn this skill? See SHOW DIFFICULTIES for a list.");
			return false;
		}

		if (!command.PopSpeech().TryParseEnum(out Difficulty value))
		{
			actor.OutputHandler.Send("That is not a valid difficulty. See SHOW DIFFICULTIES for a list.");
			return false;
		}

		LearnDifficulty = value;
		Changed = true;
		actor.OutputHandler.Send($"It will now be {value.Describe().ColourValue()} to learn this skill.");
		return true;
	}

	private bool BuildingCommandTeach(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What difficulty should be imposed on the roll to teach this skill? See SHOW DIFFICULTIES for a list.");
			return false;
		}

		if (!command.PopSpeech().TryParseEnum(out Difficulty value))
		{
			actor.OutputHandler.Send("That is not a valid difficulty. See SHOW DIFFICULTIES for a list.");
			return false;
		}

		TeachDifficulty = value;
		Changed = true;
		actor.OutputHandler.Send($"It will now be {value.Describe().ColourValue()} to teach this skill.");
		return true;
	}

	private bool BuildingCommandLearnable(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use to control the learnability of this skill?");
			return false;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"The specified prog must return a boolean value. {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourValue()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character }) &&
		    !prog.MatchesParameters(new List<ProgVariableTypes>
			    { ProgVariableTypes.Character, ProgVariableTypes.Trait }))
		{
			actor.OutputHandler.Send(
				$"The specified prog must either accept a single character parameter, or a character and trait parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		LearnableProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This skill will now use the prog {prog.MXPClickableFunctionNameWithId()} to determine whether it can be learnt.");
		return true;
	}

	private bool BuildingCommandTeachable(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to use to control the teachability of this skill?");
			return false;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"The specified prog must return a boolean value. {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourValue()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Character }) &&
		    !prog.MatchesParameters(new List<ProgVariableTypes>
			    { ProgVariableTypes.Character, ProgVariableTypes.Trait }))
		{
			actor.OutputHandler.Send(
				$"The specified prog must either accept a single character parameter, or a character and trait parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		TeachableProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This skill will now use the prog {prog.MXPClickableFunctionNameWithId()} to determine whether it can be taught.");
		return true;
	}

	private bool BuildingCommandChargen(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to use to control the availability of the skill in chargen?");
			return false;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"The specified prog must return a boolean value. {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourValue()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Toon }))
		{
			actor.OutputHandler.Send(
				$"The specified prog must accept a single toon parameter, whereas {prog.MXPClickableFunctionName()} does not.");
			return false;
		}

		AvailabilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This skill will now use the prog {prog.MXPClickableFunctionNameWithId()} to determine whether it can be picked in chargen.");
		return true;
	}

	private bool BuildingCommandBranch(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What percentage modifier to the base chance to branch do you want this skill to have?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send("You must enter a valid percentage value.");
			return false;
		}

		BranchMultiplier = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This skill is now {value.ToString("P2", actor).ColourValue()} as likely to branch relative to the base chance.");
		return true;
	}

	public IImprovementModel Improver => Gameworld.ImprovementModels.First(x => x is NonImproving);

	public ITraitExpression Cap => new TraitExpression("1000000", Gameworld);

	#endregion
}