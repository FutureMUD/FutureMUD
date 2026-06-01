#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class SealableGameItemComponentProto : GameItemComponentProto, ISealablePrototype
{
	private readonly List<string> _allowedMedia = [];

	public override string TypeDescription => "Sealable";
	public IEnumerable<string> AllowedMedia => _allowedMedia;
	public Difficulty InspectionDifficulty { get; protected set; } = Difficulty.Normal;
	public bool BrokenSealLeavesResidue { get; protected set; } = true;

	protected SealableGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "Sealable")
	{
		_allowedMedia.AddRange(["wax", "clay"]);
	}

	protected SealableGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		_allowedMedia.Clear();
		_allowedMedia.AddRange(root.Element("AllowedMedia")?.Elements("Medium").Select(x => x.Value) ?? Enumerable.Empty<string>());
		InspectionDifficulty = (Difficulty)int.Parse(root.Element("InspectionDifficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		BrokenSealLeavesResidue = bool.Parse(root.Element("BrokenSealLeavesResidue")?.Value ?? "true");
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("AllowedMedia",
				from medium in _allowedMedia
				select new XElement("Medium", new XCData(medium))),
			new XElement("InspectionDifficulty", (int)InspectionDifficulty),
			new XElement("BrokenSealLeavesResidue", BrokenSealLeavesResidue)).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new SealableGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new SealableGameItemComponent(component, this, parent);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("sealable", true,
			(gameworld, account) => new SealableGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Sealable",
			(proto, gameworld) => new SealableGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Sealable",
			$"Lets an item be {"[sealed]".Colour(Telnet.Yellow)} with a seal stamp for tamper evidence",
			BuildingHelpText);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new SealableGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3media add <text>#0 - adds an allowed seal medium keyword, tag, or material name
	#3media remove <text>#0 - removes an allowed seal medium
	#3media clear#0 - allows any seal medium
	#3residue#0 - toggles whether broken seals leave residue
	#3difficulty <difficulty>#0 - sets the seal inspection difficulty";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "media":
			case "medium":
				return BuildingCommandMedia(actor, command);
			case "residue":
				return BuildingCommandResidue(actor);
			case "difficulty":
			case "inspect":
			case "inspection":
				return BuildingCommandDifficulty(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandMedia(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "add":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What medium keyword, tag, or material name do you want to allow?");
					return false;
				}

				var text = command.SafeRemainingArgument.ToLowerInvariant();
				if (!_allowedMedia.Any(x => x.EqualTo(text)))
				{
					_allowedMedia.Add(text);
				}

				Changed = true;
				actor.OutputHandler.Send($"This component now allows {text.ColourValue()} as a seal medium.");
				return true;
			case "remove":
			case "rem":
			case "delete":
			case "del":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("Which medium do you want to remove?");
					return false;
				}

				text = command.SafeRemainingArgument;
				if (!_allowedMedia.RemoveAll(x => x.EqualTo(text)).Equals(0))
				{
					Changed = true;
					actor.OutputHandler.Send($"This component no longer allows {text.ColourValue()} as a seal medium.");
					return true;
				}

				actor.OutputHandler.Send("There was no such allowed medium.");
				return false;
			case "clear":
				_allowedMedia.Clear();
				Changed = true;
				actor.OutputHandler.Send("This component now allows any seal medium.");
				return true;
			default:
				actor.OutputHandler.Send($"Allowed media are: {(_allowedMedia.Any() ? _allowedMedia.Select(x => x.ColourValue()).ListToString() : "any".ColourValue())}.");
				return false;
		}
	}

	private bool BuildingCommandResidue(ICharacter actor)
	{
		BrokenSealLeavesResidue = !BrokenSealLeavesResidue;
		Changed = true;
		actor.OutputHandler.Send($"Broken seals will {(BrokenSealLeavesResidue ? "now" : "no longer")} leave residue.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty))
		{
			actor.OutputHandler.Send($"What difficulty should it be to inspect seals? Valid values are {Enum.GetValues<Difficulty>().Select(x => x.Describe().ColourValue()).ListToString()}.");
			return false;
		}

		InspectionDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send($"This component's seal inspection difficulty is now {InspectionDifficulty.DescribeColoured()}.");
		return true;
	}

	public bool IsAllowedMedium(IGameItem? medium)
	{
		if (medium is null || !_allowedMedia.Any())
		{
			return true;
		}

		return _allowedMedia.Any(allowed =>
			medium.Material?.Name.EqualTo(allowed) == true ||
			medium.Keywords.Any(keyword => keyword.EqualTo(allowed)) ||
			medium.Tags.Any(tag => tag.Name.EqualTo(allowed)));
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return $@"{"Sealable Game Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})

Allowed Media: {(_allowedMedia.Any() ? _allowedMedia.Select(x => x.ColourValue()).ListToString() : "Any".ColourValue())}
Inspection Difficulty: {InspectionDifficulty.DescribeColoured()}
Broken Seals Leave Residue: {BrokenSealLeavesResidue.ToColouredString()}";
	}
}
