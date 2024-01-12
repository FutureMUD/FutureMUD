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

public class SkillDefinition : TraitDefinition, ISkillDefinition
{
	private readonly List<ChargenResourceCost> _costs = new();

	public SkillDefinition(MudSharp.Models.TraitDefinition trait, IFuturemud game)
		: base(trait, game)
	{
		Improver = trait.ImproverId.HasValue
			? game.ImprovementModels.Get(trait.ImproverId.Value)
			: new NonImproving();
		AvailabilityProg = game.FutureProgs.Get(trait.AvailabilityProgId ?? 0);
		foreach (var item in trait.TraitDefinitionsChargenResources)
		{
			_costs.Add(new ChargenResourceCost(amount: item.Amount, requirementOnly: item.RequirementOnly,
				resource: Gameworld.ChargenResources.Get(item.ChargenResourceId) ??
				          throw new InvalidOperationException()));
		}

		TeachableProg = game.FutureProgs.Get(trait.TeachableProgId ?? 0);
		LearnableProg = game.FutureProgs.Get(trait.LearnableProgId ?? 0);
		TeachDifficulty = (Difficulty)trait.TeachDifficulty;
		LearnDifficulty = (Difficulty)trait.LearnDifficulty;
	}

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
		dbitem.ExpressionId = Cap.Id;
		dbitem.ImproverId = Improver.Id;
		dbitem.DecoratorId = Decorator.Id;
		Changed = false;
	}

	public override bool ImprovesWithUse => true;

	public IFutureProg AvailabilityProg { get; protected set; }

	public IImprovementModel Improver { get; protected set; }

	public ITraitExpression Cap { get; set; }

	public override TraitType TraitType => TraitType.Skill;

	public override string MaxValueString => Cap.OriginalFormulaText;

	public override void Initialise(MudSharp.Models.TraitDefinition definition)
	{
		Cap = Gameworld.TraitExpressions.Get(definition.ExpressionId ?? 0);
	}

	public override ITrait LoadTrait(MudSharp.Models.Trait trait, IHaveTraits owner)
	{
		return new Skill(this, trait, owner);
	}

	public override bool ChargenAvailable(ICharacterTemplate template)
	{
		return
			_costs.Where(x => x.RequirementOnly).All(x =>
				template.Account.AccountResources[x.Resource] >= x.Amount) &&
			((bool?)AvailabilityProg?.Execute(template, this) ?? true);
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


	#region ISkillDefinition Members

	public IFutureProg TeachableProg { get; set; }

	public string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Skill Definition #{Id.ToString("N0", voyeur)} - {Name.ColourName()}");
		sb.AppendLine($"Group: {Group.TitleCase().ColourValue()}");
		sb.AppendLine(
			$"Cap Expression: #{Cap.Id.ToString("N0", voyeur)} ({Cap.Name}): {Cap.OriginalFormulaText.ColourCommand()}");
		sb.AppendLine($"Improver: {Improver.Name.ColourValue()}");
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

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "group":
				return BuildingCommandGroup(actor, command);
			case "improver":
				return BuildingCommandImprover(actor, command);
			case "decorator":
			case "describer":
			case "describe":
			case "decorate":
				return BuildingCommandDecorator(actor, command);
			case "hidden":
				return BuildingCommandHidden(actor, command);
			case "branch":
				return BuildingCommandBranch(actor, command);
			case "expression":
				return BuildingCommandExpression(actor, command);
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

		actor.OutputHandler.Send(@"You can use the following options with the set command:

    skill set name <name> - edits the name of a skill
    skill set expression <expression> - changes the cap expression for a skill(*)
    skill set improver <which> - sets the skill improver for a skill
    skill set decorator <which> - sets the skill describer for a skill
    skill set group <group> - sets the skill group for a skill
    skill set branch <multiplier%> - sets the branch multiplier for a skill
    skill set chargen <prog> - sets a prog to determine chargen availability
    skill set teachable <prog> - sets a prog to determine teachability
    skill set learnable <prog> - sets a prog to determine learnability
    skill set teach <difficulty> - sets the difficulty of teaching the skill
    skill set learn <difficulty> - sets the difficulty of learning the skill
    skill set hidden - toggles this being a hidden skill

    * - most often you will want to use the TRAITEXPRESSION command to edit the existing trait expression rather than changing to a new one");
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to rename the skill?");
			return false;
		}

		var name = command.PopSpeech().ToLowerInvariant().TitleCase();
		if (Gameworld.Traits.Any(x => x.TraitType == TraitType.Skill && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a skill with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the skill {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
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

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"The specified prog must return a boolean value. {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourValue()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<FutureProgVariableTypes> { FutureProgVariableTypes.Character }) &&
		    !prog.MatchesParameters(new List<FutureProgVariableTypes>
			    { FutureProgVariableTypes.Character, FutureProgVariableTypes.Trait }))
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

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"The specified prog must return a boolean value. {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourValue()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<FutureProgVariableTypes> { FutureProgVariableTypes.Character }) &&
		    !prog.MatchesParameters(new List<FutureProgVariableTypes>
			    { FutureProgVariableTypes.Character, FutureProgVariableTypes.Trait }))
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

		var prog = new FutureProgLookupFromBuilderInput(actor.Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new[]
			{
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Toon
				},
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Toon,
					FutureProgVariableTypes.Trait
				}
			}).LookupProg();

		if (prog == null)
		{
			return false;
		}

		AvailabilityProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This skill will now use the prog {prog.MXPClickableFunctionNameWithId()} to determine whether it can be picked in chargen.");
		return true;
	}

	private bool BuildingCommandExpression(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait expression do you want to use for the skill cap?");
			return false;
		}

		var expression = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.TraitExpressions.Get(value)
			: Gameworld.TraitExpressions.GetByName(command.SafeRemainingArgument);
		if (expression == null)
		{
			actor.OutputHandler.Send("There is no such trait expression.");
			return false;
		}

		Cap = expression;
		Changed = true;
		actor.OutputHandler.Send(
			$"This skill will now use the {expression.Name.TitleCase().ColourValue()} trait expression for its cap formula.");
		return true;
	}

	private bool BuildingCommandHidden(ICharacter actor, StringStack command)
	{
		Hidden = !Hidden;
		Changed = true;
		actor.OutputHandler.Send($"This skill is {(Hidden ? "now" : "no longer")} hidden.");
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

	private bool BuildingCommandDecorator(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait decorator do you want to use for the skill?");
			return false;
		}

		var decorator = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.TraitDecorators.Get(value)
			: Gameworld.TraitDecorators.GetByName(command.SafeRemainingArgument);
		if (decorator == null)
		{
			actor.OutputHandler.Send("There is no such trait decorator.");
			return false;
		}

		Decorator = decorator;
		Changed = true;
		actor.OutputHandler.Send(
			$"This skill will now use the {decorator.Name.TitleCase().ColourValue()} trait decorator.");
		return true;
	}

	private bool BuildingCommandImprover(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait improver do you want to use for the skill?");
			return false;
		}

		var improver = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.ImprovementModels.Get(value)
			: Gameworld.ImprovementModels.GetByName(command.SafeRemainingArgument);
		if (improver == null)
		{
			actor.OutputHandler.Send("There is no such trait improver.");
			return false;
		}

		Improver = improver;
		Changed = true;
		actor.OutputHandler.Send(
			$"This skill will now use the {improver.Name.TitleCase().ColourValue()} trait improver.");
		return true;
	}

	private bool BuildingCommandGroup(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which group do you want this skill to belong to? Existing skills use the following groups: {Gameworld.Traits.OfType<ISkillDefinition>().Select(x => x.Group).Distinct().Select(x => x.TitleCase().ColourValue()).ListToString()}");
			return false;
		}

		Group = command.SafeRemainingArgument.ToLowerInvariant().TitleCase();
		Changed = true;
		actor.OutputHandler.Send($"The {Name.ColourName()} skill now belongs to the {Group.ColourValue()} group.");
		return true;
	}

	#endregion
}