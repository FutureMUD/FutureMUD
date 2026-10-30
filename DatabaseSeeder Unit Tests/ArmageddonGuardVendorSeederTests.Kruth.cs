#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonGuardVendorSeederTests
{
	[TestMethod]
	public void KruthDeal_DrawsEachCardOnceAndKeepsIndependentEffectiveDeckMasks()
	{
		using var db = KruthFixture(out var writer); InstallKruth(writer, db);
		var f = new KruthRuntime(db);
		using (Constants.PushRandom(new KruthFirstChoice()))
		{
			Assert.IsTrue(f.Deal("deck " + string.Join(" ", Enumerable.Repeat("guest", 25))));
			Assert.AreEqual(24, f.Received.Count);
			Assert.AreEqual(24, f.Received.Distinct().Count());
			Assert.AreEqual(16777215m, f.Masks[f.Deck.Object.Id]);
			Assert.IsTrue(f.Deal("deck guest"));
			Assert.AreEqual(24, f.Created.Count, "An exhausted deck must not create a replacement card.");
			Assert.IsTrue(f.ActorEchoes().Any(x => x == "{0} is empty."));
			f.Held.Clear(); f.Held.Add(f.Variant.Object);
			Assert.AreEqual(1m, f.Masks[f.Variant.Object.Id], "Effective reset value[5] must survive onload initialization.");
			Assert.IsTrue(f.Deal("deck guest", f.Variant.Object));
			Assert.AreEqual(1811, f.Received.Last());
			Assert.AreEqual(3m, f.Masks[f.Variant.Object.Id]);
			Assert.AreEqual(16777215m, f.Masks[f.Deck.Object.Id]);
		}
	}

	[TestMethod]
	public void KruthDeal_PreservesOrderedRecipientsPartialCompletionAndCardPrivacy()
	{
		using var db = KruthFixture(out var writer); InstallKruth(writer, db);
		var f = new KruthRuntime(db);
		using (Constants.PushRandom(new KruthFirstChoice()))
		{
			Assert.IsTrue(f.Deal("deck guest absent me"));
			CollectionAssert.AreEqual(new[] { 1810 }, f.Received);
			Assert.AreEqual(1m, f.Masks[f.Deck.Object.Id]);
			Assert.IsTrue(f.ActorEchoes().Contains("You don't see that person in the room."));
			Assert.IsTrue(KruthEchoes(f.Recipient).Contains("{0} deals {1} to you."));
			Assert.IsTrue(KruthEchoes(f.Observer).Contains("{0} deals a Kruth card to {1}."));
			var card = f.Created.Single().Object;
			IEnumerable<IPerceivable> Targets(Mock<ICharacter> who) => Mock.Get(who.Object.OutputHandler).Invocations
				.SelectMany(x => x.Arguments.OfType<EmoteOutput>()).SelectMany(x => x.DefaultEmote.Targets);
			Assert.IsTrue(Targets(f.Recipient).Any(x => ReferenceEquals(x, card)));
			Assert.IsFalse(Targets(f.Observer).Any(x => ReferenceEquals(x, card)));
			Assert.IsFalse(Targets(f.Actor).Any(x => ReferenceEquals(x, card)));
			f.ClearEchoes();
			Assert.IsTrue(f.Deal("deck me"));
			CollectionAssert.AreEqual(new[] { 1810, 1811 }, f.Received);
			Assert.IsTrue(f.ActorEchoes().Contains("You deal yourself {0}."));
			Assert.IsTrue(KruthEchoes(f.Observer).Contains("{0} deals themself a Kruth card."));
		}
	}

	[TestMethod]
	public void KruthDeal_DeclinesUnownedOrOtherDeckAndDoesNotConsumeFailedDelivery()
	{
		using var db = KruthFixture(out var writer); InstallKruth(writer, db);
		var f = new KruthRuntime(db);
		Assert.IsFalse(f.Deal("deck guest", f.Variant.Object));
		f.Held.Clear(); Assert.IsFalse(f.Deal("deck guest")); f.Held.Add(f.Deck.Object);
		Assert.IsFalse(f.Deal("missing guest"));
		Assert.IsFalse(f.Hook.Function(EventType.CommandInput, [f.Actor.Object, f.Deck.Object, "shuffle", new StringStack("deck reset")]));
		Assert.IsTrue(f.Deal("deck")); Assert.AreEqual(0, f.Created.Count);
		f.CanReceive = false;
		using (Constants.PushRandom(new KruthFirstChoice()))
		{
			Assert.IsTrue(f.Deal("deck guest"));
			Assert.AreEqual(0m, f.Masks[f.Deck.Object.Id]); Assert.AreEqual(0, f.Received.Count);
			f.Created.Single().Verify(x => x.Delete(), Times.Once);
			f.CanReceive = true;
			Assert.IsTrue(f.Deal("deck guest"));
			CollectionAssert.AreEqual(new[] { 1810 }, f.Received);
		}
	}

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void KruthDeal_UnplacedOrUnattachedDeckRemainsInactive(bool removeAttachment)
	{
		using var db = KruthFixture(out var writer, resetPlaced: removeAttachment);
		if (removeAttachment)
		{
			var source = Field<ArmageddonWorldfilePackage>(writer, "_source");
			var index = Array.FindIndex(source.Items, x => x.Vnum == 1801);
			source.Items[index] = source.Items[index] with { Programs = [] };
		}
		InstallKruth(writer, db);
		Assert.AreEqual(0, db.Hooks.Count()); Assert.AreEqual(0, db.VariableDefinitions.Count(x => x.Property == "armw_kruth_drawn"));
	}

	private static string[] KruthEchoes(Mock<ICharacter> actor) => Mock.Get(actor.Object.OutputHandler).Invocations
		.SelectMany(x => x.Arguments.OfType<EmoteOutput>()).Select(x => x.DefaultEmote.RawText).ToArray();

	private static void InstallKruth(ArmageddonWorldfileWriter writer, FuturemudDatabaseContext db)
	{
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallKruthDecks", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
	}

	private static FuturemudDatabaseContext KruthFixture(out ArmageddonWorldfileWriter writer, bool resetPlaced = true)
	{
		var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var ids = new[] { 1810, 1820, 1830, 1840 }.SelectMany(x => Enumerable.Range(x, 6)).Append(1801).ToHashSet();
		var source = new ArmageddonWorldfilePackage { Items = archive.Items.Where(x => ids.Contains(x.Vnum)).ToArray(),
			Resets = resetPlaced ? archive.Resets.Where(x => x.Opcode is "O" or "P" or "G" or "E" && x.Arguments.Length > 1 && x.Arguments[1] == 1801 && x.ReferenceIssues.Length == 0).ToArray() : [] };
		Assert.AreEqual(25, source.Items.Length); Assert.IsTrue(!resetPlaced || source.Resets.Length > 0);
		writer = new ArmageddonWorldfileWriter(db, source);
		foreach (var item in source.Items)
		{
			var proto = new Db.GameItemProto { Id = item.Vnum == 1801 ? 1801 : 100000 + item.Vnum, Name = item.ShortDescription };
			db.GameItemProtos.Add(proto);
			Field<Dictionary<int, Db.GameItemProto>>(writer, "_items").Add(item.Vnum, proto);
			Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Add(proto.Id, (item, item.Values));
		}
		var deck = source.Items.Single(x => x.Vnum == 1801); var values = deck.Values.ToArray(); values[5] = 1;
		var variant = new Db.GameItemProto { Id = 11801, Name = "Kruth reset variant" }; db.GameItemProtos.Add(variant);
		Field<Dictionary<string, Db.GameItemProto>>(writer, "_resetItemVariants").Add("1801:fixture", variant);
		Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Add(variant.Id, (deck, values));
		db.SaveChanges(); return db;
	}

	private sealed class KruthRuntime
	{
		public Mock<ICharacter> Actor { get; } = ArmageddonGuardVendorSeederTests.Actor();
		public Mock<ICharacter> Recipient { get; } = ArmageddonGuardVendorSeederTests.Actor();
		public Mock<ICharacter> Observer { get; } = ArmageddonGuardVendorSeederTests.Actor();
		public Mock<IGameItem> Deck { get; } = Item<IGameItem>(70001, ProgVariableTypes.Item);
		public Mock<IGameItem> Variant { get; } = Item<IGameItem>(70002, ProgVariableTypes.Item);
		public List<IGameItem> Held { get; } = [];
		public List<int> Received { get; } = [];
		public List<Mock<IGameItem>> Created { get; } = [];
		public Dictionary<long, decimal> Masks { get; } = [];
		public bool CanReceive { get; set; } = true;
		public HookOnInput Hook { get; }
		public KruthRuntime(FuturemudDatabaseContext db)
		{
			var (world, progs, _) = Runtime(db, register =>
			{
				register.Setup(x => x.GetType(ProgVariableTypes.Item, "armw_kruth_drawn")).Returns(ProgVariableTypes.Number);
				register.Setup(x => x.GetValue(It.IsAny<IProgVariable>(), "armw_kruth_drawn"))
					.Returns((IProgVariable item, string _) => new NumberVariable(Masks[((IGameItem)item.GetObject).Id]));
				register.Setup(x => x.SetValue(It.IsAny<IProgVariable>(), "armw_kruth_drawn", It.IsAny<IProgVariable>()))
					.Callback<IProgVariable, string, IProgVariable>((item, _, value) => Masks[((IGameItem)item.GetObject).Id] = (decimal)value.GetObject).Returns(true);
			});
			Hook = new HookOnInput(db.Hooks.Single(), world.Object);
			world.SetupGet(x => x.Hooks).Returns(new All<IHook> { Hook });
			Actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			Actor.Setup(x => x.TargetActor("guest", PerceiveIgnoreFlags.None)).Returns(Recipient.Object);
			Actor.Setup(x => x.TargetActor("me", PerceiveIgnoreFlags.None)).Returns(Actor.Object);
			Actor.Setup(x => x.GetProperty("helditems")).Returns(() => new CollectionVariable(Held, ProgVariableTypes.Item));
			Held.Add(Deck.Object);
			foreach (var (item, prototype) in new[] { (Deck, 1801), (Variant, 11801) })
			{
				item.Setup(x => x.HasKeywords(It.IsAny<IEnumerable<string>>(), Actor.Object, It.IsAny<bool>(), It.IsAny<bool>()))
					.Returns((IEnumerable<string> words, IPerceiver _, bool _, bool _) => words.All(x => x is "deck" or "cards"));
				item.Setup(x => x.GetKeywordsFor(Actor.Object)).Returns(new[] { "deck", "cards" });
				item.Setup(x => x.HasKeyword("deck", Actor.Object, true, false)).Returns(true);
				item.Setup(x => x.HasKeyword("cards", Actor.Object, true, false)).Returns(true);
				item.Setup(x => x.InstallHook(Hook)).Returns(true);
				Assert.IsTrue(progs.Get(db.GameItemProtos.Single(x => x.Id == prototype).GameItemProtosOnLoadProgs.Single().FutureProgId).ExecuteBool(item.Object));
				item.Verify(x => x.InstallHook(Hook), Times.Once);
			}
			var room = Item<IRoom>(1, ProgVariableTypes.Location);
			room.Setup(x => x.GetProperty("characters")).Returns(new CollectionVariable(new[] { Actor.Object, Recipient.Object, Observer.Object }, ProgVariableTypes.Character));
			Actor.Setup(x => x.GetProperty("location")).Returns(room.Object);
			var prototypes = new Mock<IUneditableRevisableAll<IGameItemProto>>();
			world.SetupGet(x => x.ItemProtos).Returns(prototypes.Object);
			foreach (var record in db.GameItemProtos.Where(x => x.Id != 1801 && x.Id != 11801))
			{
				var proto = new Mock<IGameItemProto>(); proto.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
				proto.Setup(x => x.CreateNew(null, null, 1, "")).Returns(() =>
				{
					// Native prototype IDs deliberately differ from source vnums.
					var item = Item<IGameItem>(record.Id - 100000, ProgVariableTypes.Item); Created.Add(item); return new[] { item.Object };
				});
				prototypes.Setup(x => x.Get(record.Id)).Returns(proto.Object);
			}
			foreach (var recipient in new[] { Actor, Recipient })
			{
				var body = new Mock<IBody>(); recipient.SetupGet(x => x.Body).Returns(body.Object);
				body.Setup(x => x.CanGet(It.IsAny<IGameItem>(), 0, ItemCanGetIgnore.None)).Returns(() => CanReceive);
				body.Setup(x => x.Get(It.IsAny<IGameItem>(), 0, null, true, ItemCanGetIgnore.None))
					.Callback<IGameItem, int, IEmote?, bool, ItemCanGetIgnore>((item, _, _, _, _) => Received.Add((int)item.Id));
			}
		}
		public bool Deal(string args, IGameItem? deck = null) => Hook.Function(EventType.CommandInput, [Actor.Object, deck ?? Deck.Object, "deal", new StringStack(args)]);
		public string[] ActorEchoes() => KruthEchoes(Actor);
		public void ClearEchoes() { foreach (var actor in new[] { Actor, Recipient, Observer }) Mock.Get(actor.Object.OutputHandler).Invocations.Clear(); }
	}

	private sealed class KruthFirstChoice : Random
	{
		public override int Next(int minValue, int maxValue) => minValue;
	}
}
