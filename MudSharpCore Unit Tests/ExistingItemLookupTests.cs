#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Functions.BuiltIn;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Character;
using MudSharp.FutureProg.Functions.Characters;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ExistingItemLookupTests
{
	[TestMethod]
	public void TargetCharacter_UsesNormalVisibleLocalTargetingIncludingDisambiguation()
	{
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.GetObject).Returns(actor.Object);
		var target = Mock.Of<ICharacter>(); actor.Setup(x => x.TargetActor("2.guard", PerceiveIgnoreFlags.None)).Returns(target);
		var function = new TargetCharacterFunction([new ConstantFunction(actor.Object), new ConstantFunction(new TextVariable("2.guard"))]);
		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreSame(target, function.Result);
		actor.Verify(x => x.TargetActor("2.guard", PerceiveIgnoreFlags.None), Times.Once);
	}

	[TestMethod]
	public void Lookup_UsesPersistentInstanceIdentityAndDoesNotLoadInvalidIds()
	{
		var world = new Mock<IFuturemud>(); var item = Mock.Of<IGameItem>();
		world.Setup(x => x.TryGetItem(42, true)).Returns(item);
		var argument = new Mock<IFunction>(); argument.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Number);
		argument.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Normal);
		argument.SetupGet(x => x.Result).Returns(new NumberVariable(42));
		var function = new GetExistingItemFunction([argument.Object], world.Object);
		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>())); Assert.AreSame(item, function.Result);
		foreach (var id in new[] { 0m, -1m, 42.5m })
		{
			argument.SetupGet(x => x.Result).Returns(new NumberVariable(id)); function.Execute(Mock.Of<IVariableSpace>());
			Assert.IsNull(function.Result);
		}
		world.Verify(x => x.TryGetItem(It.IsAny<long>(), true), Times.Once);
	}
}
