#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;


namespace MudSharp.GameItems.Prototypes;

public sealed class KeywordDescriptionsGameItemComponentProto : GameItemComponentProto
{
	private readonly List<KeywordDescription> _descriptions = [];
	public IReadOnlyCollection<KeywordDescription> KeywordDescriptions => _descriptions;
	public override string TypeDescription => "Keyword Descriptions";

	private KeywordDescriptionsGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Keyword Descriptions") { }

	private KeywordDescriptionsGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld) { }

	protected override void LoadFromXml(XElement root)
	{
		_descriptions.Clear();
		foreach (var entry in root.Element("KeywordDescriptions")?.Elements("Entry") ?? [])
		{
			var keywords = (string?)entry.Element("Keyword") ?? string.Empty;
			var description = (string?)entry.Element("Prose") ?? string.Empty;
			if (!string.IsNullOrWhiteSpace(keywords) && !string.IsNullOrWhiteSpace(description))
				_descriptions.Add(new KeywordDescription(keywords, description));
		}
	}

	protected override string SaveToXml() => new XElement("Definition",
		new XElement("KeywordDescriptions", _descriptions.Select(x => new XElement("Entry",
			new XElement("Keyword", x.Keywords), new XElement("Prose", x.Description))))).ToString();

	public override string ShowBuildingHelp => "add <keywords> | <description> adds an entry; remove <number> removes one.";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var option = command.PopSpeech().ToLowerInvariant();
		if (option == "add")
		{
			var text = command.SafeRemainingArgument;
			var separator = text.IndexOf('|');
			if (separator < 1 || separator == text.Length - 1)
			{
				actor.OutputHandler.Send("Use add <keywords> | <description>.");
				return false;
			}
			var keywords = text[..separator].Trim();
			var description = text[(separator + 1)..].Trim();
			if (string.IsNullOrWhiteSpace(keywords) || string.IsNullOrWhiteSpace(description)) return false;
			if (_descriptions.Any(x => KeywordDescriptionMatcher.Matches(x.Keywords, keywords)))
			{
				actor.OutputHandler.Send("That keyword entry already exists.");
				return false;
			}
			_descriptions.Add(new KeywordDescription(keywords, description));
			Changed = true;
			actor.OutputHandler.Send($"You add a keyword description for {keywords.ColourName()}.");
			return true;
		}
		if (option == "remove")
		{
			if (!int.TryParse(command.PopSpeech(), out var index) || index < 1 || index > _descriptions.Count)
			{
				actor.OutputHandler.Send("Choose a valid entry number to remove.");
				return false;
			}
			_descriptions.RemoveAt(index - 1);
			Changed = true;
			actor.OutputHandler.Send("You remove that keyword description.");
			return true;
		}
		return base.BuildingCommand(actor, command);
	}

	public override string ComponentDescriptionOLC(ICharacter actor) => _descriptions.Count == 0
		? "This component has no keyword descriptions."
		: string.Join("\n", _descriptions.Select((x, i) => $"{i + 1}. {x.Keywords}: {x.Description}"));

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) =>
		new KeywordDescriptionsGameItemComponent(this, parent, temporary);

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) =>
		new KeywordDescriptionsGameItemComponent(component, this, parent);

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("keyworddescriptions", true,
			(gameworld, account) => new KeywordDescriptionsGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Keyword Descriptions",
			(proto, gameworld) => new KeywordDescriptionsGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("Keyword Descriptions", "Provides contextual prose when a player looks up configured keywords on an item.",
			"Use add <keywords> | <description> and remove <number> to maintain entries.");
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) =>
		CreateNewRevision(initiator, (proto, gameworld) => new KeywordDescriptionsGameItemComponentProto(proto, gameworld));
}
