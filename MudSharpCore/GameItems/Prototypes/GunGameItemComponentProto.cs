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

public class GunGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Gun";

	public string LoadEmote { get; set; }

	public string ReadyEmote { get; set; }

	public string UnloadEmote { get; set; }

	public string UnreadyEmote { get; set; }

	public string UnreadyEmoteNoChamberedRound { get; set; }

	public string FireEmote { get; set; }
	public string FireEmoteNoChamberedRound { get; set; }

	public string ClipType { get; set; }

	#region Constructors

	protected GunGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "Gun")
	{
		LoadEmote = "@ insert|inserts $2 into $1 and it clicks into place.";
		ReadyEmote = "@ rack|racks the slide on $1, and it clicks back into place.";
		UnloadEmote = "@ hit|hits the eject button on $1 and $2 is ejected.";
		UnreadyEmote = "@ open|opens the slide on $1 and work|works out $2 from the chamber.";
		UnreadyEmoteNoChamberedRound = "@ open|opens the slide on $1, but there is no round in the chamber.";
		FireEmote = "@ squeeze|squeezes the trigger on $1 and it fires a round with an extremely loud bang.";
		FireEmoteNoChamberedRound = "@ squeeze|squeezes the trigger on $1, and nothing happens except a quiet click.";
		ClipType = Gameworld.GetStaticConfiguration("DefaultGunClipType");
		MeleeWeaponType = gameworld.WeaponTypes.Get(gameworld.GetStaticLong("DefaultGunMeleeWeaponType"));
	}

	protected GunGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		LoadEmote = root.Element("LoadEmote").Value;
		ReadyEmote = root.Element("ReadyEmote").Value;
		UnloadEmote = root.Element("UnloadEmote").Value;
		UnreadyEmote = root.Element("UnreadyEmote").Value;
		UnreadyEmoteNoChamberedRound = root.Element("UnreadyEmoteNoChamberedRound").Value;
		FireEmote = root.Element("FireEmote").Value;
		FireEmoteNoChamberedRound = root.Element("FireEmoteNoChamberedRound").Value;
		RangedWeaponType = Gameworld.RangedWeaponTypes.Get(long.Parse(root.Element("RangedWeaponType").Value));
		ClipType = root.Element("ClipType")?.Value ?? Gameworld.GetStaticConfiguration("DefaultGunClipType");
		var element = root.Element("MeleeWeaponType");
		if (element != null)
		{
			MeleeWeaponType = Gameworld.WeaponTypes.Get(long.Parse(element.Value));
		}
		else
		{
			MeleeWeaponType = Gameworld.WeaponTypes.Get(Gameworld.GetStaticLong("DefaultGunMeleeWeaponType"));
		}
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
			new XElement("ClipType", new XCData(ClipType)),
			new XElement("MeleeWeaponType", MeleeWeaponType?.Id ?? 0)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new GunGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new GunGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Gun".ToLowerInvariant(), true,
			(gameworld, account) => new GunGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Gun", (proto, gameworld) => new GunGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Gun",
			$"Makes an item a {"[ranged weapon]".Colour(Telnet.BoldCyan)} with removable-magazine semi-automatic gun mechanics",
			$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tranged <ranged type> - sets the ranged weapon type for this component. See {"show ranges".FluentTagMXP("send", "href='show ranges'")} for a list.\n\tload <emote> - sets the emote for loading this weapon. $0 is the loader, $1 is the gun, $2 is the clip.\n\tunload <emote> - sets the emote for unloading this weapon. $0 is the loader, $1 is the gun, $2 is the clip.\n\tready <emote> - sets the emote for readying this gun. $0 is the loader, $1 is the gun.\n\tunready <emote> - sets the emote for unreadying this gun. $0 is the loader, $1 is the gun and $2 is the chambered round.\n\tunreadyempty <emote> - sets the emote for unreadying this gun when there is no chambered round. $0 is the loader, $1 is the gun.\n\tfire <emote> - sets the emote for firing the gun. $0 is the firer, $1 is the target, $2 is the gun.\n\tfireempty <emote> - sets the emote for firing the gun when it is empty. $0 is the firer, $1 is the target, $2 is the gun.\n\tclip <type> - sets the type of clip that fits in this gun"
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new GunGameItemComponentProto(proto, gameworld));
	}

	#endregion

	public override string ShowBuildingHelp =>
		$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tranged <ranged type> - sets the ranged weapon type for this component. See {"show ranges".FluentTagMXP("send", "href='show ranges'")} for a list.\n\tload <emote> - sets the emote for loading this weapon. $0 is the loader, $1 is the gun, $2 is the clip.\n\tunload <emote> - sets the emote for unloading this weapon. $0 is the loader, $1 is the gun, $2 is the clip.\n\tready <emote> - sets the emote for readying this gun. $0 is the loader, $1 is the gun.\n\tunready <emote> - sets the emote for unreadying this gun. $0 is the loader, $1 is the gun and $2 is the chambered round.\n\tunreadyempty <emote> - sets the emote for unreadying this gun when there is no chambered round. $0 is the loader, $1 is the gun.\n\tfire <emote> - sets the emote for firing the gun. $0 is the firer, $1 is the target, $2 is the gun.\n\tfireempty <emote> - sets the emote for firing the gun when it is empty. $0 is the firer, $1 is the target, $2 is the gun.\n\tclip <type> - sets the type of clip that fits in this gun";

	#region Building Commands

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "ranged":
			case "ranged type":
			case "rangedtype":
			case "type":
				return BuildingCommandType(actor, command);
			case "load":
				return BuildingCommandLoadEmote(actor, command);
			case "unload":
				return BuildingCommandUnloadEmote(actor, command);
			case "ready":
				return BuildingCommandReadyEmote(actor, command);
			case "unready":
				return BuildingCommandUnreadyEmote(actor, command);
			case "unreadynoround":
			case "unreadynochamberedround":
			case "unreadynochambered":
			case "unreadyempty":
				return BuildingCommandUnreadyEmoteNoChamberedRound(actor, command);
			case "fire":
				return BuildingCommandFireEmote(actor, command);
			case "firenoround":
			case "firenochamberedround":
			case "firenochambered":
			case "fireempty":
				return BuildingCommandFireEmoteNoChamberedRound(actor, command);
			case "clip":
			case "cliptype":
			case "ammo":
				return BuildingCommandClip(actor, command);
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

	private bool BuildingCommandLoadEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people load a clip into this gun?");
			actor.Send("Hint: $0 is the loader, $1 is the gun, $2 is the clip.".Colour(Telnet.Yellow));
			return false;
		}

		LoadEmote = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send($"The following emote will now be used when this gun is loaded:\n\n{LoadEmote}\n");
		return true;
	}

	private bool BuildingCommandReadyEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people ready this gun?");
			actor.Send("Hint: $0 is the loader, $1 is the gun.".Colour(Telnet.Yellow));
			return false;
		}

		ReadyEmote = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send($"The following emote will now be used when this gun is readied:\n\n{ReadyEmote}\n");
		return true;
	}

	private bool BuildingCommandUnloadEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people unload a clip from this gun?");
			actor.Send("Hint: $0 is the loader, $1 is the gun, $2 is the clip.".Colour(Telnet.Yellow));
			return false;
		}

		UnloadEmote = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send($"The following emote will now be used when this gun is unloaded:\n\n{UnloadEmote}\n");
		return true;
	}

	private bool BuildingCommandUnreadyEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people empty the chamber on this gun?");
			actor.Send("Hint: $0 is the loader, $1 is the gun, $2 is the round in the chamber.".Colour(Telnet.Yellow));
			return false;
		}

		UnreadyEmote = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send(
			$"The following emote will now be used when the chamber of this gun is emptied:\n\n{UnreadyEmote}\n");
		return true;
	}

	private bool BuildingCommandUnreadyEmoteNoChamberedRound(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set for when people empty the chamber, but there is no chambered round?");
			actor.Send("Hint: $0 is the loader, $1 is the gun.".Colour(Telnet.Yellow));
			return false;
		}

		UnreadyEmoteNoChamberedRound = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send(
			$"The following emote will now be used when the chambered of this gun is emptied when already empty:\n\n{UnreadyEmoteNoChamberedRound}\n");
		return true;
	}

	private bool BuildingCommandFireEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people fire this gun?");
			actor.Send("Hint: $0 is the loader, $1 is the target, $2 is the gun.".Colour(Telnet.Yellow));
			return false;
		}

		FireEmote = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send($"The following emote will now be used when this gun is fired:\n\n{FireEmote}\n");
		return true;
	}

	private bool BuildingCommandFireEmoteNoChamberedRound(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people fire the gun while the chamber is empty?");
			actor.Send("Hint: $0 is the loader, $1 is the target, $2 is the gun.".Colour(Telnet.Yellow));
			return false;
		}

		FireEmoteNoChamberedRound = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send(
			$"The following emote will now be used when this gun is fired while empty:\n\n{FireEmoteNoChamberedRound}\n");
		return true;
	}

	private bool BuildingCommandClip(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What specific clip type should this gun use?");
			return false;
		}

		ClipType = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This gun will now use clips of the specific type {ClipType.Colour(Telnet.Green)}.");
		return true;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"What Ranged Weapon Type do you want to use for this gun? See {"show ranged".Colour(Telnet.Yellow)} for a list of ranged weapon types.");
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

		if (type.RangedWeaponType != Combat.RangedWeaponType.ModernFirearm)
		{
			actor.Send("You can only give modern firearms a ranged weapon type that is suitable for them.");
			return false;
		}

		RangedWeaponType = type;
		actor.Send(
			$"This gun will now be of type {RangedWeaponType.Name.TitleCase().Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	#endregion

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
						var ammo = item.GetItemType<IAmmoClip>();
						if (ammo == null)
						{
							return false;
						}

						if (ammo.ClipType != ClipType ||
						    ammo.SpecificAmmoGrade != _rangedWeaponType.SpecificAmmunitionGrade)
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
						var ammo = item.GetItemType<IAmmoClip>();
						if (ammo == null)
						{
							return false;
						}

						if (ammo.ClipType != ClipType ||
						    ammo.SpecificAmmoGrade != _rangedWeaponType.SpecificAmmunitionGrade)
						{
							return false;
						}

						if (!ammo.Contents.Any(x =>
							    x.GetItemType<IAmmo>()?.AmmoType.SpecificType
							     .EqualTo(_rangedWeaponType.SpecificAmmunitionGrade) ?? false))
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

	public IWeaponType MeleeWeaponType { get; set; }

	public IInventoryPlanTemplate LoadTemplate { get; set; }

	public IInventoryPlanTemplate LoadTemplateIgnoreEmpty { get; set; }

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(
			actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis is a modern firearm of type {4} and melee type {13}.\n\nFire: {5}\nFireEmpty: {6}\nLoad: {7}\nUnload: {8}\nReady: {9}\nUnready: {10}\nUnreadyEmpty: {11}\nClip Type: {12}",
			"Gun Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			RangedWeaponType?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red),
			FireEmote.Colour(Telnet.Cyan),
			FireEmoteNoChamberedRound.Colour(Telnet.Cyan),
			LoadEmote.Colour(Telnet.Cyan),
			UnloadEmote.Colour(Telnet.Cyan),
			ReadyEmote.Colour(Telnet.Cyan),
			UnreadyEmote.Colour(Telnet.Cyan),
			UnreadyEmoteNoChamberedRound.Colour(Telnet.Cyan),
			ClipType.Colour(Telnet.Green),
			MeleeWeaponType?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)
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