using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class MeleeWeaponGameItemComponentProto : GameItemComponentProto
{
	protected MeleeWeaponGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "MeleeWeapon")
	{
	}

	protected MeleeWeaponGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public IWeaponType WeaponType { get; set; }
	public override string TypeDescription => "MeleeWeapon";

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("WeaponType");
		if (element != null)
		{
			WeaponType = Gameworld.WeaponTypes.Get(long.Parse(element.Value));
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a melee weapon of type {4}.",
			"Melee Weapon Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			WeaponType?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)
		);
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
				new XElement("WeaponType", WeaponType?.Id ?? 0)
			).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("weapon", true,
			(gameworld, account) => new MeleeWeaponGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("melee weapon", false,
			(gameworld, account) => new MeleeWeaponGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("meleeweapon", false,
			(gameworld, account) => new MeleeWeaponGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("melee", false,
			(gameworld, account) => new MeleeWeaponGameItemComponentProto(gameworld, account));

		manager.AddDatabaseLoader("MeleeWeapon",
			(proto, gameworld) => new MeleeWeaponGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"MeleeWeapon",
			$"Turns an item into a {"[melee weapon]".Colour(Telnet.BoldGreen)}",
			$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tmelee <melee type> - sets the melee weapon type for this component. See {"show weapons".FluentTagMXP("send", "href='show weapons'")} for a list."
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new MeleeWeaponGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new MeleeWeaponGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new MeleeWeaponGameItemComponentProto(proto, gameworld));
	}

	#region Building Commands

	public override string ShowBuildingHelp =>
		$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tmelee <melee type> - sets the melee weapon type for this component. See {"show weapons".FluentTagMXP("send", "href='show weapons'")} for a list.";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
			case "melee":
				return BuildingCommand_Type(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommand_Type(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which weapon type do you want to set for this melee weapon?");
			return false;
		}

		var type = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.WeaponTypes.Get(value)
			: actor.Gameworld.WeaponTypes.GetByName(command.Last);
		if (type == null)
		{
			actor.Send("There is no such weapon type.");
			return false;
		}

		WeaponType = type;
		actor.Send($"This melee weapon will now be of type {WeaponType.Name.TitleCase().Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	#endregion

	#region Overrides of EditableItem

	public override bool CanSubmit()
	{
		return WeaponType != null && base.CanSubmit();
	}

	#region Overrides of EditableItem

	public override string WhyCannotSubmit()
	{
		return WeaponType == null ? "You must first give this component a Weapon Type." : base.WhyCannotSubmit();
	}

	#endregion

	#endregion
}