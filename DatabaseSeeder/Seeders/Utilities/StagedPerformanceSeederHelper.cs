#nullable enable

extern alias EngineCompiler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using EnginePerformanceAuthoring = EngineCompiler::MudSharp.RPG.Performances.PerformanceAuthoring;
using EnginePerformanceWork = EngineCompiler::MudSharp.RPG.Performances.PerformanceWork;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.Utilities;

public enum StagedPerformanceSeedResult
{
	Created,
	AlreadyInstalled
}

/// <summary>Creates immutable, published performance work records for setting seeders.</summary>
public static class StagedPerformanceSeederHelper
{
	public static StagedPerformanceSeedResult EnsureWork(
		FuturemudDatabaseContext context,
		string seedKey,
		EnginePerformanceWork definition,
		bool trustedStaffAuthored = false)
	{
		ArgumentNullException.ThrowIfNull(context);
		ArgumentNullException.ThrowIfNull(definition);

		if (!IsStableSeedKey(seedKey))
			throw new ArgumentException("Supply a lowercase, package-namespaced seed key such as 'setting.package-work' using only letters, digits, periods, and hyphens.", nameof(seedKey));

		var work = JsonSerializer.Deserialize<EnginePerformanceWork>(JsonSerializer.Serialize(definition))
			?? throw new InvalidOperationException($"Work for seed key '{seedKey}' could not be copied.");
		if (!string.IsNullOrWhiteSpace(work.SeedKey) && work.SeedKey != seedKey)
			throw new InvalidOperationException($"Work '{work.Name}' already carries seed key '{work.SeedKey}', not '{seedKey}'.");

		work.Id = 0;
		work.OwnerId = 0;
		work.SeedKey = seedKey;
		work.Revision = 1;
		work.Published = true;
		work.StaffAuthored = trustedStaffAuthored;

		var structuralProblem = EnginePerformanceAuthoring.ValidateDefinition(work);
		if (structuralProblem is not null)
			throw new InvalidOperationException($"Seeded performance '{seedKey}' is invalid: {structuralProblem}");

		ValidateEligibilityProgs(context, seedKey, work, trustedStaffAuthored);

		var works = context.StagedPerformanceRecords.AsNoTracking()
			.Where(x => x.Kind == "work")
			.Select(x => x.Definition)
			.AsEnumerable()
			.Select(Deserialize)
			.ToArray();

		var matches = works.Where(x => x.SeedKey == seedKey).ToArray();
		if (matches.Length > 1)
			throw new InvalidOperationException($"More than one staged performance work carries seed key '{seedKey}'. Resolve the duplicate records before rerunning this seeder.");

		if (matches.Length == 1)
		{
			var existing = matches[0];
			if (!existing.Published)
				throw new InvalidOperationException($"Staged performance '{seedKey}' is no longer published. The seeder left it unchanged.");
			if (!string.Equals(existing.Name, work.Name, StringComparison.Ordinal))
				throw new InvalidOperationException($"Staged performance seed key '{seedKey}' is already assigned to '{existing.Name}'. The seeder left it unchanged.");
			return StagedPerformanceSeedResult.AlreadyInstalled;
		}

		var sameName = works.FirstOrDefault(x => string.Equals(x.Name, work.Name, StringComparison.OrdinalIgnoreCase));
		if (sameName is not null)
		{
			var owner = sameName.SeedKey is null
				? "an existing work without this seed key"
				: $"seed key '{sameName.SeedKey}'";
			throw new InvalidOperationException($"Cannot install staged performance '{seedKey}' as '{work.Name}' because that name is already used by {owner}. The existing work was preserved.");
		}

		context.StagedPerformanceRecords.Add(new StagedPerformanceRecord
		{
			Kind = "work",
			Definition = JsonSerializer.Serialize(work)
		});
		context.SaveChanges();
		return StagedPerformanceSeedResult.Created;
	}

	private static EnginePerformanceWork Deserialize(string definition) =>
		JsonSerializer.Deserialize<EnginePerformanceWork>(definition)
		?? throw new InvalidOperationException("A staged performance work record contains invalid JSON.");

	private static bool IsStableSeedKey(string seedKey) =>
		!string.IsNullOrWhiteSpace(seedKey) &&
		seedKey == seedKey.Trim().ToLowerInvariant() &&
		seedKey.Contains('.') &&
		seedKey.Split('.').All(segment => segment.Length > 0 &&
			segment.All(character => char.IsAsciiLetterOrDigit(character) || character == '-'));

	private static void ValidateEligibilityProgs(
		FuturemudDatabaseContext context,
		string seedKey,
		EnginePerformanceWork work,
		bool trustedStaffAuthored)
	{
		var eligibilityIds = work.Roles.Where(x => x.EligibilityProgId > 0)
			.Select(x => x.EligibilityProgId)
			.Distinct()
			.ToArray();
		if (eligibilityIds.Length == 0) return;

		var progs = context.FutureProgs.AsNoTracking()
			.Where(x => eligibilityIds.Contains(x.Id))
			.ToDictionary(x => x.Id);
		var parameters = context.FutureProgsParameters.AsNoTracking()
			.Where(x => eligibilityIds.Contains(x.FutureProgId))
			.ToArray()
			.GroupBy(x => x.FutureProgId)
			.ToDictionary(x => x.Key, x => x.OrderBy(y => y.ParameterIndex).ToArray());

		foreach (var role in work.Roles.Where(x => x.EligibilityProgId > 0))
		{
			if (!progs.TryGetValue(role.EligibilityProgId, out var prog))
				throw new InvalidOperationException($"Staged performance '{seedKey}' role '{role.Key}' references missing eligibility prog {role.EligibilityProgId}.");
			if (!prog.Public && !trustedStaffAuthored)
				throw new InvalidOperationException($"Staged performance '{seedKey}' role '{role.Key}' uses private prog '{prog.FunctionName}'. Seeded work is visible to all builders, so its eligibility prog must be public.");
			var acceptsCharacter = prog.AcceptsAnyParameters ||
			                       parameters.TryGetValue(prog.Id, out var args) && args.Length == 1 &&
			                       ProgVariableTypes.Character.CompatibleWith((ProgVariableTypes)args[0].ParameterType);
			if (!acceptsCharacter || !((ProgVariableTypes)prog.ReturnType).CompatibleWith(ProgVariableTypes.Boolean))
				throw new InvalidOperationException($"Staged performance '{seedKey}' role '{role.Key}' uses '{prog.FunctionName}'. Choose a prog that returns Boolean and accepts one Character parameter.");
		}
	}
}
