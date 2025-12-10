using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class InternalMagazineGunGameItemComponentProto : FirearmBaseGameItemComponentProto
{
	public override string TypeDescription => "InternalMagazineGun";

	public bool EjectOnFire { get; set; }

	public int InternalMagazineCapacity { get; set; }

	#region Constructors

	protected InternalMagazineGunGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "InternalMagazineGun")
	{
		LoadEmote = "@ insert|inserts $2 into $1 and it clicks into place.";
		ReadyEmote = "@ rack|racks the slide on $1, and it clicks back into place.";
		UnloadEmote = "@ hit|hits the eject button on $1 and $2 is ejected.";
		UnreadyEmote = "@ open|opens the slide on $1 and work|works out $2 from the chamber.";
		UnreadyEmoteNoChamberedRound = "@ open|opens the slide on $1, but there is no round in the chamber.";
		FireEmote = "@ squeeze|squeezes the trigger on $1 and it fires a round with an extremely loud bang.";
		FireEmoteNoChamberedRound = "@ squeeze|squeezes the trigger on $1, and nothing happens except a quiet click.";
		InternalMagazineCapacity = 5;
		EjectOnFire = false;
		MeleeWeaponType = gameworld.WeaponTypes.Get(gameworld.GetStaticLong("DefaultGunMeleeWeaponType"));
	}

	protected InternalMagazineGunGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		var element = root.Element("MeleeWeaponType");
		if (element != null)
		{
			MeleeWeaponType = Gameworld.WeaponTypes.Get(long.Parse(element.Value));
		}
		else
		{
			MeleeWeaponType = Gameworld.WeaponTypes.Get(Gameworld.GetStaticLong("DefaultGunMeleeWeaponType"));
		}

		InternalMagazineCapacity = int.Parse(root.Element("InternalMagazineCapacity").Value);
		RecalculateInventoryPlans();
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("LoadEmote", new XCData(LoadEmote)),
			new XElement("ReadyEmote", new XCData(ReadyEmote)),
			new XElement("UnloadEmote", new XCData(UnloadEmote)),
			new XElement("UnreadyEmote", new XCData(UnreadyEmote)),
			new XElement("UnreadyEmoteNoChamberedRound", new XCData(UnreadyEmoteNoChamberedRound)),
			new XElement("FireEmote", new XCData(FireEmote)),
			new XElement("FireEmoteNoChamberedRound", new XCData(FireEmoteNoChamberedRound)),
			new XElement("RangedWeaponType", RangedWeaponType?.Id ?? 0),
			new XElement("EjectOnFire", EjectOnFire),
			new XElement("MeleeWeaponType", MeleeWeaponType?.Id ?? 0),
			new XElement("InternalMagazineCapacity", InternalMagazineCapacity),
			new XElement("CanWieldProg", CanWieldProg?.Id ?? 0),
			new XElement("WhyCannotWieldProg", WhyCannotWieldProg?.Id ?? 0)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new InternalMagazineGunGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new InternalMagazineGunGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("internalmagazinegun", true,
			(gameworld, account) => new InternalMagazineGunGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("imgun", false,
			(gameworld, account) => new InternalMagazineGunGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("internal magazine gun", false,
			(gameworld, account) => new InternalMagazineGunGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("internal mag gun", false,
			(gameworld, account) => new InternalMagazineGunGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("internal_magazine_gun", false,
			(gameworld, account) => new InternalMagazineGunGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("internalmaggun", false,
			(gameworld, account) => new InternalMagazineGunGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("InternalMagazineGun",
			(proto, gameworld) => new InternalMagazineGunGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"InternalMagazineGun",
			$"Makes an item a {"[ranged weapon]".Colour(Telnet.BoldCyan)} with internal-magazine semi-automatic gun mechanics",
			$@"{BuildingHelpText}
	capacity <number> - sets the internal magazine capacity
	ejectonfire - toggles whether casings are ejected on fire or on ready"
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new InternalMagazineGunGameItemComponentProto(proto, gameworld));
	}

	#endregion

	public override string ShowBuildingHelp =>
		$@"{BuildingHelpText}
	#3capacity <number>#0 - sets the internal magazine capacity
	#3ejectonfire#0 - toggles whether casings are ejected on fire or on ready";

	#region Building Commands

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "ejectonfire":
				return BuildingCommandEjectOnFire(actor, command);
			case "capacity":
			case "quantity":
			case "amount":
				return BuildingCommandCapacity(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the capacity of the internal magazine?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value <= 0)
		{
			actor.OutputHandler.Send(
				"You must enter a valid number greater than zero for the internal magazine capacity.");
			return false;
		}

		InternalMagazineCapacity = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This gun now has an internal magazine capacity of {InternalMagazineCapacity.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEjectOnFire(ICharacter actor, StringStack command)
	{
		if (EjectOnFire)
		{
			EjectOnFire = false;
			actor.Send("This component will eject casings when readied.");
		}
		else
		{
			EjectOnFire = true;
			actor.Send("This component will eject casings when fired.");
		}

		Changed = true;
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis is an internal-magazine bolt-action firearm of type {4} and melee type {13}.\nIt will {5}, and has an internal magazine capacity of {14}.\nThe CanWield prog is {15} and the WhyCannotWield prog is {16}.\n\nFire: {6}\nFireEmpty: {7}\nLoad: {8}\nUnload: {9}\nReady: {10}\nUnready: {11}\nUnreadyEmpty: {12}",
			"Internal Magazine Gun Game Item Component".Colour(Telnet.Cyan),
			Id.ToString("N0", actor),
			RevisionNumber.ToString("N0", actor),
			Name,
			RangedWeaponType?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red),
			EjectOnFire ? "eject casings when fired" : "eject casings when readied",
			FireEmote.Colour(Telnet.Cyan),
			FireEmoteNoChamberedRound.Colour(Telnet.Cyan),
			LoadEmote.Colour(Telnet.Cyan),
			UnloadEmote.Colour(Telnet.Cyan),
			ReadyEmote.Colour(Telnet.Cyan),
			UnreadyEmote.Colour(Telnet.Cyan),
			UnreadyEmoteNoChamberedRound.Colour(Telnet.Cyan),
			MeleeWeaponType?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red),
			InternalMagazineCapacity.ToString("N0", actor).ColourValue(),
			CanWieldProg?.MXPClickableFunctionName() ?? "None".ColourError(),
			WhyCannotWieldProg?.MXPClickableFunctionName() ?? "None".ColourError()
		);
	}

	protected override void RecalculateInventoryPlans()
	{
		LoadTemplate = new InventoryPlanTemplate(Gameworld, new[]
		{
			new InventoryPlanPhaseTemplate(1, new[]
			{
				InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0, item =>
				{
					var ammo = item.GetItemType<IAmmo>();
					if (ammo == null)
					{
						return false;
					}

					if (ammo.AmmoType.SpecificType != RangedWeaponType.SpecificAmmunitionGrade)
					{
						return false;
					}

					return true;
				}, null, 1, originalReference: "loaditem"),
				InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0,
					item => item.GetItemType<IRangedWeapon>()?.Prototype == this, null)
			})
		});

		LoadTemplateIgnoreEmpty = new InventoryPlanTemplate(Gameworld, new[]
		{
			new InventoryPlanPhaseTemplate(1, new[]
			{
				InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0, item =>
				{
					var ammo = item.GetItemType<IAmmo>();
					if (ammo == null)
					{
						return false;
					}

					if (ammo.AmmoType.SpecificType != RangedWeaponType.SpecificAmmunitionGrade)
					{
						return false;
					}

					return true;
				}, null, 1, originalReference: "loaditem"),
				InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0,
					item => item.GetItemType<IRangedWeapon>()?.Prototype == this, null)
			})
		});
	}
}