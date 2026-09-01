using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class ArtilleryChamberGameItemComponentProto : GameItemComponentProto, IArtilleryChamberPrototype
{
	public ArtilleryChamberGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "ArtilleryChamber")
	{
	}

	protected ArtilleryChamberGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "ArtilleryChamber";
	public string ArtilleryProfile { get; private set; } = "general";

	protected override void LoadFromXml(XElement root)
	{
		ArtilleryProfile = root.Element("ArtilleryProfile")?.Value ?? "general";
	}

	protected override string SaveToXml() => new XElement("Definition",
		new XElement("ArtilleryProfile", new XCData(ArtilleryProfile))).ToString();

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) =>
		new ArtilleryChamberGameItemComponent(this, parent, temporary);

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) =>
		new ArtilleryChamberGameItemComponent(component, this, parent);

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) =>
		CreateNewRevision(initiator, (proto, gameworld) => new ArtilleryChamberGameItemComponentProto(proto, gameworld));

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("artillerychamber", true,
			(gameworld, account) => new ArtilleryChamberGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ArtilleryChamber",
			(proto, gameworld) => new ArtilleryChamberGameItemComponentProto(proto, gameworld));
		manager.AddModernTypeHelpInfo("ArtilleryChamber", "Makes an item a removable artillery breech chamber", BuildingHelpText);
	}

	private const string BuildingHelpText = @"You can use the following options with this component:

	#3profile <name>#0 - sets the compatible artillery profile.";
	public override string ShowBuildingHelp => BuildingHelpText;
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PopForSwitch().EqualToAny("profile", "artilleryprofile"))
		{
			return base.BuildingCommand(actor, command.GetUndo());
		}

		if (command.IsFinished)
		{
			actor.Send("Which artillery profile should this chamber fit?");
			return false;
		}

		ArtilleryProfile = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.Send($"This chamber now fits the {ArtilleryProfile.ColourName()} artillery profile.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor) =>
		$"{Name.ColourName()} is a removable chamber for the {ArtilleryProfile.ColourValue()} artillery profile.";
}
