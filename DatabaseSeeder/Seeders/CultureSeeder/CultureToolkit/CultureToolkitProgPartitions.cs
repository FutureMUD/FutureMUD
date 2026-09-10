#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.CultureToolkit;

internal sealed record CulturePartitionedProg(FutureProg Main, IReadOnlyList<long> SupportingIds);

/// <summary>Keep ordinary editable FutureProg bodies below MySQL TEXT capacity without changing public contracts.</summary>
internal static class CultureToolkitProgPartitions
{
	internal static CulturePartitionedProg Upsert(FuturemudDatabaseContext context, string era, string key, string name,
		string header, IEnumerable<string> blocks, string footer, ProgVariableTypes returns,
		IReadOnlyList<(ProgVariableTypes Type, string Name)> parameters, ICollection<string> conflicts, string? collection = null)
	{
		var pieces = new List<string>();
		var current = new StringBuilder();
		foreach (var block in blocks)
		{
			if (Encoding.UTF8.GetByteCount(header + block + footer) > 48000)
				throw new InvalidOperationException($"{name}: one generated clause exceeds the prog partition size.");
			if (current.Length > 0 && Encoding.UTF8.GetByteCount(header + current + block + footer) > 48000)
			{
				pieces.Add(header + current + footer);
				current.Clear();
			}
			current.Append(block);
		}
		pieces.Add(header + current + footer);
		if (pieces.Count == 1)
			return new(CultureToolkitProgSeeder.Upsert(context, era, key, name, pieces[0], returns, parameters, conflicts), []);
		var parts = pieces.Select((body, index) => CultureToolkitProgSeeder.Upsert(context, era, $"{key}.part.{index + 1}",
			$"{name}Part{index + 1}", body, returns, parameters, conflicts)).ToArray();
		var arguments = string.Join(", ", parameters.Select(x => "@" + x.Name));
		var dispatch = collection is null
			? "var result as number\nvar candidate as number\nresult = 0\n" + string.Concat(parts.Select(x =>
				$"candidate = @{x.FunctionName}({arguments})\nif (@candidate > @result)\n  result = @candidate\nend if\n")) + "return @result"
			: $"var {collection} as {(returns.HasFlag(ProgVariableTypes.Trait) ? "trait" : "knowledge")} collection\n" + string.Concat(parts.Select(x =>
				$"foreach (entry in @{x.FunctionName}({arguments}))\n  if (not(Contains(@{collection}, @entry)))\n    additem {collection} @entry\n  end if\nend foreach\n")) + $"return @{collection}";
		return new(CultureToolkitProgSeeder.Upsert(context, era, key, name, dispatch, returns, parameters, conflicts), parts.Select(x => x.Id).ToArray());
	}
}
