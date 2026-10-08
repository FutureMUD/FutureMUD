#nullable enable
using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;
using MudSharp.Body.Traits;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework.Scheduling;
using MudSharp.Movement;

namespace MudSharp_Unit_Tests;

public partial class SpellOwnedNpcAdmissionTests
{
	[DataTestMethod]
	[DataRow("0")]
	[DataRow("-1")]
	[DataRow("1/0")]
	[DataRow("0/0")]
	[DataRow("1e30")]
	[DataRow("grade-mastery-power-100")]
	public void Cast_KnownInvalidNpcLifetime_RefusesBeforePaymentRollOrReservation(string formula)
	{
		var f = new MagicCastingFixture();
		var spell = Configure(f, Template(f.World.Object));
		Assert.IsTrue(((CreateNPCEffect)spell.SpellEffects.Single()).BuildingCommand(f.Actor.Object, new StringStack("lifetime " + formula)));
		PrepareSpatial(f);
		var acquired = f.Store.Acquisition(100, spell.Id);
		var result = f.Service.Cast(new(f.Actor.Object, f.Earth.Id, spell.Id, 3, true, ""));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.IsNull(result.OperationId);
		Assert.AreEqual(acquired, f.Store.Acquisition(100, spell.Id));
		Assert.IsTrue(f.Balances.Values.All(x => x == 100));
		Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Store.Opportunities.Count);
		Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Samples); Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Flushes);
	}

	[DataTestMethod]
	[DataRow("grade*60")]
	[DataRow("outcome*60")]
	[DataRow("variable+60")]
	[DataRow("rand(60,120)")]
	public void Quote_NpcLifetime_DoesNotReadDurationTraitsOrDrawFunctions(string formula)
	{
		var random = new AdmissionRandom();
		using var draws = ExpressionEngine.Expression.PushRandom(random);
		var f = new MagicCastingFixture(); var spell = Configure(f, Template(f.World.Object)); PrepareSpatial(f);
		Assert.IsTrue(((CreateNPCEffect)spell.SpellEffects.Single()).BuildingCommand(f.Actor.Object, new StringStack("lifetime " + formula)));
		f.Actor.Invocations.Clear();
		var quote = f.Service.Quote(new(f.Actor.Object, f.Earth.Id, spell.Id, 3, true, ""));
		Assert.IsTrue(quote.Allowed, quote.Reason);
		Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Samples); Assert.AreEqual(0, f.Store.Operations.Count);
		Assert.AreEqual(0, random.Calls, "Duration random functions must remain untouched by admission.");
		f.Actor.Verify(x => x.TraitValue(It.IsAny<ITraitDefinition>(), TraitBonusContext.SpellDuration), Times.Never);
		var copy = Copy(f, spell);
		var effect = (CreateNPCEffect)copy.SpellEffects.Single();
		effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object.Location);
		Assert.IsTrue(effect.TryPrepareApplication(f.Actor.Object, f.Actor.Object.Location, OpposedOutcomeDegree.Total, SpellPower.Insignificant,
			TimeSpan.Zero, out var application, out var error), error);
		Assert.IsNotNull(application);
		if (formula.StartsWith("rand", StringComparison.Ordinal)) Assert.IsTrue(random.Calls > 0);
		f.Actor.Verify(x => x.TraitValue(It.IsAny<ITraitDefinition>(), TraitBonusContext.SpellDuration), Times.AtLeastOnce,
			"Post-roll evaluation must retain the native duration trait-read timing even for an invariant formula.");
	}

	[DataTestMethod]
	[DataRow("coordinate")]
	[DataRow("world")]
	[DataRow("length")]
	public void Cast_KnownInvalidNpcSpawn_RefusesBeforePayment(string invalid)
	{
		var f = new MagicCastingFixture(); var spell = Configure(f, Template(f.World.Object)); PrepareSpatial(f);
		var room = Mock.Get(f.Actor.Object.Location);
		if (invalid == "world") room.SetupGet(x => x.Gameworld).Returns(Mock.Of<IFuturemud>());
		if (invalid == "coordinate") f.Actor.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(room.Object, RoomLayer.GroundLevel, 5));
		if (invalid == "length")
		{
			room.SetupGet(x => x.RouteDefinition).Returns(Mock.Of<IRouteRoomDefinition>(x => x.LengthMetres == 0));
			f.Actor.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(room.Object, RoomLayer.GroundLevel, 0));
		}
		var result = f.Service.Cast(new(f.Actor.Object, f.Earth.Id, spell.Id, 3, true, ""));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.IsNull(result.OperationId); Assert.IsTrue(f.Balances.Values.All(x => x == 100));
		Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Store.Operations.Count);
	}

	[DataTestMethod]
	[DataRow("template")]
	[DataRow("revision")]
	[DataRow("coordinate")]
	[DataRow("route")]
	[DataRow("body")]
	public void PreparedNpc_RawFrameChangeAfterConfirmation_InvalidatesFinalFence(string change)
	{
		var f = new MagicCastingFixture(); var template = Template(f.World.Object); var spell = Configure(f, template); PrepareSpatial(f);
		var route = new Mock<IRouteRoomDefinition>(); route.SetupGet(x => x.LengthMetres).Returns(100.0);
		Mock.Get(f.Actor.Object.Location).SetupGet(x => x.RouteDefinition).Returns(route.Object);
		f.Actor.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(f.Actor.Object.Location, RoomLayer.GroundLevel, 20));
		f.Actor.SetupGet(x => x.RoutePositionMetres).Returns(20);
		var copy = Copy(f, spell); var effect = (CreateNPCEffect)copy.SpellEffects.Single();
		var token = (IMagicSpellEffectPreparedSelectionRawToken)effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object.Location)!;
		Assert.IsTrue(effect.TryConfirmPreparedSelection(f.Actor.Object, f.Actor.Object.Location, out var error), error);
		if (change == "template") template.SelectedSdesc = "changed after confirmation";
		if (change == "revision") SetProperty(template, "Status", RevisionStatus.UnderDesign);
		if (change == "coordinate") f.Actor.SetupGet(x => x.RoutePositionMetres).Returns(21);
		if (change == "route") route.SetupGet(x => x.LengthMetres).Returns(200);
		if (change == "body") f.Actor.SetupGet(x => x.Body).Returns(Mock.Of<MudSharp.Body.IBody>());
		Assert.IsFalse(token.IsCurrent);
		Assert.IsFalse(effect.TryConfirmPreparedSelection(f.Actor.Object, f.Actor.Object.Location, out error));
	}

	[TestMethod]
	public void PreparedNpc_DistinctRecipientsReuseDistinctTokens()
	{
		var f = new MagicCastingFixture(); var spell = Configure(f, Template(f.World.Object)); PrepareSpatial(f);
		var copy = Copy(f, spell); var effect = (CreateNPCEffect)copy.SpellEffects.Single();
		var room = Mock.Of<IRoom>(x => x.Gameworld == f.World.Object);
		var first = effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object.Location)!;
		var second = effect.CapturePreparedSelection(f.Actor.Object, room)!;
		Assert.AreNotSame(first, second);
		var fresh = (CreateNPCEffect)Copy(f, spell).SpellEffects.Single();
		Assert.IsTrue(fresh.TryReusePreparedSelection(first, f.Actor.Object, f.Actor.Object.Location, out var error), error);
		Assert.IsTrue(fresh.TryReusePreparedSelection(second, f.Actor.Object, room, out error), error);
		Assert.IsFalse(fresh.TryReusePreparedSelection(first, f.Actor.Object, room, out error));
	}

	[TestMethod]
	public void PreparedNpc_ActiveRouteMovement_ResolvesApplicationPositionWithoutFreezingAdmissionPosition()
	{
		var f = new MagicCastingFixture(); var spell = Configure(f, Template(f.World.Object)); PrepareSpatial(f);
		var room = f.Actor.Object.Location;
		Mock.Get(room).SetupGet(x => x.RouteDefinition).Returns(Mock.Of<IRouteRoomDefinition>(x => x.LengthMetres == 100));
		f.Actor.SetupGet(x => x.RoutePositionMetres).Returns(20);
		f.Actor.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(room, RoomLayer.GroundLevel, 20));
		var position = 20.0;
		var segment = new Mock<ISpatialMovementSegment>();
		segment.SetupGet(x => x.Origin).Returns(new SpatialLocation(room, RoomLayer.GroundLevel, 20));
		segment.SetupGet(x => x.Destination).Returns(new SpatialLocation(room, RoomLayer.GroundLevel, 80));
		segment.SetupGet(x => x.Duration).Returns(TimeSpan.FromMinutes(1));
		segment.SetupGet(x => x.SpeedMetresPerSecond).Returns(1);
		segment.SetupGet(x => x.DistanceMetres).Returns(60);
		segment.Setup(x => x.PositionAt(It.IsAny<TimeSpan>())).Returns(() => new SpatialLocation(room, RoomLayer.GroundLevel, position));
		RouteSpatialService.Instance.BeginActiveMovement(f.Actor.Object, segment.Object);
		try
		{
			var effect = (CreateNPCEffect)Copy(f, spell).SpellEffects.Single();
			var token = (IMagicSpellEffectPreparedSelectionRawToken)effect.CapturePreparedSelection(f.Actor.Object, room)!;
			position = 45;
			Assert.IsTrue(token.IsCurrent);
			Assert.IsTrue(effect.TryPrepareApplication(f.Actor.Object, room, OpposedOutcomeDegree.None, SpellPower.Insignificant,
				TimeSpan.Zero, out var application, out var error), error);
			var location = (SpatialLocation)application!.GetType().GetProperty("Location")!.GetValue(application)!;
			Assert.AreEqual(45.0, location.RoutePositionMetres);
			Assert.AreEqual(180.0, application.GetType().GetProperty("Seconds")!.GetValue(application));
		}
		finally { RouteSpatialService.Instance.ClearActiveMovement(f.Actor.Object); }
	}

	[DataTestMethod]
	[DataRow("permanent")]
	[DataRow("legacy")]
	public void PreparedNpc_NonTemporaryPolicies_PreserveLifetimeFreeApplications(string mode)
	{
		var f = new MagicCastingFixture(); var spell = Configure(f, Template(f.World.Object)); PrepareSpatial(f);
		Assert.IsTrue(((CreateNPCEffect)spell.SpellEffects.Single()).BuildingCommand(f.Actor.Object, new StringStack("lifecycle " + mode)));
		var effect = (CreateNPCEffect)Copy(f, spell).SpellEffects.Single();
		var token = effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object.Location);
		if (mode == "legacy") Assert.IsNull(token); else Assert.IsNotNull(token);
		Assert.IsTrue(effect.TryPrepareApplication(f.Actor.Object, f.Actor.Object.Location, OpposedOutcomeDegree.None, SpellPower.Insignificant,
			TimeSpan.Zero, out var application, out var error), error);
		Assert.IsNotNull(application);
		if (mode == "permanent") Assert.IsNull(application.GetType().GetProperty("Seconds")!.GetValue(application));
	}

	[TestMethod]
	public void PreparedNpc_DeadlineBecomesUnrepresentableAfterConfirmation_InvalidatesRawFence()
	{
		var clock = new AdmissionClock(DateTime.SpecifyKind(DateTime.MaxValue.AddSeconds(-240), DateTimeKind.Utc));
		using var time = RuntimeClock.Push(clock);
		var f = new MagicCastingFixture(); var spell = Configure(f, Template(f.World.Object)); PrepareSpatial(f);
		var effect = (CreateNPCEffect)Copy(f, spell).SpellEffects.Single();
		var token = (IMagicSpellEffectPreparedSelectionRawToken)effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object.Location)!;
		Assert.IsTrue(effect.TryConfirmPreparedSelection(f.Actor.Object, f.Actor.Object.Location, out var error), error);
		clock.Now = clock.Now.AddSeconds(120);
		Assert.IsFalse(token.IsCurrent);
	}

	private sealed class AdmissionClock(DateTime now) : TimeProvider
	{
		public DateTime Now { get; set; } = now;
		public override DateTimeOffset GetUtcNow() => new(Now);
	}

	private sealed class AdmissionRandom : Random
	{
		public int Calls { get; private set; }
		public override double NextDouble() { Calls++; return 0.5; }
		public override int Next(int minValue, int maxValue) { Calls++; return minValue; }
	}

	[TestMethod]
	public void Cast_LaterSelectionConfirmationChangesNpcTemplate_RawFenceRefusesWithoutPayment()
	{
		var f = new MagicCastingFixture(); var template = Template(f.World.Object); var spell = Configure(f, template); PrepareSpatial(f);
		var registry = (IDictionary<string, Func<XElement, IMagicSpell, IMagicSpellEffectTemplate>>)typeof(SpellEffectFactory)
			.GetField("_loadTimeFactories", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
		const string type = "npclaterconfirmationtest";
		Assert.IsFalse(registry.ContainsKey(type));
		registry.Add(type, (_, _) => new LaterConfirmation(() => template.SelectedSdesc = "Changed by a later confirmation"));
		try
		{
			((List<IMagicSpellEffectTemplate>)typeof(MagicSpell).GetField("_spellEffects", BindingFlags.NonPublic | BindingFlags.Instance)!
				.GetValue(spell)!).Add(new LaterConfirmation(() => template.SelectedSdesc = "Changed by a later confirmation"));
			var result = f.Service.Cast(new(f.Actor.Object, f.Earth.Id, spell.Id, 3, true, ""));
			Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
			StringAssert.Contains(result.Message, "changed during admission");
			Assert.AreEqual("Changed by a later confirmation", template.SelectedSdesc);
			Assert.IsNull(result.OperationId); Assert.IsTrue(f.Balances.Values.All(x => x == 100));
			Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Store.Operations.Count); Assert.AreEqual(0, f.Flushes);
		}
		finally { registry.Remove(type); }
	}

	private sealed record LaterToken : IMagicSpellEffectPreparedSelectionToken;
	private sealed class LaterConfirmation(Action mutate) : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection
	{
		public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient) => new LaterToken();
		public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
			IPerceivable recipient, out string? error) { error = null; return true; }
		public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
		{ mutate(); error = null; return true; }
		public bool IsInstantaneous => true;
		public bool RequiresTarget => true;
		public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => true;
		public XElement SaveToXml() => new("Effect", new XAttribute("type", "npclaterconfirmationtest"));
		public IMagicSpellEffectTemplate Clone() => new LaterConfirmation(mutate);
		public bool BuildingCommand(ICharacter actor, StringStack command) => false;
		public string Show(ICharacter actor) => "A test confirmation.";
		public IMagicSpellEffect GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
			SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters) => throw new AssertFailedException();
	}

	private static void PrepareSpatial(MagicCastingFixture f)
	{
		var room = f.Actor.Object.Location;
		Mock.Get(room).SetupGet(x => x.Gameworld).Returns(f.World.Object);
		Mock.Get(room).SetupGet(x => x.RouteDefinition).Returns((IRouteRoomDefinition)null!);
		f.Actor.SetupGet(x => x.SpatialLocation).Returns(() => new SpatialLocation(room, f.Actor.Object.RoomLayer));
	}

	private static MagicSpell Copy(MagicCastingFixture f, MagicSpell spell) => (MagicSpell)typeof(MagicSpell)
		.GetMethod("CastingCopy", BindingFlags.NonPublic | BindingFlags.Instance)!
		.Invoke(spell, [f.Actor.Object, f.Traits[0], 3, SpellPower.Insignificant, Difficulty.Automatic, 2])!;
}
