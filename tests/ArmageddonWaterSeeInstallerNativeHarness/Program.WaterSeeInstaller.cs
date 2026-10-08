#nullable enable
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Resources;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static class WaterSeeInstallerEntryPoint
{
	public static int Main(string[] args)
	{
		try
		{
			GNHProgram.InstallWaterSeeOwnedConnections();
			return args.FirstOrDefault() switch
			{
				"--water-see-run" => GNHProgram.WaterSeeInstallerNative(),
				"--water-see-reader" => GNHProgram.WaterSeeInstallerReader(args[1]),
				"--water-breathing-stock-run" or "--water-breathing-stock-reader" => GNHProgram.WaterBreathingStockMain(args),
				"--see-unbodied-stock-run" or "--see-unbodied-stock-reader" => GNHProgram.SeeTheUnbodiedStockMain(args),
				_ => throw new InvalidOperationException("Select this lane's installer or stock regression mode.")
			};
		}
		catch (Exception error) { Console.Error.WriteLine(error); return 1; }
	}
}

internal static partial class GNHProgram
{
	internal static void InstallWaterSeeOwnedConnections() => OwnedConnections.Install();
	private sealed record WaterSeeReader(string Database, FixtureIds Fixture, string Bindings, Dictionary<string, long?> Identities,
		string Policy, string Players, DateTime Now, long Component, int Quantity, Guid WaterParent, Guid SeeParent,
		DateTime WaterExpiry, DateTime SeeExpiry, double Balance);

	internal static int WaterSeeInstallerNative()
	{
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		var fixture = FixtureSeed.Create(database, "water_see_installer_lane", true); var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true); ConfigureCastingWorld(seed, database.ConnectionString, true);
		Require(seed.Actor.AddTrait(seed.World.Traits.GetByName("ARM02 Agility"), 19) || seed.Actor.HasTrait(seed.World.Traits.GetByName("ARM02 Agility")), "Explicit fixture capacity attribute");
		seed.Actor.SetTraitValue(seed.World.Traits.GetByName("ARM02 Agility"), 19); FlushCasting(seed);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id); SeedCreatedWeaponPrototypes(database, seed.World.Materials.First().Id);
		SeedConsumables(database, fixture, seed.World.Materials.First().Id); SeedSeeStockComponents(database);
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

		// Preparation selects real fixture rows. It does not construct Water/See spells.
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			foreach (var (id, name) in new[] { (101L, "Explicit fixture Silt"), (102L, "Explicit fixture Shadow") })
			{
				var row = new Db.Terrain { Id = id, Name = name, MovementRate = 1, StaminaCost = 1, TerrainBehaviourMode = "land", TagInformation = "<Tags/>" };
				foreach (var property in typeof(Db.Terrain).GetProperties().Where(x => x.PropertyType == typeof(string)))
					if (property.GetValue(row) is null) property.SetValue(row, "");
				db.Terrains.Add(row);
			}
			db.SaveChanges();
			bindings = bindings with { WaterSee = new([bindings.Utilities.Water], 101, 102,
				Enumerable.Range(0, 5).Select(rank => db.Tags.Single(x => x.Name == "Ethereal Divination " + rank).Id).ToArray()) };
		}
		var players = PlayerSnapshot(database);
		var noWine = RunPrepared(database, bindings);
		Require(noWine.Status == ArmageddonInstallStatus.Blocked && noWine.Modules.Count == 0 && PreparedIdentities(database).Count == 0 && players == PlayerSnapshot(database),
			"Missing owned Wine must block before any module writes.");
		RequirePrepared(RunPrepared(database, bindings with { WaterSee = null }));
		bindings = bindings with { Provisions = PreparedProvisionSelections(database, bindings, PreparedIdentities(database)) };
		RequirePrepared(RunPrepared(database, bindings with { WaterSee = null }));
		var previousPolicy = PreparedCapabilityMeritPolicy(database); var provisions = PreparedProvisionPolicy(database);
		foreach (var boundary in new[] { ArmageddonInstallCheckpoint.ContentCreated, ArmageddonInstallCheckpoint.BeforeCommit })
		{
			var failed = RunPrepared(database, bindings, (module, point) =>
			{ if (module == ArmageddonWaterSeeInstaller.Module && point == boundary) throw new IOException("Atomic Water/See fault " + boundary); });
			using var db = NewIndependentContext(database.ConnectionString);
			Require(failed.Status == ArmageddonInstallStatus.Failed && !db.SeederManagedRecords.Any(x => x.Module == ArmageddonWaterSeeInstaller.Module) &&
				db.MagicSpells.Count() == 8 && PreparedIdentities(database).Count == 203 && players == PlayerSnapshot(database) &&
				previousPolicy == PreparedCapabilityMeritPolicy(database) && provisions == PreparedProvisionPolicy(database), "Water/See transaction rollback/boundary isolation failed.");
			Console.WriteLine("WATERSEE-rollback=passed boundary:" + boundary + " earlier203 intact");
		}
		var lost = RunPrepared(database, bindings, (module, point) =>
		{ if (module == ArmageddonWaterSeeInstaller.Module && point == ArmageddonInstallCheckpoint.AfterCommit) throw new IOException("Lost Water/See acknowledgement"); });
		Require(lost.Status == ArmageddonInstallStatus.CommittedConfirmationFailed && PreparedIdentities(database).Count == 211 &&
			previousPolicy == PreparedCapabilityMeritPolicy(database) && players == PlayerSnapshot(database), "Lost acknowledgement changed final policy/player state.");
		var identities = PreparedIdentities(database);
		var completed = RunPrepared(database, bindings); RequirePrepared(completed);
		Require(completed.Availability.All(x => x.StoredAdmissions.Count == 9 && x.WithoutStoredAdmission.Count == 73) &&
			identities.All(x => PreparedIdentities(database)[x.Key] == x.Value) && players == PlayerSnapshot(database), "Retry identity/source closure/player preservation failed.");
		var finalPolicy = PreparedCapabilityMeritPolicy(database);
		var preserved = RunPrepared(database, bindings with { WaterSee = null, Provisions = null }); RequirePrepared(preserved);
		Require(preserved.Availability.All(x => x.StoredAdmissions.Count == 9) && finalPolicy == PreparedCapabilityMeritPolicy(database) &&
			provisions == PreparedProvisionPolicy(database) && players == PlayerSnapshot(database), "Null composition did not preserve content/admissions.");
		Console.WriteLine("WATERSEE-install=passed eight-records211 nine-source-admissions no-player-refresh null-preservation lost-ack same-identities");
		var host = PrepareRetirementHost(database, fixture, clock, wielding: true, consumablesAnatomy: true);
		var native = host.Native; var actor = native.Actor;
		var environment = ConfigureSeeStockWorld(native, database);
		LoadWaterSeeNative(native, database, bindings.Utilities,
			completed.Modules.Single(x => x.Module == ArmageddonMagicInstaller.Module).Identities,
			completed.Modules.Single(x => x.Module == ArmageddonTraditionInstaller.Module + ":admissions").Identities,
			9, completed.Modules.Single(x => x.Module == ArmageddonProvisionInstaller.Module).Identities,
			completed.Modules.Where(x => x.Module is ArmageddonPierceInstaller.Module or ArmageddonWaterSeeInstaller.Module)
				.SelectMany(x => x.Identities).ToDictionary(x => x.Key, x => x.Value));
		LoadWaterSeeResource(native, database);
		var scheduler = new MudSharp.Effects.EffectScheduler(native.World, clock); native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		var casting = new MagicCastingService(native.World, clock: () => RuntimeClock.UtcNow, random: () => 0.1, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		var cap = (SkillLevelBasedMagicCapability)native.World.MagicCapabilities.Get(identities["arm.capability.sorcerer"]!.Value)!;
		var staff = Mock.Of<MudSharp.Character.ICharacter>(x => x.Id == 999 && x.IsAdministrator(MudSharp.Accounts.PermissionLevel.JuniorAdmin));
		Require(casting.Enrol(staff, actor, cap.Id, "Explicit disposable installer qualification").Allowed, "Installed enrolment refused.");
		long Id(string key) => identities[key]!.Value;
		ITraitDefinition Skill(string key) => native.World.Traits.Get(Id(key + ".skill"))!;
		// Follow the installed source graph; no root/parent is replaced by a decorative stub.
		foreach (var parent in new[] { ArmageddonReviewedUtilityContent.UnravelEnchantmentKey, ArmageddonReviewedUtilityContent.SenseEnchantmentKey,
			ArmageddonReviewedUtilityContent.DrawWaterKey })
		{
			Require(casting.Acquisition(actor, Id(parent)) is not null, "Installed prerequisite absent: " + parent);
			actor.SetTraitValue(Skill(parent), 80); casting.NotifyProgress(actor, Skill(parent).Id);
		}
		foreach (var (key, parent) in new[] { (ArmageddonWaterSeeInstaller.WaterBreathingKey, ArmageddonReviewedProvisionContent.DrawWineKey),
			(ArmageddonWaterSeeInstaller.SeeTheUnbodiedKey, ArmageddonReviewedPierceContent.Key) })
		{
			actor.SetTraitValue(Skill(parent), 79); casting.NotifyProgress(actor, Skill(parent).Id);
			Require(casting.Acquisition(actor, Id(key)) is null, "Child acquired below raw80.");
			actor.SetTraitValue(Skill(parent), 80); casting.NotifyProgress(actor, Skill(parent).Id);
			Require(casting.Acquisition(actor, Id(key)) is { ControlledGrade: 1 } && actor.TraitRawValue(Skill(key)) == 30 &&
				casting.RawSkillImprovementCap(actor, Skill(key).Id) == 90, "Installed child raw80/open30/cap90.");
			actor.SetTraitValue(Skill(key), 90);
		}
		var state = new MagicCastingStateStore();
		var water = (MagicSpell)native.World.MagicSpells.Get(Id(ArmageddonWaterSeeInstaller.WaterBreathingKey))!;
		var see = (MagicSpell)native.World.MagicSpells.Get(Id(ArmageddonWaterSeeInstaller.SeeTheUnbodiedKey))!;
		var component = (GameItem)host.Prototypes.Values.Single(x => x.Name == "ARM03B2B Ethereal token 5").CreateNew(actor);
		component.GetItemType<IStackable>()!.Quantity = 8; native.World.Add(component); actor.Location.Insert(component, true); native.Body.Get(component, silent: true); FlushCasting(native);
		actor.PositionState = PositionStanding.Instance;
		var random = new WaterStockRandom { Value = 3 }; using var selectionRandom = Constants.PushRandom(random);
		void Ready() { actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(native.Resource, 1000); FlushCasting(native); }
		MagicCastingResult Cast(MagicSpell spell, int grade)
		{
			Ready(); var intent = new MagicCastingIntent(actor, cap.Id, spell.Id, grade, false, "me"); var quote = casting.Quote(intent);
			Require(quote.Allowed, quote.Reason); var before = actor.MagicResourceAmounts[native.Resource];
			var result = casting.Cast(intent); Require(result.Status == MagicCastingStatus.Succeeded, result.Message);
			Require(before - actor.MagicResourceAmounts[native.Resource] == quote.Invocation!.Costs.Single().Amount, "Installed quote/native debit mismatch.");
			Console.WriteLine($"WATERSEE-paid=passed spell:{spell.StockIdentity} grade:{grade} cost:{before - actor.MagicResourceAmounts[native.Resource]}"); return result;
		}
		Cast(water, 1); Cast(see, 1);
		VerifyWaterStockBreathing(native, native.World.Liquids.Get(bindings.Utilities.Water)!,
			native.World.Liquids.GetByName("ARM03C2 oil")!, true); VerifySeeStockChannels(native, true);
		foreach (var spell in new[] { water, see }) state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 7 });
		Cast(water, 7); Cast(see, 7);
		Require(component.GetItemType<IStackable>()!.Quantity == 6, "Two See casts must consume exactly two directly carried quantity units.");
		environment.Current = environment.Silt; Ready(); var balance = actor.MagicResourceAmounts[native.Resource];
		Require(casting.Cast(new(actor, cap.Id, see.Id, 1, false, "me")).Status == MagicCastingStatus.Refused &&
			actor.MagicResourceAmounts[native.Resource] == balance && component.GetItemType<IStackable>()!.Quantity == 6, "Installed Silt refusal paid/consumed.");
		environment.Current = environment.Shadow; Cast(see, 1); Require(component.GetItemType<IStackable>()!.Quantity == 6, "Installed Shadow exemption consumed.");
		environment.Current = environment.Outside; actor.PositionState = PositionSitting.Instance; Ready(); balance = actor.MagicResourceAmounts[native.Resource];
		Require(casting.Cast(new(actor, cap.Id, water.Id, 1, false, "me")).Status == MagicCastingStatus.Refused && actor.MagicResourceAmounts[native.Resource] == balance,
			"Installed editable posture filter failed."); actor.PositionState = PositionStanding.Instance;
		MagicSpellParent Parent(MagicSpell spell) => actor.EffectsOfType<MagicSpellParent>().Single(x => x.Spell.Id == spell.Id);
		Require(Parent(water).LifetimeState!.Grade == 7 && Parent(see).LifetimeState!.Grade == 7, "Installed strongest lifetime grade lost.");
		FlushCasting(native);
		RunWaterSeeReader(new(database.Name, fixture, DatabaseSeeder.Seeders.ArmageddonMagicSeeder.SerializeBindings(bindings),
			identities, PreparedCapabilityMeritPolicy(database), PlayerSnapshot(database), RuntimeClock.UtcNow, component.Id, 6,
			Parent(water).Identity, Parent(see).Identity, scheduler.ScheduledExpiry(Parent(water))!.Value,
			scheduler.ScheduledExpiry(Parent(see))!.Value, actor.MagicResourceAmounts[native.Resource]));
		Console.WriteLine("WATERSEE-native=passed actual-installed-SQL normal-MagicSpell-load native-payment source-acquisition low-high scoped-water ethereal-components Silt-Shadow cold-process");
		return 0;
	}

	private static void RunWaterSeeReader(WaterSeeReader input)
	{
		var path = Path.Combine(Path.GetTempPath(), "futuremud-watersee-reader_" + Guid.NewGuid().ToString("N") + ".json");
		File.WriteAllText(path, JsonSerializer.Serialize(input), new UTF8Encoding(false));
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add("--water-see-reader"); start.ArgumentList.Add(path);
		using var child = Process.Start(start)!;
		try
		{
			var output = child.StandardOutput.ReadToEndAsync(); var errors = child.StandardError.ReadToEndAsync();
			if (!child.WaitForExit(60000)) { child.Kill(true); Require(child.WaitForExit(30000), "Owned Water/See reader would not stop."); throw new TimeoutException("Owned reader timeout."); }
			Require(child.ExitCode == 0, output.GetAwaiter().GetResult() + errors.GetAwaiter().GetResult()); Console.Write(output.GetAwaiter().GetResult());
		}
		finally { if (!child.HasExited) { child.Kill(true); Require(child.WaitForExit(30000), "Owned reader cleanup failed."); } File.Delete(path); }
	}

	internal static int WaterSeeInstallerReader(string path)
	{
		var file = new FileInfo(Path.GetFullPath(path));
		Require(file.DirectoryName!.Equals(Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase) &&
			file.Name.StartsWith("futuremud-watersee-reader_", StringComparison.Ordinal) && file.Extension == ".json" && file.Length <= 2 * 1024 * 1024, "Bounded owned reader descriptor.");
		var input = JsonSerializer.Deserialize<WaterSeeReader>(File.ReadAllText(file.FullName, Encoding.UTF8))!;
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var bindings = JsonSerializer.Deserialize<ArmageddonPreparedWorldBindings>(input.Bindings, new JsonSerializerOptions { Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() } })!;
		Require(input.Identities.All(x => PreparedIdentities(database)[x.Key] == x.Value) && input.Policy == PreparedCapabilityMeritPolicy(database) && input.Players == PlayerSnapshot(database),
			"Cold SQL identity/policy/player bytes changed.");
		var nullRerun = RunPrepared(database, bindings with { WaterSee = null, Provisions = null }); RequirePrepared(nullRerun);
		Require(nullRerun.Availability.All(x => x.StoredAdmissions.Count == 9) && input.Policy == PreparedCapabilityMeritPolicy(database) && input.Players == PlayerSnapshot(database),
			"Cold null rerun lost source admissions or refreshed a player.");
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, consumablesAnatomy: true, additionalTraitGroups: ["Armageddon Spell"]); var native = host.Native;
		ConfigureSeeStockWorld(native, database);
		IReadOnlyDictionary<string, long> Rows(string module)
		{
			using var db = NewIndependentContext(database.ConnectionString);
			return db.SeederManagedRecords.AsNoTracking().Where(x => x.Module == module).ToDictionary(x => x.StableKey, x => x.LogicalId!.Value);
		}
		LoadWaterSeeNative(native, database, bindings.Utilities, Rows(ArmageddonMagicInstaller.Module), Rows(ArmageddonTraditionInstaller.Module), 9,
			Rows(ArmageddonProvisionInstaller.Module), Rows(ArmageddonPierceInstaller.Module).Concat(Rows(ArmageddonWaterSeeInstaller.Module)).ToDictionary(x => x.Key, x => x.Value));
		LoadWaterSeeResource(native, database);
		var scheduler = new MudSharp.Effects.EffectScheduler(native.World, clock); native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		SpellScopedWaterBreathingEffect.InitialiseEffectType(); SpellDetectEtherealEffect.InitialiseEffectType();
		using (var db = NewIndependentContext(database.ConnectionString)) native.Actor.RestoreCastingEffects(db.Characters.AsNoTracking().Single(x => x.Id == native.Actor.Id).EffectData);
		typeof(PerceivedItem).GetMethod("ScheduleCachedEffects", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(native.Actor, null);
		var parents = native.Actor.EffectsOfType<MagicSpellParent>().Where(x => ((MagicSpell)x.Spell).StockIdentity is ArmageddonWaterSeeInstaller.WaterBreathingKey or ArmageddonWaterSeeInstaller.SeeTheUnbodiedKey).ToArray();
		Require(parents.Length == 2 && parents.All(x => x.LifetimeState!.Grade == 7) &&
			parents.Any(x => x.Identity == input.WaterParent && scheduler.ScheduledExpiry(x) == input.WaterExpiry && x.SpellEffects.Single() is SpellScopedWaterBreathingEffect) &&
			parents.Any(x => x.Identity == input.SeeParent && scheduler.ScheduledExpiry(x) == input.SeeExpiry && x.SpellEffects.Single() is SpellDetectEtherealEffect),
			"Cold typed lifetime parent/child/grade/identity/deadline did not reload.");
		Require(native.World.TryGetItem(input.Component, true)!.GetItemType<IStackable>()!.Quantity == input.Quantity &&
			native.Actor.MagicResourceAmounts[native.Resource] == input.Balance, "Cold quantity/reserve changed.");
		VerifyWaterStockBreathing(native, native.World.Liquids.Get(bindings.Utilities.Water)!, native.World.Liquids.GetByName("ARM03C2 oil")!, true); VerifySeeStockChannels(native, true);
		Console.WriteLine("WATERSEE-reader=passed fresh-process211 nine-admissions preserved-players typed-child exact-parent grade7 deadline quantity reserve no-reroll");
		return 0;
	}

	private static void LoadWaterSeeResource(NativeRuntime native, TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		SetPrivateField(native.Body, "_traits", db.Traits.AsNoTracking().Where(x => x.BodyId == native.Body.Id).ToArray()
			.Select(x => CastingRequired(native.World.Traits.Get(x.TraitDefinitionId)).LoadTrait(x, native.Body)).ToList());
		var old = native.Resource;
		var resource = new SimpleMagicResource(db.MagicResources.AsNoTracking().Single(x => x.Id == old.Id), native.World);
		var amounts = (DoubleCounter<IMagicResource>)native.Actor.MagicResourceAmounts; var balance = amounts[old];
		amounts.Remove(old); amounts[resource] = balance;
		var catalogue = (All<IMagicResource>)native.World.MagicResources; catalogue.Remove(old); catalogue.Add(resource); SetPrivateMember(native, "Resource", resource);
		Require(resource.TryGetResourceCap(native.Actor, out var capacity, out var error) && capacity >= 50 && double.IsFinite(capacity),
			"Saved explicit native capacity could not fund scoped casts: " + error);
	}
}
