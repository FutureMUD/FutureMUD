using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using ExpressionEngine;

namespace MudSharp.GameItems.Prototypes;

public class DestroyableGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Destroyable";

	public Expression HpExpression { get; set; }

	public Dictionary<DamageType, double> DamageTypeMultipliers { get; } = new();

	#region Saving

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XElement("HpExpression", new XCData(HpExpression.OriginalExpression)),
				new XElement("DamageMultipliers",
					from item in DamageTypeMultipliers
					select
						new XElement("DamageMultiplier", new XAttribute("type", (int)item.Key),
							new XAttribute("multiplier", item.Value))
				)).ToString();
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nHP expression: {4}.\n\nIt has the following damage type multipliers:\n{5}",
			"Destroyable Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			HpExpression?.OriginalExpression ?? "None Set".Colour(Telnet.Red),
			DamageTypeMultipliers.Select(x => $"{x.Key.Describe()}: {x.Value.ToString("N3", actor)}")
			                     .ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n", article: "\t")
		);
	}

	public override bool CanSubmit()
	{
		return HpExpression != null && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		return HpExpression == null ? "You must first give this component an HP expression." : base.WhyCannotSubmit();
	}

	#region Constructors

	private void SetupDefaultMultipliers()
	{
		foreach (var value in Enum.GetValues(typeof(DamageType)).OfType<DamageType>())
		{
			switch (value)
			{
				case DamageType.Ballistic:
				case DamageType.Piercing:
				case DamageType.Bite:
				case DamageType.Shrapnel:
					DamageTypeMultipliers[value] = 0.05;
					break;
				case DamageType.Cellular:
				case DamageType.Hypoxia:
				case DamageType.Freezing:
				case DamageType.Electrical:
					DamageTypeMultipliers[value] = 0.0;
					break;
				case DamageType.Falling:
					DamageTypeMultipliers[value] = 1.0;
					break;
				default:
					DamageTypeMultipliers[value] = 0.25;
					break;
			}
		}
	}

	protected DestroyableGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Destroyable")
	{
		HpExpression = new Expression("10 * quality");
		SetupDefaultMultipliers();
	}

	protected DestroyableGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		SetupDefaultMultipliers();
		HpExpression = new Expression(root.Element("HpExpression").Value);
		var element = root.Element("DamageMultipliers");
		if (element != null)
		{
			foreach (var sub in element.Elements())
			{
				DamageTypeMultipliers[(DamageType)int.Parse(sub.Attribute("type").Value)] =
					double.Parse(sub.Attribute("multiplier").Value);
			}
		}
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new DestroyableGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new DestroyableGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Destroyable".ToLowerInvariant(), true,
			(gameworld, account) => new DestroyableGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Destroyable",
			(proto, gameworld) => new DestroyableGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Destroyable",
			$"Makes an item able to take damage and be destroyed",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new DestroyableGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\thp <formula> - sets the formula for hp. Use parameter 'quality' for item quality\n\tdamage <type> <%> - sets a damage suffered multiplier for a type";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "hp":
			case "expression":
			case "hp expression":
			case "health":
			case "health expression":
				return BuildingCommandHpExpression(actor, command);
			case "multiplier":
			case "damage":
			case "dam":
				return BuildingCommandDamage(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandDamage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What damage type do you want to set a multiplier for?");
			return false;
		}

		if (!WoundExtensions.TryGetDamageType(command.Pop(), out var type))
		{
			actor.Send("There is no such damage type.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send("What multiplier do you want to set for that type of damage?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.Send("That is not a valid number.");
			return false;
		}

		if (value < 0.0)
		{
			actor.Send("You cannot enter a negative number for the damage multiplier.");
			return false;
		}

		DamageTypeMultipliers[type] = value;
		Changed = true;
		actor.Send(
			$"This item will now have a multiplier of {value.ToString("N3", actor).Colour(Telnet.Green)} for all damage of type {type.Describe().Colour(Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandHpExpression(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What formula do you want to use for HP?");
			return false;
		}

		var formula = new Expression(command.SafeRemainingArgument);
		if (formula.HasErrors())
		{
			actor.Send(formula.Error);
			return false;
		}

		HpExpression = formula;
		Changed = true;
		actor.Send(
			$"This item will now use the following formula for HP: {command.SafeRemainingArgument.ColourCommand()}");
		return true;
	}

	#endregion
}