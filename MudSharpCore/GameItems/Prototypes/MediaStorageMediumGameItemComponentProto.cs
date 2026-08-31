#nullable enable

using MudSharp.Accounts;
using MudSharp.Computers;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

/// <summary>
/// Describes a removable physical medium. The immutable recording data is held by the media recording service;
/// the component only owns its named references and physical constraints.
/// </summary>
public class MediaStorageMediumGameItemComponentProto : GameItemComponentProto, IMediaStorageMediumPrototype
{
	private const string SpecificBuildingHelpText = @"
	#3format <key>#0 - sets the physical format key that compatible decks accept
	#3capabilities <audio|video|av>#0 - sets the media carried by this medium
	#3capacity <minutes>#0 - sets its total recording duration capacity";

	public MediaStorageMediumGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Media Storage Medium")
	{
		FormatKey = "generic";
		Capabilities = MediaCapabilities.Audio;
		Capacity = TimeSpan.FromMinutes(30);
	}

	protected MediaStorageMediumGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public string FormatKey { get; protected set; } = "generic";
	public MediaCapabilities Capabilities { get; protected set; }
	public TimeSpan Capacity { get; protected set; }
	public override string TypeDescription => "Media Storage Medium";

	protected override void LoadFromXml(XElement root)
	{
		FormatKey = root.Element("FormatKey")?.Value?.Trim() ?? "generic";
		if (string.IsNullOrWhiteSpace(FormatKey))
		{
			FormatKey = "generic";
		}

		Capabilities = Enum.TryParse<MediaCapabilities>(root.Element("Capabilities")?.Value, true,
			out var capabilities) && capabilities != MediaCapabilities.None
			? capabilities
			: MediaCapabilities.Audio;
		Capacity = TimeSpan.FromMilliseconds(Math.Max(1L,
			long.TryParse(root.Element("CapacityMilliseconds")?.Value, out var milliseconds)
				? milliseconds
				: (long)TimeSpan.FromMinutes(30).TotalMilliseconds));
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("FormatKey", FormatKey),
			new XElement("Capabilities", Capabilities),
			new XElement("CapacityMilliseconds", (long)Capacity.TotalMilliseconds)).ToString();
	}

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "format":
				return BuildingCommandFormat(actor, command);
			case "capabilities":
			case "capability":
				return BuildingCommandCapabilities(actor, command);
			case "capacity":
			case "minutes":
				return BuildingCommandCapacity(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandFormat(ICharacter actor, StringStack command)
	{
		var value = command.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(value) || value.Any(char.IsWhiteSpace))
		{
			actor.Send("Media format keys must be a single non-empty word.");
			return false;
		}

		FormatKey = value;
		Changed = true;
		actor.Send($"This medium now uses the {FormatKey.ColourCommand()} format key.");
		return true;
	}

	private bool BuildingCommandCapabilities(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !MediaComponentUtilities.TryParseCapabilities(command.PopSpeech(), out var capabilities))
		{
			actor.Send("Choose audio, video or av for this medium.");
			return false;
		}

		Capabilities = capabilities;
		Changed = true;
		actor.Send($"This medium now stores {MediaComponentUtilities.DescribeCapabilities(Capabilities).ColourValue()} recordings.");
		return true;
	}

	private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var minutes) || minutes <= 0.0)
		{
			actor.Send("How many positive minutes of recording capacity should this medium have?");
			return false;
		}

		Capacity = TimeSpan.FromMinutes(minutes);
		Changed = true;
		actor.Send($"This medium now has {Capacity.Describe(actor).ColourValue()} of recording capacity.");
		return true;
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("mediastoragemedium", true,
			(gameworld, account) => new MediaStorageMediumGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("media storage medium", false,
			(gameworld, account) => new MediaStorageMediumGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("medium", false,
			(gameworld, account) => new MediaStorageMediumGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Media Storage Medium",
			(proto, gameworld) => new MediaStorageMediumGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("Media Storage Medium",
			$"Makes an item a reusable {"[physical media medium]".Colour(Telnet.BoldGreen)} for a compatible media deck",
			SpecificBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new MediaStorageMediumGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new MediaStorageMediumGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new MediaStorageMediumGameItemComponentProto(proto, gameworld));
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return $"{"Media Storage Medium Game Item Component".Colour(Telnet.Cyan)} " +
		       $"(#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\n" +
		       $"Format: {FormatKey.ColourCommand()}\n" +
		       $"Capabilities: {MediaComponentUtilities.DescribeCapabilities(Capabilities).ColourValue()}\n" +
		       $"Capacity: {Capacity.Describe(actor).ColourValue()}";
	}
}
