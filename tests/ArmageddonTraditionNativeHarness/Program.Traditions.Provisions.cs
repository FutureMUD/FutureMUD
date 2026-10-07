#nullable enable
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
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
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record InstalledProvisions(ArmageddonProvisionInstallPlan Plan, ArmageddonTraditionInstallPlan Traditions, Dictionary<string, long> Identities);
	private sealed record ProvisionInstallerReader(string Database, FixtureIds Fixture, DateTime Now, ArmageddonMagicInstallPlan Utilities,
		Dictionary<string, long> UtilityIds, Dictionary<string, long> TraditionIds, InstalledProvisions Installed, long Consumed, long Partial,
		long Expiring, long Light, long Vessel, double Food, double Alcohol, double PendingAlcohol, double Balance, Guid[] Operations);
	private static ArmageddonInstallResult InstallProvisions(TestDatabase database, ArmageddonProvisionInstallPlan plan, Action<ArmageddonInstallCheckpoint>? fault = null)
	{ using var db = NewIndependentContext(database.ConnectionString); return ArmageddonProvisionInstaller.Install(db, plan, fault); }
	private static InstalledProvisions InstallProvisionExtension(TestDatabase database, ArmageddonTraditionInstallPlan traditions, IReadOnlyDictionary<string, long> ids)
	{
		ArmageddonProvisionInstallPlan plan;
		Dictionary<string, long?> previousIdentities;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			previousIdentities = db.SeederManagedRecords.AsNoTracking().ToDictionary(x => x.Module + "|" + x.StableKey, x => x.LogicalId);
			var original = db.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos).Single(x => x.Name == "ARM03B2B C2 meal");
			var foods = new List<ArmageddonFoodPrototype> { new(original.Id, original.RevisionNumber) }; var next = db.GameItemProtos.Max(x => x.Id);
			for (var i = 2; i <= 6; i++)
			{
				var item = (Db.GameItemProto)db.Entry(original).CurrentValues.ToObject(); item.Id = ++next; item.Name = "ARM03B2B installed meal " + i;
				item.EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current };
				foreach (var part in original.GameItemProtosGameItemComponentProtos) item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = part.GameItemComponentProtoId, GameItemComponentRevision = part.GameItemComponentRevision });
				db.GameItemProtos.Add(item); foods.Add(new(item.Id, item.RevisionNumber));
			}
			var wine = db.Liquids.Single(x => x.Name == "ARM03C2 wine"); var recipe = (Db.Liquid)db.Entry(wine).CurrentValues.ToObject(); recipe.Id = 0; recipe.Name = "ARMTRAD authored wine recipe"; db.Liquids.Add(recipe);
			Db.FutureProg Predicate(string name, string terrain)
			{
				var prog = new Db.FutureProg { FunctionName = name, FunctionText = $"return lowercase(@caster.location.terrain.name) == \"{terrain}\"", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(),
					FunctionComment = "Explicit disposable profile; no inferred historical tribe/guild bindings.", Category = "Harness", Subcategory = "Installed Provisions" };
				prog.FutureProgsParameters.Add(new() { ParameterName = "caster", ParameterIndex = 0, ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() }); db.FutureProgs.Add(prog); return prog;
			}
			var foodPolicy = Predicate("installedFoodChoice", "silt"); var winePolicy = Predicate("installedWineChoice", "recipe fixture"); db.SaveChanges();
			plan = new(true, traditions.School, traditions.SourceResource, traditions.AlwaysFalseProg, ids[ArmageddonReviewedProvisionContent.SustainMealKey + ".skill"], ids[ArmageddonReviewedProvisionContent.DrawWineKey + ".skill"],
				[new(1, foodPolicy.Id, foods.Skip(3).ToArray()), new(32, 0, foods.Take(3).ToArray())], wine.Id, [new(1, winePolicy.Id, recipe.Id), new(32, 0, wine.Id)]);
		}
		var players = TraditionPlayers(database);
		var malformed = InstallProvisions(database, plan with { Wine = long.MaxValue }); Require(malformed.Status == ArmageddonInstallStatus.Blocked && malformed.Identities.Count == 0, "Missing wine did not fail closed.");
		foreach (var boundary in new[] { ArmageddonInstallCheckpoint.ContentCreated, ArmageddonInstallCheckpoint.BeforeCommit })
		{
			var failed = InstallProvisions(database, plan, x => { if (x == boundary) throw new IOException("Installed provision interruption"); });
			using var db = NewIndependentContext(database.ConnectionString);
			Require(failed.Status == ArmageddonInstallStatus.Failed && !db.SeederManagedRecords.Any(x => x.Module == ArmageddonProvisionInstaller.Module) && players == TraditionPlayers(database), "Native seven-record rollback failed.");
		}
		var installed = InstallProvisions(database, plan, x => { if (x == ArmageddonInstallCheckpoint.AfterCommit) throw new IOException("Lost provision acknowledgement"); });
		Require(installed.Status == ArmageddonInstallStatus.CommittedConfirmationFailed && installed.Identities.Count == 7, "Committed provision acknowledgement classified incorrectly.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var food = db.MagicSpells.Find(installed.Identities[ArmageddonReviewedProvisionContent.SustainMealKey])!; food.Name = "ARMTRAD builder provision";
			var clone = (Db.MagicSpell)db.Entry(food).CurrentValues.ToObject(); clone.Id = 0; clone.Name = "ARMTRAD unowned provision clone"; db.MagicSpells.Add(clone); db.SaveChanges();
			for (var i = 0; i < 2; i++) RequireInstalled(InstallProvisions(database, plan));
			Require(db.MagicSpells.AsNoTracking().Single(x => x.Id == food.Id).Name == food.Name && !db.SeederManagedRecords.Any(x => x.EntityType == nameof(Db.MagicSpell) && x.LogicalId == clone.Id), "Native builder rename/clone was taken over.");
			var record = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonReviewedProvisionContent.DrawWineKey); record.Retired = true; db.SaveChanges();
			Require(InstallProvisions(database, plan).Status == ArmageddonInstallStatus.Blocked, "Native provision retirement ignored."); record.Retired = false; db.SaveChanges();
			var wine = db.MagicSpells.Find(record.LogicalId)!; db.Remove(wine); db.SaveChanges(); Require(InstallProvisions(database, plan).Status == ArmageddonInstallStatus.Blocked && !db.MagicSpells.AsNoTracking().Any(x => x.Id == wine.Id), "Native missing provision resurrected.");
			db.Entry(wine).State = EntityState.Added; db.SaveChanges();
		}
		var bindings = traditions.ImplementedSpells.ToDictionary(x => x.Key, x => x.Value);
		foreach (var key in new[] { ArmageddonReviewedProvisionContent.SustainMealKey, ArmageddonReviewedProvisionContent.DrawWineKey }) bindings.Add(key, installed.Identities[key]);
		var expanded = traditions with { ImplementedSpells = bindings }; var result = InstallTraditions(database, expanded); RequireTraditions(result);
		Require(result.AvailableSpells.Count == 6 && result.UnavailableSpells.Count == 76 && result.Identities.All(x => ids[x.Key] == x.Value) && players == TraditionPlayers(database), "Expansion altered stable171 IDs/player state or opened an incomplete path.");
		using (var db = NewIndependentContext(database.ConnectionString)) Require(db.SeederManagedRecords.Count() == 199 && db.SeederManagedRecords.AsNoTracking().Where(x => x.Module != ArmageddonProvisionInstaller.Module).AsEnumerable().All(x => previousIdentities[x.Module + "|" + x.StableKey] == x.LogicalId), "Expansion changed previous171/21 ownership identities.");
		Console.WriteLine("ARMTRAD-provisions-install=passed seven-atomic-records retained171-and21 missing-bindings-refused rollback lost-acknowledgement builder-rename-clone deletion-retirement six-closed-admissions 76-unavailable no-player-refresh");
		return new(plan, expanded, installed.Identities.ToDictionary(x => x.Key, x => x.Value));
	}
	private static void VerifyProvisionExtension(RetirementHost host, TestDatabase database, FixtureIds fixture, HarnessClock clock,
		ArmageddonMagicInstallPlan utilities, IReadOnlyDictionary<string, long> utilityIds, IReadOnlyDictionary<string, long> ids, InstalledProvisions installed, bool reproduceCustody)
	{
		DelayedNeedsFulfillment.InitialiseEffectType(); var native = host.Native; var actor = native.Actor; var world = native.World; var service = (MagicCastingService)CastingRequired(world.MagicCasting);
		var cap = (SkillLevelBasedMagicCapability)CastingRequired(world.MagicCapabilities.Get(ids["arm.capability.sorcerer"]));
		var water = CastingRequired(world.Traits.Get(ids[ArmageddonReviewedUtilityContent.DrawWaterKey + ".skill"])); var meal = CastingRequired(world.Traits.Get(installed.Plan.MealSkill)); var wineSkill = CastingRequired(world.Traits.Get(installed.Plan.WineSkill));
		var lightSkill = CastingRequired(world.Traits.Get(ids[ArmageddonReviewedUtilityContent.HoveringLightKey + ".skill"]));
		void Advance(ITraitDefinition trait, Action belowThreshold)
		{
			actor.SetTraitValue(trait, 79.5); service.NotifyProgress(actor, trait.Id);
			belowThreshold();
			Require(((Skill)actor.GetTrait(trait)).TraitUsed(actor, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []) && actor.TraitRawValue(trait) == 80, "Actual source threshold use failed.");
		}
		Require(!actor.HasTrait(meal) && !actor.HasTrait(wineSkill), "Provision child opened before water80."); Advance(water, () => Require(!actor.HasTrait(meal) && !actor.HasTrait(wineSkill), "Provision children opened below water80."));
		foreach (var (key, trait) in new[] { (ArmageddonReviewedProvisionContent.SustainMealKey, meal), (ArmageddonReviewedProvisionContent.DrawWineKey, wineSkill) })
			Require(actor.GetTrait(trait) is Skill && actor.TraitRawValue(trait) == 30 && service.Acquisition(actor, installed.Identities[key])!.ControlledGrade == 1, "Water80 did not open both native child skills30/grade1.");
		Require(!actor.HasTrait(lightSkill), "Light opened below food80."); Advance(meal, () => Require(!actor.HasTrait(lightSkill), "Light opened below food80."));
		Require(actor.GetTrait(lightSkill) is Skill && actor.TraitRawValue(lightSkill) == 30 && service.Acquisition(actor, utilityIds[ArmageddonReviewedUtilityContent.HoveringLightKey])!.ControlledGrade == 1, "Food80 did not open light30/grade1.");
		actor.GetTrait(meal).Value += 100; Require(actor.TraitRawValue(meal) == 90 && !actor.HasTrait(world.Traits.Get(ids[ArmageddonReviewedUtilityContent.MendFleshKey + ".skill"])), "Food cap90 or incomplete Mend path failed.");
		Console.WriteLine("ARMTRAD-provisions-progression=passed actual-native-TraitUsed water79.5-to80 both-children30 food79.5-to80 light30 grade1 raw-cap90 Mend-Pierce-unavailable");
		var owned = new SpellOwnedItemService(world); native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(owned); native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(new EffectScheduler(world, clock));
		void Terrain(string name)
		{
			var terrain = new Mock<MudSharp.Construction.ITerrain>(); terrain.SetupGet(x => x.Type).Returns(ProgVariableTypes.Terrain); terrain.SetupGet(x => x.GetObject).Returns(terrain.Object); terrain.Setup(x => x.GetProperty("name")).Returns(new MudSharp.FutureProg.Variables.TextVariable(name));
			var room = Mock.Get(actor.Location); room.SetupGet(x => x.Type).Returns(ProgVariableTypes.Location); room.SetupGet(x => x.GetObject).Returns(actor.Location); room.Setup(x => x.GetProperty("terrain")).Returns(terrain.Object);
		}
		var operations = new List<Guid>();
		void Cast(long spell, string target = "")
		{
			actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(native.Resource, 100); FlushCasting(native); var before = actor.MagicResourceAmounts[native.Resource];
			var quote = service.Quote(new(actor, cap.Id, spell, 1, false, target)); Require(quote.Allowed, quote.Reason);
			var result = service.Cast(new(actor, cap.Id, spell, 1, false, target)); Require(result.Status == MagicCastingStatus.Succeeded && before - actor.MagicResourceAmounts[native.Resource] == quote.Invocation!.Costs.Single().Amount, "Installed paid native output failed: " + result.Message);
			operations.Add(result.OperationId!.Value);
			Console.WriteLine($"ARMTRAD-provisions-paid=passed spell:{spell} grade:1 cost:{quote.Invocation.Costs.Single().Amount}");
		}
		Terrain("Desert"); var food = installed.Identities[ArmageddonReviewedProvisionContent.SustainMealKey]; var wine = installed.Identities[ArmageddonReviewedProvisionContent.DrawWineKey];
		IGameItem CreatedFood() => host.Items.Last(x => !x.Deleted && x.SpellCreationOrigin is not null && installed.Plan.FoodProfiles.SelectMany(p => p.Foods).Any(p => p.Id == x.Prototype.Id));
		if (reproduceCustody)
		{
			QualifySenseFood(host, database, fixture, utilities, utilityIds, ids, installed, Cast, CreatedFood);
			return;
		}
		// Clear the earlier paid Sense effect through its legitimate installed predecessor.
		// The reviewed native item-removal adapter requires a callback-free body custodian.
		Cast(utilityIds[ArmageddonReviewedUtilityContent.UnravelEnchantmentKey], "me");
		Require(!actor.Effects.Any() && !native.Body.Effects.Any(), "Installed Unravel did not release the earlier Sense effect for native food consumption.");
		Cast(food); var consumed = CreatedFood(); Require(installed.Plan.FoodProfiles.Last().Foods.Any(x => x.Id == consumed.Prototype.Id), "Explicit fallback pool not selected."); native.Body.Get(consumed, silent: true); Require(native.Body.SilentEat(consumed.GetItemType<IEdible>()!, 0) && consumed.Deleted, "Installed food full consumption failed.");
		Terrain("Silt"); Cast(food); var partial = CreatedFood(); Require(installed.Plan.FoodProfiles.First().Foods.Any(x => x.Id == partial.Prototype.Id), "Explicit first-matching pool not selected.");
		native.Body.Get(partial, silent: true); var satiation = actor.NeedsModel.FoodSatiatedHours; Require(native.Body.Eat(partial.GetItemType<IEdible>()!, null, null, 1.25, null) && partial.GetItemType<IEdible>()!.BitesRemaining == 2.75 && actor.NeedsModel.FoodSatiatedHours == satiation + 0.625, "Installed native partial nutrition failed."); native.Body.Drop(partial, silent: true);
		Terrain("Desert"); Cast(food); var expiring = CreatedFood(); Require(expiring.SpellCreationOrigin!.DeadlineUtc == RuntimeClock.UtcNow.AddSeconds(1350), "Installed food deadline drifted.");
		var vessel = (GameItem)host.Prototypes.Values.Single(x => x.Name == "ARM03B2B C2 vessel").CreateNew(actor); world.Add(vessel); actor.Location.Insert(vessel, true); vessel.Login(); var container = vessel.GetItemType<ILiquidContainer>()!; container.Open();
		Terrain("recipe fixture"); Cast(wine, "vessel"); Require(container.LiquidVolume == 500 && container.LiquidMixture!.Instances.All(x => x.Liquid.Id == installed.Plan.WineRecipes.First().Liquid), "Installed native recipe/0.5L output failed.");
		var alcohol = actor.NeedsModel.AlcoholLitres; Require(native.Body.SilentDrink(container, 250) && container.LiquidVolume == 250 && actor.NeedsModel.AlcoholLitres > alcohol, "Installed wine consumption failed.");
		Terrain("Desert"); Cast(utilityIds[ArmageddonReviewedUtilityContent.HoveringLightKey], "me"); var light = host.Items.Single(x => x.SpellCreationOrigin?.DeadlineUtc == RuntimeClock.UtcNow.AddSeconds(1800));
		FlushCasting(native); var players = TraditionPlayers(database); RequireInstalled(InstallProvisions(database, installed.Plan)); RequireTraditions(InstallTraditions(database, installed.Traditions)); Require(players == TraditionPlayers(database), "Active installer rerun changed acquisition, proficiency, reserve, merits, or consumption.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			foreach (var item in host.Items.Where(x => !x.Deleted && x.InInventoryOf is null && x.ContainedIn is null)) if (!db.RoomsGameItems.Any(x => x.GameItemId == item.Id)) db.RoomsGameItems.Add(new() { RoomId = fixture.RoomId, GameItemId = item.Id }); db.SaveChanges();
		}
		RunItemReaderProcess(new ProvisionInstallerReader(database.Name, fixture, RuntimeClock.UtcNow, utilities, utilityIds.ToDictionary(x => x.Key, x => x.Value), ids.ToDictionary(x => x.Key, x => x.Value), installed,
			consumed.Id, partial.Id, expiring.Id, light.Id, vessel.Id, actor.NeedsModel.FoodSatiatedHours, actor.NeedsModel.AlcoholLitres, native.Body.EffectsOfType<DelayedNeedsFulfillment>().Sum(x => x.Payload.AlcoholLitres), actor.MagicResourceAmounts[native.Resource], operations.ToArray()), "--traditions-provisions-reader");
		Console.WriteLine("ARMTRAD-provisions-output=passed clean-body-after-paid-Unravel actual-paid-grade1 native-food conditional-profile full-and-fractional-bites wine-selected-recipe-drink HoveringLight1800 active-rerun-conservation active-Sense-consumption-not-qualified");
	}
	private static int ProvisionInstallerRestart(string encoded)
	{
		var input = JsonSerializer.Deserialize<ProvisionInstallerReader>(Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock); DelayedNeedsFulfillment.InitialiseEffectType();
		var players = TraditionPlayers(database); var installed = InstallProvisions(database, input.Installed.Plan); RequireInstalled(installed); Require(installed.Identities.All(x => input.Installed.Identities[x.Key] == x.Value) && players == TraditionPlayers(database), "Fresh installer rerun changed IDs/player state.");
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, consumablesAnatomy: true, additionalTraitGroups: ["Armageddon Spell"]); var native = host.Native; var world = native.World;
		Require(world.Traits.Any(x => x.Group == "ARM02") && world.Traits.Count(x => x.Group == "Armageddon Spell") == 82, "Explicit source-group loader lost default ARM02 or additional native skills.");
		LoadTraditionNative(native, database, input.Utilities, input.UtilityIds, input.TraditionIds, 6, input.Installed.Identities);
		var owned = new SpellOwnedItemService(world); native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(owned);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			native.Body.LoadInventory(db.Bodies.Include(x => x.BodiesGameItems).Single(x => x.Id == native.Body.Id));
			foreach (var id in db.RoomsGameItems.Where(x => x.RoomId == input.Fixture.RoomId).Select(x => x.GameItemId).ToArray()) { var item = world.TryGetItem(id, true)!; if (item.InInventoryOf is null && item.ContainedIn is null) native.Actor.Location.Insert(item, true); }
			Require(!db.GameItems.Any(x => x.Id == input.Consumed), "Consumed installed food recreated.");
		}
		foreach (var item in host.Items.ToArray()) item.FinaliseLoadTimeTasks();
		var partial = world.TryGetItem(input.Partial, true)!; var expiring = world.TryGetItem(input.Expiring, true)!; var light = world.TryGetItem(input.Light, true)!; var vessel = world.TryGetItem(input.Vessel, true)!;
		Require(partial.GetItemType<IEdible>()!.BitesRemaining == 2.75 && vessel.GetItemType<ILiquidContainer>()!.LiquidVolume == 250 && native.Actor.NeedsModel.FoodSatiatedHours == input.Food && native.Actor.NeedsModel.AlcoholLitres == input.Alcohol &&
			native.Actor.MagicResourceAmounts[native.Resource] == input.Balance && native.Body.EffectsOfType<DelayedNeedsFulfillment>().Sum(x => x.Payload.AlcoholLitres) == input.PendingAlcohol && input.Operations.All(x => new MagicCastingStateStore().Operation(x)!.Stage == "Completed"), "Fresh process lost/doubled output, reserve, receipt or needs.");
		foreach (var effect in native.Body.EffectsOfType<DelayedNeedsFulfillment>().ToArray()) effect.ExpireEffect();
		Require(native.Actor.NeedsModel.AlcoholLitres == input.Alcohol + input.PendingAlcohol && !native.Body.EffectsOfType<DelayedNeedsFulfillment>().Any(), "Pending alcohol did not fulfil exactly once.");
		clock.Advance(TimeSpan.FromSeconds(1349)); owned.ReconcileRetirements(RuntimeClock.UtcNow); Require(!partial.Deleted && !expiring.Deleted && !light.Deleted, "Installed outputs expired early.");
		clock.Advance(TimeSpan.FromSeconds(1)); owned.ReconcileRetirements(RuntimeClock.UtcNow); Require(partial.Deleted && expiring.Deleted && !light.Deleted && !vessel.Deleted && vessel.GetItemType<ILiquidContainer>()!.LiquidVolume == 250, "Exact food1350 expiry mutated light/wine.");
		clock.Advance(TimeSpan.FromSeconds(450)); owned.ReconcileRetirements(RuntimeClock.UtcNow); Require(light.Deleted && !vessel.Deleted && native.Actor.NeedsModel.FoodSatiatedHours == input.Food, "Exact light1800 expiry changed consumed nutrition/wine.");
		Console.WriteLine("ARMTRAD-provisions-reader=passed fresh-native-process seven-stable-identities six-ready-admissions consumed-absent fractional-bites needs-pending-alcohol reserve completed-receipts exact-food1350 light1800 independent-wine-conservation"); return 0;
	}
	private static int ProvisionCustodyRestart(string encoded) => SenseFoodRestart(encoded);
}
