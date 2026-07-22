#nullable enable

using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.Work.Projects.ConcreteTypes;
using System.Globalization;
using ProjectAction = MudSharp.Models.ProjectAction;

namespace MudSharp.Work.Projects.Actions;

public class ResourceDiscoveryProjectAction : BaseAction
{
	private long _requiredLocationTagId;
	private long _outputItemProtoId;
	private int _outputItemProtoRevision;
	private long _duplicatePreventionTagId;

	public ResourceDiscoveryProjectAction(ProjectAction action, IFuturemud gameworld) : base(action, gameworld)
	{
		var root = XElement.Parse(action.Definition);
		_requiredLocationTagId = GetLongElement(root, "RequiredLocationTagId");
		_outputItemProtoId = GetLongElement(root, "OutputItemProtoId");
		_outputItemProtoRevision = (int)GetLongElement(root, "OutputItemProtoRevision");
		_duplicatePreventionTagId = GetLongElement(root, "DuplicatePreventionTagId");
		RequiredLocationTag = _requiredLocationTagId == 0 ? null : Gameworld.Tags.Get(_requiredLocationTagId);
		OutputItemPrototype = _outputItemProtoRevision > 0
			? Gameworld.ItemProtos.Get(_outputItemProtoId, _outputItemProtoRevision)
			: Gameworld.ItemProtos.Get(_outputItemProtoId);
		DuplicatePreventionTag = _duplicatePreventionTagId == 0 ? null : Gameworld.Tags.Get(_duplicatePreventionTagId);
		Echo = root.Element("Echo")?.Value;
		AlreadyPresentEcho = root.Element("AlreadyPresentEcho")?.Value;
		FailureEcho = root.Element("FailureEcho")?.Value;

		if (string.IsNullOrWhiteSpace(Echo))
		{
			Echo = null;
		}

		if (string.IsNullOrWhiteSpace(AlreadyPresentEcho))
		{
			AlreadyPresentEcho = null;
		}

		if (string.IsNullOrWhiteSpace(FailureEcho))
		{
			FailureEcho = null;
		}
	}

	public ResourceDiscoveryProjectAction(IProjectPhase phase, IFuturemud gameworld) : base(phase, gameworld,
		"resourcediscovery")
	{
		Description = "Reveal a configured resource marker item in the project location.";
		Changed = true;
	}

	public ResourceDiscoveryProjectAction(ResourceDiscoveryProjectAction rhs, IProjectPhase newPhase) : base(rhs,
		newPhase, "resourcediscovery")
	{
		_requiredLocationTagId = rhs._requiredLocationTagId;
		_outputItemProtoId = rhs._outputItemProtoId;
		_outputItemProtoRevision = rhs._outputItemProtoRevision;
		_duplicatePreventionTagId = rhs._duplicatePreventionTagId;
		RequiredLocationTag = rhs.RequiredLocationTag;
		OutputItemPrototype = rhs.OutputItemPrototype;
		DuplicatePreventionTag = rhs.DuplicatePreventionTag;
		Echo = rhs.Echo;
		AlreadyPresentEcho = rhs.AlreadyPresentEcho;
		FailureEcho = rhs.FailureEcho;
	}

	public ITag? RequiredLocationTag { get; protected set; }

	public IGameItemProto? OutputItemPrototype { get; protected set; }

	public ITag? DuplicatePreventionTag { get; protected set; }

	public string? Echo { get; protected set; }

	public string? AlreadyPresentEcho { get; protected set; }

	public string? FailureEcho { get; protected set; }

	protected override XElement SaveDefinition()
	{
		return new XElement("Action",
			new XElement("RequiredLocationTagId", RequiredLocationTag?.Id ?? _requiredLocationTagId),
			new XElement("OutputItemProtoId", OutputItemPrototype?.Id ?? _outputItemProtoId),
			new XElement("OutputItemProtoRevision", OutputItemPrototype?.RevisionNumber ?? _outputItemProtoRevision),
			new XElement("DuplicatePreventionTagId", DuplicatePreventionTag?.Id ?? _duplicatePreventionTagId),
			new XElement("Echo", Echo ?? string.Empty),
			new XElement("AlreadyPresentEcho", AlreadyPresentEcho ?? string.Empty),
			new XElement("FailureEcho", FailureEcho ?? string.Empty)
		);
	}

	public override void CompleteAction(IActiveProject project)
	{
		var (valid, _) = CanSubmit();
		if (!valid || OutputItemPrototype is null)
		{
			return;
		}

		var location = ResolveProjectLocation(project);
		if (location is null)
		{
			project.CharacterOwner?.OutputHandler.Send(
				$"Project action {Name.ColourName()} could not reveal a resource because no project location was available.");
			return;
		}

		var roomLayer = ResolveRoomLayer(project, location);
		if (RequiredLocationTag is not null && !location.IsA(RequiredLocationTag))
		{
			if (!string.IsNullOrWhiteSpace(FailureEcho))
			{
				HandleProjectEcho(project, location, FailureEcho, roomLayer);
			}

			return;
		}

		if (MatchingResourceAlreadyPresent(project, location))
		{
			if (!string.IsNullOrWhiteSpace(AlreadyPresentEcho))
			{
				HandleProjectEcho(project, location, AlreadyPresentEcho, roomLayer);
			}

			return;
		}

		var item = OutputItemPrototype.CreateNew(project.CharacterOwner);
		if (project.CharacterOwner is not null)
		{
			item.SetOwner(project.CharacterOwner);
		}
		item.RoomLayer = roomLayer;
		Gameworld.Add(item);
		if (project is ILocalProject localProject)
		{
			item.MoveTo(localProject.SpatialLocation, noSave: true);
			location.Insert(item, true);
		}
		else
		{
			var placementSource = project.ActiveLabour
				.Select(x => x.Character)
				.FirstOrDefault(x => ReferenceEquals(x?.Location, location)) ??
				(ReferenceEquals(project.CharacterOwner?.Location, location) ? project.CharacterOwner : null);
			if (placementSource is not null)
			{
				item.InsertAtSource(placementSource, true);
			}
			else
			{
				location.Insert(item, true);
			}
		}

		HandleProjectEcho(project, location,
			Echo ?? $"Signs of {OutputItemPrototype.ShortDescription} are revealed by the project.", roomLayer);
	}

	private bool MatchingResourceAlreadyPresent(IActiveProject project, ICell location)
	{
		var candidates = project is ILocalProject localProject
			? LocalProjectSpatialRules.GameItemsAtSite(localProject.SpatialLocation)
			: location.GameItems;
		return candidates.Any(item =>
			(DuplicatePreventionTag is not null && item.IsA(DuplicatePreventionTag)) ||
			(OutputItemPrototype is not null &&
			 item.Prototype.Id == OutputItemPrototype.Id &&
			 item.Prototype.RevisionNumber == OutputItemPrototype.RevisionNumber));
	}

	private static ICell? ResolveProjectLocation(IActiveProject project)
	{
		return (project as ActiveProject)?.Location ?? project.CharacterOwner?.Location;
	}

	private static RoomLayer ResolveRoomLayer(IActiveProject project, ICell location)
	{
		if (project is ILocalProject localProject)
		{
			return localProject.RoomLayer;
		}

		return project.ActiveLabour
		              .Select(x => x.Character)
		              .FirstOrDefault(x => x?.Location == location)
		              ?.RoomLayer ??
		       project.CharacterOwner?.RoomLayer ??
		       RoomLayer.GroundLevel;
	}

	private static void HandleProjectEcho(IActiveProject project, ICell location, string text, RoomLayer layer)
	{
		if (project is not ILocalProject localProject || location.RouteDefinition is null)
		{
			location.HandleRoomEcho(text, layer);
			return;
		}

		foreach (var character in LocalProjectSpatialRules.CharactersAtSite(localProject.SpatialLocation))
		{
			character.OutputHandler.Send(text);
		}
	}

	public override IProjectAction Duplicate(IProjectPhase newPhase)
	{
		return new ResourceDiscoveryProjectAction(this, newPhase);
	}

	public override string Show(ICharacter actor)
	{
		var requiredText = RequiredLocationTag?.FullName.ColourName() ?? "any location".ColourValue();
		var outputText = OutputItemPrototype?.ShortDescription.ColourName() ?? "no item".ColourError();
		var duplicateText = DuplicatePreventionTag is null
			? "same prototype".ColourValue()
			: DuplicatePreventionTag.FullName.ColourName();
		return $"[{Name}] reveal {outputText} when location is tagged {requiredText}; duplicate check {duplicateText} - {Description}";
	}

	public override string ShowToPlayer(ICharacter actor)
	{
		return Description;
	}

	public override (bool Truth, string Error) CanSubmit()
	{
		if (_requiredLocationTagId != 0 && RequiredLocationTag is null)
		{
			return (false, "The configured required location tag no longer exists.");
		}

		if (_outputItemProtoId == 0 && OutputItemPrototype is null)
		{
			return (false, "You must set an item prototype to reveal.");
		}

		if (OutputItemPrototype is null)
		{
			return (false, "The configured output item prototype no longer exists.");
		}

		if (_duplicatePreventionTagId != 0 && DuplicatePreventionTag is null)
		{
			return (false, "The configured duplicate-prevention tag no longer exists.");
		}

		return base.CanSubmit();
	}

	protected override string HelpText => $@"{base.HelpText}

	#3locationtag <tag|none>#0 - sets or clears the required location resource tag
	#3item <prototype>#0 - sets the visible marker item prototype to reveal
	#3duplicate <tag|none>#0 - sets or clears a duplicate-prevention item tag
	#3echo <text|none>#0 - sets or clears the success echo
	#3alreadyecho <text|none>#0 - sets or clears the already-present echo
	#3failureecho <text|none>#0 - sets or clears the no-resource echo";

	public override bool BuildingCommand(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "locationtag":
			case "location tag":
			case "resource":
			case "required":
				return BuildingCommandLocationTag(actor, command);
			case "item":
			case "proto":
			case "prototype":
				return BuildingCommandItem(actor, command);
			case "duplicate":
			case "dupetag":
			case "duplicate tag":
				return BuildingCommandDuplicateTag(actor, command);
			case "echo":
				return BuildingCommandText(actor, command, "success echo", value => Echo = value);
			case "alreadyecho":
			case "already":
			case "already echo":
				return BuildingCommandText(actor, command, "already-present echo", value => AlreadyPresentEcho = value);
			case "failureecho":
			case "failure":
			case "fail echo":
				return BuildingCommandText(actor, command, "no-resource echo", value => FailureEcho = value);
		}

		return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"), phase);
	}

	private bool BuildingCommandLocationTag(ICharacter actor, StringStack command)
	{
		return BuildingCommandTag(actor, command, "required location tag", tag =>
		{
			RequiredLocationTag = tag;
			_requiredLocationTagId = tag?.Id ?? 0;
		});
	}

	private bool BuildingCommandDuplicateTag(ICharacter actor, StringStack command)
	{
		return BuildingCommandTag(actor, command, "duplicate-prevention tag", tag =>
		{
			DuplicatePreventionTag = tag;
			_duplicatePreventionTagId = tag?.Id ?? 0;
		});
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command, string purpose, Action<ITag?> assign)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which tag should be used as the {purpose}?");
			return false;
		}

		var tagText = command.SafeRemainingArgument;
		if (tagText.Equals("none", StringComparison.OrdinalIgnoreCase))
		{
			assign(null);
			Changed = true;
			actor.OutputHandler.Send($"This action will no longer use a {purpose}.");
			return true;
		}

		var tags = actor.Gameworld.Tags.FindMatchingTags(tagText);
		if (tags.Count == 0)
		{
			actor.OutputHandler.Send($"There is no tag identified by {tagText.ColourCommand()}.");
			return false;
		}

		if (tags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{tags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
			return false;
		}

		assign(tags.Single());
		Changed = true;
		actor.OutputHandler.Send($"This action will now use {tags.Single().FullName.ColourName()} as the {purpose}.");
		return true;
	}

	private bool BuildingCommandItem(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which item prototype should this action reveal?");
			return false;
		}

		var protoText = command.SafeRemainingArgument;
		var proto = actor.Gameworld.ItemProtos.GetByIdOrUniqueNameOrName(protoText);
		if (proto is null)
		{
			actor.OutputHandler.Send($"There is no item prototype identified by {protoText.ColourCommand()}.");
			return false;
		}

		OutputItemPrototype = proto;
		_outputItemProtoId = proto.Id;
		_outputItemProtoRevision = proto.RevisionNumber;
		Changed = true;
		actor.OutputHandler.Send($"This action will now reveal {proto.ShortDescription.ColourName()}.");
		return true;
	}

	private bool BuildingCommandText(ICharacter actor, StringStack command, string name, Action<string?> assign)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What text should be used for the {name}? Use {"none".ColourCommand()} to clear it.");
			return false;
		}

		var text = command.SafeRemainingArgument;
		if (text.Equals("none", StringComparison.OrdinalIgnoreCase))
		{
			assign(null);
			Changed = true;
			actor.OutputHandler.Send($"The {name} has been cleared.");
			return true;
		}

		assign(text);
		Changed = true;
		actor.OutputHandler.Send($"The {name} is now set to {text.ColourCommand()}.");
		return true;
	}

	private static long GetLongElement(XElement root, string name)
	{
		return long.TryParse(root.Element(name)?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture,
			out var value)
			? value
			: 0L;
	}
}
