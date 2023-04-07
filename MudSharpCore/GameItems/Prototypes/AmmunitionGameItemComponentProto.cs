using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class AmmunitionGameItemComponentProto : GameItemComponentProto
{
	protected AmmunitionGameItemComponentProto(IFuturemud gameworld, IAccount originator, string type = "Ammunition")
		: base(gameworld, originator, type)
	{
	}

	protected AmmunitionGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public IAmmunitionType AmmoType { get; set; }

	private long _casingProtoId;
	protected long CasingProtoId => _casingProtoId;
	public IGameItemProto CasingProto => Gameworld.ItemProtos.Get(_casingProtoId);

	private long _bulletProtoId;
	protected long BulletProtoId => _bulletProtoId;
	public IGameItemProto BulletProto => Gameworld.ItemProtos.Get(_bulletProtoId);

	public override string TypeDescription => "Ammunition";

	protected override void LoadFromXml(XElement root)
	{
		AmmoType = Gameworld.AmmunitionTypes.Get(long.Parse(root.Element("AmmoType").Value));
		_casingProtoId = long.Parse(root.Element("CasingProto")?.Value ?? "0");
		_bulletProtoId = long.Parse(root.Element("BulletProto")?.Value ?? "0");
	}

	public override string ShowBuildingHelp =>
		$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tammo <ammo>- sets the ammo grade, for example, {"9x19mm Parabellum".Colour(Telnet.Green)}.\n\tbullet <proto> - sets the bullet proto for this round, which will be loaded when fired.\n\tshell <proto> - sets the shell proto for this round, which will be loaded when fired.";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
			case "ammo":
			case "ammunition":
				return BuildingCommandAmmunition(actor, command);
			case "bullet":
				return BuildingCommandBullet(actor, command);
			case "casing":
			case "shell":
			case "shells":
			case "shell casings":
			case "casings":
				return BuildingCommandCasing(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis is ammunition of type {4}.{5}",
			"Ammunition Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			AmmoType != null ? $"{AmmoType.Name} (#{AmmoType.Id:N0})" : "None".Colour(Telnet.Red),
			AmmoType?.RangedWeaponTypes.Contains(RangedWeaponType.ModernFirearm) ?? false
				? $"\nWhen fired, it loads prototype {(CasingProto != null ? _casingProtoId.ToString(actor).Colour(Telnet.Green) : "None".Colour(Telnet.Red))} as a shell casing and {(BulletProto != null ? _bulletProtoId.ToString(actor).Colour(Telnet.Green) : "None".Colour(Telnet.Red))} as a bullet."
				: ""
		);
	}

	public override bool CanSubmit()
	{
		return AmmoType != null && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (AmmoType == null)
		{
			return "You must first give this item an ammo type.";
		}

		return base.WhyCannotSubmit();
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("AmmoType", AmmoType?.Id ?? 0),
			new XElement("CasingProto", CasingProto?.Id ?? 0),
			new XElement("BulletProto", BulletProto?.Id ?? 0)
		).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ammo", true,
			(gameworld, account) => new AmmunitionGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("ammunition", false,
			(gameworld, account) => new AmmunitionGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Ammunition",
			(proto, gameworld) => new AmmunitionGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Ammunition",
			"Turns an item into a round fired from a gun",
			$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tammo <ammo>- sets the ammo grade, for example, {"9x19mm Parabellum".Colour(Telnet.Green)}.\n\tbullet <proto> - sets the bullet proto for this round, which will be loaded when fired.\n\tshell <proto> - sets the shell proto for this round, which will be loaded when fired."
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new AmmunitionGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new AmmunitionGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new AmmunitionGameItemComponentProto(proto, gameworld));
	}

	#region Building Commands

	private bool BuildingCommandCasing(ICharacter actor, StringStack command)
	{
		if (AmmoType?.RangedWeaponTypes.Contains(RangedWeaponType.ModernFirearm) != true)
		{
			actor.Send(
				"You must set the ammunition type to one that uses modern firearms before you can set a shell casing prototype.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send("What item prototype do you want to use for the shell casing when this round is fired?");
			return false;
		}

		var proto = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.ItemProtos.Get(value)
			: Gameworld.ItemProtos.GetByName(command.SafeRemainingArgument);
		if (proto == null)
		{
			actor.Send("There is no such proto.");
			return false;
		}

		_casingProtoId = proto.Id;
		Changed = true;
		actor.Send(
			$"This ammunition will now produce the shell casings from item proto {_casingProtoId.ToString(actor).Colour(Telnet.Cyan)} ({proto.Name.Colour(Telnet.Cyan)}).");
		return true;
	}

	private bool BuildingCommandBullet(ICharacter actor, StringStack command)
	{
		if (AmmoType?.RangedWeaponTypes.Contains(RangedWeaponType.ModernFirearm) != true)
		{
			actor.Send(
				"You must set the ammunition type to one that uses modern firearms before you can set a bullet prototype.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send("What item prototype do you want to use for the bullet when this round is fired?");
			return false;
		}

		var proto = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.ItemProtos.Get(value)
			: Gameworld.ItemProtos.GetByName(command.SafeRemainingArgument);
		if (proto == null)
		{
			actor.Send("There is no such proto.");
			return false;
		}

		_bulletProtoId = proto.Id;
		Changed = true;
		actor.Send(
			$"This ammunition will now produce the bullets from item proto {_bulletProtoId.ToString(actor).Colour(Telnet.Cyan)} ({proto.Name.Colour(Telnet.Cyan)}).");
		return true;
	}

	private bool BuildingCommandAmmunition(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What ammunition type do you want this component to be for?");
			return false;
		}

		var ammo = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.AmmunitionTypes.Get(value)
			: Gameworld.AmmunitionTypes.GetByName(command.Last);
		if (ammo == null)
		{
			actor.Send("There is no such ammunition type.");
			return false;
		}

		AmmoType = ammo;
		Changed = true;
		actor.Send($"This ammunition is now of type {AmmoType.Name.Colour(Telnet.Cyan)}.");
		return true;
	}

	#endregion
}