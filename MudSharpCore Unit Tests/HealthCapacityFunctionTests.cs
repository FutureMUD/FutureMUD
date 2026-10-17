#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions.Characters;
using MudSharp.Health;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HealthCapacityFunctionTests
{
	[DataTestMethod]
	[DataRow("health", false, 100.0)]
	[DataRow("stun", false, 200.0)]
	[DataRow("pain", false, 300.0)]
	[DataRow("stamina", false, 120.0)]
	[DataRow("health", true, 90.0)]
	[DataRow("stun", true, 180.0)]
	[DataRow("pain", true, 270.0)]
	[DataRow("stamina", true, 90.0)]
	public void Capacity_UsesCurrentNativeBodyAndWoundsWithoutMutation(string channel, bool remaining, double expected)
	{
		var actor = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		var strategy = new Mock<IHealthStrategy>();
		var wound = new Mock<IWound>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.HealthStrategy).Returns(strategy.Object);
		actor.SetupGet(x => x.Wounds).Returns(new[] { wound.Object });
		body.SetupGet(x => x.MaximumStamina).Returns(120.0);
		body.SetupGet(x => x.CurrentStamina).Returns(90.0);
		strategy.Setup(x => x.MaxHP(actor.Object)).Returns(100.0);
		strategy.Setup(x => x.MaxStun(actor.Object)).Returns(200.0);
		strategy.Setup(x => x.MaxPain(actor.Object)).Returns(300.0);
		wound.SetupGet(x => x.CurrentDamage).Returns(10.0);
		wound.SetupGet(x => x.CurrentStun).Returns(20.0);
		wound.SetupGet(x => x.CurrentPain).Returns(30.0);
		var function = new HealthCapacityFunction([Constant(actor.Object, ProgVariableTypes.Character),
			Constant(channel, ProgVariableTypes.Text)], remaining);

		Assert.AreEqual(StatementResult.Normal, function.Execute(new VariableSpace()));
		Assert.AreEqual((decimal)expected, function.Result.GetObject);
		body.VerifySet(x => x.CurrentStamina = It.IsAny<double>(), Times.Never);
		wound.VerifySet(x => x.CurrentDamage = It.IsAny<double>(), Times.Never);
	}

	[DataTestMethod]
	[DataRow(double.NaN)]
	[DataRow(double.PositiveInfinity)]
	[DataRow(double.NegativeInfinity)]
	[DataRow(-1.0)]
	[DataRow(0.0)]
	[DataRow(double.MaxValue)]
	public void Capacity_InvalidMaximumFailsInsteadOfReturningAFreePrice(double maximum)
	{
		var actor = new Mock<ICharacter>();
		var strategy = new Mock<IHealthStrategy>();
		actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
		actor.SetupGet(x => x.HealthStrategy).Returns(strategy.Object);
		strategy.Setup(x => x.MaxHP(actor.Object)).Returns(maximum);
		var function = new HealthCapacityFunction([Constant(actor.Object, ProgVariableTypes.Character),
			Constant("health", ProgVariableTypes.Text)], false);
		Assert.AreEqual(StatementResult.Error, function.Execute(new VariableSpace()));
		Assert.IsFalse(string.IsNullOrEmpty(function.ErrorMessage));
	}

	[TestMethod]
	public void Capacity_RejectsMissingActorAndUnknownChannelAndCompilesExactContracts()
	{
		var absent = new HealthCapacityFunction([Constant(null, ProgVariableTypes.Character),
			Constant("health", ProgVariableTypes.Text)], false);
		Assert.AreEqual(StatementResult.Error, absent.Execute(new VariableSpace()));
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
		actor.SetupGet(x => x.HealthStrategy).Returns(Mock.Of<IHealthStrategy>());
		var invalid = new HealthCapacityFunction([Constant(actor.Object, ProgVariableTypes.Character),
			Constant("unknown", ProgVariableTypes.Text)], false);
		Assert.AreEqual(StatementResult.Error, invalid.Execute(new VariableSpace()));
		FutureProgTestBootstrap.EnsureInitialised();
		foreach (var name in new[] { "healthcapacity", "healthremaining" })
		{
			var prog = new FutureProg(FutureProgTestBootstrap.Gameworld, "contract_" + name, ProgVariableTypes.Number,
				new[] { Tuple.Create(ProgVariableTypes.Character, "actor") }, $"return {name}(@actor, \"health\")");
			Assert.IsTrue(prog.Compile(), prog.CompileError);
		}
	}

	private static IFunction Constant(object? value, ProgVariableTypes type)
	{
		var variable = new Mock<IProgVariable>();
		variable.SetupGet(x => x.GetObject).Returns(value!);
		variable.SetupGet(x => x.Type).Returns(type);
		var function = new Mock<IFunction>();
		function.SetupGet(x => x.ReturnType).Returns(type);
		function.SetupGet(x => x.Result).Returns(variable.Object);
		function.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Normal);
		return function.Object;
	}
}
