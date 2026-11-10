#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemIntrinsicMagicTests
{
	private sealed class Fixture
	{
		public VancianItemTests.ItemFixture Base { get; } = new();
		public Mock<IGameItem> Item { get; }
		public List<IEffect> Effects { get; } = [];
		public Fixture()
		{
			Item = Base.Item(901);
			Item.SetupGet(x => x.Effects).Returns(() => Effects);
			Item.Setup(x => x.EffectsOfType<IItemPickupAttemptEffect>(null)).Returns(() => Effects.OfType<IItemPickupAttemptEffect>());
			Item.Setup(x => x.EffectsOfType<IIntrinsicItemMagicEffect>(null)).Returns(() => Effects.OfType<IIntrinsicItemMagicEffect>());
			Item.Setup(x => x.EffectsOfType<IDispelMagicProxyEffect>(null)).Returns([]);
			Item.Setup(x => x.EffectsOfType<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>())).Returns([]);
			Item.Setup(x => x.GetItemTypes<IItemInteractionPermissions>()).Returns([]);
			Item.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((e, _) => Effects.Remove(e));
		}
		public MagicEffectOperation Dispel(int grade, bool enabled = true, long spellFilter = 0)
		{
			var xml = new XElement("Effect", new XAttribute("type", "dispelmagic"), new XElement("Mode", 0),
				new XElement("CasterPolicy", 1), new XElement("AllowHostile", true), new XElement("EffectKey", "any"),
				new XElement("SpellId", spellFilter), new XElement("IncludeIntrinsicItemEffects", enabled));
			var effect = (DispelMagicEffect)SpellEffectFactory.LoadEffect(xml, Base.Spell);
			return effect.Apply(Base.F.Actor.Object, Item.Object, OpposedOutcomeDegree.None,
				(SpellPower)((int)SpellPower.ExtremelyWeak + grade - 1), null!, []);
		}
	}

	[TestMethod]
	public void PickupHazard_StopsOneActualPickupAndPersistsItsSpentState()
	{
		var f = new Fixture();
		var hazard = new ItemPickupHazardEffect(f.Item.Object, "0", "0", minimumDispelGrade: 3);
		f.Effects.Add(hazard);
		Assert.IsFalse(ItemPickupAttempt.TryRun(f.Item.Object, f.Base.F.Actor.Object, () => true));
		Assert.IsTrue(ItemPickupAttempt.TryRun(f.Item.Object, f.Base.F.Actor.Object, () => true));
		Assert.IsFalse(hazard.IsIntrinsicMagicActive);
		ItemPickupHazardEffect.InitialiseEffectType();
		var restored = (ItemPickupHazardEffect)Effect.LoadEffect(hazard.SaveToXml(new Dictionary<IEffect, TimeSpan>()), f.Item.Object);
		Assert.IsFalse(restored.IsIntrinsicMagicActive);
		Assert.IsTrue(restored.HandlePickupAttempt(f.Base.F.Actor.Object));
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void PickupHazard_NativeItemCopyPreservesArmedAndSpentState(bool spent)
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var prototype = new Mock<IGameItemProto> { DefaultValue = DefaultValue.Mock };
		prototype.SetupGet(x => x.Gameworld).Returns(world.Object);
		prototype.SetupGet(x => x.Name).Returns("goods");
		var original = new GameItem(prototype.Object);
		var hazard = new ItemPickupHazardEffect(original, "0", "0", minimumDispelGrade: 3);
		original.AddEffect(hazard);
		var initialisation = new ItemInitialisation(original);
		Assert.IsTrue(initialisation.TryInitialise("flag:magic"));
		Assert.IsTrue(initialisation.TryInitialise("flag:invisible")); original.AddEffect(initialisation);
		original.AddEffect(new DetectionVisibility(original, PerceptionTypes.VisualMagical));
		original.AddEffect(new ItemMagicAura(original));
		if (spent) Assert.IsTrue(hazard.TryDispel(3));
		var copy = new GameItem(original, temporary: false);
		var copied = copy.EffectsOfType<ItemPickupHazardEffect>().Single();
		Assert.AreEqual(1, copy.EffectsOfType<DetectionVisibility>().Count());
		Assert.AreEqual(1, copy.EffectsOfType<ItemMagicAura>().Count());
		Assert.AreNotSame(hazard, copied);
		Assert.AreEqual(!spent, copied.IsIntrinsicMagicActive);
		Assert.AreEqual(3, copied.MinimumDispelGrade);
		Assert.IsFalse(copy.EffectsOfType<ItemInitialisation>().Single().TryInitialise("flag:magic"), "A split must not restore cleansed initial magic on its next load.");
	}

	[TestMethod]
	public void CleansedPresentation_CannotMergeBackIntoAnEnchantedStack()
	{
		var f = new Fixture(); var plain = f.Base.Item(999);
		plain.SetupGet(x => x.Effects).Returns([]);
		foreach (var effect in new IEffect[] { new ItemMagicAura(f.Item.Object), new ItemPresentation(f.Item.Object, false),
			new ItemPresentation(f.Item.Object, true), new DetectionVisibility(f.Item.Object, PerceptionTypes.VisualMagical) })
			Assert.IsTrue(effect.PreventsItemFromMerging(f.Item.Object, plain.Object));
	}

	[TestMethod]
	public void PickupHazard_InvalidImmunityDoesNotConsumeOrPermitPickup()
	{
		var f = new Fixture();
		var hazard = new ItemPickupHazardEffect(f.Item.Object, "0", "0", immunityProgId: 123);
		Assert.IsFalse(hazard.HandlePickupAttempt(f.Base.F.Actor.Object));
		Assert.IsTrue(hazard.IsIntrinsicMagicActive);
	}

	[TestMethod]
	public void PickupHazard_ImmunityPermitsPickupWithoutSpendingTheGlyph()
	{
		var f = new Fixture(); var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(123);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		object result = true;
		prog.Setup(x => x.ExecuteWithStatus(out result, It.IsAny<object[]>())).Returns(true);
		var progs = new All<IFutureProg>(); progs.Add(prog.Object); f.Base.F.World.SetupGet(x => x.FutureProgs).Returns(progs);
		var hazard = new ItemPickupHazardEffect(f.Item.Object, "0", "0", immunityProgId: 123); f.Effects.Add(hazard);
		Assert.IsTrue(ItemPickupAttempt.TryRun(f.Item.Object, f.Base.F.Actor.Object, () => true));
		Assert.IsTrue(hazard.IsIntrinsicMagicActive);
	}

	private delegate bool ExecutePolicy(out object result, object[] arguments);
	[TestMethod]
	public void PickupHazard_ImmunityCallbackCanDispelWithoutRetriggeringTheSpentGlyph()
	{
		var f = new Fixture(); var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(123); prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		var hazard = new ItemPickupHazardEffect(f.Item.Object, "0", "0", immunityProgId: 123, minimumDispelGrade: 3);
		f.Effects.Add(hazard);
		object result = false;
		prog.Setup(x => x.ExecuteWithStatus(out result, It.IsAny<object[]>())).Returns(new ExecutePolicy((out object value, object[] _) =>
		{
			Assert.IsTrue(hazard.TryDispel(3)); value = false; return true;
		}));
		var progs = new All<IFutureProg>(); progs.Add(prog.Object); f.Base.F.World.SetupGet(x => x.FutureProgs).Returns(progs);
		Assert.IsTrue(ItemPickupAttempt.TryRun(f.Item.Object, f.Base.F.Actor.Object, () => true));
		Assert.IsFalse(hazard.IsIntrinsicMagicActive);
	}

	[TestMethod]
	public void PickupHazard_CustodyFencePreventsInvocation()
	{
		var f = new Fixture(); var hazard = new ItemPickupHazardEffect(f.Item.Object, "0", "0"); f.Effects.Add(hazard);
		Assert.IsFalse(ItemPickupAttempt.TryRun(f.Item.Object, f.Base.F.Actor.Object, () => false));
		Assert.IsTrue(hazard.IsIntrinsicMagicActive);
	}

	[TestMethod]
	public void LowGradeDispel_RemovesInvisibilityThenStopsAtGlyph()
	{
		var f = new Fixture();
		var invisible = new DetectionVisibility(f.Item.Object, MudSharp.Effects.PerceptionTypes.VisualMagical);
		var glyph = new ItemPickupHazardEffect(f.Item.Object, "0", "0", minimumDispelGrade: 3);
		var glow = new ItemPresentation(f.Item.Object, false);
		f.Effects.AddRange([glow, glyph, invisible]);
		Assert.AreEqual(MagicEffectOperationStatus.Applied, f.Dispel(2).Status);
		Assert.IsFalse(f.Effects.Contains(invisible)); Assert.IsTrue(f.Effects.Contains(glow)); Assert.IsTrue(glyph.IsIntrinsicMagicActive);
		Assert.AreEqual(MagicEffectOperationStatus.Applied, f.Dispel(3).Status);
		Assert.IsFalse(glyph.IsIntrinsicMagicActive); Assert.IsFalse(f.Effects.Contains(glow));
	}

	[TestMethod]
	[DataRow(false, 0L)]
	[DataRow(true, 555L)]
	public void NativeDefaultAndScopedDispels_DoNotStripIntrinsicMagic(bool enabled, long filter)
	{
		var f = new Fixture(); var glyph = new ItemPickupHazardEffect(f.Item.Object, "0", "0"); f.Effects.Add(glyph);
		Assert.AreEqual(MagicEffectOperationStatus.NoChange, f.Dispel(7, enabled, filter).Status);
		Assert.IsTrue(glyph.IsIntrinsicMagicActive);
	}
}
