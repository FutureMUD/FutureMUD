#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Functions.BuiltIn;
using MudSharp.FutureProg.Variables;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PositionIdFunctionTests
{
	[DataTestMethod]
	[DataRow(0L), DataRow(1L), DataRow(2L), DataRow(3L), DataRow(4L), DataRow(5L), DataRow(6L), DataRow(7L), DataRow(8L),
	 DataRow(9L), DataRow(10L), DataRow(11L), DataRow(12L), DataRow(13L), DataRow(14L), DataRow(15L), DataRow(16L),
	 DataRow(17L), DataRow(18L), DataRow(19L), DataRow(20L)]
	public void NativePositions_QueryReturnsIdentityWithoutCharacterCallbacks(long id)
	{
		PositionState.SetupPositions();
		var actor = new Mock<ICharacter>(MockBehavior.Strict);
		actor.SetupGet(x => x.PositionState).Returns(PositionState.GetState(id));
		var function = new PositionIdFunction([Argument(actor.Object)]);
		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual((decimal)id, function.Result.GetObject);
		actor.VerifyGet(x => x.PositionState, Times.Once);
		actor.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void RepeatedInvocation_ReadsCurrentPostureOncePerCall()
	{
		PositionState.SetupPositions();
		IPositionState position = PositionState.GetState(9);
		var actor = new Mock<ICharacter>(MockBehavior.Strict);
		actor.SetupGet(x => x.PositionState).Returns(() => position);
		var function = new PositionIdFunction([Argument(actor.Object)]);
		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(9M, function.Result.GetObject);
		position = PositionState.GetState(16);
		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(16M, function.Result.GetObject);
		actor.VerifyGet(x => x.PositionState, Times.Exactly(2));
		actor.VerifyNoOtherCalls();
	}

	[DataTestMethod, DataRow("null-character"), DataRow("null-position"), DataRow("wrong-object"), DataRow("null-variable")]
	public void UnavailableInput_ReturnsZeroWithoutEligibilitySideEffects(string scenario)
	{
		var actor = new Mock<ICharacter>(MockBehavior.Strict);
		actor.SetupGet(x => x.PositionState).Returns((IPositionState)null!);
		var value = scenario == "null-position" ? actor.Object : scenario == "wrong-object" ? new object() : null;
		var function = new PositionIdFunction([scenario == "null-variable" ? Argument(null, nullResult: true) : Argument(value)]);
		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(0M, function.Result.GetObject);
		actor.VerifyGet(x => x.PositionState, scenario == "null-position" ? Times.Once() : Times.Never());
		actor.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void ParameterFailure_PropagatesBeforeReadingCharacter()
	{
		var parameter = new Mock<IFunction>(MockBehavior.Strict);
		parameter.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Error);
		parameter.SetupGet(x => x.ErrorMessage).Returns("Parameter refused.");
		var function = new PositionIdFunction([parameter.Object]);
		Assert.AreEqual(StatementResult.Error, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual("Parameter refused.", function.ErrorMessage);
		parameter.VerifyGet(x => x.Result, Times.Never);
	}

	[TestMethod]
	public void Registration_CompilesOnlyCharacterInputWithDocumentedNumberResult()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var metadata = FutureProg.GetFunctionCompilerInformations().Single(x => x.FunctionName == "positionid");
		CollectionAssert.AreEqual(new[] { ProgVariableTypes.Character }, metadata.Parameters.ToArray());
		Assert.AreEqual(ProgVariableTypes.Number, metadata.ReturnType);
		var prog = new FutureProg(FutureProgTestBootstrap.Gameworld, "position_identity_test", ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Character, "actor")], "return positionid(@actor)");
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		Assert.AreEqual(0M, prog.Execute((object)null!));
		var wrong = new FutureProg(FutureProgTestBootstrap.Gameworld, "position_wrong_type", ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Text, "actor")], "return positionid(@actor)");
		Assert.IsFalse(wrong.Compile());
	}

	private static IFunction Argument(object? value, bool nullResult = false)
	{
		var parameter = new Mock<IFunction>();
		parameter.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Normal);
		parameter.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Character);
		parameter.SetupGet(x => x.Result).Returns(nullResult ? null! : new ObjectValue(value));
		return parameter.Object;
	}

	private sealed class ObjectValue(object? value) : IProgVariable
	{
		public ProgVariableTypes Type => ProgVariableTypes.Character;
		public object GetObject => value!;
		public IProgVariable GetProperty(string property) => throw new NotSupportedException();
	}
}
