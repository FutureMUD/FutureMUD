#nullable enable
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Magic;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PreparedStockAttunementAcceptance(TestDatabase database)
	{
		Func<MudSharp.Database.FuturemudDatabaseContext> factory = () => NewIndependentContext(database.ConnectionString);
		var players = TraditionPlayers(database);
		var before = PreparedDatabaseChecksum(database);
		var stopped = ArmageddonStockSpellInstaller.Install(factory, (_, stage) =>
		{
			if (stage == ArmageddonInstallCheckpoint.BeforeCommit) throw new IOException("S4 contribution interruption before commit");
		});
		Require(stopped.Status == ArmageddonInstallStatus.Failed && before == PreparedDatabaseChecksum(database),
			"S4 native precommit rollback changed tables or returned the wrong outcome.");
		var lostAck = ArmageddonStockSpellInstaller.Install(factory, (key, stage) =>
		{
			if (key == ArmageddonCreatureSpellContent.Contributions.Last().Key && stage == ArmageddonInstallCheckpoint.AfterCommit) throw new IOException("S4 contribution lost acknowledgement");
		});
		Require(lostAck.Status == ArmageddonInstallStatus.CommittedConfirmationFailed, string.Join("\n", lostAck.Messages));
		before = PreparedDatabaseChecksum(database);
		var rerun = ArmageddonStockSpellInstaller.Install(factory);
		Require(rerun.Status == ArmageddonInstallStatus.Completed && before == PreparedDatabaseChecksum(database) && players == TraditionPlayers(database),
			"S4 reconciliation changed installed/player tables: " + string.Join("\n", rerun.Messages));
		using var db = factory();
		// Use collection instance Contains; span-based array overloads cannot be evaluated by EF9's parameter interpreter.
		var capabilityKeys = new ArmageddonStockProgression(ArmageddonInstallation.Read(db)!.Mode).Capabilities.Select(x => x.Key).ToList();
		var packageCaps = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			capabilityKeys.Contains(x.StableKey) && x.EntityType == nameof(Db.MagicCapability) && !x.Retired).Select(x => x.LogicalId).ToList();
		Require(packageCaps.Count == 10 && packageCaps.All(x => x is > 0) && packageCaps.Distinct().Count() == 10,
			"S4 requires the ten distinct typed package capability identities.");
		Require(db.MagicCapabilities.AsNoTracking().Where(x => packageCaps.Contains(x.Id)).AsEnumerable().All(x =>
			XElement.Parse(x.Definition).Element("Casting") is { } casting && (bool?)casting.Attribute("enabled") == false && !casting.Elements("Admission").Any()),
			"S4 installed a partial active package roster.");
		Console.WriteLine("ARMS4-attunement-install=passed actual-MySQL precommit-all-table-rollback lost-ack-resume stable-children no-op-all-table-checksum no-player-writes all-ten-package-caps-disabled");
		if (Environment.GetEnvironmentVariable("FUTUREMUD_S4_ATTUNEMENT_STOCK_SMOKE") != "1") return;
		// This separate, explicitly disposable builder capability qualifies the installed spell.
		// It is never package-owned and never activates a partial Armageddon roster.
		var settings = ArmageddonInstallation.Read(db)!;
		var ids = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.LogicalId != null).ToDictionary(x => x.StableKey, x => x.LogicalId!.Value);
		var spell = db.MagicSpells.AsNoTracking().Single(x => x.Id == ids[ArmageddonStockSpellContent.ReadAttunementKey]);
		var reserve = ids["arm.resource.water"];
		var actor = db.Characters.AsNoTracking().Single(x => x.IsAdminAvatar && x.AccountId == settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount]);
		var cap = new Db.MagicCapability
		{
			Name = "S4 Attunement Qualification", CapabilityModel = "skilllevel", MagicSchoolId = spell.MagicSchoolId, PowerLevel = 1,
			Definition = new XElement("Definition", new XElement("ConcentrationTrait", spell.CastingTraitDefinitionId),
				new XElement("ConcentrationCapabilityExpression", "1"), new XElement("ConcentrationDifficultyExpression", "5"), new XElement("Regenerators"),
				new XElement("Casting", new XAttribute("version", 1), new XAttribute("identity", Guid.NewGuid()), new XAttribute("enabled", true),
					new XAttribute("trait", spell.CastingTraitDefinitionId), new XAttribute("source", ids[ArmageddonProgressionInstaller.SourceResourceKey]),
					new XAttribute("reserve", reserve), new XAttribute("passive", false), new XAttribute("startingVersion", 1),
					new XElement("Admission", new XAttribute("key", Guid.NewGuid()), new XAttribute("spell", spell.Id), new XAttribute("starting", true),
						new XAttribute("min", 1), new XAttribute("max", 7), new XAttribute("opening", 100), new XAttribute("rawCap", 100))),
				new XElement("Gathering", new XAttribute("version", 2), new XElement("Method", new XAttribute("key", Guid.NewGuid()), new XAttribute("alias", "draw"),
					new XAttribute("name", "S4 qualification paid Self gathering"), new XAttribute("kind", "Self"), new XAttribute("destination", reserve),
					new XAttribute("min", 1), new XAttribute("max", 100), new XAttribute("duration", 2), new XAttribute("ratio", 1), new XAttribute("stamina", 1),
					new XAttribute("minimumStamina", 0), new XAttribute("maximumHealthSeverity", "None")))).ToString()
		};
		db.Add(cap); db.SaveChanges();
		var merit = new Db.Merit
		{
			Name = "S4 Attunement Qualification Caster", Type = "Magic Capability", MeritScope = 2, MeritType = 0,
			Definition = new XElement("Definition", new XElement("ChargenAvailableProg", settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg]),
				new XElement("ApplicabilityProg", settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]), new XElement("ChargenBlurb", "Disposable S4 acceptance only"),
				new XElement("DescriptionText", "@ have|has an acceptance capability."), new XElement("Capabilities", new XElement("Capability", cap.Id))).ToString()
		};
		db.Add(merit); db.SaveChanges();
		Require(players == TraditionPlayers(database), "S4 qualification preparation granted, refilled or changed a player.");
		var input = Path.Combine(Directory.GetParent(CatalogueRepository)!.FullName, "prepared-stock-attunement-input_" + Guid.NewGuid().ToString("N") + ".json");
		File.WriteAllText(input, JsonSerializer.Serialize(new
		{
			sql_schema = InstalledSqlSchema(db),
			mode = settings.Mode.ToString(), classes_enabled = settings.ClassesEnabled,
			character = actor.Id, body = actor.BodyId, spell = spell.Id, resource = reserve, capability = cap.Id, merit = merit.Id,
			trait = spell.CastingTraitDefinitionId, school = spell.MagicSchoolId, source = ids[ArmageddonProgressionInstaller.SourceResourceKey],
			definition = spell.Definition, duration = spell.EffectDurationExpressionId,
			stock_spells = (from claim in db.SeederManagedRecords.AsNoTracking()
				join native in db.MagicSpells.AsNoTracking() on claim.LogicalId equals (long?)native.Id
				where claim.Seeder == ArmageddonMagicInstaller.Package && claim.Module == ArmageddonStockSpellInstaller.Module &&
					claim.EntityType == nameof(Db.MagicSpell) && !claim.Retired
				orderby claim.StableKey
				select new { key = claim.StableKey, id = native.Id, name = native.Name, definition = native.Definition }).ToArray(),
			package_capabilities = db.SeederManagedRecords.AsNoTracking().Where(x => x.EntityType == nameof(Db.MagicCapability) && x.Module == ArmageddonProgressionInstaller.Module).Select(x => x.LogicalId).ToArray(),
			limits = "Explicit separate builder qualification capability; native enrolment starts grade one, no stock roster activation, no S5 natural progression/affordability certificate."
		}));
		Environment.SetEnvironmentVariable("FUTUREMUD_S4_ATTUNEMENT_STOCK_INPUT", input);
		Console.WriteLine("ARMS4-attunement-qualification-input=" + input + " separate-builder-fixture no-player-mutation");
	}
}
