#nullable enable

using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record OrderedCurrencySavedCoin(long Coin, int Count);
	private sealed record OrderedCurrencySavedItem(long Id, OrderedCurrencySavedCoin[] Coins,
		ItemOwnershipReference? Title, long? Body, long? Container, long? Room, RoomLayer Layer, double? Route);
	private sealed record OrderedCurrencyReader(string Database, FixtureIds Fixture, DateTime Now,
		OrderedCurrencyIds Money, long Bag, OrderedCurrencySavedItem[] Items, string SavedState, long? BagBody = null);

	private static int RunOrderedCurrencyReader(string[] args)
	{
		Require(args.Length == 1, "Currency reader needs one owned-database receipt.");
		var input = JsonSerializer.Deserialize<OrderedCurrencyReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args[0])))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database);
		ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime);
		using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, corpseAnimationAnatomy: true);
		var native = host.Native; var world = native.World;
		ConfigureStormHands(native);
		var room = CreateAreaRoom(native, database.ConnectionString, input.Fixture.RoomId, create: false);
		SetPrivateMember(native.Actor, "Location", room);
		var money = ConfigureOrderedCurrency(host, database, input.Money, native.Actor);
		try
		{
			Require(input.SavedState == SavedOrderedCurrencyState(database, money.Currency.Id), "Currency cold reader began with different saved inputs.");
			using (var db = NewIndependentContext(database.ConnectionString))
				native.Body.LoadInventory(db.Bodies.Include(x => x.BodiesGameItems).Single(x => x.Id == native.Body.Id));
			var bag = (GameItem)world.TryGetItem(input.Bag, true)!;
			bag.FinaliseLoadTimeTasks();
			if (input.BagBody is null) room.Insert(bag, true);
			else Require(bag.InInventoryOf?.Id == input.BagBody && ReferenceEquals(bag.InInventoryOf, native.Body) &&
				native.Body.HeldItems.Any(x => ReferenceEquals(x, bag)), "Cold container did not reconstruct its actual held body membership.");
			foreach (var saved in input.Items.Where(x => x.Room.HasValue))
			{
				var item = (GameItem)world.TryGetItem(saved.Id, true)!;
				Require(item.InInventoryOf is null && item.ContainedIn is null, "Cold currency floor root already has another custodian.");
				room.Insert(item, true);
			}
			foreach (var saved in input.Items)
			{
				var item = (GameItem)world.TryGetItem(saved.Id, true)!;
				var pile = item.GetItemType<ICurrencyPile>();
				Require(pile is not null && ReferenceEquals(pile.Currency, money.Currency) &&
					pile.Coins.Count() == saved.Coins.Length && saved.Coins.All(x => pile.Coins.Any(c => c.Item1.Id == x.Coin && c.Item2 == x.Count)),
					"Fresh native currency component lost exact saved denomination maps.");
				Require(item.OwnershipReference == saved.Title && item.GetItemType<IHoldable>()?.HeldBy?.Id == saved.Body &&
					item.ContainedIn?.Id == saved.Container && item.DirectLocation?.Id == saved.Room &&
					item.RoomLayer == saved.Layer && item.RoutePositionMetres == saved.Route,
					"Fresh native currency loading lost title, direct custody, layer or route.");
				Require(saved.Body is null || native.Body.HeldItems.Any(x => ReferenceEquals(x, item)), "Cold body join did not reconstruct actual held membership.");
				Require(saved.Container is null || bag.GetItemType<IContainer>().Contents.Any(x => ReferenceEquals(x, item)), "Cold container XML did not reconstruct actual member identity.");
				Require(saved.Room is null || room.GameItems.Any(x => ReferenceEquals(x, item)), "Cold cell join did not reconstruct actual floor membership.");
			}
			Require(host.Items.Where(x => !x.Deleted && x.GetItemType<ICurrencyPile>() is { } pile && ReferenceEquals(pile.Currency, money.Currency))
				.Select(x => x.Id).OrderBy(x => x).SequenceEqual(input.Items.Select(x => x.Id).OrderBy(x => x)),
				"Currency cold loading replayed creation or loaded an unrecorded funded item.");
			Require(input.SavedState == SavedOrderedCurrencyState(database, money.Currency.Id), "Native currency cold reconstruction changed saved rows/maps/custody.");
			Console.WriteLine($"ARMCurrency-reader=passed process:{Environment.ProcessId} fresh-native-GameItems body-inventory container-contents floor-members exact-title-and-denominations no-creation-replay no-saved-mutation");
			return 0;
		}
		finally { CurrencyGameItemComponentProto.ItemPrototype = money.PreviousSystemPrototype!; }
	}
}
