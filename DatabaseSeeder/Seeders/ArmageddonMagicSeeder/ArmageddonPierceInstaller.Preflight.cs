extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonPierceInstaller
{
	private static void Preflight(FuturemudDatabaseContext db, ArmageddonPierceInstallPlan plan, List<Contribution> contributions,
		Dictionary<string, SeederManagedRecord> records, List<string> errors)
	{
		foreach (var contribution in contributions)
		{
			if (records.TryGetValue(contribution.Key, out var record))
			{
				if (record.Retired || record.EntityType != contribution.Type.Name || record.RevisionNumber is not null ||
					record.ManifestVersion != Version || Find(db, contribution, record) is null)
					errors.Add($"{contribution.Key}: owned row missing/retired/invalid; restore or explicitly rebind, no resurrection.");
				if (db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
					errors.Add($"{contribution.Key}: competing retained ownership claim.");
			}
			else if (db.SeederManagedRecords.Any(x => x.Seeder == Package && x.StableKey == contribution.Key))
				errors.Add($"{contribution.Key}: cross-module ownership key; no adoption.");
			else if (contribution.Type == typeof(MagicSpell) && (db.MagicSpells.Any(x => x.MagicSchoolId == plan.School && x.Name.ToLower() == contribution.Name.ToLower()) ||
				db.MagicSpells.AsNoTracking().Select(x => x.Definition).AsEnumerable().Any(x => StockIdentity(x) == contribution.Key)))
				errors.Add($"{contribution.Key}: unowned spell name/stock-identity collision.");
		}
		if (records.Keys.Except(contributions.Select(x => x.Key)).Any()) errors.Add("Unknown retained Pierce module keys require explicit version reconciliation.");
		if (new[] { plan.School, plan.Resource, plan.AlwaysFalseProg, plan.PierceSkill }.Any(x => x <= 0) ||
			!db.MagicSchools.Any(x => x.Id == plan.School) ||
			!db.MagicResources.Any(x => x.Id == plan.Resource && (x.MagicResourceType & (int)MagicResourceType.PlayerResource) != 0))
			errors.Add("Select an existing native school and player-capable source resource.");
		if (!db.TraitDefinitions.Any(x => x.Id == plan.PierceSkill && x.Type == 0 && x.OwnerScope == 1))
			errors.Add("Select the existing character-owned native Pierce skill.");
		var progs = db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray();
		using var compiler = new OfflineProgCompilation(progs);
		if (progs.SingleOrDefault(x => x.Id == plan.AlwaysFalseProg) is not { } no || no.FunctionText.Trim() != "return false" ||
			compiler.Compile(no.Id) is var compiled && (compiled.ReturnType != ProgVariableTypes.Boolean || !compiled.MatchesParameters([])))
			errors.Add("Select the ordinary compiled no-argument always-false support prog.");
		if (errors.Count != 0) return;
		var generated = ArmageddonReviewedPierceContent.PierceConcealment().EligibilityRow(0)!;
		generated.Id = -1;
		using var generatedCompiler = new OfflineProgCompilation(progs.Append(generated));
		var predicate = generatedCompiler.Compile(generated.Id);
		if (predicate.ReturnType != ProgVariableTypes.Boolean || !predicate.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Character]))
			errors.Add("Reviewed Pierce eligibility must compile as native boolean(character, character).");
	}

	private static string? StockIdentity(string xml)
	{
		try { return (string?)XElement.Parse(xml).Element("StockIdentity"); }
		catch { return null; }
	}
}
