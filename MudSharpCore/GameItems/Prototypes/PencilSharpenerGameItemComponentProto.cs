using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class PencilSharpenerGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "PencilSharpener";

	public string SharpenEmote { get; set; }

	#region Constructors

	protected PencilSharpenerGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "PencilSharpener")
	{
		SharpenEmote = "$0 stick|sticks $2 in $1 and sharpen|sharpens it to a fine point.";
	}

	protected PencilSharpenerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		SharpenEmote = root.Element("SharpenEmote")?.Value;
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("SharpenEmote", new XCData(SharpenEmote))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new PencilSharpenerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new PencilSharpenerGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("PencilSharpener".ToLowerInvariant(), true,
			(gameworld, account) => new PencilSharpenerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("PencilSharpener",
			(proto, gameworld) => new PencilSharpenerGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"PencilSharpener",
			$"Can sharpen a pencil item",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new PencilSharpenerGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\temote <emote> - sets the emote when using. $0 for the person, $1 for the pencil sharpener and $2 for the pencil";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "emote":
				return BuildingCommandEmote(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set for this pencil sharpener? Use $0 for the person, $1 for the pencil sharpener and $2 for the thing being sharpened.");
			return false;
		}

		SharpenEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"The emote when sharpening with this will now be: {SharpenEmote.ColourCommand()}");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a pencil sharpener, for sharpening pencils.\nSharpen Emote: {4}",
			"PencilSharpener Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			SharpenEmote
		);
	}
}