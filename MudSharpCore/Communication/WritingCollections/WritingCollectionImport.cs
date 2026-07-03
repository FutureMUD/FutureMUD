using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Form.Colour;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#nullable enable

namespace MudSharp.Communication;

public sealed record WritingCollectionImportResult(
	bool Success,
	string Error,
	IReadOnlyList<(int Page, ICanBeRead Readable)> Entries,
	string? DefaultTitle = null,
	string? Description = null);

public static class WritingCollectionImport
{
	private sealed record WritingSpec(int Page, string Text, string? Language, string? Script, string? Provenance, string? Colour, string? Style);
	private sealed record DrawingSpec(int Page, string ShortDescription, string FullDescription, string? Size, string? Implement);

	private static readonly Regex PageMarkerRegex = new(@"^\s*(?:---|===)\s*page(?:\s+(?<page>\d+))?\s*(?:---|===)\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	private static readonly Regex AttributeRegex = new(@"(?<key>[A-Za-z][A-Za-z0-9_]*)\s*=\s*(?:(?:""(?<quoted>[^""]*)"")|(?<bare>\S+))", RegexOptions.Compiled);

	public static WritingCollectionImportResult ImportMarkdown(IFuturemud gameworld, ICharacter actor, string text)
	{
		var defaults = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
		var body = ExtractFrontMatter(text ?? string.Empty, defaults);
		var specs = new List<object>();
		var page = 1;
		var buffer = new List<string>();
		foreach (var rawLine in SplitLines(body))
		{
			var match = PageMarkerRegex.Match(rawLine);
			if (match.Success)
			{
				AddMarkdownPageSpecs(specs, page, buffer, defaults);
				buffer.Clear();
				page = match.Groups["page"].Success ? int.Parse(match.Groups["page"].Value) : page + 1;
				continue;
			}

			buffer.Add(rawLine);
		}
		AddMarkdownPageSpecs(specs, page, buffer, defaults);

		return CreateReadables(gameworld, actor, specs, defaults);
	}

	public static WritingCollectionImportResult ImportJson(IFuturemud gameworld, ICharacter actor, string text)
	{
		JObject root;
		try
		{
			root = JObject.Parse(text ?? string.Empty);
		}
		catch (Exception ex)
		{
			return new WritingCollectionImportResult(false, $"The JSON could not be parsed: {ex.Message}", Array.Empty<(int, ICanBeRead)>());
		}

		var defaults = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
		foreach (var key in new[] { "language", "script", "provenance", "colour", "color", "style", "title", "description" })
		{
			var value = root[key]?.Value<string>();
			if (!string.IsNullOrWhiteSpace(value))
			{
				defaults[key] = value;
			}
		}

		var pages = root["pages"] as JArray;
		if (pages is null)
		{
			return new WritingCollectionImportResult(false, "The JSON manifest must contain a pages array.", Array.Empty<(int, ICanBeRead)>());
		}

		var specs = new List<object>();
		var pageIndex = 1;
		foreach (var pageToken in pages.OfType<JObject>())
		{
			var page = pageToken["page"]?.Value<int>() ?? pageIndex;
			if (page < 1)
			{
				return new WritingCollectionImportResult(false, "Page numbers must be positive.", Array.Empty<(int, ICanBeRead)>());
			}

			var pageText = pageToken["text"]?.Value<string>();
			if (!string.IsNullOrWhiteSpace(pageText))
			{
				specs.Add(new WritingSpec(page, pageText, ValueOrDefault(pageToken, "language"), ValueOrDefault(pageToken, "script"),
					ValueOrDefault(pageToken, "provenance"), ValueOrDefault(pageToken, "colour") ?? ValueOrDefault(pageToken, "color"),
					ValueOrDefault(pageToken, "style")));
			}

			foreach (var entry in (pageToken["entries"] as JArray ?? new JArray()).OfType<JObject>())
			{
				var type = entry["type"]?.Value<string>() ?? "writing";
				if (type.EqualToAny("writing", "text", "printed"))
				{
					var entryText = entry["text"]?.Value<string>();
					if (string.IsNullOrWhiteSpace(entryText))
					{
						return new WritingCollectionImportResult(false, $"A writing entry on page {page:N0} does not have any text.", Array.Empty<(int, ICanBeRead)>());
					}
					specs.Add(new WritingSpec(page, entryText, ValueOrDefault(entry, "language"), ValueOrDefault(entry, "script"),
						ValueOrDefault(entry, "provenance"), ValueOrDefault(entry, "colour") ?? ValueOrDefault(entry, "color"),
						ValueOrDefault(entry, "style")));
					continue;
				}

				if (type.EqualTo("drawing"))
				{
					var shortDescription = ValueOrDefault(entry, "short") ?? ValueOrDefault(entry, "shortDescription");
					var fullDescription = ValueOrDefault(entry, "full") ?? ValueOrDefault(entry, "fullDescription") ?? entry["text"]?.Value<string>();
					if (string.IsNullOrWhiteSpace(shortDescription) || string.IsNullOrWhiteSpace(fullDescription))
					{
						return new WritingCollectionImportResult(false, $"A drawing entry on page {page:N0} must have short and full descriptions.", Array.Empty<(int, ICanBeRead)>());
					}
					specs.Add(new DrawingSpec(page, shortDescription, fullDescription, ValueOrDefault(entry, "size"), ValueOrDefault(entry, "implement")));
					continue;
				}

				return new WritingCollectionImportResult(false, $"The entry type {type.ColourCommand()} is not supported.", Array.Empty<(int, ICanBeRead)>());
			}

			pageIndex++;
		}

		return CreateReadables(gameworld, actor, specs, defaults);
	}

	private static WritingCollectionImportResult CreateReadables(IFuturemud gameworld, ICharacter actor, List<object> specs, Dictionary<string, string> defaults)
	{
		if (specs.Count == 0)
		{
			return new WritingCollectionImportResult(false, "The import did not contain any readable entries.", Array.Empty<(int, ICanBeRead)>());
		}

		foreach (var spec in specs)
		{
			var page = spec switch
			{
				WritingSpec writingSpec => writingSpec.Page,
				DrawingSpec drawingSpec => drawingSpec.Page,
				_ => 0
			};
			if (page < 1)
			{
				return new WritingCollectionImportResult(false, "Page numbers must be positive.", Array.Empty<(int, ICanBeRead)>());
			}
		}

		var preparedEntries = new List<(int Page, Func<ICanBeRead> CreateReadable)>();
		foreach (var spec in specs)
		{
			switch (spec)
			{
				case WritingSpec writingSpec:
					if (!TryResolveWritingDefaults(gameworld, actor, writingSpec, defaults, out var language, out var script, out var colour, out var style, out var provenance, out var error))
					{
						return new WritingCollectionImportResult(false, error, Array.Empty<(int, ICanBeRead)>());
					}

					preparedEntries.Add((writingSpec.Page, () => new PrintedWriting(gameworld, writingSpec.Text, language, script, provenance, style, colour)));
					break;
				case DrawingSpec drawingSpec:
					if (!TryResolveDrawingDefaults(drawingSpec, out var size, out var implement, out error))
					{
						return new WritingCollectionImportResult(false, error, Array.Empty<(int, ICanBeRead)>());
					}

					var drawingSkill = actor.TraitValue(actor.Gameworld.Traits.Get(actor.Gameworld.GetStaticLong("DrawingTraitId")));
					preparedEntries.Add((drawingSpec.Page, () => new Drawing(actor, drawingSkill,
						drawingSpec.ShortDescription, drawingSpec.FullDescription, implement, size)));
					break;
			}
		}

		var entries = new List<(int Page, ICanBeRead Readable)>();
		foreach (var entry in preparedEntries)
		{
			var readable = entry.CreateReadable();
			if (readable is ILateInitialisingItem lateInitialisingItem)
			{
				gameworld.SaveManager.DirectInitialise(lateInitialisingItem);
			}

			switch (readable)
			{
				case IWriting writing:
					gameworld.Add(writing);
					break;
				case IDrawing drawing:
					gameworld.Add(drawing);
					break;
			}
			entries.Add((entry.Page, readable));
		}

		defaults.TryGetValue("title", out var title);
		defaults.TryGetValue("description", out var description);
		return new WritingCollectionImportResult(true, string.Empty, entries, title, description);
	}

	private static bool TryResolveWritingDefaults(IFuturemud gameworld, ICharacter actor, WritingSpec spec, Dictionary<string, string> defaults,
		out ILanguage language, out IScript script, out IColour colour, out WritingStyleDescriptors style, out string provenance, out string error)
	{
		language = ResolveLanguage(gameworld, spec.Language ?? GetDefault(defaults, "language") ?? actor.CurrentWritingLanguage?.Name)!;
		script = ResolveScript(gameworld, spec.Script ?? GetDefault(defaults, "script") ?? actor.CurrentScript?.Name)!;
		colour = ResolveColour(gameworld, spec.Colour ?? GetDefault(defaults, "colour") ?? GetDefault(defaults, "color"))!;
		style = ResolveStyle(spec.Style ?? GetDefault(defaults, "style"));
		provenance = spec.Provenance ?? GetDefault(defaults, "provenance") ?? string.Empty;
		error = string.Empty;

		if (language is null)
		{
			error = "The import must specify a valid language, or the builder must have a current writing language.";
			return false;
		}

		if (script is null)
		{
			error = "The import must specify a valid script, or the builder must have a current script.";
			return false;
		}

		if (colour is null)
		{
			error = "There is no valid writing colour for the import.";
			return false;
		}

		return true;
	}

	private static bool TryResolveDrawingDefaults(DrawingSpec spec, out DrawingSize size, out WritingImplementType implement, out string error)
	{
		size = DrawingSize.Sketch;
		implement = WritingImplementType.Pencil;
		error = string.Empty;
		if (!string.IsNullOrWhiteSpace(spec.Size) && !spec.Size.TryParseEnum(out size))
		{
			error = $"The drawing size {spec.Size.ColourCommand()} is not valid.";
			return false;
		}

		if (!string.IsNullOrWhiteSpace(spec.Implement) && !spec.Implement.TryParseEnum(out implement))
		{
			error = $"The writing implement type {spec.Implement.ColourCommand()} is not valid.";
			return false;
		}

		return true;
	}

	private static string ExtractFrontMatter(string text, Dictionary<string, string> defaults)
	{
		var lines = SplitLines(text).ToList();
		if (lines.Count == 0 || !lines[0].Trim().EqualTo("---"))
		{
			return text;
		}

		var end = lines.Skip(1).TakeWhile(x => !x.Trim().EqualTo("---")).Count() + 1;
		if (end >= lines.Count)
		{
			return text;
		}

		foreach (var line in lines.Skip(1).Take(end - 1))
		{
			var index = line.IndexOf(':');
			if (index <= 0)
			{
				continue;
			}

			defaults[line[..index].Trim()] = line[(index + 1)..].Trim();
		}

		return string.Join("\n", lines.Skip(end + 1));
	}

	private static void AddMarkdownPageSpecs(List<object> specs, int page, List<string> lines, Dictionary<string, string> defaults)
	{
		var buffer = new List<string>();
		for (var i = 0; i < lines.Count; i++)
		{
			var line = lines[i];
			if (line.TrimStart().StartsWith(":::drawing", StringComparison.InvariantCultureIgnoreCase))
			{
				AddTextSpec(specs, page, buffer, defaults);
				buffer.Clear();
				var attributes = ParseAttributes(line);
				var drawingLines = new List<string>();
				i++;
				while (i < lines.Count && !lines[i].Trim().EqualTo(":::"))
				{
					drawingLines.Add(lines[i]);
					i++;
				}

				var full = string.Join("\n", drawingLines).Trim();
				var shortDescription = attributes.GetValueOrDefault("short") ?? attributes.GetValueOrDefault("sdesc") ?? "an imported drawing";
				specs.Add(new DrawingSpec(page, shortDescription, full, attributes.GetValueOrDefault("size"), attributes.GetValueOrDefault("implement")));
				continue;
			}

			buffer.Add(line);
		}

		AddTextSpec(specs, page, buffer, defaults);
	}

	private static void AddTextSpec(List<object> specs, int page, List<string> buffer, Dictionary<string, string> defaults)
	{
		var text = string.Join("\n", buffer).Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			return;
		}

		specs.Add(new WritingSpec(page, text, null, null, null, null, null));
	}

	private static Dictionary<string, string> ParseAttributes(string line)
	{
		return AttributeRegex.Matches(line).Cast<Match>()
		                     .ToDictionary(x => x.Groups["key"].Value, x => x.Groups["quoted"].Success ? x.Groups["quoted"].Value : x.Groups["bare"].Value,
			                     StringComparer.InvariantCultureIgnoreCase);
	}

	private static IEnumerable<string> SplitLines(string text)
	{
		return (text ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
	}

	private static string? GetDefault(Dictionary<string, string> defaults, string key)
	{
		return defaults.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value : null;
	}

	private static string? ValueOrDefault(JObject root, string key)
	{
		return root[key]?.Value<string>();
	}

	private static ILanguage? ResolveLanguage(IFuturemud gameworld, string? text)
	{
		return string.IsNullOrWhiteSpace(text)
			? null
			: long.TryParse(text, out var value)
				? gameworld.Languages.Get(value)
				: gameworld.Languages.GetByName(text);
	}

	private static IScript? ResolveScript(IFuturemud gameworld, string? text)
	{
		return string.IsNullOrWhiteSpace(text)
			? null
			: long.TryParse(text, out var value)
				? gameworld.Scripts.Get(value)
				: gameworld.Scripts.GetByName(text);
	}

	private static IColour? ResolveColour(IFuturemud gameworld, string? text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return gameworld.Colours.Get(gameworld.GetStaticLong("DefaultWritingColourInText"));
		}

		return long.TryParse(text, out var value)
			? gameworld.Colours.Get(value)
			: gameworld.Colours.GetByName(text);
	}

	private static WritingStyleDescriptors ResolveStyle(string? text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return WritingStyleDescriptors.MachinePrinted;
		}

		var parsed = WritingStyleDescriptors.None.Parse(text);
		return parsed == WritingStyleDescriptors.None ? WritingStyleDescriptors.MachinePrinted : parsed;
	}
}
