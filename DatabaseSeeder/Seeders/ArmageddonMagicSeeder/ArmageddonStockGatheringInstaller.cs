extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders;

/// <summary>Source-faithful stock policies, with independently denied capability admission and no instance writes.</summary>
public static partial class ArmageddonStockGatheringInstaller
{
	private static readonly string[] PlaneKeys = [ArmageddonStockSupportInstaller.EarthPlaneKey,
		ArmageddonStockSupportInstaller.FirePlaneKey, ArmageddonStockSupportInstaller.WaterPlaneKey,
		ArmageddonStockSupportInstaller.WindPlaneKey, ArmageddonStockSupportInstaller.ShadowPlaneKey,
		ArmageddonStockSupportInstaller.LightningPlaneKey, ArmageddonStockSupportInstaller.VoidPlaneKey];

	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory)
	{
		using var db = factory();
		var messages = new List<string>();
		var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		var committed = false;
		var committing = false;
		var writing = false;
		try
		{
			var settings = ArmageddonInstallation.Read(db) ?? throw new InvalidOperationException("Install fixed Armageddon settings first.");
			if (db.ChangeTracker.HasChanges() || db.Database.CurrentTransaction is not null)
				throw new InvalidOperationException("Use a fresh dedicated context without pending writes or a transaction.");
			messages.AddRange(ArmageddonInstallation.ValidateFoundations(db, settings));
			if (messages.Count > 0) return new(ArmageddonInstallStatus.Blocked, messages, ids);
			ArmageddonStockSupportInstaller.ValidateGatheringGate(db);
			ArmageddonGatheringRegisters.Preflight(db);
			var skill = Require<TraitDefinition>(db, ArmageddonProgressionInstaller.GatheringTraitKey, ArmageddonProgressionInstaller.Module);
			if (skill.Type != 0 || skill.OwnerScope != 1) throw new InvalidOperationException("The owned Gather foundation must be a character skill.");
			var gate = Require<FutureProg>(db, ArmageddonStockSupportInstaller.GatheringAvailabilityKey, ArmageddonCatalogueOwnership.Module);
			var planes = PlaneKeys.Select(key => Require<Plane>(db, key, ArmageddonCatalogueOwnership.Module).Id).ToArray();
			var owned = new ArmageddonCatalogueOwnership(db, settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount], messages);
			var tags = TerrainClasses.ToDictionary(x => x.Key, x => owned.Id<Tag>(TerrainTagKey(x.Key)), StringComparer.Ordinal);
			var desired = Policies(skill.Id, gate.FunctionName, tags, planes, owned.Id<FutureProg>(AshVisibleKey));
			foreach (var entry in desired) entry.Value.Id = owned.Id<FutureProg>(entry.Key);
			var existingProgs = db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToDictionary(x => x.Id);
			var planned = new Dictionary<long, FutureProg>(existingProgs);
			foreach (var entry in desired)
			{
				var claim = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == entry.Key);
				if (claim is not null)
				{
					var actual = Require<FutureProg>(db, entry.Key, ArmageddonCatalogueOwnership.Module);
					db.Entry(actual).Collection(x => x.FutureProgsParameters).Load();
					ValidateSignature(entry.Key, actual, entry.Value);
					planned[actual.Id] = actual;
				}
				else planned[entry.Value.Id] = entry.Value;
			}
			var definitions = db.VariableDefinitions.AsNoTracking().ToList();
			foreach (var definition in ArmageddonGatheringRegisters.Definitions)
				if (!definitions.Any(x => x.OwnerTypeDefinition == definition.OwnerTypeDefinition && x.Property == definition.Property)) definitions.Add(definition);
			using (var compiler = new OfflineProgCompilation(planned.Values, definitions))
				foreach (var entry in desired) compiler.Compile(entry.Value.Id);
			foreach (var terrain in TerrainClasses)
			{
				var key = TerrainTagKey(terrain.Key);
				if (!db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key) &&
					db.Tags.Any(x => x.Name == "Armageddon gathering " + terrain.Name))
					throw new InvalidOperationException($"{key}: unowned terrain classification tag collision; no adoption.");
			}
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			writing = true;
			ArmageddonGatheringRegisters.Install(db, messages);
			foreach (var terrain in TerrainClasses)
			{
				var key = TerrainTagKey(terrain.Key);
				ids.Add(key, owned.Put(key, new Tag { Id = tags[terrain.Key], Name = "Armageddon gathering " + terrain.Name,
					ShouldSeeProgId = settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg] }, []).Id);
			}
			foreach (var entry in desired)
			{
				var dependencies = entry.Key == AshVisibleKey ? new[] { "arm.register.gather.ash_until" } :
					entry.Key.EndsWith(".success", StringComparison.Ordinal) ? entry.Key.Contains(".land.", StringComparison.Ordinal)
						? new[] { ArmageddonProgressionInstaller.GatheringTraitKey, "arm.register.gather.ecology", "arm.register.gather.alignment_until", "arm.register.gather.ash_until", AshVisibleKey }
						: new[] { ArmageddonProgressionInstaller.GatheringTraitKey } :
					entry.Key == LandRatioKey || entry.Key == LandScarKey ? [] :
					new[] { ArmageddonProgressionInstaller.GatheringTraitKey, ArmageddonStockSupportInstaller.GatheringAvailabilityKey,
						"arm.register.gather.ecology", "arm.register.gather.ash_until" }.Concat(PlaneKeys).Concat(tags.Keys.Select(TerrainTagKey)).ToArray();
				ids.Add(entry.Key, owned.Put(entry.Key, entry.Value, dependencies).Id);
			}
			var installed = db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray();
			using (var compiler = new OfflineProgCompilation(installed, db.VariableDefinitions.AsNoTracking().ToArray()))
				foreach (var entry in desired)
				{
					ValidateSignature(entry.Key, installed.Single(x => x.Id == ids[entry.Key]), entry.Value);
					compiler.Compile(ids[entry.Key]);
				}
			db.SaveChanges();
			committing = true; transaction?.Commit(); committed = true;
			messages.Add("Source gathering policies committed with exact typed native links: bodily Self/Land prices, explicit terrain classes, one native life-unit/scar per Land action, source alignment timing and separate additive ash expiry. No room/terrain bindings or instance register values changed. Independent gathering permission retains the installation defaults and builder edits.");
			return new(ArmageddonInstallStatus.Completed, messages, ids);
		}
		catch (Exception error)
		{
			if (!committed) ids.Clear();
			messages.Add((committed ? "Committed; confirmation failed. " : committing ? "Commit outcome unknown; reconnect before rerun. " : writing ? "Gathering policy batch failed before commit; relational transaction rolled back. " : "Gathering policy preflight blocked before writes. ") + error.Message);
			return new(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown :
				writing ? ArmageddonInstallStatus.Failed : ArmageddonInstallStatus.Blocked, messages, ids);
		}
	}

	internal static T Require<T>(FuturemudDatabaseContext db, string key, string module) where T : class
	{
		var claim = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		var version = module == ArmageddonProgressionInstaller.Module ? ArmageddonProgressionInstaller.Version : ArmageddonCatalogueOwnership.Version;
		if (claim is null || claim.Module != module || claim.ManifestVersion != version || claim.EntityType != typeof(T).Name ||
			claim.Retired || claim.LogicalId is not > 0 || claim.RevisionNumber is not null || db.SeederManagedRecords.Any(x =>
				x.Id != claim.Id && x.EntityType == claim.EntityType && x.LogicalId == claim.LogicalId))
			throw new InvalidOperationException($"{key}: missing compatible exact owned gathering dependency.");
		return db.Find<T>(claim.LogicalId.Value) ?? throw new InvalidOperationException($"{key}: native gathering dependency is missing.");
	}

	private static void ValidateSignature(string key, FutureProg actual, FutureProg desired)
	{
		if (actual.AcceptsAnyParameters || actual.StaticType != 0 || actual.ReturnTypeDefinition != desired.ReturnTypeDefinition ||
			!actual.FutureProgsParameters.OrderBy(x => x.ParameterIndex).Select(x => (x.ParameterIndex, x.ParameterName, x.ParameterTypeDefinition))
				.SequenceEqual(desired.FutureProgsParameters.OrderBy(x => x.ParameterIndex).Select(x => (x.ParameterIndex, x.ParameterName, x.ParameterTypeDefinition))))
			throw new InvalidOperationException($"{key}: retained gathering policy requires its exact non-static native signature.");
	}
}
