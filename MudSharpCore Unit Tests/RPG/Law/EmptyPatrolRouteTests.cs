#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Law;
using MudSharp.RPG.Law.PatrolStrategies;

namespace MudSharp_Unit_Tests.RPG.Law;

[TestClass]
public class EmptyPatrolRouteTests
{
	[DataTestMethod]
	[DataRow(0, 0)]
	[DataRow(1, 1)]
	[DataRow(1, 2)]
	public void DoorDutiesSelectEnforcers_EmptyRoute_ReturnsNoEnforcers(int available, int requested)
	{
		var fixture = new StatusFixture();
		var route = fixture.EmptyDoorRoute();
		var pool = Enumerable.Range(0, available).Select(_ => Mock.Of<ICharacter>()).ToList();

		Assert.AreEqual("the route has no patrol nodes", route.WhyCannotBeginPatrol());
		Assert.AreEqual(0, route.PatrolStrategy.SelectEnforcers(route, pool, requested).Count());
		Assert.AreEqual(available, pool.Count);
	}

	[DataTestMethod]
	[DataRow(1, 1)]
	[DataRow(2, 2)]
	public void DoorDutiesSelectEnforcers_ValidRoute_PreservesLocalPreferenceAndFallback(int requested, int expected)
	{
		var fixture = new StatusFixture();
		var node = Mock.Of<ICell>();
		fixture.Route.SetupGet(x => x.PatrolNodes).Returns([node]);
		var local = Mock.Of<ICharacter>(x => x.Location == node);
		var remote = Mock.Of<ICharacter>(x => x.Location == Mock.Of<ICell>());
		var pool = new[] { remote, local };
		var strategy = TestObjectFactory.CreateUninitialized<DoorDutiesPatrolStrategy>();

		var selected = strategy.SelectEnforcers(fixture.Route.Object, pool, requested).ToList();

		Assert.AreEqual(expected, selected.Count);
		Assert.AreEqual(expected, selected.Distinct().Count());
		Assert.IsTrue(selected.All(pool.Contains));
		if (requested > 0)
		{
			Assert.AreSame(local, selected[0]);
		}
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void LegalStatus_ReadyEmptyDoorRouteWithFreeEnforcer_ReportsNodeProblem(bool administrator)
	{
		var fixture = new StatusFixture(administrator);
		fixture.Routes.Add(fixture.EmptyDoorRoute());

		fixture.RunStatus();

		StringAssert.Contains(fixture.Message, "the route has no patrol nodes");
		StringAssert.Contains(fixture.Message, "Patrol Route");
		StringAssert.Contains(fixture.Message, "Free Patrol Enforcers: 1");
		Assert.IsFalse(fixture.Message.Contains("Patrol Staffing"));
		Assert.AreEqual(administrator, fixture.Message.Contains("(#42)"));
	}

	[TestMethod]
	public void LegalStatus_SetupBlockedRoute_DoesNotInvokeStrategySelection()
	{
		var fixture = new StatusFixture();
		fixture.AddRoute("the start patrol prog returned false");
		fixture.Strategy.Setup(x => x.SelectEnforcers(It.IsAny<IPatrolRoute>(), It.IsAny<IEnumerable<ICharacter>>(), 1))
			.Throws(new InvalidOperationException("Selection must not run for a setup-blocked route"));

		fixture.RunStatus();

		StringAssert.Contains(fixture.Message, "the start patrol prog returned false");
		Assert.IsFalse(fixture.Message.Contains("Patrol Staffing"));
		fixture.VerifySelection(Times.Never());
	}

	[DataTestMethod]
	[DataRow("crime-targeted routes wait for a matching reported crime")]
	[DataRow("the current time of day (Morning) is not one of the route's allowed times")]
	[DataRow("there are no due condemned prisoners")]
	[DataRow("there is already an active execution patrol")]
	[DataRow("")]
	public void LegalStatus_StartableOrWaitingRoute_StillReportsSelectableStaffingShortage(string reason)
	{
		var fixture = new StatusFixture();
		fixture.AddRoute(reason);
		fixture.Strategy.Setup(x => x.SelectEnforcers(fixture.Route.Object, It.IsAny<IEnumerable<ICharacter>>(), 1))
			.Returns(Array.Empty<ICharacter>());

		fixture.RunStatus();

		StringAssert.Contains(fixture.Message, "Patrol Staffing");
		StringAssert.Contains(fixture.Message, "requires 1 selectable Guard enforcers");
		StringAssert.Contains(fixture.Message, "only 0 match the strategy selection rules");
		Assert.IsFalse(fixture.Message.Contains("cannot begin:"));
		fixture.VerifySelection(Times.Once());
	}

	[TestMethod]
	public void LegalStatus_ValidStaffedRoute_DoesNotReportStaffingProblem()
	{
		var fixture = new StatusFixture();
		fixture.AddRoute(string.Empty);
		fixture.Strategy.Setup(x => x.SelectEnforcers(fixture.Route.Object, It.IsAny<IEnumerable<ICharacter>>(), 1))
			.Returns(new[] { fixture.Enforcer.Object });

		fixture.RunStatus();

		Assert.IsFalse(fixture.Message.Contains("Patrol Staffing"));
		fixture.VerifySelection(Times.Once());
	}

	[TestMethod]
	public void LegalStatus_InsufficientFreeEnforcers_ReportsRawShortageWithoutSelection()
	{
		var fixture = new StatusFixture();
		fixture.AddRoute(string.Empty);
		fixture.Numbers[fixture.Enforcement.Object] = 2;

		fixture.RunStatus();

		StringAssert.Contains(fixture.Message, "Patrol Staffing");
		StringAssert.Contains(fixture.Message, "requires 2 Guards enforcers, but only 1 are free");
		fixture.VerifySelection(Times.Never());
	}

	private sealed class StatusFixture
	{
		public Mock<IFuturemud> World { get; } = new();
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<INPC> Enforcer { get; } = new();
		public Mock<ILegalAuthority> Legal { get; } = new();
		public Mock<IEnforcementAuthority> Enforcement { get; } = new();
		public Mock<IPatrolRoute> Route { get; } = new();
		public Mock<IPatrolStrategy> Strategy { get; } = new();
		public Counter<IEnforcementAuthority> Numbers { get; } = new();
		public List<IPatrolRoute> Routes { get; } = new();
		public string Message { get; private set; } = string.Empty;

		public StatusFixture(bool administrator = false)
		{
			Legal.SetupGet(x => x.Id).Returns(7);
			Enforcer.SetupGet(x => x.Id).Returns(5);
			var authorities = new All<ILegalAuthority>();
			authorities.Add(Legal.Object);
			World.SetupGet(x => x.LegalAuthorities).Returns(authorities);
			var npcs = new All<ICharacter>();
			npcs.Add(Enforcer.Object);
			World.SetupGet(x => x.NPCs).Returns(npcs);
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(administrator);
			Actor.Setup(x => x.GetFormat(It.IsAny<Type>())).Returns<Type>(CultureInfo.InvariantCulture.GetFormat);
			var account = new Mock<IAccount>();
			account.SetupGet(x => x.LineFormatLength).Returns(240);
			account.SetupGet(x => x.InnerLineFormatLength).Returns(240);
			Actor.SetupGet(x => x.Account).Returns(account.Object);
			var output = new Mock<IOutputHandler>();
			output.Setup(x => x.Send(It.IsAny<string>(), true, false))
				.Callback<string, bool, bool>((message, _, _) => Message = message.StripANSIColour());
			Actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
			Legal.SetupGet(x => x.Name).Returns("Test");
			Legal.SetupGet(x => x.Gameworld).Returns(World.Object);
			Legal.SetupGet(x => x.PatrolRoutes).Returns(Routes);
			Legal.SetupGet(x => x.EnforcementAuthorities).Returns([Enforcement.Object]);
			Legal.Setup(x => x.GetEnforcementAuthority(It.IsAny<ICharacter>())).Returns(Enforcement.Object);
			Enforcement.SetupGet(x => x.Id).Returns(3);
			Enforcement.SetupGet(x => x.Name).Returns("Guard");
			Enforcer.SetupGet(x => x.AIs).Returns([TestObjectFactory.CreateUninitialized<EnforcerAI>()]);
			Enforcer.Setup(x => x.AffectedBy<EnforcerEffect>(Legal.Object)).Returns(true);
			Numbers[Enforcement.Object] = 1;
			Route.SetupGet(x => x.Id).Returns(42);
			Route.SetupGet(x => x.Name).Returns("Door Post");
			Route.SetupGet(x => x.IsReady).Returns(true);
			Route.SetupGet(x => x.PatrolNodes).Returns([Mock.Of<ICell>()]);
			Route.SetupGet(x => x.PatrollerNumbers).Returns(Numbers);
			Route.SetupGet(x => x.PatrolStrategy).Returns(Strategy.Object);
			Strategy.SetupGet(x => x.Name).Returns("ArmedPatrol");
		}

		public PatrolRoute EmptyDoorRoute()
		{
			// Exercise the real route and selector without constructing unrelated inventory templates
			// from Futuremud.Games or invoking the database-writing route constructor.
			var route = TestObjectFactory.CreateUninitialized<PatrolRoute>();
			route.Id = 42;
			typeof(FrameworkItem).GetField("_name", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(route, "Door Post");
			typeof(PatrolRoute).GetField("_patrolNodes", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(route, new List<ICell>());
			typeof(PatrolRoute).GetField("<PatrollerNumbers>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(route, Numbers);
			typeof(PatrolRoute).GetProperty(nameof(PatrolRoute.IsReady))!.SetValue(route, true);
			typeof(PatrolRoute).GetProperty(nameof(PatrolRoute.PatrolStrategy))!.SetValue(route, TestObjectFactory.CreateUninitialized<DoorDutiesPatrolStrategy>());
			return route;
		}

		public void AddRoute(string reason)
		{
			Route.Setup(x => x.WhyCannotBeginPatrol()).Returns(reason);
			Routes.Add(Route.Object);
		}

		public void RunStatus()
		{
			var command = typeof(LegalModule).GetMethod("LegalStatus", BindingFlags.Static | BindingFlags.NonPublic)!
				.CreateDelegate<Action<ICharacter, string>>();
			command(Actor.Object, "legalstatus");
		}

		public void VerifySelection(Times times)
		{
			Strategy.Verify(x => x.SelectEnforcers(It.IsAny<IPatrolRoute>(), It.IsAny<IEnumerable<ICharacter>>(), It.IsAny<int>()), times);
		}
	}
}
