#nullable enable
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.FutureProg;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static class PreparedSeederEntryPoint
{
	public static int Main(string[] args)
	{
		try { return GNHProgram.PreparedSeederMain(args); }
		catch (Exception error) { Console.Error.WriteLine(error); return 1; }
	}
}

internal static partial class GNHProgram
{
	private sealed record PreparedReader(string Database, string Bindings, Dictionary<string, long?> Identities,
		string? Policy = null, string? WithoutPierceVariant = null, long? Pierce = null, bool ReadOnly = false, string? ProvisionsPolicy = null);
	internal static int PreparedSeederMain(string[] args)
	{
		if (args.FirstOrDefault() == "--installed-components-check") return PreparedInstalledComponentPreflight();
		OwnedConnections.Install();
		return args.FirstOrDefault() switch
		{
			"--prepared-run" => PreparedSeederNative(),
			"--prepared-reader" => PreparedSeederReader(args[1]),
			"--replay-run" => PreparedSeederReplayNative(),
			"--installed-run" => PreparedSeederReplayNative(true),
			"--sense-run" => TraditionNative(true, true),
			"--sense-control-run" => PreparedSenseControlNative(),
			"--traditions-reader" => TraditionRestart(args[1]),
			"--traditions-custody-reader" => ProvisionCustodyRestart(args[1]),
			_ => throw new InvalidOperationException("Select the dedicated prepared/replay/Sense lane entrypoint.")
		};
	}
	private static ArmageddonPreparedWorldInstallResult RunPrepared(TestDatabase database, ArmageddonPreparedWorldBindings bindings,
		Action<string, ArmageddonInstallCheckpoint>? fault = null) => ArmageddonPreparedWorldInstaller.Install(
			() => NewIndependentContext(database.ConnectionString), bindings, fault);
	private static void RequirePrepared(ArmageddonPreparedWorldInstallResult result) => Require(result.Status == ArmageddonInstallStatus.Completed, result.Describe());
	private static Dictionary<string, long?> PreparedIdentities(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		return db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package).ToDictionary(x => x.StableKey, x => x.LogicalId);
	}
	private static int PreparedSeederNative()
	{
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		var fixture = FixtureSeed.Create(database, "arm_prepared_seeder_lane", true); var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true); ConfigureCastingWorld(seed, database.ConnectionString, true);
		Require(seed.Actor.AddTrait(seed.World.Traits.GetByName("ARM02 Agility"), 19) || seed.Actor.HasTrait(seed.World.Traits.GetByName("ARM02 Agility")), "Prepared fixture body attribute"); FlushCasting(seed);
		seed.Actor.SetTraitValue(seed.World.Traits.GetByName("ARM02 Agility"), 19); FlushCasting(seed);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id); SeedCreatedWeaponPrototypes(database, seed.World.Materials.First().Id);
		SeedConsumables(database, fixture, seed.World.Materials.First().Id);
		ArmageddonPreparedWorldBindings bindings;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var yes = new Db.FutureProg { FunctionName = "traditionAlwaysTrue", FunctionText = "return true", FunctionComment = "Ordinary disposable no-argument true",
				Category = "Harness", Subcategory = "Prepared Seeder", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString() };
			var decorator = new Db.TraitDecorator { Name = "ARMTRAD prepared numbers", Type = "SimpleNumeric", Contents = "" };
			var improver = new Db.Improver { Name = "ARMTRAD prepared gather use", Type = "classic", Definition = "<Definition Chance='1' Expression='0.5' ImproveOnFail='true' ImproveOnSuccess='true' DifficultyThresholdInterval='0' NoGainSecondsDiceExpression='0'/>" };
			db.AddRange(yes, decorator, improver); db.SaveChanges();
			var gather = new Db.TraitDefinition { Name = "ARMTRAD prepared gather", Type = 0, OwnerScope = 1, TraitGroup = "ARM02", DecoratorId = decorator.Id,
				ImproverId = improver.Id, ExpressionId = fixture.TraitExpressionId, Alias = "", ChargenBlurb = "Native explicit gathering counterpart", ValueExpression = "" };
			db.Add(gather); db.SaveChanges();
			var skills = new Dictionary<string, long>();
			foreach (var content in ArmageddonMagicInstaller.Content(new(false, 0, 0, skills, 0, 0, 0, 0, 0, 0, 0, 0, 0)))
			{
				var skill = new Db.TraitDefinition { Name = "ARMTRAD prepared external " + content.Name, Type = 0, OwnerScope = 1, TraitGroup = "ARM02",
					Alias = "", ChargenBlurb = "Explicit utility binding", ValueExpression = "100", ExpressionId = fixture.TraitExpressionId };
				db.Add(skill); db.SaveChanges(); skills.Add(content.Key, skill.Id);
			}
			var eligibility = new Db.FutureProg { FunctionName = "preparedMendPolicy", FunctionText = "return true", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(),
				Category = "Harness", Subcategory = "Prepared Seeder", FunctionComment = "Explicit disposable policy" };
			foreach (var name in new[] { "target", "caster" }) eligibility.FutureProgsParameters.Add(new() { ParameterIndex = eligibility.FutureProgsParameters.Count, ParameterName = name, ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			db.Add(eligibility); var authority = new Db.AuthorityGroup { Name = "ARMTRAD prepared authority" }; db.Add(authority); db.SaveChanges();
			var builder = new Db.Account { Name = "ARMTRAD prepared disposable builder", CreationDate = DateTime.UtcNow, AuthorityGroupId = authority.Id };
			foreach (var property in typeof(Db.Account).GetProperties().Where(x => x.PropertyType == typeof(string))) if (property.GetValue(builder) is null) property.SetValue(builder, "");
			db.Add(builder); db.SaveChanges();
			var light = db.GameItemProtos.Single(x => x.Name == "ARM03B2B C2 light"); var hold = db.GameItemComponentProtos.Single(x => x.Name == "ARM03B2B Holdable");
			var utility = new ArmageddonMagicInstallPlan(true, seed.Capability.School.Id, seed.Resource.Id, skills, db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id,
				eligibility.Id, db.Liquids.Single(x => x.Name == "ARM03C2 water").Id, light.Id, light.RevisionNumber, hold.Id, hold.RevisionNumber, seed.World.Materials.First().Id, builder.Id);
			var attribute = db.TraitDefinitions.Single(x => x.Name == "ARM02 Agility");
			var expression = new Db.TraitExpression { Name = "ARMTRAD prepared authored capacity", Expression = "variable*10" }; db.Add(expression); db.SaveChanges();
			var resource = db.MagicResources.Single(x => x.Id == seed.Resource.Id); var definition = XElement.Parse(resource.Definition);
			definition.Element("AttributeCapacity")?.Remove(); definition.Add(new XElement("AttributeCapacity", new XAttribute("version", 1),
				new XAttribute("attribute", attribute.Id), new XAttribute("expression", expression.Id), new XAttribute("basis", "raw"))); resource.Definition = definition.ToString(); db.SaveChanges();
			bindings = new(utility, utility.Resource, decorator.Id, yes.Id, fixture.CapabilityId, new Dictionary<string, long> { ["arm.support.gather"] = gather.Id },
				attribute.Id, expression.Id, "raw", ArmageddonTraditionInstaller.Variants.ToDictionary(x => x, _ => (IReadOnlyList<MagicGatheringMethodKind>)new[] { MagicGatheringMethodKind.Self }));
		}
		var players = TraditionPlayers(database); var seeder = new ArmageddonMagicSeeder();
		var beforeMenu = PreparedDatabaseChecksum(database);
		var declineMenu = PreparedMenu(database, false, null);
		Require(declineMenu.Contains("Suggested default: no") && declineMenu.Contains("declined") && beforeMenu == PreparedDatabaseChecksum(database), "Actual menu default decline changed content.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(seeder.Questions.First().DefaultAnswerResolver!(db, new Dictionary<string, string>()) == "no", "Native default opt-in changed.");
			Require(seeder.SeedData(db, new Dictionary<string, string> { [ArmageddonMagicSeeder.InstallQuestion] = "no" }).Contains("declined") && PreparedIdentities(database).Count == 0, "Native decline wrote content.");
			Require(ArmageddonMagicSeeder.ValidateBindings(ArmageddonMagicSeeder.SerializeBindings(bindings), db).Success, "Native prepared bindings rejected.");
		}
		foreach (var boundary in new[] { ArmageddonInstallCheckpoint.ContentCreated, ArmageddonInstallCheckpoint.BeforeCommit })
		{
			var failed = RunPrepared(database, bindings, (module, point) => { if (module == ArmageddonMagicInstaller.Module && point == boundary) throw new IOException("Native prepared interruption"); });
			Require(failed.Status == ArmageddonInstallStatus.Failed && failed.Modules.Count == 1 && PreparedIdentities(database).Count == 0 && players == TraditionPlayers(database), "Utility transaction did not roll back or report stop.");
		}
		var uncertain = RunPrepared(database, bindings, (module, point) => { if (module == ArmageddonMagicInstaller.Module && point == ArmageddonInstallCheckpoint.AfterCommit) throw new IOException("Native prepared lost acknowledgement"); });
		Require(uncertain.Status == ArmageddonInstallStatus.CommittedConfirmationFailed && uncertain.Modules.Count == 1 && PreparedIdentities(database).Count == 21, "Committed module lost acknowledgement misclassified.");
		Console.WriteLine("ARMPREP-boundaries=passed utility-real-rollback lost-ack-stop no-cross-module-rollback-claim");
		var stopped = RunPrepared(database, bindings, (module, point) => { if (module == ArmageddonTraditionInstaller.Module && point == ArmageddonInstallCheckpoint.BeforeCommit) throw new IOException("Native later module interruption"); });
		Require(stopped.Status == ArmageddonInstallStatus.Failed && stopped.Modules.Count == 2 && stopped.Modules[0].Status == ArmageddonInstallStatus.Completed && PreparedIdentities(database).Count == 21, "Later rollback discarded committed utilities or left traditions.");
		var json = ArmageddonMagicSeeder.SerializeBindings(bindings);
		QualifyFreshPreparedBootstrap(database, bindings, json);
		var positiveMenu = PreparedMenu(database, true, json);
		Require(positiveMenu.Contains("4/82") && positiveMenu.Contains("unattainable") && positiveMenu.Contains("Completed"), "Actual opt-in menu misreported closure or failed.");
		var complete = RunPrepared(database, bindings); RequirePrepared(complete); Require(PreparedIdentities(database).Count == 196 && complete.Availability.All(x => x.StoredAdmissions.Count == 4), "Native four-admission closure wrong.");
		Console.WriteLine("ARMPREP-entrypoint=passed real-question-contract real-SeedData 196-owned-records four-stored-admissions 78-unavailable no-new-provisions no-player-mutation");
		var retained = PreparedIdentities(database); RunPreparedReader(new(database.Name, json, retained)); Require(players == TraditionPlayers(database), "Entry/rerun/restart mutated players.");
		QualifyPreparedComposition(database, bindings, json, retained);
		QualifyPreparedNewProvisions(database, bindings, retained);
		string provisionBefore;
		using (var db = NewIndependentContext(database.ConnectionString)) provisionBefore = PreparedProvisionSnapshot(db);
		var expanded = RunPrepared(database, bindings); RequirePrepared(expanded);
		using (var db = NewIndependentContext(database.ConnectionString)) Require(provisionBefore == PreparedProvisionSnapshot(db), "Unselected provision module changed existing definitions/ownership.");
		Require(PreparedIdentities(database).Count == 203 && expanded.Availability.All(x => x.StoredAdmissions.Count == 7) && players == TraditionPlayers(database), "Existing provision preservation lost real closure or changed players.");
		RunPreparedReader(new(database.Name, json, PreparedIdentities(database)));
		Console.WriteLine("ARMPREP-preservation=passed actual-builder-removals existing-seven-provision-records-byte-preserved seven-stored-admissions explicit-selection-only");
		var utilityIds = expanded.Modules.First().Identities; var traditionIds = expanded.Modules.Last().Identities;
		Dictionary<string, long> provisionIds;
		using (var db = NewIndependentContext(database.ConnectionString)) provisionIds = db.SeederManagedRecords.AsNoTracking().Where(x => x.Module == ArmageddonProvisionInstaller.Module).ToDictionary(x => x.StableKey, x => x.LogicalId!.Value);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var improver = db.Improvers.Find(traditionIds["arm.improver.spell_practice"])!; var definition = XElement.Parse(improver.Definition);
			definition.SetAttributeValue("Chance", 1); definition.SetAttributeValue("NoGainSecondsDiceExpression", "0"); improver.Definition = definition.ToString(); db.SaveChanges();
		}
		var host = PrepareRetirementHost(database, fixture, clock, wielding: true, consumablesAnatomy: true); var native = host.Native;
		LoadTraditionNative(native, database, bindings.Utilities, utilityIds, traditionIds, 7, provisionIds, expanded.Modules.Single(x => x.Module == ArmageddonPierceInstaller.Module).Identities);
		PreparedRuntimeCapacity(native, database, bindings);
		var implemented = bindings.Utilities.SpellSkills.Keys.ToDictionary(x => x, x => utilityIds[x]);
		implemented[ArmageddonReviewedPierceContent.Key] = expanded.Modules.Single(x => x.Module == ArmageddonPierceInstaller.Module).Identities[ArmageddonReviewedPierceContent.Key];
		foreach (var key in new[] { ArmageddonReviewedProvisionContent.SustainMealKey, ArmageddonReviewedProvisionContent.DrawWineKey }) implemented[key] = provisionIds[key];
		VerifyInstalledTraditionProgression(native, database, new(true, bindings.Utilities.School, bindings.Utilities.Resource, bindings.ReserveResource,
			bindings.Decorator, bindings.Utilities.AlwaysFalseProg, bindings.AlwaysTrueProg, bindings.GatheringTemplate, implemented, bindings.SupportSkills, bindings.AllowedMethods), traditionIds, clock);
		Console.WriteLine("ARMPREP-native-validation=passed real-loadable-spells casting-and-gathering-configurations three-variants no-passive-regeneration");
		return 0;
	}
	private static string PreparedProvisionSnapshot(MudSharp.Database.FuturemudDatabaseContext db) => JsonSerializer.Serialize(new
	{
		Records = db.SeederManagedRecords.AsNoTracking().Where(x => x.Module == ArmageddonProvisionInstaller.Module).OrderBy(x => x.StableKey).ToArray(),
		Spells = db.MagicSpells.AsNoTracking().Where(x => x.Id == db.SeederManagedRecords.Where(y => y.StableKey == ArmageddonReviewedProvisionContent.SustainMealKey).Select(y => y.LogicalId).First() ||
			x.Id == db.SeederManagedRecords.Where(y => y.StableKey == ArmageddonReviewedProvisionContent.DrawWineKey).Select(y => y.LogicalId).First()).OrderBy(x => x.Id).ToArray()
	});
	private static ArmageddonProvisionInstallPlan PreparedProvisionSelections(TestDatabase database, ArmageddonPreparedWorldBindings bindings, Dictionary<string, long?> ids)
	{
		ArmageddonProvisionInstallPlan plan;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var original = db.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos).Single(x => x.Name == "ARM03B2B C2 meal");
			var foods = new List<ArmageddonFoodPrototype> { new(original.Id, original.RevisionNumber) }; var next = db.GameItemProtos.Max(x => x.Id);
			for (var i = 2; i <= 3; i++)
			{
				var item = (Db.GameItemProto)db.Entry(original).CurrentValues.ToObject(); item.Id = ++next; item.Name = "ARM03B2B prepared legacy meal " + i;
				item.EditableItem = new() { BuilderDate = DateTime.UtcNow, RevisionStatus = (int)MudSharp.Framework.Revision.RevisionStatus.Current };
				foreach (var part in original.GameItemProtosGameItemComponentProtos) item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = part.GameItemComponentProtoId, GameItemComponentRevision = part.GameItemComponentRevision });
				db.Add(item); foods.Add(new(item.Id, item.RevisionNumber));
			}
			db.SaveChanges(); var wine = db.Liquids.Single(x => x.Name == "ARM03C2 wine");
			plan = new(true, bindings.Utilities.School, bindings.Utilities.Resource, bindings.Utilities.AlwaysFalseProg,
				ids[ArmageddonReviewedProvisionContent.SustainMealKey + ".skill"]!.Value, ids[ArmageddonReviewedProvisionContent.DrawWineKey + ".skill"]!.Value, [new(32, 0, foods)], wine.Id, [new(32, 0, wine.Id)]);
		}
		return plan; // Existing approved selections only; installation uses the actual prepared entrypoint.
	}
	private static void RunPreparedReader(PreparedReader input)
	{
		// Policy baselines exceed Windows' command-line limit. Keep the bounded receipt in
		// this run's uniquely named local file until its owned child has stopped.
		var receipt = Path.Combine(Path.GetTempPath(), "futuremud-prepared-reader_" + Guid.NewGuid().ToString("N") + ".json");
		File.WriteAllText(receipt, JsonSerializer.Serialize(input), new UTF8Encoding(false));
		var info = new ProcessStartInfo("dotnet") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
		info.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); info.ArgumentList.Add("--prepared-reader"); info.ArgumentList.Add(receipt);
		Process? process = null;
		try
		{
			process = Process.Start(info)!; var output = process.StandardOutput.ReadToEndAsync(); var errors = process.StandardError.ReadToEndAsync();
			if (!process.WaitForExit(60000))
			{
				process.Kill(true); Require(process.WaitForExit(30000), "Owned prepared reader did not stop after timeout.");
				throw new TimeoutException("Owned prepared reader timeout.");
			}
			Require(process.ExitCode == 0, output.GetAwaiter().GetResult() + errors.GetAwaiter().GetResult()); Console.Write(output.GetAwaiter().GetResult());
		}
		finally
		{
			if (process is { HasExited: false })
			{
				process.Kill(true); Require(process.WaitForExit(30000), "Owned prepared reader cleanup did not stop its child.");
			}
			process?.Dispose(); File.Delete(receipt);
		}
	}
	private static int PreparedSeederReader(string receipt)
	{
		var path = Path.GetFullPath(receipt); var file = new FileInfo(path);
		Require(string.Equals(file.DirectoryName, Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase) &&
			file.Name.StartsWith("futuremud-prepared-reader_", StringComparison.Ordinal) && file.Extension == ".json" && file.Length <= 2 * 1024 * 1024,
			"Prepared reader requires a bounded uniquely named local receipt.");
		var input = JsonSerializer.Deserialize<PreparedReader>(File.ReadAllText(path, Encoding.UTF8))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString); var players = TraditionPlayers(database);
		VerifyPreparedReaderPolicy(database, input);
		if (input.ReadOnly)
		{
			Require(input.Identities.OrderBy(x => x.Key).SequenceEqual(PreparedIdentities(database).OrderBy(x => x.Key)), "Fresh readonly reader changed identities.");
			Console.WriteLine("ARMPREP-composition-reader=passed fresh-process exact-policy-baselines identities readonly-before-resume"); return 0;
		}
		var result = RunPrepared(database, ArmageddonMagicSeeder.ParseBindings(input.Bindings)); RequirePrepared(result);
		VerifyPreparedReaderPolicy(database, input);
		Require(input.Identities.OrderBy(x => x.Key).SequenceEqual(PreparedIdentities(database).OrderBy(x => x.Key)) && players == TraditionPlayers(database), "Fresh-process prepared rerun changed identities/players.");
		Console.WriteLine("ARMPREP-reader=passed fresh-process same-owned-identities no-player-mutation"); return 0;
	}
}
