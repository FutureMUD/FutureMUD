#nullable enable

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private static readonly Dictionary<int, string> NoteLanguages = new()
	{
		[151] = "Sirihish", [152] = "Bendune", [153] = "Allundean", [154] = "Mirukkim",
		[155] = "Kentu", [156] = "Nrizkt", [157] = "Untal", [158] = "Cavilish", [159] = "Tatlum",
		[236] = "Anyar", [238] = "Heshrak", [240] = "Vloran", [242] = "Domat", [244] = "Ghaati"
	};

	private Script InstallWorldfileWritingScript()
	{
		var languages = _db.Languages.Local.Concat(_db.Languages).DistinctBy(x => x.Id)
			.Where(x => NoteLanguages.Values.Contains(x.Name, StringComparer.OrdinalIgnoreCase)).ToArray();
		var available = Prog("ArmageddonWorldfileWritingKnowledge", "switch (@skill.Name)\n" +
			string.Join("\n", languages.Select(x => $"case (\"{x.Name}\")\nreturn true")) + "\nend switch\nreturn false",
			ProgVariableTypes.Boolean, ("ch", ProgVariableTypes.Chargen), ("skill", ProgVariableTypes.Trait));
		var knowledge = new Knowledge
		{
			Id = Next<Knowledge>(), Name = "Zalanthan Script", Description = "Reading and writing the shared Zalanthan script",
			LongDescription = "The common writing system used to record the languages of Zalanthas.", Type = "Script", Subtype = "Zalanthan",
			LearnableType = (int)(LearnableType.LearnableAtChargen | LearnableType.LearnableFromTeacher),
			LearnDifficulty = (int)Difficulty.VeryHard, TeachDifficulty = (int)Difficulty.VeryHard, LearningSessionsRequired = 10,
			CanAcquireProgId = available.Id, CanLearnProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]
		};
		_db.Knowledges.Add(knowledge);
		var script = new Script { Id = Next<Script>(), Name = "Zalanthan", KnowledgeId = knowledge.Id,
			KnownScriptDescription = "the Zalanthan script", UnknownScriptDescription = "an unfamiliar script",
			DocumentLengthModifier = 1.0, InkUseModifier = 1.0 };
		_db.Scripts.Add(script);
		foreach (var language in languages)
			_db.ScriptsDesignedLanguages.Add(new ScriptsDesignedLanguage { ScriptId = script.Id, LanguageId = language.Id });
		Record("writing.shared-script", nameof(Script), script.Id, null,
			new { Knowledge = knowledge.Id, Disposition = "Approved shared script for all imported languages; native literacy, knowledge and language comprehension. No direct character grants; normal knowledge-picker behaviour applies." });
		return script;
	}

	private void InstallNoteComponents()
	{
		var notes = _effectiveItems.Where(x => x.Value.Source.ItemType == 16).ToArray();
		if (notes.Length == 0) return;
		if (!_items.TryGetValue(1503, out var paper))
			throw new InvalidOperationException("Native worldfile books require source paper Vnum1503.");
		var sheet = Component("writing.paper", "PaperSheet", new XElement("Definition",
			new XElement("MaximumCharacterLengthOfText", 2080)));
		Script? script = null;
		foreach (var (id, effective) in notes)
		{
			var proto = _items.Values.Concat(_resetItemVariants.Values).First(x => x.Id == id);
			var keywordComponents = proto.GameItemProtosGameItemComponentProtos
				.Select(link => _db.GameItemComponentProtos.Local.Single(x => x.Id == link.GameItemComponentProtoId))
				.Where(x => x.Type == "Keyword Descriptions").ToArray();
			var descriptions = effective.Source.ExtraDescriptions.Concat(keywordComponents.SelectMany(x =>
				XElement.Parse(x.Definition).Descendants("Entry").Select(e => new ArmWorldExtra(
					e.Element("Keyword")!.Value, e.Element("Prose")!.Value))))
				.GroupBy(x => x.Keyword).Select(x => x.Last()).ToArray();
			var pages = Math.Max(0, effective.Values[0]);
			var authored = descriptions.Select(x => (Source: x, Match: Regex.Match(x.Keyword, @"^page_?(\d+)$", RegexOptions.IgnoreCase)))
				.Where(x => x.Match.Success).Select(x => (Page: int.Parse(x.Match.Groups[1].Value), x.Source))
				.OrderBy(x => x.Page).ToArray();
			var language = NoteLanguages.TryGetValue(effective.Values[2], out var languageName)
				? _db.Languages.Local.Concat(_db.Languages).FirstOrDefault(x => x.Name.Equals(languageName, StringComparison.OrdinalIgnoreCase)) : null;
			if ((descriptions.Length > 0 || !string.IsNullOrWhiteSpace(effective.Source.FullDescription)) &&
				(authored.Length == 0 || language is null || authored.Any(x => x.Page < 1 || x.Page > pages)))
			{
				Adapt("note writing", effective.Source.Vnum, "Unmapped text, language or page numbering retained as source prose; not blank consumable paper.");
				continue;
			}
			var references = new List<XElement>();
			if (authored.Length > 0)
			{
				script ??= InstallWorldfileWritingScript();
				var colour = _db.Colours.Local.Concat(_db.Colours).First(x => x.Name.Equals("black", StringComparison.OrdinalIgnoreCase));
				foreach (var page in authored)
				{
					var writing = new Writing { Id = Next<Writing>(), WritingType = "printed", LanguageId = language!.Id,
						ScriptId = script.Id, WritingColour = colour.Id, ImplementType = (int)WritingImplementType.Printed,
						LiteracySkill = 100, LanguageSkill = 100, HandwritingSkill = 100,
						Definition = new XElement("Definition", new XElement("Text", page.Source.Description),
							new XElement("Provenance", $"Armageddon Vnum{effective.Source.Vnum}, page {page.Page}")).ToString() };
					_db.Writings.Add(writing);
					references.Add(new XElement("Writing", new XAttribute("Id", writing.Id), new XAttribute("Page", page.Page), new XAttribute("Order", 1)));
				}
				// Page prose must pass through native literacy rather than keyword inspection.
				foreach (var keywords in keywordComponents)
				{
					var xml = XElement.Parse(keywords.Definition);
					xml.Descendants("Entry").Where(x => authored.Any(a => a.Source.Keyword == x.Element("Keyword")?.Value)).Remove();
					keywords.Definition = xml.ToString();
				}
			}
			var looseBlankSheet = pages == 1 && references.Count == 0;
			var component = looseBlankSheet ? sheet : Component("writing.book." + id, "Book", new XElement("Definition",
				new XElement("PaperProto", paper.Id), new XElement("PageCount", pages), new XElement("DefaultTitle", ""),
				new XElement("InitialReadables", references)));
			Attach(proto, component);
			foreach (var item in _db.GameItems.Local.Where(x => x.GameItemProtoId == id))
			{
				var state = looseBlankSheet ? new XElement("Definition") : new XElement("Definition",
					new XElement("Open", false), new XElement("Title", ""), new XElement("CurrentPage", pages > 0 ? 1 : 0),
					new XElement("TornPages"), new XElement("Writings", references));
				var instance = new GameItemComponent { Id = Next<GameItemComponent>(), GameItemId = item.Id,
					GameItemComponentProtoId = component.Id, GameItemComponentProtoRevision = component.RevisionNumber, Definition = state.ToString() };
				_db.GameItemComponents.Add(instance);
				item.GameItemComponents.Add(instance);
			}
			Record("writing.note." + id, nameof(GameItemComponentProto), component.Id, component.RevisionNumber,
				new { SourceVnum = effective.Source.Vnum, Pages = pages, WrittenPages = references.Count, Language = language?.Name,
					Disposition = "Native sheet or paged book; authored pages preserve language and share the Zalanthan script. Native writing/turning/tearing and comprehension. Historical note-file identifiers do not import private notes. Native page capacity is 2080 characters." });
		}
	}

	private void InstallWritingImplements()
	{
		var pens = _effectiveItems.Where(x => x.Value.Source.ItemType == 21).ToArray();
		if (pens.Length == 0) return;
		var black = _db.Colours.Local.Concat(_db.Colours).First(x => x.Name.Equals("black", StringComparison.OrdinalIgnoreCase));
		foreach (var (id, effective) in pens)
		{
			// These two source pens are solid bone/soft-stone points; the other five are feathers.
			var implement = effective.Source.Vnum is 1595 or 20039 ? WritingImplementType.Stylus : WritingImplementType.Quill;
			var component = Component("writing.implement." + implement, "ScribingImplement", new XElement("Definition",
				new XElement("ImplementType", implement.ToString()), new XElement("Colour", black.Id),
				new XElement("ColourCharacteristic", 0), new XElement("TotalUses", 0)));
			var proto = _items.Values.Concat(_resetItemVariants.Values).First(x => x.Id == id);
			AttachDeliveryComponent(proto, component, new XElement("Definition", new XElement("RemainingUses", 0)));
			Record("writing.implement." + id, nameof(GameItemComponentProto), component.Id, component.RevisionNumber,
				new { SourceVnum = effective.Source.Vnum, Implement = implement.ToString(), Colour = black.Id,
					Disposition = "Source ITEM_PEN uses native writing/drawing. Source writing consumes neither pen charges nor ink; native nonconsuming mode preserves this. Black is an authored presentation default. No new supplies, resets or ink economy." });
		}
	}
}
