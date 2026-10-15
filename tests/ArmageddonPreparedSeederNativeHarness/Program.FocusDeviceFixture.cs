#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PrepareFocusDeviceFixture(TestDatabase database)
	{
		Func<MudSharp.Database.FuturemudDatabaseContext> factory = () => NewIndependentContext(database.ConnectionString);
		var before = PreparedDatabaseChecksum(database);
		var result = ArmageddonStockFocusDeviceInstaller.Install(factory);
		Require(result.Status == ArmageddonInstallStatus.Completed && before == PreparedDatabaseChecksum(database), "S4 focus/device native rerun changed installed tables.");
		using var db = factory();
		var settings = ArmageddonInstallation.Read(db)!;
		var claims = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package && !x.Retired && x.LogicalId > 0)
			.ToDictionary(x => x.StableKey, x => x.LogicalId!.Value);
		var path = Environment.GetEnvironmentVariable("FUTUREMUD_S4_ATTUNEMENT_STOCK_INPUT")!;
		var input = JsonNode.Parse(File.ReadAllText(path))!.AsObject();
		var characterId = input["character"]!.GetValue<long>();
		var craftingId = claims[ArmageddonStockProgression.CraftTraitKey];
		var crafting = db.CharacterTraits.SingleOrDefault(x => x.CharacterId == characterId && x.TraitDefinitionId == craftingId);
		if (crafting is null) db.CharacterTraits.Add(new() { CharacterId = characterId, TraitDefinitionId = craftingId, Value = 90 });
		else crafting.Value = 90;
		// A controlled ordinary work room supplies light; this grants no casting admission.
		var actor = db.Characters.Single(x => x.Id == characterId);
		var room = db.Rooms.Include(x => x.CurrentOverlay).Single(x => x.Id == actor.Location);
		room.CurrentOverlay.AddedLight = 1000;
		db.SaveChanges();
		var read = db.MagicSpells.Single(x => x.Id == claims["arm.spell.read_attunement"]);
		var mend = db.MagicSpells.Single(x => x.Id == claims["arm.spell.mend_flesh"]);
		var reserve = claims["arm.resource.water"];
		var route = PreparedStockCampaignRoute(db, settings, "arm.spell.mend_flesh", mend, reserve, claims[ArmageddonProgressionInstaller.SourceResourceKey], 1);
		// Disposable copies change only availability. The twelve owned prototypes, all
		// original recipes, both stock gates and all package capability policies stay disabled.
		var samples = new List<object>();
		foreach (var key in new[] { "arm.focus.water", "arm.item.blank_wand", "arm.item.blank_staff" })
		{
			var original = db.GameItemProtos.AsNoTracking().Single(x => x.Id == claims[key]);
			var component = db.GameItemComponentProtos.AsNoTracking().Single(x => x.Id == claims[key + ".component"]);
			var copyComponent = new Db.GameItemComponentProto(); db.Entry(copyComponent).CurrentValues.SetValues(component);
			copyComponent.Id = db.GameItemComponentProtos.Max(x => x.Id) + 1;
			copyComponent.Name = "S4 disposable " + key;
			var xml = XElement.Parse(component.Definition); xml.SetElementValue("UseProg", 0); copyComponent.Definition = xml.ToString();
			db.Add(copyComponent); db.SaveChanges();
			var copyItem = new Db.GameItemProto(); db.Entry(copyItem).CurrentValues.SetValues(original);
			copyItem.Id = db.GameItemProtos.Max(x => x.Id) + 1; copyItem.Name = "S4 disposable " + key;
			copyItem.Keywords = key.EndsWith("water") ? "focus" : key.EndsWith("wand") ? "wand" : "staff";
			foreach (var link in db.GameItemProtosGameItemComponentProtos.AsNoTracking().Where(x => x.GameItemProtoId == original.Id && x.GameItemProtoRevision == original.RevisionNumber))
				copyItem.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = link.GameItemComponentProtoId == component.Id ? copyComponent.Id : link.GameItemComponentProtoId, GameItemComponentRevision = link.GameItemComponentRevision });
			db.Add(copyItem); db.SaveChanges();
			var originalCraft = db.Crafts.Include(x => x.CraftInputs).Include(x => x.CraftProducts).Include(x => x.CraftPhases).Single(x => x.Id == claims[key + ".recipe"]);
			var copyCraft = new Db.Craft(); db.Entry(copyCraft).CurrentValues.SetValues(originalCraft);
			copyCraft.Id = db.Crafts.Max(x => x.Id) + 1; copyCraft.Name = "S4 sample " + copyItem.Keywords;
			copyCraft.AppearInCraftsListProgId = settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]; copyCraft.CanUseProgId = copyCraft.AppearInCraftsListProgId;
			foreach (var phase in originalCraft.CraftPhases) copyCraft.CraftPhases.Add(new() { PhaseNumber = phase.PhaseNumber, PhaseLengthInSeconds = phase.PhaseLengthInSeconds, Echo = phase.Echo, FailEcho = phase.FailEcho, ExertionLevel = phase.ExertionLevel, StaminaUsage = phase.StaminaUsage });
			foreach (var ingredient in originalCraft.CraftInputs) copyCraft.CraftInputs.Add(new() { InputType = ingredient.InputType, Definition = ingredient.Definition, InputQualityWeight = ingredient.InputQualityWeight, OriginalAdditionTime = ingredient.OriginalAdditionTime });
			foreach (var product in originalCraft.CraftProducts)
			{
				var definition = XElement.Parse(product.Definition); definition.SetElementValue("ProductProducedId", copyItem.Id);
				copyCraft.CraftProducts.Add(new() { ProductType = product.ProductType, Definition = definition.ToString(), OriginalAdditionTime = product.OriginalAdditionTime });
			}
			db.Add(copyCraft); db.SaveChanges();
			samples.Add(new { key, prototype = copyItem.Id, component = copyComponent.Id, keyword = copyItem.Keywords, craft = copyCraft.Name,
				seconds = originalCraft.CraftPhases.Sum(x => x.PhaseLengthInSeconds), original = original.Id, original_component = component.Id });
		}
		var school = db.MagicSchools.Single(x => x.Id == read.MagicSchoolId);
		var source = Path.Combine(CatalogueRepository, "tests", "ArmageddonPreparedSeederNativeHarness", "Program.FocusDeviceFixture.cs");
		input["focus_devices"] = JsonSerializer.SerializeToNode(new { samples, keys = ArmageddonStockFocusDeviceInstaller.Keys,
			blank = claims["arm.item.focus_blank"], seal = claims["arm.item.binding_seal"], craft_trait = claims[ArmageddonStockProgression.CraftTraitKey],
			use_gate = claims[ArmageddonStockFocusDeviceInstaller.UseGateKey], craft_gate = claims[ArmageddonStockFocusDeviceInstaller.CraftGateKey],
			read_spell = read.Id, read_capability = input["capability"]!.GetValue<long>(), read_merit = input["merit"]!.GetValue<long>(),
			mend_spell = mend.Id, mend_capability = route.Capability, mend_merit = route.Merit, resource = reserve, invoked = school.SchoolVerb.ToLowerInvariant(),
			source = new { path = source, sha256 = BaseHash(source) } });
		File.WriteAllText(path, input.ToJsonString());
		SaveFocusDeviceFixture(database, path);
		Console.WriteLine("ARMS4-focus-device-input=prepared twelve owned prototypes and recipes three availability-only disposable copies package gates unchanged");
	}
	private sealed record FocusDeviceFixture(string Kind, string Database, string SqlFile, string SqlSha256,
		string InputFile, string InputSha256, string BaseManifestSha256, string PackageSourceSha256,
		IReadOnlyDictionary<string, string> RuntimeAssemblies);
	private static string FocusDeviceFixtureRoot => Path.Combine(CatalogueRepository, ".artifacts", "s4-focus-device-fixtures");

	private static void SaveFocusDeviceFixture(TestDatabase database, string suppliedInput)
	{
		var folder = Path.Combine(FocusDeviceFixtureRoot, DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ") + "_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(folder); RequireBasePath(folder, FocusDeviceFixtureRoot, directory: true);
		var sql = new MySqlDatabaseBackupService().CreateBackup(database.ConnectionString, folder);
		RequireBaseDumpTargets(File.ReadAllText(sql), database.Name);
		var input = Path.Combine(folder, "focus-device-input.json"); File.Copy(suppliedInput, input);
		var assemblies = new[] { "MudSharp.dll", "FutureMUDLibrary.dll", "MudsharpDatabaseLibrary.dll", "ExpressionEngine.dll" }
			.ToDictionary(x => x, x => BaseHash(Path.Combine(CatalogueRepository, "MudSharpCore", "bin", "Debug", "net10.0", x)));
		var descriptor = new FocusDeviceFixture("FutureMUD-S4-focus-device-prepared-v1", database.Name,
			Path.GetFileName(sql), BaseHash(sql), Path.GetFileName(input), BaseHash(input),
			Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE_SHA256")!, CatalogueContentSourceHash(), assemblies);
		var manifest = Path.Combine(folder, "focus-device.json");
		File.WriteAllText(manifest, JsonSerializer.Serialize(descriptor, BaseJsonOptions));
		Console.WriteLine("ARMS4-focus-device-fixture=" + manifest + " sha256:" + BaseHash(manifest));
	}

}
