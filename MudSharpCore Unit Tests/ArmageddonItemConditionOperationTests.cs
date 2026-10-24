#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonItemConditionOperationTests
{
	[TestMethod]
	public void ShatterStone_SelectedGradeDamagesExactStoneItemAndRejectsCustodyDrift()
	{
		var f = new Fixture(ArmageddonItemConditionSpellContent.ShatterStoneKey, 3, MaterialBehaviourType.Stone, false);
		var token = f.Effect.CapturePreparedSelection(f.Cast.Actor.Object, f.Item.Object);
		Assert.IsTrue(((IMagicSpellEffectPreparedSelectionRawToken)token).IsCurrent);
		var packets = new List<IDamage>();
		f.Item.Setup(x => x.SufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(damage =>
		{
			damage.MutationGuard!.Validate(f.Item.Object, damage);
			packets.Add(damage);
			return [];
		});
		var applied = f.Effect.Apply(f.Cast.Actor.Object, f.Item.Object, OpposedOutcomeDegree.None,
			SpellPower.ExtremelyWeak, null!, []);
		Assert.AreEqual(MagicEffectOperationStatus.NoChange, applied.Status); // A blocked native wound does not claim success.
		Assert.AreEqual(1, packets.Count);
		Assert.AreEqual(15.0, packets[0].DamageAmount);
		Assert.AreEqual(DamageType.Crushing, packets[0].DamageType);
		f.Item.SetupGet(x => x.InInventoryOf).Returns(new Mock<IBody>().Object);
		Assert.IsFalse(f.Effect.TryReusePreparedSelection(token, f.Cast.Actor.Object, f.Item.Object, out _));
		Assert.ThrowsExactly<InvalidOperationException>(() => f.Effect.Apply(f.Cast.Actor.Object, f.Item.Object,
			OpposedOutcomeDegree.None, SpellPower.ExtremelyWeak, null!, []));
		Assert.AreEqual(1, packets.Count);
	}

	[TestMethod]
	public void ErodeObject_RefusesNonOrganicMaterialAndContainedItemsBeforeMutation()
	{
		var f = new Fixture(ArmageddonItemConditionSpellContent.ErodeObjectKey, 1, MaterialBehaviourType.Metal, false);
		Assert.ThrowsExactly<InvalidOperationException>(() => f.Effect.CapturePreparedSelection(f.Cast.Actor.Object, f.Item.Object));
		f.Material.SetupGet(x => x.Organic).Returns(true);
		f.Item.SetupGet(x => x.ShallowItems).Returns(new[] { f.Item.Object, new Mock<IGameItem>().Object });
		Assert.ThrowsExactly<InvalidOperationException>(() => f.Effect.CapturePreparedSelection(f.Cast.Actor.Object, f.Item.Object));
		f.Item.Verify(x => x.SufferDamage(It.IsAny<IDamage>()), Times.Never);
	}

	[TestMethod]
	public void WitherGrowth_AcceptsOnlyLocalOrganicPlantItemsAndBoundsNativeDamage()
	{
		var f = new Fixture(ArmageddonItemConditionSpellContent.WitherGrowthKey, 4, MaterialBehaviourType.Wood, true);
		Assert.ThrowsExactly<InvalidOperationException>(() => f.Effect.CapturePreparedSelection(f.Cast.Actor.Object, f.Item.Object));
		f.Material.SetupGet(x => x.BehaviourType).Returns(MaterialBehaviourType.Plant);
		f.Material.SetupGet(x => x.Organic).Returns(false);
		Assert.ThrowsExactly<InvalidOperationException>(() => f.Effect.CapturePreparedSelection(f.Cast.Actor.Object, f.Item.Object));
		f.Material.SetupGet(x => x.Organic).Returns(true);
		f.Effect.CapturePreparedSelection(f.Cast.Actor.Object, f.Item.Object);
		IDamage? delivered = null;
		f.Item.Setup(x => x.SufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(damage =>
		{
			damage.MutationGuard!.Validate(f.Item.Object, damage);
			delivered = damage;
			return [];
		});
		f.Effect.Apply(f.Cast.Actor.Object, f.Item.Object, OpposedOutcomeDegree.None,
			SpellPower.ExtremelyWeak, null!, []);
		Assert.IsNotNull(delivered);
		Assert.AreEqual(20.0, delivered.DamageAmount);
		Assert.AreEqual(DamageType.Chemical, delivered.DamageType);
	}

	[TestMethod]
	public void StoneDamage_BoundsNativeMaterialMultiplierAndRefusesFragileTarget()
	{
		var f = new Fixture(ArmageddonItemConditionSpellContent.ShatterStoneKey, 7, MaterialBehaviourType.Stone, false);
		f.DamageMultiplier = 3;
		f.MaximumDamage = 40;
		f.Effect.CapturePreparedSelection(f.Cast.Actor.Object, f.Item.Object);
		IDamage? delivered = null;
		f.Item.Setup(x => x.SufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(damage =>
		{
			damage.MutationGuard!.Validate(f.Item.Object, damage);
			delivered = damage;
			return [];
		});
		f.Effect.Apply(f.Cast.Actor.Object, f.Item.Object, OpposedOutcomeDegree.None,
			SpellPower.ExtremelyWeak, null!, []);
		Assert.IsNotNull(delivered);
		Assert.AreEqual(35.0 / 3, delivered.DamageAmount, 1e-7);
		Assert.AreEqual(35.0, f.Destroyable.Object.GetActualDamage(delivered).DamageAmount, 1e-7);
		var fragile = new Fixture(ArmageddonItemConditionSpellContent.ShatterStoneKey, 7, MaterialBehaviourType.Stone, false);
		fragile.MaximumDamage = 35;
		Assert.ThrowsExactly<InvalidOperationException>(() => fragile.Effect.CapturePreparedSelection(fragile.Cast.Actor.Object, fragile.Item.Object));
		fragile.Item.Verify(x => x.SufferDamage(It.IsAny<IDamage>()), Times.Never);
	}

	[TestMethod]
	public void RestoreObject_RepairsAtMostGradeBudgetWithoutChangingQualityOrMaterial()
	{
		var f = new Fixture(ArmageddonItemConditionSpellContent.RestoreObjectKey, 3, MaterialBehaviourType.Metal, false);
		var first = new Mock<IWound>(); var second = new Mock<IWound>();
		foreach (var wound in new[] { first, second })
		{
			wound.SetupProperty(x => x.CurrentDamage, 10.0);
			wound.SetupGet(x => x.Repairable).Returns(true);
			wound.SetupGet(x => x.Parent).Returns(f.Item.Object);
		}
		f.Wounds.AddRange([first.Object, second.Object]);
		f.Effect.CapturePreparedSelection(f.Cast.Actor.Object, f.Item.Object);
		var result = f.Effect.Apply(f.Cast.Actor.Object, f.Item.Object, OpposedOutcomeDegree.None,
			SpellPower.ExtremelyWeak, null!, []);
		Assert.AreEqual(MagicEffectOperationStatus.Applied, result.Status);
		Assert.AreEqual(5.0, f.Wounds.Sum(x => x.CurrentDamage));
		Assert.AreEqual(ItemQuality.Standard, f.Item.Object.Quality);
		Assert.AreSame(f.Material.Object, f.Item.Object.Material);
		Assert.AreEqual(0.75, f.Item.Object.Condition);
		f.Item.Verify(x => x.EvaluateWounds(), Times.Once);
		f.Item.Verify(x => x.SufferDamage(It.IsAny<IDamage>()), Times.Never);
	}

	private sealed class Fixture
	{
		public MagicCastingFixture Cast { get; } = new();
		public Mock<IGameItem> Item { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ISolid> Material { get; } = new();
		public Mock<IDestroyable> Destroyable { get; } = new();
		public double DamageMultiplier { get; set; } = 1;
		public double MaximumDamage { get; set; } = 100;
		public List<IWound> Wounds { get; } = [];
		public BoundedItemConditionEffect Effect { get; }

		public Fixture(string key, int grade, MaterialBehaviourType behaviour, bool organic)
		{
			Cast.Body.SetupGet(x => x.Actor).Returns(Cast.Actor.Object);
			var room = Cast.Actor.Object.Location;
			Material.SetupGet(x => x.BehaviourType).Returns(behaviour);
			Material.SetupGet(x => x.Organic).Returns(organic);
			Item.SetupGet(x => x.Id).Returns(91);
			Item.SetupGet(x => x.Gameworld).Returns(Cast.World.Object);
			Item.SetupGet(x => x.Material).Returns(Material.Object);
			Item.SetupGet(x => x.TrueLocations).Returns([room]);
			Item.SetupGet(x => x.InInventoryOf).Returns(Cast.Body.Object);
			Item.SetupGet(x => x.ShallowItems).Returns(() => [Item.Object]);
			Item.SetupGet(x => x.Components).Returns([]);
			Item.SetupGet(x => x.Prototype).Returns(Mock.Of<IGameItemProto>());
			Item.SetupGet(x => x.Wounds).Returns(() => Wounds);
			Item.SetupGet(x => x.Quality).Returns(ItemQuality.Standard);
			Item.SetupGet(x => x.RawQuality).Returns(ItemQuality.Standard);
			Item.SetupGet(x => x.Condition).Returns(0.75);
			Destroyable.SetupGet(x => x.MaximumDamage).Returns(() => MaximumDamage);
			Destroyable.Setup(x => x.GetActualDamage(It.IsAny<IDamage>())).Returns<IDamage>(damage =>
				new Damage(damage) { DamageAmount = damage.DamageAmount * DamageMultiplier });
			Item.Setup(x => x.GetItemType<IDestroyable>()).Returns(Destroyable.Object);
			Cast.World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection(() => new[] { Item.Object }));
			var content = ArmageddonItemConditionSpellContent.Create().Single(x => x.Content.Key == key).Content;
			var spell = Cast.NewSpell(2, content.Name,
				content.BuildDefinition(10, 1, 0).Element("Effects")!.Element("Effect")!.ToString());
			Assert.IsTrue(spell.BuildingCommand(Cast.Actor.Object, new StringStack("trigger new item")));
			Assert.IsTrue(spell.BuildingCommand(Cast.Actor.Object, new StringStack("grades fixture")));
			var copy = (MagicSpell)typeof(MagicSpell).GetMethod("CastingCopy", BindingFlags.NonPublic | BindingFlags.Instance)!
				.Invoke(spell, [Cast.Actor.Object, Cast.Traits[0], grade, SpellPower.ExtremelyWeak, Difficulty.Automatic, 1])!;
			Effect = copy.SpellEffects.OfType<BoundedItemConditionEffect>().Single();
		}
	}
}
