#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Construction;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class MagicStockOperationReportingTests
{
	[TestCleanup]
	public void RemoveTestOnlyFactory()
	{
		// Keep the process-wide production compatibility registry unchanged for other tests.
		var factories = (IDictionary<string, Func<XElement, IMagicSpell, IMagicSpellEffectTemplate>>)
			typeof(SpellEffectFactory).GetField("_loadTimeFactories", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		factories.Remove("reportingtestprepared");
	}

	[DataTestMethod]
	[DataRow(MagicEffectOperationStatus.Applied, 3)]
	[DataRow(MagicEffectOperationStatus.NoChange, 2)]
	[DataRow(MagicEffectOperationStatus.Rejected, 2)]
	[DataRow(MagicEffectOperationStatus.Unknown, 2)]
	public void PreparedApplication_OutcomeControlsOrdinaryMasteryAndOnePayment(MagicEffectOperationStatus status, int expectedGrade)
	{
		var f = PreparedFixture(status);
		var before = f.Balances[f.Resources[1]];
		var quote = f.Service.Quote(f.Intent());
		Assert.IsTrue(quote.Allowed, quote.Reason);
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(expectedGrade, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		Assert.AreEqual(before - quote.Invocation!.Costs.Single().Amount, f.Balances[f.Resources[1]]);
		var xml = XElement.Parse(f.Store.Operations[result.OperationId!.Value].Definition);
		Assert.AreEqual(status == MagicEffectOperationStatus.Applied, (bool)xml.Attribute("applied")!);
		Assert.AreEqual(1, f.Rolls);
		Assert.AreEqual(status == MagicEffectOperationStatus.Applied ? 1 : 0, f.Samples);
	}

	[TestMethod]
	public void PreparedApplication_ExceptionAfterPayment_QuarantinesWithoutReplayOrRefund()
	{
		var f = PreparedFixture(MagicEffectOperationStatus.Unknown, throws: true);
		var before = f.Balances[f.Resources[1]];
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.IsTrue(f.Balances[f.Resources[1]] < before);
		Assert.AreEqual(2, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		var paid = f.Balances[f.Resources[1]]; f.Restart();
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(f.Intent()).Status);
		Assert.AreEqual(paid, f.Balances[f.Resources[1]]);
		Assert.AreEqual(0, f.Samples);
	}

	private static MagicCastingFixture PreparedFixture(MagicEffectOperationStatus status, bool throws = false)
	{
		SpellEffectFactory.RegisterLoadTimeFactory("reportingtestprepared", (root, spell) => new PreparedTemplate(spell, status, throws));
		var f = new MagicCastingFixture();
		var spell = f.NewSpell(1, "Prepared reporting fixture", "<Effect type='reportingtestprepared'/>");
		f.Spells.Remove(f.Spell);
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades fixture")));
		f.Acquire(); return f;
	}

	[DataTestMethod]
	[DataRow(8.0, 3.0, MagicEffectOperationStatus.Applied)]
	[DataRow(0.0, 3.0, MagicEffectOperationStatus.NoChange)]
	[DataRow(8.0, 0.0, MagicEffectOperationStatus.NoChange)]
	public void Heal_ActualDamageChangeReportsWithoutPersistentChild(double damage, double budget, MagicEffectOperationStatus expected)
	{
		var f = new MagicCastingFixture();
		var wound = new Mock<IWound>(); wound.SetupProperty(x => x.CurrentDamage, damage);
		wound.Setup(x => x.CanBeTreated(TreatmentType.Mend)).Returns(Difficulty.Normal);
		f.Body.SetupGet(x => x.Wounds).Returns([wound.Object]);
		var effect = SpellEffectFactory.LoadEffect(XElement.Parse($"<Effect type='heal'><HealWorstWoundsFirst>true</HealWorstWoundsFirst><HealOverflow>true</HealOverflow><HealingAmount>{budget}</HealingAmount></Effect>"), f.Spell);
		var report = ((IMagicSpellEffectOperation)effect).Apply(f.Actor.Object, f.Actor.Object, default, SpellPower.Weak, null!, []);
		Assert.AreEqual(expected, report.Status); Assert.IsNull(report.Effect);
		Assert.AreEqual(Math.Max(0, damage-budget), wound.Object.CurrentDamage);
		Assert.AreEqual(MagicEffectOperationStatus.Rejected, ((IMagicSpellEffectOperation)effect).Apply(f.Actor.Object, Mock.Of<IPerceivable>(), default, default, null!, []).Status);
	}

	[TestMethod]
	public void DetectMagick_ReportsARealChildAndRejectsNonCharacters()
	{
		var f = new MagicCastingFixture();
		var effect = (IMagicSpellEffectOperation)SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='detectmagick'/>"), f.Spell);
		var parent = new MagicSpellParent(f.Actor.Object, f.Spell, f.Actor.Object);
		var report = effect.Apply(f.Actor.Object, f.Actor.Object, default, SpellPower.Weak, parent, []);
		Assert.AreEqual(MagicEffectOperationStatus.Applied, report.Status); Assert.IsNotNull(report.Effect);
		Assert.AreEqual(MagicEffectOperationStatus.Rejected, effect.Apply(f.Actor.Object, Mock.Of<IPerceivable>(), default, default, parent, []).Status);
	}

	[DataTestMethod]
	[DataRow(false, MagicEffectOperationStatus.NoChange)]
	[DataRow(true, MagicEffectOperationStatus.Applied)]
	public void Dispel_OnlyActualMatchedRemovalReportsApplied(bool removes, MagicEffectOperationStatus expected)
	{
		var f = new MagicCastingFixture();
		var parent = new MagicSpellParent(f.Actor.Object, f.Spell, f.Actor.Object);
		var effects = new List<IEffect> { parent };
		f.Actor.SetupGet(x => x.Effects).Returns(effects);
		f.Actor.Setup(x => x.EffectsOfType<MagicSpellParent>(null)).Returns(() => effects.OfType<MagicSpellParent>());
		f.Actor.Setup(x => x.EffectsOfType<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>()))
			.Returns<Predicate<MagicSpellParent>>(predicate => effects.OfType<MagicSpellParent>().Where(x => predicate == null || predicate(x)));
		f.Actor.Setup(x => x.EffectsOfType<IDispelMagicProxyEffect>(null)).Returns([]);
		f.Actor.Setup(x => x.RemoveEffect(parent, true)).Callback(() => { if(removes) effects.Remove(parent); });
		var effect = (IMagicSpellEffectOperation)SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='dispelmagic'><Mode>0</Mode><CasterPolicy>0</CasterPolicy><EffectKey>any</EffectKey></Effect>"), f.Spell);
		Assert.AreEqual(expected, effect.Apply(f.Actor.Object, f.Actor.Object, default, SpellPower.Weak, null!, []).Status);
		Assert.AreEqual(MagicEffectOperationStatus.Rejected, effect.Apply(f.Actor.Object, null, default, default, null!, []).Status);
	}

	[DataTestMethod]
	[DataRow(0, false, MagicEffectOperationStatus.NoChange)]
	[DataRow(1, false, MagicEffectOperationStatus.Unknown)]
	[DataRow(1, true, MagicEffectOperationStatus.Unknown)]
	public void Dispel_CountdownAndUnobservableSchedulerDoNotInventApplication(int mode, bool shortens, MagicEffectOperationStatus expected)
	{
		var f = new MagicCastingFixture(); var parent = new MagicSpellParent(f.Actor.Object, f.Spell, f.Actor.Object);
		f.Actor.Setup(x => x.EffectsOfType<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>())).Returns([parent]);
		f.Actor.Setup(x => x.EffectsOfType<IDispelMagicProxyEffect>(null)).Returns([]);
		var duration = TimeSpan.FromSeconds(100);
		Mock.Get(f.World.Object.EffectScheduler).Setup(x => x.OriginalDuration(parent)).Returns(() => duration);
		Mock.Get(f.World.Object.EffectScheduler).SetupSequence(x => x.RemainingDuration(parent)).Returns(TimeSpan.FromSeconds(100)).Returns(TimeSpan.FromSeconds(99));
		f.Actor.Setup(x => x.RemoveDuration(parent, It.IsAny<TimeSpan>(), true)).Callback(() => { if(shortens) duration -= TimeSpan.FromSeconds(60); });
		var effect = (IMagicSpellEffectOperation)SpellEffectFactory.LoadEffect(XElement.Parse($"<Effect type='dispelmagic'><Mode>{mode}</Mode><EffectKey>any</EffectKey></Effect>"), f.Spell);
		Assert.AreEqual(expected, effect.Apply(f.Actor.Object, f.Actor.Object, default, default, null!, []).Status);
	}

	[DataTestMethod]
	[DataRow(false, MagicEffectOperationStatus.NoChange)]
	[DataRow(true, MagicEffectOperationStatus.Applied)]
	public void PreparedContainerFill_ReportsOnlyVolumeActuallyMerged(bool merges, MagicEffectOperationStatus expected)
	{
		var f = new MagicCastingFixture(); var liquid = Mock.Of<ILiquid>(x => x.Id == 1);
		f.Body.SetupGet(x => x.BasePlanarPresence).Returns(MudSharp.Planes.PlanarPresenceDefinition.DefaultMaterial(f.World.Object));
		var liquids = new All<ILiquid>(); liquids.Add(liquid); f.World.SetupGet(x => x.Liquids).Returns(liquids);
		var effect = (CreateLiquidEffect)SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='createliquid'><LiquidId>1</LiquidId><AmountFormula>1</AmountFormula><ContainerFill version='1'><Litres>1</Litres></ContainerFill></Effect>"), f.Spell);
		var target = new Mock<IGameItem>(); target.SetupGet(x => x.Gameworld).Returns(f.World.Object); target.SetupGet(x => x.InInventoryOf).Returns(f.Body.Object);
		target.SetupGet(x => x.BasePlanarPresence).Returns(MudSharp.Planes.PlanarPresenceDefinition.DefaultMaterial(f.World.Object));
		var container = new Mock<ILiquidContainer>(); target.Setup(x => x.GetItemType<ILiquidContainer>()).Returns(container.Object);
		container.SetupGet(x => x.Parent).Returns(target.Object); container.SetupGet(x => x.OwnsLiquidMixture).Returns(true); container.SetupGet(x => x.IsOpen).Returns(true); container.SetupGet(x => x.LiquidCapacity).Returns(10);
		LiquidMixture? mixture = null; container.SetupGet(x => x.LiquidMixture).Returns(() => mixture!);
		container.Setup(x => x.MergeLiquid(It.IsAny<LiquidMixture>(), f.Actor.Object, "spell")).Callback<LiquidMixture,ICharacter,string>((incoming,_,__) => { if(merges) mixture=incoming; });
		var applicationType = typeof(CreateLiquidEffect).GetNestedType("ContainerFill", BindingFlags.NonPublic)!;
		var application = (IMagicSpellEffectApplicationOperation)Activator.CreateInstance(applicationType, effect, f.Actor.Object, target.Object, 6.0, liquid)!;
		Assert.AreEqual(expected, application.Apply(null!).Status); Assert.AreEqual(merges ? 6 : 0, mixture?.TotalVolume ?? 0);
		container.SetupGet(x => x.IsOpen).Returns(false);
		Assert.ThrowsException<InvalidOperationException>(() => application.Apply(null!));
	}

	[TestMethod]
	public void PreparedNativeCreation_ReportsActualServiceOutputAndPreservesUncertainFailure()
	{
		var f = new MagicCastingFixture(); var owned = new Mock<ISpellOwnedItemService>(); f.World.SetupGet(x => x.SpellOwnedItems).Returns(owned.Object);
		var prototype = Mock.Of<IGameItemProto>(x => x.Id == 12 && x.RevisionNumber == 0);
		var item = new Mock<IGameItem>(); var target = new Mock<IRoom>(); var effect = (CreateItemEffect)SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='createitem'><ItemQuality>base</ItemQuality><ItemPrototypeId>12</ItemPrototypeId><ItemSkinId>0</ItemSkinId><Quantity>1</Quantity><LoadString></LoadString><Lifecycle version='1' mode='TemporaryCleanup'><Family>reporting-fixture</Family><Seconds>60</Seconds><Placement>standard</Placement></Lifecycle></Effect>"), f.Spell);
		owned.Setup(x => x.Create(prototype, f.Actor.Object, ItemQuality.Standard, It.IsAny<SpellLifecycleOrigin>())).Returns(item.Object);
		var type = typeof(CreateItemEffect).GetNestedType("NativeItemCreation", BindingFlags.NonPublic)!;
		IMagicSpellEffectApplicationOperation Application(Guid[] ids) => (IMagicSpellEffectApplicationOperation)Activator.CreateInstance(type, ids, effect, f.Actor.Object, target.Object, Enumerable.Repeat(prototype, ids.Length).ToArray(), Enumerable.Repeat(ItemQuality.Standard, ids.Length).ToArray(), 1, SpellLifecycleMode.TemporaryCleanup, 60.0, (Guid?)null)!;
		Assert.AreEqual(MagicEffectOperationStatus.NoChange, Application([]).Apply(null!).Status);
		Assert.AreEqual(MagicEffectOperationStatus.Applied, Application([Guid.NewGuid()]).Apply(null!).Status);
		owned.Verify(x => x.Create(prototype, f.Actor.Object, ItemQuality.Standard, It.IsAny<SpellLifecycleOrigin>()), Times.Once);
		target.Verify(x => x.Insert(item.Object, true), Times.Once);
		owned.Setup(x => x.Create(prototype, f.Actor.Object, ItemQuality.Standard, It.IsAny<SpellLifecycleOrigin>())).Throws(new InvalidOperationException("creation failed"));
		Assert.ThrowsException<InvalidOperationException>(() => Application([Guid.NewGuid()]).Apply(null!));
	}

	private sealed class PreparedTemplate(IMagicSpell spell, MagicEffectOperationStatus status, bool throws) : IMagicSpellEffectTemplate, IMagicSpellEffectAdmission

	{
		public IMagicSpell Spell => spell;
		public IFuturemud Gameworld => spell.Gameworld;
		public bool IsInstantaneous => true;
		public bool RequiresTarget => true;
		public bool IsCompatibleWithTrigger(IMagicTrigger _) => true;
		public XElement SaveToXml() => XElement.Parse("<Effect type='reportingtestprepared'/>");
		public IMagicSpellEffectTemplate Clone() => new PreparedTemplate(spell,status,throws);
		public bool BuildingCommand(ICharacter _, StringStack __) => false;
		public string Show(ICharacter _) => "Prepared reporting fixture";
		public IMagicSpellEffect GetOrApplyEffect(ICharacter _, IPerceivable __, OpposedOutcomeDegree ___, SpellPower ____, IMagicSpellEffectParent _____, SpellAdditionalParameter[] ______) => throw new AssertFailedException("Prepared path bypassed.");
		public bool TryPrepareApplication(ICharacter _, IPerceivable __, OpposedOutcomeDegree ___, SpellPower ____, TimeSpan _____, out IMagicSpellEffectApplication? application, out string? error)
		{ application = new PreparedApplication(status,throws); error=null; return true; }
	}
	private sealed record PreparedApplication(MagicEffectOperationStatus Status, bool Throws) : IMagicSpellEffectApplicationOperation
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent _) => throw new AssertFailedException("Prepared reporting path bypassed.");
		public MagicEffectOperation Apply(IMagicSpellEffectParent _) => Throws ? throw new InvalidOperationException("Injected application failure") : new(Status,null);
	}
}
