using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class InscribableSurfaceGameItemComponentProto : GameItemComponentProto, IWriteablePrototype, IReadablePrototype
{
	public override string TypeDescription => "InscribableSurface";

	public int MaximumCharacterLengthOfText { get; set; }
	public HashSet<WritingImplementType> AllowedImplementTypes { get; } = new();

	public override bool WarnBeforePurge => true;

	#region Constructors

	protected InscribableSurfaceGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "InscribableSurface")
	{
		MaximumCharacterLengthOfText = 2080;
		AllowedImplementTypes.Add(WritingImplementType.Stylus);
	}

	protected InscribableSurfaceGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		MaximumCharacterLengthOfText = int.Parse(root.Element("MaximumCharacterLengthOfText")?.Value ?? "2080");
		AllowedImplementTypes.Clear();
		foreach (var element in root.Element("AllowedImplementTypes")?.Elements("Type") ?? Enumerable.Empty<XElement>())
		{
			AllowedImplementTypes.Add(ParseImplementType(element.Value));
		}

		if (AllowedImplementTypes.Count == 0)
		{
			AllowedImplementTypes.Add(WritingImplementType.Stylus);
		}
	}

	private static WritingImplementType ParseImplementType(string text)
	{
		return int.TryParse(text, out var value)
			? (WritingImplementType)value
			: Enum.Parse<WritingImplementType>(text, true);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("MaximumCharacterLengthOfText", MaximumCharacterLengthOfText),
			new XElement("AllowedImplementTypes",
				AllowedImplementTypes
					.OrderBy(x => x.ToString(), StringComparer.Ordinal)
					.Select(x => new XElement("Type", x.ToString())))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new InscribableSurfaceGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new InscribableSurfaceGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("InscribableSurface".ToLowerInvariant(), true,
			(gameworld, account) => new InscribableSurfaceGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("Inscribable Surface".ToLowerInvariant(), false,
			(gameworld, account) => new InscribableSurfaceGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("Writing Surface".ToLowerInvariant(), false,
			(gameworld, account) => new InscribableSurfaceGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("InscribableSurface",
			(proto, gameworld) => new InscribableSurfaceGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"InscribableSurface",
			$"Item is a writable {"[surface]".Colour(Telnet.Yellow)} with configurable acceptable writing implements",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new InscribableSurfaceGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tsize <#> - the number of characters of text that can fit on this surface\n\timplement add <type> - allows a writing implement type\n\timplement remove <type> - removes a writing implement type\n\timplement clear - clears all allowed writing implement types";

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
			case "implement":
			case "implements":
			case "type":
			case "types":
				return BuildingCommandImplement(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandLength(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What should be the upper limit of number of characters able to be written on this surface?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.Send("You must enter a valid number that is greater than zero for the length.");
			return false;
		}

		MaximumCharacterLengthOfText = value;
		Changed = true;
		actor.Send($"This surface will now hold {MaximumCharacterLengthOfText:N0} characters of written text.");
		return true;
	}

	private bool BuildingCommandImplement(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"This surface currently accepts {AllowedImplementTypes.Select(x => x.Describe()).ListToString()}.");
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "add":
				return BuildingCommandImplementAdd(actor, command);
			case "remove":
			case "delete":
			case "rem":
				return BuildingCommandImplementRemove(actor, command);
			case "clear":
				AllowedImplementTypes.Clear();
				Changed = true;
				actor.Send("This surface no longer accepts any writing implement types.");
				return true;
			default:
				actor.Send("You must specify ADD, REMOVE, or CLEAR.");
				return false;
		}
	}

	private bool BuildingCommandImplementAdd(ICharacter actor, StringStack command)
	{
		if (!TryParseImplement(command, actor, out var type))
		{
			return false;
		}

		AllowedImplementTypes.Add(type);
		Changed = true;
		actor.Send($"This surface now accepts {type.Describe().ColourName()} implements.");
		return true;
	}

	private bool BuildingCommandImplementRemove(ICharacter actor, StringStack command)
	{
		if (!TryParseImplement(command, actor, out var type))
		{
			return false;
		}

		AllowedImplementTypes.Remove(type);
		Changed = true;
		actor.Send($"This surface no longer accepts {type.Describe().ColourName()} implements.");
		return true;
	}

	private static bool TryParseImplement(StringStack command, ICharacter actor, out WritingImplementType type)
	{
		type = WritingImplementType.Stylus;
		if (command.IsFinished)
		{
			actor.Send("Which writing implement type?");
			return false;
		}

		if (Enum.TryParse(command.SafeRemainingArgument, true, out type))
		{
			return true;
		}

		actor.Send("That is not a valid writing implement type.");
		return false;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is an inscribable surface. It can contain at most {4:N0} characters of text and accepts {5}.",
			"InscribableSurface Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			MaximumCharacterLengthOfText,
			AllowedImplementTypes.Select(x => x.Describe()).ListToString()
		);
	}
}
