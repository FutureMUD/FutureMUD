using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Health.Breathing;
using MudSharp.Health.Strategies;
using MudSharp.Health.Wounds;
using MudSharp.Planes;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Combat;
using MudSharp.Magic;
using MudSharp.Body.Traits;
using MudSharp.FutureProg;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class EnvironmentalExposureIntegrationTests
{
	private sealed class WorldFixture
	{
		public readonly Mock<IFuturemud> World = new() { DefaultValue = DefaultValue.Mock };
		public readonly Mock<IRoom> Room = new();
		public readonly Mock<ISolid> Material = new();
		public readonly Mock<IGas> Gas = new();
		public readonly Mock<IGameItem> Item = new();
		public readonly List<IDamage> Damage = new();
		public readonly SurfaceLiquidState State;
		public readonly LiquidSurfaceReaction Rule;
		public readonly EnvironmentalExposureService Service;
		public DateTime Now = DateTime.UtcNow;
		public WorldFixture()
		{
			World.Setup(x => x.GetStaticConfiguration("EnvironmentalExposureMode")).Returns("Enabled");
			World.Setup(x => x.UnitManager.BaseFluidToLitres).Returns(1);
			var materialPlane = Mock.Of<IPlane>(x => x.Id == 1);
			var planes = new All<IPlane>(); planes.Add(materialPlane);
			World.SetupGet(x => x.DefaultPlane).Returns(materialPlane); World.SetupGet(x => x.Planes).Returns(planes);
			World.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
			World.SetupGet(x => x.Tags).Returns(new All<ITag>());
			World.SetupGet(x => x.Liquids).Returns(new All<ILiquid>());
			World.SetupGet(x => x.MagicalSubstances).Returns(new All<IMagicalSubstance>());
			World.Setup(x => x.GetStaticDouble("LiquidContaminationEffectDuration")).Returns(1e9);
			World.Setup(x => x.GetStaticDouble("BodyLiquidContaminationEffectDuration")).Returns(1e9);
			Material.SetupGet(x => x.Id).Returns(1);
			var materials = new All<ISolid>(); materials.Add(Material.Object); World.SetupGet(x => x.Materials).Returns(materials);
			Material.SetupGet(x => x.ExposureProperties).Returns(new MaterialExposureProperties());
			Rule = new LiquidSurfaceReaction(World.Object) { TargetMaterial = Material.Object, Routes = ExposureRoute.GasContact | ExposureRoute.Inhalation };
			Rule.Convert(10, 0, 0, false);
			Assert.AreEqual(0, Rule.ValidationErrors(true).Count(), "Fixture reaction must be valid.");
			Gas.SetupGet(x => x.EnvironmentalReactions).Returns(new[] { Rule });
			Room.SetupGet(x => x.Atmosphere).Returns(Gas.Object);
			Room.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(20);
			Item.SetupGet(x => x.Gameworld).Returns(World.Object);
			Item.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			Item.SetupGet(x => x.Material).Returns(Material.Object);
			Item.SetupGet(x => x.Size).Returns((SizeCategory)5);
			Item.SetupGet(x => x.Location).Returns(Room.Object);
			Item.SetupGet(x => x.LocationLevelPerceivable).Returns(Item.Object);
			State = new SurfaceLiquidState(World.Object);
			Item.SetupGet(x => x.SurfaceLiquidState).Returns(State);
			Item.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(Damage.Add).Returns(Array.Empty<IWound>());
			Service = new EnvironmentalExposureService(World.Object, () => Now);
		}
		public void Advance(double seconds) { Now = Now.AddSeconds(seconds); Service.Advance(Now); }
		public Mock<IBody> Body(string strategy, params IBodypart[] parts)
		{
			var body = new Mock<IBody>(); var actor = new Mock<ICharacter>(); var race = new Mock<IRace>();
			body.SetupGet(x => x.Gameworld).Returns(World.Object); actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			actor.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			body.SetupGet(x => x.Actor).Returns(actor.Object); actor.SetupGet(x => x.Body).Returns(body.Object);
			body.SetupGet(x => x.Location).Returns(Room.Object); actor.SetupGet(x => x.Location).Returns(Room.Object);
			body.SetupGet(x => x.Bodyparts).Returns(parts); body.SetupGet(x => x.Organs).Returns(Array.Empty<IOrganProto>());
			body.SetupGet(x => x.Race).Returns(race.Object); race.Setup(x => x.BreathingRate(body.Object, It.IsAny<IFluid>())).Returns(1);
			body.SetupGet(x => x.BreathingStrategy).Returns(Mock.Of<IBreathingStrategy>(x => x.Name == strategy && x.NeedsToBreathe));
			body.Setup(x => x.GetMaterial(It.IsAny<IBodypart>())).Returns(Material.Object);
			body.SetupGet(x => x.SurfaceLiquidState).Returns(new BodySurfaceLiquidState(World.Object, () => parts.OfType<IExternalBodypart>(), () => { }));
			body.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(Damage.Add).Returns(Array.Empty<IWound>());
			return body;
		}
	}

	[TestMethod]
	public void N10_RealScheduler_SettlesSubTickEntryAndExit()
	{
		var f = new WorldFixture(); f.Advance(0.1); f.Service.Track(f.Item.Object);
		f.Now = f.Now.AddSeconds(0.2);
		using (f.Service.Change(f.Item.Object)) f.Item.SetupGet(x => x.Location).Returns((IRoom)null!);
		Assert.AreEqual(2, f.Damage.Sum(x => x.DamageAmount), 1e-8);
		f.Advance(10); Assert.AreEqual(2, f.Damage.Sum(x => x.DamageAmount), 1e-8);
		Assert.AreEqual(0, f.Service.ActiveCount);
	}

	[DataTestMethod]
	[DataRow(0.25, 2.5)] [DataRow(200.0, 1000.0)] [DataRow(double.NaN, 0.0)] [DataRow(-1.0, 0.0)]
	public void AmbientIntensityHook_IsBoundedAndFailsClosed(double multiplier, double expected)
	{
		var f = new WorldFixture(); f.Room.SetupGet(x => x.Atmosphere).Returns((IFluid)null!);
		f.Material.SetupGet(x => x.HeatDamagePoint).Returns(100);
		f.Material.Object.ExposureProperties.ThermalSlope = 0.5;
		f.Material.Object.ExposureProperties.ThermalIntensityProgId = 71;
		f.Room.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(120);
		var prog = new Mock<IFutureProg>(); prog.SetupGet(x => x.Id).Returns(71);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Number);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		prog.Setup(x => x.Execute<double?>(It.IsAny<object[]>())).Returns(multiplier);
		var progs = new All<IFutureProg>(); progs.Add(prog.Object); f.World.SetupGet(x => x.FutureProgs).Returns(progs);
		f.Service.Track(f.Item.Object); f.Advance(1);
		Assert.AreEqual(expected, f.Damage.Sum(x => x.DamageAmount), 1e-8);
		var patch = new ExposurePatch(f.Item.Object, null, f.Material.Object, 1, 1);
		prog.Invocations.Clear();
		Assert.AreEqual(0, f.Service.Resolver.ThermalModifier(patch, "diagnostic", 1, dryRun: true));
		prog.Verify(x => x.Execute<double?>(It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	public void AmbientIntensityHook_DisablingTheModeCannotCommitInjury()
	{
		var f = new WorldFixture(); f.Room.SetupGet(x => x.Atmosphere).Returns((IFluid)null!);
		f.Material.SetupGet(x => x.HeatDamagePoint).Returns(100);
		f.Material.Object.ExposureProperties.ThermalIntensityProgId = 71;
		f.Room.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(120);
		var prog = new Mock<IFutureProg>(); prog.SetupGet(x => x.Id).Returns(71);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Number);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		prog.Setup(x => x.Execute<double?>(It.IsAny<object[]>())).Callback(() =>
			f.World.Setup(x => x.GetStaticConfiguration("EnvironmentalExposureMode")).Returns("Disabled")).Returns(1);
		var progs = new All<IFutureProg>(); progs.Add(prog.Object); f.World.SetupGet(x => x.FutureProgs).Returns(progs);
		f.Service.Track(f.Item.Object); f.Advance(1);
		Assert.AreEqual(0, f.Damage.Count);
	}

	[TestMethod]
	public void AmbientIntensityBuilder_SettlesTheIntervalBeforeChangingTheProg()
	{
		var f = new WorldFixture(); f.Room.SetupGet(x => x.Atmosphere).Returns((IFluid)null!);
		var material = new Solid(new MudSharp.Models.Material { Id = 1, Name = "fixture", HeatDamagePoint = 100, ResidueColour = "white" }, f.World.Object);
		material.ExposureProperties.ThermalSlope = 0.5;
		f.Item.SetupGet(x => x.Material).Returns(material);
		f.Room.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(120);
		var prog = new Mock<IFutureProg>(); prog.SetupGet(x => x.Id).Returns(71); prog.SetupGet(x => x.Name).Returns("zeroheat");
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Number);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		prog.Setup(x => x.Execute<double?>(It.IsAny<object[]>())).Returns(0);
		var progs = new All<IFutureProg>(); progs.Add(prog.Object); f.World.SetupGet(x => x.FutureProgs).Returns(progs);
		f.Item.SetupGet(x => x.Id).Returns(1);
		var items = new All<IGameItem>(); items.Add(f.Item.Object); f.World.SetupGet(x => x.Items).Returns(items);
		f.World.SetupGet(x => x.Actors).Returns(new All<ICharacter>());
		var service = EnvironmentalExposureService.For(f.World.Object);
		service.Clock = () => f.Now;
		f.Now = f.Now.AddSeconds(1); service.Advance(f.Now); service.Track(f.Item.Object);
		f.Now = f.Now.AddSeconds(0.5);
		var builder = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		Assert.IsTrue(material.BuildingCommand(builder.Object, new StringStack("thermalresponse intensity zeroheat")));
		f.Now = f.Now.AddSeconds(0.5); service.Advance(f.Now);
		Assert.AreEqual(5, f.Damage.Sum(x => x.DamageAmount), 1e-8);
		Assert.AreEqual(71L, material.ExposureProperties.ThermalIntensityProgId);
	}

	[TestMethod]
	public void RealScheduler_TenSecondsMatchesTenAdvances_AndDuplicateTrackDoesNotMultiply()
	{
		var a = new WorldFixture(); var b = new WorldFixture();
		a.Service.Track(a.Item.Object); a.Service.Track(a.Item.Object); b.Service.Track(b.Item.Object);
		a.Advance(10); for (var i = 0; i < 10; i++) b.Advance(1);
		Assert.AreEqual(100, a.Damage.Sum(x => x.DamageAmount), 1e-7);
		Assert.AreEqual(a.Damage.Sum(x => x.DamageAmount), b.Damage.Sum(x => x.DamageAmount), 1e-7);
		Assert.AreEqual(1, a.Service.ActiveCount);
	}

	[TestMethod]
	public void CommittedProjectionRelease_DoesNotAdvanceExposureOrRunHealthCallbacks()
	{
		var f = new WorldFixture();
		var service = EnvironmentalExposureService.For(f.World.Object);
		var body = f.Body("lungs");
		var handler = new MudSharp.Effects.EffectHandler(body.Object);
		var effect = new Mock<MudSharp.Effects.IEffect>();
		handler.AddEffect(effect.Object);
		service.Clock = () => throw new AssertFailedException("Committed release must not settle exposure against deleted rows.");
		handler.ForgetCommittedRetirementEffects();
		EnvironmentalExposureService.ForgetCommittedProjectionBody(f.World.Object, body.Object);
		Assert.IsFalse(handler.Effects.Any());
		effect.Verify(x => x.RemovalEffect(), Times.Never);
		Assert.AreEqual(0, f.Damage.Count);
	}

	[TestMethod]
	public void CommittedShelterWardRelease_DoesNotAdvanceExposureOrRunRemovalCallbacks()
	{
		var f = new WorldFixture();
		var service = EnvironmentalExposureService.For(f.World.Object);
		var room = new Mock<IRoom>(); room.SetupGet(x => x.Gameworld).Returns(f.World.Object); room.SetupGet(x => x.Hooks).Returns([]);
		var handler = new MudSharp.Effects.EffectHandler(room.Object);
		room.SetupGet(x => x.Effects).Returns(() => handler.Effects);
		var configuration = new SpellShelterWardConfiguration([1], [], MagicInterdictionCoverage.Both);
		var origin = new SpellLifecycleOrigin(Guid.NewGuid(), 1, 1, 100, SpellShelterAnchor.Family,
			SpellLifecycleMode.TemporaryCleanup, f.Now, f.Now.AddSeconds(60), "typed fixture");
		var ward = new SpellShelterWard(SpellShelterWard.Envelope(origin.Id, origin.SpellId, configuration), room.Object);
		handler.AddEffect(ward);
		service.Clock = () => throw new AssertFailedException("Committed ward detach must not settle deleted room exposure.");
		MudSharp.Magic.Lifecycle.SpellOwnedShelterService.DetachCommittedWard(room.Object, handler, origin, configuration);
		Assert.IsFalse(handler.Effects.Any()); Assert.AreEqual(0, f.Damage.Count);
	}

	[TestMethod]
	public void RuntimeDisableAndResume_DoesNotReplayDisabledTime()
	{
		var f = new WorldFixture(); f.Service.Track(f.Item.Object); f.Advance(1);
		using (f.Service.DefinitionsChanging()) f.World.Setup(x => x.GetStaticConfiguration("EnvironmentalExposureMode")).Returns("Disabled");
		f.Advance(200);
		using (f.Service.DefinitionsChanging()) f.World.Setup(x => x.GetStaticConfiguration("EnvironmentalExposureMode")).Returns("Enabled");
		f.Service.Track(f.Item.Object); f.Advance(1);
		Assert.AreEqual(20, f.Damage.Sum(x => x.DamageAmount), 1e-7);
	}

	[DataTestMethod]
	[DataRow("simple", BodypartTypeEnum.Lung)]
	[DataRow("gills", BodypartTypeEnum.Gill)]
	[DataRow("blowhole", BodypartTypeEnum.Blowhole)]
	[DataRow("partless", BodypartTypeEnum.Mouth)]
	public void ActualRespiratorySamples_UseStrategyAnatomyAndSampleStrength(string strategy, BodypartTypeEnum type)
	{
		var f = new WorldFixture();
		var part = Mock.Of<IExternalBodypart>(x => x.Id == 1 && x.RelativeHitChance == 1 && x.BodypartType == type);
		var body = f.Body(strategy, part);
		f.Service.ResolveRespiratorySample(new(body.Object, f.Gas.Object, "sample", f.Room.Object, RoomLayer.GroundLevel, 0.25, 2, true, true, true));
		Assert.AreEqual(5, f.Damage.Sum(x => x.DamageAmount), 1e-8);
		Assert.IsTrue(f.Damage.All(x => ReferenceEquals(x.Bodypart, part)));
	}

	[DataTestMethod]
	[DataRow(false, true)] [DataRow(true, false)]
	public void NoAirflowOrFailedWithdrawal_DoesNotDeliverRespiratoryInjury(bool airflow, bool success)
	{
		var f = new WorldFixture(); var part = Mock.Of<IExternalBodypart>(x => x.RelativeHitChance == 1);
		var body = f.Body("partless", part);
		f.Service.ResolveRespiratorySample(new(body.Object, f.Gas.Object, "sample", f.Room.Object, RoomLayer.GroundLevel, 1, 10, airflow, true, success));
		Assert.AreEqual(0, f.Damage.Count);
	}

	[TestMethod]
	public void CleanSuppliedSample_AndNonBreather_DoNotInhaleAmbientHazard()
	{
		var f = new WorldFixture(); var part = Mock.Of<IExternalBodypart>(x => x.RelativeHitChance == 1);
		var body = f.Body("partless", part); var clean = new Mock<IGas>();
		f.Service.ResolveRespiratorySample(new(body.Object, clean.Object, "clean supply", f.Room.Object, RoomLayer.GroundLevel, 1, 10, true, true, true));
		var nonbreather = f.Body("nonbreather", part);
		f.Service.ResolveRespiratorySample(new(nonbreather.Object, f.Gas.Object, "sample", f.Room.Object, RoomLayer.GroundLevel, 1, 10, true, false, true));
		Assert.AreEqual(0, f.Damage.Count);
	}

	[TestMethod]
	public void ThermalGasAndAmbientHeat_CombineEachInjuryChannelOnce()
	{
		var f = new WorldFixture(); f.Rule.Channel = "thermal"; f.Rule.DamageType = DamageType.Burning; f.Rule.Convert(1, 30, 40, false);
		f.Material.SetupGet(x => x.HeatDamagePoint).Returns(100);
		f.Material.Object.ExposureProperties.ThermalSlope = 0.5;
		f.Material.Object.ExposureProperties.ThermalCap = 60;
		f.Room.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(140);
		f.Service.Track(f.Item.Object); f.Advance(2);
		Assert.AreEqual(40, f.Damage.Sum(x => x.DamageAmount), 1e-7);
		Assert.AreEqual(60, f.Damage.Sum(x => x.PainAmount), 1e-7);
		Assert.AreEqual(80, f.Damage.Sum(x => x.StunAmount), 1e-7);
	}

	[TestMethod]
	public void RetainedSurface_ContinuesWithoutLooking_AndDryingDoesNotInvokeDamage()
	{
		var f = new WorldFixture(); f.Room.SetupGet(x => x.Atmosphere).Returns((IFluid)null!);
		f.Rule.Routes = ExposureRoute.LiquidContact;
		var liquid = new Mock<ILiquid>(); liquid.SetupGet(x => x.EnvironmentalReactions).Returns(new[] { f.Rule });
		liquid.SetupGet(x => x.RelativeEnthalpy).Returns(1);
		f.State.AddLiquid(new LiquidMixture(liquid.Object, 1, f.World.Object));
		f.Service.Track(f.Item.Object); f.Advance(1);
		Assert.AreEqual(10, f.Damage.Sum(x => x.DamageAmount), 1e-6);
		var count = f.Damage.Count;
		f.State.GetAddendumText(1, 0, false); f.State.GetAddendumText(1, 0, false);
		Assert.AreEqual(count, f.Damage.Count);
	}

	[TestMethod]
	public void ContinuousHealthBatch_ProcessesEachWoundOnceOnItsActualBody()
	{
		var actor = new Mock<ICharacter>(); var actualBody = new Mock<IBody>(); var selectedBody = new Mock<IBody>();
		actor.SetupGet(x => x.Body).Returns(selectedBody.Object); actualBody.SetupGet(x => x.Actor).Returns(actor.Object);
		var first = Mock.Of<IWound>(x => x.Parent == actor.Object);
		var second = Mock.Of<IWound>(x => x.Parent == actor.Object);
		using (ContinuousExposureDamage.BeginHealthBatch())
		{
			for (var i = 0; i < 20; i++)
				using (ContinuousExposureDamage.BeginHealthBatch())
					ContinuousExposureDamage.ProcessWounds(actualBody.Object, new[] { first, second });
			actualBody.Verify(x => x.CheckHealthStatus(), Times.Never);
		}
		actualBody.Verify(x => x.ProcessPassiveWound(first), Times.Once);
		actualBody.Verify(x => x.ProcessPassiveWound(second), Times.Once);
		actualBody.Verify(x => x.CheckHealthStatus(), Times.Once);
		selectedBody.Verify(x => x.CheckHealthStatus(), Times.Never);
		actor.Verify(x => x.CheckHealthStatus(), Times.Never);
	}

	[TestMethod]
	public void CorpseHealthProcessing_UsesTheOriginalBodyAfterItsControllerChangesBodies()
	{
		var f = new WorldFixture();
		var part = Mock.Of<IExternalBodypart>(x => x.Id == 7 && x.RelativeHitChance == 1);
		var original = f.Body("simple", part); var selected = new Mock<IBody>();
		var actor = Mock.Get(original.Object.Actor); actor.SetupGet(x => x.Body).Returns(selected.Object);
		f.Item.Setup(x => x.GetItemType<ICorpse>()).Returns(Mock.Of<ICorpse>(x => x.OriginalBody == original.Object));
		var wound = Mock.Of<IWound>(x => x.Parent == actor.Object && x.CurrentDamage == 1);
		f.Item.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Returns(new[] { wound });
		EnvironmentalExposureResolver.ApplyDamage(new(f.Item.Object, part, f.Material.Object, 1, 1),
			new(ExposureRoute.LiquidContact, ExposureSourceKind.Retained, "corpse", "chemical", "chemical", Guid.NewGuid(), 1), DamageType.Chemical, 1, 0, 0);
		original.Verify(x => x.ProcessPassiveWound(wound), Times.Once);
		original.Verify(x => x.CheckHealthStatus(), Times.Once);
		selected.Verify(x => x.CheckHealthStatus(), Times.Never);
		actor.Verify(x => x.CheckHealthStatus(), Times.Never);
	}

	[TestMethod]
	public void ContinuousHealthBatch_ItemFailureIsProcessedImmediately()
	{
		var item = new Mock<IGameItem>(); var wound = Mock.Of<IWound>(x => x.Parent == item.Object);
		using (ContinuousExposureDamage.BeginHealthBatch())
		{
			ContinuousExposureDamage.ProcessWounds(null, new[] { wound });
			item.Verify(x => x.ProcessPassiveWound(wound), Times.Once);
			item.Verify(x => x.CheckHealthStatus(), Times.Once);
		}
	}

	[TestMethod]
	public void SaveFlush_DefersExposureWithoutLosingTheElapsedInterval()
	{
		var f = new WorldFixture(); f.Service.Track(f.Item.Object);
		f.World.Setup(x => x.SaveManager.Flushing).Returns(true);
		f.Advance(1);
		Assert.AreEqual(0, f.Damage.Count, "A save cannot create new damage or keep its own queue alive.");
		f.World.Setup(x => x.SaveManager.Flushing).Returns(false);
		f.Service.Advance(f.Now);
		Assert.AreEqual(10, f.Damage.Sum(x => x.DamageAmount), 1e-8);
	}

	[DataTestMethod]
	[DataRow("Enabled")] [DataRow("Disabled")] [DataRow("Legacy")]
	public void OrdinaryWetness_DriesOnTheClockWithoutReadsOrHazardRules(string mode)
	{
		var f = new WorldFixture(); f.Room.SetupGet(x => x.Atmosphere).Returns((IFluid)null!);
		f.World.Setup(x => x.GetStaticConfiguration("EnvironmentalExposureMode")).Returns(mode);
		f.World.Setup(x => x.GetStaticDouble("LiquidContaminationEffectDuration")).Returns(1);
		var water = Mock.Of<ILiquid>(x => x.RelativeEnthalpy == 1);
		f.State.AddLiquid(new LiquidMixture(water, 1, f.World.Object));
		f.Service.Refresh(); f.Service.Track(f.Item.Object); f.State.LastResolvedUtc = f.Now;
		f.Advance(2.1);
		Assert.AreEqual(0.81, f.State.LiquidVolume, 1e-8);
		Assert.AreEqual(0, f.Damage.Count);
	}

	[TestMethod]
	public void RoomSurfaceSerialisationAndDescription_DoNotAdvanceExposureOrDrying()
	{
		var f = new WorldFixture(); f.Room.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		var room = new Room(f.Room.Object, -123);
		var state = (SurfaceLiquidState)typeof(Room).GetMethod("GetOrCreateSurfaceState", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(room, new object?[] { RoomLayer.GroundLevel, null })!;
		state.AddLiquid(new LiquidMixture(Mock.Of<ILiquid>(x => x.RelativeEnthalpy == 1), 1, f.World.Object));
		state.LastResolvedUtc = f.Now.AddHours(-1);
		var service = EnvironmentalExposureService.For(f.World.Object); service.Clock = () => f.Now;
		service.Track(f.Item.Object); f.Now = f.Now.AddSeconds(2);
		var before = state.SaveToXml().ToString();
		var save = typeof(Room).GetMethod("SaveSurfaceLiquidState", BindingFlags.Instance | BindingFlags.NonPublic)!;
		for (var i = 0; i < 3; i++)
		{
			Assert.IsNotNull(save.Invoke(room, null));
			room.DescribeLiquidSurface(RoomLayer.GroundLevel, f.Item.Object, false);
		}
		Assert.AreEqual(before, state.SaveToXml().ToString());
		Assert.AreEqual(0, f.Damage.Count);
		service.Advance(f.Now);
		Assert.IsTrue(f.Damage.Sum(x => x.DamageAmount) > 0, "The test has a live exposure clock to distinguish pure reads from settlement.");
	}

	[TestMethod]
	public void LocalisedAnatomyChange_PreservesVolumeAndFreshSurvivorClock()
	{
		var f = new WorldFixture(); var liquid = Mock.Of<ILiquid>(x => x.Density == 1);
		var left = Mock.Of<IExternalBodypart>(x => x.Id == 1 && x.RelativeHitChance == 1);
		var right = Mock.Of<IExternalBodypart>(x => x.Id == 2 && x.RelativeHitChance == 1);
		IExternalBodypart[] parts = [left, right];
		var state = new BodySurfaceLiquidState(f.World.Object, () => parts, () => { });
		state.ForPart(left).AddLiquid(new LiquidMixture(liquid, 10, f.World.Object));
		state.ForPart(left).LastResolvedUtc = f.Now.AddDays(-1);
		state.ForPart(right).AddLiquid(new LiquidMixture(liquid, 10, f.World.Object));
		state.ForPart(right).LastResolvedUtc = f.Now;
		parts = [right]; state.ReconcileParts();
		Assert.AreEqual(20, state.ForPart(right).LiquidVolume);
		Assert.AreEqual(f.Now, state.ForPart(right).LastResolvedUtc);
		Assert.AreEqual(1, state.Parts.Count());
	}

	[TestMethod]
	public void ReplacingSaturatedWetness_ConservesAndAdmitsNewSpecies()
	{
		var f = new WorldFixture(); var water = Mock.Of<ILiquid>(x => x.Id == 10); var acid = Mock.Of<ILiquid>(x => x.Id == 20);
		f.State.AddLiquid(new LiquidMixture(water, 10, f.World.Object));
		var incoming = new LiquidMixture(acid, 4, f.World.Object);
		ExposureTransport.Retain(f.Item.Object, f.State, incoming, 10, LiquidExposureDirection.Irrelevant, null);
		Assert.AreEqual(10, f.State.LiquidVolume); Assert.AreEqual(4, incoming.TotalVolume);
		Assert.AreEqual(4, f.State.ContaminatingLiquid.Instances.Where(x => x.Liquid == acid).Sum(x => x.Amount));
		Assert.AreEqual(14, f.State.LiquidVolume + incoming.TotalVolume);
	}

	[TestMethod]
	public void LayerTransmission_IsIndependentOfSurfaceSusceptibility_AndDestroyedLayerStopsShielding()
	{
		var f = new WorldFixture(); var part = Mock.Of<IExternalBodypart>(x => x.Id == 1 && x.RelativeHitChance == 1);
		var body = f.Body("partless", part);
		var garment = new Mock<IGameItem>(); var cloth = new Mock<ISolid>();
		cloth.SetupGet(x => x.ExposureProperties).Returns(new MaterialExposureProperties { LiquidTransmission = 0.25, GasTransmission = 1 });
		garment.SetupGet(x => x.Material).Returns(cloth.Object);
		body.Setup(x => x.WornItemsFor(part)).Returns(new[] { garment.Object });
		Assert.AreEqual(0.25, ExposureTransport.ExternalPatches(body.Object, new[] { part }, ExposureRoute.LiquidContact).Single(x => x.Target == body.Object).Transmission);
		Assert.AreEqual(1, ExposureTransport.ExternalPatches(body.Object, new[] { part }, ExposureRoute.GasContact).Single(x => x.Target == body.Object).Transmission);
		garment.SetupGet(x => x.Deleted).Returns(true);
		Assert.AreEqual(1, ExposureTransport.ExternalPatches(body.Object, new[] { part }, ExposureRoute.LiquidContact).Single().Transmission);
	}

	[TestMethod]
	[DoNotParallelize]
	public void HealthStrategy_CapsAndPersistsWoundOnTheInjuredBody_NotControllersActiveBody()
	{
		var f = new WorldFixture(); var actor = new Mock<ICharacter>(); var active = new Mock<IBody>(); var injured = new Mock<IBody>();
		var part = Mock.Of<IExternalBodypart>(x => x.Id == 3 && x.DamageModifier == 1 && x.PainModifier == 1 && x.StunModifier == 1);
		var race = Mock.Of<IRace>(); actor.SetupGet(x => x.Id).Returns(1); actor.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		actor.SetupGet(x => x.Body).Returns(active.Object); actor.SetupGet(x => x.Race).Returns(race);
		active.Setup(x => x.HitpointsForBodypart(part)).Returns(5); injured.Setup(x => x.HitpointsForBodypart(part)).Returns(25);
		injured.SetupGet(x => x.Id).Returns(22); injured.SetupGet(x => x.Race).Returns(race);
		var strategy = (SimpleLivingHealthStrategy)RuntimeHelpers.GetUninitializedObject(typeof(SimpleLivingHealthStrategy));
		var wound = (SimpleOrganicWound)strategy.SufferDamage(actor.Object,
			new Damage { TargetBody = injured.Object, Bodypart = part, DamageType = DamageType.Chemical, DamageAmount = 50 }, part).Single();
		Assert.AreEqual(25, wound.CurrentDamage);
		var property = typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!;
		var previous = property.GetValue(null);
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
		try
		{
			property.SetValue(null, context);
			var saved = (MudSharp.Models.Wound)wound.DatabaseInsert();
			Assert.AreEqual(22L, saved.BodyId); Assert.AreEqual(3L, saved.BodypartProtoId); Assert.AreEqual(25, saved.CurrentDamage);
		}
		finally { property.SetValue(null, previous); }
	}

	[DataTestMethod]
	[DataRow(typeof(SimpleLivingHealthStrategy), true, true, true)]
	[DataRow(typeof(ComplexLivingHealthStrategy), true, true, true)]
	[DataRow(typeof(RobotHealthStrategy), true, false, true)]
	[DataRow(typeof(BrainHitpointsStrategy), true, false, false)]
	[DataRow(typeof(ConstructHealthStrategy), false, false, false)]
	[DataRow(typeof(BrainConstructHealthStrategy), false, false, false)]
	[DataRow(typeof(GameItemHealthStrategy), false, false, false)]
	public void ContinuousWounds_AccumulateWithModifiersCapsAndPersistenceAcrossSubdivision(Type strategyType, bool modifiers, bool pain, bool stun)
	{
		var one = Run(1, false); var split = Run(20, false); var resumed = Run(20, true);
		CollectionAssert.AreEqual(one, split);
		CollectionAssert.AreEqual(one, resumed, $"one={string.Join(',', one)}; resumed={string.Join(',', resumed)}");
		Assert.AreEqual(modifiers ? 3.0 : 1.0, one[0]);
		Assert.AreEqual(modifiers ? 24.0 : 12.0, one[1], 1e-7);
		Assert.AreEqual(pain ? 18.0 : 0.0, one[2], 1e-7);
		Assert.AreEqual(stun ? 12.0 : 0.0, one[3], 1e-7);
		return;

		double[] Run(int steps, bool reload)
		{
			var f = new WorldFixture();
			var part = Mock.Of<IExternalBodypart>(x => x.Id == 3 && x.DamageModifier == 2 && x.PainModifier == 3 && x.StunModifier == 4);
			Mock.Get(part).SetupGet(x => x.IdHasBeenRegistered).Returns(true);
			var parts = new All<IBodypart>(); parts.Add(part); f.World.SetupGet(x => x.BodypartPrototypes).Returns(parts);
			var body = f.Body("nonbreather", part); var actor = Mock.Get(body.Object.Actor);
			body.SetupGet(x => x.Id).Returns(22); body.Setup(x => x.HitpointsForBodypart(part)).Returns(10);
			Mock.Get(body.Object.Race).SetupGet(x => x.BloodLiquid).Returns(Mock.Of<ILiquid>());
			actor.SetupGet(x => x.Race).Returns(body.Object.Race);
			actor.Setup(x => x.GetSeverityFor(It.IsAny<IWound>())).Returns<IWound>(w => w.CurrentDamage >= 5 ? WoundSeverity.Moderate : WoundSeverity.Minor);
			var wounds = new List<IWound>(); body.SetupGet(x => x.Wounds).Returns(wounds);
			// A controller's other active body must not become the accumulator owner.
			actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
			var item = strategyType == typeof(GameItemHealthStrategy);
			f.Item.SetupGet(x => x.Wounds).Returns(wounds);
			IHaveWounds owner = item ? f.Item.Object : actor.Object;
			var strategy = (IHealthStrategy)RuntimeHelpers.GetUninitializedObject(strategyType);
			typeof(BaseHealthStrategy).GetField("LodgeDamageExpression", BindingFlags.Instance | BindingFlags.NonPublic)!
				.SetValue(strategy, new ExpressionEngine.Expression("0"));
			var damage = new Damage { DamageType = DamageType.Slashing, TargetBody = item ? null : body.Object, Bodypart = item ? null : part,
				DamageAmount = 1, PainAmount = 0, StunAmount = 0 };
			var combat = strategy.SufferDamage(owner, damage, damage.Bodypart).Single(); wounds.Add(combat);
			var originalCombatDamage = combat.CurrentDamage;
			for (var i = 0; i < steps; i++)
			{
				var packet = new Damage(damage) { DamageAmount = 12.0 / steps, PainAmount = 6.0 / steps, StunAmount = 3.0 / steps,
					ExposureContext = new(ExposureRoute.LiquidContact, ExposureSourceKind.Retained, "fixture", "chemical", "chemical", Guid.Empty, 1.0 / steps, 22) };
				foreach (var wound in strategy.SufferDamage(owner, packet, packet.Bodypart)) if (!wounds.Contains(wound)) wounds.Add(wound);
				if (!reload || i != steps / 2 - 1) continue;
				var loaded = wounds.Select(w =>
				{
					var saved = new MudSharp.Models.Wound { Id = wounds.IndexOf(w) + 1, BodyId = item ? null : 22, BodypartProtoId = w.Bodypart?.Id,
						OriginalDamage = w.OriginalDamage, CurrentDamage = w.CurrentDamage, CurrentPain = w.CurrentPain, CurrentStun = w.CurrentStun,
						DamageType = (int)w.DamageType, ExtraInformation = (string)w.GetType().GetMethod("SaveExtras")!.Invoke(w, null)! };
					var restored = (IWound)Activator.CreateInstance(w.GetType(), owner, saved, f.World.Object, item ? null : body.Object)!;
					Assert.AreSame(w.Bodypart, restored.Bodypart, $"Saved wound part={w.Bodypart?.Id}; saved={saved.BodypartProtoId}; restored={restored.Bodypart?.Id}; registered={f.World.Object.BodypartPrototypes.Get(saved.BodypartProtoId ?? 0)?.Id}; originalIsRegistered={ReferenceEquals(w.Bodypart, f.World.Object.BodypartPrototypes.Get(saved.BodypartProtoId ?? 0))}.");
					Assert.AreEqual(((IContinuousExposureWound)w).ExposureKey, ((IContinuousExposureWound)restored).ExposureKey);
					return restored;
				}).ToArray();
				wounds.Clear(); wounds.AddRange(loaded);
			}
			Assert.AreEqual(originalCombatDamage, wounds.Single(w => ((IContinuousExposureWound)w).ExposureKey is null).CurrentDamage);
			var continuous = wounds.OfType<IContinuousExposureWound>().Where(w => w.ExposureKey is not null).ToArray();
			if (modifiers) Assert.IsTrue(continuous.All(w => w.CurrentDamage <= 10 + 1e-8));
			return new[] { (double)continuous.Length, Math.Round(continuous.Sum(w => w.CurrentDamage), 7), Math.Round(continuous.Sum(w => w.CurrentPain), 7),
				Math.Round(continuous.Sum(w => w.CurrentStun), 7), continuous.Sum(w => (double)w.Severity), continuous.Sum(w => (double)w.BleedStatus) };
		}
	}

	private sealed class MinimumRandom : Random
	{
		public override int Next(int minValue, int maxValue) => minValue;
	}

	[TestMethod]
	public void ItemCombat_CannotMergeIntoAnExposureWound()
	{
		var f = new WorldFixture(); var wounds = new List<IWound>();
		f.Item.SetupGet(x => x.Wounds).Returns(wounds);
		var strategy = (GameItemHealthStrategy)RuntimeHelpers.GetUninitializedObject(typeof(GameItemHealthStrategy));
		var damage = new Damage { DamageType = DamageType.Chemical, DamageAmount = 2,
			ExposureContext = new(ExposureRoute.LiquidContact, ExposureSourceKind.Retained, "fixture", "chemical", "chemical", Guid.Empty, 1) };
		var exposure = strategy.SufferDamage(f.Item.Object, damage, null).Single(); wounds.Add(exposure);
		using var random = Constants.PushRandom(new MinimumRandom());
		var combat = strategy.SufferDamage(f.Item.Object, new Damage(damage) { ExposureContext = null, DamageAmount = 3 }, null).Single();
		Assert.AreNotSame(exposure, combat); Assert.AreEqual(2, exposure.CurrentDamage); Assert.AreEqual(3, combat.CurrentDamage);
		Assert.IsNull(((IContinuousExposureWound)combat).ExposureKey);
	}

	[DataTestMethod]
	[DataRow(DamageType.Hypoxia)] [DataRow(DamageType.Cellular)] [DataRow(DamageType.Chemical)]
	public void ContinuousWounds_DistinctSourcesRetainAllCapOverflow(DamageType type)
	{
		var f = new WorldFixture();
		var part = Mock.Of<IExternalBodypart>(x => x.Id == 3 && x.DamageModifier == 2 && x.PainModifier == 1 && x.StunModifier == 1);
		var body = f.Body("nonbreather", part); body.Setup(x => x.HitpointsForBodypart(part)).Returns(10);
		Mock.Get(body.Object.Actor).SetupGet(x => x.Race).Returns(body.Object.Race);
		var wounds = new List<IWound>(); body.SetupGet(x => x.Wounds).Returns(wounds);
		var strategy = (SimpleLivingHealthStrategy)RuntimeHelpers.GetUninitializedObject(typeof(SimpleLivingHealthStrategy));
		foreach (var source in new[] { "first", "second" })
			wounds.AddRange(strategy.SufferDamage(body.Object.Actor, new Damage { TargetBody = body.Object, Bodypart = part, DamageType = type, DamageAmount = 12,
				ExposureContext = new(ExposureRoute.Inhalation, ExposureSourceKind.Breath, source, "chemical", "chemical", Guid.Empty, 1) }, part));
		Assert.AreEqual(6, wounds.Count); Assert.AreEqual(48, wounds.Sum(w => w.CurrentDamage), 1e-8);
		Assert.AreEqual(2, wounds.Cast<IContinuousExposureWound>().Select(w => w.ExposureKey).Distinct().Count());
	}

	[TestMethod]
	public void RetainedCorpseContact_ContinuesOnOriginalPartWithoutDoubleTickingBody()
	{
		var f = new WorldFixture(); f.Room.SetupGet(x => x.Atmosphere).Returns((IFluid)null!);
		f.Rule.Routes = ExposureRoute.LiquidContact;
		var liquid = Mock.Of<ILiquid>(x => x.RelativeEnthalpy == 1 && x.EnvironmentalReactions == new[] { f.Rule });
		var part = Mock.Of<IExternalBodypart>(x => x.Id == 1 && x.RelativeHitChance == 1);
		var body = f.Body("nonbreather", part);
		((ILocalisedSurfaceLiquidState)body.Object.SurfaceLiquidState).ForPart(part).AddLiquid(new LiquidMixture(liquid, 1, f.World.Object));
		f.Service.Track(body.Object);
		f.Item.Setup(x => x.GetItemType<ICorpse>()).Returns(Mock.Of<ICorpse>(x => x.OriginalBody == body.Object));
		f.Service.Track(f.Item.Object); f.Advance(0.5);
		Assert.AreEqual(5, f.Damage.Sum(x => x.DamageAmount), 1e-8);
		Assert.IsTrue(f.Damage.All(x => x.Bodypart == part));
		f.Service.Forget(f.Item.Object); f.Item.SetupGet(x => x.Deleted).Returns(true);
		f.Service.Track(body.Object); f.Advance(0.5);
		Assert.AreEqual(10, f.Damage.Sum(x => x.DamageAmount), 1e-8);
	}

	[DataTestMethod]
	[DataRow(false, false)] [DataRow(true, false)] [DataRow(false, true)] [DataRow(true, true)]
	public void CorpseExternalContact_UsesOriginalAnatomyAtTheRemainsLocation(bool heat, bool contained)
	{
		var f = new WorldFixture();
		var left = Mock.Of<IExternalBodypart>(x => x.Id == 1 && x.RelativeHitChance == 1);
		var right = Mock.Of<IExternalBodypart>(x => x.Id == 2 && x.RelativeHitChance == 1);
		var body = f.Body("nonbreather", left, right);
		body.SetupGet(x => x.Id).Returns(41);
		body.SetupGet(x => x.Location).Returns((IRoom)null!);
		f.Item.Setup(x => x.GetItemType<ICorpse>()).Returns(Mock.Of<ICorpse>(x => x.OriginalBody == body.Object));
		if (heat)
		{
			f.Room.SetupGet(x => x.Atmosphere).Returns((IFluid)null!);
			f.Material.SetupGet(x => x.HeatDamagePoint).Returns(100);
			f.Material.Object.ExposureProperties.ThermalSlope = 0.5;
			f.Room.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(120);
		}
		if (contained)
		{
			var container = new Mock<IGameItem>();
			container.SetupGet(x => x.Gameworld).Returns(f.World.Object);
			container.SetupGet(x => x.Material).Returns(f.Material.Object);
			container.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			container.SetupGet(x => x.Location).Returns(f.Room.Object);
			container.SetupGet(x => x.LocationLevelPerceivable).Returns(container.Object);
			container.SetupGet(x => x.SurfaceLiquidState).Returns(new SurfaceLiquidState(f.World.Object));
			container.Setup(x => x.GetItemTypes<IContainer>()).Returns(new[] { Mock.Of<IContainer>(x => x.Contents == new[] { f.Item.Object }) });
			f.Item.SetupGet(x => x.ContainedIn).Returns(container.Object);
			f.Service.Track(container.Object);
		}
		f.Service.Track(f.Item.Object); f.Advance(1);
		Assert.AreEqual(10, f.Damage.Sum(x => x.DamageAmount), 1e-8);
		Assert.AreEqual(5, f.Damage.Where(x => x.Bodypart == left).Sum(x => x.DamageAmount), 1e-8);
		Assert.AreEqual(5, f.Damage.Where(x => x.Bodypart == right).Sum(x => x.DamageAmount), 1e-8);
		Assert.IsTrue(f.Damage.All(x => x.TargetBody == body.Object && x.ExposureContext!.TargetBodyId == 41));
	}

	[TestMethod]
	public void CorpseSplash_RetainsFiniteLiquidOnTheSelectedOriginalPart()
	{
		var f = new WorldFixture(); f.Room.SetupGet(x => x.Atmosphere).Returns((IFluid)null!);
		f.Rule.Routes = ExposureRoute.LiquidContact;
		var acid = Mock.Of<ILiquid>(x => x.RelativeEnthalpy == 1 && x.EnvironmentalReactions == new[] { f.Rule });
		var left = Mock.Of<IExternalBodypart>(x => x.Id == 1 && x.RelativeHitChance == 1);
		var right = Mock.Of<IExternalBodypart>(x => x.Id == 2 && x.RelativeHitChance == 1);
		var body = f.Body("nonbreather", left, right);
		body.Setup(x => x.LiquidAbsorbtionAmountsForBodyparts(It.IsAny<IEnumerable<IExternalBodypart>>())).Returns((1.0, 0.0));
		f.Item.Setup(x => x.GetItemType<ICorpse>()).Returns(Mock.Of<ICorpse>(x => x.OriginalBody == body.Object));
		var extinguishTag = Mock.Of<ITag>(); Mock.Get(acid).Setup(x => x.IsA(extinguishTag)).Returns(true);
		var fire = new OnFire(f.Item.Object, Mock.Of<IFireProfile>(x => x.ExtinguishTags == new[] { extinguishTag }));
		f.Item.Setup(x => x.EffectsOfType<OnFire>(It.IsAny<Predicate<OnFire>>())).Returns(new[] { fire });
		var source = new LiquidMixture(acid, 0.1, f.World.Object);
		ExposureTransport.Item(f.Item.Object, source, left, LiquidExposureDirection.FromOnTop);
		var state = (ILocalisedSurfaceLiquidState)body.Object.SurfaceLiquidState;
		Assert.AreEqual(0.1, state.ForPart(left).LiquidVolume, 1e-8);
		Assert.AreEqual(0, state.ForPart(right).LiquidVolume, 1e-8);
		Assert.AreEqual(0, source.TotalVolume, 1e-8);
		Assert.IsTrue(f.Damage.Count > 0 && f.Damage.All(x => x.Bodypart == left));
		f.Item.Verify(x => x.RemoveEffect(fire, true), Times.Once);
	}

	[TestMethod]
	public void CorpseImmersion_SaturationDoesNotManufactureRunoff()
	{
		var f = new WorldFixture(); f.Room.SetupGet(x => x.Atmosphere).Returns((IFluid)null!);
		var water = Mock.Of<ILiquid>(x => x.RelativeEnthalpy == 1);
		var left = Mock.Of<IExternalBodypart>(x => x.Id == 1 && x.RelativeHitChance == 1);
		var right = Mock.Of<IExternalBodypart>(x => x.Id == 2 && x.RelativeHitChance == 1);
		var body = f.Body("nonbreather", left, right);
		body.SetupGet(x => x.LiquidAbsorbtionAmounts).Returns((1.0, 0.0));
		body.Setup(x => x.LiquidAbsorbtionAmountsForBodyparts(It.IsAny<IEnumerable<IExternalBodypart>>())).Returns((0.5, 0.0));
		f.Item.SetupGet(x => x.LiquidAbsorbtionAmounts).Returns((5.0, 0.0));
		f.Item.Setup(x => x.GetItemType<ICorpse>()).Returns(Mock.Of<ICorpse>(x => x.OriginalBody == body.Object));
		f.Service.RefreshImmersion(f.Item.Object, water, true);
		f.Service.RefreshImmersion(f.Item.Object, water, true);
		Assert.AreEqual(1, body.Object.SurfaceLiquidState.LiquidVolume, 1e-8);
		f.Room.Verify(x => x.AddLiquidToSurface(It.IsAny<LiquidMixture>(), It.IsAny<RoomLayer>(), It.IsAny<IPerceivable>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(true, DamageType.Burning, 20.0)] [DataRow(false, DamageType.Freezing, 30.0)]
	public void ThermalGas_ExclusionOrDifferentInjuryTypeDoesNotSuppressAmbientBurning(bool exclude, DamageType type, double total)
	{
		var f = new WorldFixture(); f.Rule.Channel = "thermal"; f.Rule.DamageType = type; f.Rule.NoReaction = exclude;
		f.Material.SetupGet(x => x.HeatDamagePoint).Returns(100); f.Material.Object.ExposureProperties.ThermalSlope = 0.5;
		f.Room.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(140);
		f.Service.Track(f.Item.Object); f.Advance(1);
		Assert.AreEqual(total, f.Damage.Sum(x => x.DamageAmount), 1e-8);
		Assert.AreEqual(20, f.Damage.Where(x => x.DamageType == DamageType.Burning).Sum(x => x.DamageAmount), 1e-8);
	}

	[TestMethod]
	public void FiniteClosedVessel_InteriorReactsAndDebitsItsOwnedContents()
	{
		var f = new WorldFixture(); f.Room.SetupGet(x => x.Atmosphere).Returns((IFluid)null!);
		f.Rule.Routes = ExposureRoute.LiquidContact; f.Rule.Consumption = ReactionConsumption.PerExposure; f.Rule.ConsumptionRate = 0.1;
		var liquid = Mock.Of<ILiquid>(x => x.EnvironmentalReactions == new[] { f.Rule });
		var contents = new LiquidMixture(liquid, 10, f.World.Object);
		var vessel = Mock.Of<ILiquidContainer>(x => x.OwnsLiquidMixture && x.LiquidMixture == contents && x.LiquidCapacity == 1);
		f.Item.Setup(x => x.GetItemTypes<ILiquidContainer>()).Returns(new[] { vessel });
		f.Item.Setup(x => x.GetItemType<IOpenable>()).Returns(Mock.Of<IOpenable>(x => !x.IsOpen));
		f.Service.Track(f.Item.Object); f.Advance(2);
		Assert.AreEqual(20, f.Damage.Sum(x => x.DamageAmount), 1e-7);
		Assert.AreEqual(9.8, contents.TotalVolume, 1e-7);
		Assert.IsTrue(f.Damage.All(x => x.ExposureContext!.SourceKind == ExposureSourceKind.ContainerInterior));
	}

	[DataTestMethod]
	[DataRow(false, 0.0)] [DataRow(true, 10.0)]
	public void Enclosure_GasTightLidGatesNestedContents(bool open, double expected)
	{
		var f = new WorldFixture(); f.Material.Object.ExposureProperties.GasTransmission = 0;
		var child = new Mock<IGameItem>();
		child.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		child.SetupGet(x => x.Id).Returns(2); child.SetupGet(x => x.Material).Returns(f.Material.Object);
		child.SetupGet(x => x.ContainedIn).Returns(f.Item.Object); child.SetupGet(x => x.Size).Returns((SizeCategory)5);
		var childDamage = new List<IDamage>(); child.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(childDamage.Add).Returns(Array.Empty<IWound>());
		f.Item.Setup(x => x.GetItemTypes<IContainer>()).Returns(new[] { Mock.Of<IContainer>(x => x.Contents == new[] { child.Object }) });
		f.Item.Setup(x => x.GetItemType<IOpenable>()).Returns(Mock.Of<IOpenable>(x => x.IsOpen == open));
		f.Service.Track(f.Item.Object); f.Advance(1);
		Assert.AreEqual(expected, childDamage.Sum(x => x.DamageAmount), 1e-8);
		Assert.AreEqual(10, f.Damage.Sum(x => x.DamageAmount), 1e-8);
	}

	[TestMethod]
	public void ThermalSources_RespectIndependentRouteProtectionBeforeDeduplication()
	{
		var f = new WorldFixture(); f.Rule.Channel = "thermal"; f.Rule.DamageType = DamageType.Burning; f.Rule.Convert(10, 0, 0, false);
		f.Material.SetupGet(x => x.HeatDamagePoint).Returns(100); f.Material.Object.ExposureProperties.ThermalSlope = 0.5;
		f.Room.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(140);
		var resistance = new Mock<IExposureResistance>();
		resistance.Setup(x => x.ExposureDamageMultiplier(It.IsAny<ExposureDamageContext>(), It.IsAny<IBodypart>()))
			.Returns<ExposureDamageContext, IBodypart>((context, _) => context.Route == ExposureRoute.AmbientHeat ? 0 : 1);
		f.Item.Setup(x => x.EffectsOfType<IExposureResistance>(It.IsAny<Predicate<IExposureResistance>>())).Returns(new[] { resistance.Object });
		f.Service.Track(f.Item.Object); f.Advance(1);
		Assert.AreEqual(10, f.Damage.Sum(x => x.DamageAmount), 1e-8);
	}

	[TestMethod]
	public void ExpiringProtection_SettlesProtectedAndUnprotectedIntervals()
	{
		var f = new WorldFixture(); var multiplier = 0.25;
		var resistance = new Mock<IExposureResistance>(); resistance.Setup(x => x.ExposureDamageMultiplier(It.IsAny<ExposureDamageContext>(), It.IsAny<IBodypart>())).Returns(() => multiplier);
		f.Item.Setup(x => x.EffectsOfType<IExposureResistance>(It.IsAny<Predicate<IExposureResistance>>())).Returns(new[] { resistance.Object });
		f.Service.Track(f.Item.Object); f.Now = f.Now.AddSeconds(5);
		using (f.Service.Change(f.Item.Object)) multiplier = 1;
		f.Advance(5); Assert.AreEqual(62.5, f.Damage.Sum(x => x.DamageAmount), 1e-7);
	}

	[DataTestMethod]
	[DataRow(0.05)] [DataRow(0.25)] [DataRow(1.0)]
	public void StalePatch_CallbackMovementPreventsRemainingDamage(double substep)
	{
		var f = new WorldFixture(); var moved = new Mock<IRoom>();
		f.World.Setup(x => x.GetStaticConfiguration("EnvironmentalExposureSubstep")).Returns(substep.ToString(System.Globalization.CultureInfo.InvariantCulture));
		f.Service.Refresh();
		f.Item.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(damage =>
		{ f.Damage.Add(damage); f.Item.SetupGet(x => x.Location).Returns(moved.Object); }).Returns(Array.Empty<IWound>());
		f.Service.Track(f.Item.Object); f.Advance(1);
		Assert.AreEqual(10 * substep, f.Damage.Sum(x => x.DamageAmount), 1e-8);
	}

	[TestMethod]
	public void SpellArmour_PreviewHasNoCapacityCost_AndRuntimePassesThroughCorrectPacket()
	{
		var f = new WorldFixture(); var body = f.Body("partless"); var actor = body.Object.Actor;
		var armour = new Mock<IArmourType>(); var incoming = new Damage { DamageAmount = 40 };
		var passed = new Damage { DamageAmount = 10 }; var absorbed = new Damage { DamageAmount = 40 };
		armour.Setup(x => x.AbsorbDamageViaSpell(incoming, f.Material.Object, It.IsAny<ItemQuality>(), actor, true)).Returns((passed, absorbed));
		var configuration = new MagicArmourConfiguration(f.World.Object) { ArmourType = armour.Object, ArmourMaterial = f.Material.Object,
			MaximumDamageAbsorbed = Mock.Of<ITraitExpression>(x => x.OriginalFormulaText == "0") };
		var effect = new SpellArmourProtectionEffect(actor, Mock.Of<IMagicSpellEffectParent>(), configuration);
		Assert.AreSame(passed, effect.PreviewDamage(incoming)); Assert.AreEqual(0, effect.TotalDamageAbsorbed);
		var wounds = new List<IWound>(); Assert.AreSame(passed, effect.PassiveSufferDamage(incoming, ref wounds));
		Assert.AreEqual(30, effect.TotalDamageAbsorbed);
	}

	[DataTestMethod]
	[DataRow(0.0)] [DataRow(100.0)]
	public void NakedImmersion_PrewettingDoesNotPreventContinuousOrRetainedInjury(double prewet)
	{
		var f = new WorldFixture(); f.Rule.Routes = ExposureRoute.LiquidContact;
		f.Room.SetupGet(x => x.Atmosphere).Returns((IFluid)null!);
		f.Room.Setup(x => x.IsSwimmingLayer(It.IsAny<RoomLayer>())).Returns(true);
		var acid = Mock.Of<ILiquid>(x => x.Id == 1 && x.RelativeEnthalpy == 1 && x.EnvironmentalReactions == new[] { f.Rule });
		var water = Mock.Of<ILiquid>(x => x.Id == 2 && x.RelativeEnthalpy == 1);
		f.Room.Setup(x => x.Terrain(It.IsAny<ICharacter>())).Returns(Mock.Of<ITerrain>(x => x.WaterFluid == acid));
		f.World.Setup(x => x.GetStaticDouble("BodyLiquidContaminationEffectDuration")).Returns(1e9);
		var foot = Mock.Of<IExternalBodypart>(x => x.Id == 1 && x.RelativeHitChance == 1 && x.Orientation == Orientation.Lowest);
		var body = f.Body("nonbreather", foot);
		body.SetupGet(x => x.LiquidAbsorbtionAmounts).Returns((1.0, 0.0));
		body.Setup(x => x.LiquidAbsorbtionAmountsForBodyparts(It.IsAny<IEnumerable<IExternalBodypart>>())).Returns((1.0, 0.0));
		var local = ((ILocalisedSurfaceLiquidState)body.Object.SurfaceLiquidState).ForPart(foot);
		if (prewet > 0) local.AddLiquid(new LiquidMixture(water, prewet, f.World.Object));
		f.Service.Track(body.Object); f.Service.RefreshImmersion(body.Object); f.Advance(1);
		Assert.AreEqual(10, f.Damage.Sum(x => x.DamageAmount), 1e-7);
		Assert.AreEqual(1, local.ContaminatingLiquid.Instances.Where(x => x.Liquid == acid).Sum(x => x.Amount), 1e-8);
		using (f.Service.Change(body.Object)) f.Room.Setup(x => x.IsSwimmingLayer(It.IsAny<RoomLayer>())).Returns(false);
		f.Advance(1); Assert.AreEqual(20, f.Damage.Sum(x => x.DamageAmount), 1e-7);
	}

	[TestMethod]
	public void SaturatedImmersion_RefreshDoesNotRepeatWholeBodyDryingOrMutateWetness()
	{
		var f = new WorldFixture();
		var water = Mock.Of<ILiquid>(x => x.Id == 2 && x.RelativeEnthalpy == 1);
		f.Room.Setup(x => x.IsSwimmingLayer(It.IsAny<RoomLayer>())).Returns(true);
		f.Room.Setup(x => x.IsUnderwaterLayer(It.IsAny<RoomLayer>())).Returns(true);
		f.Room.Setup(x => x.Terrain(It.IsAny<ICharacter>())).Returns(Mock.Of<ITerrain>(x => x.WaterFluid == water));
		var parts = Enumerable.Range(1, 100).Select(id => Mock.Of<IExternalBodypart>(x => x.Id == id && x.RelativeHitChance == 1)).ToArray();
		var body = f.Body("nonbreather", parts);
		body.SetupGet(x => x.LiquidAbsorbtionAmounts).Returns((1.0, 0.0));
		var local = (ILocalisedSurfaceLiquidState)body.Object.SurfaceLiquidState;
		foreach (var part in parts) local.ForPart(part).AddLiquid(new LiquidMixture(water, 0.01, f.World.Object));
		var before = local.SaveToXml().ToString();
		for (var i = 0; i < 20; i++) f.Service.RefreshImmersion(body.Object, true);
		Assert.AreEqual(before, local.SaveToXml().ToString());
		body.Verify(x => x.ResolveSurfaceLiquidDrying(), Times.Never);
		var patches = ExposureTransport.ExternalPatches(body.Object, parts, ExposureRoute.LiquidContact);
		Assert.AreEqual(1, patches.Sum(x => x.Area), 1e-12);
		Assert.AreEqual(1, patches.Sum(x => x.Capacity), 1e-12);
	}

	[TestMethod]
	public void Immersion_SelectsSubmergedAnatomyAndExcludesFlying()
	{
		var f = new WorldFixture(); f.Room.Setup(x => x.IsSwimmingLayer(It.IsAny<RoomLayer>())).Returns(true);
		var foot = Mock.Of<IExternalBodypart>(x => x.Id == 1 && x.RelativeHitChance == 1 && x.Orientation == Orientation.Lowest);
		var head = Mock.Of<IExternalBodypart>(x => x.Id == 2 && x.RelativeHitChance == 1 && x.Orientation == Orientation.Highest && x.BodypartType == BodypartTypeEnum.Mouth);
		var body = f.Body("simple", foot, head);
		CollectionAssert.AreEqual(new[] { foot }, EnvironmentalExposureService.ImmersedParts(body.Object));
		f.Room.Setup(x => x.IsUnderwaterLayer(It.IsAny<RoomLayer>())).Returns(true);
		Assert.AreEqual(2, EnvironmentalExposureService.ImmersedParts(body.Object).Length);
		body.SetupGet(x => x.PositionState).Returns(MudSharp.Body.Position.PositionStates.PositionFlying.Instance);
		Assert.AreEqual(0, EnvironmentalExposureService.ImmersedParts(body.Object).Length);
	}

	[TestMethod]
	public void Clouds_DeduplicateSourceAndCapCombinedContact_WhileCleanSupplyExcludesThem()
	{
		var f = new WorldFixture(); f.Room.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		f.Room.SetupGet(x => x.Atmosphere).Returns((IFluid)null!); f.Gas.SetupGet(x => x.Id).Returns(3);
		var gases = new All<IGas>(); gases.Add(f.Gas.Object); f.World.SetupGet(x => x.Gases).Returns(gases);
		var a = new TrapGasCloudEffect(f.Room.Object, f.Gas.Object, 999, RoomLayer.GroundLevel, "", 999, 0.75);
		var b = new TrapGasCloudEffect(f.Room.Object, f.Gas.Object, 999, RoomLayer.GroundLevel, "", 999, 0.75);
		f.Room.Setup(x => x.EffectsOfType<TrapGasCloudEffect>(It.IsAny<Predicate<TrapGasCloudEffect>>())).Returns(new[] { a, a, b });
		f.Service.Track(f.Item.Object); f.Advance(1);
		Assert.AreEqual(10, f.Damage.Sum(x => x.DamageAmount), 1e-7);
		var part = Mock.Of<IExternalBodypart>(x => x.Id == 1 && x.RelativeHitChance == 1); var body = f.Body("partless", part);
		var before = f.Damage.Count;
		f.Service.ResolveRespiratorySample(new(body.Object, Mock.Of<IGas>(), "supply", f.Room.Object, RoomLayer.GroundLevel, 1, 1, true, true, true));
		Assert.AreEqual(before, f.Damage.Count);
	}
}
