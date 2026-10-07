#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void RunExtractedContainerPersistenceControls(TestDatabase database, RetirementHost host,
		ICharacter caster, OrderedCurrencyFixture money, FixtureIds fixture)
	{
		var world = host.Native.World;
		var room = (Room)caster.Location;
		foreach (var held in new[] { false, true })
		{
			var bag = (GameItem)host.Prototypes.Values.Single(x => x.Name == "ARM03B2B bag").CreateNew(caster);
			world.Add(bag); room.Insert(bag, true);
			if (held)
			{
				((MudSharp.Body.Implementations.Body)caster.Body).GetWithoutMerge(bag);
				Require(ReferenceEquals(bag.GetItemType<IHoldable>()!.HeldBy, caster.Body) && caster.Body.HeldItems.Contains(bag),
					"Container control requires an actually held native bag.");
			}
			var container = bag.GetItemType<IContainer>()!;
			var survivor = (GameItem)NewOrderedCurrencyPile(host, database, money, 2, 1, room);
			var beforeQuery = SavedOrderedCurrencyState(database, money.Currency.Id);
			Require(container.CanPut(survivor), "Ordinary container CanPut refused compatible native currency.");
			world.SaveManager.Flush();
			Require(beforeQuery == SavedOrderedCurrencyState(database, money.Currency.Id) &&
				ReferenceEquals(survivor.DirectLocation, room) && room.GameItems.Contains(survivor), "Container CanPut changed native custody or persistence.");
			room.Extract(survivor);
			Require(ReferenceEquals(survivor.DirectLocation, room) && !room.GameItems.Contains(survivor),
				"Native Cell.Extract did not retain the expected removal-listener pointer.");
			container.Put(null, survivor, false);
			Require(container.Contents.Single() == survivor && ReferenceEquals(survivor.ContainedIn, bag) &&
				survivor.DirectLocation is null && survivor.GetItemType<IHoldable>()!.HeldBy is null &&
				ReferenceEquals(survivor.InInventoryOf, held ? caster.Body : null), "Extracted native source was not adopted by the exact floor/held container.");
			var absorbed = (GameItem)NewOrderedCurrencyPile(host, database, money, 3, 1, room);
			var absorbedId = absorbed.Id;
			room.Extract(absorbed);
			container.Put(null, absorbed, true);
			var pile = survivor.GetItemType<ICurrencyPile>()!;
			Require(absorbed.Deleted && container.Contents.Single() == survivor && pile.TotalValue == 15m &&
				pile.Coins.Single(x => x.Item1.Id == money.One.Id).Item2 == 5 &&
				pile.Coins.Single(x => x.Item1.Id == money.Five.Id).Item2 == 2, "Container merge changed survivor identity or failed exact native coin conservation.");
			world.SaveManager.Flush();
			using (var db = NewIndependentContext(database.ConnectionString))
			{
				Require(db.GameItems.AsNoTracking().Single(x => x.Id == survivor.Id).ContainerId == bag.Id &&
					!db.BodiesGameItems.Any(x => x.GameItemId == survivor.Id) && !db.RoomsGameItems.Any(x => x.GameItemId == survivor.Id),
					"Container child save retained a direct body/floor join or lost ContainerId.");
				var definitions = db.GameItemComponents.AsNoTracking().Where(x => x.GameItemId == bag.Id).ToArray();
				Require(definitions.SelectMany(x => XElement.Parse(x.Definition).Elements("Contained")).Select(x => (long)x)
					.SequenceEqual(new[] { survivor.Id }), "Native container XML lost its exact merged child.");
				Require(!db.GameItems.Any(x => x.Id == absorbedId) && !db.GameItemComponents.Any(x => x.GameItemId == absorbedId) &&
					!db.BodiesGameItems.Any(x => x.GameItemId == absorbedId) && !db.RoomsGameItems.Any(x => x.GameItemId == absorbedId),
					"Absorbed native source retained saved rows or custody joins.");
				Require(held ? db.BodiesGameItems.Count(x => x.GameItemId == bag.Id && x.BodyId == caster.Body.Id) == 1 :
					db.RoomsGameItems.Count(x => x.GameItemId == bag.Id && x.RoomId == room.Id) == 1, "Native bag lost its actual held/floor save membership.");
			}
			var savedState = SavedOrderedCurrencyState(database, money.Currency.Id);
			var saved = new OrderedCurrencySavedItem(survivor.Id, pile.Coins.Select(x => new OrderedCurrencySavedCoin(x.Item1.Id, x.Item2)).ToArray(),
				survivor.OwnershipReference, null, bag.Id, null, survivor.RoomLayer, survivor.RoutePositionMetres);
			RunItemReaderProcess(new OrderedCurrencyReader(database.Name, fixture, RuntimeClock.UtcNow, money.Ids, bag.Id,
				[saved], savedState, held ? caster.Body.Id : null), "--ordered-currency-reader");
			Require(savedState == SavedOrderedCurrencyState(database, money.Currency.Id), "Container cold reader changed saved custody/maps.");
			container.Take(null, survivor, 0); survivor.Delete();
			if (held) caster.Body.Take(bag);
			bag.Delete(); world.SaveManager.Flush();
			Console.WriteLine($"ARMOrdered-container-{(held ? "held" : "floor")}=passed native-Insert-Extract-Put query-purity actual-currency-merge exact-survivor-15u absorbed-row-deletion saved-container-XML-and-ContainerId fresh-native-reader no-orphan");
		}
	}
}
