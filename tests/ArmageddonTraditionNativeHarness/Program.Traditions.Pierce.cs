#nullable enable
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Resources;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record InstalledPierce(ArmageddonPierceInstallPlan Plan, ArmageddonTraditionInstallPlan Traditions, Dictionary<string, long> Identities);
	private sealed record InstalledPierceReader(string Database, FixtureIds Fixture, DateTime Now, ArmageddonMagicInstallPlan Utilities,
		Dictionary<string, long> UtilityIds, Dictionary<string, long> TraditionIds, InstalledProvisions Provisions, InstalledPierce Pierce,
		Guid Parent, DateTime Expiry, double Balance, Guid[] Operations);
	private static ArmageddonInstallResult InstallPierce(TestDatabase database, ArmageddonPierceInstallPlan plan, Action<ArmageddonInstallCheckpoint>? fault = null)
	{ using var db = NewIndependentContext(database.ConnectionString); return ArmageddonPierceInstaller.Install(db, plan, fault); }
	private static InstalledPierce InstallPierceExtension(TestDatabase database, InstalledProvisions provisions, IReadOnlyDictionary<string, long> ids)
	{
		var traditions = provisions.Traditions;
		var plan = new ArmageddonPierceInstallPlan(true, traditions.School, traditions.SourceResource, traditions.AlwaysFalseProg,
			ids[ArmageddonReviewedPierceContent.Key + ".skill"]);
		var players = TraditionPlayers(database);
		Dictionary<string, long?> previous;
		using (var db = NewIndependentContext(database.ConnectionString))
			previous = db.SeederManagedRecords.AsNoTracking().ToDictionary(x => x.Module + "|" + x.StableKey, x => x.LogicalId);
		Require(InstallPierce(database, plan with { PierceSkill = long.MaxValue }).Status == ArmageddonInstallStatus.Blocked, "Missing Pierce dependency did not fail closed.");
		foreach (var boundary in new[] { ArmageddonInstallCheckpoint.ContentCreated, ArmageddonInstallCheckpoint.BeforeCommit })
		{
			var failed = InstallPierce(database, plan, x => { if (x == boundary) throw new IOException("Installed Pierce interruption"); });
			using var db = NewIndependentContext(database.ConnectionString);
			Require(failed.Status == ArmageddonInstallStatus.Failed && !db.SeederManagedRecords.Any(x => x.Module == ArmageddonPierceInstaller.Module) && players == TraditionPlayers(database), "Native four-record rollback failed.");
		}
		var installed = InstallPierce(database, plan, x => { if (x == ArmageddonInstallCheckpoint.AfterCommit) throw new IOException("Lost Pierce acknowledgement"); });
		Require(installed.Status == ArmageddonInstallStatus.CommittedConfirmationFailed && installed.Identities.Count == 4, "Pierce lost confirmation classification failed.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var spell = db.MagicSpells.Find(installed.Identities[ArmageddonReviewedPierceContent.Key])!;
			spell.Name = "ARMTRAD builder Pierce";
			var clone = (Db.MagicSpell)db.Entry(spell).CurrentValues.ToObject(); clone.Id = 0; clone.Name = "ARMTRAD unowned Pierce clone"; db.MagicSpells.Add(clone); db.SaveChanges();
			RequireInstalled(InstallPierce(database, plan)); RequireInstalled(InstallPierce(database, plan));
			Require(db.MagicSpells.AsNoTracking().Single(x => x.Id == spell.Id).Name == spell.Name && !db.SeederManagedRecords.Any(x => x.EntityType == nameof(Db.MagicSpell) && x.LogicalId == clone.Id), "Pierce rerun adopted builder rename/clone.");
			var record = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonReviewedPierceContent.Key);
			record.Retired = true; db.SaveChanges(); Require(InstallPierce(database, plan).Status == ArmageddonInstallStatus.Blocked, "Pierce retirement ignored.");
			record.Retired = false; db.SaveChanges(); db.Remove(spell); db.SaveChanges();
			Require(InstallPierce(database, plan).Status == ArmageddonInstallStatus.Blocked && !db.MagicSpells.AsNoTracking().Any(x => x.Id == spell.Id), "Pierce owned deletion resurrected.");
			db.Entry(spell).State = EntityState.Added; db.SaveChanges();
		}
		var bindings = traditions.ImplementedSpells.ToDictionary(x => x.Key, x => x.Value);
		bindings.Add(ArmageddonReviewedPierceContent.Key, installed.Identities[ArmageddonReviewedPierceContent.Key]);
		var expanded = traditions with { ImplementedSpells = bindings };
		var result = InstallTraditions(database, expanded); RequireTraditions(result);
		Require(result.AvailableSpells.Count == 7 && result.UnavailableSpells.Count == 75 && result.Identities.All(x => ids[x.Key] == x.Value) && players == TraditionPlayers(database), "Pierce closure changed existing identities/player state or admitted incomplete paths.");
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(db.SeederManagedRecords.Count() == 203 && db.SeederManagedRecords.AsNoTracking().Where(x => x.Module != ArmageddonPierceInstaller.Module).AsEnumerable().All(x => previous[x.Module + "|" + x.StableKey] == x.LogicalId), "Pierce changed previous199 ownership identities.");
		Console.WriteLine("ARMTRAD-pierce-install=passed four-atomic-records retained171-21-7 missing-dependency-refusal rollback lost-ack builder-rename-clone deletion-retirement seven-closed-admissions 75-unavailable no-player-refresh");
		return new(plan, expanded, installed.Identities.ToDictionary(x => x.Key, x => x.Value));
	}
	private static void InstalledPierceTerrain(NativeRuntime native, string name)
	{
		var terrain = Mock.Get(native.Actor.Location.CurrentOverlay.Terrain);
		terrain.SetupGet(x => x.Type).Returns(ProgVariableTypes.Terrain); terrain.SetupGet(x => x.GetObject).Returns(terrain.Object);
		terrain.SetupGet(x => x.Name).Returns(name); terrain.Setup(x => x.GetProperty("name")).Returns(new TextVariable(name));
		var room = Mock.Get(native.Actor.Location); room.SetupGet(x => x.Type).Returns(ProgVariableTypes.Location);
		room.SetupGet(x => x.GetObject).Returns(native.Actor.Location); room.Setup(x => x.GetProperty("terrain")).Returns(terrain.Object);
	}
	private static void InstalledPierceReserve(NativeRuntime native, TestDatabase database, bool author)
	{
		// The shared fixture caps its stand-in reserve at100. Use an actual saved, builder-authored
		// resource with explicit test capacity for seven-grade paid overreach; installation grants none.
		using var db = NewIndependentContext(database.ConnectionString);
		if (!author)
		{
			// LoadTraditionNative installs its native definition catalogue after the host loads the body.
			// Rebind persisted body traits to that same catalogue; never fabricate a reload attribute.
			SetPrivateField(native.Body, "_traits", db.Traits.AsNoTracking().Where(x => x.BodyId == native.Body.Id).ToArray()
				.Select(x => CastingRequired(native.World.Traits.Get(x.TraitDefinitionId)).LoadTrait(x, native.Body)).ToList());
		}
		var old = native.Resource;
		var resource = new SimpleMagicResource(db.MagicResources.AsNoTracking().Single(x => x.Id == old.Id), native.World);
		var amounts = (DoubleCounter<IMagicResource>)native.Actor.MagicResourceAmounts;
		var balance = amounts[old];
		if (author)
		{
			var attribute = native.World.Traits.GetByName("ARM02 Agility")!;
			Require(native.Actor.HasTrait(attribute) || native.Actor.AddTrait(attribute, 19), "Installed Pierce capacity attribute");
			var capacity = new Db.TraitExpression { Name = "Installed Pierce disposable test capacity", Expression = "1000" };
			db.TraitExpressions.Add(capacity); db.SaveChanges();
			((All<ITraitExpression>)native.World.TraitExpressions).Add(new TraitExpression(capacity, native.World));
			Require(resource.BuildingCommand(native.Actor, new StringStack($"capattribute {attribute.Id} {capacity.Id} raw")), "Installed Pierce authored reserve capacity");
			using (new FMDB()) { resource.Save(); FMDB.Context.SaveChanges(); }
			FlushCasting(native);
		}
		// Configure before catalogue replacement: adding a native attribute reconciles live caps,
		// and an unconfigured SimpleMagicResource legitimately has a zero cap.
		amounts.Remove(old); amounts[resource] = balance;
		var catalogue = (All<IMagicResource>)native.World.MagicResources;
		catalogue.Remove(old); catalogue.Add(resource); SetPrivateMember(native, "Resource", resource);
		Require(resource.TryGetResourceCap(native.Actor, out var capacityValue, out var capacityError) && capacityValue == 1000 && native.Actor.MagicResourceAmounts[resource] == balance, $"Installed Pierce saved capacity:{capacityValue} balance:{native.Actor.MagicResourceAmounts[resource]} expected:{balance} configuration:{capacityError}");
	}
	private static void VerifyInstalledPierce(RetirementHost host, TestDatabase database, FixtureIds fixture, HarnessClock clock,
		ArmageddonMagicInstallPlan utilities, IReadOnlyDictionary<string, long> utilityIds, IReadOnlyDictionary<string, long> ids,
		InstalledProvisions provisions, InstalledPierce installed)
	{
		var native = host.Native; var world = native.World; var actor = native.Actor;
		var casting = (MagicCastingService)CastingRequired(world.MagicCasting);
		var sense = CastingRequired(world.Traits.Get(ids[ArmageddonReviewedUtilityContent.SenseEnchantmentKey + ".skill"]));
		var trait = CastingRequired(world.Traits.Get(installed.Plan.PierceSkill)); var spellId = installed.Identities[ArmageddonReviewedPierceContent.Key];
		Require(!actor.HasTrait(trait), "Pierce acquired before Sense80.");
		actor.SetTraitValue(sense, 79.5); casting.NotifyProgress(actor, sense.Id);
		Require(!actor.HasTrait(trait) && casting.Acquisition(actor, spellId) is null, "Pierce opened below Sense80.");
		Require(((Skill)actor.GetTrait(sense)).TraitUsed(actor, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []) && actor.TraitRawValue(sense) == 80,
			"Actual native Sense use did not cross80.");
		Require(actor.GetTrait(trait) is Skill && actor.TraitRawValue(trait) == 30 && casting.Acquisition(actor, spellId)!.ControlledGrade == 1,
			"Sense80 did not open native Pierce30/grade1.");
		actor.GetTrait(trait).Value += 100;
		Require(actor.TraitRawValue(trait) == 90 && !actor.HasTrait(world.Traits.Get(ids["arm.spell.night_eyes.skill"])) &&
			!actor.HasTrait(world.Traits.Get(ids[ArmageddonReviewedUtilityContent.MendFleshKey + ".skill"])), "Pierce cap90 or unavailable child/Mend path failed.");
		Console.WriteLine("ARMTRAD-pierce-progression=passed actual-native-TraitUsed Sense79.5-to80 Pierce30-grade1 raw-cap90 NightEyes-Mend-unavailable");
		var scheduler = new EffectScheduler(world, clock); native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		InstalledPierceTerrain(native, "Desert");
		InstalledPierceReserve(native, database, true);
		var cap = ids["arm.capability.sorcerer"]; var operations = new List<Guid>(); var state = new MagicCastingStateStore();
		MagicCastingResult Cast(long spell, int grade, bool overreach = false)
		{
			actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(native.Resource, 1000); FlushCasting(native);
			var intent = new MagicCastingIntent(actor, cap, spell, grade, overreach, "me"); var quote = casting.Quote(intent); Require(quote.Allowed, $"Installed Pierce spell:{spell} grade:{grade} overreach:{overreach} reserve:{actor.MagicResourceAmounts[native.Resource]}: {quote.Reason}");
			var balance = actor.MagicResourceAmounts[native.Resource]; var result = casting.Cast(intent);
			Require(result.Status == MagicCastingStatus.Succeeded && balance - actor.MagicResourceAmounts[native.Resource] == quote.Invocation!.Costs.Single().Amount, "Installed Pierce actual cast/debit failed: " + result.Message);
			operations.Add(result.OperationId!.Value); FlushCasting(native); return result;
		}
		Cast(utilityIds[ArmageddonReviewedUtilityContent.UnravelEnchantmentKey], 1);
		Require(!actor.Effects.Any(), "Paid Unravel did not remove preceding Sense before isolated Pierce controls.");
		MagicSpellParent Parent() => actor.EffectsOfType<MagicSpellParent>().Single(x => x.LifetimeGroup == ArmageddonReviewedPierceContent.LifetimeGroup);
		Cast(spellId, 1);
		Require(scheduler.ScheduledExpiry(Parent()) == RuntimeClock.UtcNow.AddSeconds(3000) && Parent().LifetimeState!.Grade == 1, "Installed initial source lifetime/power failed.");
		for (var grade = 2; grade <= 7; grade++)
		{
			clock.Advance(TimeSpan.FromSeconds(601)); scheduler.CheckSchedules();
			var previous = Parent(); var oldGrade = previous.LifetimeState!.Grade;
			var expected = RuntimeClock.UtcNow.AddSeconds(Math.Min(48, Math.Ceiling(scheduler.RemainingDuration(previous).TotalSeconds / 600) + 5 * grade) * 600);
			var paid = Cast(spellId, grade, true); var current = Parent();
			Require(current.Identity != previous.Identity && !actor.Effects.Contains(previous) && !previous.SpellEffects.Any() && !scheduler.IsScheduled(previous) &&
				current.LifetimeState!.Grade == grade && grade > oldGrade && scheduler.ScheduledExpiry(current) == expected && casting.Acquisition(actor, spellId)!.ControlledGrade == grade &&
				XElement.Parse(state.Operation(paid.OperationId!.Value)!.Definition).Attribute("masterySample") is not null, "Paid installed overreach did not accumulate/master through actual service.");
		}
		var strongest = Parent(); var power = strongest.Power; var expiry = scheduler.ScheduledExpiry(strongest);
		Require(expiry == RuntimeClock.UtcNow.AddSeconds(28800) && strongest.LifetimeState!.Grade == 7, "Installed Pierce failed cap48/strongest grade7.");
		var unchanged = Cast(spellId, 1);
		Require(Parent().LifetimeState!.Grade == 7 && Parent().Power == power && scheduler.ScheduledExpiry(Parent()) == expiry &&
			!(bool)XElement.Parse(state.Operation(unchanged.OperationId!.Value)!.Definition).Attribute("applied")! &&
			XElement.Parse(state.Operation(unchanged.OperationId.Value)!.Definition).Attribute("masterySample") is null, "At-cap installed low-grade cast changed strength/deadline or sampled mastery.");
		FlushCasting(native); var players = TraditionPlayers(database);
		RequireInstalled(InstallPierce(database, installed.Plan)); RequireInstalled(InstallProvisions(database, provisions.Plan)); RequireTraditions(InstallTraditions(database, installed.Traditions));
		Require(players == TraditionPlayers(database), "Active seven-admission installer rerun changed acquisition, native skills, reserve or merits.");
		RunItemReaderProcess(new InstalledPierceReader(database.Name, fixture, RuntimeClock.UtcNow, utilities, utilityIds.ToDictionary(x => x.Key, x => x.Value),
			ids.ToDictionary(x => x.Key, x => x.Value), provisions, installed, Parent().Identity, expiry!.Value, actor.MagicResourceAmounts[native.Resource], operations.ToArray()), "--traditions-pierce-reader");
		Console.WriteLine("ARMTRAD-pierce-output=passed installed-native-paid-source-accumulation legitimate-six-overreach-mastery-advances cap48 strongest-grade7-power paid-nochange active-rerun-conservation");
	}
	private static int InstalledPierceRestart(string encoded)
	{
		var input = JsonSerializer.Deserialize<InstalledPierceReader>(Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var players = TraditionPlayers(database); var installed = InstallPierce(database, input.Pierce.Plan); RequireInstalled(installed);
		Require(installed.Identities.All(x => input.Pierce.Identities[x.Key] == x.Value) && players == TraditionPlayers(database), "Fresh Pierce installer changed stable IDs/player state.");
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, consumablesAnatomy: true, additionalTraitGroups: ["Armageddon Spell"]); var native = host.Native; var actor = native.Actor;
		Require(native.World.Traits.Any(x => x.Group == "ARM02") && native.World.Traits.Count(x => x.Group == "Armageddon Spell") == 82, "Pierce reader lost default/additional native trait selection.");
		LoadTraditionNative(native, database, input.Utilities, input.UtilityIds, input.TraditionIds, 7, input.Provisions.Identities, installed.Identities);
		InstalledPierceReserve(native, database, false);
		var scheduler = new EffectScheduler(native.World, clock); native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		SpellDetectInvisibleEffect.InitialiseEffectType();
		using (var db = NewIndependentContext(database.ConnectionString)) actor.RestoreCastingEffects(db.Characters.Find(actor.Id)!.EffectData);
		typeof(PerceivedItem).GetMethod("ScheduleCachedEffects", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(actor, null);
		MagicSpellParent Parent() => actor.EffectsOfType<MagicSpellParent>().Single(x => x.LifetimeGroup == ArmageddonReviewedPierceContent.LifetimeGroup);
		var parent = Parent(); var spellId = installed.Identities[ArmageddonReviewedPierceContent.Key]; var state = new MagicCastingStateStore();
		Require(parent.Identity == input.Parent && parent.LifetimeState == new MagicSpellLifetimeState(new(ArmageddonReviewedPierceContent.LifetimeGroup, 600, 48), 7) &&
			parent.Power == ((MagicSpell)native.World.MagicSpells.Get(spellId)!).GradeProfile!.Grades.Single(x => x.Grade == 7).Power && scheduler.ScheduledExpiry(parent) == input.Expiry &&
			parent.SpellEffects.Count() == 1 && actor.MagicResourceAmounts[native.Resource] == input.Balance && state.Acquisition(actor.Id, spellId)!.ControlledGrade == 7 &&
			input.Operations.All(x => state.Operation(x)!.Stage == "Completed"), "Fresh installed Pierce lost policy/strength/deadline/reserve/acquisition/receipts.");
		InstalledPierceTerrain(native, "Desert");
		var casting = new MagicCastingService(native.World, clock: () => RuntimeClock.UtcNow, random: () => throw new InvalidOperationException("No replay sample"), flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		var retry = casting.Cast(new(actor, input.TraditionIds["arm.capability.sorcerer"], spellId, 1, false, "me", OriginId: input.Operations.Last()));
		Require(retry.Status == MagicCastingStatus.Refused && actor.MagicResourceAmounts[native.Resource] == input.Balance && scheduler.ScheduledExpiry(parent) == input.Expiry, "Fresh installed origin replayed/refunded/refreshed.");
		clock.Advance(TimeSpan.FromSeconds(601)); scheduler.CheckSchedules();
		var expected = RuntimeClock.UtcNow.AddSeconds(Math.Min(48, Math.Ceiling(scheduler.RemainingDuration(parent).TotalSeconds / 600) + 5) * 600);
		actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(native.Resource, 1000); FlushCasting(native);
		var intent = new MagicCastingIntent(actor, input.TraditionIds["arm.capability.sorcerer"], spellId, 1, false, "me"); var quote = casting.Quote(intent); Require(quote.Allowed, quote.Reason);
		var balance = actor.MagicResourceAmounts[native.Resource]; var result = casting.Cast(intent); var current = Parent();
		Require(result.Status == MagicCastingStatus.Succeeded && balance - actor.MagicResourceAmounts[native.Resource] == quote.Invocation!.Costs.Single().Amount &&
			current.Identity != parent.Identity && current.LifetimeState!.Grade == 7 && current.Power == parent.Power && scheduler.ScheduledExpiry(current) == expected &&
			!actor.Effects.Contains(parent) && !parent.SpellEffects.Any() && !scheduler.IsScheduled(parent), "Fresh ordinary installed low-grade accumulation failed.");
		var paidBalance = actor.MagicResourceAmounts[native.Resource]; clock.Advance(expected - RuntimeClock.UtcNow); scheduler.CheckSchedules(); FlushCasting(native);
		Require(!actor.EffectsOfType<MagicSpellParent>().Any(x => x.LifetimeGroup == ArmageddonReviewedPierceContent.LifetimeGroup) && !actor.EffectsOfType<SpellDetectInvisibleEffect>().Any() &&
			(actor.GetPerception(PerceptionTypes.None) & PerceptionTypes.VisualMagical) == PerceptionTypes.None && actor.MagicResourceAmounts[native.Resource] == paidBalance, "Fresh exact Pierce expiry retained child/perception or changed reserve.");
		Console.WriteLine("ARMTRAD-pierce-reader=passed fresh-process seven-native-admissions stable-four-identities policy-source-grade7-power exact-deadline completed-operations no-replay no-refund fresh-paid-low-grade-accumulation exact-expiry-cleanup");
		return 0;
	}
}
