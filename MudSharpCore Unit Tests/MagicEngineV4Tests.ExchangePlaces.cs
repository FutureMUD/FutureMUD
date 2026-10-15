#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Powers;
using MudSharp.NPC.AI;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	[DataTestMethod]
	[DataRow(1, 0)] [DataRow(2, 0)] [DataRow(3, 1)] [DataRow(4, 2)]
	[DataRow(5, 3)] [DataRow(6, 4)] [DataRow(7, 5)]
	public void ExchangePlaces_StockPaidCastSwapsExactlyTwoBodiesAndLayersWithRankedMaterials(int grade, int rank)
	{
		var f = new ExchangeFixture(rank); f.TargetLayer = RoomLayer.HighInAir;
		var result = f.Cast(grade);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreSame(f.Destination.Object, f.CasterLocation, $"Moves={f.Moves}; consumed={3-f.AlterationQuantity}; {string.Join("; ", f.F.Messages)}"); Assert.AreSame(f.Source.Object, f.TargetLocation);
		Assert.AreEqual(RoomLayer.HighInAir, f.CasterLayer); Assert.AreEqual(RoomLayer.GroundLevel, f.TargetLayer);
		Assert.AreEqual(2, f.Moves); Assert.AreEqual(2, f.AlterationQuantity); Assert.AreEqual(3, f.TeleportationQuantity);
		Assert.AreEqual(grade == 7 ? 50.0 : 67.0, f.F.Balances[f.F.Resources[1]]);
		var before = f.Moves;
		var retry = f.F.Service.Cast(f.Intent(grade) with { OriginId = result.OperationId });
		Assert.AreEqual(MagicCastingStatus.Refused, retry.Status); Assert.AreEqual(before, f.Moves);
	}

	[DataTestMethod]
	[DataRow("contact")] [DataRow("indoors")] [DataRow("staff")] [DataRow("restrictedSource")]
	[DataRow("restrictedDestination")] [DataRow("populated")] [DataRow("samePosition")]
	[DataRow("mounted")] [DataRow("retained")] [DataRow("consumed")] [DataRow("plane")]
	public void ExchangePlaces_StockRefusalsNeverPayConsumeOrMove(string reason)
	{
		var f = new ExchangeFixture();
		switch (reason)
		{
			case "contact": f.CasterEffects.Clear(); break;
			case "indoors": f.Destination.Setup(x => x.OutdoorsType(It.IsAny<IPerceiver>())).Returns(RoomOutdoorsType.Indoors); break;
			case "staff": f.Target.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true); break;
			case "restrictedSource": f.Source.Setup(x => x.IsA(f.Restricted)).Returns(true); break;
			case "restrictedDestination": f.Destination.Setup(x => x.IsA(f.Restricted)).Returns(true); break;
			case "populated": f.Destination.Setup(x => x.IsA(f.Populated)).Returns(true); break;
			case "samePosition": f.TargetLocation = f.Source.Object; break;
			case "mounted": f.Target.SetupGet(x => x.RidingMount).Returns(Mock.Of<ICharacter>()); break;
			case "retained": f.Items.Remove(f.Teleportation); break;
			case "consumed": f.Items.Remove(f.Alteration); break;
			case "plane": f.TargetBody.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(2)); break;
		}
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(3, f.AlterationQuantity);
		Assert.AreEqual(0, f.Moves); Assert.IsNull(result.OperationId);
	}

	[DataTestMethod]
	[DataRow("target")] [DataRow("contact")] [DataRow("component")]
	public void ExchangePlaces_FinalAdmissionRejectsFrozenInputChangesBeforePayment(string change)
	{
		var f = new ExchangeFixture();
		f.F.Checkpoint = stage =>
		{
			if (stage != "BeforePayment") return;
			if (change == "target") f.TargetLocation = f.Source.Object;
			if (change == "contact") f.TargetEffects.Clear();
			if (change == "component") f.Items.Remove(f.Teleportation);
		};
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(3, f.AlterationQuantity); Assert.AreEqual(0, f.Moves);
	}

	[TestMethod]
	public void ExchangePlaces_SecondMoveRefusalRestoresFirstWithoutRefundOrMastery()
	{
		var f = new ExchangeFixture { RefuseTargetMove = true };
		var result = f.Cast(1);
		Assert.AreEqual("false", (string?)XElement.Parse(f.F.Store.Operations[result.OperationId!.Value].Definition).Attribute("applied"));
		Assert.AreSame(f.Source.Object, f.CasterLocation); Assert.AreSame(f.Destination.Object, f.TargetLocation);
		Assert.AreEqual(3, f.Moves); Assert.AreEqual(67.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(2, f.AlterationQuantity);
		Assert.AreEqual(0, f.F.Samples);
	}

	[TestMethod]
	public void ExchangePlaces_ContactExpiryDuringFirstNativeMoveDoesNotRetargetOrCancelSecondMove()
	{
		var f = new ExchangeFixture { SeverContactAfterFirstMove = true };
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreSame(f.Destination.Object, f.CasterLocation); Assert.AreSame(f.Source.Object, f.TargetLocation); Assert.AreEqual(2, f.Moves);
	}

	[TestMethod]
	public void ExchangePlaces_ArrivalAndRollbackCallbackFailuresIndependentlyCompensateBothBodies()
	{
		var f = new ExchangeFixture { ThrowAfterTargetArrival = true };
		var result = f.Cast(1);
		Assert.AreEqual("false", (string?)XElement.Parse(f.F.Store.Operations[result.OperationId!.Value].Definition).Attribute("applied"));
		Assert.AreSame(f.Source.Object, f.CasterLocation); Assert.AreSame(f.Destination.Object, f.TargetLocation);
		CollectionAssert.AreEquivalent(new[] { f.F.Actor.Object }, f.SourceMembers);
		CollectionAssert.AreEquivalent(new[] { f.Target.Object }, f.DestinationMembers);
		CollectionAssert.AreEquivalent(new[] { f.F.Actor.Object, f.Target.Object }, f.ZoneMembers);
		Assert.AreEqual(4, f.Moves);
		Assert.AreEqual(67.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(2, f.AlterationQuantity);
		Assert.AreEqual(0, f.F.Samples);
	}

	[TestMethod]
	public void ExchangePlaces_UnrepairedDepartureGapQuarantinesInsteadOfReportingNoChange()
	{
		// An uncooperative movement seam leaves a gap which the real native receipt
		// normally repairs. A location pointer alone must never certify NoChange.
		var f = new ExchangeFixture { ThrowAfterFirstLeave = true };
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreSame(f.Source.Object, f.CasterLocation);
		Assert.IsFalse(f.SourceMembers.Any(x => ReferenceEquals(x, f.F.Actor.Object)));
		Assert.AreEqual(0, f.F.Samples); Assert.AreEqual(67.0, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(2, f.AlterationQuantity);
	}

	[DataTestMethod]
	[DataRow("open")] [DataRow("closed")] [DataRow("foreign")] [DataRow("room")]
	[DataRow("moved")] [DataRow("closing")] [DataRow("consumedPouch")]
	public void ExchangePlaces_RetainedComponentRequiresExactAccessibleCarriedPouch(string condition)
	{
		var f = new ExchangeFixture(); var pouch = f.PutRetainedInPouch();
		if (condition == "closed") f.PouchOpen = false;
		if (condition == "foreign") pouch.SetupGet(x => x.InInventoryOf).Returns(f.TargetBody.Object);
		if (condition == "room") pouch.SetupGet(x => x.InInventoryOf).Returns((IBody)null!);
		if (condition == "consumedPouch") pouch.Setup(x => x.IsA(f.F.World.Object.Tags.Get(11))).Returns(true);
		f.F.Checkpoint = stage =>
		{
			if (stage != "BeforePayment") return;
			if (condition == "closing") f.PouchOpen = false;
			if (condition == "moved") f.Items.RemoveAll(x => ReferenceEquals(x, pouch.Object));
		};
		var result = f.Cast(1);
		if (condition == "open")
		{
			Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
			Assert.AreEqual(2, f.Moves); Assert.AreEqual(2, f.AlterationQuantity);
			Assert.AreSame(f.Destination.Object, f.CasterLocation);
		}
		else
		{
			Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
			Assert.AreEqual(0, f.Moves); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(3, f.AlterationQuantity);
		}
		Assert.AreEqual(3, f.TeleportationQuantity);
	}

	[TestMethod]
	public void ExchangePlaces_IncompleteRollbackQuarantinesPaidOriginWithoutMasteryOrReplay()
	{
		var f = new ExchangeFixture { RefuseTargetMove = true, RefuseCasterRollback = true };
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreSame(f.Destination.Object, f.CasterLocation); Assert.AreEqual(0, f.F.Samples);
		Assert.AreEqual(67.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(2, f.AlterationQuantity);
		var moves = f.Moves;
		Assert.AreEqual(MagicCastingStatus.Refused, f.F.Service.Cast(f.Intent(1) with { OriginId = result.OperationId }).Status);
		Assert.AreEqual(moves, f.Moves);
	}

	[TestMethod]
	public void ExchangePlaces_SecondMoveCallbackPolicyChangeStopsAndRestoresWithoutMastery()
	{
		var f = new ExchangeFixture(); var outdoors = RoomOutdoorsType.Outdoors;
		Mock.Get(f.Destination.Object.CurrentOverlay).SetupGet(x => x.OutdoorsType).Returns(() => outdoors);
		f.BeforeTargetMove = () => outdoors = RoomOutdoorsType.Indoors;
		var result = f.Cast(1);
		Assert.AreEqual("false", (string?)XElement.Parse(f.F.Store.Operations[result.OperationId!.Value].Definition).Attribute("applied"));
		Assert.AreSame(f.Source.Object, f.CasterLocation); Assert.AreSame(f.Destination.Object, f.TargetLocation);
		Assert.AreEqual(3, f.Moves); Assert.AreEqual(0, f.F.Samples);
		Assert.AreEqual(67.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(2, f.AlterationQuantity);
	}

	[DataTestMethod]
	[DataRow("outdoors")] [DataRow("roomtag")]
	public void ExchangePlaces_WardCallbackCannotChangeAnAlreadyCheckedRoomPolicyBeforePayment(string change)
	{
		var f = new ExchangeFixture(); var armed = false; var tags = new List<ITag>();
		var overlay = Mock.Get(f.Destination.Object.CurrentOverlay); var outdoors = RoomOutdoorsType.Outdoors;
		overlay.SetupGet(x => x.OutdoorsType).Returns(() => outdoors);
		f.Destination.Setup(x => x.OutdoorsType(It.IsAny<IPerceiver>())).Returns(() => overlay.Object.OutdoorsType);
		f.Destination.SetupGet(x => x.Tags).Returns(tags);
		f.Destination.Setup(x => x.IsA(It.IsAny<ITag>())).Returns<ITag>(tag => tags.Contains(tag));
		var ward = new Mock<IMagicInterdictionEffect>(); var effect = ward.As<IEffect>();
		ward.SetupGet(x => x.Coverage).Returns(MagicInterdictionCoverage.Incoming);
		ward.Setup(x => x.ShouldInterdict(It.IsAny<ICharacter>(), It.IsAny<IMagicSchool>())).Returns(() =>
		{
			if (armed)
			{
				if (change == "outdoors") outdoors = RoomOutdoorsType.Indoors;
				else if (!tags.Contains(f.Restricted)) tags.Add(f.Restricted);
			}
			return false;
		});
		f.Destination.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([ward.Object]);
		f.Destination.SetupGet(x => x.Effects).Returns([effect.Object]);
		f.F.Checkpoint = stage => { if (stage == "BeforePayment") armed = true; };
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(3, f.AlterationQuantity); Assert.AreEqual(0, f.Moves);
	}

	private sealed class ExchangeFixture
	{
		public MagicCastingFixture F { get; } = new(physicalInstance: true);
		public Mock<IRoom> Source { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IRoom> Destination { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICharacter> Target { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IBody> TargetBody { get; } = new() { DefaultValue = DefaultValue.Mock };
		public List<IEffect> CasterEffects { get; } = [];
		public List<IEffect> TargetEffects { get; } = [];
		public List<IGameItem> Items { get; } = [];
		public List<ICharacter> SourceMembers { get; } = [];
		public List<ICharacter> DestinationMembers { get; } = [];
		public List<ICharacter> ZoneMembers { get; } = [];
		public IGameItem Teleportation { get; }
		public IGameItem Alteration { get; }
		public ITag Restricted { get; }
		public ITag Populated { get; }
		public IRoom CasterLocation { get; set; }
		public IRoom TargetLocation { get; set; }
		public RoomLayer CasterLayer { get; set; }
		public RoomLayer TargetLayer { get; set; }
		public int AlterationQuantity { get; private set; } = 3;
		public int TeleportationQuantity { get; private set; } = 3;
		public int Moves { get; private set; }
		public bool RefuseTargetMove { get; set; }
		public bool RefuseCasterRollback { get; set; }
		public bool PouchOpen { get; set; } = true;
		public bool SeverContactAfterFirstMove { get; set; }
		public bool ThrowAfterFirstLeave { get; set; }
		public bool ThrowAfterCasterArrival { get; set; }
		public bool ThrowAfterTargetArrival { get; set; }
		public Action? BeforeTargetMove { get; set; }
		public ExchangeFixture(int rank = 5)
		{
			Target.As<ICharacterInstance>();
			CasterLocation = Source.Object; TargetLocation = Destination.Object;
			F.Actor.SetupGet(x => x.Identity).Returns(Mock.Of<ICharacterIdentity>(x => x.Id == 100 && x.Instances == new[] { (ICharacterInstance)F.Actor.Object }));
			F.Body.SetupGet(x => x.Actor).Returns(F.Actor.Object);
			F.Actor.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
			F.Actor.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			Target.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			F.Actor.SetupGet(x => x.Combat).Returns((MudSharp.Combat.ICombat)null!);
			F.Actor.SetupGet(x => x.RidingMount).Returns((ICharacter)null!);
			Target.SetupGet(x => x.RidingMount).Returns((ICharacter)null!);
			F.Actor.SetupGet(x => x.Location).Returns(() => CasterLocation); F.Actor.SetupGet(x => x.RoomLayer).Returns(() => CasterLayer);
			F.Actor.SetupGet(x => x.Effects).Returns(CasterEffects);
			F.World.SetupGet(x => x.HeartbeatManager).Returns(Mock.Of<IHeartbeatManager>());
			F.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => new[] { CreateProg(0, true).Object, CreateProg(1, true).Object }));
			var zone = CreateZone(1).Object;
			Mock.Get(zone).SetupGet(x => x.Characters).Returns(ZoneMembers);
			var shard = new Mock<IShard>(); shard.SetupGet(x => x.Characters).Returns(ZoneMembers);
			Mock.Get(zone).SetupGet(x => x.Shard).Returns(shard.Object);
			SourceMembers.Add(F.Actor.Object); DestinationMembers.Add(Target.Object);
			ZoneMembers.AddRange([F.Actor.Object, Target.Object]);
			foreach (var (room, id) in new[] { (Source, 301), (Destination, 302) })
			{
				room.SetupGet(x => x.Id).Returns(id); room.SetupGet(x => x.Gameworld).Returns(F.World.Object); room.SetupGet(x => x.Zone).Returns(zone);
				room.SetupGet(x => x.RouteDefinition).Returns((IRouteRoomDefinition)null!);
				room.SetupGet(x => x.OwningZone).Returns(zone);
				room.SetupGet(x => x.Characters).Returns(ReferenceEquals(room, Source) ? SourceMembers : DestinationMembers);
				room.Setup(x => x.OutdoorsType(It.IsAny<IPerceiver>())).Returns(RoomOutdoorsType.Outdoors);
				Mock.Get(room.Object.CurrentOverlay.Package).SetupGet(x => x.Status).Returns(RevisionStatus.Current);
				var terrain = Mock.Of<ITerrain>(x => x.TerrainLayers == new[] { RoomLayer.GroundLevel, RoomLayer.HighInAir });
				room.Setup(x => x.Terrain(It.IsAny<IPerceiver>())).Returns(terrain);
				Mock.Get(room.Object.CurrentOverlay).SetupGet(x => x.Terrain).Returns(terrain);
			}
			Target.SetupGet(x => x.Id).Returns(101); Target.SetupGet(x => x.InstanceId).Returns(101);
			Target.SetupGet(x => x.Identity).Returns(Mock.Of<ICharacterIdentity>(x => x.Id == 101 && x.Instances == new[] { (ICharacterInstance)Target.Object }));
			Target.SetupGet(x => x.Gameworld).Returns(F.World.Object); Target.SetupGet(x => x.Location).Returns(() => TargetLocation);
			Target.SetupGet(x => x.RoomLayer).Returns(() => TargetLayer); Target.SetupGet(x => x.Body).Returns(TargetBody.Object);
			Target.SetupGet(x => x.State).Returns(CharacterState.Awake); Target.SetupGet(x => x.Effects).Returns(TargetEffects);
			Target.Setup(x => x.HasKeyword("traveller", F.Actor.Object, true, false)).Returns(true);
			TargetBody.SetupGet(x => x.Actor).Returns(Target.Object); TargetBody.SetupGet(x => x.Id).Returns(201);
			TargetBody.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			F.World.SetupGet(x => x.Actors).Returns(MagicCastingFixture.Collection(() => new[] { F.Actor.Object, Target.Object }));
			F.World.SetupGet(x => x.Bodies).Returns(MagicCastingFixture.Collection<IBody>(() => []));
			F.World.SetupGet(x => x.Rooms).Returns(MagicCastingFixture.Collection(() => new[] { Source.Object, Destination.Object }));
			var model = CreateConnectMindModel(0); var definition = XElement.Parse(model.Definition);
			definition.SetElementValue("PowerDistance", (int)MagicPowerDistance.SameZoneOnly); model.Definition = definition.ToString();
			var contact = new ConnectMindEffect(F.Actor.Object, Target.Object, (ConnectMindPower)MagicPowerFactory.LoadPower(model, F.World.Object));
			CasterEffects.Add(contact); TargetEffects.Add(contact.ChildEffect);
			F.Actor.Setup(x => x.EffectsOfType<ConnectMindEffect>(It.IsAny<Predicate<ConnectMindEffect>>()))
				.Returns<Predicate<ConnectMindEffect>>(predicate => CasterEffects.OfType<ConnectMindEffect>().Where(x => predicate is null || predicate(x)).ToArray());
			var tags = new List<ITag>();
			ITag[] Ranks(long start) => Enumerable.Range(0, 6).Select(index =>
			{
				var tag = new Mock<ITag>(); tag.SetupGet(x => x.Id).Returns(start + index); tag.SetupGet(x => x.Name).Returns($"rank {start + index}");
				tag.Setup(x => x.IsA(It.IsAny<ITag>())).Returns<ITag>(other => other.Id >= start && other.Id <= start + index);
				tags.Add(tag.Object); return tag.Object;
			}).ToArray();
			var retained = Ranks(1); var consumed = Ranks(11);
			for (var index = 1; index < 6; index++) { Mock.Get(retained[index]).SetupGet(x => x.Parent).Returns(retained[index - 1]); Mock.Get(consumed[index]).SetupGet(x => x.Parent).Returns(consumed[index - 1]); }
			Restricted = Mock.Of<ITag>(x => x.Id == 21); Populated = Mock.Of<ITag>(x => x.Id == 22); tags.Add(Restricted); tags.Add(Populated);
			F.World.SetupGet(x => x.Tags).Returns(MagicCastingFixture.Collection(() => tags));
			IGameItem Component(long id, ITag tag, bool alteration)
			{
				var item = new Mock<IGameItem>(); var stack = new Mock<IStackable>();
				var proto = Mock.Of<IGameItemProto>(x => x.Id == id && x.Tags == new[] { tag });
				item.SetupGet(x => x.Id).Returns(id); item.SetupGet(x => x.Gameworld).Returns(F.World.Object); item.SetupGet(x => x.Prototype).Returns(proto);
				item.SetupGet(x => x.Tags).Returns(new[] { tag }); item.Setup(x => x.IsA(It.IsAny<ITag>())).Returns<ITag>(other => tag.IsA(other));
				item.Setup(x => x.GetItemType<IStackable>()).Returns(stack.Object); item.SetupGet(x => x.Components).Returns(new[] { stack.Object });
				item.SetupGet(x => x.InInventoryOf).Returns(F.Body.Object); item.SetupGet(x => x.ContainedIn).Returns((IGameItem)null!);
				item.SetupGet(x => x.Quantity).Returns(() => alteration ? AlterationQuantity : TeleportationQuantity);
				item.SetupGet(x => x.Deleted).Returns(false); item.SetupGet(x => x.Destroyed).Returns(false); item.SetupGet(x => x.DeepItems).Returns(new[] { item.Object });
				item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(other => ReferenceEquals(other, item.Object));
				stack.SetupGet(x => x.Quantity).Returns(() => alteration ? AlterationQuantity : TeleportationQuantity);
				stack.SetupSet(x => x.Quantity = It.IsAny<int>()).Callback<int>(value => { if (alteration) AlterationQuantity = value; else TeleportationQuantity = value; });
				Items.Add(item.Object); return item.Object;
			}
			Teleportation = Component(401, retained[rank], false); Alteration = Component(402, consumed[rank], true);
			F.World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection(() => Items));
			F.Body.SetupGet(x => x.DirectItems).Returns(Items); F.Actor.SetupGet(x => x.Inventory).Returns(Items);
			F.Body.SetupGet(x => x.HeldItems).Returns(Items); F.Body.SetupGet(x => x.HeldOrWieldedItems).Returns(Items);
			F.Body.SetupGet(x => x.WieldedItems).Returns([]); F.Body.SetupGet(x => x.WornItems).Returns([]);
			var stock = ArmageddonExchangePlacesSpellContent.Create(21, 22, retained.Select(x => x.Id).ToArray(), consumed.Select(x => x.Id).ToArray()).Content;
			var row = stock.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2; row.Definition = stock.BuildDefinition(10, 1, 0).ToString();
			F.Spells.Clear(); var spell = new MagicSpell(row, F.World.Object); spell.EffectDurationExpression = new MudSharp.Body.Traits.TraitExpression("0", F.World.Object); F.Spells.Add(spell);
			F.Skills[1] = 100; F.Acquire(7);
			void Move(bool target, IInvocation call)
			{
				Assert.IsFalse((bool)call.Arguments[2], "No followers may be carried by the pair exchange."); Moves++;
				if (target && (bool)call.Arguments[3]) BeforeTargetMove?.Invoke();
				if (!CommandExecutionScope.TryContinue(target ? Target.Object : F.Actor.Object)) return;
				if (target && RefuseTargetMove) return;
				if (!target && Moves > 1 && RefuseCasterRollback) return;
				var actor = target ? Target.Object : F.Actor.Object;
				SourceMembers.RemoveAll(x => ReferenceEquals(x, actor)); DestinationMembers.RemoveAll(x => ReferenceEquals(x, actor)); ZoneMembers.RemoveAll(x => ReferenceEquals(x, actor));
				if (!target && Moves == 1 && ThrowAfterFirstLeave) throw new InvalidOperationException("Native leave callback failed before publishing location.");
				if (target) { TargetLocation = (IRoom)call.Arguments[0]; TargetLayer = (RoomLayer)call.Arguments[1]; }
				else { CasterLocation = (IRoom)call.Arguments[0]; CasterLayer = (RoomLayer)call.Arguments[1]; }
				(ReferenceEquals(call.Arguments[0], Source.Object) ? SourceMembers : DestinationMembers).Add(actor);
				ZoneMembers.Add(actor);
				if (!target && Moves == 1 && ThrowAfterCasterArrival) throw new InvalidOperationException("Native arrival callback failed after publishing caster membership.");
				if (target && ThrowAfterTargetArrival) throw new InvalidOperationException("Native arrival callback failed after publishing membership.");
				if (!target && Moves == 1 && SeverContactAfterFirstMove) { CasterEffects.Clear(); TargetEffects.Clear(); }
			}
			F.Actor.Setup(x => x.Teleport(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(), false, It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback(new InvocationAction(call => Move(false, call)));
			Target.Setup(x => x.Teleport(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(), false, It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback(new InvocationAction(call => Move(true, call)));
		}
		public Mock<IGameItem> PutRetainedInPouch()
		{
			var pouch = new Mock<IGameItem>(); var container = new Mock<IContainer>(); var openable = container.As<IOpenable>();
			pouch.SetupGet(x => x.Id).Returns(500); pouch.SetupGet(x => x.Gameworld).Returns(F.World.Object);
			pouch.SetupGet(x => x.Prototype).Returns(Mock.Of<IGameItemProto>());
			pouch.SetupGet(x => x.InInventoryOf).Returns(F.Body.Object);
			pouch.Setup(x => x.GetItemType<IContainer>()).Returns(container.Object); pouch.Setup(x => x.GetItemType<IOpenable>()).Returns(openable.Object);
			pouch.SetupGet(x => x.Components).Returns([container.Object]); container.SetupGet(x => x.Contents).Returns([Teleportation]);
			openable.SetupGet(x => x.IsOpen).Returns(() => PouchOpen);
			Items.Add(pouch.Object);
			Mock.Get(Teleportation).SetupGet(x => x.ContainedIn).Returns(pouch.Object);
			Mock.Get(Teleportation).SetupGet(x => x.InInventoryOf).Returns((IBody)null!);
			F.Body.SetupGet(x => x.DirectItems).Returns(() => Items.Where(x => !ReferenceEquals(x, Teleportation)).ToArray());
			F.Body.SetupGet(x => x.HeldItems).Returns([Alteration]); F.Body.SetupGet(x => x.HeldOrWieldedItems).Returns([Alteration]);
			F.Body.SetupGet(x => x.WornItems).Returns([pouch.Object]);
			F.Body.Setup(x => x.CanGet(Teleportation, pouch.Object, 1, It.IsAny<ItemCanGetIgnore>())).Returns(true);
			return pouch;
		}
		public MagicCastingIntent Intent(int grade) => new(F.Actor.Object, F.Earth.Id, 1, grade, false, "traveller", OriginId: Guid.NewGuid());
		public MagicCastingResult Cast(int grade) => F.Service.Cast(Intent(grade));
	}
}
