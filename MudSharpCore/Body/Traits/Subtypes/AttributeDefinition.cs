using MudSharp.Body.Traits.Decorators;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits.Subtypes;

public class AttributeDefinition : TraitDefinition, IAttributeDefinition
{
	public AttributeDefinition(MudSharp.Models.TraitDefinition trait, IFuturemud game)
		: base(trait, game)
	{
		_maxValue = 50;
		ChargenBlurb = trait.ChargenBlurb;
		Alias = trait.Alias;
		DisplayAsSubAttribute = trait.DisplayAsSubAttribute;
		DisplayOrder = trait.DisplayOrder;
		ShowInAttributeCommand = trait.ShowInAttributeCommand;
		ShowInScoreCommand = trait.ShowInScoreCommand;
	}

	public override TraitType TraitType => TraitType.Attribute;

	public override ITrait LoadTrait(MudSharp.Models.Trait trait, IHaveTraits owner)
	{
		return new Attribute(this, trait, owner);
	}


	public override void Save()
	{
		var dbitem = FMDB.Context.TraitDefinitions.Find(Id);
		dbitem.Name = Name;
		dbitem.TraitGroup = Group;
		dbitem.Type = (int)TraitType;
		dbitem.DerivedType = 0;
		dbitem.Alias = Alias;
		dbitem.TeachDifficulty = (int)Difficulty.Impossible;
		dbitem.LearnDifficulty = (int)Difficulty.Impossible;
		dbitem.ChargenBlurb = ChargenBlurb;
		dbitem.Hidden = Hidden;
		dbitem.BranchMultiplier = BranchMultiplier;
		dbitem.DecoratorId = Decorator.Id;
		dbitem.DisplayOrder = DisplayOrder;
		dbitem.DisplayAsSubAttribute = DisplayAsSubAttribute;
		dbitem.ShowInScoreCommand = ShowInScoreCommand;
		dbitem.ShowInAttributeCommand = ShowInAttributeCommand;
		Changed = false;
	}

	#region IAttributeDefinition Members

	public string Alias { get; protected set; }

	public string ChargenBlurb { get; protected set; }

	public int DisplayOrder { get; protected set; }
	public bool DisplayAsSubAttribute { get; protected set; }
	public bool ShowInScoreCommand { get; protected set; }
	public bool ShowInAttributeCommand { get; protected set; }

	#endregion
}