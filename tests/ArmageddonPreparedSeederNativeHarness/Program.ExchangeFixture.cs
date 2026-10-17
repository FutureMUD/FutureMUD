#nullable enable

using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Magic;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Framework;
using MudSharp.Body;
using MudSharp.Database;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PrepareExchangeFixture(TestDatabase database, bool installedStock = false)
	{
		Func<MudSharp.Database.FuturemudDatabaseContext> factory = () => NewIndependentContext(database.ConnectionString);
		var players = TraditionPlayers(database);
		if (!installedStock)
		{
			var installed = ArmageddonStockSpellInstaller.Install(factory);
			Require(installed.Status == ArmageddonInstallStatus.Completed, string.Join("; ", installed.Messages));
			SaveExchangeStockFixture(database);
		}
		Require(players == TraditionPlayers(database), "Exchange stock install changed player state.");
		// Reconciliation/rollback is covered by the unchanged shared installer and its
		// owning tests. This focused restored-world lane exercises the new paid move.
		using var db = factory();
		var settings = ArmageddonInstallation.Read(db)!;
		var ids = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package && !x.Retired && x.LogicalId != null)
			.ToDictionary(x => x.StableKey, x => x.LogicalId!.Value);
		var packageCaps = db.SeederManagedRecords.AsNoTracking().Where(x => x.EntityType == nameof(Db.MagicCapability) &&
			x.Module == ArmageddonProgressionInstaller.Module && !x.Retired).Select(x => x.LogicalId).ToList();
		Require(!settings.ClassesEnabled && packageCaps.Count == 10 && db.MagicCapabilities.AsNoTracking().Where(x => packageCaps.Contains(x.Id))
			.AsEnumerable().All(x => XElement.Parse(x.Definition).Element("Casting") is { } casting &&
				(bool?)casting.Attribute("enabled") == false && !casting.Elements("Admission").Any()), "Exchange fixture changed package gates.");
		var spell = db.MagicSpells.AsNoTracking().Single(x => x.Id == ids[ArmageddonExchangePlacesSpellContent.Key]);
		var school = db.MagicSchools.AsNoTracking().Single(x => x.Id == spell.MagicSchoolId);
		var reserve = ids["arm.resource.wind"];
		var route = PreparedStockCampaignRoute(db, settings, ArmageddonExchangePlacesSpellContent.Key, spell, reserve,
			ids[ArmageddonProgressionInstaller.SourceResourceKey], 1);
		// An isolated native contact power establishes the source selector. It is deliberately
		// separate from the disabled package and is invoked normally over Telnet.
		var stock = PsionicStockContent.Powers.Single(x => x.Verb == "contact");
		var definition = PsionicStockContent.Definition(stock, spell.CastingTraitDefinitionId!.Value, reserve,
			settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg], settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg],
			settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg], settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]);
		definition.Element("InvocationCosts")!.Remove(); definition.Element("SustainResourceCosts")!.Remove();
		definition.SetElementValue("ConcentrationPointsToSustain", 0);
		var power = new Db.MagicPower { Name = "S4 Exchange Contact Qualification", Blurb = "Disposable native contact fixture", ShowHelp = "contact <local target>", MagicSchoolId = spell.MagicSchoolId,
			PowerModel = "connectmind", Definition = definition.ToString() };
		db.Add(power); db.SaveChanges();
		var capability = db.MagicCapabilities.Single(x => x.Id == route.Capability);
		var root = XElement.Parse(capability.Definition);
		root.Add(new XElement("Power", new XAttribute("trait", spell.CastingTraitDefinitionId.Value), new XAttribute("minvalue", 0), new XAttribute("power", power.Id)));
		capability.Definition = root.ToString(); db.SaveChanges();
		var actor = db.Characters.AsNoTracking().Single(x => x.IsAdminAvatar && x.AccountId == settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount]);
		var pouch = CreateExchangePouch(db, settings, ids);
		var input = Path.Combine(Directory.GetParent(CatalogueRepository)!.FullName, "prepared-stock-attunement-input_" + Guid.NewGuid().ToString("N") + ".json");
		File.WriteAllText(input, JsonSerializer.Serialize(new
		{
			character = actor.Id, body = actor.BodyId, package_capabilities = packageCaps,
			exchange_places = new { spell = spell.Id, capability = route.Capability, merit = route.Merit, resource = reserve,
				invoked = school.SchoolVerb.ToLowerInvariant(), cost = 50, contact_power = power.Id, pouch = pouch.Id,
				target_template = ids["arm.npc.earth_mount.g1"],
				teleportation = ids["arm.material.teleportation.rank0"], alteration = ids["arm.material.alteration.rank0"] }
		}, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }));
		Environment.SetEnvironmentVariable("FUTUREMUD_S4_ATTUNEMENT_STOCK_INPUT", input);
		Console.WriteLine("ARMS4-Exchange-focused-input=" + input + " native-contact separate-builder-route package-gates-disabled");
	}

	private static Db.GameItemProto CreateExchangePouch(MudSharp.Database.FuturemudDatabaseContext db,
		ArmageddonInstallationSettings settings, IReadOnlyDictionary<string, long> ids)
	{
		var body = db.Races.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.HumanRace]).BaseBodyId;
		var lineage = new List<long>(); long? current = body;
		while (current is { } id)
		{
			Require(!lineage.Contains(id) && lineage.Count < 32, "Invalid native body inheritance for the pouch fixture.");
			lineage.Add(id); current = db.BodyProtos.Single(x => x.Id == id).CountsAsId;
		}
		var profiles = db.WearProfiles.Where(x => x.Name == "Waist").AsEnumerable().Where(x => lineage.Contains(x.BodyPrototypeId)).ToArray();
		var profileBody = lineage.First(id => profiles.Any(x => x.BodyPrototypeId == id));
		var waist = profiles.Single(x => x.BodyPrototypeId == profileBody);
		var kilograms = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
		Db.EditableItem Approval() => new() { BuilderAccountId = settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount],
			ReviewerAccountId = settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount], RevisionStatus = (int)RevisionStatus.Current,
			BuilderDate = DateTime.UnixEpoch, ReviewerDate = DateTime.UnixEpoch, BuilderComment = "Disposable native Exchange fixture", ReviewerComment = "Disposable fixture" };
		var next = db.GameItemComponentProtos.Max(x => x.Id) + 1;
		var wearable = new Db.GameItemComponentProto { Id = next, Name = "S4 Exchange pouch wearable", Description = "Disposable native worn container",
			Type = "Wearable", EditableItem = Approval(), Definition = new XElement("Definition",
				new XElement("Profiles", new XAttribute("Default", waist.Id), new XElement("Profile", waist.Id))).ToString() };
		var container = new Db.GameItemComponentProto { Id = next + 1, Name = "S4 Exchange pouch container", Description = "Disposable openable pouch",
			Type = "Container", EditableItem = Approval(), Definition = new XElement("Definition", new XAttribute("Weight", 100 * kilograms),
				new XAttribute("MaxSize", (int)SizeCategory.Normal), new XAttribute("Preposition", "in"), new XAttribute("Closable", true)).ToString() };
		db.AddRange(wearable, container); db.SaveChanges();
		var holdable = db.GameItemComponentProtos.Include(x => x.EditableItem).Single(x => x.Id == ids["arm.catalogue.component.holdable"] && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current);
		var pouch = new Db.GameItemProto { Id = db.GameItemProtos.Max(x => x.Id) + 1, Name = "qualification pouch", Keywords = "qualification pouch",
			ShortDescription = "a disposable qualification pouch", FullDescription = "An ordinary openable worn pouch for the isolated native scenario.",
			MaterialId = settings.NativeBindings[ArmageddonNativeBinding.Wood], Size = (int)SizeCategory.Small, Weight = .1 * kilograms,
			BaseItemQuality = (int)ItemQuality.Standard, MorphEmote = "$0 changes.", EditableItem = Approval() };
		foreach (var component in new[] { holdable, wearable, container }) pouch.GameItemProtosGameItemComponentProtos.Add(new()
			{ GameItemComponentProtoId = component.Id, GameItemComponentRevision = component.RevisionNumber });
		db.Add(pouch); db.SaveChanges(); return pouch;
	}

	private sealed record ExchangeStockFixture(string Kind, string Database, string SqlFile, string SqlSha256, string BaseManifestSha256, string PackageSourceSha256);
	private static string ExchangeFixtureRoot => Path.Combine(CatalogueRepository, ".artifacts", "s4-exchange-fixtures");
	private static void SaveExchangeStockFixture(TestDatabase database)
	{
		var folder = Path.Combine(ExchangeFixtureRoot, DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ") + "_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(folder); RequireBasePath(folder, ExchangeFixtureRoot, directory: true);
		var sql = new MySqlDatabaseBackupService().CreateBackup(database.ConnectionString, folder);
		RequireBaseDumpTargets(File.ReadAllText(sql), database.Name);
		var manifest = Path.Combine(folder, "exchange-stock.json");
		File.WriteAllText(manifest, JsonSerializer.Serialize(new ExchangeStockFixture("FutureMUD-S4-Exchange-v1", database.Name,
			Path.GetFileName(sql), BaseHash(sql), Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE_SHA256")!, CatalogueContentSourceHash()), BaseJsonOptions));
		Console.WriteLine("ARMS4-Exchange-stock-fixture=" + manifest + " sha256:" + BaseHash(manifest));
	}

	private static int RestoreExchangeStockFixture()
	{
		var manifest = Path.GetFullPath(Environment.GetEnvironmentVariable("FUTUREMUD_S4_EXCHANGE_FIXTURE") ?? throw new InvalidOperationException("Missing explicit Exchange fixture."));
		RequireBasePath(manifest, ExchangeFixtureRoot);
		Require(Path.GetFileName(manifest) == "exchange-stock.json" && BaseHash(manifest) == Environment.GetEnvironmentVariable("FUTUREMUD_S4_EXCHANGE_FIXTURE_SHA256"), "Exchange descriptor digest differs.");
		var fixture = JsonSerializer.Deserialize<ExchangeStockFixture>(File.ReadAllText(manifest)) ?? throw new InvalidOperationException("Malformed Exchange descriptor.");
		Require(fixture.Kind == "FutureMUD-S4-Exchange-v1" && TestDatabase.HasOwnedPrefix(fixture.Database) &&
			fixture.PackageSourceSha256 == CatalogueContentSourceHash() && fixture.BaseManifestSha256 == Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE_SHA256") &&
			fixture.SqlFile == Path.GetFileName(fixture.SqlFile), "Exchange fixture source, base or identity differs.");
		var sql = Path.Combine(Path.GetDirectoryName(manifest)!, fixture.SqlFile); RequireBasePath(sql, ExchangeFixtureRoot);
		Require(BaseHash(sql) == fixture.SqlSha256, "Exchange SQL digest differs."); RequireBaseDumpTargets(File.ReadAllText(sql), fixture.Database);
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		using (var db = NewIndependentContext(database.ConnectionString)) Require(!db.Accounts.Any() && !db.SeederChoices.Any(), "Exchange restore requires a fresh owned target.");
		database.RestoreCatalogueFixture(sql, fixture.Database); ConfigureNativeDatabase(database.ConnectionString);
		using (var db = NewIndependentContext(database.ConnectionString)) Require(!db.Database.GetPendingMigrations().Any() && !db.Database.HasPendingModelChanges(), "Exchange restored schema differs.");
		PrepareExchangeFixture(database, installedStock: true);
		RunPreparedReplayBoot(database, typeof(ArmageddonMagicSeeder).Assembly, "PreparedCatalogueBootSmoke.py", "prepared-catalogue-boot-native");
		Require(BaseHash(sql) == fixture.SqlSha256, "Canonical Exchange SQL changed during qualification.");
		WriteBaseRunReceipt(new { Status = "RESTORED_EXCHANGE_STOCK", Manifest = manifest, ManifestSha256 = BaseHash(manifest), fixture.SqlSha256,
			Database = database.Name, Ownership = "fresh-owned-marker-resealed", PackageRerun = false });
		return 0;
	}
}
