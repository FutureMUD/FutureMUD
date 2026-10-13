#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Character;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonScalarUtilityContentTests
{
	[DataTestMethod]
	[DataRow(1, SpellPower.ExtremelyStrong)]
	[DataRow(7, SpellPower.ExtremelyWeak)]
	[DataRow(3, SpellPower.VeryStrong)]
	public void CoolingShade_NativeChildrenAndColdReloadRespectSelectedGrade(int grade, SpellPower power)
	{
		var f = new MagicCastingFixture();
		var content = ArmageddonScalarUtilityContent.Create()[0].Content;
		var spell = Load(f, content);
		var original = spell.SpellEffects.Select(x => x.SaveToXml()).ToArray();
		var copy = spell.CastingCopy(f.Actor.Object, f.Traits[0], grade, power, Difficulty.Normal);
		Assert.AreEqual(60.0 * grade, copy.EffectDurationExpression.Evaluate(f.Actor.Object));
		Assert.AreEqual(0, copy.GradeConfigurationErrors().Count);
		var parent = new MagicSpellParent(f.Actor.Object.Location, copy, f.Actor.Object, power);
		var children = copy.SpellEffects.Select(x => x.GetOrApplyEffect(f.Actor.Object, f.Actor.Object.Location,
			OpposedOutcomeDegree.None, power, parent, [])).ToArray();
		Assert.IsInstanceOfType(children[0], typeof(SpellRoomLightEffect));
		Assert.IsInstanceOfType(children[1], typeof(SpellRoomTemperatureEffect));
		Assert.AreEqual(-10.0 * grade, ((IAreaLightEffect)children[0]!).AddedLight, 1e-10);
		Assert.AreEqual(-0.5 * grade, ((IAffectEnvironmentalTemperature)children[1]!).TemperatureDelta, 1e-10);
		MagicSpellParent.InitialiseEffectType(); SpellRoomLightEffect.InitialiseEffectType(); SpellRoomTemperatureEffect.InitialiseEffectType();
		foreach (var child in children) parent.AddSpellEffect(child!);
		var saved = parent.SaveToXml(new() { [parent] = TimeSpan.FromSeconds(60 * grade) });
		var cold = saved.Element("Effect")!.Element("Children")!.Elements().Last();
		Assert.AreEqual("SpellRoomTemperature", cold.Element("Type")!.Value);
		var loaded = (IAffectEnvironmentalTemperature)Effect.LoadEffect(cold, f.Actor.Object.Location);
		Assert.AreEqual(-0.5 * grade, loaded.TemperatureDelta, 1e-10);
		var loadedParent = (MagicSpellParent)Effect.LoadEffect(saved, f.Actor.Object.Location);
		Assert.AreEqual(2, loadedParent.SpellEffects.Count());
		Assert.AreEqual(-0.5 * grade, ((IAffectEnvironmentalTemperature)loadedParent.SpellEffects.Last()).TemperatureDelta, 1e-10);
		Assert.AreEqual(60000L * grade, (long)saved.Element("Remaining")!);
		foreach (var pair in spell.SpellEffects.Zip(original))
		{
			Assert.IsTrue(XNode.DeepEquals(pair.First.SaveToXml(), pair.Second));
			Assert.IsTrue(XNode.DeepEquals(pair.First.Clone().SaveToXml(), pair.Second));
		}
	}

	[DataTestMethod]
	[DataRow(1, SpellPower.ExtremelyStrong)]
	[DataRow(7, SpellPower.ExtremelyWeak)]
	public void Kindle_NativeWeaponEnhancementChangesOnlyAttackBonus(int grade, SpellPower power)
	{
		var f = new MagicCastingFixture(); var content = ArmageddonScalarUtilityContent.Create()[1].Content;
		var spell = Load(f, content);
		Assert.IsTrue(XNode.DeepEquals(spell.SpellEffects.Single().SaveToXml(), spell.SpellEffects.Single().Clone().SaveToXml()));
		var copy = spell.CastingCopy(f.Actor.Object, f.Traits[0], grade, power, Difficulty.Normal);
		Assert.IsTrue(content.SpellRow(1, 1, 1).AppliedEffectsAreExclusive);
		Assert.AreEqual(60.0 * grade, copy.EffectDurationExpression.Evaluate(f.Actor.Object));
		var item = NativeItem(f, true);
		var parent = new MagicSpellParent(item, copy, f.Actor.Object, power);
		var child = (SpellItemEnchantmentEffect)copy.SpellEffects.Single().GetOrApplyEffect(f.Actor.Object,
			item, OpposedOutcomeDegree.None, power, parent, [])!;
		var hook = (IMagicWeaponEnhancementEffect)child;
		Assert.AreEqual((double)grade, hook.AttackCheckBonus);
		Assert.IsTrue(hook.AppliesToWeaponAttack(f.Actor.Object, f.Actor.Object, item));
		Assert.IsFalse(hook.AppliesToWeaponAttack(f.Actor.Object, f.Actor.Object, Mock.Of<IGameItem>()));
		Assert.AreEqual(0.0, hook.QualityBonus); Assert.AreEqual(0.0, hook.DamageBonus);
		Assert.AreEqual(0.0, hook.PainBonus); Assert.AreEqual(0.0, hook.StunBonus);
		item.AddEffect(child);
		Assert.AreSame(f.Body.Object, item.GetItemType<IWearable>().WornBy);
		Assert.AreEqual(ItemQuality.Standard, item.Quality); Assert.AreEqual(ItemQuality.Standard, item.RawQuality);
		SpellItemEnchantmentEffect.InitialiseEffectType();
		var loaded = (IMagicWeaponEnhancementEffect)Effect.LoadEffect(child.SaveToXml(new()), item);
		Assert.AreEqual((double)grade, loaded.AttackCheckBonus); Assert.AreEqual(0.0, loaded.QualityBonus);
	}

	[TestMethod]
	public void Content_InventoryFloorsTypedAdmissionAndLegacyUnboundFieldsRemainExact()
	{
		foreach (var contribution in ArmageddonScalarUtilityContent.Create())
		{
			var f = new MagicCastingFixture(); var content = contribution.Content;
			Assert.AreEqual(content.Key.EndsWith("cooling_shade") ? 20.0 : 33.0, content.MinimumEnergy);
			Assert.AreEqual(content.MinimumEnergy + "*grade", content.CostRow(1).Expression);
			var spell = Load(f, content); var row = spell.SnapshotModel(); var xml = XElement.Parse(row.Definition);
			xml.Element("ControlledPower")!.Element("ScalarBindings")!.RemoveNodes(); row.Definition = xml.ToString();
			var copy = new MagicSpell(row, f.World.Object).CastingCopy(f.Actor.Object, f.Traits[0], 7, SpellPower.ExtremelyStrong, Difficulty.Normal);
			if (content.EligibilitySource is null)
			{
				Assert.AreEqual(-10.0, ((RoomLightEffect)copy.SpellEffects.First()).AddedLuxPerPower);
				Assert.AreEqual(-0.5, ((RoomTemperatureEffect)copy.SpellEffects.Last()).TemperatureDeltaPerPower);
			}
			else
			{
				Assert.AreEqual(0.0, ((ItemEnchantEffect)copy.SpellEffects.Single()).AttackCheckBonus);
				Assert.AreEqual("return @target.isweapon", content.EligibilitySource);
				var eligibility = f.World.Object.FutureProgs.Get(9);
				Assert.IsTrue(eligibility.ExecuteBool(NativeItem(f, true), f.Actor.Object));
				Assert.IsFalse(eligibility.ExecuteBool(NativeItem(f, false), f.Actor.Object));
				Assert.ThrowsException<ArgumentOutOfRangeException>(() => content.BuildDefinition(10, 1, 0));
			}
		}
	}

	[DataTestMethod]
	[DataRow(0, 0, "1/0")]
	[DataRow(0, 1, "sqrt(-1)")]
	[DataRow(1, 0, "1/0")]
	public void Scalars_InvalidEvaluationRefusesBeforePayment(int contentIndex, int bindingIndex, string formula)
	{
		var f = new MagicCastingFixture(); f.Acquire(); var content = ArmageddonScalarUtilityContent.Create()[contentIndex].Content;
		var row = Load(f, content).SnapshotModel(); var xml = XElement.Parse(row.Definition);
		xml.Element("ControlledPower")!.Element("ScalarBindings")!.Elements().ElementAt(bindingIndex).Value = formula;
		row.Definition = xml.ToString(); f.Spells[0] = new MagicSpell(row, f.World.Object);
		var item = NativeItem(f, true);
		f.Actor.Setup(x => x.TargetItem("weapon")).Returns(item);
		var result = f.Service.Cast(f.Intent(1, false) with { Targets = contentIndex == 0 ? "here" : "weapon" });
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		StringAssert.Contains(result.Message, "target[" + bindingIndex + "]");
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Store.Operations.Count);
	}

	private static GameItem NativeItem(MagicCastingFixture f, bool weapon)
	{
		var proto = new Mock<IGameItemProto>(); proto.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		proto.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		proto.SetupGet(x => x.Keywords).Returns(["weapon"]);
		var component = new Mock<IGameItemComponentProto>();
		var melee = new Mock<IMeleeWeapon>();
		component.Setup(x => x.CreateNew(It.IsAny<IGameItem>(), It.IsAny<ICharacter>(), It.IsAny<bool>())).Returns(melee.Object);
		var wearable = new Mock<IWearable>(); wearable.SetupGet(x => x.WornBy).Returns(f.Body.Object);
		var wearProto = new Mock<IGameItemComponentProto>();
		wearProto.Setup(x => x.CreateNew(It.IsAny<IGameItem>(), It.IsAny<ICharacter>(), It.IsAny<bool>())).Returns(wearable.Object);
		proto.SetupGet(x => x.Components).Returns(weapon ? new[] { component.Object, wearProto.Object } : []);
		return new GameItem(proto.Object, quality: ItemQuality.Standard);
	}

	private static MagicSpell Load(MagicCastingFixture f, ArmageddonUtilitySpellContent content)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		Mock.Get(f.Actor.Object.Location).SetupGet(x => x.Gameworld).Returns(f.World.Object);
		Mock.Get(f.World.Object.LightModel).Setup(x => x.GetIlluminationDescription(It.IsAny<double>())).Returns("dim");
		if (content.EligibilityRow(1) is { } progRow)
		{
			progRow.Id = 9; var prog = new FutureProg(progRow, f.World.Object);
			Assert.IsTrue(prog.Compile(), prog.CompileError);
			var progs = new All<IFutureProg>(); foreach (var old in f.World.Object.FutureProgs) progs.Add(old); progs.Add(prog);
			f.World.SetupGet(x => x.FutureProgs).Returns(progs);
		}
		f.Expressions.Clear(); var cost = content.CostRow(1); cost.Id = 1; var duration = content.DurationRow(1); duration.Id = 2;
		f.Expressions.Add(new TraitExpression(cost, f.World.Object)); f.Expressions.Add(new TraitExpression(duration, f.World.Object));
		var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2;
		row.Definition = content.BuildDefinition(10, 1, content.EligibilitySource is null ? 0 : 9).ToString();
		return new MagicSpell(row, f.World.Object);
	}
}
