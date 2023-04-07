using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class BowGameItemComponentProto : GameItemComponentProto
{
	private IRangedWeaponType _rangedWeaponType;

	protected BowGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Bow")
	{
		RangedWeaponType =
			gameworld.RangedWeaponTypes.FirstOrDefault(x => x.RangedWeaponType == Combat.RangedWeaponType.Bow);
		StaminaPerTick = gameworld.GetStaticDouble("DefaultBowStaminaDrain");
		MeleeWeaponType = gameworld.WeaponTypes.Get(gameworld.GetStaticLong("DefaultBowMeleeWeaponType"));
	}

	protected BowGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public IRangedWeaponType RangedWeaponType
	{
		get => _rangedWeaponType;
		set
		{
			_rangedWeaponType = value;
			LoadTemplate = new InventoryPlanTemplate(Gameworld, new[]
			{
				new InventoryPlanPhaseTemplate(1, new[]
				{
					InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0, item =>
					{
						var ammo = item.GetItemType<IAmmo>();

						if (ammo?.AmmoType.RangedWeaponTypes.Contains(Combat.RangedWeaponType.Bow) != true)
						{
							return false;
						}

						return ammo.AmmoType.SpecificType.EqualTo(_rangedWeaponType.SpecificAmmunitionGrade);
					}, null, 1, originalReference: "loaditem"),
					InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Wielded, 0, 0,
						item => item.GetItemType<IRangedWeapon>()?.Prototype == this,
						null)
				})
			});
		}
	}

	public IWeaponType MeleeWeaponType { get; set; }

	public double StaminaPerTick { get; set; }

	public IInventoryPlanTemplate LoadTemplate { get; set; }

	public override string TypeDescription => "Bow";

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("RangedWeaponType");
		if (element != null)
		{
			RangedWeaponType = Gameworld.RangedWeaponTypes.Get(long.Parse(element.Value));
		}

		element = root.Element("MeleeWeaponType");
		if (element != null)
		{
			MeleeWeaponType = Gameworld.WeaponTypes.Get(long.Parse(element.Value));
		}
		else
		{
			MeleeWeaponType = Gameworld.WeaponTypes.Get(Gameworld.GetStaticLong("DefaultBowMeleeWeaponType"));
		}

		element = root.Element("StaminaPerTick");
		if (element != null)
		{
			StaminaPerTick = double.Parse(element.Value);
		}
		else
		{
			StaminaPerTick = Gameworld.GetStaticDouble("DefaultBowStaminaDrain");
			Changed = true;
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(
			actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a bow of type {4} and melee type {6}. It drains {5:N2} stamina per 5 seconds whilst drawn.",
			"Bow Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			RangedWeaponType?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red),
			StaminaPerTick,
			MeleeWeaponType?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)
		);
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
				new XElement("RangedWeaponType", RangedWeaponType?.Id ?? 0),
				new XElement("StaminaPerTick", StaminaPerTick),
				new XElement("MeleeWeaponType", MeleeWeaponType?.Id ?? 0)
			).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("bow", true,
			(gameworld, account) => new BowGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Bow", (proto, gameworld) => new BowGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Bow",
			$"Makes an item a {"[ranged weapon]".Colour(Telnet.BoldCyan)} with bow mechanics",
			$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tranged <ranged type> - sets the ranged weapon type for this component. See {"show ranges".FluentTagMXP("send", "href='show ranges'")} for a list.\n\tstamina <amount> - sets the stamina drained per 5 seconds of wielding a drawn bow."
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new BowGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new BowGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new BowGameItemComponentProto(proto, gameworld));
	}

	#region Building Commands

	public override string ShowBuildingHelp =>
		$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tranged <ranged type> - sets the ranged weapon type for this component. See {"show ranges".FluentTagMXP("send", "href='show ranges'")} for a list.\n\tstamina <amount> - sets the stamina drained per 5 seconds of wielding a drawn bow.";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "ranged":
			case "ranged type":
			case "rangedtype":
			case "type":
				return BuildingCommand_Type(actor, command);
			case "stamina":
			case "stam":
				return BuildingCommand_Stamina(actor, command);
			case "melee":
			case "meleetype":
			case "melee type":
			case "melee_type":
				return BuildingCommand_Melee(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommand_Melee(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which melee weapon type do you want to set for this component?");
			return false;
		}

		var type = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.WeaponTypes.Get(value)
			: actor.Gameworld.WeaponTypes.GetByName(command.Last);
		if (type == null)
		{
			actor.Send("There is no such melee weapon type.");
			return false;
		}

		MeleeWeaponType = type;
		Changed = true;
		actor.Send(
			$"This component will now use the melee weapon type {MeleeWeaponType.Name.TitleCase().Colour(Telnet.Green)}.");
		return true;
	}

	private bool BuildingCommand_Stamina(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How much stamina should this bow drain per 5 seconds of use held drawn?");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var value) || value < 0.0)
		{
			actor.Send("You must enter a positive amount of stamina for this bow to drain.");
			return false;
		}

		StaminaPerTick = value;
		Changed = true;
		actor.Send($"This bow will now drain {StaminaPerTick:N2} stamina per 5 seconds whilst held drawn.");
		return true;
	}

	private bool BuildingCommand_Type(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which weapon type do you want to set for this ranged weapon?");
			return false;
		}

		var type = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.RangedWeaponTypes.Get(value)
			: actor.Gameworld.RangedWeaponTypes.GetByName(command.Last);
		if (type == null)
		{
			actor.Send("There is no such ranged weapon type.");
			return false;
		}

		if (type.RangedWeaponType != Combat.RangedWeaponType.Bow)
		{
			actor.Send("You can only give bows a ranged weapon type that is also for a bow.");
			return false;
		}

		RangedWeaponType = type;
		actor.Send(
			$"This ranged weapon will now be of type {RangedWeaponType.Name.TitleCase().Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	#endregion


	#region Overrides of EditableItem

	public override bool CanSubmit()
	{
		if (MeleeWeaponType == null)
		{
			return false;
		}

		if (RangedWeaponType == null)
		{
			return false;
		}

		return base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (MeleeWeaponType == null)
		{
			return "You must give this component a melee weapon type.";
		}

		if (RangedWeaponType == null)
		{
			return "You must give this component a ranged weapon type.";
		}

		return base.WhyCannotSubmit();
	}

	#endregion
}