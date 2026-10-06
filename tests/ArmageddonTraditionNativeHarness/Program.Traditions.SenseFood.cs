#nullable enable
using System.Reflection;
using System.Text;
using System.Text.Json;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;

namespace FutureMUD.GatheringNativePersistenceHarness;

// Combined installed-content qualification, adapted from cleared repair cb687f704.
internal static partial class GNHProgram
{
	private static bool _senseColdControl;
	private static int PreparedSenseControlNative()
	{
		_senseColdControl = true;
		return TraditionNative(true, true);
	}
	private sealed record SenseFoodReader(string Database, FixtureIds Fixture, DateTime Now,
		ArmageddonMagicInstallPlan Utilities, Dictionary<string, long> UtilityIds,
		Dictionary<string, long> TraditionIds, InstalledProvisions Installed,
		long Item, Guid Origin, double Bites, double Food, double Balance,
		string?[] ActorEffects, string?[] ItemEffects, Guid SenseIdentity, string Action);

	private static void QualifySenseFood(RetirementHost host, TestDatabase database, FixtureIds fixture,
		ArmageddonMagicInstallPlan utilities, IReadOnlyDictionary<string, long> utilityIds,
		IReadOnlyDictionary<string, long> ids, InstalledProvisions installed,
		Action<long, string> cast, Func<IGameItem> createdFood)
	{
		var native = host.Native; var actor = native.Actor; var world = native.World;
		var food = installed.Identities[ArmageddonReviewedProvisionContent.SustainMealKey];
		Require(actor.Effects.Any(x => x.GetType() == typeof(SpellDetectMagickEffect)) &&
			actor.Effects.Any(x => x.GetType() == typeof(MagicSpellParent)), "Actual installed paid Sense must remain active.");
		// The earlier progression-only fixture cast used its default mock scheduler. Release it
		// through paid Unravel and legitimately recast Sense under this real native scheduler.
		cast(utilityIds[ArmageddonReviewedUtilityContent.UnravelEnchantmentKey], "me");
		Require(!actor.Effects.Any(), "Paid Unravel must release the earlier fixture Sense graph.");
		cast(utilityIds[ArmageddonReviewedUtilityContent.SenseEnchantmentKey], "me");
		var scheduledSense = actor.Effects.OfType<MagicSpellParent>().Single();
		Require(world.EffectScheduler.IsScheduled(scheduledSense) && world.EffectScheduler.RemainingDuration(scheduledSense) > TimeSpan.Zero,
			"Paid recast must retain the installed Sense duration under the actual native scheduler.");
		void Read(IGameItem item, string action)
		{
			FlushCasting(native);
			RunItemReaderProcess(new SenseFoodReader(database.Name, fixture, RuntimeClock.UtcNow, utilities,
				utilityIds.ToDictionary(x => x.Key, x => x.Value), ids.ToDictionary(x => x.Key, x => x.Value), installed,
				item.Id, item.SpellCreationOrigin!.LifecycleId, item.GetItemType<IEdible>()!.BitesRemaining,
				actor.NeedsModel.FoodSatiatedHours, actor.MagicResourceAmounts[native.Resource],
				actor.Effects.Select(x => x.GetType().FullName).ToArray(), item.Effects.Select(x => x.GetType().FullName).ToArray(),
				actor.Effects.OfType<MagicSpellParent>().Single().Identity, action), "--traditions-custody-reader");
		}
		IGameItem Meal()
		{
			cast(food, ""); var item = createdFood(); native.Body.Get(item, silent: true); FlushCasting(native);
			Require(native.Body.HeldItems.Contains(item) && ReferenceEquals(item.InInventoryOf, native.Body), "Paid food must enter real native held custody.");
			return item;
		}
		void Eat(IGameItem item, double bites)
		{
			var before = actor.NeedsModel.FoodSatiatedHours; var balance = actor.MagicResourceAmounts[native.Resource];
			var edible = item.GetItemType<IEdible>()!; var portion = bites == 0 ? edible.BitesRemaining : bites;
			Require(native.Body.SilentEat(edible, bites) && edible.BitesRemaining == (bites == 0 ? 0 : 4 - bites) &&
				actor.NeedsModel.FoodSatiatedHours == before + 2 * portion / 4 && actor.MagicResourceAmounts[native.Resource] == balance,
				"Native eating must credit only its consumed portion without another debit.");
		}
		if (_senseColdControl)
		{
			var held = Meal(); var edible = held.GetItemType<IEdible>()!;
			var before = actor.NeedsModel.FoodSatiatedHours; var balance = actor.MagicResourceAmounts[native.Resource]; var calls = 0;
			MudSharp.Body.InventoryChangeEvent callback = (_, state, item) =>
			{
				if (!ReferenceEquals(item, held) || state != MudSharp.Body.InventoryState.Dropped) return;
				calls++; throw new InvalidOperationException("Cold retry native callback refusal");
			};
			native.Body.OnInventoryChange += callback;
			string? refusal = null;
			try { native.Body.SilentEat(edible, 0); }
			catch (InvalidOperationException error) { refusal = error.Message; }
			finally { native.Body.OnInventoryChange -= callback; }
			Require(calls == 1 && refusal is not null && !held.Deleted && edible.BitesRemaining == 0 &&
				native.Body.HeldItems.Contains(held) && ReferenceEquals(held.InInventoryOf, native.Body) &&
				actor.NeedsModel.FoodSatiatedHours == before + 2 && actor.MagicResourceAmounts[native.Resource] == balance &&
				host.Store.Find(held.SpellCreationOrigin!.LifecycleId)!.State == SpellLifecycleState.Retiring,
				"Cold retry fixture must preserve real refused retirement, exact exhausted custody and once-only nutrition/payment.");
			Read(held, "cold-retry");
			// The child owns retry and expiry. Do not save this now-stale original host afterward.
			Console.WriteLine("ARMSENSE-cold-control=passed fresh-process-retirement-retry scheduled-Sense-expiry terminal-second-reader no-original-host-save");
			return;
		}
		var partial = Meal(); Eat(partial, 1.25); Read(partial, "partial");
		var remainingNeeds = actor.NeedsModel.FoodSatiatedHours;
		Require(native.Body.SilentEat(partial.GetItemType<IEdible>()!, 0) && partial.Deleted &&
			actor.NeedsModel.FoodSatiatedHours == remainingNeeds + 1.375, "Finishing partial food must consume exactly its remainder.");
		Read(partial, "removed");
		var full = Meal(); Eat(full, 0);
		Require(full.Deleted && !native.Body.AllItems.Any(x => ReferenceEquals(x, full)), "Full consumption must clear exact held custody.");
		Read(full, "removed");
		Console.WriteLine("ARMSENSE-food=passed actual-installed-paid-Sense-and-SustainMeal native-Body.Get-SilentEat partial1.25-full4 finish2.75 exact-needs unchanged-reserve passive-effects-preserved fresh-readers");

		foreach (var dependency in new[] { false, true })
		{
			var held = Meal(); var edible = held.GetItemType<IEdible>()!;
			var before = actor.NeedsModel.FoodSatiatedHours; var balance = actor.MagicResourceAmounts[native.Resource];
			var calls = 0; MagicSpellParent? itemParent = null;
			MudSharp.Body.InventoryChangeEvent callback = (_, state, item) =>
			{
				if (!ReferenceEquals(item, held) || state != MudSharp.Body.InventoryState.Dropped) return;
				calls++;
				if (!dependency) throw new InvalidOperationException("Sense food native removal callback refusal");
				itemParent = new MagicSpellParent(held, ((MagicSpellParent)actor.Effects.Single(x => x.GetType() == typeof(MagicSpellParent))).Spell, actor);
				var child = new SpellDetectMagickEffect(held, itemParent); itemParent.AddSpellEffect(child);
				held.AddEffect(child); held.AddEffect(itemParent);
			};
			native.Body.OnInventoryChange += callback;
			string? refusal = null;
			try { native.Body.SilentEat(edible, 0); }
			catch (InvalidOperationException error) { refusal = error.Message; }
			finally { native.Body.OnInventoryChange -= callback; }
			Require(calls == 1 && refusal is not null && !held.Deleted && edible.BitesRemaining == 0 &&
				native.Body.HeldItems.Contains(held) && ReferenceEquals(held.InInventoryOf, native.Body) &&
				actor.NeedsModel.FoodSatiatedHours == before + 2 && actor.MagicResourceAmounts[native.Resource] == balance,
				"Real native callback refusal must retain exact held zero remainder and credit nutrition once.");
			Require(!native.Body.SilentEat(edible, 0) && actor.NeedsModel.FoodSatiatedHours == before + 2,
				"Refused retirement must not supply a second meal.");
			Read(held, "held");
			if (itemParent is not null) held.RemoveEffect(itemParent, true);
			FlushCasting(native); held.Delete();
			Require(held.Deleted && host.Store.Find(held.SpellCreationOrigin!.LifecycleId)!.State == SpellLifecycleState.Completed &&
				actor.NeedsModel.FoodSatiatedHours == before + 2 && actor.MagicResourceAmounts[native.Resource] == balance,
				"Retry must retire only the exact origin without replaying nutrition or payment.");
			Read(held, "removed");
			Console.WriteLine($"ARMSENSE-callback=passed kind:{(dependency ? "foreign-item-effect" : "native-throw")} one-callback zero-bites exact-custody-compensation fresh-held-reader same-origin-retry fresh-removed-reader no-meal-replay");
		}
		// This reader owns the final consumption. The paused original host performs no later saves.
		var coldFinish = Meal(); Eat(coldFinish, 1.25); Read(coldFinish, "cold-finish");
		Console.WriteLine("ARMSENSE-cold-consumption=passed fresh-native-Sense-parent-child-graph real-SilentEat2.75 terminal-second-cold-reader exact-needs no-original-host-save-after-child-commit");
	}

	private static int SenseFoodRestart(string encoded)
	{
		var input = JsonSerializer.Deserialize<SenseFoodReader>(Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		MagicSpellParent.InitialiseEffectType(); SpellDetectMagickEffect.InitialiseEffectType();
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, consumablesAnatomy: true, additionalTraitGroups: ["Armageddon Spell"]);
		var native = host.Native;
		var effectScheduler = new EffectScheduler(native.World, clock); native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(effectScheduler);
		LoadTraditionNative(native, database, input.Utilities, input.UtilityIds, input.TraditionIds, 6, input.Installed.Identities);
		var owned = new SpellOwnedItemService(native.World); native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(owned);
		using var db = NewIndependentContext(database.ConnectionString);
		native.Actor.RestoreCastingEffects(db.Characters.Find(native.Actor.Id)!.EffectData);
		// Perform the same cached-schedule phase used by Character.Login, without claiming
		// a complete login/server fixture or inventing a duration for the saved native effect.
		typeof(PerceivedItem).GetMethod("ScheduleCachedEffects", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(native.Actor, null);
		native.Body.LoadInventory(db.Bodies.Include(x => x.BodiesGameItems).Single(x => x.Id == native.Body.Id));
		if (input.Action == "expired")
		{
			Require(input.ActorEffects.Length == 0 && !native.Actor.Effects.Any() &&
				host.Store.Find(input.Origin)!.State == SpellLifecycleState.Completed && !db.GameItems.Any(x => x.Id == input.Item) &&
				!db.GameItemComponents.Any(x => x.GameItemId == input.Item) && !db.BodiesGameItems.Any(x => x.GameItemId == input.Item) &&
				!db.CellsGameItems.Any(x => x.GameItemId == input.Item) && !native.Body.AllItems.Any(x => x.Id == input.Item) &&
				native.Actor.NeedsModel.FoodSatiatedHours == input.Food && native.Actor.MagicResourceAmounts[native.Resource] == input.Balance,
				"Second cold reader must preserve scheduled Sense expiry, terminal exact-origin removal and conserved needs/reserve.");
			Console.WriteLine("ARMSENSE-expiry-reader=passed fresh-native-process no-Sense-parent-or-child terminal-origin no-item-custody needs-reserve-conserved");
			return 0;
		}
		var senseParent = native.Actor.Effects.OfType<MagicSpellParent>().Single();
		var sense = native.Actor.Effects.OfType<SpellDetectMagickEffect>().Single();
		Require(native.Actor.Effects.Count() == input.ActorEffects.Length &&
			native.Actor.Effects.Select(x => x.GetType().FullName).ToHashSet().SetEquals(input.ActorEffects) &&
			senseParent.Identity == input.SenseIdentity && ReferenceEquals(sense.ParentEffect, senseParent) &&
			senseParent.SpellEffects.Count() == 1 && ReferenceEquals(senseParent.SpellEffects.Single(), sense) &&
			native.Actor.Effects.Any(x => x.GetType() == typeof(SpellDetectMagickEffect)) &&
			native.Actor.NeedsModel.FoodSatiatedHours == input.Food && native.Actor.MagicResourceAmounts[native.Resource] == input.Balance,
			"Fresh native reader must preserve installed active Sense, consumed needs and paid reserve.");
		var life = CastingRequired(host.Store.Find(input.Origin));
		if (input.Action == "removed")
		{
			Require(life.State == SpellLifecycleState.Completed && !db.GameItems.Any(x => x.Id == input.Item) &&
				!db.GameItemComponents.Any(x => x.GameItemId == input.Item) && !db.BodiesGameItems.Any(x => x.GameItemId == input.Item) &&
				!db.CellsGameItems.Any(x => x.GameItemId == input.Item) && !native.Body.AllItems.Any(x => x.Id == input.Item),
				"Committed removal must persist terminal lifecycle and clear only its exact item/components/custody.");
		}
		else
		{
			var held = CastingRequired(native.World.TryGetItem(input.Item, true)); held.FinaliseLoadTimeTasks();
			Require(held.GetItemType<IEdible>()!.BitesRemaining == input.Bites && !held.Deleted &&
				native.Body.HeldItems.Contains(held) && ReferenceEquals(held.InInventoryOf, native.Body) &&
				held.Effects.Select(x => x.GetType().FullName).ToHashSet().SetEquals(input.ItemEffects) &&
				held.Effects.Count() == input.ItemEffects.Length &&
				life.State == (input.Action is "partial" or "cold-finish" ? SpellLifecycleState.Active : SpellLifecycleState.Retiring),
				"Fresh native reader must preserve exact remainder, active/held intent, foreign effects and saved custody.");
			if (input.Action == "held") Require(!native.Body.SilentEat(held.GetItemType<IEdible>()!, 0) && native.Actor.NeedsModel.FoodSatiatedHours == input.Food,
				"Reloaded exhausted hold must not credit a second meal.");
			if (input.Action == "cold-finish")
			{
				Require(input.Bites == 2.75 && native.Body.SilentEat(held.GetItemType<IEdible>()!, 0) && held.Deleted &&
					native.Actor.NeedsModel.FoodSatiatedHours == input.Food + 1.375 &&
					native.Actor.MagicResourceAmounts[native.Resource] == input.Balance && native.Actor.Effects.Contains(sense) &&
					native.Actor.Effects.Contains(senseParent), "Fresh-loaded Sense must allow the actual native remaining meal and preserve effects/reserve.");
				FlushCasting(native);
				RunItemReaderProcess(input with { Bites = 0, Food = native.Actor.NeedsModel.FoodSatiatedHours, Action = "removed" }, "--traditions-custody-reader");
				life = CastingRequired(host.Store.Find(input.Origin));
			}
			if (input.Action == "cold-retry")
			{
				Require(input.Bites == 0 && !native.Body.SilentEat(held.GetItemType<IEdible>()!, 0) &&
					native.Actor.NeedsModel.FoodSatiatedHours == input.Food, "Cold exhausted retirement must not replay a meal.");
				Require(owned.ReconcileRetirements(RuntimeClock.UtcNow) >= 1 && held.Deleted &&
					host.Store.Find(input.Origin)!.State == SpellLifecycleState.Completed && !native.Body.AllItems.Any(x => x.Id == input.Item) &&
					native.Actor.NeedsModel.FoodSatiatedHours == input.Food && native.Actor.MagicResourceAmounts[native.Resource] == input.Balance &&
					native.Actor.Effects.Contains(sense) && native.Actor.Effects.Contains(senseParent),
					"Fresh retirement reconciliation must remove only the exact refused origin with active Sense and no nutrition/payment replay.");
				var remaining = effectScheduler.RemainingDuration(senseParent);
				Require(effectScheduler.IsScheduled(senseParent) && remaining > TimeSpan.Zero, "Reloaded native Sense must retain its real scheduled duration.");
				clock.Advance(remaining - TimeSpan.FromTicks(1)); effectScheduler.CheckSchedules();
				Require(native.Actor.Effects.Contains(sense) && native.Actor.Effects.Contains(senseParent), "Sense expired before its actual persisted deadline.");
				clock.Advance(TimeSpan.FromTicks(1)); effectScheduler.CheckSchedules();
				Require(!native.Actor.Effects.Any() && !effectScheduler.IsScheduled(senseParent) &&
					native.Actor.NeedsModel.FoodSatiatedHours == input.Food && native.Actor.MagicResourceAmounts[native.Resource] == input.Balance,
					"Actual scheduler expiry must remove Sense parent/child without replaying consumed needs or reserve.");
				FlushCasting(native);
				RunItemReaderProcess(input with { Now = RuntimeClock.UtcNow, ActorEffects = [], Action = "expired" }, "--traditions-custody-reader");
				life = CastingRequired(host.Store.Find(input.Origin));
				Console.WriteLine("ARMSENSE-cold-retry=passed actual-ReconcileRetirements same-origin active-Sense persisted-deadline scheduler-exact-expiry no-meal-or-payment-replay");
			}
		}
		Console.WriteLine("ARMSENSE-reader=passed " + JsonSerializer.Serialize(new { input.Action, input.Item, input.Origin,
			bites = input.Action == "cold-finish" ? 0 : input.Bites, food = native.Actor.NeedsModel.FoodSatiatedHours,
			input.Balance, lifecycle = life.State.ToString(), nativeSenseGraphValidated = true,
			activeNativeSense = native.Actor.Effects.OfType<SpellDetectMagickEffect>().Any(), input.SenseIdentity }));
		return 0;
	}
}
