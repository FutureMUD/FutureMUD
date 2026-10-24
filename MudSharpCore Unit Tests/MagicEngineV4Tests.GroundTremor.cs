#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	[TestMethod]
	[DataRow(1)] [DataRow(2)] [DataRow(3)] [DataRow(4)] [DataRow(5)] [DataRow(6)] [DataRow(7)]
	public void GroundTremor_AllGradesApplyFullLocalDamageAndCasterThirdExactlyOnce(int grade)
	{
		var f = new TremorFixture(localTarget: true);
		var result = f.Cast(grade);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		CollectionAssert.AreEqual(new[] { (double)grade }, f.CasterDamage);
		CollectionAssert.AreEqual(new[] { 3.0 * grade }, f.TargetDamage);
		Assert.AreEqual(0, f.Falls); Assert.AreEqual(0, f.AgilityChecks);
		Assert.AreEqual(1, f.Pair.F.Rolls);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Pair.F.Service.Cast(f.Intent(grade) with { OriginId = result.OperationId }).Status);
		Assert.AreEqual(1, f.TargetDamage.Count);
	}

	[TestMethod]
	[DataRow(false, 1)] [DataRow(true, 0)]
	public void GroundTremor_ProtectedLocalCasterDoesNotPreventIndependentNearbyAgilityFall(bool agilityPass, int falls)
	{
		var f = new TremorFixture(protectedCaster: true) { AgilityPass = agilityPass };
		var result = f.Cast(2);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(falls, f.Falls); Assert.AreEqual(1, f.AgilityChecks);
		Assert.AreEqual(0, f.CasterDamage.Count); Assert.AreEqual(0, f.TargetDamage.Count);
	}

	[TestMethod]
	[DataRow("grade1")] [DataRow("silt")] [DataRow("water")] [DataRow("void")] [DataRow("wind")]
	public void GroundTremor_NearbyTraversalHonoursSourceDepthAndTypedTerrainPlaneStops(string stop)
	{
		var f = new TremorFixture(protectedCaster: true);
		if (stop == "silt") Mock.Get(f.Pair.Destination.Object.CurrentOverlay.Terrain).Setup(x => x.IsA(f.Silt)).Returns(true);
		if (stop is "water" or "void" or "wind")
		{
			var presence = PlanarPresenceDefinition.DefaultMaterial(stop == "water" ? 4 : stop == "void" ? 5 : 3);
			var overlay = Mock.Of<IPlanarOverlayEffect>(x => x.PlanarPresenceDefinition == presence && x.OverridesBasePlanarPresence);
			Mock.Get(overlay).Setup(x => x.Applies()).Returns(true);
			f.Pair.Destination.SetupGet(x => x.Effects).Returns([overlay]);
			f.Pair.Destination.Setup(x => x.EffectsOfType<IPlanarOverlayEffect>(It.IsAny<Predicate<IPlanarOverlayEffect>>())).Returns([overlay]);
		}
		var result = f.Cast(stop == "grade1" ? 1 : 7);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(0, f.Falls); Assert.AreEqual(0, f.AgilityChecks);
	}

	[TestMethod]
	[DataRow("staff")] [DataRow("flying")] [DataRow("ethereal")] [DataRow("stoneskin")]
	public void GroundTremor_NearbyProtectedBodiesReceiveNoDamageOrFall(string protection)
	{
		var f = new TremorFixture(protectedCaster: true);
		if (protection == "staff") f.Pair.Target.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		if (protection == "flying") f.Pair.Target.SetupGet(x => x.PositionState).Returns(PositionFlying.Instance);
		if (protection == "ethereal") f.Pair.TargetBody.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.NonCorporeal(f.Shadow));
		if (protection == "stoneskin") f.Pair.TargetEffects.Add(Mock.Of<IMagicSpellEffectParent>(x => x.Spell.Id == 99 && x.SpellEffects == new[] { Mock.Of<IMagicSpellEffect>() }));
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(2).Status);
		Assert.AreEqual(0, f.Falls); Assert.AreEqual(0, f.AgilityChecks); Assert.AreEqual(0, f.TargetDamage.Count);
	}

	[TestMethod]
	public void GroundTremor_ChangedBodyBeforePaymentRefusesWithoutResourcesOrFall()
	{
		var f = new TremorFixture(protectedCaster: true);
		f.Pair.F.Checkpoint = stage => { if (stage == "BeforePayment") f.Pair.Target.SetupGet(x => x.Body).Returns(Mock.Of<IBody>()); };
		var result = f.Cast(2);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(100.0, f.Pair.F.Balances[f.Pair.F.Resources[1]]); Assert.AreEqual(0, f.Falls); Assert.AreEqual(0, f.AgilityChecks);
	}

	[TestMethod]
	public void GroundTremor_AirborneCasterStillShakesGroundedLocalBodiesWithoutSelfInjury()
	{
		var f = new TremorFixture(localTarget: true);
		f.Pair.CasterLayer = RoomLayer.HighInAir;
		f.Pair.F.Actor.SetupGet(x => x.PositionState).Returns(PositionFlying.Instance);
		var result = f.Cast(2);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		CollectionAssert.AreEqual(new[] { 6.0 }, f.TargetDamage); Assert.AreEqual(0, f.CasterDamage.Count);
	}

	[TestMethod]
	public void GroundTremor_AgilityCallbackCannotMoveAReplacementBody()
	{
		var f = new TremorFixture(protectedCaster: true);
		f.BeforeAgilityResult = () => f.Pair.Target.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(2).Status);
		Assert.AreEqual(1, f.AgilityChecks); Assert.AreEqual(0, f.Falls);
	}

	[TestMethod]
	[DataRow(false)] [DataRow(true)]
	public void GroundTremor_IndividualIncomingWardProtectsLocalHarmAndNearbyFalls(bool local)
	{
		var f = new TremorFixture(localTarget: local, protectedCaster: true);
		var ward = new Mock<IMagicInterdictionEffect>(); ward.As<IEffect>();
		ward.SetupGet(x => x.Coverage).Returns(MagicInterdictionCoverage.Incoming);
		ward.SetupGet(x => x.Mode).Returns(MagicInterdictionMode.Fail);
		ward.SetupGet(x => x.School).Returns(f.Pair.F.School);
		ward.Setup(x => x.ShouldInterdict(f.Pair.F.Actor.Object, f.Pair.F.School)).Returns(true);
		f.Pair.TargetEffects.Add((IEffect)ward.Object);
		f.Pair.Target.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([ward.Object]);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(2).Status);
		Assert.AreEqual(0, f.TargetDamage.Count); Assert.AreEqual(0, f.Falls); Assert.AreEqual(0, f.AgilityChecks);
	}

	[TestMethod]
	public void GroundTremor_LateTerrainOverrideMutationRefusesBeforePayment()
	{
		var f = new TremorFixture(protectedCaster: true);
		var terrain = f.Pair.Destination.Object.CurrentOverlay.Terrain;
		var modifier = new Mock<IOverrideTerrain>(); modifier.As<IEffect>(); modifier.SetupGet(x => x.Terrain).Returns(() => terrain);
		f.Pair.Destination.SetupGet(x => x.Effects).Returns([(IEffect)modifier.Object]);
		f.Pair.Destination.Setup(x => x.Terrain(f.Pair.F.Actor.Object)).Returns(() => terrain);
		f.Pair.F.Checkpoint = stage =>
		{
			if (stage != "BeforePayment") return;
			terrain = Mock.Of<MudSharp.Construction.ITerrain>(x => x.TerrainLayers == new[] { RoomLayer.GroundLevel });
			Mock.Get(terrain).Setup(x => x.IsA(f.Silt)).Returns(true);
		};
		Assert.AreEqual(MagicCastingStatus.Refused, f.Cast(2).Status);
		Assert.AreEqual(100.0, f.Pair.F.Balances[f.Pair.F.Resources[1]]); Assert.AreEqual(0, f.Falls);
	}

	private sealed class TremorFixture
	{
		public ExchangeFixture Pair { get; } = new();
		public List<double> CasterDamage { get; } = [];
		public List<double> TargetDamage { get; } = [];
		public int Falls { get; private set; }
		public int AgilityChecks { get; private set; }
		public bool AgilityPass { get; set; }
		public Action? BeforeAgilityResult { get; set; }
		public ITag Silt { get; } = Mock.Of<ITag>(x => x.Id == 900);
		public IPlane Shadow { get; }
		public TremorFixture(bool localTarget = false, bool protectedCaster = false)
		{
			var f = Pair.F;
			var planes = Enumerable.Range(1, 5).Select(id => Mock.Of<IPlane>(x => x.Id == id)).ToArray(); Shadow = planes[1];
			f.World.SetupGet(x => x.Planes).Returns(MagicCastingFixture.Collection(() => planes));
			f.World.SetupGet(x => x.Tags).Returns(MagicCastingFixture.Collection(() => new[] { Silt }));
			f.Traits.Add(Mock.Of<IAttributeDefinition>(x => x.Id == 123));
			Pair.Target.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
			if (protectedCaster) f.Actor.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
			if (localTarget) { Pair.TargetLocation = Pair.Source.Object; Pair.DestinationMembers.Clear(); Pair.SourceMembers.Add(Pair.Target.Object); }
			var edge = Mock.Of<IRoomExit>(x => x.Origin == Pair.Source.Object && x.Destination == Pair.Destination.Object && x.Exit.Id == 700);
			Pair.Source.Setup(x => x.ExitsFor(f.Actor.Object, true)).Returns([edge]);
			Pair.Destination.Setup(x => x.ExitsFor(f.Actor.Object, true)).Returns([]);
			Mock.Get(Pair.Source.Object.CurrentOverlay).SetupGet(x => x.ExitIDs).Returns([700L]);
			Mock.Get(Pair.Destination.Object.CurrentOverlay).SetupGet(x => x.ExitIDs).Returns([]);
			var check = new Mock<ICheck>(); f.World.Setup(x => x.GetCheck(CheckType.GenericAttributeCheck)).Returns(check.Object);
			check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), Difficulty.Normal, It.IsAny<ITraitDefinition>(),
				It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
				.Returns(() => { AgilityChecks++; BeforeAgilityResult?.Invoke(); return CheckOutcome.SimpleOutcome(CheckType.GenericAttributeCheck, AgilityPass ? Outcome.Pass : Outcome.Fail); });
			Pair.Target.Setup(x => x.SetPosition(PositionSprawled.Instance, PositionModifier.None, null, null)).Callback(() => Falls++);
			var part = Mock.Of<IBodypart>(x => x.Id == 800);
			f.World.SetupGet(x => x.BodypartPrototypes).Returns(MagicCastingFixture.Collection(() => new[] { part }));
			void Body(Mock<IBody> body, List<double> damage)
			{
				body.SetupGet(x => x.Bodyparts).Returns([part]); body.SetupGet(x => x.RandomBodypart).Returns(part);
				body.SetupGet(x => x.Limbs).Returns([]); body.SetupGet(x => x.Wounds).Returns([]);
				body.SetupGet(x => x.AllItems).Returns([]); body.SetupGet(x => x.WornItems).Returns([]); body.SetupGet(x => x.Prosthetics).Returns([]);
				body.Setup(x => x.SufferDamage(It.IsAny<IDamage>())).Returns<IDamage>(packet =>
				{
					packet.MutationGuard?.Validate(body.Object, packet); damage.Add(packet.DamageAmount);
					return new[] { Mock.Of<IWound>(x => x.CurrentDamage == packet.DamageAmount) };
				});
			}
			Body(f.Body, CasterDamage); Body(Pair.TargetBody, TargetDamage);
			var stock = ArmageddonGroundTremorSpellContent.Create(99, 123, 2, 3, 4, 5, 900).Content;
			var row = stock.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2; row.Definition = stock.BuildDefinition(10, 1, 0).ToString();
			f.Spells.Clear(); f.Spells.Add(Mock.Of<IMagicSpell>(x => x.Id == 99));
			var spell = new MagicSpell(row, f.World.Object) { EffectDurationExpression = new TraitExpression("0", f.World.Object) }; f.Spells.Add(spell);
		}
		public MagicCastingIntent Intent(int grade) => new(Pair.F.Actor.Object, Pair.F.Earth.Id, 1, grade, false, "here", OriginId: Guid.NewGuid());
		public MagicCastingResult Cast(int grade) => Pair.F.Service.Cast(Intent(grade));
	}
}
