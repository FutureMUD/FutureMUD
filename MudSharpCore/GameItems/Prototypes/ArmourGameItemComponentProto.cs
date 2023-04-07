using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class ArmourGameItemComponentProto : GameItemComponentProto
{
	protected ArmourGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Armour")
	{
	}

	protected ArmourGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public IArmourType ArmourType { get; set; }
	public override string TypeDescription => "Armour";

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("ArmourType");
		if (element != null)
		{
			ArmourType = Gameworld.ArmourTypes.Get(long.Parse(element.Value));
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is armour of type {4}.",
			"Armour Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			ArmourType?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)
		);
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
				new XElement("ArmourType", ArmourType?.Id ?? 0)
			).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("armour", true,
			(gameworld, account) => new ArmourGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("armor", false,
			(gameworld, account) => new ArmourGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Armour", (proto, gameworld) => new ArmourGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Armour",
			$"Turns an item into {"[armour]".Colour(Telnet.Cyan)}. Must be paired with separate a {"[wearable]".Colour(Telnet.BoldYellow)}",
			$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\ttype <armour type> - sets the armour type for this component. See {"show armours".FluentTagMXP("send", "href='show armours'")} for a list."
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ArmourGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ArmourGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new ArmourGameItemComponentProto(proto, gameworld));
	}

	#region Building Commands

	public override string ShowBuildingHelp =>
		$"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\ttype <armour type> - sets the armour type for this component. See {"show armours".FluentTagMXP("send", "href='show armours'")} for a list.";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
				return BuildingCommand_Type(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommand_Type(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which armour type do you want to set for this armour?");
			return false;
		}

		var type = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.ArmourTypes.Get(value)
			: actor.Gameworld.ArmourTypes.GetByName(command.Last);
		if (type == null)
		{
			actor.Send("There is no such armour type.");
			return false;
		}

		ArmourType = type;
		actor.Send($"This armour will now be of type {ArmourType.Name.TitleCase().Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	#endregion

	#region Overrides of EditableItem

	public override bool CanSubmit()
	{
		return ArmourType != null && base.CanSubmit();
	}

	#region Overrides of EditableItem

	public override string WhyCannotSubmit()
	{
		return ArmourType == null ? "You must first give this component an Armour Type." : base.WhyCannotSubmit();
	}

	#endregion

	#endregion
}