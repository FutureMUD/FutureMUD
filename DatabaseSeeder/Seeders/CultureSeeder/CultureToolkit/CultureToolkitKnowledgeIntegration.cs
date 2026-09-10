#nullable enable

using System;
using System.Collections.Generic;
using System.Text.Json;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.CultureToolkit;

internal sealed record CultureWritingLegacyBlock(long TargetId, string Body);

internal static class CultureToolkitKnowledgeIntegration
{
	internal static void PreserveLegacyBlock(FuturemudDatabaseContext context, string era, CultureWritingLegacyBlock? block)
	{
		if (block is null || CultureToolkitManagedEntities.Find(context, "LegacyWritingBlock", $"writing.legacy.{block.TargetId}") is not null) return;
		context.SeederManagedRecords.Add(new SeederManagedRecord
		{
			Seeder = "CultureSeeder", EntityType = "LegacyWritingBlock", StableKey = $"writing.legacy.{block.TargetId}", Module = era,
			LogicalId = block.TargetId, ManifestVersion = "2026-09-07", AppliedAt = DateTime.UtcNow,
			SeedBaseline = JsonSerializer.Serialize(new Dictionary<string, string> { ["body"] = block.Body })
		});
		context.SaveChanges();
	}

	internal static bool IsInstalled(FuturemudDatabaseContext context) =>
		CultureToolkitManagedEntities.Find(context, "FutureProg", "writing.knowledges") is not null;

	internal static string Block(FuturemudDatabaseContext context, FutureProg target, string? existing,
		string broad, string newline, bool persist, ICollection<string> issues)
	{
		var helperRecord = CultureToolkitManagedEntities.Find(context, "FutureProg", "writing.knowledges");
		if (helperRecord is null) return broad;
		var appliesRecord = CultureToolkitManagedEntities.Find(context, "FutureProg", "writing.applies")
			?? throw new InvalidOperationException("Writing policy has no resolved applicability prog.");
		var helper = context.FutureProgs.Find(helperRecord.LogicalId)!;
		var applies = context.FutureProgs.Find(appliesRecord.LogicalId)!;
		CultureToolkitProgSeeder.Validate(helper, ProgVariableTypes.Knowledge | ProgVariableTypes.Collection, ProgVariableTypes.Chargen);
		CultureToolkitProgSeeder.Validate(applies, ProgVariableTypes.Boolean, ProgVariableTypes.Chargen);
		var start = ChargenFreeKnowledgeProgReconciler.CultureStartMarker;
		var end = ChargenFreeKnowledgeProgReconciler.CultureEndMarker;
		var legacy = CultureToolkitManagedEntities.Find(context, "LegacyWritingBlock", $"writing.legacy.{target.Id}");
		if (legacy is not null) broad = JsonSerializer.Deserialize<Dictionary<string, string>>(legacy.SeedBaseline!)!["body"];
		var broadBody = broad.Length == 0 ? "" : broad[start.Length..^end.Length].Trim();
		var desired = $"{start}\nif (@{applies.FunctionName}(@ch))\n  foreach (knowledge in @{helper.FunctionName}(@ch))\n    if (not(Contains(@knowledges, @knowledge)))\n      additem knowledges @knowledge\n    end if\n  end foreach\nelse\n{broadBody}\nend if\n{end}"
			.Replace("\n", newline);
		var key = $"writing.integration.{target.Id}";
		var stored = CultureToolkitManagedEntities.Find(context, "ProgBlock", key);
		var record = persist ? stored : null;
		// Preview uses a detached copy: checking drift must not advance an ownership baseline.
		record ??= new SeederManagedRecord
		{
			Seeder = "CultureSeeder", EntityType = "ProgBlock", StableKey = key, Module = helperRecord.Module,
			LogicalId = target.Id, ManifestVersion = "2026-09-07", AppliedAt = DateTime.UtcNow, SeedBaseline = stored?.SeedBaseline
		};
		var verifiedStock = stored is null && (existing is null || existing == broad);
		if (verifiedStock && existing is not null) record.SeedBaseline = JsonSerializer.Serialize(new Dictionary<string, string> { ["body"] = existing });
		var result = SeederManagedRecordReconciler.Reconcile(record,
			existing is null ? new Dictionary<string, string>() : new Dictionary<string, string> { ["body"] = existing },
			new Dictionary<string, string> { ["body"] = desired }, verifiedStock && existing is null, issues);
		if (persist && stored is null) context.SeederManagedRecords.Add(record);
		return result.GetValueOrDefault("body") ?? "";
	}
}
