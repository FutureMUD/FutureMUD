#nullable enable

using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Commands.Helpers;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	internal static int EtherealComponentAdapterMain(string[] args)
	{
		if (args.FirstOrDefault() is not ("--ethereal-component-adapter-run" or "--ethereal-component-adapter-reader")) return EtherealAdapterMain(args);
		try
		{
			OwnedConnections.Install();
			return args[0] == "--ethereal-component-adapter-reader" ? ReadEtherealComponentAdapter(args[1]) : RunEtherealComponentAdapter();
		}
		catch (Exception error) { Console.Error.WriteLine(error); return 1; }
	}

	private const string EtherealComponentAdapterGroup = "armageddon.detect_ethereal";
	private sealed record EtherealComponentAdapterReader(string Database, FixtureIds Fixture, DateTime Now, long Spell,
		DateTime Expiry, Guid ParentIdentity, Guid Operation, double Balance, long Component, int Quantity, bool Expire);

	private static void VerifyNativeEtherealComponentChannels(NativeRuntime native, bool granted)
	{
		var expected = PerceptionTypes.VisualEthereal | PerceptionTypes.SenseEthereal;
		Require((native.Actor.GetPerception(PerceptionTypes.None) & expected) == (granted ? expected : PerceptionTypes.None),
			"Native character ethereal perception channels disagree with retained grant");
	}

	private sealed class EtherealComponentEnvironment(ITerrain outside, ITerrain silt, ITerrain shadow)
	{
		public ITerrain Outside { get; } = outside;
		public ITerrain Silt { get; } = silt;
		public ITerrain Shadow { get; } = shadow;
		public ITerrain Current { get; set; } = outside;
	}

	private static EtherealComponentEnvironment ConfigureEtherealComponentWorld(NativeRuntime native, TestDatabase database)
	{
		FinaliseStormTags(database, native.World);
		ITerrain Terrain(long id, string name) => Mock.Of<ITerrain>(x => x.Id == id && x.Name == name &&
			x.TerrainBehaviourString == "land" && x.TerrainLayers == new[] { RoomLayer.GroundLevel } && x.Tags == Array.Empty<ITag>());
		var environment = new EtherealComponentEnvironment(Terrain(100, "Outside fixture"), Terrain(101, "Silt fixture"), Terrain(102, "Shadow fixture"));
		var terrains = new All<ITerrain>(); foreach (var terrain in new[] { environment.Outside, environment.Silt, environment.Shadow }) terrains.Add(terrain);
		native.WorldMock.SetupGet(x => x.Terrains).Returns(terrains);
		Mock.Get(native.Actor.Location.CurrentOverlay).SetupGet(x => x.Terrain).Returns(() => environment.Current);
		return environment;
	}

	private static void SeedEtherealComponents(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var stack = db.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos).Single(x => x.Name == "ARM03B2B stack");
		long id = db.GameItemProtos.Max(x => x.Id); MudSharp.Models.Tag? parent = null;
		for (var rank = 0; rank <= 5; ++rank)
		{
			var tag = new MudSharp.Models.Tag { Name = $"Ethereal Divination {rank}", ParentId = parent?.Id };
			db.Tags.Add(tag); db.SaveChanges(); parent = tag;
			if (rank is not (0 or 5)) continue;
			var prototype = new MudSharp.Models.GameItemProto { Id = ++id, Name = $"ARM03B2B Ethereal token {rank}", Keywords = "divination token",
				ShortDescription = "a Divination token", FullDescription = "Declared native rank fixture; source consumption adapts to one quantity unit.",
				MaterialId = stack.MaterialId, Size = 1, Weight = 1, BaseItemQuality = (int)ItemQuality.Standard,
				EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)MudSharp.Framework.Revision.RevisionStatus.Current } };
			foreach (var component in stack.GameItemProtosGameItemComponentProtos) prototype.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.GameItemComponentProtoId });
			prototype.GameItemProtosTags.Add(new() { TagId = tag.Id }); db.GameItemProtos.Add(prototype); db.SaveChanges();
		}
	}

	private static int RunEtherealComponentAdapter()
	{
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.CreateFresh("futuremud_land_");
		ConfigureNativeDatabase(database.ConnectionString); Console.WriteLine("ETHEREAL-COMPONENT-database=" + database.Name);
		var fixture = FixtureSeed.Create(database, "ethereal_component_adapter_lane", true);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true); ConfigureCastingWorld(seed, database.ConnectionString, true);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id); SeedCreatedWeaponPrototypes(database, seed.World.Materials.First().Id);
		SeedConsumables(database, fixture, seed.World.Materials.First().Id);
		SeedEtherealComponents(database);
		var host = PrepareRetirementHost(database, fixture, clock, wielding: true, consumablesAnatomy: true);
		var native = host.Native; var actor = native.Actor; var world = native.World;
		var environment = ConfigureEtherealComponentWorld(native, database);
		var ranks = Enumerable.Range(0, 5).Select(x => world.Tags.GetByName($"Ethereal Divination {x}")!).ToArray();
		var scheduler = new EffectScheduler(world, clock); native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		var expressions = (All<ITraitExpression>)world.TraitExpressions; var spells = (All<IMagicSpell>)world.MagicSpells;
		var progs = (All<IFutureProg>)world.FutureProgs;
		native.WorldMock.Setup(x => x.Add(It.IsAny<IMagicSpell>())).Callback<IMagicSpell>(x => spells.Add(x));
		native.WorldMock.SetupGet(x => x.AlwaysFalseProg).Returns(progs.GetByName("AlwaysFalse")!);
		PierceRealResource(native, database.ConnectionString);
		var attribute = world.Traits.GetByName("ARM02 Agility")!;
		Require(actor.AddTrait(attribute, 19), "Ethereal native capacity attribute");
		MudSharp.Models.TraitExpression capacity; MudSharp.Models.TraitExpression duration; MudSharp.Models.TraitExpression cost;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			capacity = new() { Name = "Ethereal adapter capacity", Expression = "80+2*variable" };
			duration = new() { Name = "Ethereal adapter duration", Expression = "1800*grade" };
			cost = new() { Name = "Ethereal adapter minimum", Expression = "7" };
			db.TraitExpressions.AddRange(capacity, duration, cost); db.SaveChanges();
		}
		foreach (var model in new[] { capacity, duration, cost }) expressions.Add(new TraitExpression(model, world));
		Require(native.Resource.BuildingCommand(actor, new StringStack($"capattribute {attribute.Id} {capacity.Id} raw")) &&
			native.Resource.ResourceCap(actor) == 118, "Actual editable resource capacity");
		var cap = (SkillLevelBasedMagicCapability)native.Capability; var skill = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		EditableItemHelper.MagicSpellHelper.EditableNewAction(actor, new StringStack($"\"Ethereal Component Adapter Fixture\" {cap.School.Id}"));
		var spell = (MagicSpell)spells.Single(x => x.Name == "Ethereal Component Adapter Fixture");
		foreach (var edit in new[] { "trigger new self", "effect add detectethereal", $"effect 1 lifetime accumulate {EtherealComponentAdapterGroup} 600 36",
			$"trait {skill.Id}", "difficulty easy", $"duration {duration.Id}", $"cost {native.Resource.Id} {cost.Id}",
			"castemote $0 invokes an ethereal perception enchantment.", "failcastemote $0 falters.", "targetemote $0 surrounds $1 with an enchantment.",
			"grades fixture", "grades efficiency source 7 1" })
			Require(spell.BuildingCommand(actor, new StringStack(edit)), "Ordinary ethereal adapter builder: " + edit);
		foreach (var edit in new[] { $"plan add consume {ranks[0].Id} 1", "plan carried 1 on",
			$"plan ranks 1 -3 {string.Join(' ', ranks.Select(x => x.Id))}", "effect 1 source component 1 see.divination",
			"effect 1 source 101 102 see.divination" })
			Require(spell.BuildingCommand(actor, new StringStack(edit)), "Ordinary source component builder: " + edit);
		if (!spell.AppliedEffectsAreExclusive)
			Require(spell.BuildingCommand(actor, new StringStack("exclusiveeffect")), "Enable exclusive replacement when absent");
		Require(spell.ReadyForGame && spell.StockIdentity is null, "Editable adapter readiness; this is not source stock acceptance");
		using (new FMDB()) { spell.Save(); FMDB.Context.SaveChanges(); }
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var reload = new MagicSpell(db.MagicSpells.AsNoTracking().Single(x => x.Id == spell.Id), world);
			Require(reload.ReadyForGame && XNode.DeepEquals(spell.SpellEffects.Single().SaveToXml(), reload.SpellEffects.Single().SaveToXml()),
				"Editable ethereal template database reload");
			spells.Remove(spell); spells.Add(reload); spell = reload;
		}
		foreach (var edit in new[] { $"casting trait {skill.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive",
			$"casting entry add {spell.Id}", $"casting entry skill {spell.Id} 30 90 relative", $"casting entry starting {spell.Id} on", "casting enable on" })
			Require(cap.BuildingCommand(actor, new StringStack(edit)), "Ethereal adapter capability: " + edit);
		actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>(null, true); actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]); actor.SetTraitValue(skill, 90);
		var staff = Mock.Of<ICharacter>(x => x.Id == 999 && x.IsAdministrator(PermissionLevel.JuniorAdmin));
		Action<string>? callback = null;
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1,
			checkpoint: stage => callback?.Invoke(stage), flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		Require(casting.Enrol(staff, actor, cap.Id, "Ethereal adapter native fixture").Allowed, "Adapter enrolment");
		var state = new MagicCastingStateStore(); state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 7 });
		GameItem NewComponent(int rank, int quantity)
		{
			var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == $"ARM03B2B Ethereal token {rank}").CreateNew(actor);
			item.GetItemType<IStackable>()!.Quantity = quantity; world.Add(item); actor.Location.Insert(item, true); item.Login();
			native.Body.Get(item, silent: true); Require(ReferenceEquals(item.InInventoryOf, native.Body), "Native component was not carried");
			world.SaveManager.Flush(); return item;
		}
		actor.AddResource(native.Resource, 118); FlushCasting(native);
		var low = NewComponent(0, 3);
		var rejectedBalance = actor.MagicResourceAmounts[native.Resource];
		var rejected = casting.Cast(new(actor, cap.Id, spell.Id, 7, false, "me"));
		Require(rejected.Status == MagicCastingStatus.Refused && rejected.OperationId is null && low.GetItemType<IStackable>()!.Quantity == 3 &&
			actor.MagicResourceAmounts[native.Resource] == rejectedBalance, "Low-rank prepayment conservation");
		native.Body.Drop(low, silent: true);
		var component = NewComponent(5, 40); var stack = component.GetItemType<IStackable>()!;
		void Ready() { actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(native.Resource, 118); FlushCasting(native); }
		MagicSpellParent Parent() => actor.EffectsOfType<MagicSpellParent>().Single(x => x.LifetimeGroup == EtherealComponentAdapterGroup);
		MagicCastingResult Cast(int grade, bool overreach = false, bool applied = true)
		{
			Ready(); var intent = new MagicCastingIntent(actor, cap.Id, spell.Id, grade, overreach, "me");
			var quote = casting.Quote(intent); Require(quote.Allowed, quote.Reason); var before = actor.MagicResourceAmounts[native.Resource];
			var quantity = stack.Quantity; var result = casting.Cast(intent);
			Require(stack.Quantity == quantity - (ReferenceEquals(environment.Current, environment.Shadow) ? 0 : 1), "Observed native quantity conservation");
			Require(result.Status == MagicCastingStatus.Succeeded, "Actual paid ethereal adapter cast: " + result.Message);
			Require(before - actor.MagicResourceAmounts[native.Resource] == quote.Invocation!.Costs.Single().Amount &&
				(overreach || before - actor.MagicResourceAmounts[native.Resource] == Math.Max(7, 50 / (8 - grade))), "Quote/debit mismatch");
			Require((bool)XElement.Parse(state.Operation(result.OperationId!.Value)!.Definition).Attribute("applied")! == applied,
				"Ethereal native application receipt");
			VerifyNativeEtherealComponentChannels(native, true);
			Console.WriteLine($"ETHEREAL-COMPONENT-paid=passed grade:{grade} cost:{before - actor.MagicResourceAmounts[native.Resource]} applied:{applied}");
			return result;
		}
		Cast(1); Require(scheduler.ScheduledExpiry(Parent()) == RuntimeClock.UtcNow.AddSeconds(1800), "Grade1 three-hour increment");
		var old = Parent(); Cast(7);
		Require(Parent().LifetimeState!.Grade == 7 && scheduler.ScheduledExpiry(Parent()) == RuntimeClock.UtcNow.AddSeconds(14400) &&
			!actor.Effects.Contains(old) && !old.SpellEffects.Any() && !scheduler.IsScheduled(old), "Native high-grade accumulation/cleanup");
		Cast(7); Require(scheduler.ScheduledExpiry(Parent()) == RuntimeClock.UtcNow.AddSeconds(21600), "Native cap36");
		for (var grade = 1; grade <= 7; ++grade) Cast(grade, applied: false);
		environment.Current = environment.Shadow; var shadowQuantity = stack.Quantity; Cast(1, applied: false);
		Require(stack.Quantity == shadowQuantity, "Shadow consumed the dedicated component");
		environment.Current = environment.Silt; Ready(); var siltBalance = actor.MagicResourceAmounts[native.Resource];
		var silt = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, "me"));
		Require(silt.Status == MagicCastingStatus.Refused && silt.OperationId is null && actor.MagicResourceAmounts[native.Resource] == siltBalance &&
			stack.Quantity == shadowQuantity, "Silt prepayment refusal"); environment.Current = environment.Outside;

		state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 1, NextMasteryUtc = DateTime.UnixEpoch });
		var unchanged = Cast(2, true, false);
		Require(casting.Acquisition(actor, spell.Id)!.ControlledGrade == 1 &&
			XElement.Parse(state.Operation(unchanged.OperationId!.Value)!.Definition).Attribute("masterySample") is null,
			"No-change paid overreach sampled mastery");
		state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 7 });
		Ready(); old = Parent(); var balance = actor.MagicResourceAmounts[native.Resource];
		callback = stage => { if (stage == "BeforePayment") scheduler.Reschedule(old, TimeSpan.FromSeconds(6000)); };
		var refused = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, "me")); callback = null;
		Require(refused.Status == MagicCastingStatus.Refused && refused.OperationId is null &&
			actor.MagicResourceAmounts[native.Resource] == balance && actor.Effects.Contains(old), "Prepayment cohort drift charged or replaced");
		Ready(); balance = actor.MagicResourceAmounts[native.Resource];
		callback = stage => { if (stage == "Committed") scheduler.Reschedule(old, TimeSpan.FromSeconds(6600)); };
		var uncertain = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, "me")); callback = null;
		Require(uncertain.Status == MagicCastingStatus.NeedsReview && balance - actor.MagicResourceAmounts[native.Resource] == 7 &&
			actor.Effects.Contains(old) && Parent() == old, "Paid cohort uncertainty guessed replacement or refunded");
		var paidBalance = actor.MagicResourceAmounts[native.Resource];
		var retry = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, "me", OriginId: uncertain.OperationId));
		Require(retry.Status == MagicCastingStatus.Refused && actor.MagicResourceAmounts[native.Resource] == paidBalance &&
			Parent() == old && XElement.Parse(state.Operation(uncertain.OperationId!.Value)!.Definition).Attribute("masterySample") is null,
			"Paid uncertainty replayed, refunded, replaced the cohort or sampled mastery");
		// Staff reconciles this inspected disposable fixture explicitly so later independent
		// acceptance can continue. The engine has not automatically resolved uncertainty.
		Require(casting.ReconcileOperation(staff, actor, uncertain.OperationId!.Value,
			"Owned disposable fixture: retained old ethereal cohort and paid reserve inspected").Allowed,
			"Explicit disposable-fixture reconciliation");
		Require(actor.MagicResourceAmounts[native.Resource] == paidBalance, "Fixture reconciliation refunded payment");
		foreach (var startShadow in new[] { false, true })
		{
			environment.Current = startShadow ? environment.Shadow : environment.Outside;
			Ready(); old = Parent(); var quantity = stack.Quantity; balance = actor.MagicResourceAmounts[native.Resource];
			callback = stage => { if (stage == "PaymentMutated") environment.Current = startShadow ? environment.Outside : environment.Shadow; };
			var drift = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, "me")); callback = null;
			Require(drift.Status == MagicCastingStatus.NeedsReview && actor.MagicResourceAmounts[native.Resource] == balance - 7 &&
				stack.Quantity == quantity && Parent() == old && XElement.Parse(state.Operation(drift.OperationId!.Value)!.Definition).Attribute("masterySample") is null,
				"Post-debit environment drift guessed consumption or perception");
			FlushCasting(native); world.SaveManager.Flush();
			RunItemReaderProcess(new EtherealComponentAdapterReader(database.Name, fixture, RuntimeClock.UtcNow, spell.Id,
				scheduler.ScheduledExpiry(old)!.Value, old.Identity, drift.OperationId.Value, actor.MagicResourceAmounts[native.Resource], component.Id, stack.Quantity, false),
				"--ethereal-component-adapter-reader");
			Require(casting.ReconcileOperation(staff, actor, drift.OperationId.Value, "Owned fixture retained old cohort, exact quantity and paid reserve inspected").Allowed,
				"Explicit environment drift fixture reconciliation");
		}
		environment.Current = environment.Outside;
		Ready(); native.Body.Drop(component, silent: true); var whole = NewComponent(5, 1); var wholeId = whole.Id;
		var wholeResult = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, "me"));
		Require(wholeResult.Status == MagicCastingStatus.Succeeded && whole.Deleted && world.Items.Get(wholeId) is null &&
			!native.Body.HeldOrWieldedItems.Any(x => x.Id == wholeId), "Native whole-item disappearance did not prove consumption: " + wholeResult.Message);
		world.SaveManager.Flush();
		using (var db = NewIndependentContext(database.ConnectionString)) Require(!db.GameItems.Any(x => x.Id == wholeId), "Consumed item database row survived");
		Console.WriteLine("ETHEREAL-COMPONENT-whole-item=passed native-quantity-one Delete catalogue-body-database-disappearance");
		native.Body.Get(component, silent: true);
		var final = Cast(7); var saved = Parent(); FlushCasting(native);
		RunItemReaderProcess(new EtherealComponentAdapterReader(database.Name, fixture, RuntimeClock.UtcNow, spell.Id,
			scheduler.ScheduledExpiry(saved)!.Value, saved.Identity, final.OperationId!.Value, actor.MagicResourceAmounts[native.Resource], component.Id, stack.Quantity, true),
			"--ethereal-component-adapter-reader");
		Console.WriteLine("ETHEREAL-COMPONENT-adapter=passed ordinary-builder reload paid-grade1-grade7 min7 cumulative-cap36 strongest-grade nochange-no-mastery cohort-drift fresh-reader expiry; component-adapter-only-source-stock-factory-prerequisite-and-practice-unqualified");
		return 0;
	}

	private static int ReadEtherealComponentAdapter(string encoded)
	{
		var input = JsonSerializer.Deserialize<EtherealComponentAdapterReader>(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.OpenExistingOwned(input.Database);
		ConfigureNativeDatabase(database.ConnectionString); var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime);
		using var time = RuntimeClock.Push(clock); var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, consumablesAnatomy: true);
		var native = host.Native; var actor = native.Actor;
		ConfigureEtherealComponentWorld(native, database);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var spells = (All<IMagicSpell>)native.World.MagicSpells;
			foreach (var existing in spells.ToArray()) spells.Remove(existing);
			foreach (var model in db.MagicSpells.AsNoTracking()) spells.Add(new MagicSpell(model, native.World));
		}
		var component = native.World.TryGetItem(input.Component, true)!;
		Require(component.GetItemType<IStackable>()!.Quantity == input.Quantity, "Fresh process component quantity changed");
		var source = (MagicSpell)native.World.MagicSpells.Get(input.Spell)!;
		Require(source.SpellEffects.Single().SaveToXml().Element("SourceScope") is { } scope && (long)scope.Attribute("silt")! == 101 &&
			(long)scope.Attribute("shadow")! == 102 && ((IMagicSpellEffectLifetimePolicy)source.SpellEffects.Single()).LifetimePolicyError is null,
			"Fresh process source scope configuration did not reload");
		var scheduler = new EffectScheduler(native.World, clock);
		native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(scheduler); PierceRealResource(native, database.ConnectionString);
		SpellDetectEtherealEffect.InitialiseEffectType();
		using (var db = NewIndependentContext(database.ConnectionString)) actor.RestoreCastingEffects(db.Characters.AsNoTracking().Single(x => x.Id == actor.Id).EffectData);
		typeof(PerceivedItem).GetMethod("ScheduleCachedEffects", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(actor, null);
		var parent = actor.EffectsOfType<MagicSpellParent>().Single(x => x.Spell.Id == input.Spell);
		Require(parent.Identity == input.ParentIdentity && parent.LifetimeState == new MagicSpellLifetimeState(new(EtherealComponentAdapterGroup, 600, 36), 7) &&
			scheduler.ScheduledExpiry(parent) == input.Expiry && parent.SpellEffects.Single() is SpellDetectEtherealEffect &&
			actor.Effects.Contains(parent.SpellEffects.Single()), "Fresh ethereal parent/child/policy/grade/identity/deadline");
		VerifyNativeEtherealComponentChannels(native, true);
		var operation = new MagicCastingStateStore().Operation(input.Operation)!;
		Require(input.Expire ? operation.Stage == "Completed" && XElement.Parse(operation.Definition).Attribute("applied") is not null :
			operation.Stage == "NeedsReview" && XElement.Parse(operation.Definition).Attribute("masterySample") is null, "Persisted completion/quarantine report");
		var casting = new MagicCastingService(native.World, clock: () => RuntimeClock.UtcNow,
			random: () => throw new InvalidOperationException("No reader reroll"), flush: () => FlushCasting(native));
		var operationReceipt = XElement.Parse(operation.Definition);
		var retry = casting.Cast(new(actor, operation.CapabilityId, input.Spell, (int)operationReceipt.Attribute("grade")!, false,
			operationReceipt.Element("Targets")!.Value, OriginId: input.Operation));
		Require(retry.Status == MagicCastingStatus.Refused && actor.MagicResourceAmounts[native.Resource] == input.Balance &&
			scheduler.ScheduledExpiry(parent) == input.Expiry, "Fresh retry replayed or refunded");
		Require(component.GetItemType<IStackable>()!.Quantity == input.Quantity, "Fresh retry consumed material again");
		if (!input.Expire)
		{
			Console.WriteLine("ETHEREAL-COMPONENT-reader-quarantine=passed fresh-process exact-quantity paid-reserve old-cohort no-replay no-consume no-refund no-mastery"); return 0;
		}
		clock.Advance(input.Expiry - RuntimeClock.UtcNow + TimeSpan.FromSeconds(1)); scheduler.CheckSchedules(); FlushCasting(native);
		Require(!actor.EffectsOfType<MagicSpellParent>().Any(x => x.Spell.Id == input.Spell) && !actor.EffectsOfType<SpellDetectEtherealEffect>().Any() &&
			actor.MagicResourceAmounts[native.Resource] == input.Balance, "Expiry orphaned ethereal parent/child or refunded");
		VerifyNativeEtherealComponentChannels(native, false);
		Console.WriteLine("ETHEREAL-COMPONENT-reader=passed fresh-process native-typed-child identity grade7 policy exact-deadline no-replay no-refund expiry-removes-parent-child-perception");
		return 0;
	}
}
