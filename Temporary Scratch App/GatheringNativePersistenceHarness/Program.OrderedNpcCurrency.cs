// Currency acceptance runs only in the owning disposable database and native area cell.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Decorators;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.GameItems.Components;
using MudSharp.Events;
using MudSharp.FutureProg;
using Db = MudSharp.Models;

#nullable enable
namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RunOrderedNpcCurrency(TestDatabase database, RetirementHost host, ICharacter caster,
		ICharacter foe, HarnessClock clock, Func<ScriptedAiCharacterInstance> cast,
		Action<ScriptedAiCharacterInstance> restored, Action<ScriptedAiCharacterInstance, ICharacter, string> order, FixtureIds fixture)
	{
		var money = ConfigureOrderedCurrency(host, database, SeedOrderedCurrency(database, host.Native.World.Materials.First().Id), caster);
		var world = host.Native.World;
		var cell = (Cell)caster.Location;
		var corpse = cell.GameItems.Select(x => x.GetItemType<ICorpse>()).Single(x => x is not null)!;
		GameItem[] Live() => host.Items.OfType<GameItem>().Where(x => !x.Deleted && !x.Destroyed &&
			x.GetItemType<ICurrencyPile>() is { } pile && ReferenceEquals(pile.Currency, money.Currency)).ToArray();
		void CheckSaved(decimal expected)
		{
			world.SaveManager.Flush();
			Require(Live().Sum(x => x.GetItemType<ICurrencyPile>().TotalValue) == expected, "Native currency runtime conservation failed.");
			using var db = NewIndependentContext(database.ConnectionString);
			var rows = db.GameItemComponents.AsNoTracking().ToArray().Where(x => XElement.Parse(x.Definition).Attribute("Currency")?.Value == money.Currency.Id.ToString()).ToArray();
			var persisted = rows.Sum(row => XElement.Parse(row.Definition).Element("Coins")!.Elements("Coin").Sum(x =>
				((long)x.Attribute("Id")! == money.One.Id ? 1m : 5m) * (int)x.Attribute("Count")!));
			Require(persisted == expected, "Independent native saved currency conservation failed.");
			foreach (var item in Live())
			{
				var saved = rows.Single(x => x.GameItemId == item.Id);
				var map = XElement.Parse(saved.Definition).Element("Coins")!.Elements("Coin").ToDictionary(x => (long)x.Attribute("Id")!, x => (int)x.Attribute("Count")!);
				Require(item.GetItemType<ICurrencyPile>().Coins.All(x => map.TryGetValue(x.Item1.Id, out var count) && count == x.Item2) && map.Count == item.GetItemType<ICurrencyPile>().Coins.Count(), "Native saved denomination map mismatch.");
				var row = db.GameItems.AsNoTracking().Single(x => x.Id == item.Id);
				var bodyRows = db.BodiesGameItems.AsNoTracking().Where(x => x.GameItemId == item.Id).ToArray();
				var cellRows = db.CellsGameItems.AsNoTracking().Where(x => x.GameItemId == item.Id).ToArray();
				Require(item.InInventoryOf is null ? bodyRows.Length == 0 : bodyRows.Length == 1 && bodyRows[0].BodyId == item.InInventoryOf.Id, "Native currency body joins mismatch.");
				Require(item.DirectLocation is null ? cellRows.Length == 0 : cellRows.Length == 1 && cellRows[0].CellId == item.DirectLocation.Id, "Native currency floor joins mismatch.");
				Require(row.ContainerId == item.ContainedIn?.Id && (item.InInventoryOf is not null || item.ContainedIn is not null || item.DirectLocation is not null), "Currency became a saved detached orphan.");
			}
		}
		void ClearMoney()
		{
			foreach (var item in Live()) { item.InInventoryOf?.Take(item); item.ContainedIn?.Take(item); item.Delete(); }
			world.SaveManager.Flush();
			CheckSaved(0);
		}
		try
		{
			RunExtractedContainerPersistenceControls(database, host, caster, money, fixture);
			foreach (var scenario in new[] { "get-room", "get-container", "put", "drop-merge", "drop-new", "give-body", "give-corpse", "get-container-close", "put-close" })
			{
				var operation = scenario.Replace("-close", string.Empty);
				var bag = host.Prototypes.Values.Single(x => x.Name == "ARM03B2B bag").CreateNew(caster);
				world.Add(bag); cell.Insert(bag, true);
				var first = NewOrderedCurrencyPile(host, database, money, 3, 0, cell);
				var second = NewOrderedCurrencyPile(host, database, money, 3, 0, cell);
				var sourceIds = new[] { first.Id, second.Id };
				var destination = bag.GetItemType<IContainer>();
				if (operation == "get-container")
				{
					foreach (var item in new[] { first, second }) { cell.Extract(item); item.Drop(null); destination.Put(null, item, false); }
				}
				else if (operation != "get-room")
				{
					((MudSharp.Body.Implementations.Body)caster.Body).GetWithoutMerge(first);
					((MudSharp.Body.Implementations.Body)caster.Body).GetWithoutMerge(second);
					Require(ReferenceEquals(first.InInventoryOf, caster.Body) && ReferenceEquals(second.InInventoryOf, caster.Body), "Native currency setup needs two actual held sources.");
				}
				var floorSeed = operation.StartsWith("drop-") ? NewOrderedCurrencyPile(host, database, money, 1, 0, cell) : null;
				var expected = floorSeed is null ? 6m : 7m;
				CheckSaved(expected);
				CheckOrderedCurrencyPreview(host, database, money,
					() => operation switch
					{
						"get-room" => caster.Body.CanGet(money.Currency, 99m, true),
						"get-container" => caster.Body.CanGet(money.Currency, bag, 99m, true),
						"put" => caster.Body.CanPut(money.Currency, bag, null, 99m, true),
						"give-body" => caster.Body.CanGive(money.Currency, foe.Body, 99m, true),
						"give-corpse" => caster.Body.CanGive(money.Currency, corpse, 99m, true),
						_ => caster.Body.CanDrop(money.Currency, 99m, true)
					},
					() => operation switch
					{
						"get-room" => caster.Body.WhyCannotGet(money.Currency, 99m, true),
						"get-container" => caster.Body.WhyCannotGet(money.Currency, bag, 99m, true),
						"put" => caster.Body.WhyCannotPut(money.Currency, bag, null, 99m, true),
						"give-body" => caster.Body.WhyCannotGive(money.Currency, foe.Body, 99m, true),
						"give-corpse" => caster.Body.WhyCannotGive(money.Currency, corpse, 99m, true),
						_ => caster.Body.WhyCannotDrop(money.Currency, 99m, true)
					});
				var before = SavedOrderedCurrencyState(database, money.Currency.Id);
				var allowed = operation switch
				{
					"get-room" => caster.Body.CanGet(money.Currency, 6m, true),
					"get-container" => caster.Body.CanGet(money.Currency, bag, 6m, true),
					"put" => caster.Body.CanPut(money.Currency, bag, null, 6m, true),
					"give-body" => caster.Body.CanGive(money.Currency, foe.Body, 6m, true),
					"give-corpse" => caster.Body.CanGive(money.Currency, corpse, 6m, true),
					_ => caster.Body.CanDrop(money.Currency, 6m, true)
				};
				if (!allowed && operation == "put")
				{
					var diagnostic = CurrencyGameItemComponentProto.CreateNewCurrencyPile(money.Currency, new[] { (money.One, 6) }, true);
					Console.WriteLine($"ARMOrdered-currency-put-diagnostic=reach:{caster.CanReachItem(bag, false)} manual:{caster.Body.CanPerformManualAction(out var manualReason)} reason:{manualReason} access:{cell.CanGetAccess(bag, caster)} container:{destination.CanPut(diagnostic)} size:{diagnostic.Size} weight:{diagnostic.Weight} movement:{diagnostic.PreventsMovement()} mount:{caster.RidingMount?.Name} physical:{caster.Body.CanPut(diagnostic, bag, null, 0, false)} selected:{MudSharp.Body.Implementations.Body.FindCurrencyPreservingOwnership(money.Currency, caster.Body.HeldItems.Select(x => x.GetItemType<ICurrencyPile>()), 6m).Count}");
				}
				Require(allowed, "Native currency baseline admission refused " + operation +
					(operation == "put" ? ": " + caster.Body.WhyCannotPut(money.Currency, bag, null, 6m, true) +
						$" sourceCount={caster.Body.HeldItems.Count()} bagLayer={bag.RoomLayer} actorLayer={caster.RoomLayer} bagOpen={bag.GetItemType<IOpenable>()?.IsOpen}" : string.Empty));
				world.SaveManager.Flush();
				Require(before == SavedOrderedCurrencyState(database, money.Currency.Id), "Currency Can query wrote persistent rows or maps.");
				if (scenario.EndsWith("-close", StringComparison.Ordinal))
				{
					var previousProximity = world.ProximityEventService;
					var closePreparation = new Mock<IProximityEventService>();
					var closed = 0;
					closePreparation.Setup(x => x.BeginChange(ProximityChangeCause.Containment, It.IsAny<IPerceivable[]>()))
						.Callback(() => { ++closed; bag.GetItemType<IOpenable>().Close(); }).Returns(Mock.Of<IProximityChangeBatch>());
					host.Native.WorldMock.SetupGet(x => x.ProximityEventService).Returns(closePreparation.Object);
					try { InvokeOrderedCurrencyBaseline(operation, caster.Body, money, bag, foe.Body, corpse); }
					finally { host.Native.WorldMock.SetupGet(x => x.ProximityEventService).Returns(previousProximity); }
					CheckSaved(expected);
					Require(closed > 0 && !bag.GetItemType<IOpenable>().IsOpen &&
						before == SavedOrderedCurrencyState(database, money.Currency.Id) && Live().Length == 2,
						"Closing the prepared native container must refuse without publishing a funded split or changing source maps/custody.");
					ClearMoney(); bag.Delete(); world.SaveManager.Flush();
					Console.WriteLine($"ARMOrdered-currency-{scenario}=passed actual-Close precommit-refusal original-source-maps-and-custody independent-database");
					continue;
				}
				InvokeOrderedCurrencyBaseline(operation, caster.Body, money, bag, foe.Body, corpse);
				CheckSaved(expected);
				var output = operation == "drop-merge" ? (GameItem)floorSeed! : Live().Single(x => !sourceIds.Contains(x.Id) && !ReferenceEquals(x, floorSeed));
				Require(output.GetItemType<ICurrencyPile>().TotalValue == (operation == "drop-merge" ? 7m : 6m) && first.Deleted && second.Deleted, "Transfer did not debit both selected sources fully and retain the true survivor.");
				Require(operation switch
				{
					"get-room" or "get-container" => ReferenceEquals(output.InInventoryOf, caster.Body),
					"put" => ReferenceEquals(output.ContainedIn, bag) && destination.Contents.Contains(output),
					"give-body" => ReferenceEquals(output.InInventoryOf, foe.Body),
					"give-corpse" => ReferenceEquals(output.InInventoryOf, corpse.Body),
					_ => ReferenceEquals(output.DirectLocation, cell) && cell.GameItems.Contains(output)
				}, "Transfer survivor has wrong native destination custody.");
				if (operation == "drop-new") Require(floorSeed!.GetItemType<ICurrencyPile>().TotalValue == 1m && Live().Length == 2, "newStack changed or absorbed the original floor pile.");
				ClearMoney(); bag.Delete(); world.SaveManager.Flush();
				Console.WriteLine($"ARMOrdered-currency-{operation}=passed direct-native-Body all-source-debits actual-denominations empty-cleanup merge-survivor coherent-custody independent-database-maps-and-joins");
			}
			int CashCount(IGameItem item, ICoin coin) => item.GetItemType<ICurrencyPile>().Coins
				.Where(x => ReferenceEquals(x.Item1, coin)).Sum(x => x.Item2);
			void CashMap(IGameItem item, int ones, int fives)
			{
				var coins = item.GetItemType<ICurrencyPile>().Coins.ToArray();
				Require(CashCount(item, money.One) == ones && CashCount(item, money.Five) == fives &&
					coins.All(x => ReferenceEquals(x.Item1, money.One) || ReferenceEquals(x.Item1, money.Five)) &&
					coins.Length == (ones == 0 ? 0 : 1) + (fives == 0 ? 0 : 1), "Adversarial exact native denomination map mismatch.");
			}
			void SavedTitle(IGameItem item, ItemOwnershipReference? title)
			{
				Require(item.OwnershipReference == title, "Currency changed legal title during a split/merge.");
				world.SaveManager.Flush();
				using var db = NewIndependentContext(database.ConnectionString);
				var row = db.GameItems.AsNoTracking().Single(x => x.Id == item.Id);
				Require(row.OwnerId == title?.Id && (title is null ? string.IsNullOrEmpty(row.OwnerType) : row.OwnerType == title.Value.FrameworkItemType),
					"Independent currency row lost legal title.");
			}
			GameItem CashBag()
			{
				var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == "ARM03B2B bag").CreateNew(caster);
				world.Add(item); cell.Insert(item, true); world.SaveManager.Flush(); return item;
			}
			void CashIntoBag(IGameItem item, GameItem bag)
			{
				// Fixture setup only; hooks are unarmed. Real native container, no list/dictionary edits.
				cell.Extract(item); item.Drop(null); bag.GetItemType<IContainer>().Put(null, item, false);
				world.SaveManager.Flush();
				Require(ReferenceEquals(item.ContainedIn, bag) && bag.GetItemType<IContainer>().Contents.Any(x => ReferenceEquals(x, item)),
					"Adversarial setup did not establish actual native container custody.");
			}
			void SavedBag(GameItem bag)
			{
				world.SaveManager.Flush();
				var contents = bag.GetItemType<IContainer>().Contents.Select(x => x.Id).OrderBy(x => x).ToArray();
				var prototype = ((GameItemComponent)bag.GetItemType<IContainer>()).Prototype;
				using var db = NewIndependentContext(database.ConnectionString);
				var component = db.GameItemComponents.AsNoTracking().Single(x => x.GameItemId == bag.Id &&
					x.GameItemComponentProtoId == prototype.Id && x.GameItemComponentProtoRevision == prototype.RevisionNumber);
				var xmlIds = XElement.Parse(component.Definition).Elements("Contained").Select(x => (long)x).OrderBy(x => x).ToArray();
				var rowIds = db.GameItems.AsNoTracking().Where(x => x.ContainerId == bag.Id).Select(x => x.Id).OrderBy(x => x).ToArray();
				Require(contents.SequenceEqual(xmlIds) && contents.SequenceEqual(rowIds), "Native container list/XML/ContainerId disagree.");
			}
			void CashHeld(IGameItem item, IBody body)
			{
				((MudSharp.Body.Implementations.Body)body).GetWithoutMerge(item);
				Require(ReferenceEquals(item.InInventoryOf, body) && body.HeldItems.Any(x => ReferenceEquals(x, item)),
					"Adversarial setup lacks a real held source/survivor.");
				world.SaveManager.Flush();
			}

			// 1. Mixed 1/5 denominations, partial retained source, real saved legal title.
			{
				ClearMoney();
				var source = (GameItem)NewOrderedCurrencyPile(host, database, money, 3, 2, cell); // 13
				source.SetOwner(caster); var title = source.OwnershipReference;
				CheckSaved(13); SavedTitle(source, title);
				caster.Body.Get(money.Currency, 6m, true, silent: true);
				var result = Live().Single(x => !ReferenceEquals(x, source));
				CashMap(source, 2, 1); CashMap(result, 1, 1);
				Require(!source.Deleted && ReferenceEquals(source.DirectLocation, cell) && cell.GameItems.Any(x => ReferenceEquals(x, source)) &&
					ReferenceEquals(result.InInventoryOf, caster.Body), "Partial denomination split lost retained source or destination custody.");
				CheckSaved(13); SavedTitle(source, title); SavedTitle(result, title);
				Console.WriteLine("ARMOrdered-currency-partial-title=passed exact-one-and-five residual native-floor held-split saved-legal-title");
				ClearMoney();
			}

			// 2a. Body merge must retain the actual held survivor; absorbed draft/source are not survivors.
			{
				var source = (GameItem)NewOrderedCurrencyPile(host, database, money, 3, 1, cell); // 8
				var survivor = (GameItem)NewOrderedCurrencyPile(host, database, money, 1, 1, cell); // 6
				source.SetOwner(caster); survivor.SetOwner(caster); var title = source.OwnershipReference;
				CashHeld(source, caster.Body); CashHeld(survivor, foe.Body); CheckSaved(14);
				caster.Body.Give(money.Currency, foe.Body, 6m, true);
				CashMap(source, 2, 0); CashMap(survivor, 2, 2);
				Require(Live().Length == 2 && ReferenceEquals(survivor.InInventoryOf, foe.Body) &&
					foe.Body.HeldItems.Any(x => ReferenceEquals(x, survivor)) && ReferenceEquals(source.InInventoryOf, caster.Body),
					"Currency body merge replaced the real survivor or changed retained source custody.");
				CheckSaved(14); SavedTitle(source, title); SavedTitle(survivor, title);
				Console.WriteLine("ARMOrdered-currency-body-merge-survivor=passed native-held-original-survivor exact-maps independent-database");
				ClearMoney();
			}
			// 2b. Same survivor identity guarantee for a real Container merge, including saved Contents XML.
			{
				var bag = CashBag();
				var source = (GameItem)NewOrderedCurrencyPile(host, database, money, 3, 1, cell);
				var survivor = (GameItem)NewOrderedCurrencyPile(host, database, money, 1, 1, cell);
				source.SetOwner(caster); survivor.SetOwner(caster); var title = source.OwnershipReference;
				CashHeld(source, caster.Body); CashIntoBag(survivor, bag); CheckSaved(14); SavedBag(bag);
				caster.Body.Put(money.Currency, bag, null, 6m, true, silent: true);
				CashMap(source, 2, 0); CashMap(survivor, 2, 2);
				Require(Live().Length == 2 && ReferenceEquals(survivor.ContainedIn, bag) &&
					ReferenceEquals(bag.GetItemType<IContainer>().Contents.Single(), survivor) && ReferenceEquals(source.InInventoryOf, caster.Body),
					"Currency container merge replaced the actual survivor or detached retained source.");
				CheckSaved(14); SavedBag(bag); SavedTitle(source, title); SavedTitle(survivor, title);
				Console.WriteLine("ARMOrdered-currency-container-merge-survivor=passed original-native-contained-survivor saved-container-XML exact-maps");
				ClearMoney(); bag.Delete(); world.SaveManager.Flush();
			}

			// 3. Empty-source OnDeleted observer may claim new custody, and may refill using REAL reserve coins.
			// Two rows distinguish zero-valued relocation from a conserved refill+relocation.
			foreach (var refill in new[] { false, true })
			{
				var first = (GameItem)NewOrderedCurrencyPile(host, database, money, 3, 0, cell);
				var second = (GameItem)NewOrderedCurrencyPile(host, database, money, 3, 0, cell);
				var bag = CashBag();
				var reserve = (GameItem)NewOrderedCurrencyPile(host, database, money, 2, 0, cell);
				CashIntoBag(reserve, bag); CheckSaved(8);
				var notified = 0;
				PerceivableEvent observer = _ =>
				{
					Require(++notified == 1 && !first.GetItemType<ICurrencyPile>().Coins.Any(), "Empty-source deletion observer saw funded or repeated cleanup.");
					if (refill)
					{
						Require(reserve.GetItemType<ICurrencyPile>().RemoveCoins(new[] { (money.One, 1) }), "Reserve debit must retain one real coin.");
						first.GetItemType<ICurrencyPile>().AddCoins(new[] { (money.One, 1) }); // conserved bank -> source
					}
					CashHeld(first, foe.Body); // actual native physical Get, independent recipient
					CashMap(first, refill ? 1 : 0, 0); CashMap(reserve, refill ? 1 : 2, 0);
					CheckSaved(8); SavedBag(bag); // flush INSIDE observer proves coherent postcommit maps/custody
				};
				first.OnDeleted += observer;
				try { caster.Body.Get(money.Currency, 6m, true, silent: true); }
				finally { first.OnDeleted -= observer; }
				var result = Live().Single(x => !ReferenceEquals(x, first) && !ReferenceEquals(x, reserve));
				Require(notified == 1 && !first.Deleted && !first.Destroyed && second.Deleted &&
					ReferenceEquals(first.InInventoryOf, foe.Body) && ReferenceEquals(result.InInventoryOf, caster.Body),
					"Committed empty cleanup stole observer custody/value or lost the true funded result.");
				CashMap(first, refill ? 1 : 0, 0); CashMap(reserve, refill ? 1 : 2, 0); CashMap(result, 6, 0);
				CheckSaved(8); SavedBag(bag);
				Console.WriteLine($"ARMOrdered-currency-empty-source-{(refill ? "refill-relocate" : "relocate")}=passed real-OnDeleted conserved-reserve new-native-custody saved-joins");
				ClearMoney(); bag.Delete(); world.SaveManager.Flush();
			}

			// 4. Recipient's actor rebinds DURING preparation. Controlled fixture identity change only:
			// SetPrivateMember is the existing harness helper, not a production body-switch proof.
			// A native SwitchToBody row needs an already authored form; do not seed/claim one here.
			{
				var first = (GameItem)NewOrderedCurrencyPile(host, database, money, 3, 0, cell);
				var second = (GameItem)NewOrderedCurrencyPile(host, database, money, 3, 0, cell);
				CashHeld(first, caster.Body); CashHeld(second, caster.Body); CheckSaved(6);
				var recipientBody = foe.Body;
				Require(!ReferenceEquals(recipientBody, caster.Body) && ReferenceEquals(recipientBody.Actor, foe) &&
					caster.Body.CanGive(money.Currency, recipientBody, 6m, true), "Native recipient-rebind fixture must initially be admissible.");
				var before = SavedOrderedCurrencyState(database, money.Currency.Id);
				var previousProximity = world.ProximityEventService; var changed = 0;
				var preparation = new Mock<IProximityEventService>();
				preparation.Setup(x => x.BeginChange(ProximityChangeCause.Containment, It.IsAny<IPerceivable[]>()))
					.Callback(() => { if (++changed == 1) SetPrivateMember(foe, "Body", caster.Body); })
					.Returns(Mock.Of<IProximityChangeBatch>());
				host.Native.WorldMock.SetupGet(x => x.ProximityEventService).Returns(preparation.Object);
				try
				{
					caster.Body.Give(money.Currency, recipientBody, 6m, true);
					Require(changed > 0 && !ReferenceEquals(foe.Body, recipientBody), "Recipient rebind preparation callback did not run.");
				}
				finally
				{
					SetPrivateMember(foe, "Body", recipientBody);
					host.Native.WorldMock.SetupGet(x => x.ProximityEventService).Returns(previousProximity);
					Require(ReferenceEquals(foe.Body, recipientBody) && ReferenceEquals(recipientBody.Actor, foe) &&
						ReferenceEquals(caster.Body.Actor, caster), "Recipient/body actor bindings must be restored before saved assertions and ClearMoney.");
				}
				CashMap(first, 3, 0); CashMap(second, 3, 0); CheckSaved(6);
				Require(Live().Length == 2 && ReferenceEquals(first.InInventoryOf, caster.Body) && ReferenceEquals(second.InInventoryOf, caster.Body) &&
					!recipientBody.HeldItems.Any(x => x.IsItemType<ICurrencyPile>()) && before == SavedOrderedCurrencyState(database, money.Currency.Id),
					"Noncanonical recipient preparation consumed/created currency or retargeted the captured body.");
				Console.WriteLine("ARMOrdered-currency-recipient-rebind-refusal=passed actual-preparation-callback native-captured-body no-debit no-publish saved-original");
				ClearMoney();
			}

			// 5. Grant expires in committed OnLoad, yet EXACTLY one body and one item inventory event
			// report the captured committed Get after source cleanup. No second requested operation runs.
			{
				var actor = cast(); actor.Currency = money.Currency; var originalBody = actor.Body;
				var first = (GameItem)NewOrderedCurrencyPile(host, database, money, 3, 0, cell);
				var second = (GameItem)NewOrderedCurrencyPile(host, database, money, 3, 0, cell);
				var bag = CashBag(); var reserve = (GameItem)NewOrderedCurrencyPile(host, database, money, 1, 0, cell);
				CashIntoBag(reserve, bag); CheckSaved(7);
				GameItem? committed = null; var loaded = 0;
				var events = new List<(string Origin, InventoryState Old, InventoryState New, IGameItem Item)>();
				void AssertCommittedNotification()
				{
					Require(committed is not null && first.Deleted && second.Deleted &&
						ReferenceEquals(committed.InInventoryOf, originalBody) && ReferenceEquals(actor.Body, originalBody) &&
						!world.SpellOwnedCorpseAnimations!.CanCommand(actor.InstanceId, caster.Id), "Inventory event lacks the exact committed map/body after grant expiry.");
					CashMap(committed!, 6, 0); CashMap(reserve, 1, 0); CheckSaved(7); SavedBag(bag);
				}
				InventoryChangeEvent bodyEvent = (oldState, newState, item) =>
				{
					events.Add(("body", oldState, newState, item)); AssertCommittedNotification();
					var before = SavedOrderedCurrencyState(database, money.Currency.Id);
					originalBody.Get(money.Currency, bag, 1m, true, silent: true);
					CheckSaved(7); Require(before == SavedOrderedCurrencyState(database, money.Currency.Id) &&
						ReferenceEquals(reserve.ContainedIn, bag), "Expired event callback started another requested transfer.");
				};
				InventoryChangeEvent itemEvent = (oldState, newState, item) =>
				{ events.Add(("item", oldState, newState, item)); AssertCommittedNotification(); };
				var onLoad = new Mock<IFutureProg>();
				onLoad.Setup(x => x.Execute(It.IsAny<object[]>())).Callback<object[]>(args =>
				{
					if (args[0] is not GameItem item || !ReferenceEquals(item.InInventoryOf, originalBody)) return;
					Require(++loaded == 1, "Committed currency draft loaded more than once.");
					committed = item; committed.OnInventoryChange += itemEvent;
					// At this phase selected sources may still be empty native roots; every map is already committed.
					CashMap(first, 0, 0); CashMap(second, 0, 0); CashMap(item, 6, 0); CheckSaved(7);
					var grant = world.SpellOwnedCorpseAnimations!.CommandGrant(actor.InstanceId, caster.Id)!;
					var source = XElement.Parse(XElement.Parse(grant.Provenance).Element("Source")!.Value);
					var until = DateTime.Parse(source.Element("ControlUntilUtc")!.Value, System.Globalization.CultureInfo.InvariantCulture,
						System.Globalization.DateTimeStyles.RoundtripKind);
					clock.Advance(until - RuntimeClock.UtcNow);
					Require(!world.SpellOwnedCorpseAnimations.CanCommand(actor.InstanceId, caster.Id) && actor.IsEmbodied,
						"Notification fixture must expire control without retiring the committed body.");
				}).Returns(null!);
				originalBody.OnInventoryChange += bodyEvent; money.Prototype.OnLoadProgs.Add(onLoad.Object);
				try { order(actor, caster, "get exactly \"6u\""); }
				finally
				{
					originalBody.OnInventoryChange -= bodyEvent; money.Prototype.OnLoadProgs.Remove(onLoad.Object);
					if (committed is not null) committed.OnInventoryChange -= itemEvent;
				}
				Require(loaded == 1 && events.Count == 2 && events[0].Origin == "body" && events[1].Origin == "item" &&
					events.All(x => x.Old == InventoryState.Dropped && x.New == InventoryState.Held && ReferenceEquals(x.Item, committed)),
					"Committed Get lost, duplicated, reordered or misidentified inventory notifications after grant expiry.");
				AssertCommittedNotification();
				Require(world.SpellOwnedCorpseAnimations!.TryRetire(actor.InstanceId, MudSharp.Magic.SpellRetirementReason.Dismissal, out var why), why);
				restored(actor); ClearMoney(); bag.Delete(); world.SaveManager.Flush();
				Console.WriteLine("ARMOrdered-currency-committed-inventory-after-expiry=passed exact-body-item-events original-body survivor coherent-saved-maps reentrant-transfer-refused");
			}

			foreach (var change in new[] { "valid", "pre-expire", "post-expire" })
			{
				var actor = cast(); actor.Currency = money.Currency;
				var first = NewOrderedCurrencyPile(host, database, money, 3, 0, cell);
				var second = NewOrderedCurrencyPile(host, database, money, 3, 0, cell);
				var bag = host.Prototypes.Values.Single(x => x.Name == "ARM03B2B bag").CreateNew(caster);
				world.Add(bag); cell.Insert(bag, true);
				var reserve = NewOrderedCurrencyPile(host, database, money, 1, 0, cell);
				cell.Extract(reserve); reserve.Drop(null); bag.GetItemType<IContainer>().Put(null, reserve, false);
				world.SaveManager.Flush(); CheckSaved(7);
				var before = SavedOrderedCurrencyState(database, money.Currency.Id);
				void Expire()
				{
					var grant = world.SpellOwnedCorpseAnimations!.CommandGrant(actor.InstanceId, caster.Id)!;
					var source = XElement.Parse(XElement.Parse(grant.Provenance).Element("Source")!.Value);
					var until = DateTime.Parse(source.Element("ControlUntilUtc")!.Value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind);
					clock.Advance(until - RuntimeClock.UtcNow);
					Require(!world.SpellOwnedCorpseAnimations.CanCommand(actor.InstanceId, caster.Id), "Currency callback did not revoke actual native stock control.");
				}
				var notified = 0;
				var onLoad = new Mock<IFutureProg>();
				onLoad.Setup(x => x.Execute(It.IsAny<object[]>())).Callback<object[]>(args =>
				{
					if (args[0] is not GameItem item || !ReferenceEquals(item.InInventoryOf, actor.Body) || ++notified != 1) return;
					Require(!first.GetItemType<ICurrencyPile>().Coins.Any() && !second.GetItemType<ICurrencyPile>().Coins.Any() &&
						item.GetItemType<ICurrencyPile>().TotalValue == 6m && actor.Body.HeldItems.Contains(item), "OnLoad observed a partial debit or incoherent native destination.");
					CheckSaved(7);
					if (change != "post-expire") return;
					Expire();
					actor.Body.Get(money.Currency, bag, 1m, true, silent: true);
					Require(reserve.GetItemType<ICurrencyPile>().TotalValue == 1m && ReferenceEquals(reserve.ContainedIn, bag), "Reentrant original order consumed reserve after revocation.");
					caster.Body.Get(money.Currency, bag, 1m, true, silent: true);
					Require(reserve.Deleted && caster.Body.HeldItems.Any(x => x.GetItemType<ICurrencyPile>()?.TotalValue == 1m), "Independent native receiver operation was blocked by original revocation.");
				}).Returns(null!);
				var oldProximity = world.ProximityEventService;
				var proximity = new Mock<IProximityEventService>(); var batch = new Mock<IProximityChangeBatch>();
				var prepared = 0;
				proximity.Setup(x => x.BeginChange(ProximityChangeCause.Containment, It.IsAny<IPerceivable[]>())).Callback(() => { if (++prepared == 1 && change == "pre-expire") Expire(); }).Returns(batch.Object);
				money.Prototype.OnLoadProgs.Add(onLoad.Object);
				host.Native.WorldMock.SetupGet(x => x.ProximityEventService).Returns(proximity.Object);
				try { order(actor, caster, "get exactly \"6u\""); }
				finally { money.Prototype.OnLoadProgs.Remove(onLoad.Object); host.Native.WorldMock.SetupGet(x => x.ProximityEventService).Returns(oldProximity); }
				CheckSaved(7);
				if (change == "pre-expire")
					Require(prepared > 0 && notified == 0 && before == SavedOrderedCurrencyState(database, money.Currency.Id), "Precommit native callback refusal changed currency maps, roots or saved custody.");
				else Require(notified > 0 && first.Deleted && second.Deleted && actor.Body.HeldItems.Any(x => x.GetItemType<ICurrencyPile>()?.TotalValue == 6m), "Actual allowed get currency command did not commit all participants.");
				Require(world.SpellOwnedCorpseAnimations!.TryRetire(actor.InstanceId, MudSharp.Magic.SpellRetirementReason.Dismissal, out var why), why);
				restored(actor); ClearMoney(); bag.Delete(); world.SaveManager.Flush();
				Console.WriteLine($"ARMOrdered-currency-{change}=passed paid-stock actual-default-get-command native-control original-body complete-map-notification refusal reentrant-order-blocked independent-control persisted-conservation");
			}
			// A valid policy callback at the final gate may change native spatial state.
			{
				var actor = cast();
				actor.Currency = money.Currency;
				var originalLayer = actor.RoomLayer;
				var first = NewOrderedCurrencyPile(host, database, money, 3, 0, cell);
				var second = NewOrderedCurrencyPile(host, database, money, 3, 0, cell);
				CheckSaved(6);
				var before = SavedOrderedCurrencyState(database, money.Currency.Id);
				var originalIds = Live().Select(x => x.Id).Order().ToArray();
				var ai = actor.AIs.OfType<MudSharp.NPC.AI.CommandableAI>().Single();
				var policyField = typeof(MudSharp.NPC.AI.CommandableAI).GetField("_canCommandProg", BindingFlags.Instance | BindingFlags.NonPublic)!;
				var previousPolicy = (IFutureProg)policyField.GetValue(ai)!;
				var policy = new Mock<IFutureProg>(); policy.SetupGet(x => x.Id).Returns(previousPolicy.Id);
				var armed = false; var finalCalls = 0; var notifications = 0;
				var onLoad = new Mock<IFutureProg>();
				onLoad.Setup(x => x.Execute(It.IsAny<object[]>())).Callback(() => notifications++).Returns(null!);
				policy.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns<object[]>(args =>
				{
					var allowed = previousPolicy.ExecuteBool(args) == true;
					if (armed && ++finalCalls == 2)
					{
						Require(allowed && world.SpellOwnedCorpseAnimations!.CanCommand(actor.InstanceId, caster.Id),
							"Final policy fixture must retain the actual native control grant.");
						var who = new Mock<IFunction>(); who.SetupGet(x => x.Result).Returns(actor);
						who.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Normal);
						var layer = new Mock<IFunction>(); layer.SetupGet(x => x.Result).Returns(new MudSharp.FutureProg.Variables.TextVariable("InTrees"));
						layer.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Normal);
						var type = typeof(GameItem).Assembly.GetType("MudSharp.FutureProg.Functions.Location.SetLayer", true)!;
						var function = (IFunction)type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
							[typeof(IList<IFunction>), typeof(IFuturemud)], null)!.Invoke([new List<IFunction> { who.Object, layer.Object }, world]);
						Require(function.Execute(Mock.Of<IVariableSpace>()) == StatementResult.Normal &&
							(bool)function.Result.GetObject && actor.RoomLayer == RoomLayer.InTrees,
							"Final policy did not execute native SetLayer.");
					}
					return allowed;
				});
				var oldProximity = world.ProximityEventService;
				var proximity = new Mock<IProximityEventService>(); var batch = new Mock<IProximityChangeBatch>();
				proximity.Setup(x => x.BeginChange(ProximityChangeCause.Containment, It.IsAny<IPerceivable[]>()))
					.Callback(() => armed = true).Returns(batch.Object);
				policyField.SetValue(ai, policy.Object); money.Prototype.OnLoadProgs.Add(onLoad.Object);
				host.Native.WorldMock.SetupGet(x => x.ProximityEventService).Returns(proximity.Object);
				try { order(actor, caster, "get exactly \"6u\""); }
				finally
				{
					policyField.SetValue(ai, previousPolicy); money.Prototype.OnLoadProgs.Remove(onLoad.Object);
					host.Native.WorldMock.SetupGet(x => x.ProximityEventService).Returns(oldProximity);
					actor.RoomLayer = originalLayer;
				}
				Require(armed && finalCalls == 2 && notifications == 0 && originalIds.SequenceEqual(Live().Select(x => x.Id).Order()),
					"Final policy spatial change allocated currency or notified a committed transfer.");
				CashMap(first, 3, 0); CashMap(second, 3, 0); CheckSaved(6);
				Require(before == SavedOrderedCurrencyState(database, money.Currency.Id) &&
					ReferenceEquals(first.Location, cell) && ReferenceEquals(second.Location, cell) &&
					!actor.Body.HeldItems.Any(x => x.GetItemType<ICurrencyPile>() is not null),
					"Final policy spatial change debited old-layer sources or changed saved currency custody.");
				Require(world.SpellOwnedCorpseAnimations!.TryRetire(actor.InstanceId, MudSharp.Magic.SpellRetirementReason.Dismissal, out var why), why);
				restored(actor); ClearMoney();
				Console.WriteLine("ARMOrdered-currency-final-policy-setlayer=passed actual-valid-command-grant native-SetLayer second-final-policy no-debit-allocation-notification saved-custody-unchanged");
			}
			// Save three actual currency custodians, then reconstruct them in a fresh process.
			var coldBag = CashBag();
			var coldFloor = (GameItem)NewOrderedCurrencyPile(host, database, money, 3, 2, cell);
			coldFloor.SetOwner(caster);
			caster.Body.Get(money.Currency, 6m, true, silent: true);
			var coldHeld = (GameItem)NewOrderedCurrencyPile(host, database, money, 3, 1, cell);
			coldHeld.SetOwner(caster); CashHeld(coldHeld, caster.Body);
			var coldContained = (GameItem)NewOrderedCurrencyPile(host, database, money, 1, 1, cell);
			coldContained.SetOwner(caster); CashIntoBag(coldContained, coldBag);
			caster.Body.Put(money.Currency, coldBag, null, 6m, true, silent: true);
			CashMap(coldFloor, 2, 1); CashMap(coldHeld, 3, 1); CashMap(coldContained, 2, 2);
			CheckSaved(27); SavedBag(coldBag);
			var coldBefore = SavedOrderedCurrencyState(database, money.Currency.Id);
			var coldItems = Live().Select(x => new OrderedCurrencySavedItem(x.Id,
				x.GetItemType<ICurrencyPile>().Coins.Select(c => new OrderedCurrencySavedCoin(c.Item1.Id, c.Item2)).ToArray(),
				x.OwnershipReference, x.GetItemType<IHoldable>()?.HeldBy?.Id, x.ContainedIn?.Id, x.DirectLocation?.Id,
				x.RoomLayer, x.RoutePositionMetres)).ToArray();
			RunItemReaderProcess(new OrderedCurrencyReader(database.Name, fixture, RuntimeClock.UtcNow, money.Ids,
				coldBag.Id, coldItems, coldBefore), "--ordered-currency-reader");
			Require(coldBefore == SavedOrderedCurrencyState(database, money.Currency.Id), "Fresh currency reader mutated saved maps/custody or published an orphan.");
			CheckSaved(27); ClearMoney(); coldBag.Delete(); world.SaveManager.Flush();
			Console.WriteLine("ARMOrdered-currency-cold-reload=passed fresh-process native-Body-LoadInventory GameItem-and-container-loading exact-maps-title-floor-held-container no-replay no-saved-mutation");
		}
		finally { CurrencyGameItemComponentProto.ItemPrototype = money.PreviousSystemPrototype!; }
		return 0;
	}

	private sealed record OrderedCurrencyIds(long Currency, long One, long Five,
		long Decorator, long Component, long Prototype);
	private sealed record OrderedCurrencyFixture(Currency Currency, ICoin One, ICoin Five,
		GameItemProto Prototype, IGameItemProto? PreviousSystemPrototype, OrderedCurrencyIds Ids);

	private static OrderedCurrencyIds SeedOrderedCurrency(TestDatabase database, long material)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var currency = new Db.Currency { Name = "ARMOrdered native currency",
			BaseCurrencyToGlobalBaseCurrencyConversion = 1m };
		var division = new Db.CurrencyDivision { Name = "unit", BaseUnitConversionRate = 1m,
			IgnoreCase = true, Currency = currency };
		division.CurrencyDivisionAbbreviations.Add(new() { Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:u|unit|units))$" });
		currency.CurrencyDivisions.Add(division);
		var one = new Db.Coin { Name = "native copper", ShortDescription = "a copper coin",
			FullDescription = "An owned acceptance coin.", Value = 1m, Weight = 0.001,
			GeneralForm = "coin", PluralWord = "coin", UseForChange = true, Currency = currency };
		var five = new Db.Coin { Name = "native silver", ShortDescription = "a silver coin",
			FullDescription = "An owned acceptance coin.", Value = 5m, Weight = 0.001,
			GeneralForm = "coin", PluralWord = "coin", UseForChange = true, Currency = currency };
		currency.Coins.Add(one); currency.Coins.Add(five);
		// Null applicability is explicitly accepted by Currency.Describe. Long also loads Wordy.
		foreach (var type in new[] { CurrencyDescriptionPatternType.Casual,
			CurrencyDescriptionPatternType.Short, CurrencyDescriptionPatternType.ShortDecimal,
			CurrencyDescriptionPatternType.Long })
		{
			var pattern = new Db.CurrencyDescriptionPattern { Currency = currency, Type = (int)type,
				Order = 1, NegativePrefix = "minus ", UseNaturalAggregationStyle = true };
			pattern.CurrencyDescriptionPatternElements.Add(new Db.CurrencyDescriptionPatternElement {
				CurrencyDivision = division, Order = 1, Pattern = "{0} units", AlternatePattern = "",
				PluraliseWord = "unit", ShowIfZero = true, RoundingMode = (int)RoundingMode.Truncate });
			currency.CurrencyDescriptionPatterns.Add(pattern);
		}
		db.Currencies.Add(currency);
		var decorator = new Db.StackDecorator { Name = "ARMOrdered native pile", Type = "Pile",
			Description = "Owned currency fixture", Definition =
			"<Definition><Range Min='0' Max='2147483647' Item='a pile of '/></Definition>" };
		db.StackDecorators.Add(decorator); db.SaveChanges();
		// Deliberately NOT named ARM03B2B: the host's hard-coded component loader rejects
		// Currency Pile. ConfigureOrderedCurrency loads this real DB row explicitly afterward.
		var component = new Db.GameItemComponentProto {
			Id = (db.GameItemComponentProtos.Max(x => (long?)x.Id) ?? 0) + 1,
			Name = "ARMOrdered currency component", Type = "Currency Pile", Description = "Currency Pile",
			Definition = $"<Definition Decorator='{decorator.Id}'/>",
			EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
		db.GameItemComponentProtos.Add(component); db.SaveChanges();
		var hold = db.GameItemComponentProtos.Single(x => x.Name == "ARM03B2B Holdable");
		var prototype = new Db.GameItemProto {
			Id = (db.GameItemProtos.Max(x => (long?)x.Id) ?? 0) + 1,
			Name = "ARMOrdered native currency pile", Keywords = "money coins currency",
			ShortDescription = "a pile of coins", FullDescription = "Owned native currency.",
			MaterialId = material, Size = 1, Weight = 0, BaseItemQuality = (int)ItemQuality.Standard,
			MorphTimeSeconds = 0, MorphEmote = "$0 decays.",
			EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
		prototype.GameItemProtosGameItemComponentProtos.Add(new() {
			GameItemComponentProtoId = hold.Id, GameItemComponentRevision = hold.RevisionNumber });
		prototype.GameItemProtosGameItemComponentProtos.Add(new() {
			GameItemComponentProtoId = component.Id, GameItemComponentRevision = component.RevisionNumber });
		db.GameItemProtos.Add(prototype); db.SaveChanges();
		return new(currency.Id, one.Id, five.Id, decorator.Id, component.Id, prototype.Id);
	}

	private static OrderedCurrencyFixture ConfigureOrderedCurrency(
		RetirementHost host, TestDatabase database, OrderedCurrencyIds ids, ICharacter caster)
	{
		Require(caster.Location is Cell, "Currency Drop qualification requires the active native CreateAreaCell cell.");
		var world = host.Native.World; using var db = NewIndependentContext(database.ConnectionString);
		// The archive omits unit configuration. These are authored acceptance units,
		// not assertions about historical game balance: one weight unit is one kg,
		// and one fluid unit is one millilitre.
		foreach (var (setting, replacement) in new[] { ("BaseWeightUOMToKilograms", "1"), ("BaseFluidUOMToLitres", "0.001") })
		{
			var row = db.StaticConfigurations.SingleOrDefault(x => x.SettingName == setting);
			if (row is null)
			{
				row = new Db.StaticConfiguration { SettingName = setting, Definition = replacement };
				db.StaticConfigurations.Add(row);
				db.SaveChanges();
			}
			var configured = row.Definition;
			host.Native.WorldMock.Setup(x => x.GetStaticConfiguration(setting)).Returns(configured);
		}
		var units = new MudSharp.Framework.Units.UnitManager(world);
		Require(double.IsFinite(units.BaseWeightToKilograms) && units.BaseWeightToKilograms > 0 &&
			double.IsFinite(units.BaseFluidToLitres) && units.BaseFluidToLitres > 0,
			"Currency fixture requires the seeded native weight and fluid conversions.");
		host.Native.WorldMock.SetupGet(x => x.UnitManager).Returns(units);
		var coins = new All<ICoin>();
		foreach (var coin in world.Coins) coins.Add(coin);
		host.Native.WorldMock.SetupGet(x => x.Coins).Returns(coins);
		host.Native.WorldMock.Setup(x => x.Add(It.IsAny<ICoin>())).Callback<ICoin>(coin => coins.Add(coin));
		var model = db.Currencies.Include(x => x.Coins)
			.Include(x => x.CurrencyDivisions).ThenInclude(x => x.CurrencyDivisionAbbreviations)
			.Include(x => x.CurrencyDescriptionPatterns).ThenInclude(x => x.CurrencyDescriptionPatternElements)
			.ThenInclude(x => x.CurrencyDescriptionPatternElementSpecialValues).Single(x => x.Id == ids.Currency);
		var currency = new Currency(model, world);
		var currencies = new All<ICurrency>();
		foreach (var existing in world.Currencies) currencies.Add(existing);
		currencies.Add(currency); host.Native.WorldMock.SetupGet(x => x.Currencies).Returns(currencies);
		var decorators = new All<IStackDecorator>();
		foreach (var existing in world.StackDecorators) decorators.Add(existing);
		decorators.Add(new PileDecorator(db.StackDecorators.Single(x => x.Id == ids.Decorator)));
		host.Native.WorldMock.SetupGet(x => x.StackDecorators).Returns(decorators);
		var componentModel = db.GameItemComponentProtos.Include(x => x.EditableItem).Single(x => x.Id == ids.Component);
		var constructor = typeof(CurrencyGameItemComponentProto).GetConstructor(
			BindingFlags.NonPublic | BindingFlags.Instance, null,
			new[] { typeof(Db.GameItemComponentProto), typeof(IFuturemud) }, null)!;
		var component = (CurrencyGameItemComponentProto)constructor.Invoke(new object[] { componentModel, world });
		var originalCatalogue = world.ItemComponentProtos;
		var catalogue = new Mock<IUneditableRevisableAll<IGameItemComponentProto>>();
		catalogue.Setup(x => x.Get(It.IsAny<long>(), It.IsAny<int>())).Returns<long, int>((id, revision) =>
			id == component.Id && revision == component.RevisionNumber ? component : originalCatalogue.Get(id, revision));
		host.Native.WorldMock.SetupGet(x => x.ItemComponentProtos).Returns(catalogue.Object);
		var prototypeModel = db.GameItemProtos.Include(x => x.EditableItem)
			.Include(x => x.GameItemProtosTags).Include(x => x.GameItemProtosGameItemComponentProtos)
			.Single(x => x.Id == ids.Prototype);
		var prototype = new GameItemProto(prototypeModel, world);
		host.Prototypes.Add(prototype.Id, prototype); // Existing host ItemProtos.Get closes over this dictionary.
		Require(prototype.Components.Any(x => x is HoldableGameItemComponentProto) &&
			prototype.Components.Any(x => x is CurrencyGameItemComponentProto), "System prototype must be native Currency + Holdable.");
		var previous = CurrencyGameItemComponentProto.ItemPrototype;
		CurrencyGameItemComponentProto.ItemPrototype = prototype;
		return new(currency, currency.Coins.Single(x => x.Id == ids.One), currency.Coins.Single(x => x.Id == ids.Five), prototype, previous, ids);
		// Caller MUST restore fixture.PreviousSystemPrototype in its finally. Host catalogues are
		// owned by this fresh host, not global server state. Do not share another host concurrently.
	}

	private static IGameItem NewOrderedCurrencyPile(RetirementHost host, TestDatabase database,
		OrderedCurrencyFixture money, int ones, int fives, Cell cell)
	{
		var item = CurrencyGameItemComponentProto.CreateNewCurrencyPile(money.Currency,
			new[] { (money.One, ones), (money.Five, fives) }.Where(x => x.Item2 > 0));
		host.Native.World.Add(item); cell.Insert(item, true); host.Native.World.SaveManager.Flush();
		Require(item.GetItemType<ICurrencyPile>()!.TotalValue == ones + 5m * fives,
			"Fixture must hold real native coin maps.");
		Require(double.IsFinite(item.Weight) && item.Weight >= 0,
			"Currency fixture must have a finite native weight before admission checks.");
		using var db = NewIndependentContext(database.ConnectionString);
		Require(db.CellsGameItems.Count(x => x.GameItemId == item.Id && x.CellId == cell.Id) == 1,
			"Native Cell.Save must persist fixture floor membership without manual join repairs.");
		return item;
	}
	private static string SavedOrderedCurrencyState(TestDatabase database, long currencyId)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var components = db.GameItemComponents.AsNoTracking().ToArray();
		var currencyRows = components.Where(x => {
			var xml = XElement.Parse(x.Definition); return xml.Attribute("Currency")?.Value == currencyId.ToString();
		}).OrderBy(x => x.GameItemId).ToArray();
		var ids = currencyRows.Select(x => x.GameItemId).ToArray();
		var maps = currencyRows.Select(x => $"{x.GameItemId}:" + string.Join(",", XElement.Parse(x.Definition)
			.Element("Coins")!.Elements("Coin").OrderBy(x => (long)x.Attribute("Id")!).Select(x =>
				$"{(long)x.Attribute("Id")!}={(int)x.Attribute("Count")!}")));
		var bodies = db.BodiesGameItems.AsNoTracking().Where(x => ids.Contains(x.GameItemId))
			.OrderBy(x => x.GameItemId).Select(x => new { x.GameItemId, x.BodyId }).ToArray();
		var cells = db.CellsGameItems.AsNoTracking().Where(x => ids.Contains(x.GameItemId))
			.OrderBy(x => x.GameItemId).Select(x => new { x.GameItemId, x.CellId }).ToArray();
		var containers = db.GameItems.AsNoTracking().Where(x => ids.Contains(x.Id))
			.OrderBy(x => x.Id).Select(x => new { x.Id, x.ContainerId }).ToArray();
		return $"rows={db.GameItems.Count()}/{components.Length};maps=" + string.Join(";", maps) +
			";body=" + string.Join(";", bodies.Select(x => $"{x.GameItemId}@{x.BodyId}")) +
			";cell=" + string.Join(";", cells.Select(x => $"{x.GameItemId}@{x.CellId}")) +
			";container=" + string.Join(";", containers.Select(x => $"{x.Id}@{x.ContainerId}"));
	}

	private static void CheckOrderedCurrencyPreview(RetirementHost host, TestDatabase database,
		OrderedCurrencyFixture money, Func<bool> can, Func<string> why)
	{
		host.Native.World.SaveManager.Flush();
		var saved = SavedOrderedCurrencyState(database, money.Currency.Id);
		var runtime = host.Items.Where(x => !x.Deleted && x.IsItemType<ICurrencyPile>()).OrderBy(x => x.Id)
			.Select(x => $"{x.Id}:" + string.Join(",", x.GetItemType<ICurrencyPile>()!.Coins
				.OrderBy(c => c.Item1.Id).Select(c => $"{c.Item1.Id}={c.Item2}"))).ToArray();
		Require(!can() && !string.IsNullOrWhiteSpace(why()), "Refused Can/Why preview must be truthful.");
		host.Native.World.SaveManager.Flush();
		Require(saved == SavedOrderedCurrencyState(database, money.Currency.Id), "Can/Why created rows, changed coins or persisted custody.");
		var current = host.Items.Where(x => !x.Deleted && x.IsItemType<ICurrencyPile>()).OrderBy(x => x.Id)
			.Select(x => $"{x.Id}:" + string.Join(",", x.GetItemType<ICurrencyPile>()!.Coins
				.OrderBy(c => c.Item1.Id).Select(c => $"{c.Item1.Id}={c.Item2}"))).ToArray();
		Require(runtime.SequenceEqual(current), "Can/Why mutated native maps or world roots.");
	}

	// The following exact public calls exercise the prepared-all implementation, not the
	// old sequential proposal. Each row needs fresh sources, sufficient free hands, and
	// runtime + independent DB assertions detailed in the accompanying notes.
	private static void InvokeOrderedCurrencyBaseline(string operation, IBody sender,
		OrderedCurrencyFixture money, IGameItem bag, IBody receiver, ICorpse corpse)
	{
		switch (operation)
		{
			case "get-room": sender.Get(money.Currency, 6m, true, silent: true); break;
			case "get-container": sender.Get(money.Currency, bag, 6m, true, silent: true); break;
			case "put": sender.Put(money.Currency, bag, null, 6m, true, silent: true); break;
			case "drop-merge": sender.Drop(money.Currency, 6m, true, newStack: false, silent: true); break;
			case "drop-new": sender.Drop(money.Currency, 6m, true, newStack: true, silent: true); break;
			case "give-body": sender.Give(money.Currency, receiver, 6m, true); break;
			case "give-corpse": sender.Give(money.Currency, corpse, 6m, true); break;
			default: throw new ArgumentOutOfRangeException(nameof(operation));
		}
	}
}
