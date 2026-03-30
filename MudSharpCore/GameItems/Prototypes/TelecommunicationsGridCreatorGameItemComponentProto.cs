#nullable enable
using System.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class TelecommunicationsGridCreatorGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "TelecommunicationsGridCreator";
	public string Prefix { get; set; }
	public int NumberLength { get; set; }

	protected TelecommunicationsGridCreatorGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(
		gameworld, originator, "TelecommunicationsGridCreator")
	{
		Prefix = "555";
		NumberLength = 4;
	}

	protected TelecommunicationsGridCreatorGameItemComponentProto(Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(System.Xml.Linq.XElement root)
	{
		Prefix = root.Element("Prefix")?.Value ?? "555";
		NumberLength = int.Parse(root.Element("NumberLength")?.Value ?? "4");
	}

	protected override string SaveToXml()
	{
		return new System.Xml.Linq.XElement("Definition",
			new System.Xml.Linq.XElement("Prefix", Prefix),
			new System.Xml.Linq.XElement("NumberLength", NumberLength)
		).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new TelecommunicationsGridCreatorGameItemComponent(this, parent, loader, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new TelecommunicationsGridCreatorGameItemComponent(component, this, parent);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("TelecommunicationsGridCreator".ToLowerInvariant(), true,
			(gameworld, account) => new TelecommunicationsGridCreatorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("TelecommunicationsGridCreator",
			(proto, gameworld) => new TelecommunicationsGridCreatorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"TelecommunicationsGridCreator",
			$"When put in a room, creates a {"[telecommunications grid]".Colour(Telnet.BoldBlue)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new TelecommunicationsGridCreatorGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tprefix <digits> - sets the number prefix for telephones on this grid\n\tdigits <#> - sets the number of subscriber digits after the prefix";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "prefix":
				return BuildingCommandPrefix(actor, command);
			case "digits":
			case "length":
			case "numberlength":
				return BuildingCommandDigits(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandPrefix(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What numeric prefix should telephones on this grid use?");
			return false;
		}

		var prefix = new string(command.PopSpeech().Where(char.IsDigit).ToArray());
		if (string.IsNullOrEmpty(prefix))
		{
			actor.Send("The prefix must contain at least one digit.");
			return false;
		}

		Prefix = prefix;
		Changed = true;
		actor.Send($"This telecommunications grid will now use the prefix {Prefix.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDigits(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many subscriber digits should telephones on this grid have after the prefix?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value < 1)
		{
			actor.Send("You must enter a positive whole number of digits.");
			return false;
		}

		NumberLength = value;
		Changed = true;
		actor.Send($"This telecommunications grid will now use {NumberLength.ToString("N0", actor).ColourValue()} subscriber digits.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item creates a telecommunications grid with prefix {4} and {5:N0} subscriber digits.",
			"TelecommunicationsGridCreator Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Prefix.ColourValue(),
			NumberLength
		);
	}
}
