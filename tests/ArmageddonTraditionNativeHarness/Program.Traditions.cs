#nullable enable
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Logging;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Gathering;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static class TraditionEntryPoint
{
	public static int Main(string[] args)
	{
		try { return GNHProgram.TraditionMain(args); }
		catch (Exception error) { Console.Error.WriteLine(error); return 1; }
	}
}

internal static partial class GNHProgram
{
	private sealed record TraditionReader(string Database, ArmageddonTraditionInstallPlan Plan, Dictionary<string, long> Identities);
	internal static int TraditionMain(string[] args)
	{
		OwnedConnections.Install();
		return args.FirstOrDefault() switch
		{
			"--traditions-run" => TraditionNative(), "--traditions-reader" => TraditionRestart(args[1]),
			"--traditions-provisions-run" => TraditionNative(true), "--traditions-provisions-reader" => ProvisionInstallerRestart(args[1]),
			"--traditions-custody-run" => TraditionNative(true, true),
			"--traditions-custody-reader" => ProvisionCustodyRestart(args[1]),
			"--traditions-pierce-run" => TraditionNative(true, false, true),
			"--traditions-pierce-reader" => InstalledPierceRestart(args[1]),
			"--traditions-direct-run" => RunCastingAcceptanceChecks(),
			"--traditions-progression-run" => RunCompletionProgressionAcceptanceChecks(),
			"--traditions-support-run" => RunSupportProgressionAcceptanceChecks(),
			"--traditions-capacity-run" => RunCapacityAcceptanceChecks(),
			_ => Main(args)
		};
	}

	private static ArmageddonTraditionInstallResult InstallTraditions(TestDatabase database, ArmageddonTraditionInstallPlan plan,
		Action<ArmageddonInstallCheckpoint>? fault = null)
	{ using var db = NewIndependentContext(database.ConnectionString); return ArmageddonTraditionInstaller.Install(db, plan, fault); }
	private static void RequireTraditions(ArmageddonTraditionInstallResult result) =>
		Require(result.Status == ArmageddonInstallStatus.Completed, string.Join("\n", result.Messages));
	private static string TraditionPlayers(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		return PlayerSnapshot(database) + JsonSerializer.Serialize(db.PerceiverMerits.AsNoTracking().OrderBy(x => x.MeritId).ToArray());
	}
	private static int TraditionNative(bool provisions = false, bool reproduceCustody = false, bool pierce = false)
	{
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		var fixture = FixtureSeed.Create(database, "arm_traditions_lane", true); var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true); ConfigureCastingWorld(seed, database.ConnectionString, true);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id); SeedCreatedWeaponPrototypes(database, seed.World.Materials.First().Id);
		SeedConsumables(database, fixture, seed.World.Materials.First().Id);
		ArmageddonMagicInstallPlan utilityPlan;
		long decoratorId, gatherId;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			db.FutureProgs.Add(new() { FunctionName = "traditionAlwaysTrue", FunctionText = "return true", FunctionComment = "Ordinary no-argument fixture true",
				Category = "Harness", Subcategory = "Traditions", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString() });
			var decorator = new Db.TraitDecorator { Name = "ARMTRAD native numbers", Type = "SimpleNumeric", Contents = "" };
			var improver = new Db.Improver { Name = "ARMTRAD native gather use", Type = "classic", Definition = "<Definition Chance='1' Expression='0.5' ImproveOnFail='true' ImproveOnSuccess='true' DifficultyThresholdInterval='0' NoGainSecondsDiceExpression='0'/>" };
			db.AddRange(decorator, improver); db.SaveChanges(); decoratorId = decorator.Id;
			var gather = new Db.TraitDefinition { Name = "ARMTRAD native gather", Type = 0, OwnerScope = 1, TraitGroup = "ARM02",
				DecoratorId = decorator.Id, ImproverId = improver.Id, ExpressionId = fixture.TraitExpressionId,
				Alias = "", ChargenBlurb = "Native gathering counterpart", ValueExpression = "" };
			db.TraitDefinitions.Add(gather); db.SaveChanges(); gatherId = gather.Id;
			var skills = new Dictionary<string, long>();
			foreach (var content in ArmageddonMagicInstaller.Content(new(false, 0, 0, skills, 0, 0, 0, 0, 0, 0, 0, 0, 0)))
			{
				var skill = new Db.TraitDefinition { Name = "ARMTRAD external " + content.Name, Type = 0, OwnerScope = 1, TraitGroup = "ARM02",
					Alias = "", ChargenBlurb = "Explicit utility factory binding", ValueExpression = "100", ExpressionId = fixture.TraitExpressionId };
				db.TraitDefinitions.Add(skill); db.SaveChanges(); skills.Add(content.Key, skill.Id);
			}
			var eligibility = new Db.FutureProg { FunctionName = "traditionMendPolicy", FunctionText = "return true", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(),
				Category = "Harness", Subcategory = "Traditions", FunctionComment = "Owned fixture policy" };
			foreach (var name in new[] { "target", "caster" }) eligibility.FutureProgsParameters.Add(new() { ParameterIndex = eligibility.FutureProgsParameters.Count, ParameterName = name, ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			db.FutureProgs.Add(eligibility); db.SaveChanges();
			var authority = new Db.AuthorityGroup { Name = "ARMTRAD fixture authority" }; db.AuthorityGroups.Add(authority); db.SaveChanges();
			var builder = new Db.Account { Name = "ARMTRAD disposable builder", CreationDate = DateTime.UtcNow, AuthorityGroupId = authority.Id };
			foreach (var property in typeof(Db.Account).GetProperties().Where(x => x.PropertyType == typeof(string))) if (property.GetValue(builder) is null) property.SetValue(builder, "");
			db.Accounts.Add(builder); db.SaveChanges(); var light = db.GameItemProtos.Single(x => x.Name == "ARM03B2B C2 light"); var hold = db.GameItemComponentProtos.Single(x => x.Name == "ARM03B2B Holdable");
			utilityPlan = new(true, seed.Capability.School.Id, seed.Resource.Id, skills, db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id,
				eligibility.Id, db.Liquids.Single(x => x.Name == "ARM03C2 water").Id, light.Id, light.RevisionNumber, hold.Id, hold.RevisionNumber, seed.World.Materials.First().Id, builder.Id);
		}
		var utilities = InstallOwned(database, utilityPlan); RequireInstalled(utilities);
		ArmageddonTraditionInstallPlan plan;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			plan = new(true, utilityPlan.School, utilityPlan.Resource, utilityPlan.Resource, decoratorId, utilityPlan.AlwaysFalseProg,
				db.FutureProgs.Single(x => x.FunctionName == "traditionAlwaysTrue").Id, fixture.CapabilityId,
				utilities.Identities.Where(x => utilityPlan.SpellSkills.ContainsKey(x.Key)).ToDictionary(x => x.Key, x => x.Value),
				new Dictionary<string, long> { ["arm.support.gather"] = gatherId },
				ArmageddonTraditionInstaller.Variants.ToDictionary(x => x, _ => (IReadOnlyList<MagicGatheringMethodKind>)new[] { MagicGatheringMethodKind.Self }));
		}
		var players = TraditionPlayers(database);
		Require(InstallTraditions(database, plan with { Install = false }).Status == ArmageddonInstallStatus.Declined && players == TraditionPlayers(database), "Decline changed players.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var collision = new Db.TraitExpression { Name = "Armageddon heal cap", Expression = "55" }; db.TraitExpressions.Add(collision); db.SaveChanges();
			Require(InstallTraditions(database, plan).Status == ArmageddonInstallStatus.Blocked && !db.SeederManagedRecords.Any(x => x.Module == ArmageddonTraditionInstaller.Module), "Collision was adopted or partially installed.");
			db.Remove(collision); db.SaveChanges();
		}
		foreach (var boundary in new[] { ArmageddonInstallCheckpoint.ContentCreated, ArmageddonInstallCheckpoint.BeforeCommit })
		{
			var failed = InstallTraditions(database, plan, x => { if (x == boundary) throw new IOException("Owned tradition interruption " + boundary); });
			using var db = NewIndependentContext(database.ConnectionString);
			Require(failed.Status == ArmageddonInstallStatus.Failed && !db.SeederManagedRecords.Any(x => x.Module == ArmageddonTraditionInstaller.Module) &&
				!db.TraitDefinitions.Any(x => x.TraitGroup == "Armageddon Spell") && players == TraditionPlayers(database), "Real rollback left a partial graph or touched players.");
			Console.WriteLine("ARMTRAD-rollback=passed boundary:" + boundary);
		}
		var uncertain = InstallTraditions(database, plan, x => { if (x == ArmageddonInstallCheckpoint.AfterCommit) throw new IOException("Lost acknowledgement"); });
		Require(uncertain.Status == ArmageddonInstallStatus.CommittedConfirmationFailed && uncertain.Identities.Count == 171 && uncertain.AvailableSpells.Count == 3 && uncertain.UnavailableSpells.Count == 79, "Partial receipt misclassified.");
		var ids = uncertain.Identities.ToDictionary(x => x.Key, x => x.Value); RunTraditionReader(new(database.Name, plan, ids));
		Require(players == TraditionPlayers(database), "Install/restart touched players.");
		Console.WriteLine("ARMTRAD-source=passed 94-source-rows 82-distinct-native-skills three-prerequisite-closed-admissions 79-unavailable no-fabricated-roots-or-supports no-player-mutation");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var owned = db.MagicCapabilities.Find(ids["arm.capability.defiler"])!; var original = owned.Definition;
			owned.Name = "ARMTRAD builder Defiler"; var xml = XElement.Parse(owned.Definition); xml.Element("ConcentrationCapabilityExpression")!.Value = "4"; owned.Definition = xml.ToString();
			var clone = new Db.MagicCapability { Name = "ARMTRAD unowned clone", CapabilityModel = "skilllevel", MagicSchoolId = owned.MagicSchoolId, Definition = owned.Definition };
			db.MagicCapabilities.Add(clone); db.SaveChanges(); var edited = owned.Definition;
			for (var i = 0; i < 2; i++) RequireTraditions(InstallTraditions(database, plan));
			Require(db.MagicCapabilities.AsNoTracking().Single(x => x.Id == owned.Id).Definition == edited && !db.SeederManagedRecords.Any(x => x.EntityType == nameof(Db.MagicCapability) && x.LogicalId == clone.Id), "Builder edit/clone was taken over.");
			var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.merit.preserver"); record.Retired = true; db.SaveChanges();
			Require(InstallTraditions(database, plan).Status == ArmageddonInstallStatus.Blocked, "Retirement was ignored."); record.Retired = false; db.SaveChanges();
			var merit = db.Merits.Find(record.LogicalId)!; db.Remove(merit); db.SaveChanges();
			Require(InstallTraditions(database, plan).Status == ArmageddonInstallStatus.Blocked && !db.Merits.AsNoTracking().Any(x => x.Id == merit.Id), "Missing identity resurrected.");
			db.Entry(merit).State = EntityState.Added; db.SaveChanges();
			var claim = new Db.SeederManagedRecord { Seeder = "ARMTRAD foreign", Module = "foreign", StableKey = "foreign.cap", EntityType = record.EntityType, LogicalId = record.LogicalId, ManifestVersion = "foreign", AppliedFingerprint = "unchanged", AppliedAt = DateTime.UnixEpoch };
			db.SeederManagedRecords.Add(claim); db.SaveChanges(); Require(InstallTraditions(database, plan).Status == ArmageddonInstallStatus.Blocked, "Competing identity was adopted."); db.Remove(claim); db.SaveChanges();
			// The earlier whole-XML override intentionally keeps the builder's three admissions.
			// Restore it only in this disposable expansion scenario to exercise the default six-entry graph.
			if (provisions) { owned.Definition = original; db.SaveChanges(); }
			// Builder chooses deterministic native improvement only inside this owned disposable fixture.
			var improver = db.Improvers.Find(ids["arm.improver.spell_practice"])!; var improvement = XElement.Parse(improver.Definition);
			improvement.SetAttributeValue("Chance", 1); improvement.SetAttributeValue("NoGainSecondsDiceExpression", "0"); improver.Definition = improvement.ToString(); db.SaveChanges();
		}
		Require(players == TraditionPlayers(database), "Reconciliation touched players.");
		Console.WriteLine("ARMTRAD-ownership=passed stable-IDs builder-edit clone deletion retirement competing-claim full-transaction no-player-refresh");
		var expanded = provisions ? InstallProvisionExtension(database, plan, ids) : null;
		var perception = pierce ? InstallPierceExtension(database, expanded!, ids) : null;
		if (perception is not null) expanded = expanded! with { Traditions = perception.Traditions };
		if (expanded is not null) plan = expanded.Traditions;
		var host = PrepareRetirementHost(database, fixture, clock, wielding: true, consumablesAnatomy: true); var native = host.Native;
		Require(native.World.Traits.Any(x => x.Group == "ARM02") && !native.World.Traits.Any(x => x.Group == "Armageddon Spell"), "Default shared fixture trait selection changed.");
		LoadTraditionNative(native, database, utilityPlan, utilities.Identities, ids, expanded is null ? 3 : perception is null ? 6 : 7, expanded?.Identities, perception?.Identities);
		VerifyInstalledTraditionProgression(native, database, plan, ids, clock);
		if (perception is not null) VerifyInstalledPierce(host, database, fixture, clock, utilityPlan, utilities.Identities, ids, expanded!, perception);
		else if (expanded is not null) VerifyProvisionExtension(host, database, fixture, clock, utilityPlan, utilities.Identities, ids, expanded, reproduceCustody);
		return 0;
	}

	private static void RunTraditionReader(TraditionReader input)
	{
		var info = new ProcessStartInfo("dotnet") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
		info.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); info.ArgumentList.Add("--traditions-reader"); info.ArgumentList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input))));
		using var process = Process.Start(info)!; var output = process.StandardOutput.ReadToEndAsync(); var errors = process.StandardError.ReadToEndAsync();
		if (!process.WaitForExit(60000)) { process.Kill(true); throw new TimeoutException("Owned tradition reader timeout."); }
		Require(process.ExitCode == 0, output.GetAwaiter().GetResult() + errors.GetAwaiter().GetResult()); Console.Write(output.GetAwaiter().GetResult());
	}
	private static int TraditionRestart(string encoded)
	{
		var input = JsonSerializer.Deserialize<TraditionReader>(Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var players = TraditionPlayers(database); var result = InstallTraditions(database, input.Plan); RequireTraditions(result);
		Require(result.Identities.Count == 171 && result.Identities.All(x => input.Identities[x.Key] == x.Value) && players == TraditionPlayers(database), "Fresh-process rerun changed identities/player state.");
		Console.WriteLine("ARMTRAD-restart=passed acknowledged-lost-confirmation same-171-identities no-player-mutation"); return 0;
	}
}
