extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonStockGatheringInstaller
{
	internal sealed record Bindings(long Trait, IReadOnlyDictionary<string, long> Progs);

	internal static Bindings Resolve(FuturemudDatabaseContext db)
	{
		ArmageddonStockSupportInstaller.ValidateGatheringGate(db);
		ArmageddonGatheringRegisters.Preflight(db);
		foreach (var definition in ArmageddonGatheringRegisters.Definitions)
			if (!db.VariableDefinitions.Any(x => x.OwnerTypeDefinition == definition.OwnerTypeDefinition && x.Property == definition.Property))
				throw new InvalidOperationException("Missing installed source gathering register definition.");
		var trait = Require<TraitDefinition>(db, ArmageddonProgressionInstaller.GatheringTraitKey, ArmageddonProgressionInstaller.Module);
		if (trait.Type != 0 || trait.OwnerScope != 1) throw new InvalidOperationException("Gather must remain a character skill.");
		Require<MagicGenerator>(db, ArmageddonProgressionInstaller.LandProfileKey, ArmageddonProgressionInstaller.Module);
		Require<MagicResource>(db, ArmageddonProgressionInstaller.LandMarkerResourceKey, ArmageddonProgressionInstaller.Module);
		var gate = Require<FutureProg>(db, ArmageddonStockSupportInstaller.GatheringAvailabilityKey, ArmageddonCatalogueOwnership.Module);
		var tags = TerrainClasses.ToDictionary(x => x.Key, x => Require<Tag>(db, TerrainTagKey(x.Key), ArmageddonCatalogueOwnership.Module).Id);
		var planes = PlaneKeys.Select(x => Require<Plane>(db, x, ArmageddonCatalogueOwnership.Module).Id).ToArray();
		var ash = Require<FutureProg>(db, AshVisibleKey, ArmageddonCatalogueOwnership.Module);
		var policies = Policies(trait.Id, gate.FunctionName, tags, planes, ash.Id);
		var all = db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray();
		var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		using var compiler = new OfflineProgCompilation(all, db.VariableDefinitions.AsNoTracking().ToArray());
		foreach (var entry in policies)
		{
			var row = Require<FutureProg>(db, entry.Key, ArmageddonCatalogueOwnership.Module);
			ValidateSignature(entry.Key, all.Single(x => x.Id == row.Id), entry.Value);
			compiler.Compile(row.Id);
			ids.Add(entry.Key, row.Id);
		}
		return new(trait.Id, ids);
	}

	internal static XElement Methods(string capability, long destination, Bindings bindings)
	{
		var kinds = capability switch
		{
			"arm.capability.sorcerer" => new[] { "Self", "Land" },
			"arm.capability.preserver" => ["Self"],
			"arm.capability.defiler" => ["Land"],
			_ => Array.Empty<string>()
		};
		return new XElement("Gathering", new XAttribute("version", 2), kinds.Select(kind =>
		{
			long Prog(string channel) => bindings.Progs[PolicyKey(kind, channel)];
			var method = new XElement("Method", new XAttribute("key", ArmageddonStockProgression.Identity(capability, "gather:" + kind.ToLowerInvariant())),
				new XAttribute("alias", kind.ToLowerInvariant()), new XAttribute("name", "Gather from " + kind.ToLowerInvariant()),
				new XAttribute("kind", kind), new XAttribute("destination", destination), new XAttribute("min", 1), new XAttribute("max", 1000),
				new XAttribute("duration", 0.25), new XAttribute("permission", Prog("permission")), new XAttribute("damageProg", Prog("damage")),
				new XAttribute("stunProg", Prog("stun")), new XAttribute("staminaProg", Prog("stamina")), new XAttribute("onGathered", Prog("success")),
				new XAttribute("maximumHealthSeverity", "Horrifying"), new XAttribute("structuralVersion", 1));
			if (kind == "Land") method.Add(new XElement("Land", new XAttribute("damage", 1), new XAttribute("pressure", 0),
				new XAttribute("damageProg", bindings.Progs[LandScarKey]), new[] { "crop", "woodland", "pasture" }.Select(selector =>
					new XElement("Source", new XAttribute("key", ArmageddonStockProgression.Identity(capability, "gather:land:" + selector)),
						new XAttribute("selector", selector), new XAttribute("ratio", 1), new XAttribute("allowAbsent", true),
						new XAttribute("collateral", false), new XAttribute("ratioProg", bindings.Progs[LandRatioKey])))));
			return method;
		}));
	}

	internal static XElement Support(string capability, Bindings bindings) => new("SupportGrant",
		new XAttribute("key", ArmageddonStockProgression.Identity(capability, ArmageddonProgressionInstaller.GatheringTraitKey)),
		new XAttribute("trait", bindings.Trait), new XAttribute("starting", true), new XAttribute("opening", 10), new XAttribute("rawCap", 90));
}
