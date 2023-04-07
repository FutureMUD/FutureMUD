using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Body.Traits.Subtypes;
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

	public virtual string MaxValueString => _maxValue.ToString(CultureInfo.InvariantCulture);

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

	public virtual ITrait NewTrait(IHaveTraits owner, double value)
	{
		var
			ownerId = owner
				.Id; // Note, it is necessary to do this prior to creating the MudSharp.Models.Trait because owner.ID may trigger a database save, which would flush the DBContext
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

	#endregion

	#region IFutureProgVariable Members

	private static FutureProgVariableTypes DotReferenceHandler(string property)
	{
		var returnVar = FutureProgVariableTypes.Error;
		switch (property.ToLowerInvariant())
		{
			case "id":
				returnVar = FutureProgVariableTypes.Number;
				break;
			case "name":
				returnVar = FutureProgVariableTypes.Text;
				break;
			case "group":
				returnVar = FutureProgVariableTypes.Text;
				break;
		}

		return returnVar;
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "group", FutureProgVariableTypes.Text }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "group", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Trait, DotReferenceHandler(),
			DotReferenceHelp());
	}

	public IFutureProgVariable GetProperty(string property)
	{
		IFutureProgVariable returnVar = null;
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
		}

		return returnVar;
	}

	FutureProgVariableTypes IFutureProgVariable.Type => FutureProgVariableTypes.Trait;

	public object GetObject => this;

	#endregion
}