#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Health.Breathing;
using MudSharp.Magic;
using MudSharp.Magic.WaterBreathing;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WaterBreathingCompatibilityTests
{
	[DataTestMethod, DataRow("simple"), DataRow("gills"), DataRow("blowhole"), DataRow("partless")]
	public void NativeStrategies_AllowMappedLiquidRefuseUnmappedAndGasAndKeepRacialCompatibility(string strategy)
	{
		var f = new Fixture(strategy); Assert.IsFalse(f.Strategy.CanBreathe(f.Body.Object));
		f.Effects.Add(f.Scoped); Assert.IsTrue(f.Strategy.CanBreathe(f.Body.Object));
		f.Terrain.SetupGet(x => x.WaterFluid).Returns(f.Unmapped); Assert.IsFalse(f.Strategy.CanBreathe(f.Body.Object));
		f.Race.Setup(x => x.CanBreatheFluid(f.Unmapped)).Returns((true, 1.0)); Assert.IsTrue(f.Strategy.CanBreathe(f.Body.Object));
		f.Cell.Setup(x => x.IsUnderwaterLayer(RoomLayer.Underwater)).Returns(false);
		Assert.IsFalse(f.Strategy.CanBreathe(f.Body.Object));
		f.Race.Setup(x => x.CanBreatheFluid(f.Gas)).Returns((true, 1.0)); Assert.IsTrue(f.Strategy.CanBreathe(f.Body.Object));
	}

	[DataTestMethod, DataRow("simple"), DataRow("blowhole"), DataRow("gills")]
	public void MappedMagic_DoesNotBypassOrgansOrWorkingParts(string strategy)
	{
		var f = new Fixture(strategy); f.Effects.Add(f.Scoped); Assert.IsTrue(f.Strategy.CanBreathe(f.Body.Object));
		f.Body.Setup(x => x.OrganFunction<HeartProto>()).Returns(0); Assert.IsFalse(f.Strategy.CanBreathe(f.Body.Object));
		f.Body.Setup(x => x.OrganFunction<HeartProto>()).Returns(1);
		if (strategy != "gills")
		{
			f.Body.Setup(x => x.OrganFunction<LungProto>()).Returns(0.49); Assert.IsFalse(f.Strategy.CanBreathe(f.Body.Object));
			f.Body.Setup(x => x.OrganFunction<LungProto>()).Returns(1);
			f.Body.Setup(x => x.OrganFunction<TracheaProto>()).Returns(0); Assert.IsFalse(f.Strategy.CanBreathe(f.Body.Object));
			f.Body.Setup(x => x.OrganFunction<TracheaProto>()).Returns(1);
		}
		f.Body.SetupGet(x => x.Bodyparts).Returns(Array.Empty<IBodypart>()); Assert.IsFalse(f.Strategy.CanBreathe(f.Body.Object));
	}

	[DataTestMethod, DataRow("simple", true), DataRow("gills", false), DataRow("blowhole", false), DataRow("partless", false)]
	public void LegacyBroadGrant_RemainsLimitedToPreviousLungIntegration(string strategy, bool expected)
	{
		var f = new Fixture(strategy); f.Effects.Add(new SpellWaterBreathingEffect(f.Actor.Object, f.Parent));
		Assert.AreEqual(expected, f.Strategy.CanBreathe(f.Body.Object));
		f.Cell.Setup(x => x.IsUnderwaterLayer(RoomLayer.Underwater)).Returns(false);
		Assert.AreEqual(expected, f.Strategy.CanBreathe(f.Body.Object));
	}

	[TestMethod]
	public void NonBreather_RemainsWithoutRespiratoryRequirementOrGrantedBreathing()
	{
		var f = new Fixture("partless"); f.Effects.Add(f.Scoped); var non = new NonBreather();
		Assert.IsFalse(non.NeedsToBreathe); Assert.IsFalse(non.CanBreathe(f.Body.Object));
	}

	[TestMethod]
	public void MalformedPersistedScope_IsInactiveAndPreservedWithoutBroadFallback()
	{
		var f = new Fixture("partless"); SpellScopedWaterBreathingEffect.InitialiseEffectType();
		var xml = f.Scoped.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		var scope = xml.Element("Effect")!.Element("WaterScope")!; scope.SetAttributeValue("version", 99);
		var restored = (SpellScopedWaterBreathingEffect)Effect.LoadEffect(xml, f.Actor.Object);
		Assert.IsFalse(restored.AppliesToFluid(f.Water)); Assert.IsFalse(restored.AppliesToFluid(f.Gas));
		Assert.IsTrue(XNode.DeepEquals(scope, restored.SaveToXml(new Dictionary<IEffect, TimeSpan>()).Element("Effect")!.Element("WaterScope")));
	}

	private sealed class Fixture
	{
		public Mock<IBody> Body { get; } = new(); public Mock<ICharacter> Actor { get; } = new();
		public Mock<IRace> Race { get; } = new(); public Mock<ICell> Cell { get; } = new();
		public Mock<ITerrain> Terrain { get; } = new(); public List<IEffect> Effects { get; } = [];
		public ILiquid Water { get; } = Mock.Of<ILiquid>(x => x.Id == 71);
		public ILiquid Unmapped { get; } = Mock.Of<ILiquid>(x => x.Id == 72);
		public IGas Gas { get; } = Mock.Of<IGas>(x => x.Id == 73);
		public IBreathingStrategy Strategy { get; }
		public MagicSpellParent Parent { get; }
		public SpellScopedWaterBreathingEffect Scoped { get; }
		public Fixture(string strategy)
		{
			Strategy = strategy switch { "simple" => new LungBreather(), "gills" => new GillBreather(), "blowhole" => new BlowholeBreather(), _ => new PartlessBreather() };
			var world = new Mock<IFuturemud>(); world.SetupGet(x => x.Liquids).Returns(MagicCastingFixture.Collection(() => new[] { Water, Unmapped }));
			world.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => Array.Empty<MudSharp.FutureProg.IFutureProg>()));
			Actor.SetupGet(x => x.Gameworld).Returns(world.Object); Actor.SetupGet(x => x.Body).Returns(Body.Object);
			Body.SetupGet(x => x.Gameworld).Returns(world.Object); Body.SetupGet(x => x.Actor).Returns(Actor.Object);
			Body.SetupGet(x => x.Race).Returns(Race.Object); Body.SetupGet(x => x.Location).Returns(Cell.Object);
			Body.SetupGet(x => x.RoomLayer).Returns(RoomLayer.Underwater); Body.SetupGet(x => x.BreathingStrategy).Returns(Strategy);
			Cell.Setup(x => x.IsUnderwaterLayer(RoomLayer.Underwater)).Returns(true); Cell.Setup(x => x.Terrain(Actor.Object)).Returns(Terrain.Object);
			Cell.SetupGet(x => x.Atmosphere).Returns(Gas); Terrain.SetupGet(x => x.WaterFluid).Returns(Water);
			Body.Setup(x => x.OrganFunction<HeartProto>()).Returns(1); Body.Setup(x => x.OrganFunction<LungProto>()).Returns(1);
			Body.Setup(x => x.OrganFunction<TracheaProto>()).Returns(1);
			Body.SetupGet(x => x.Bodyparts).Returns(new IBodypart[] {
				(IBodypart)RuntimeHelpers.GetUninitializedObject(typeof(MouthProto)),
				(IBodypart)RuntimeHelpers.GetUninitializedObject(typeof(GillProto)),
				(IBodypart)RuntimeHelpers.GetUninitializedObject(typeof(BlowholeProto)) });
			Body.Setup(x => x.WornItemsFor(It.IsAny<IBodypart>())).Returns(Array.Empty<MudSharp.GameItems.IGameItem>());
			Body.Setup(x => x.CombinedEffectsOfType<IAdditionalBreathableFluidEffect>()).Returns(() => Effects.OfType<IAdditionalBreathableFluidEffect>());
			Parent = new MagicSpellParent(Actor.Object, Mock.Of<IMagicSpell>(x => x.Gameworld == world.Object), Actor.Object);
			Scoped = new(Actor.Object, Parent, new WaterBreathingFluidScope([Water]));
		}
	}
}
