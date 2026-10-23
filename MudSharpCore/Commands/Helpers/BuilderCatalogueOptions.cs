#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.Commands.Helpers;

/// <summary>Output controls are parsed before domain filters, and applied before row formatting.</summary>
internal sealed class BuilderCatalogueOptions
{
	public const string Help = "#3list [filters] [--page <number>] [--limit <1-1000>] [--count] [--sort id|name|revision] [--descending] [--export csv]#0";
	public int Page { get; private set; } = 1;
	public int? Limit { get; private set; }
	public bool CountOnly { get; private set; }
	public bool Descending { get; private set; }
	public bool ExportCsv { get; private set; }
	public string Sort { get; private set; } = "id";
	private bool SortSpecified { get; set; }

	public static bool TryParse(StringStack input, bool revisable, out BuilderCatalogueOptions options,
		out StringStack filters, out string error)
	{
		options = new BuilderCatalogueOptions();
		var remaining = new List<string>();
		var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		error = "";
		while (!input.IsFinished)
		{
			var before = input.RemainingArgument;
			var token = input.PopSpeech();
			if (!token.StartsWith("--", StringComparison.Ordinal))
			{
				// Preserve the original quoted token for the family's existing parser.
				remaining.Add(before[..(before.Length - input.RemainingArgument.Length)].TrimEnd());
				continue;
			}
			var option = token.ToLowerInvariant();
			if (!seen.Add(option)) { error = $"Duplicate option {token}."; break; }
			switch (option)
			{
				case "--count": options.CountOnly = true; break;
				case "--descending": options.Descending = true; break;
				case "--page":
				case "--limit":
					if (!int.TryParse(input.PopSpeech(), out var number) || number < 1 ||
					    (option == "--limit" && number > 1000))
					{ error = $"{token} requires a positive integer{(option == "--limit" ? " no greater than 1000" : "")}."; break; }
					if (option == "--page") options.Page = number; else options.Limit = number;
					break;
				case "--sort":
					options.SortSpecified = true;
					options.Sort = input.PopSpeech().ToLowerInvariant();
					if (options.Sort is not ("id" or "name") && !(revisable && options.Sort == "revision"))
						error = "Sort by id or name" + (revisable ? ", or revision." : ".");
					break;
				case "--export":
					if (!input.PopSpeech().EqualTo("csv")) error = "The supported export format is csv.";
					else options.ExportCsv = true;
					break;
				default: error = $"Unknown catalogue option {token}."; break;
			}
			if (error.Length > 0) break;
		}
		if (seen.Contains("--page") && options.Limit is null) options.Limit = 50;
		if (options.CountOnly && options.ExportCsv) error = "Use either --count or --export csv.";
		filters = new StringStack(string.Join(" ", remaining));
		return error.Length == 0;
	}

	internal IReadOnlyList<T> Select<T>(IEnumerable<T> source, out int total, Func<IEnumerable<T>, IEnumerable<T>>? defaultOrder = null) where T : IFrameworkItem
	{
		var items = source.ToList();
		total = items.Count;
		if (CountOnly) return Array.Empty<T>();
		IOrderedEnumerable<T> ordered = Sort switch
		{
			"name" => Descending ? items.OrderByDescending(x => x.Name, StringComparer.OrdinalIgnoreCase) : items.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase),
			"revision" => Descending ? items.OrderByDescending(x => ((IEditableRevisableItem)x).RevisionNumber) : items.OrderBy(x => ((IEditableRevisableItem)x).RevisionNumber),
			_ => Descending ? items.OrderByDescending(x => x.Id) : items.OrderBy(x => x.Id)
		};
		IEnumerable<T> stable = !SortSpecified && !Descending && defaultOrder is not null
			? defaultOrder(items)
			: ordered.ThenBy(x => x.Id).ThenBy(x => (x as IEditableRevisableItem)?.RevisionNumber ?? 0);
		var offset = (long)(Page - 1) * (Limit ?? total);
		return offset > int.MaxValue ? Array.Empty<T>() : stable.Skip((int)offset).Take(Limit ?? total).ToList();
	}

	public void Display<T>(ICharacter actor, IEnumerable<T> source,
		Func<IEnumerable<T>, IEnumerable<IEnumerable<string>>> rows, IEnumerable<string> headers, Func<IEnumerable<T>, IEnumerable<T>>? defaultOrder = null) where T : IFrameworkItem
	{
		var selected = Select(source, out var total, defaultOrder);
		if (CountOnly) { actor.OutputHandler.Send($"{total.ToString("N0", actor)} matching records."); return; }
		var table = SortSpecified || Descending ? selected.SelectMany(item => rows(new[] { item })) : rows(selected);
		if (ExportCsv)
		{
			actor.OutputHandler.Send(string.Join("\n", new[] { headers }.Concat(table)
				.Select(row => string.Join(",", row.Select(CsvCell)))));
		}
		else
		{
			actor.OutputHandler.Send(StringUtilities.GetTextTable(table, headers, actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode));
		}
		actor.OutputHandler.Send($"Showing {selected.Count.ToString("N0", actor)} of {total.ToString("N0", actor)} matching records (page {Page.ToString("N0", actor)})." );
	}

	internal static string CsvCell(string value)
	{
		var plain = (value ?? "").StripANSIColour().StripMXP();
		// Export is intended for spreadsheets; neutralise formula execution in untrusted builder text.
		if ((plain.TrimStart().Length > 0 && "=+-@".Contains(plain.TrimStart()[0])) || plain.StartsWith('\t') || plain.StartsWith('\r')) plain = "'" + plain;
		return "\"" + plain.Replace("\"", "\"\"") + "\"";
	}
}
