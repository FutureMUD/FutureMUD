#nullable enable
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Merits;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record StockProgressionReader(string Database, FixtureIds Fixture, ArmageddonProgressionMode Mode,
		long MainTrait, long RootSpell, long BranchSpell, double Raw, double Balance, DateTime SkillDeadline,
		Dictionary<long, int> Grades);

	private static void RunStockProgressionFixtureChecks()
	{
		foreach (var mode in Enum.GetValues<ArmageddonProgressionMode>()) StockProgressionFixture(mode);
	}

	private static int RunStockProgressionFixturesOnly()
	{
		RunStockProgressionFixtureChecks();
		return 0;
	}

	private static void StockProgressionFixture(ArmageddonProgressionMode mode)
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		var fixture = FixtureSeed.Create(database, "arm_s2_" + mode, false);
		var native = NativeRuntime.Load(fixture, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, true);
		var stock = new ArmageddonStockProgression(mode);
		var cap = stock.Capabilities.Single(x => x.Key == "arm.capability.earth");
		var bindings = stock.Bindings.Where(x => x.CapabilityKey == cap.Key).ToArray();
		var traits = new Dictionary<string, long>();
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			foreach (var key in bindings.Select(x => x.TraitKey).Distinct())
			{
				var trait = new Db.TraitDefinition { Name = "S2 fixture " + key, Type = 0, OwnerScope = 1, TraitGroup = "ARM02",
					Alias = "", ChargenBlurb = "Isolated progression fixture; not production spell content", ValueExpression = "100",
					ExpressionId = fixture.TraitExpressionId };
				db.TraitDefinitions.Add(trait);
				db.SaveChanges();
				traits.Add(key, trait.Id);
			}
		}
		ConfigurePracticeImprovement(native, database.ConnectionString, true);
		var world = native.World;
		var actor = native.Actor;
		var rootKey = "arm.spell.earthen_ward";
		var branchKey = "arm.spell.stone_skin";
		var main = world.Traits.Get(traits[bindings.Single(x => x.ContentKey == rootKey).TraitKey])!;
		var spells = new Dictionary<string, long>();
		foreach (var key in cap.SpellKeys)
		{
			var spell = NewSupportFixtureSpell(native, "S2 isolated " + key, main);
			Require(spell.BuildingCommand(actor, new StringStack("grades practice fixture")), "S2 fixture practice configuration refused.");
			spells.Add(key, spell.Id);
		}
		world.SaveManager.Flush();
		SkillLevelBasedMagicCapability capability;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var row = db.MagicCapabilities.Single(x => x.Id == native.Capability.Id);
			var xml = XElement.Parse(row.Definition);
			xml.Element("Casting")?.Remove();
			xml.Element("Regenerators")?.RemoveNodes();
			xml.Add(stock.Casting(cap, traits, native.Resource.Id, spells, sourceResource: native.Resource.Id));
			row.Definition = xml.ToString();
			db.SaveChanges();
			capability = (SkillLevelBasedMagicCapability)MagicCapabilityFactory.LoadCapability(row, world);
			((All<IMagicCapability>)world.MagicCapabilities).Remove(native.Capability);
			((All<IMagicCapability>)world.MagicCapabilities).Add(capability);
		}
		Require(capability.CastingConfigurationErrors().Count == 0, string.Join("\n", capability.CastingConfigurationErrors()));
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(capability)]);
		FlushCasting(native);
		var now = new DateTime(2026, 10, 11, 0, 0, 0, DateTimeKind.Utc);
		var service = new MagicCastingService(world, clock: () => now, random: () => 0.1, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var role = new Mock<IChargenRole>();
		role.SetupProperty(x => x.RoleType, ChargenRoleType.Class);
		role.SetupGet(x => x.Id).Returns(1);
		role.SetupGet(x => x.AdditionalMerits).Returns(actor.Merits);
		var balance = actor.MagicResourceAmounts.GetValueOrDefault(native.Resource);
		var result = service.EnrolFromSelectedClass(actor, role.Object).Single();
		Require(result.Result.Allowed, result.Result.Message);
		var rootBinding = bindings.Single(x => x.ContentKey == rootKey);
		Require(actor.TraitRawValue(main) == rootBinding.Opening && service.Acquisition(actor, spells[rootKey])!.ControlledGrade == 1,
			"S2 fixture new-class enrolment did not open exact raw/grade-one roots.");
		Require(actor.MagicResourceAmounts.GetValueOrDefault(native.Resource) == balance && service.Acquisition(actor, spells[branchKey]) is null,
			"S2 fixture enrolment refilled a reserve or prematurely acquired a child.");
		// Deterministic native improver accelerates this fixture only; stock remains chance .1/gain .5.
		var iterations = mode == ArmageddonProgressionMode.Simple ? 1 : 2;
		for (var i = 0; i < iterations; i++)
		{
			now += TimeSpan.FromMinutes(11);
			actor.AddResource(native.Resource, 100);
			var quote = service.Quote(new(actor, capability.Id, spells[rootKey], 1, false, "", MagicCastingMode.Practice));
			Require(quote.Allowed, quote.Reason);
			var before = actor.MagicResourceAmounts[native.Resource];
			var raw = actor.TraitRawValue(main);
			var cast = service.Cast(new(actor, capability.Id, spells[rootKey], 1, false, "", MagicCastingMode.Practice));
			Require(cast.Status == MagicCastingStatus.Started, cast.Message);
			var action = actor.EffectsOfType<MagicPracticeAction>().Single();
			Require(actor.MagicResourceAmounts[native.Resource] == before - quote.Invocation!.Costs.Single().Amount && actor.TraitRawValue(main) == raw,
				"S2 fixture did not prepay practice or advanced raw before deadline.");
			now += TimeSpan.FromSeconds(30);
			action.ExpireEffect();
			action.ExpireEffect();
			Require(actor.TraitRawValue(main) == Math.Min(rootBinding.RawCap, raw + 10) && !actor.EffectsOfType<MagicSpellParent>().Any(),
				"S2 fixture ordinary paid practice did not make one raw gain or manifested an effect.");
		}
		Require(service.Acquisition(actor, spells[branchKey]) is { ControlledGrade: 1 },
			$"S2 fixture normal raw gain did not acquire the approved child at grade one; mode:{mode} raw:{actor.TraitRawValue(main)}.");
		var childTrait = world.Traits.Get(traits[bindings.Single(x => x.ContentKey == branchKey).TraitKey])!;
		Require(actor.TraitRawValue(childTrait) == (mode == ArmageddonProgressionMode.Simple ? 20 : bindings.Single(x => x.ContentKey == branchKey).Opening),
			"S2 fixture child raw opening is not the approved mode-specific value.");
		var store = new MagicCastingStateStore();
		var grades = spells.Values.Where(id => store.Acquisition(actor.Id, id) is not null).ToDictionary(id => id, id => store.Acquisition(actor.Id, id)!.ControlledGrade);
		var reader = new StockProgressionReader(database.Name, fixture, mode, main.Id, spells[rootKey], spells[branchKey], actor.TraitRawValue(main),
			actor.MagicResourceAmounts[native.Resource], store.Opportunity(actor.Id, main.Id)!.NextUtc, grades);
		FlushCasting(native);
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location);
		start.ArgumentList.Add("--stock-progression-reader");
		start.ArgumentList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(reader))));
		using var child = Process.Start(start)!;
		var stdout = child.StandardOutput.ReadToEndAsync();
		var stderr = child.StandardError.ReadToEndAsync();
		if (!child.WaitForExit(60000)) { child.Kill(true); throw new TimeoutException("S2 fixture restart exceeded 60 seconds."); }
		Require(child.ExitCode == 0, stdout.GetAwaiter().GetResult() + stderr.GetAwaiter().GetResult());
		Console.Write(stdout.GetAwaiter().GetResult());
		Console.WriteLine($"ARMS2-progression-fixture=passed mode:{mode} complete-Earth-graph:{spells.Count} real-native-fixture-spells paid-practice:30s raw:{reader.Raw} automatic-child-grade:1 separate-process-reload production-spell-effects:NOT_QUALIFIED");
	}

	private static int ReadStockProgressionFixture(string encoded)
	{
		var input = JsonSerializer.Deserialize<StockProgressionReader>(Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database);
		ConfigureNativeDatabase(database.ConnectionString);
		var native = NativeRuntime.Load(input.Fixture, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, false);
		ConfigurePracticeImprovement(native, database.ConnectionString, false);
		var actor = native.Actor;
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(native.Capability)]);
		var service = new MagicCastingService(native.World, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var store = new MagicCastingStateStore();
		Require(actor.TraitRawValue(native.World.Traits.Get(input.MainTrait)) == input.Raw && actor.MagicResourceAmounts[native.Resource] == input.Balance,
			"S2 fixture restart changed raw proficiency or reserve.");
		foreach (var grade in input.Grades) Require(store.Acquisition(actor.Id, grade.Key)!.ControlledGrade == grade.Value, "S2 fixture restart changed grade history.");
		Require(store.Opportunity(actor.Id, input.MainTrait)!.NextUtc == input.SkillDeadline && store.Unresolved(actor.Id).Count == 0,
			"S2 fixture restart changed training deadline or left an unresolved operation.");
		var before = store.Enrolment(actor.Id, ((IMagicCastingCapability)native.Capability).CastingPolicy!.Identity)!;
		service.Reconcile(actor);
		Require(store.Enrolment(actor.Id, before.CapabilityIdentity) == before && actor.TraitRawValue(native.World.Traits.Get(input.MainTrait)) == input.Raw,
			"S2 fixture restart re-enrolled or trained the character.");
		Console.WriteLine($"ARMS2-restart-fixture=passed mode:{input.Mode} raw-grade-reserve-deadline preserved");
		return 0;
	}
}
