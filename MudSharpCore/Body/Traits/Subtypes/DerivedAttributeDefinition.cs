using System;
using System.Collections.Generic;
using System.Text;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits.Subtypes;

public class DerivedAttributeDefinition : DerivedTraitDefinition, IAttributeDefinition
{
	public DerivedAttributeDefinition(Models.TraitDefinition trait, IFuturemud game) : base(trait, game)
	{
		Alias = trait.Alias;
		ChargenBlurb = trait.ChargenBlurb;
		DisplayAsSubAttribute = trait.DisplayAsSubAttribute;
		DisplayOrder = trait.DisplayOrder;
		ShowInAttributeCommand = trait.ShowInAttributeCommand;
		ShowInScoreCommand = trait.ShowInScoreCommand;
	}

	#region Overrides of TraitDefinition

	public override TraitType TraitType => TraitType.DerivedAttribute;

	#endregion

	#region Overrides of SaveableItem

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

	#endregion

	#region Implementation of IAttributeDefinition

	public string Alias { get; set; }
	public string ChargenBlurb { get; set; }

	public override ITrait LoadTrait(MudSharp.Models.Trait trait, IHaveTraits owner)
	{
		return new DerivedAttribute(this, trait, owner);
	}

	/// <inheritdoc />
	protected override ITrait NewTraitBeforeInsert(IHaveTraits owner, double value)
	{
		return new DerivedAttribute(this, owner);
	}

	public int DisplayOrder { get; set; }
	public bool DisplayAsSubAttribute { get; set; }
	public bool ShowInScoreCommand { get; set; }
	public bool ShowInAttributeCommand { get; set; }

	#endregion
}