using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class LaserGameItemComponentProto : GameItemComponentProto
{
	private IRangedWeaponType _rangedRangedWeaponType;

	public IRangedWeaponType RangedWeaponType
	{
		get => _rangedRangedWeaponType;
		set
		{
			_rangedRangedWeaponType = value;
			LoadTemplate = new InventoryPlanTemplate(Gameworld, new[]
			{
				new InventoryPlanPhaseTemplate(1, new[]
				{
					InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0, item =>
					{
						var ammo = item.GetItemType<ILaserPowerPack>();
						if (ammo == null)
						{
							return false;
						}

						if (ammo.ClipType != ClipType)
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
						var ammo = item.GetItemType<ILaserPowerPack>();
						if (ammo == null)
						{
							return false;
						}

						if (ammo.ClipType != ClipType)
						{
							return false;
						}

						if (!ammo.CanDraw(WattsPerShot))
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

	public string LoadEmote { get; protected set; }

	public string ReadyEmote { get; protected set; }

	public string UnloadEmote { get; protected set; }

	public string UnreadyEmote { get; protected set; }

	public string FireEmote { get; protected set; }

	public string FireEmoteNoAmmo { get; protected set; }

	public string ClipType { get; protected set; }

	public AudioVolume FireVolume { get; protected set; }

	public double WattsPerShot { get; protected set; }

	public double PainMultiplier { get; protected set; }

	public double StunMultiplier { get; protected set; }

	public override string TypeDescription => "Laser";

	#region Constructors

	protected LaserGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Laser")
	{
		LoadEmote = "@ insert|inserts $2 into $1 and it clicks into place.";
		ReadyEmote = "@ press|presses the prime shot button on $1.";
		UnloadEmote = "@ hit|hits the eject button on $1 and $2 is ejected.";
		UnreadyEmote = "@ press|presses the flush shot button on $1.";
		FireEmote = "@ squeeze|squeezes the trigger on $2 and it shoots out a beam of red light at $1.";
		FireEmoteNoAmmo =
			"@ squeeze|squeezes the trigger on $1, and nothing happens except a blinking red status indicator.";
		ClipType = Gameworld.GetStaticConfiguration("DefaultLaserClipType");
		WattsPerShot = 10000;
		FireVolume = AudioVolume.Silent;
		PainMultiplier = 1.0;
		StunMultiplier = 0.0;
		MeleeWeaponType = gameworld.WeaponTypes.Get(gameworld.GetStaticLong("DefaultGunMeleeWeaponType"));
	}

	protected LaserGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		LoadEmote = root.Element("LoadEmote").Value;
		ReadyEmote = root.Element("ReadyEmote").Value;
		UnloadEmote = root.Element("UnloadEmote").Value;
		UnreadyEmote = root.Element("UnreadyEmote").Value;
		FireEmote = root.Element("FireEmote").Value;
		FireEmoteNoAmmo = root.Element("FireEmoteNoAmmo").Value;
		WattsPerShot = double.Parse(root.Element("WattsPerShot").Value);
		PainMultiplier = double.Parse(root.Element("PainMultiplier").Value);
		StunMultiplier = double.Parse(root.Element("StunMultiplier").Value);
		RangedWeaponType = Gameworld.RangedWeaponTypes.Get(long.Parse(root.Element("RangedWeaponType").Value));
		ClipType = root.Element("ClipType")?.Value ?? Gameworld.GetStaticConfiguration("DefaultLaserClipType");
		FireVolume = (AudioVolume)int.Parse(root.Element("FireVolume").Value);
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
			new XElement("FireEmote", new XCData(FireEmote)),
			new XElement("FireEmoteNoAmmo", new XCData(FireEmoteNoAmmo)),
			new XElement("RangedWeaponType", RangedWeaponType?.Id ?? 0),
			new XElement("ClipType", new XCData(ClipType)),
			new XElement("FireVolume", (int)FireVolume),
			new XElement("WattsPerShot", WattsPerShot),
			new XElement("PainMultiplier", PainMultiplier),
			new XElement("StunMultiplier", StunMultiplier),
			new XElement("MeleeWeaponType", MeleeWeaponType?.Id ?? 0)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new LaserGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new LaserGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Laser".ToLowerInvariant(), true,
			(gameworld, account) => new LaserGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Laser", (proto, gameworld) => new LaserGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Laser",
			$"Makes an item a {"[ranged weapon]".Colour(Telnet.BoldCyan)} with laser gun mechanics",
			$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tranged <ranged type> - sets the ranged weapon type for this component. See {"show ranges".FluentTagMXP("send", "href='show ranges'")} for a list.\n\tload <emote> - sets the emote for loading this weapon. $0 is the loader, $1 is the laser, $2 is the clip.\n\tunload <emote> - sets the emote for unloading this weapon. $0 is the loader, $1 is the laser, $2 is the clip.\n\tready <emote> - sets the emote for readying this laser. $0 is the loader, $1 is the laser.\n\tunready <emote> - sets the emote for unreadying this laser. $0 is the loader, $1 is the laser.\n\tfire <emote> - sets the emote for firing the laser. $0 is the firer, $1 is the target, $2 is the laser.\n\tfireempty <emote> - sets the emote for firing the laser when it is empty. $0 is the firer, $1 is the target, $2 is the laser.\n\tvolume <volume> - sets the volume when fired\n\tclip <type> - sets the type of power pack used by this laser\n\twatts <amount> - sets the number of watt-seconds drawn down from the battery pack per shot\n\tpain <multiplier> - a multiplier for pain compared to damage\n\tstun <multiplier> - a multiplier for stun compared to damage"
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new LaserGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	public override string ShowBuildingHelp =>
		$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tranged <ranged type> - sets the ranged weapon type for this component. See {"show ranges".FluentTagMXP("send", "href='show ranges'")} for a list.\n\tload <emote> - sets the emote for loading this weapon. $0 is the loader, $1 is the laser, $2 is the clip.\n\tunload <emote> - sets the emote for unloading this weapon. $0 is the loader, $1 is the laser, $2 is the clip.\n\tready <emote> - sets the emote for readying this laser. $0 is the loader, $1 is the laser.\n\tunready <emote> - sets the emote for unreadying this laser. $0 is the loader, $1 is the laser.\n\tfire <emote> - sets the emote for firing the laser. $0 is the firer, $1 is the target, $2 is the laser.\n\tfireempty <emote> - sets the emote for firing the laser when it is empty. $0 is the firer, $1 is the target, $2 is the laser.\n\tvolume <volume> - sets the volume when fired\n\tclip <type> - sets the type of power pack used by this laser\n\twatts <amount> - sets the number of watt-seconds drawn down from the battery pack per shot\n\tpain <multiplier> - a multiplier for pain compared to damage\n\tstun <multiplier> - a multiplier for stun compared to damage";

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
			case "fire":
				return BuildingCommandFireEmote(actor, command);
			case "firenoround":
			case "firenochamberedround":
			case "firenochambered":
			case "fireempty":
				return BuildingCommandFireEmoteNoChamberedRound(actor, command);
			case "noise":
			case "volume":
				return BuildingCommandVolume(actor, command);
			case "clip":
			case "cliptype":
			case "ammo":
				return BuildingCommandClip(actor, command);
			case "watts":
			case "watt":
				return BuildingCommandWatts(actor, command);
			case "pain":
				return BuildingCommandPain(actor, command);
			case "stun":
				return BuildingCommandStun(actor, command);
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

	private bool BuildingCommandPain(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What multiplier should be applied to the base damage when calculating the pain of being shot with this laser?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.Send("You must enter a valid number for the multiplier.");
			return false;
		}

		PainMultiplier = value;
		Changed = true;
		actor.Send($"The pain caused by this laser is now {PainMultiplier:P2} of the damage.");
		return true;
	}

	private bool BuildingCommandStun(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What multiplier should be applied to the base damage when calculating the stun of being shot with this laser?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.Send("You must enter a valid number for the multiplier.");
			return false;
		}

		StunMultiplier = value;
		Changed = true;
		actor.Send($"The stun caused by this laser is now {StunMultiplier:P2} of the damage.");
		return true;
	}

	private bool BuildingCommandWatts(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"How many watts should this laser drawdown from its power pack per shot? Note that it is technically a watt-second.");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.Send(
				"You must enter a valid number of watts for this laser to drawdown from its power pack per shot.");
			return false;
		}

		WattsPerShot = value;
		Changed = true;
		actor.Send($"This laser now draws down {WattsPerShot} watts while firing.");
		return true;
	}

	private bool BuildingCommandClip(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What specific power pack type should this laser use?");
			return false;
		}

		ClipType = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This laser will now use power pack of the specific type {ClipType.Colour(Telnet.Green)}.");
		return true;
	}

	private bool BuildingCommandVolume(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What volume do you want to set for his laser when it fires? See SHOW VOLUMES for more info.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<AudioVolume>(out var value))
		{
			actor.Send("That is not a valid volume. See SHOW VOLUMES for more info.");
			return false;
		}

		FireVolume = value;
		Changed = true;
		actor.Send($"This laser is now {FireVolume.Describe().Colour(Telnet.Green)} when fired.");
		return true;
	}

	private bool BuildingCommandLoadEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people load a clip into this laser?");
			actor.Send("Hint: $0 is the loader, $1 is the laser, $2 is the clip.".Colour(Telnet.Yellow));
			return false;
		}

		LoadEmote = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send($"The following emote will now be used when this laser is loaded:\n\n{LoadEmote}\n");
		return true;
	}

	private bool BuildingCommandReadyEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people ready this laser?");
			actor.Send("Hint: $0 is the loader, $1 is the laser.".Colour(Telnet.Yellow));
			return false;
		}

		ReadyEmote = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send($"The following emote will now be used when this laser is readied:\n\n{ReadyEmote}\n");
		return true;
	}

	private bool BuildingCommandUnloadEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people unload a clip from this laser?");
			actor.Send("Hint: $0 is the loader, $1 is the laser, $2 is the clip.".Colour(Telnet.Yellow));
			return false;
		}

		UnloadEmote = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send($"The following emote will now be used when this laser is unloaded:\n\n{UnloadEmote}\n");
		return true;
	}

	private bool BuildingCommandUnreadyEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people unready this laser?");
			actor.Send("Hint: $0 is the loader, $1 is the laser.".Colour(Telnet.Yellow));
			return false;
		}

		UnreadyEmote = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send(
			$"The following emote will now be used when the chamber of this laser is emptied:\n\n{UnreadyEmote}\n");
		return true;
	}

	private bool BuildingCommandFireEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people fire this laser?");
			actor.Send("Hint: $0 is the loader, $1 is the target, $2 is the laser.".Colour(Telnet.Yellow));
			return false;
		}

		FireEmote = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send($"The following emote will now be used when this laser is fired:\n\n{FireEmote}\n");
		return true;
	}

	private bool BuildingCommandFireEmoteNoChamberedRound(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for when people fire the laser while the ammo is empty?");
			actor.Send("Hint: $0 is the loader, $1 is the target, $2 is the laser.".Colour(Telnet.Yellow));
			return false;
		}

		FireEmoteNoAmmo = command.RemainingArgument.Fullstop();
		Changed = true;
		actor.Send(
			$"The following emote will now be used when this laser is fired while empty:\n\n{FireEmoteNoAmmo}\n");
		return true;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"What Ranged Weapon Type do you want to use for this laser? See {"show ranged".Colour(Telnet.Yellow)} for a list of ranged weapon types.");
			return false;
		}

		var type = long.TryParse(command.SafeRemainingArgument, out var value)
			? actor.Gameworld.RangedWeaponTypes.Get(value)
			: actor.Gameworld.RangedWeaponTypes.GetByName(command.SafeRemainingArgument);
		if (type == null)
		{
			actor.Send("There is no such ranged weapon type.");
			return false;
		}

		if (type.RangedWeaponType != Combat.RangedWeaponType.Laser)
		{
			actor.Send("You can only give lasers a ranged weapon type that is suitable for them.");
			return false;
		}

		RangedWeaponType = type;
		actor.Send(
			$"This laser will now be of type {RangedWeaponType.Name.TitleCase().Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis is a laser of type {4} and melee type {16}.\n\nFire: {5}\nFireEmpty: {6}\nLoad: {7}\nUnload: {8}\nReady: {9}\nUnready: {10}\nVolume: {11}\nClip Type: {12}\nWatts per Shot: {13}\nPain Multiplier: {14:P2}\nStun Multiplier: {15:P2}",
			"Laser Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			RangedWeaponType?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red),
			FireEmote.Colour(Telnet.Cyan),
			FireEmoteNoAmmo.Colour(Telnet.Cyan),
			LoadEmote.Colour(Telnet.Cyan),
			UnloadEmote.Colour(Telnet.Cyan),
			ReadyEmote.Colour(Telnet.Cyan),
			UnreadyEmote.Colour(Telnet.Cyan),
			FireVolume.Describe().Colour(Telnet.Green),
			ClipType.Colour(Telnet.Green),
			WattsPerShot,
			PainMultiplier,
			StunMultiplier,
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