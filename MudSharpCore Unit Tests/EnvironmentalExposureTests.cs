#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EnvironmentalExposureTests
{
	private static All<T> Collection<T>(params T[] items) where T : class, IFrameworkItem
	{ var result = new All<T>(); foreach (var item in items) result.Add(item); return result; }
	private sealed class Fixture
	{
		public readonly Mock<IFuturemud> World = new() { DefaultValue = DefaultValue.Mock };
		public readonly Mock<ISolid> Material = new();
		public readonly Mock<ILiquid> Acid = new();
		public readonly Mock<ILiquid> Water = new();
		public readonly Mock<IGameItem> Item = new();
		public readonly List<IDamage> Damage = new();
		public readonly List<IEnvironmentalReaction> Rules = new();
		public LiquidSurfaceReaction Rule;
		public EnvironmentalExposureResolver Resolver;
		public Fixture()
		{
			World.Setup(x => x.GetStaticConfiguration("EnvironmentalExposureMode")).Returns("Enabled");
			World.Setup(x => x.UnitManager.BaseFluidToLitres).Returns(1);
			Material.SetupGet(x => x.Id).Returns(1); Material.SetupGet(x => x.Name).Returns("test material");
			World.SetupGet(x => x.Materials).Returns(Collection(Material.Object));
			World.SetupGet(x => x.Tags).Returns(Collection<ITag>());
			World.SetupGet(x => x.FutureProgs).Returns(Collection<IFutureProg>());
			World.SetupGet(x => x.MagicalSubstances).Returns(Collection<IMagicalSubstance>());
			Acid.SetupGet(x => x.Id).Returns(2); Acid.SetupGet(x => x.Name).Returns("acid"); Acid.SetupGet(x => x.Density).Returns(1);
			Water.SetupGet(x => x.Id).Returns(3); Water.SetupGet(x => x.Name).Returns("water"); Water.SetupGet(x => x.Density).Returns(1);
			World.SetupGet(x => x.Liquids).Returns(Collection(Acid.Object, Water.Object));
			Acid.SetupGet(x => x.EnvironmentalReactions).Returns(Rules);
			Item.SetupGet(x => x.Gameworld).Returns(World.Object); Item.SetupGet(x => x.Material).Returns(Material.Object);
			Item.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(Damage.Add).Returns(Array.Empty<IWound>());
			Rule = new LiquidSurfaceReaction(World.Object) { TargetMaterial = Material.Object, DamageType = DamageType.Chemical, Consumption = ReactionConsumption.PerExposure, ConsumptionRate = 2 };
			Rule.Convert(10, 0, 0, false); Rules.Add(Rule); Resolver = new(World.Object);
		}
		public ExposurePatch Patch(double transmission = 1) => new(Item.Object, null, Material.Object, 1, 1, transmission);
		public LiquidMixture Mix(double acid, double water = 0) => new(new[] { new LiquidInstance { Liquid = Acid.Object, Amount = acid }, new LiquidInstance { Liquid = Water.Object, Amount = water } }, World.Object);
		public IReadOnlyList<ExposureResolution> Run(LiquidMixture mix, double seconds, bool dry = false) => Resolver.Liquid(mix, false, new[] { Patch() }, ExposureSourceKind.Retained, "test", seconds, 20, dryRun: dry);
	}
	[DataTestMethod]
	[DataRow(0.25, 12.5)] [DataRow(0.0, 0.0)]
	public void N06_Resistance_ChangesInjuryWithoutChangingConsumption(double multiplier, double expected)
	{
		var f = new Fixture();
		var resistance = new Mock<IExposureResistance>();
		resistance.Setup(x => x.ExposureDamageMultiplier(It.IsAny<ExposureDamageContext>(), It.IsAny<IBodypart>())).Returns(multiplier);
		f.Item.Setup(x => x.EffectsOfType<IExposureResistance>(It.IsAny<Predicate<IExposureResistance>>())).Returns(new[] { resistance.Object });
		var mix = f.Mix(100); var result = f.Run(mix, 5);
		Assert.AreEqual(50, result.Sum(x => x.RawDamage), 1e-8);
		Assert.AreEqual(90, mix.TotalVolume, 1e-8); Assert.AreEqual(expected, f.Damage.Sum(x => x.DamageAmount), 1e-8);
	}
	[TestMethod]
	public void N08_SpentCarrier_ChangesOnlyConsumedSpeciesAndCreatesNoCharges()
	{
		var f = new Fixture(); f.Rule.SpentLiquid = f.Water.Object; f.Rule.ConsumptionRate = 40;
		var mix = f.Mix(25, 75);
		f.Resolver.Liquid(mix, false, new[] { f.Patch() }, ExposureSourceKind.Splash, "N08", 1, 20, splash: true);
		Assert.AreEqual(100, mix.TotalVolume, 1e-8);
		Assert.AreEqual(15, mix.Instances.Where(x => x.Liquid == f.Acid.Object).Sum(x => x.Amount), 1e-8);
		Assert.AreEqual(85, mix.Instances.Where(x => x.Liquid == f.Water.Object).Sum(x => x.Amount), 1e-8);
		Assert.IsTrue(mix.Instances.Where(x => x.Liquid == f.Water.Object).All(x => x.MagicalCharges.Count == 0));
		Assert.AreEqual(100 - mix.Instances.Where(x => x.Liquid == f.Acid.Object).Sum(x => x.Amount), mix.Instances.Where(x => x.Liquid == f.Water.Object).Sum(x => x.Amount), 1e-8);
	}
	[TestMethod]
	public void EvolvingFiniteMixture_TenSecondsAndTenSingleSeconds_Agree()
	{
		var a = new Fixture(); var b = new Fixture(); var mixA = a.Mix(20, 80); var mixB = b.Mix(20, 80);
		a.Run(mixA, 10); for (var i = 0; i < 10; i++) b.Run(mixB, 1);
		Assert.AreEqual(mixA.TotalVolume, mixB.TotalVolume, 1e-8);
		Assert.AreEqual(a.Damage.Sum(x => x.DamageAmount), b.Damage.Sum(x => x.DamageAmount), 1e-8);
		Assert.IsTrue(a.Damage.Sum(x => x.DamageAmount) <= 100 + 1e-8);
	}
	[TestMethod]
	public void DryRun_DoesNotMutateSourceOrCommitWounds()
	{
		var f = new Fixture(); var mix = f.Mix(100); var xml = mix.SaveToXml().ToString();
		Assert.IsTrue(f.Run(mix, 5, true).Sum(x => x.Consumed) > 0);
		Assert.AreEqual(xml, mix.SaveToXml().ToString()); Assert.AreEqual(0, f.Damage.Count);
	}
	[DataTestMethod]
	[DataRow("Disabled")] [DataRow("Legacy")]
	public void Modes_OtherThanEnabled_DoNotRunV2OrConsume(string mode)
	{
		var f = new Fixture(); f.World.Setup(x => x.GetStaticConfiguration("EnvironmentalExposureMode")).Returns(mode);
		var mix = f.Mix(100); Assert.AreEqual(0, f.Run(mix, 10).Count); Assert.AreEqual(100, mix.TotalVolume);
	}
	[TestMethod]
	public void N12_Transmission_IsAppliedOnceAndCarriesHealthDeliveryContext()
	{
		var f = new Fixture(); f.Rule.Consumption = ReactionConsumption.None;
		f.Resolver.Liquid(f.Mix(100), true, new[] { f.Patch(0.5) }, ExposureSourceKind.Immersion, "pool", 4, 20);
		Assert.AreEqual(20, f.Damage.Single().DamageAmount, 1e-9);
		Assert.IsNotNull(f.Damage.Single().ExposureContext);
		Assert.AreEqual(ExposureSourceKind.Immersion, f.Damage.Single().ExposureContext!.SourceKind);
	}
	[TestMethod]
	public void LegacyConversion_RoundTripsOriginalCoefficientsAndStableIdentity()
	{
		var f = new Fixture();
		var legacy = new LiquidSurfaceReaction(XElement.Parse("<Reaction DamageType='7' DamagePerTick='125' PainPerTick='175' StunPerTick='0'><Tags/></Reaction>"), f.World.Object);
		var identity = legacy.Id; legacy.TargetMaterial = f.Material.Object; legacy.Convert(2, 3, 0);
		var loaded = new LiquidSurfaceReaction(legacy.SaveToXml(), f.World.Object);
		Assert.AreEqual(identity, loaded.Id); Assert.AreEqual(2, loaded.DamageRate); Assert.AreEqual(125, loaded.Legacy!.DamagePerTick); Assert.AreEqual(175, loaded.Legacy.PainPerTick);
	}
	[TestMethod]
	public void LocalisedState_OldSaveDistributesOnceAndAggregateUsesOwnedInstances()
	{
		var f = new Fixture();
		var left = Mock.Of<IExternalBodypart>(x => x.Id == 11 && x.RelativeHitChance == 1);
		var right = Mock.Of<IExternalBodypart>(x => x.Id == 12 && x.RelativeHitChance == 3);
		var old = new SurfaceLiquidState(f.World.Object); old.AddLiquid(f.Mix(40));
		var state = new BodySurfaceLiquidState(f.World.Object, () => new[] { left, right }, () => { }, old.SaveToXml());
		Assert.AreEqual(10, state.ForPart(left).LiquidVolume); Assert.AreEqual(30, state.ForPart(right).LiquidVolume);
		Assert.AreSame(state.ForPart(left).ContaminatingLiquid.Instances.Single(), state.ContaminatingLiquid.Instances.First());
		Assert.ThrowsException<InvalidOperationException>(() => state.ContaminatingLiquid.RemoveLiquidVolume(1));
		var loaded = new BodySurfaceLiquidState(f.World.Object, () => new[] { left, right }, () => { }, state.SaveToXml());
		Assert.AreEqual(40, loaded.LiquidVolume); Assert.AreEqual(10, loaded.ForPart(left).LiquidVolume);
	}
	[TestMethod]
	public void EqualPriorityOverlap_IsQuarantinedAndExactExclusionWins()
	{
		var f = new Fixture(); var other = new LiquidSurfaceReaction(f.Rule, f.World.Object) { Id = Guid.NewGuid(), NoReaction = true };
		f.Rules.Add(other);
		Assert.AreEqual(0, EnvironmentalExposureResolver.SelectRules(f.Acid.Object, f.Material.Object, ExposureRoute.LiquidContact, 20, out var errors).Count);
		Assert.AreEqual(1, errors.Count); other.Priority = 1;
		Assert.IsTrue(EnvironmentalExposureResolver.SelectRules(f.Acid.Object, f.Material.Object, ExposureRoute.LiquidContact, 20, out _).Single().NoReaction);
	}
}
