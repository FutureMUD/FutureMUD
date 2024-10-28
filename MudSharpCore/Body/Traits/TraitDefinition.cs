using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Body.Traits;

public abstract class TraitDefinition : SaveableItem, ITraitDefinition
{
	public override string FrameworkItemType => "TraitDefinition";
	protected double _maxValue;

	protected TraitDefinition(IFuturemud game)
	{
		Gameworld = game;
	}

	protected TraitDefinition(MudSharp.Models.TraitDefinition trait, IFuturemud game)
	{
		_id = trait.Id;
		_name = trait.Name;
		Gameworld = game;
		Group = trait.TraitGroup;
		Decorator = game.TraitDecorators.Get(trait.DecoratorId);
		Hidden = trait.Hidden ?? false;
		BranchMultiplier = trait.BranchMultiplier;
	}

	public virtual bool ImprovesWithUse => false;
	public string Group { get; protected set; }
	public abstract TraitType TraitType { get; }
	public virtual double MaxValue => _maxValue;

	public virtual string MaxValueString => _maxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

	public bool Hidden { get; protected set; }

	public ITraitValueDecorator Decorator { get; protected set; }

	public virtual void Initialise(MudSharp.Models.TraitDefinition definition)
	{
	}

	public static ITrait LoadTrait(MudSharp.Models.Trait trait, IFuturemud game, IHaveTraits owner)
	{
		var definition = game.Traits.Get(trait.TraitDefinitionId);
		return definition.LoadTrait(trait, owner);
	}

	public static ITraitDefinition LoadTraitDefinition(MudSharp.Models.TraitDefinition trait, IFuturemud game)
	{
		switch ((TraitType)trait.Type)
		{
			case TraitType.Skill:
				return new SkillDefinition(trait, game);
			case TraitType.Attribute:
				return new AttributeDefinition(trait, game);
			case TraitType.TheoreticalSkill:
				return new TheoreticalSkillDefinition(trait, game);
			case TraitType.DerivedAttribute:
				return new DerivedAttributeDefinition(trait, game);
			case TraitType.DerivedSkill:
				return new DerivedSkillDefinition(trait, game);
			default:
				return null;
		}
	}

	#region ITraitDefinition Members

	public abstract ITrait LoadTrait(MudSharp.Models.Trait trait, IHaveTraits owner);

	protected abstract ITrait NewTraitBeforeInsert(IHaveTraits owner, double value);

	public virtual ITrait NewTrait(IHaveTraits owner, double value)
	{
		// Note, it is necessary to do this prior to creating the MudSharp.Models.Trait because owner.ID may trigger a database save, which would flush the DBContext
		var ownerId = owner.Id; 

		// It's also possible that the OwnerID is 0, which happens when a skill gets added during creation from a CharacterTemplate due to a trait adjustment. We need to handle that separately as the character and body won't have been inserted into the database at that point.
		if (ownerId == 0L)
		{
			return NewTraitBeforeInsert(owner, value);
		}

		using (new FMDB())
		{
			var trait = FMDB.Context.Traits.Find(owner.Id, Id);
			if (trait == null)
			{
				trait = new Models.Trait();
				FMDB.Context.Traits.Add(trait);
			}

			trait.Value = value;
			trait.BodyId = ownerId;
			trait.TraitDefinitionId = Id;
			FMDB.Context.SaveChanges();
			return LoadTrait(trait, owner);
		}
	}

	public virtual bool ChargenAvailable(ICharacterTemplate template)
	{
		return true;
	}

	public virtual int ResourceCost(IChargenResource resource)
	{
		return 0;
	}

	public virtual int ResourceRequirement(IChargenResource resource)
	{
		return 0;
	}

	public virtual bool HasResourceCosts => false;

	public double BranchMultiplier { get; set; }

	public virtual IEnumerable<(IChargenResource resource, int cost)> ResourceCosts =>
		Enumerable.Empty<(IChargenResource, int)>();

	public abstract string Show(ICharacter actor);

	public string HelpText => @$"You can use the following options with this command:

	#3name <name>#0 - change the name
	#3group <which>#0 - sets the group
	#3decorator <which>#0 - sets the trait decorator
	#3hidden#0 - toggles whether or not this trait is hidden
	{SubtypeHelpText}";

	protected abstract string SubtypeHelpText { get; }

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "group":
				return BuildingCommandGroup(actor, command);
			case "decorator":
			case "describer":
			case "describe":
			case "decorate":
				return BuildingCommandDecorator(actor, command);
			case "hidden":
				return BuildingCommandHidden(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to rename the trait?");
			return false;
		}

		var name = command.PopSpeech().ToLowerInvariant().TitleCase();
		if (this is ISkillDefinition)
		{
			if (Gameworld.Traits.Any(x => x.TraitType == TraitType.Skill && x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a skill with that name. Names must be unique.");
				return false;
			}
		}
		else
		{
			if (Gameworld.Traits.Any(x => x.TraitType == TraitType.Attribute && x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already an attribute with that name. Names must be unique.");
				return false;
			}
		}
		
		actor.OutputHandler.Send($"You rename the trait {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandHidden(ICharacter actor, StringStack command)
	{
		Hidden = !Hidden;
		Changed = true;
		actor.OutputHandler.Send($"This trait is {(Hidden ? "now" : "no longer")} hidden.");
		return true;
	}
	private bool BuildingCommandDecorator(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait decorator do you want to use for the trait?");
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
			$"This trait will now use the {decorator.Name.TitleCase().ColourValue()} trait decorator.");
		return true;
	}

	private bool BuildingCommandGroup(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			if (this is ISkillDefinition)
			{
				actor.OutputHandler.Send(
					$"Which group do you want this trait to belong to?\nExisting skills use the following groups: {Gameworld.Traits.OfType<ISkillDefinition>().Select(x => x.Group).Distinct().Select(x => x.TitleCase().ColourValue()).ListToString()}");
			}
			else
			{
				actor.OutputHandler.Send(
					$"Which group do you want this trait to belong to?\nExisting attributes use the following groups: {Gameworld.Traits.OfType<IAttributeDefinition>().Select(x => x.Group).Distinct().Select(x => x.TitleCase().ColourValue()).ListToString()}");
			}
			
			return false;
		}

		Group = command.SafeRemainingArgument.ToLowerInvariant().TitleCase();
		Changed = true;
		actor.OutputHandler.Send($"The {Name.ColourName()} trait now belongs to the {Group.ColourValue()} group.");
		return true;
	}


	#endregion

	#region IFutureProgVariable Members

	private static ProgVariableTypes DotReferenceHandler(string property)
	{
		var returnVar = ProgVariableTypes.Error;
		switch (property.ToLowerInvariant())
		{
			case "id":
				returnVar = ProgVariableTypes.Number;
				break;
			case "name":
				returnVar = ProgVariableTypes.Text;
				break;
			case "group":
				returnVar = ProgVariableTypes.Text;
				break;
			case "isskill":
				returnVar = ProgVariableTypes.Boolean;
				break;
			case "isattribute":
				returnVar = ProgVariableTypes.Boolean;
				break;
		}

		return returnVar;
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "group", ProgVariableTypes.Text },
			{ "isskill", ProgVariableTypes.Boolean},
			{ "isattribute", ProgVariableTypes.Boolean},
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The ID of the trait" },
			{ "name", "The name of the trait" },
			{ "group", "The group the trait belongs to" },
			{ "isskill", "True if this trait is for a skill"},
			{ "isattribute", "True if this trait is for an attribute"},
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Trait, DotReferenceHandler(),
			DotReferenceHelp());
	}

	public IProgVariable GetProperty(string property)
	{
		IProgVariable returnVar = null;
		switch (property.ToLowerInvariant())
		{
			case "id":
				returnVar = new NumberVariable(Id);
				break;
			case "name":
				returnVar = new TextVariable(Name);
				break;
			case "group":
				returnVar = new TextVariable(Group);
				break;
			case "isskill":
				returnVar = new BooleanVariable(this is ISkillDefinition);
				break;
			case "isattribute":
				returnVar = new BooleanVariable(this is IAttributeDefinition);
				break;
		}

		return returnVar;
	}

	ProgVariableTypes IProgVariable.Type => ProgVariableTypes.Trait;

	public object GetObject => this;

	#endregion
}