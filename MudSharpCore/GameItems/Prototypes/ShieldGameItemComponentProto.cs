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

public class ShieldGameItemComponentProto : GameItemComponentProto
{
	protected ShieldGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Shield")
	{
		ShieldType = gameworld.ShieldTypes.FirstOrDefault();
		MeleeWeaponType = gameworld.WeaponTypes.Get(gameworld.GetStaticLong("DefaultShieldMeleeWeaponType"));
	}

	protected ShieldGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public IShieldType ShieldType { get; set; }
	public IWeaponType MeleeWeaponType { get; set; }

	public override string TypeDescription => "Shield";

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("ShieldType");
		if (element != null)
		{
			ShieldType = Gameworld.ShieldTypes.Get(long.Parse(element.Value));
		}

		element = root.Element("MeleeWeaponType");
		if (element != null)
		{
			MeleeWeaponType = Gameworld.WeaponTypes.Get(long.Parse(element.Value));
		}
		else
		{
			MeleeWeaponType = Gameworld.WeaponTypes.Get(Gameworld.GetStaticLong("DefaultShieldMeleeWeaponType"));
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a shield of type {4} and melee weapon type {5}.",
			"Shield Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			ShieldType?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red),
			MeleeWeaponType?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)
		);
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
				new XElement("ShieldType", ShieldType?.Id ?? 0),
				new XElement("MeleeWeaponType", MeleeWeaponType?.Id ?? 0)
			).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("shield", true,
			(gameworld, account) => new ShieldGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Shield", (proto, gameworld) => new ShieldGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Shield",
			$"Item becomes a {"[shield]".Colour(Telnet.BoldCyan)} and {"[melee weapon]".Colour(Telnet.BoldCyan)}",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ShieldGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ShieldGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ShieldGameItemComponentProto(proto, gameworld));
	}

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\ttype <type> - sets the shield type this component is for\n\tmelee <which> - sets the melee weapon type";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
			case "shield":
			case "shieldtype":
			case "shield type":
			case "shield_type":
				return BuildingCommand_Type(actor, command);
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

	private bool BuildingCommand_Type(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which shield type do you want to set for this shield component?");
			return false;
		}

		var type = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.ShieldTypes.Get(value)
			: actor.Gameworld.ShieldTypes.GetByName(command.Last);
		if (type == null)
		{
			actor.Send("There is no such shield type.");
			return false;
		}

		ShieldType = type;
		actor.Send($"This shield will now be of type {ShieldType.Name.TitleCase().Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	#endregion

	#region Overrides of EditableItem

	public override bool CanSubmit()
	{
		return ShieldType != null &&
		       MeleeWeaponType != null &&
		       base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (ShieldType == null)
		{
			return "You must first give this component a Shield Type.";
		}

		if (MeleeWeaponType == null)
		{
			return "You must first give this component a Melee Type";
		}

		return base.WhyCannotSubmit();
	}

	#endregion
}