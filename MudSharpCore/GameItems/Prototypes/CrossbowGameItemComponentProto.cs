using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class CrossbowGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Crossbow";

	private IRangedWeaponType _rangedWeaponType;

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

						if (ammo?.AmmoType.RangedWeaponTypes.Contains(Combat.RangedWeaponType.Crossbow) != true)
						{
							return false;
						}

						return ammo.AmmoType.SpecificType.Equals(_rangedWeaponType.SpecificAmmunitionGrade,
							StringComparison.InvariantCultureIgnoreCase);
					}, null, 1, originalReference: "loaditem"),
					InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0,
						item => item.GetItemType<IRangedWeapon>()?.Prototype == this,
						null)
				})
			});
		}
	}

	public IWeaponType MeleeWeaponType { get; set; }
#nullable enable
	public IFutureProg? CanWieldProg { get; private set; }
	public IFutureProg? WhyCannotWieldProg { get; private set; }
#nullable restore
	public IInventoryPlanTemplate LoadTemplate { get; set; }

	#region Constructors

	protected CrossbowGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Crossbow")
	{
		RangedWeaponType =
			gameworld.RangedWeaponTypes.FirstOrDefault(x => x.RangedWeaponType == Combat.RangedWeaponType.Crossbow);
		MeleeWeaponType = gameworld.WeaponTypes.Get(gameworld.GetStaticLong("DefaultCrossbowMeleeWeaponType"));
	}

	protected CrossbowGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

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
			MeleeWeaponType = Gameworld.WeaponTypes.Get(Gameworld.GetStaticLong("DefaultCrossbowMeleeWeaponType"));
		}

		CanWieldProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanWieldProg")?.Value ?? "0"));
		WhyCannotWieldProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WhyCannotWieldProg")?.Value ?? "0"));
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
				new XElement("RangedWeaponType", RangedWeaponType?.Id ?? 0),
				new XElement("MeleeWeaponType", MeleeWeaponType?.Id ?? 0),
				new XElement("CanWieldProg", CanWieldProg?.Id ?? 0),
				new XElement("WhyCannotWieldProg", WhyCannotWieldProg?.Id ?? 0)
			).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new CrossbowGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new CrossbowGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Crossbow".ToLowerInvariant(), true,
			(gameworld, account) => new CrossbowGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Crossbow",
			(proto, gameworld) => new CrossbowGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Crossbow",
			$"Makes an item a {"[ranged weapon]".Colour(Telnet.BoldCyan)} with crossbow mechanics",
			$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tranged <ranged type> - sets the ranged weapon type for this component. See {"show ranges".FluentTagMXP("send", "href='show ranges'")} for a list."
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new CrossbowGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	public override string ShowBuildingHelp =>
		$@"You can use the following options:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3ranged <ranged type>#0 - sets the ranged weapon type for this component. See {"show ranges".FluentTagMXP("send", "href='show ranges'")} for a list.
	#3canwield <prog>#0 - sets a prog controlling if this can be wielded
	#3canwield none#0 - removes a canwield prog
	#3whycantwield <prog>#0 - sets a prog giving the error message if canwield fails
	#3whycantwield none#0 - clears the whycantwield prog";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "ranged":
			case "ranged type":
			case "rangedtype":
			case "type":
				return BuildingCommand_Type(actor, command);
			case "melee":
			case "meleetype":
			case "melee type":
			case "melee_type":
				return BuildingCommand_Melee(actor, command);
			case "canwield":
			case "canwieldprog":
				return BuildingCommandCanWieldProg(actor, command);
			case "whycantwield":
			case "whycantwieldprog":
			case "whycannotwield":
			case "whycannotwieldprog":
				return BuildingCommandWhyCannotWieldProg(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}
	private bool BuildingCommandCanWieldProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify a prog, or the keyword #3none#0 to remove one.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			CanWieldProg = null;
			Changed = true;
			actor.OutputHandler.Send($"This item will no longer use a prog to determine if it can be wielded.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
			[
				[ProgVariableTypes.Character],
				[ProgVariableTypes.Character, ProgVariableTypes.Item]
			]
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		CanWieldProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This item will now use the {prog.MXPClickableFunctionName()} prog to determine if it can be wielded.");
		return true;
	}

	private bool BuildingCommandWhyCannotWieldProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"You must either specify a prog, or the keyword #3none#0 to remove one.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			CanWieldProg = null;
			Changed = true;
			actor.OutputHandler.Send($"This item will no longer use a prog to generate an error message if it cannot be wielded.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Text,
			[
				[ProgVariableTypes.Character],
				[ProgVariableTypes.Character, ProgVariableTypes.Item]
			]
		).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WhyCannotWieldProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This item will now use the {prog.MXPClickableFunctionName()} prog to generate an error message if it cannot be wielded.");
		return true;
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

		if (type.RangedWeaponType != Combat.RangedWeaponType.Crossbow)
		{
			actor.Send("You can only give crossbows a ranged weapon type that is also for a crossbow.");
			return false;
		}

		RangedWeaponType = type;
		actor.Send(
			$"This ranged weapon will now be of type {RangedWeaponType.Name.TitleCase().Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(
			actor, "{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a crossbow of type {4} and melee type {5}.\nThe CanWield prog is {6} and the WhyCannotWield prog is {7}.",
			"Crossbow Weapon Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			RangedWeaponType?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red),
			MeleeWeaponType?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red),
			CanWieldProg?.MXPClickableFunctionName() ?? "None".ColourError(),
			WhyCannotWieldProg?.MXPClickableFunctionName() ?? "None".ColourError()
		);
	}


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