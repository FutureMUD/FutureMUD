using System;
using System.Linq;
using System.Text;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
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

	public AttributeDefinition(IFuturemud gameworld, string name, string alias) : base(gameworld)
	{
		_name = name;
		Alias = alias;
		DisplayAsSubAttribute = false;
		DisplayOrder = 0;
		ShowInAttributeCommand = true;
		ShowInScoreCommand = true;
		ChargenBlurb = "This attribute has no description";
		Hidden = false;
		Group = "Physical";
		Decorator = Gameworld.TraitDecorators.First();
		BranchMultiplier = 1.0;
		DoDatabaseInsert();
	}

	protected AttributeDefinition(AttributeDefinition rhs, string name, string alias) : base(rhs.Gameworld)
	{
		_name = name;
		Alias = alias;
		DisplayAsSubAttribute = rhs.DisplayAsSubAttribute;
		DisplayOrder = rhs.DisplayOrder;
		ShowInAttributeCommand = rhs.ShowInAttributeCommand;
		ShowInScoreCommand = rhs.ShowInScoreCommand;
		ChargenBlurb = rhs.ChargenBlurb;
		Hidden = rhs.Hidden;
		Group = rhs.Group;
		Decorator = rhs.Decorator;
		BranchMultiplier = rhs.BranchMultiplier;
		DoDatabaseInsert();
	}

	protected void DoDatabaseInsert()
	{
		using (new FMDB())
		{
			var dbitem = new Models.TraitDefinition
			{
				Name = Name,
				Type = (int)Traits.TraitType.Attribute,
				DecoratorId = Decorator.Id,
				TraitGroup = Group,
				DerivedType = 0,
				Hidden = Hidden,
				ChargenBlurb = ChargenBlurb,
				BranchMultiplier = BranchMultiplier,
				Alias = Alias,
				TeachDifficulty = 0,
				LearnDifficulty = 0,
				DisplayOrder = DisplayOrder,
				DisplayAsSubAttribute = DisplayAsSubAttribute,
				ShowInScoreCommand = ShowInScoreCommand,
				ShowInAttributeCommand = ShowInAttributeCommand,
			};
			FMDB.Context.TraitDefinitions.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public IAttributeDefinition Clone(string name, string alias)
	{
		return new AttributeDefinition(this, name, alias);
	}

	public override TraitType TraitType => TraitType.Attribute;

	public override ITrait LoadTrait(MudSharp.Models.Trait trait, IHaveTraits owner)
	{
		return new Attribute(this, trait, owner);
	}

	/// <inheritdoc />
	protected override ITrait NewTraitBeforeInsert(IHaveTraits owner, double value)
	{
		return new Attribute(this, value, owner);
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

	#region IEditableItem Members

	/// <inheritdoc />
	public override string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Attribute Definition #{Id.ToString("N0", voyeur)} - {Name}".GetLineWithTitleInner(voyeur, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Alias: {Alias.ColourValue()}");
		sb.AppendLine($"Group: {Group.TitleCase().ColourValue()}");
		sb.AppendLine($"Decorator: {Decorator.Name.ColourValue()}");
		sb.AppendLine($"Hidden: {Hidden.ToColouredString()}");
		sb.AppendLine($"Sub Attribute: {DisplayAsSubAttribute.ToColouredString()}");
		sb.AppendLine($"Show in Score: {ShowInScoreCommand.ToColouredString()}");
		sb.AppendLine($"Show in Attributes: {ShowInAttributeCommand.ToColouredString()}");
		sb.AppendLine();
		sb.AppendLine("Chargen Blurb:");
		sb.AppendLine();
		sb.AppendLine(ChargenBlurb.Wrap(voyeur.InnerLineFormatLength, "\t"));
		sb.AppendLine();
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

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "alias":
				return BuildingCommandAlias(actor, command);
			case "description":
			case "desc":
			case "chargendesc":
			case "chargendescription":
				return BuildingCommandChargenDescription(actor, command);
			case "score":
				return BuildingCommandScore(actor);
			case "attributes":
				return BuildingCommandAttributes(actor);
			case "sub":
				return BuildingCommandSub(actor);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandSub(ICharacter actor)
	{
		DisplayAsSubAttribute = !DisplayAsSubAttribute;
		Changed = true;
		actor.OutputHandler.Send($"This attribute will {DisplayAsSubAttribute.NowNoLonger()} display as a sub attribute.");
		return true;
	}

	private bool BuildingCommandAttributes(ICharacter actor)
	{
		ShowInAttributeCommand = !ShowInAttributeCommand;
		Changed = true;
		actor.OutputHandler.Send($"This attribute will {ShowInAttributeCommand.NowNoLonger()} display in the attribute command.");
		return true;
	}

	private bool BuildingCommandScore(ICharacter actor)
	{
		ShowInScoreCommand = !ShowInScoreCommand;
		Changed = true;
		actor.OutputHandler.Send($"This attribute will {ShowInScoreCommand.NowNoLonger()} display in the score command.");
		return true;
	}

	private bool BuildingCommandChargenDescription(ICharacter actor, StringStack command)
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(ChargenBlurb))
		{
			sb.AppendLine("Replacing:\n\n");
			sb.AppendLine(ChargenBlurb.SubstituteANSIColour().ProperSentences().Wrap(actor.InnerLineFormatLength, "\t"));
			sb.AppendLine();
		}

		sb.AppendLine("Enter the chargen text in the editor below.");
		sb.AppendLine();
		actor.OutputHandler.Send(sb.ToString());
		actor.EditorMode(postAction: PostAction, cancelAction: CancelAction, recallText: ChargenBlurb, suppliedArguments: [actor.InnerLineFormatLength]);
		return true;
	}

	private void CancelAction(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to update that attribute's description.");
	}

	private void PostAction(string text, IOutputHandler handler, object[] args)
	{
		ChargenBlurb = text;
		Changed = true;
		handler.Send($"You change the chargen blurb for this attribute to:\n\n{text.SubstituteANSIColour().ProperSentences().Wrap((int)args[0])}");
	}

	private bool BuildingCommandAlias(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What alias do you want to set for this attribute?");
			return false;
		}

		var alias = command.SafeRemainingArgument.ToLowerInvariant();
		if (Gameworld.Traits.OfType<IAttributeDefinition>().Any(x => x.Alias.EqualTo(alias)))
		{
			actor.OutputHandler.Send($"There is already an attribute with the alias {alias.ColourCommand()}. Aliases must be unique.");
			return false;
		}

		Alias = alias;
		Changed = true;
		actor.OutputHandler.Send($"This attribute's alias is now {alias.ColourCommand()}.");
		return true;
	}

	/// <inheritdoc />
	protected override string SubtypeHelpText => @"#3alias <alias>#0 - changes the alias
	#3score#0 - toggles appearing in score command
	#3attributes#0 - toggles appearing in attributes command
	#3sub#0 - toggles displaying as a sub-attribute
	#3blurb#0 - drops you into an editor to change the chargen blurb";

	#endregion
}