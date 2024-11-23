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
	public bool ApplyArmourPenalties { get; set; } = true;

	public override string TypeDescription => "Armour";

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("ArmourType");
		if (element != null)
		{
			ArmourType = Gameworld.ArmourTypes.Get(long.Parse(element.Value));
		}

		element = root.Element("ApplyArmourPenalties");
		ApplyArmourPenalties = element is null || bool.Parse(element.Value);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, @"{0} (#{1:N0}r{2:N0}, {3})

This item is armour of type {4}.
It {5} apply armour type penalties for stacking.",
			"Armour Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			ArmourType?.Name.TitleCase().Colour(Telnet.Green) ?? "None".Colour(Telnet.Red),
			ApplyArmourPenalties ? "does".ColourValue() : "does not".ColourError()
		);
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
				new XElement("ArmourType", ArmourType?.Id ?? 0),
				new XElement("ApplyArmourPenalties", ApplyArmourPenalties)
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
			$@"You can use the following options:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3type <armour type>#0 - sets the armour type for this component. See {"show armours".FluentTagMXP("send", "href='show armours'")} for a list."
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
		$@"You can use the following options:
	
	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3type <armour type>#0 - sets the armour type for this component. See {"show armours".FluentTagMXP("send", "href='show armours'")} for a list.
	#3penalties#0 - toggles applying armour skill penalties for this armour";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "type":
				return BuildingCommand_Type(actor, command);
			case "penalties":
				return BuildingCommandPenalties(actor);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandPenalties(ICharacter actor)
	{

		ApplyArmourPenalties = !ApplyArmourPenalties;
		Changed = true;
		actor.OutputHandler.Send($"This armour type will {ApplyArmourPenalties.NowNoLonger()} apply worn penalties on its wearer for the armour type.");
		return true;
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