#nullable enable

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PatronCastingTests
{
	[TestMethod]
	public void Patron_RefillBeforeBodyAssignmentDoesNotInspectCapabilities()
	{
		var actor = new Mock<MudSharp.Character.ICharacter>(MockBehavior.Strict);
		actor.SetupGet(x => x.Body).Returns((MudSharp.Body.IBody)null!);
		PatronMagicService.Refill(actor.Object);
		actor.VerifyGet(x => x.Capabilities, Times.Never);
	}

	private static MagicCastingFixture Fixture(bool fallback = false)
	{
		var f = new MagicCastingIncantationTests.SpeechFixture().F;
		var incantation = f.Spell.GradeProfile!.Incantation! with { Reach = "route-near", Element = "route-earth", Sphere = "route-protect", Mood = "route-shape",
			Aliases = ["route-stone"], Deliveries = [new("Shout", MudSharp.Form.Audio.AudioVolume.ExtremelyLoud, 1, 0)] };
		if (fallback) Assert.IsTrue(f.Spell.BuildingCommand(f.Actor.Object, new StringStack("grades incantation off")));
		f.Store.Acquired.Clear();
		var authority = Mock.Get(f.World.Object.FutureProgs.Get(1));
		authority.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		authority.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		f.Body.Setup(x => x.Shout(It.IsAny<IPerceivable>(), It.IsAny<string>(), It.IsAny<IEmote>()))
			.Callback<IPerceivable, string, IEmote>((target, text, emote) => f.Body.Object.Communications.Shout(f.Body.Object, target, text, emote));
		var casting = new XElement("Casting", new XAttribute("version", 1), new XAttribute("identity", Guid.NewGuid()),
			new XAttribute("enabled", true), new XAttribute("trait", 1), new XAttribute("source", 10), new XAttribute("reserve", 11),
			new XAttribute("passive", false), new XAttribute("startingVersion", 1), MagicCastingFixture.Admission(1),
			new XElement("Patron", JsonSerializer.Serialize(new PatronCastingPolicy(1, 1, 7, true, true, true, [1], Incantations: new Dictionary<long, ControlledSpellIncantation> { [1] = incantation }))));
		f.ActiveCapabilities.Clear();
		f.ActiveCapabilities.Add(f.NewCapability(3, 1, 11, false, casting));
		f.Actor.Setup(x => x.AddResource(It.IsAny<IMagicResource>(), It.IsAny<double>()))
			.Callback<IMagicResource, double>((resource, amount) => f.Balances[resource] += amount);
		return f;
	}

	[TestMethod]
	public void Patron_PermanentSpellUsesFixedRouteGradeWithoutPromotingOtherRoutes()
	{
		var f = Fixture();
		var quote = f.Service.Quote(f.Intent(7, false, 3));
		Assert.IsNotNull(quote.Invocation, quote.Reason);
		Assert.AreEqual(7, quote.Invocation.Grade);
		Assert.AreEqual(7, quote.Invocation.ControlledGrade);
		Assert.AreEqual("Shout", quote.Invocation.Delivery!.Method);
		Assert.AreEqual(0, f.Store.Acquired.Count);
		Assert.IsNotNull(f.Service.Preflight(f.Actor.Object, 3, 1, 6, false));
		Assert.IsNotNull(f.Service.Preflight(f.Actor.Object, 3, 1, 7, true));
		f.ActiveCapabilities.Add(f.Earth);
		Assert.IsNotNull(f.Service.Preflight(f.Actor.Object, 1, 1, 7, false), "Patron access must not create global acquisition.");
	}

	[TestMethod]
	public void Patron_CastingInTerritoryPaysNoReserveAndCreatesNoSkillOrMasteryOpportunity()
	{
		var f = Fixture();
		var resource = f.Resources.Single(x => x.Id == 11);
		f.Balances[resource] = 25;
		var result = f.Service.Cast(f.Intent(7, false, 3));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(100, f.Balances[resource]);
		Assert.AreEqual(0, f.SkillUses);
		Assert.AreEqual(0, f.Store.Acquired.Count);
		Assert.AreEqual(0, f.Store.Opportunities.Count);
	}

	[TestMethod]
	public void Patron_RemovedAuthorityImmediatelyRefusesPermanentSpell()
	{
		var f = Fixture();
		Mock.Get(f.World.Object.FutureProgs.Get(1)).Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(false);
		Assert.IsNotNull(f.Service.Preflight(f.Actor.Object, 3, 1, 7, false));
		Assert.AreEqual(0, f.Rolls);
	}

	[TestMethod]
	public void Patron_RouteFallbackSupportsNamedAndFormulaCastingWithoutChangingCanonicalProfile()
	{
		var f = Fixture(true);
		var quote = f.Service.Quote(f.Intent(7, false, 3));
		Assert.IsNotNull(quote.Invocation, quote.Reason);
		Assert.AreEqual("Shout", quote.Invocation.Delivery!.Method);
		StringAssert.Contains(quote.Invocation.Delivery.Incantation, "route-near");
		Assert.IsNull(f.Spell.GradeProfile!.Incantation);
		var other = f.NewSpell(2, "Unadmitted Other Stone", "<Effect type='boost' trait='3' bonus='0' context='0' />");
		Assert.IsTrue(other.BuildingCommand(f.Actor.Object, new StringStack("grades fixture")));
		Assert.IsTrue(other.BuildingCommand(f.Actor.Object, new StringStack("grades incantation fixture 1 route-near route-earth route-protect route-shape route-stone")));
		var formula = f.Service.CastFormula(f.Actor.Object, "mon route-near route-earth route-protect route-shape on self via 3", "Shout");
		Assert.AreEqual(MagicCastingStatus.Succeeded, formula!.Status, formula.Message);
		Assert.AreEqual(0, f.Store.Acquired.Count);
		f.ActiveCapabilities.Add(f.Earth);
		var foreign = f.Service.CastFormula(f.Actor.Object, "mon route-near route-earth route-protect route-shape on self via 1", "Shout");
		Assert.AreEqual(MagicCastingStatus.Refused, foreign!.Status, "Patron vocabulary leaked to an elemental route.");
		Assert.IsNull(f.Service.Quote(f.Intent(7, false, 3) with { Targets = "self trailing" }).Invocation, "Route-only speech must retain complete target parsing.");
	}

	[TestMethod]
	public void Patron_CanonicalIncantationTakesPrecedenceOverRouteFallback()
	{
		var f = Fixture();
		var quote = f.Service.Quote(f.Intent(7, false, 3));
		Assert.IsNotNull(quote.Invocation, quote.Reason);
		StringAssert.Contains(quote.Invocation.Delivery!.Incantation, "fm-near");
		Assert.IsFalse(quote.Invocation.Delivery.Incantation.Contains("route-near"));
	}
}
