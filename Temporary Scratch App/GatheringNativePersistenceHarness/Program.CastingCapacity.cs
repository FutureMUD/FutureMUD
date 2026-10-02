using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Resources;

#nullable enable
namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RunCapacityAcceptanceChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString);
		var first = FixtureSeed.Create(database, "arm_capacity", false);
		var second = FixtureSeed.Create(database, "arm_capacity_other", false);
		var native = NativeRuntime.Load(first, database.ConnectionString, true);
		ConfigureCastingWorld(native, database.ConnectionString, true);
		var actor = native.Actor; var world = native.World;
		((All<ICharacter>)world.Characters).Add(actor);
		var attribute = CastingRequired(world.Traits.GetByName("ARM02 Agility"));
		var skill = CastingRequired(world.Traits.GetByName("ARM02 Earth Proficiency"));
		var otherSkill = CastingRequired(world.Traits.GetByName("ARM02 Sorcerer Proficiency"));
		var input = ReadCapacityInput(out var root);
		var formula = input.GetProperty("capacity_formula").GetString()!;
		TraitExpression expression;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var model = new MudSharp.Models.TraitExpression { Name = "ARM explicit capacity", Expression = formula };
			db.TraitExpressions.Add(model); db.SaveChanges();
			expression = new TraitExpression(model, world); ((All<ITraitExpression>)world.TraitExpressions).Add(expression);
		}
		Require(actor.AddTrait(attribute, 19), "Native capacity attribute did not open.");
		var reserve = (SimpleMagicResource)native.Resource;
		Require(reserve.BuildingCommand(actor, new StringStack($"capattribute {attribute.Id} {expression.Id} raw")), "Explicit native capacity binding refused.");
		var cap = (SkillLevelBasedMagicCapability)native.Capability;
		var spell = NewSupportFixtureSpell(native, "ARM Capacity Native", skill);
		foreach (var command in new[] { "grades efficiency source 50 0.5", "grades overreach 1.5 1" })
			Require(spell.BuildingCommand(actor, new StringStack(command)), "Capacity spell refused: " + command);
		foreach (var command in new[] { $"casting trait {skill.Id}", $"casting resources {reserve.Id} {reserve.Id} gather",
			$"casting entry add {spell.Id}", $"casting entry starting {spell.Id} on", $"casting entry skill {spell.Id} 30 90 relative", "casting enable on" })
			Require(cap.BuildingCommand(actor, new StringStack(command)), "Capacity route refused: " + command);
		var other = (SkillLevelBasedMagicCapability)cap.Clone("ARM Capacity Alternate");
		((All<IMagicCapability>)world.MagicCapabilities).Add(other);
		Require(other.BuildingCommand(actor, new StringStack($"casting trait {otherSkill.Id}")), "Alternate capacity trait refused.");
		Require(other.BuildingCommand(actor, new StringStack($"casting resources {reserve.Id} {second.ResourceId} gather")), "Alternate capacity reserve refused.");
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap), NativeRuntime.NewCapabilityMerit(other)]);
		world.SaveManager.Flush();
		var now = new DateTime(2026, 10, 2, 0, 0, 0, DateTimeKind.Utc);
		var service = new MagicCastingService(world, clock: () => now, random: () => 0.99, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var staff = new Mock<ICharacter>(); staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		Require(service.Enrol(staff.Object, actor, cap.Id, "capacity fixture").Allowed &&
			service.Enrol(staff.Object, actor, other.Id, "alternate capacity fixture").Allowed, "Capacity enrolment refused.");
		Require(actor.MagicResourceAmounts[reserve] == 0 && reserve.ResourceCap(actor) == 118, "Enrolment confused balance with maximum.");
		WriteCapacityAffordabilityMatrix(native, attribute, reserve, input, root);
		Require(actor.MagicResourceAmounts[reserve] == 0, "Matrix setup or attribute changes refilled energy.");
		actor.GetTrait(skill).Value = 90; // Affordability setup; natural improvement was verified in stage1A/1B.
		Require(reserve.ResourceCap(actor) == 118 && actor.TraitRawValue(skill) == 90, "Skill growth changed maximum capacity.");
		actor.AddResource(reserve, 200);
		Require(actor.MagicResourceAmounts[reserve] == 118, "Legitimate fixture credit exceeded native capacity.");
		var store = new MagicCastingStateStore();
		var pairs = 0;
		for (var mastery = 1; mastery <= 7; mastery++)
		{
			store.Write(acquired: store.Acquisition(actor.Id, spell.Id)! with { ControlledGrade = mastery, NextMasteryUtc = now.AddHours(1) });
			for (var requested = 1; requested <= Math.Min(7, mastery + 1); requested++)
			{
				var overreach = requested > mastery;
				var quote = service.Quote(new(actor, cap.Id, spell.Id, requested, overreach, "self"));
				Require(quote.Allowed && quote.Invocation!.Costs.Single().Amount == (overreach ? 56.25 : 25),
					$"Maximum-envelope native quote refused/mispriced controlled {mastery}, requested {requested}: {quote.Reason}");
				pairs++;
			}
		}
		Require(pairs == 34 && actor.MagicResourceAmounts[reserve] == 118, "Native quote matrix wrote resources.");
		store.Write(acquired: store.Acquisition(actor.Id, spell.Id)! with { ControlledGrade = 7 });
		MagicModule.MagicGeneric(actor, $"{cap.School.SchoolVerb} cast \"{spell.Name}\" grade 7 on self via {cap.Id}");
		Require(actor.MagicResourceAmounts[reserve] == 93 && store.Unresolved(actor.Id).Count == 0, "Native grade7 command failed to debit 25/complete.");
		actor.RemoveAllEffects<MagicSpellParent>(null, true); actor.RemoveAllEffects<MagicSpellLockout>(null, true);
		actor.AddResource(reserve, 200); now = now.AddHours(2);
		store.Write(acquired: store.Acquisition(actor.Id, spell.Id)! with { ControlledGrade = 6, NextMasteryUtc = now.AddHours(1) });
		MagicModule.MagicGeneric(actor, $"{cap.School.SchoolVerb} cast \"{spell.Name}\" grade 7 overreach on self via {cap.Id}");
		Require(actor.MagicResourceAmounts[reserve] == 61.75 && store.Unresolved(actor.Id).Count == 0, "Native grade7 overreach failed to debit 56.25/complete.");
		actor.RemoveAllEffects<MagicSpellParent>(null, true); actor.RemoveAllEffects<MagicSpellLockout>(null, true);
		store.Write(acquired: store.Acquisition(actor.Id, spell.Id)! with { ControlledGrade = 7 });
		actor.AddResource(reserve, 200);
		actor.SetTraitValue(attribute, 4);
		Require(reserve.ResourceCap(actor) == 48 && actor.MagicResourceAmounts[reserve] == 48, "Native attribute drop did not immediately clamp.");
		actor.SetTraitValue(attribute, 19);
		Require(reserve.ResourceCap(actor) == 118 && actor.MagicResourceAmounts[reserve] == 48, "Attribute rise refilled energy.");
		actor.AddResource(reserve, 200);
		Require(reserve.BuildingCommand(actor, new StringStack($"capattribute {attribute.Id} {expression.Id} effective")), "Effective capacity binding refused.");
		var penalty = new SpellTraitBoostEffect(actor, Mock.Of<IMagicSpellEffectParent>(), null!) { Trait = attribute, Bonus = -15 };
		actor.AddEffect(penalty);
		Require(reserve.ResourceCap(actor) == 48 && actor.MagicResourceAmounts[reserve] == 48, "Native effective penalty did not clamp before accounting.");
		actor.RemoveEffect(penalty);
		Require(reserve.ResourceCap(actor) == 118 && actor.MagicResourceAmounts[reserve] == 48, "Effect removal refilled energy.");
		Require(reserve.BuildingCommand(actor, new StringStack($"capattribute {attribute.Id} {expression.Id} raw")), "Raw capacity restore refused.");
		var focused = CreateSupportFocusedBody(database, native, second, cap, other);
		Require(focused.Body.AddTrait(attribute, 4), "Second body attribute refused.");
		var focus = typeof(MudSharp.Character.Character).GetMethod("SetFocusedInstance", BindingFlags.Instance | BindingFlags.NonPublic)!;
		focus.Invoke(actor, [focused.Actor]);
		Require(reserve.ResourceCap(focused.Actor) == 118 && focused.Actor.MagicResourceAmounts[reserve] == 48, "Focus substituted a secondary attribute or refilled the canonical reserve.");
		actor.AddResource(reserve, 200);
		actor.AttachBody(focused.Body); service.Reconcile(actor);
		Require(reserve.ResourceCap(actor) == 48 && actor.MagicResourceAmounts[reserve] == 48, "Canonical body fixture replacement did not clamp.");
		actor.AttachBody(native.Body); service.Reconcile(actor); focus.Invoke(actor, [null]);
		Require(reserve.ResourceCap(actor) == 118 && actor.MagicResourceAmounts[reserve] == 48, "Restoring canonical body refilled energy.");
		foreach (var invalid in new[] { "-1", "1.0/0.0" })
		{
			Require(expression.BuildingCommand(actor, new StringStack("formula " + invalid)), "Invalid runtime fixture expression did not parse.");
			var before = store.Acquisition(actor.Id, spell.Id);
			using var read = NewIndependentContext(database.ConnectionString);
			var operations = read.MagicCastingOperations.Count(x => x.CharacterId == actor.Id);
			actor.AddResource(reserve, 5);
			var quote = service.Quote(new(actor, cap.Id, spell.Id, 7, false, "self"));
			Require(!quote.Allowed && quote.Reason.Contains("capacity", StringComparison.OrdinalIgnoreCase) &&
				service.Cast(new(actor, cap.Id, spell.Id, 7, false, "self")).Status == MagicCastingStatus.Refused &&
				!actor.CanUseResource(reserve, 0) && !actor.UseResource(reserve, 0) && actor.MagicResourceAmounts[reserve] == 48 &&
				store.Acquisition(actor.Id, spell.Id) == before && read.MagicCastingOperations.Count(x => x.CharacterId == actor.Id) == operations,
				"Invalid capacity corrupted resources, payment receipts or mastery.");
		}
		Require(expression.BuildingCommand(actor, new StringStack("formula " + formula)), "Capacity expression repair refused.");
		Require(actor.MagicResourceAmounts[reserve] == 48, "Repairing capacity refilled energy.");
		Require(expression.BuildingCommand(actor, new StringStack("formula variable")) && actor.MagicResourceAmounts[reserve] == 19,
			"Editing a valid mapped expression did not immediately clamp.");
		Require(expression.BuildingCommand(actor, new StringStack("formula " + formula)) && actor.MagicResourceAmounts[reserve] == 19,
			"Raising an edited expression refilled energy.");
		actor.SetTraitValue(attribute, 0); actor.SetTraitValue(attribute, 19);
		Require(actor.MagicResourceAmounts[reserve] == 0 && reserve.ResourceCap(actor) == 118, "Zero/rising maximum refilled energy.");
		actor.AddResource(reserve, 17);
		Require(cap.BuildingCommand(actor, new StringStack("casting enable off")), "Capacity route disable refused.");
		service.Reconcile(actor); actor.SetMerits([]); service.Reconcile(actor);
		Require(cap.BuildingCommand(actor, new StringStack("casting enable on")), "Capacity route enable refused.");
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap), NativeRuntime.NewCapabilityMerit(other)]); service.Reconcile(actor);
		Require(service.Enrol(staff.Object, actor, cap.Id, "repeat capacity enrolment").Allowed && actor.MagicResourceAmounts[reserve] == 17,
			"Disable/detach/reattach/enrolment refill exploit.");
		FlushCasting(native);
		RunCastingReaderProcess(new(database.Name, first, second, spell.Id, cap.Id, other.Id, skill.Id, otherSkill.Id,
			null, 7, 17, 0, RawSkill: 90, CapacityAttribute: attribute.Id, CapacityExpression: expression.Id, Capacity: 118));
		Require(expression.BuildingCommand(actor, new StringStack("formula variable*10")) && actor.SetTraitValue(attribute, 10) &&
			reserve.BuildingCommand(actor, new StringStack($"capattribute {attribute.Id} {expression.Id} effective")), "Effective reload fixture setup refused.");
		var savedBoosts = new MagicSpellParent(actor, spell, actor);
		foreach (var bonus in new[] { 3.0, 7.0 })
		{
			var boost = new SpellTraitBoostEffect(actor, savedBoosts, null!) { Trait = attribute, Bonus = bonus };
			savedBoosts.AddSpellEffect(boost); actor.AddEffect(boost);
		}
		actor.AddEffect(savedBoosts); actor.AddResource(reserve, 133);
		Require(reserve.ResourceCap(actor) == 200 && actor.MagicResourceAmounts[reserve] == 150, "Saved effective reload fixture capacity/balance mismatch.");
		FlushCasting(native);
		RunCastingReaderProcess(new(database.Name, first, second, spell.Id, cap.Id, other.Id, skill.Id, otherSkill.Id,
			null, 7, 150, 0, RawSkill: 90, CapacityAttribute: attribute.Id, CapacityExpression: expression.Id, Capacity: 200, CapacityRaw: false));
		Console.WriteLine("ARM-CAPACITY-native=passed production-attribute-evaluator raw-and-effective immediate-clamp no-refill invalid-cap-payment-refusal canonical-focus owner-body-fixture-replacement grade7-normal:25 grade7-overreach:56.25 pairs:34 balance:17 max:118 separate-process-reload full-body-form-and-backup-command:NOT_RUN stock-mapping:PROVISIONAL");
		return 0;
	}

	private static JsonElement ReadCapacityInput(out string root)
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Design Documents", "Magic", "Armageddon_Capacity_Affordability_Scenarios.json"))) directory = directory.Parent;
		root = directory?.FullName ?? throw new InvalidOperationException("Capacity scenario input was not found in the checkout.");
		using var input = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "Design Documents", "Magic", "Armageddon_Capacity_Affordability_Scenarios.json")));
		return input.RootElement.Clone();
	}

	private static void WriteCapacityAffordabilityMatrix(NativeRuntime native, ITraitDefinition attribute, IMagicResource reserve, JsonElement input, string root)
	{
		using var tree = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "Design Documents", "Magic", input.GetProperty("source_roster").GetString()!)));
		var spells = tree.RootElement.GetProperty("rows").EnumerateArray().Where(x => x.GetProperty("kind").GetString() == "spell").ToArray();
		Require(spells.Length == 82, "Affordability source roster differs from exact 82 spells.");
		var output = Environment.GetEnvironmentVariable("FUTUREMUD_CAPACITY_ACCEPTANCE_OUTPUT") ?? Path.Combine(root, ".artifacts", "test-runs", "native-armageddon-20261002-stage1C");
		Directory.CreateDirectory(output);
		using var csv = new StreamWriter(Path.Combine(output, "affordability.csv"));
		csv.WriteLine("spell,printed_minimum,profile,scenario,attribute,capacity,controlled_grade,requested_grade,mode,energy,full_pool_affordable");
		var rows = 0; var refused = new Dictionary<string, int>(); var maxima = new Dictionary<string, double>();
		static string Number(double value) => value.ToString("R", CultureInfo.InvariantCulture);
		foreach (var profile in input.GetProperty("profiles").EnumerateArray())
		foreach (var scenario in input.GetProperty("scenarios").EnumerateArray())
		{
			native.Actor.SetTraitValue(attribute, scenario.GetProperty("attribute").GetDouble());
			Require(MagicResourceCapacity.TryGetCap(reserve, native.Actor, out var capacity, out var error) && capacity == scenario.GetProperty("expected_capacity").GetDouble(), "Native matrix capacity mismatch: " + error);
			var label = $"{profile.GetProperty("key").GetString()}/{scenario.GetProperty("key").GetString()}"; refused[label] = 0; maxima[label] = 0;
			foreach (var spell in spells)
			{
				var efficiency = new ControlledSpellEfficiency(spell.GetProperty("printed_minimum_mana").GetDouble(), profile.GetProperty("energy_scale").GetDouble());
				for (var mastery = 1; mastery <= 7; mastery++)
				for (var requested = 1; requested <= Math.Min(7, mastery + 1); requested++)
				{
					var overreach = requested > mastery;
					var energy = efficiency.Cost(mastery, requested) * (overreach ? profile.GetProperty("overreach_multiplier").GetDouble() : 1) + profile.GetProperty("secondary_cost").GetDouble();
					var affordable = energy <= capacity; if (!affordable) refused[label]++;
					maxima[label] = Math.Max(maxima[label], energy);
					csv.WriteLine($"{spell.GetProperty("key").GetString()},{Number(efficiency.MinimumCost)},{profile.GetProperty("key").GetString()},{scenario.GetProperty("key").GetString()},{Number(scenario.GetProperty("attribute").GetDouble())},{Number(capacity)},{mastery},{requested},{(overreach ? "overreach" : "normal")},{Number(energy)},{affordable.ToString().ToLowerInvariant()}");
					rows++;
				}
			}
		}
		csv.Flush();
		Require(rows == input.GetProperty("coverage").GetProperty("expected_matrix_rows").GetInt32() && refused.Where(x => x.Key.EndsWith("/advanced", StringComparison.Ordinal)).All(x => x.Value == 0), "A supported energy envelope has no advanced attainable configuration.");
		File.WriteAllText(Path.Combine(output, "affordability-summary.json"), JsonSerializer.Serialize(new { rows, spells = spells.Length, unaffordable_rows = refused, maximum_energy = maxima,
			status = "Provisional engineering scenarios; production curve and native attribute caps; energy envelopes only", quiet = "NOT_IMPLEMENTED", area = "NOT_IMPLEMENTED", additional_candidates = "72 require authored minima/configuration" }, new JsonSerializerOptions { WriteIndented = true }));
		native.Actor.SetTraitValue(attribute, 19);
		Console.WriteLine($"ARM-CAPACITY-matrix=passed rows:{rows} spells:82 normal-pairs:28 overreach-pairs:6 profiles:2 scenarios:3 advanced-unaffordable:0 provisional-mapping-and-coefficients energy-envelope-only");
	}

	private static void VerifyCapacityReload(TestDatabase database, CastingReader input)
	{
		var native = NativeRuntime.Load(input.Earth, database.ConnectionString, true, runtime =>
		{
			ConfigureCastingWorld(runtime, database.ConnectionString, false);
			var installedService = new MagicCastingService(runtime.World, flush: () => FlushCasting(runtime));
			runtime.WorldMock.SetupGet(x => x.MagicCasting).Returns(installedService);
		});
		var actor = native.Actor; var world = native.World; ((All<ICharacter>)world.Characters).Add(actor);
		var cap = CastingRequired(world.MagicCapabilities.Get(input.EarthCapability)); var other = CastingRequired(world.MagicCapabilities.Get(input.SorcererCapability));
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap), NativeRuntime.NewCapabilityMerit(other)]);
		var service = new MagicCastingService(world, flush: () => FlushCasting(native)); native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var resource = (SimpleMagicResource)native.Resource;
		Require(resource.AttributeCapacity == new MagicResourceAttributeCapacity(input.CapacityAttribute!.Value, input.CapacityExpression!.Value, input.CapacityRaw) &&
			MagicResourceCapacity.TryGetCap(resource, actor, out var maximum, out _) && maximum == input.Capacity && actor.MagicResourceAmounts[resource] == input.Balance,
			"Restart lost explicit attribute/expression configuration or confused maximum and balance.");
		service.Reconcile(actor); actor.CheckResources();
		if (!input.CapacityRaw)
		{
			Require(actor.MagicResourceAmounts[resource] == 150 && resource.ResourceCap(actor) == 200, "Live-service reload clamped before both persisted bonuses were restored.");
			actor.RemoveAllEffects<MagicSpellParent>(null, true);
			Require(resource.ResourceCap(actor) == 100 && actor.MagicResourceAmounts[resource] == 100, "Restored bonus removal did not clamp to the final real cap.");
			Console.WriteLine("ARM-CAPACITY-effective-reload=passed service-before-balance-inventory-effects-load persisted-bonuses:3+7 raw:10 max:200 saved-balance:150 preserved-until-complete bonus-removal-max:100-balance:100 production-restoration-scope full-character-login:NOT_RUN");
			return;
		}
		Require(actor.MagicResourceAmounts[resource] == 17 && !service.Quote(new(actor, cap.Id, input.Spell, 7, false, "self")).Allowed,
			"Restart/refreshed entitlement refilled energy or maximum funded a spent balance.");
		actor.SetTraitValue(CastingRequired(world.Traits.Get(input.CapacityAttribute.Value)), 20);
		Require(resource.ResourceCap(actor) == 121 && actor.MagicResourceAmounts[resource] == 17, "Restart attribute rise refilled energy.");
		Console.WriteLine("ARM-CAPACITY-reload=passed persisted-explicit-raw-attribute-expression max:118 balance:17 enrolment-no-refill attribute-rise-max:121-balance:17 grade7-insufficient-spent-balance");
	}
}
