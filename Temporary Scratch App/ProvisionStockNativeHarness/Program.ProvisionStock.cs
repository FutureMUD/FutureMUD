#nullable enable
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;
namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed class ProvisionCreationFault(ISpellOwnedItemService inner) : ISpellOwnedItemService
	{
		internal int Attempts;
		public IGameItem Create(IGameItemProto prototype, ICharacter caster, ItemQuality quality, SpellLifecycleOrigin origin)
		{ if (++Attempts == 2) throw new InvalidOperationException("Provision fixture second-output creation fault"); return inner.Create(prototype, caster, quality, origin); }
		public SpellOwnedItemOrigin? FindOrigin(long id) => inner.FindOrigin(id);
		public bool TryPrepareRemoval(IGameItem item, out string diagnostic) => inner.TryPrepareRemoval(item, out diagnostic);
		public void ObserveRemoval(IGameItem item) => inner.ObserveRemoval(item);
		public int ReconcileRetirements(DateTime now, int limit = 100) => inner.ReconcileRetirements(now, limit);
	}
	private sealed class ProvisionNoDrawRandom : Random
	{
		public override int Next(int maxValue) => throw new InvalidOperationException("Prepared selection must not redraw during fresh-copy admission");
	}
	internal static int ProvisionStockMain(string[] args)
	{
		if (args.FirstOrDefault() is not ("--provision-stock-run" or "--provision-stock-reader")) return FiveStockMain(args);
		try { OwnedConnections.Install(); DelayedNeedsFulfillment.InitialiseEffectType(); return args[0] == "--provision-stock-run" ? RunProvisionStockChecks() : ReadProvisionStock(args[1]); }
		catch (Exception error) { Console.Error.WriteLine(error); return 1; }
	}
	private sealed record ProvisionReader(string Database, FixtureIds Fixture, DateTime Now, long[] Spells, long[] Food,
		long Consumed, long Partial, Guid[] Origins, long Vessel, double Volume, double FoodHours, double Alcohol, double PendingAlcohol,
		Guid[] Operations, long Foreign, DateTime Deadline);
	private static void SeedProvisionProfiles(TestDatabase database, long material)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var parts = db.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos).Single(x => x.Name == "ARM03B2B C2 meal").GameItemProtosGameItemComponentProtos.ToArray();
		var id = db.GameItemProtos.Max(x => x.Id);
		for (var i = 2; i <= 6; ++i) {
			var proto = new Db.GameItemProto { Id = ++id, Name = "ARM03B2B C2 meal" + i, Keywords = "meal", ShortDescription = "an authored meal " + i,
				FullDescription = "Provision fixture; unavailable historical nutrition and object profiles are not asserted.", MaterialId = material, Size = 1, Weight = 0.1,
				BaseItemQuality = (int)ItemQuality.Standard, EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			foreach (var part in parts) proto.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = part.GameItemComponentProtoId });
			db.GameItemProtos.Add(proto); db.SaveChanges();
		}
		var wine = db.Liquids.Single(x => x.Name == "ARM03C2 wine");
		var variant = (Db.Liquid)db.Entry(wine).CurrentValues.ToObject(); variant.Id = 0; variant.Name = "Provision authored wine variant";
		db.Liquids.Add(variant); db.SaveChanges();
	}
	private static int RunProvisionStockChecks()
	{
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		var fixture = FixtureSeed.Create(database, "provision_stock_lane", true); var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true); ConfigureCastingWorld(seed, database.ConnectionString, true);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id); SeedCreatedWeaponPrototypes(database, seed.World.Materials.First().Id);
		SeedConsumables(database, fixture, seed.World.Materials.First().Id); SeedProvisionProfiles(database, seed.World.Materials.First().Id);
		var host = PrepareRetirementHost(database, fixture, clock, wielding: true, consumablesAnatomy: true); var native = host.Native; var actor = native.Actor; var world = native.World;
		var owned = new SpellOwnedItemService(world); native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(owned);
		native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(new EffectScheduler(world, clock));
		var expressions = (All<ITraitExpression>)world.TraitExpressions; var spells = (All<IMagicSpell>)world.MagicSpells; var progs = (All<IFutureProg>)world.FutureProgs;
		native.WorldMock.Setup(x => x.Add(It.IsAny<ITraitExpression>())).Callback<ITraitExpression>(value => expressions.Add(value));
		native.WorldMock.Setup(x => x.Add(It.IsAny<IMagicSpell>())).Callback<IMagicSpell>(value => spells.Add(value));
		native.WorldMock.Setup(x => x.Add(It.IsAny<IFutureProg>())).Callback<IFutureProg>(value => progs.Add(value));
		native.WorldMock.SetupGet(x => x.AlwaysFalseProg).Returns(progs.GetByName("AlwaysFalse")!);
		var cap = (SkillLevelBasedMagicCapability)native.Capability; var trait = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		var wines = world.Liquids.GetByName("ARM03C2 wine")!; var variant = world.Liquids.GetByName("Provision authored wine variant")!;
		var pool = host.Prototypes.Values.Where(x => x.Name is "ARM03B2B C2 meal" or "ARM03B2B C2 meal2" or "ARM03B2B C2 meal3").OrderBy(x => x.Id).ToArray();
		var siltPool = host.Prototypes.Values.Where(x => x.Name is "ARM03B2B C2 meal4" or "ARM03B2B C2 meal5" or "ARM03B2B C2 meal6").OrderBy(x => x.Id).ToArray();
		var messages = new List<string>(); var output = new Mock<MudSharp.PerceptionEngine.IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback<string, bool, bool>((text, _, _) => messages.Add(text)).Returns(true);
		MagicSpell Build(string token, string name, string extra) {
			var command = $"stock {token} {cap.School.Id} {trait.Id} {native.Resource.Id} {extra}"; var old = actor.OutputHandler; SetPrivateMember(actor, "OutputHandler", output.Object);
			try { EditableItemHelper.MagicSpellHelper.EditableNewAction(actor, new StringStack(command)); var count = spells.Count; EditableItemHelper.MagicSpellHelper.EditableNewAction(actor, new StringStack(command)); Require(count == spells.Count, "Duplicate provision stock changed rows"); }
			finally { SetPrivateMember(actor, "OutputHandler", old); }
			Require(spells.Any(x => x.Name == name), "Ordinary provision builder failed: " + string.Join(';', messages)); var result = (MagicSpell)spells.GetByName(name);
			Require(result.ReadyForGame, "Provision stock not ready: " + (result.ReadyForGame ? "" : result.WhyNotReadyForGame(actor))); return result;
		}
		var food = Build("sustain-meal", ArmageddonSustainMealStock.Name, string.Join(' ', pool.Select(x => x.Id)));
		var wine = Build("draw-wine", ArmageddonDrawWineStock.Name, $"{wines.Id} none");
		IFutureProg Predicate(string name, string terrain) {
			using var db = NewIndependentContext(database.ConnectionString);
			var model = new Db.FutureProg { FunctionName = name, FunctionText = $"return lowercase(@caster.location.terrain.name) == \"{terrain}\"", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(),
				FunctionComment = "Authored fixture mapping, not historical guild/tribe IDs.", Category = "Harness", Subcategory = "Provision Stock" };
			model.FutureProgsParameters.Add(new() { ParameterName = "caster", ParameterIndex = 0, ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() }); db.FutureProgs.Add(model); db.SaveChanges();
			var result = new FutureProg(model, world); Require(result.Compile(), result.CompileError); progs.Add(result); return result;
		}
		var silt = Predicate("provisionSiltFood", "silt"); var choice = Predicate("provisionWineRecipe", "recipe fixture");
		Require(food.BuildingCommand(actor, new StringStack($"effect 1 foodprofile 1 {silt.Id} {string.Join(' ', siltPool.Select(x => x.Id))}")), "Native food mapping builder");
		Require(wine.BuildingCommand(actor, new StringStack($"effect 1 recipe 1 {choice.Id} {variant.Id}")), "Native wine recipe builder");
		var all = new[] { food, wine };
		foreach (var spell in all) {
			using (new FMDB()) { spell.Save(); FMDB.Context.SaveChanges(); }
			using var db = NewIndependentContext(database.ConnectionString); var loaded = new MagicSpell(db.MagicSpells.AsNoTracking().Single(x => x.Id == spell.Id), world);
			Require(loaded.ReadyForGame && loaded.StockIdentity == spell.StockIdentity && XNode.DeepEquals(spell.Trigger.SaveToXml(), loaded.Trigger.SaveToXml()), "Provision definition persistence");
			foreach (var command in new[] { $"casting trait {trait.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive", $"casting entry add {spell.Id}", $"casting entry skill {spell.Id} 30 90 relative", $"casting entry starting {spell.Id} on", "casting enable on" }) Require(cap.BuildingCommand(actor, new StringStack(command)), "Provision route refused " + command);
		}
		actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>(null, true); actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]);
		var staff = new Mock<ICharacter>(); staff.SetupGet(x => x.Id).Returns(999); staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		Action<string>? checkpoint = null;
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1, checkpoint: stage => checkpoint?.Invoke(stage), flush: () => FlushCasting(native)); native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		Require(casting.Enrol(staff.Object, actor, cap.Id, "Provision stock native lane").Allowed, "Provision enrolment failed"); actor.SetTraitValue(trait, 90);
		var state = new MagicCastingStateStore(); foreach (var spell in all) state.Write(acquired: casting.Acquisition(actor, spell.Id)! with { ControlledGrade = 7 });
		var operations = new List<Guid>();
		MagicCastingResult Cast(MagicSpell spell, int grade, string target = "") {
			actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(native.Resource, 100); FlushCasting(native);
			var before = actor.MagicResourceAmounts[native.Resource]; var quote = casting.Quote(new(actor, cap.Id, spell.Id, grade, false, target)); Require(quote.Allowed, quote.Reason);
			var result = casting.Cast(new(actor, cap.Id, spell.Id, grade, false, target)); Require(result.Status == MagicCastingStatus.Succeeded, result.Message);
			Require(before - actor.MagicResourceAmounts[native.Resource] == quote.Invocation!.Costs.Single().Amount && (bool)XElement.Parse(state.Operation(result.OperationId!.Value)!.Definition).Attribute("applied")!, "Provision cost/application report");
			operations.Add(result.OperationId.Value); Console.WriteLine($"PROVISION-paid=passed key:{spell.StockIdentity} grade:{grade} cost:{before - actor.MagicResourceAmounts[native.Resource]} applied:true"); return result;
		}
		void Refuse(MagicSpell spell, string target, string reason) {
			actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(native.Resource, 100); FlushCasting(native); var balance = actor.MagicResourceAmounts[native.Resource];
			using var before = NewIndependentContext(database.ConnectionString); var counts = (before.MagicCastingOperations.Count(), before.GameItems.Count(), before.MagicSpellLifecycles.Count());
			var result = casting.Cast(new(actor, cap.Id, spell.Id, 1, false, target)); using var after = NewIndependentContext(database.ConnectionString);
			Require(result.Status == MagicCastingStatus.Refused && result.OperationId is null && balance == actor.MagicResourceAmounts[native.Resource] && counts == (after.MagicCastingOperations.Count(), after.GameItems.Count(), after.MagicSpellLifecycles.Count()), "Provision refusal changed state " + reason + ": " + result.Message);
			Console.WriteLine($"PROVISION-refusal=passed key:{spell.StockIdentity} reason:{reason} before-payment-operation-item-lifecycle-unchanged");
		}
		void Terrain(string name) { var terrain = new Mock<MudSharp.Construction.ITerrain>(); terrain.SetupGet(x => x.Type).Returns(ProgVariableTypes.Terrain); terrain.SetupGet(x => x.GetObject).Returns(terrain.Object); terrain.Setup(x => x.GetProperty("name")).Returns(new MudSharp.FutureProg.Variables.TextVariable(name));
			var cell = Mock.Get(actor.Location); cell.SetupGet(x => x.Type).Returns(ProgVariableTypes.Location); cell.SetupGet(x => x.GetObject).Returns(actor.Location); cell.Setup(x => x.GetProperty("terrain")).Returns(terrain.Object); }
		Terrain("Desert");
		VerifyProvisionSeededFood(food, native, trait, pool.Select(x => x.Id).ToArray());
		var room = actor.Location; SetPrivateMember(actor, "Location", null!);
		try { Refuse(food, "", "missing-physical-room-target"); } finally { SetPrivateMember(actor, "Location", room); FlushCasting(native); }
		Refuse(wine, "missing-target", "missing-item-target");
		checkpoint = stage => { if (stage == "BeforePayment") Terrain("Silt"); };
		try { Refuse(food, "", "first-matching-food-profile-changed-before-payment"); } finally { checkpoint = null; Terrain("Desert"); }
		var low = Cast(food, 1); var lowMeal = host.Items.Single(x => x.SpellCreationOrigin is not null);
		Require(lowMeal.SpellCreationOrigin!.DeadlineUtc == RuntimeClock.UtcNow.AddSeconds(1350) && lowMeal.Location == actor.Location, "Source-ground low food deadline/placement");
		Cast(food, 7); var high = host.Items.Where(x => x.SpellCreationOrigin?.DeadlineUtc == RuntimeClock.UtcNow.AddSeconds(9450)).ToArray(); Require(high.Length == 7 && high.All(x => pool.Any(p => p.Id == x.Prototype.Id)), "Grade-count food independent pool output");
		Terrain("Silt"); Cast(food, 1); var siltMeal = host.Items.Single(x => x.SpellCreationOrigin is not null && siltPool.Any(p => p.Id == x.Prototype.Id)); siltMeal.Delete(); Terrain("Desert");
		var consumed = high[0]; native.Body.Get(consumed, silent: true); Require(native.Body.SilentEat(consumed.GetItemType<IEdible>()!, 0) && consumed.Deleted, "Native complete food consumption");
		var partial = high[1]; native.Body.Get(partial, silent: true); var foodHours = actor.NeedsModel.FoodSatiatedHours; Require(native.Body.Eat(partial.GetItemType<IEdible>()!, null, null, 1.25, null) && partial.GetItemType<IEdible>()!.BitesRemaining == 2.75 && actor.NeedsModel.FoodSatiatedHours == foodHours + 0.625, "Partial food nutrition conservation");
		native.Body.Drop(partial, silent: true); Require(partial.SpellCreationOrigin!.DeadlineUtc == RuntimeClock.UtcNow.AddSeconds(9450), "Food custody reset deadline");
		var oldText = silt.FunctionText; silt.FunctionText = "not valid futureprog"; Require(!silt.Compile(), "Broken food predicate fixture"); Refuse(food, "", "invalid-configured-profile"); silt.FunctionText = oldText; Require(silt.Compile(), "Restore food predicate");
		var itemCatalogue = world.ItemProtos; var incompleteFoods = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		incompleteFoods.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => id == siltPool[2].Id ? null! : itemCatalogue.Get(id));
		native.WorldMock.SetupGet(x => x.ItemProtos).Returns(incompleteFoods.Object);
		try { Refuse(food, "", "missing-unselected-food-pool-member"); } finally { native.WorldMock.SetupGet(x => x.ItemProtos).Returns(itemCatalogue); }
		GameItem New(string name) { var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == "ARM03B2B C2 " + name).CreateNew(actor); world.Add(item); actor.Location.Insert(item, true); item.Login(); world.SaveManager.Flush(); return item; }
		var vessel = New("vessel"); var container = vessel.GetItemType<ILiquidContainer>()!; Refuse(wine, "vessel", "closed-owning-container"); container.Open();
		var liquidCatalogue = world.Liquids; var incompleteRecipes = new All<ILiquid>();
		foreach (var liquid in liquidCatalogue.Where(x => x.Id != variant.Id)) incompleteRecipes.Add(liquid);
		native.WorldMock.SetupGet(x => x.Liquids).Returns(incompleteRecipes);
		try { Refuse(wine, "vessel", "missing-unselected-recipe-liquid"); } finally { native.WorldMock.SetupGet(x => x.Liquids).Returns(liquidCatalogue); }
		Terrain("Silt"); Refuse(wine, "vessel", "safe-Silt-refusal"); Terrain("Fire Plane"); Refuse(wine, "vessel", "source-Fire-Plane-refusal"); Terrain("Desert");
		checkpoint = stage => { if (stage == "BeforePayment") Terrain("recipe fixture"); };
		try { Refuse(wine, "vessel", "first-matching-wine-recipe-changed-before-payment"); } finally { checkpoint = null; Terrain("Desert"); }
		Cast(wine, 1, "vessel"); Require(container.LiquidVolume == 500, "Wine low five-unit amount"); Cast(wine, 7, "vessel"); Require(container.LiquidVolume == 4000, "Wine high five-unit amount");
		Require(wine.BuildingCommand(actor, new StringStack($"effect 1 bonusplane {world.DefaultPlane.Id} 2")), "Wine bonus edit"); Cast(wine, 7, "vessel"); Require(container.LiquidVolume == 5000, "Wine bonus clamp"); Refuse(wine, "vessel", "full-container");
		var alcohol = actor.NeedsModel.AlcoholLitres; Require(native.Body.SilentDrink(container, 250) && container.LiquidVolume == 4750 && actor.NeedsModel.AlcoholLitres > alcohol, "Native wine alcohol/volume conservation");
		container.RemoveLiquidAmount(container.LiquidVolume, actor, "fixture"); container.MergeLiquid(new LiquidMixture(world.Liquids.GetByName("ARM03C2 oil")!, 250, world), actor, "fixture"); Refuse(wine, "vessel", "incompatible-liquid-preserved"); Require(container.LiquidVolume == 250, "Wine mutated incompatible liquid");
		container.RemoveLiquidAmount(container.LiquidVolume, actor, "fixture"); Terrain("recipe fixture"); Cast(wine, 1, "vessel"); Require(container.LiquidVolume == 1000 && container.LiquidMixture!.Instances.All(x => x.Liquid.Id == variant.Id), "Conditional recipe precedence/bonus"); Terrain("Desert"); Refuse(wine, "vessel", "different-selected-recipe-refuses-mixing");
		clock.Advance(TimeSpan.FromSeconds(1349)); owned.ReconcileRetirements(RuntimeClock.UtcNow); Require(!lowMeal.Deleted && high.Skip(1).All(x => !x.Deleted), "Early food expiry");
		clock.Advance(TimeSpan.FromSeconds(1)); owned.ReconcileRetirements(RuntimeClock.UtcNow); Require(lowMeal.Deleted && high.Skip(1).All(x => !x.Deleted), "Exact low food expiry");
		actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(native.Resource, 100); FlushCasting(native);
		var partialBefore = actor.MagicResourceAmounts[native.Resource]; var existing = host.Items.Select(x => x.Id).ToHashSet();
		var faulty = new ProvisionCreationFault(owned); native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(faulty);
		var partialOrigin = Guid.NewGuid(); MagicCastingResult uncertain;
		try { uncertain = casting.Cast(new(actor, cap.Id, food.Id, 7, false, "", OriginId: partialOrigin)); }
		finally { native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(owned); }
		var partialOutputs = host.Items.Where(x => !existing.Contains(x.Id)).ToArray();
		Require(uncertain.Status == MagicCastingStatus.NeedsReview && faulty.Attempts == 2 && partialOutputs.Length == 1 &&
			actor.MagicResourceAmounts[native.Resource] < partialBefore && state.Operation(partialOrigin)!.Stage == "NeedsReview", "Partial provision creation lost paid uncertainty");
		var afterPartial = actor.MagicResourceAmounts[native.Resource];
		Require(casting.Cast(new(actor, cap.Id, food.Id, 7, false, "", OriginId: partialOrigin)).Status == MagicCastingStatus.Refused &&
			faulty.Attempts == 2 && actor.MagicResourceAmounts[native.Resource] == afterPartial, "Partial creation replayed or refunded");
		Require(casting.ReconcileOperation(staff.Object, actor, partialOrigin, "Audited exact one created food output; no replay or refund").Allowed, "Partial acknowledgement");
		partialOutputs[0].Delete(); Require(partialOutputs[0].Deleted && actor.MagicResourceAmounts[native.Resource] == afterPartial, "Partial output exact removal/refund");
		Console.WriteLine("PROVISION-partial=passed paid-native-first-output second-create-fault durable-NeedsReview origin-retry-no-replay-no-refund staff-acknowledgement exact-owned-removal");
		var foreign = New("gear"); FlushCasting(native);
		using (var db = NewIndependentContext(database.ConnectionString)) {
			foreach (var item in host.Items.Where(x => !x.Deleted && x.InInventoryOf is null && x.ContainedIn is null)) if (!db.CellsGameItems.Any(x => x.GameItemId == item.Id)) db.CellsGameItems.Add(new() { CellId = fixture.CellId, GameItemId = item.Id }); db.SaveChanges();
		}
		var input = new ProvisionReader(database.Name, fixture, RuntimeClock.UtcNow, all.Select(x => x.Id).ToArray(), high.Select(x => x.Id).ToArray(), consumed.Id, partial.Id,
			high.Select(x => x.SpellCreationOrigin!.LifecycleId).ToArray(), vessel.Id, container.LiquidVolume, actor.NeedsModel.FoodSatiatedHours, actor.NeedsModel.AlcoholLitres,
			native.Body.EffectsOfType<DelayedNeedsFulfillment>().Sum(x => x.Payload.AlcoholLitres), operations.ToArray(), foreign.Id, high[1].SpellCreationOrigin!.DeadlineUtc!.Value);
		RunItemReaderProcess(input, "--provision-stock-reader");
		Console.WriteLine("PROVISION-acceptance=passed two-normal-builder-stocks paid-low-high actual-reports conditional-pools-and-recipes native-consumption independent-item-decay fresh-process-conservation"); return 0;
	}
	private static void VerifyProvisionSeededFood(MagicSpell spell, NativeRuntime native, ITraitDefinition trait, long[] pool)
	{
		var seen = new HashSet<long>();
		for (var seed = 0; seed < 10; ++seed) {
			var copy = spell.CastingCopy(native.Actor, trait, 7, SpellPower.ExtremelyStrong, Difficulty.Normal, 7);
			var effect = (CreateItemEffect)copy.SpellEffects.Single(); Require(effect.TryPrepareFoodOutputs(native.Actor, 7, new Random(seed), out var first, out var error), error!);
			Require(effect.TryPrepareFoodOutputs(native.Actor, 7, new Random(seed + 100), out var second, out error) && ReferenceEquals(first, second) && first!.Length == 7, "Food selection rerolled"); foreach (var item in first!) seen.Add(item.Id);
			var token = effect.CapturePreparedSelection(native.Actor, native.Actor.Location)!;
			for (var recheck = 0; recheck < 2; ++recheck) {
				var fresh = (CreateItemEffect)spell.CastingCopy(native.Actor, trait, 7, SpellPower.ExtremelyStrong, Difficulty.Normal, 7).SpellEffects.Single();
				Require(fresh.TryReusePreparedSelection(token, native.Actor, native.Actor.Location, out error) &&
					fresh.TryPrepareFoodOutputs(native.Actor, 7, new ProvisionNoDrawRandom(), out var frozen, out error) &&
					first!.Select(x => x.Id).SequenceEqual(frozen!.Select(x => x.Id)), "Fresh food copy changed sequence or drew random: " + error);
			}
		}
		Require(seen.SetEquals(pool), "Seeded native food choice did not cover every pool option"); Console.WriteLine("PROVISION-food-pool=passed deterministic-seeds all-three-approved-native-options grade-seven-independent-choices two-fresh-copy-live-revalidations exact-sequence throwing-random-not-called no-reroll");
	}
	private static int ReadProvisionStock(string encoded)
	{
		var input = JsonSerializer.Deserialize<ProvisionReader>(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, consumablesAnatomy: true); var native = host.Native; var world = native.World;
		var owned = new SpellOwnedItemService(world); native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(owned);
		using (var db = NewIndependentContext(database.ConnectionString)) {
			foreach (var model in db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().Where(x => x.Subcategory == "Provision Stock" || x.Subcategory == "Draw Wine")) { var prog = new FutureProg(model, world); Require(prog.Compile(), prog.CompileError); if (!world.FutureProgs.Has(prog.Id)) ((All<IFutureProg>)world.FutureProgs).Add(prog); }
			native.Body.LoadInventory(db.Bodies.Include(x => x.BodiesGameItems).Single(x => x.Id == native.Body.Id));
			foreach (var id in db.CellsGameItems.Where(x => x.CellId == input.Fixture.CellId).Select(x => x.GameItemId).ToArray()) { var item = world.TryGetItem(id, true)!; if (item.InInventoryOf is null && item.ContainedIn is null) native.Actor.Location.Insert(item, true); }
			Require(!db.GameItems.Any(x => x.Id == input.Consumed), "Consumed provision food recreated");
		}
		foreach (var item in host.Items.ToArray()) item.FinaliseLoadTimeTasks();
		var partial = world.TryGetItem(input.Partial, true)!; var vessel = world.TryGetItem(input.Vessel, true)!;
		Require(input.Spells.All(id => world.MagicSpells.Get(id).ReadyForGame) && input.Operations.All(id => new MagicCastingStateStore().Operation(id)!.Stage == "Completed"), "Fresh provision definitions/receipts");
		Require(partial.GetItemType<IEdible>()!.BitesRemaining == 2.75 && native.Actor.NeedsModel.FoodSatiatedHours == input.FoodHours && native.Actor.NeedsModel.AlcoholLitres == input.Alcohol && vessel.GetItemType<ILiquidContainer>()!.LiquidVolume == input.Volume, "Fresh consumption state lost or doubled");
		Require(native.Body.EffectsOfType<DelayedNeedsFulfillment>().Sum(x => x.Payload.AlcoholLitres) == input.PendingAlcohol, "Pending consumed alcohol lost during reload");
		foreach (var effect in native.Body.EffectsOfType<DelayedNeedsFulfillment>().ToArray()) effect.ExpireEffect();
		var fulfilledAlcohol = input.Alcohol + input.PendingAlcohol;
		Require(native.Actor.NeedsModel.AlcoholLitres == fulfilledAlcohol && !native.Body.EffectsOfType<DelayedNeedsFulfillment>().Any(), "Delayed alcohol did not fulfil exactly once");
		var balance = native.Actor.MagicResourceAmounts[native.Resource]; var operation = new MagicCastingStateStore().Operation(input.Operations[0])!;
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => throw new Exception("No paid retry reroll"), flush: () => FlushCasting(native)); native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		Require(casting.Cast(new(native.Actor, operation.CapabilityId, input.Spells[0], 1, false, "", OriginId: operation.Id)).Status == MagicCastingStatus.Refused && native.Actor.MagicResourceAmounts[native.Resource] == balance, "Completed provision operation replayed");
		clock.Advance(input.Deadline - RuntimeClock.UtcNow - TimeSpan.FromSeconds(1)); owned.ReconcileRetirements(RuntimeClock.UtcNow); Require(!partial.Deleted, "High food expired early");
		clock.Advance(TimeSpan.FromSeconds(1)); owned.ReconcileRetirements(RuntimeClock.UtcNow); world.SaveManager.Flush(); owned.ReconcileRetirements(RuntimeClock.UtcNow);
		using var verify = NewIndependentContext(database.ConnectionString);
		Require(input.Origins.All(id => host.Store.Find(id)!.State == SpellLifecycleState.Completed) && !verify.GameItems.Any(x => input.Food.Contains(x.Id)) && !verify.GameItemComponents.Any(x => input.Food.Contains(x.GameItemId)) &&
			verify.GameItems.Any(x => x.Id == input.Foreign) && !vessel.Deleted && vessel.GetItemType<ILiquidContainer>()!.LiquidVolume == input.Volume && native.Actor.NeedsModel.FoodSatiatedHours == input.FoodHours && native.Actor.NeedsModel.AlcoholLitres == fulfilledAlcohol, "High expiry lost foreign/liquid/fulfilled needs or recreated consumed food");
		Console.WriteLine("PROVISION-reader=passed fresh-native-process editable-definitions recipes-pools completed-receipts fractional-bites nutrition-alcohol-no-replay exact9450-deadline consumed-output-absent independent-wine-and-foreign-goods-conserved"); return 0;
	}
}
