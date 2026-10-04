#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellAnimationControlTests
{
	private static readonly DateTime Start = new(2026, 10, 4, 0, 0, 0, DateTimeKind.Utc);
	private static SpellOwnedLifecycle Life(string? control = "2026-10-04T01:29:59.0000000Z") => new(
		new(Guid.NewGuid(), 1, 3, 2, "raise-servitor", SpellLifecycleMode.TemporaryCleanup, Start, Start.AddSeconds(7799),
			new XElement("CorpseAnimation", new XAttribute("version", 1), new XAttribute("corpse", 4), new XAttribute("owner", 9),
				new XAttribute("body", 5), new XAttribute("cell", 6), new XAttribute("layer", 0),
				new XElement("Source", new XElement("Presentation", control is null ? null : new XElement("ControlUntilUtc", control)).ToString())).ToString()),
		[new(SpellOwnedEntityKind.CharacterInstance, 3)], SpellLifecycleState.Active, null, null, null, Start, 1, "");

	[DataTestMethod]
	[DataRow(-1, false)]
	[DataRow(0, true)]
	[DataRow(5398, true)]
	[DataRow(5399, false)]
	[DataRow(7798, false)]
	[DataRow(7799, false)]
	public void CommandGrant_UsesSeparateAbsoluteControlDeadline(int seconds, bool allowed) =>
		Assert.AreEqual(allowed, SpellOwnedCorpseAnimationService.HasCommandGrant(Life(), 3, 2, Start.AddSeconds(seconds)));

	[DataTestMethod]
	[DataRow(null)]
	[DataRow("invalid")]
	[DataRow("2026-10-04T01:29:59")]
	[DataRow("2026-10-04T04:00:00Z")]
	[DataRow("2026-10-03T23:59:59Z")]
	public void CommandGrant_MissingMalformedOrUnboundedControlFailsClosed(string? deadline) =>
		Assert.IsFalse(SpellOwnedCorpseAnimationService.HasCommandGrant(Life(deadline), 3, 2, Start));

	[TestMethod]
	public void CommandGrant_RequiresCreatorExactInstanceAndActiveJournal()
	{
		Assert.IsFalse(SpellOwnedCorpseAnimationService.HasCommandGrant(Life(), 3, 9, Start));
		Assert.IsFalse(SpellOwnedCorpseAnimationService.HasCommandGrant(Life(), 4, 2, Start));
		foreach (var state in new[] { SpellLifecycleState.Retiring, SpellLifecycleState.RemainsPending, SpellLifecycleState.Completed })
			Assert.IsFalse(SpellOwnedCorpseAnimationService.HasCommandGrant(Life() with { State = state }, 3, 2, Start));
		Assert.IsFalse(SpellOwnedCorpseAnimationService.HasCommandGrant(Life() with { Entities = [new(SpellOwnedEntityKind.AutonomousCharacter, 3)] }, 3, 2, Start));
	}

	[DataTestMethod]
	[DataRow(1, 6599, 4199)]
	[DataRow(2, 7199, 4799)]
	[DataRow(3, 7799, 5399)]
	[DataRow(4, 8399, 5999)]
	[DataRow(5, 8999, 6599)]
	[DataRow(6, 9599, 7199)]
	[DataRow(7, 10199, 7799)]
	public void StockTiming_BindsSelectedGradeToHistoricalFreshAffectDeadline(int grade, int lifetime, int control)
	{
		var world = Mock.Of<IFuturemud>(); var actor = Mock.Of<ICharacter>(); var trait = Mock.Of<ITraitDefinition>();
		double Evaluate(string formula) => CastingNumerics.Bind(new TraitExpression(formula, world), trait, grade,
			SpellPower.Standard, "stock", world).Evaluate(actor);
		Assert.AreEqual(lifetime, Evaluate(ArmageddonRaiseServitorStock.LifetimeSeconds));
		Assert.AreEqual(control, Evaluate(ArmageddonRaiseServitorStock.ControlSeconds));
	}

	[TestMethod]
	public void StockDefinition_ContainsSeparateControlNoReagentAndBoundedOrders()
	{
		var root = ArmageddonRaiseServitorStock.Definition(1, 2, 3, 4, 5);
		var life = root.Element("Effects")!.Element("Effect")!.Element("Lifecycle")!;
		Assert.AreEqual(ArmageddonRaiseServitorStock.Key, root.Element("StockIdentity")!.Value);
		Assert.AreEqual(ArmageddonRaiseServitorStock.LifetimeSeconds, life.Element("Seconds")!.Value);
		Assert.AreEqual(ArmageddonRaiseServitorStock.ControlSeconds, life.Element("Control")!.Element("Seconds")!.Value);
		Assert.AreEqual("3", life.Element("Control")!.Element("EligibilityProg")!.Value);
		Assert.AreEqual("true", life.Element("FollowCaster")!.Value);
		Assert.IsFalse(root.Element("Plan")!.Elements().Any());
		Assert.AreEqual(7, root.Element("ControlledPower")!.Elements("Grade").Count());
		CollectionAssert.Contains(ArmageddonRaiseServitorStock.Commands.ToArray(), "hit");
		CollectionAssert.DoesNotContain(ArmageddonRaiseServitorStock.Commands.ToArray(), "bodyguard");
		CollectionAssert.DoesNotContain(ArmageddonRaiseServitorStock.Commands.ToArray(), "quit");
	}
}
