#nullable enable
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Planes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CurrentPlaneIdFutureProgTests
{
	[TestMethod]
	[DataRow(true, true, 1L)]
	[DataRow(false, true, 3L)]
	[DataRow(false, false, 2L)]
	public void NativeCurrentPlaneId_UsesDefaultThenDisplayOrderThenIdentity(bool defaultPresent, bool differentOrder, long expected)
	{
		var world = new Mock<IFuturemud>(); var actor = new Mock<ICharacter>(); var body = new Mock<IBody>();
		var planes = new All<IPlane>();
		foreach (var id in new[] { 1L, 2L, 3L })
		{
			var plane = new Mock<IPlane>(); plane.SetupGet(x => x.Id).Returns(id); plane.SetupGet(x => x.Name).Returns("renamed " + id);
			plane.SetupGet(x => x.DisplayOrder).Returns(id == 3 && differentOrder ? 0 : 10); planes.Add(plane.Object);
		}
		world.SetupGet(x => x.Planes).Returns(planes); world.SetupGet(x => x.DefaultPlane).Returns(planes.Get(1));
		actor.SetupGet(x => x.Gameworld).Returns(world.Object); actor.SetupGet(x => x.Body).Returns(body.Object);
		var presence = PlanarPresenceDefinition.DefaultMaterial(defaultPresent ? new[] { 3L, 2L, 1L } : new[] { 3L, 2L });
		body.SetupGet(x => x.BasePlanarPresence).Returns(presence);
		Assert.AreEqual(expected, Execute(world.Object, actor.Object));
		Assert.AreSame(presence, body.Object.BasePlanarPresence);
		Assert.AreEqual(actor.Object.CurrentPlane().Id, expected);
	}

	[TestMethod]
	public void NativeCurrentPlaneId_NoSubjectOrAvailablePlaneReturnsZero()
	{
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.Planes).Returns(new All<IPlane>());
		var actor = new Mock<ICharacter>(); var body = new Mock<IBody>();
		actor.SetupGet(x => x.Gameworld).Returns(world.Object); actor.SetupGet(x => x.Body).Returns(body.Object);
		body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(99));
		Assert.AreEqual(0L, Execute(world.Object, actor.Object)); Assert.AreEqual(0L, Execute(world.Object, null));
	}

	[TestMethod]
	public void NativeCurrentPlaneId_ParameterFailurePropagatesWithoutReadingSubject()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var parameter = new Mock<IFunction>(); parameter.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Perceivable);
		parameter.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Error);
		var function = FutureProg.GetFunctionCompilerInformations().Single(x => x.FunctionName == "currentplaneid")
			.CompilerFunction(new[] { parameter.Object }, Mock.Of<IFuturemud>());
		Assert.AreEqual(StatementResult.Error, function.Execute(Mock.Of<IVariableSpace>()));
		parameter.VerifyGet(x => x.Result, Times.Never);
	}

	private static long Execute(IFuturemud world, IPerceivable? subject)
	{
		FutureProgTestBootstrap.EnsureInitialised(); var variable = new Mock<IProgVariable>(); variable.SetupGet(x => x.GetObject).Returns(subject);
		var parameter = new Mock<IFunction>(); parameter.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Perceivable);
		parameter.SetupGet(x => x.Result).Returns(variable.Object); parameter.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Normal);
		var registration = FutureProg.GetFunctionCompilerInformations().Single(x => x.FunctionName == "currentplaneid");
		CollectionAssert.AreEqual(new[] { ProgVariableTypes.Perceivable }, registration.Parameters.ToArray());
		var function = registration.CompilerFunction(new[] { parameter.Object }, world);
		Assert.AreEqual(ProgVariableTypes.Number, function.ReturnType); Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		return Convert.ToInt64(function.Result.GetObject);
	}
}
