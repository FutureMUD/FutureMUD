using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class PaperSheetGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "PaperSheet";

	public int MaximumCharacterLengthOfText { get; set; }

	public override bool WarnBeforePurge => true;

	#region Constructors

	protected PaperSheetGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"PaperSheet")
	{
		MaximumCharacterLengthOfText = 2080;
	}

	protected PaperSheetGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		MaximumCharacterLengthOfText = int.Parse(root.Element("MaximumCharacterLengthOfText").Value);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("MaximumCharacterLengthOfText", MaximumCharacterLengthOfText)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new PaperSheetGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new PaperSheetGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("PaperSheet".ToLowerInvariant(), true,
			(gameworld, account) => new PaperSheetGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("Paper Sheet".ToLowerInvariant(), false,
			(gameworld, account) => new PaperSheetGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("Sheet".ToLowerInvariant(), false,
			(gameworld, account) => new PaperSheetGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("Paper".ToLowerInvariant(), false,
			(gameworld, account) => new PaperSheetGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("PaperSheet",
			(proto, gameworld) => new PaperSheetGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"PaperSheet",
			$"Item is a writable {"[paper]".Colour(Telnet.Yellow)} item that can be in a {"[book]".Colour(Telnet.Pink)} or on its own",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new PaperSheetGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tsize <#> - the number of characters of text that can fit on this sheet";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "length":
			case "max":
			case "size":
			case "capacity":
				return BuildingCommandLength(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandLength(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What should be the upper limit of number of characters able to be written on this sheet of paper at one time?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.Send(
				"You must enter a valid number that is greater than zero for the length. Hint: an A4 sheet of paper holds about 2000 characters.");
			return false;
		}

		MaximumCharacterLengthOfText = value;
		Changed = true;
		actor.Send($"This sheet of paper will now hold {MaximumCharacterLengthOfText:N0} characters of written text.");
		return false;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a sheet of paper to be written on. It can contain at most {4:N0} characters of text.",
			"PaperSheet Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			MaximumCharacterLengthOfText
		);
	}
}