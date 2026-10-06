#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.GameItems.Interfaces;
using MudSharp.Form.Shape;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StormSpearComponentTests
{
	private sealed class PlacementEffect(XElement root, IMagicSpell spell) : CreateItemEffect(root, spell);
	private sealed class Fixture
	{
		public All<ITag> Tags { get; } = new();
		public ITag[] Ranks { get; }
		public Mock<IBody> Body { get; } = new();
		public Mock<ICharacter> Actor { get; } = new();
		public IFuturemud World { get; }

		public Fixture()
		{
			Ranks = Enumerable.Range(0, 5).Select(rank =>
			{
				var tag = new Mock<ITag>();
				tag.SetupGet(x => x.Id).Returns(rank + 1);
				tag.SetupGet(x => x.Name).Returns($"Creation {rank}");
				tag.Setup(x => x.IsA(It.IsAny<ITag>())).Returns((ITag other) => other.Id <= rank + 1);
				Tags.Add(tag.Object);
				return tag.Object;
			}).ToArray();
			World = Mock.Of<IFuturemud>(x => x.Tags == Tags);
			Actor.SetupGet(x => x.Body).Returns(Body.Object);
			Body.SetupGet(x => x.HeldItems).Returns([]);
			Body.SetupGet(x => x.WieldedItems).Returns([]);
			Body.SetupGet(x => x.WornItems).Returns([]);
			Actor.SetupGet(x => x.Inventory).Returns([]);
		}

		public InventoryPlanActionConsume Action(bool ranked = true)
		{
			var action = new InventoryPlanActionConsume(World, 1, Ranks[0].Id, 0, null!, null!) { CarriedOnly = true };
			if (ranked) action.ConfigureGradeRanks(-3, Ranks);
			return action;
		}

		public Mock<IGameItem> Item(int rank, bool carried = true)
		{
			var item = new Mock<IGameItem>();
			item.Setup(x => x.IsA(It.IsAny<ITag>())).Returns((ITag tag) => tag.Id <= rank + 1);
			item.SetupGet(x => x.InInventoryOf).Returns(carried ? Body.Object : null);
			item.SetupGet(x => x.DeepItems).Returns([item.Object]);
			return item;
		}
	}

	private static void Bind(InventoryPlanActionConsume action, int grade) =>
		typeof(InventoryPlanActionConsume).GetMethod("BindSelectedGrade", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(action, [grade]);

	[DataTestMethod]
	[DataRow(1, 0)]
	[DataRow(2, 0)]
	[DataRow(3, 0)]
	[DataRow(4, 1)]
	[DataRow(5, 2)]
	[DataRow(6, 3)]
	[DataRow(7, 4)]
	public void ScoutTarget_SerializedRankBinding_RejectsLowRankAndAcceptsSufficientDirectMaterial(int grade, int minimum)
	{
		var f = new Fixture();
		var action = new InventoryPlanActionConsume(f.Action().SaveToXml(), f.World);
		Bind(action, grade);
		var wrongScope = f.Item(4, false);
		var low = f.Item(Math.Max(0, minimum - 1));
		if (minimum == 0) low.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(false);
		var sufficient = f.Item(minimum);
		f.Body.SetupGet(x => x.HeldItems).Returns([wrongScope.Object, low.Object, sufficient.Object]);
		Assert.AreSame(sufficient.Object, action.ScoutTarget(f.Actor.Object));
		wrongScope.Verify(x => x.Delete(), Times.Never);
		low.Verify(x => x.Delete(), Times.Never);
	}

	[TestMethod]
	public void ScoutTarget_UnboundRankTemplate_FailsClosedAndClonesDoNotShareBinding()
	{
		var f = new Fixture(); var template = f.Action();
		var candidate = f.Item(0); f.Body.SetupGet(x => x.HeldItems).Returns([candidate.Object]);
		Assert.IsNull(template.ScoutTarget(f.Actor.Object));
		var low = new InventoryPlanActionConsume(template.SaveToXml(), f.World);
		var high = new InventoryPlanActionConsume(template.SaveToXml(), f.World);
		Bind(low, 1); Bind(high, 7);
		Assert.AreSame(candidate.Object, low.ScoutTarget(f.Actor.Object));
		Assert.IsNull(high.ScoutTarget(f.Actor.Object));
		Assert.IsNull(template.ScoutTarget(f.Actor.Object));
	}

	[TestMethod]
	public void ConfigureGradeRanks_InvalidReplacement_PreservesPreviousDefinition()
	{
		var f = new Fixture(); var action = f.Action(); var before = action.SaveToXml().ToString();
		Assert.ThrowsException<FormatException>(() => action.ConfigureGradeRanks(-2, f.Ranks));
		Assert.AreEqual(before, action.SaveToXml().ToString());
		Assert.ThrowsException<InvalidOperationException>(() => action.ConfigureGradeRanks(-3, f.Ranks.Reverse().ToArray()));
		Assert.AreEqual(before, action.SaveToXml().ToString());
		action.ClearGradeRanks();
		Assert.IsNull(action.SaveToXml().Element("GradeRank"));
		Assert.IsTrue(action.CarriedOnly);
	}

	[TestMethod]
	public void BindSelectedGrade_UnresolvedTag_RefusesInsteadOfIgnoringRequirement()
	{
		var f = new Fixture(); var xml = f.Action().SaveToXml();
		xml.Element("GradeRank")!.Elements("Rank").Last().SetAttributeValue("tag", 999);
		var action = new InventoryPlanActionConsume(xml, f.World);
		var error = Assert.ThrowsException<TargetInvocationException>(() => Bind(action, 1));
		Assert.IsInstanceOfType(error.InnerException, typeof(InvalidOperationException));
	}

	[DataTestMethod]
	[DataRow("scope")]
	[DataRow("rank")]
	[DataRow("contained")]
	[DataRow("deleted")]
	public void TakeAction_StaleSelectedMaterial_RefusesBeforeDeleting(string change)
	{
		var f = new Fixture(); var action = f.Action(); Bind(action, 7);
		var item = f.Item(4); f.Body.SetupGet(x => x.HeldItems).Returns([item.Object]);
		Assert.AreSame(item.Object, action.ScoutTarget(f.Actor.Object));
		switch (change)
		{
			case "scope": item.SetupGet(x => x.InInventoryOf).Returns((IBody?)null); break;
			case "rank": item.Setup(x => x.IsA(f.Ranks[4])).Returns(false); break;
			case "contained": item.SetupGet(x => x.ContainedIn).Returns(Mock.Of<IGameItem>()); break;
			case "deleted": item.SetupGet(x => x.Deleted).Returns(true); break;
		}
		var plan = new InventoryPlanTemplate(f.World, action);
		var method = typeof(InventoryPlanTemplate).GetMethod("TakeAction", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var error = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(plan,
			[f.Actor.Object, item.Object, null, DesiredItemState.Consumed, action, true]));
		Assert.IsInstanceOfType(error.InnerException, typeof(InvalidOperationException));
		item.Verify(x => x.Delete(), Times.Never);
		item.Verify(x => x.Quit(), Times.Never);
	}

	[TestMethod]
	public void ScoutTarget_LegacyDefinition_DoesNotGainDirectCarriedRestriction()
	{
		var f = new Fixture(); var action = f.Action(false); action.CarriedOnly = false;
		var loose = f.Item(0, false); f.Body.SetupGet(x => x.HeldItems).Returns([loose.Object]);
		var loaded = new InventoryPlanActionConsume(action.SaveToXml(), f.World);
		Assert.IsFalse(loaded.HasPersistedSelection);
		Assert.AreSame(loose.Object, loaded.ScoutTarget(f.Actor.Object));
	}

	[DataTestMethod]
	[DataRow(1, 0)]
	[DataRow(7, 4)]
	public void CastingService_GradeBoundMaterial_QuotesAndPaysUsingInvocationPlan(int grade, int rank)
	{
		var f = new MagicCastingFixture(); var tags = new Fixture();
		f.World.SetupGet(x => x.Tags).Returns(tags.Tags);
		var action = new InventoryPlanActionConsume(f.World.Object, 1, tags.Ranks[0].Id, 0, null!, null!) { CarriedOnly = true };
		action.ConfigureGradeRanks(-3, tags.Ranks);
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.World.Object, action);
		var component = tags.Item(rank);
		component.SetupGet(x => x.Id).Returns(123);
		component.SetupGet(x => x.InInventoryOf).Returns(f.Body.Object);
		f.Body.SetupGet(x => x.HeldItems).Returns([component.Object]);
		f.Body.SetupGet(x => x.WieldedItems).Returns([]);
		f.Body.SetupGet(x => x.WornItems).Returns([]);
		f.Actor.SetupGet(x => x.Inventory).Returns([component.Object]);
		f.Skills[1] = 100; f.Acquire(7);
		var intent = f.Intent(grade, false);
		var quote = f.Service.Quote(intent);
		Assert.IsTrue(quote.Allowed, quote.Reason);
		component.Verify(x => x.Delete(), Times.Never);
		var cast = f.Service.Cast(intent);
		Assert.AreEqual(MagicCastingStatus.Succeeded, cast.Status, cast.Message);
		component.Verify(x => x.Delete(), Times.Once);
		Assert.AreEqual(100 - 5.0 * grade, f.Balances[f.Resources[1]]);
		Assert.IsNull(action.ScoutTarget(f.Actor.Object), "The shared template must remain unbound after casting.");
	}

	[DataTestMethod]
	[DataRow("get")]
	[DataRow("wield")]
	public void PrimaryHandPlacement_CallbackChangesCustody_RefusesAtBoundary(string boundary)
	{
		var body = new Mock<IBody>(); var world = new Mock<IFuturemud>();
		var actor = Mock.Of<ICharacter>(x => x.Body == body.Object);
		var hand = Mock.Of<IWield>(x => x.Id == 1 && x.Alignment == Alignment.Right);
		Mock.Get(hand).Setup(x => x.Hands(It.IsAny<IGameItem>())).Returns(1);
		body.SetupGet(x => x.Handedness).Returns(Alignment.Right);
		body.SetupGet(x => x.WieldLocs).Returns([hand]);
		body.Setup(x => x.CanGet(It.IsAny<IGameItem>(), 0, ItemCanGetIgnore.None)).Returns(true);
		body.Setup(x => x.CanWield(It.IsAny<IGameItem>(), hand, ItemCanWieldFlags.None)).Returns(true);
		var item = new Mock<IGameItem>(); IBody? custody = null;
		item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns((IGameItem other) => ReferenceEquals(other, item.Object));
		item.SetupGet(x => x.InInventoryOf).Returns(() => custody);
		var held = new System.Collections.Generic.List<IGameItem>();
		var wielded = new System.Collections.Generic.List<IGameItem>();
		body.SetupGet(x => x.HeldItems).Returns(held); body.SetupGet(x => x.WieldedItems).Returns(wielded);
		var wieldable = new Mock<IWieldable>(); wieldable.SetupGet(x => x.PrimaryWieldedLocation).Returns(hand);
		item.Setup(x => x.GetItemType<IWieldable>()).Returns(wieldable.Object);
		body.Setup(x => x.Get(item.Object, 0, null, true, ItemCanGetIgnore.None, null))
			.Callback(() => { custody = boundary == "get" ? null : body.Object; if (custody is not null) held.Add(item.Object); })
			.Returns(item.Object);
		body.Setup(x => x.Wield(item.Object, hand, null, true, ItemCanWieldFlags.None))
			.Callback(() => { held.Clear(); wielded.Add(item.Object); custody = null; }).Returns(true);
		var spell = Mock.Of<IMagicSpell>(x => x.Gameworld == world.Object);
		var effect = new PlacementEffect(XElement.Parse("<Effect><ItemQuality>base</ItemQuality><ItemPrototypeId>0</ItemPrototypeId><ItemSkinId>0</ItemSkinId><Quantity>1</Quantity><LoadString/></Effect>"), spell);
		typeof(CreateItemEffect).GetProperty(nameof(CreateItemEffect.PrimaryHand))!.SetValue(effect, true);
		var method = typeof(CreateItemEffect).GetMethod("PlaceOwnedItem", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var error = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(effect, [item.Object, actor, actor]));
		Assert.IsInstanceOfType(error.InnerException, typeof(InvalidOperationException));
		body.Verify(x => x.Wield(item.Object, hand, null, true, ItemCanWieldFlags.None), boundary == "get" ? Times.Never() : Times.Once());
		item.Verify(x => x.Delete(), Times.Never, "A changed-custody output retains its exact lifecycle claim for safe reconciliation.");
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void CastingService_RuntimeSelector_PreservesItsMaterialFilterForLegacyAndRankPlans(bool ranked)
	{
		var f = new MagicCastingFixture(); var tags = new Fixture();
		f.World.SetupGet(x => x.Tags).Returns(tags.Tags);
		var wrong = tags.Item(0); wrong.SetupGet(x => x.Id).Returns(122);
		var right = tags.Item(0); right.SetupGet(x => x.Id).Returns(123);
		f.Body.SetupGet(x => x.HeldItems).Returns([wrong.Object, right.Object]);
		f.Body.SetupGet(x => x.WieldedItems).Returns([]); f.Body.SetupGet(x => x.WornItems).Returns([]);
		f.Actor.SetupGet(x => x.Inventory).Returns([wrong.Object, right.Object]);
		var action = new InventoryPlanActionConsume(f.World.Object, 1, tags.Ranks[0].Id, 0, item => item.Id == 123, null!);
		if (ranked) action.ConfigureGradeRanks(-3, tags.Ranks);
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.World.Object, action);
		f.Acquire(7); f.Skills[1] = 100;
		var result = f.Service.Cast(f.Intent(1, false));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		right.Verify(x => x.Delete(), Times.Once); wrong.Verify(x => x.Delete(), Times.Never);
	}
}
