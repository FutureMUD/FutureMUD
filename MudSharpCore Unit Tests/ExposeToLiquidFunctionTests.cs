#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ExposeToLiquidFunctionTests
{
	[TestMethod]
	public void Execute_ValidItemLiquidAndVolume_ExposesItemFromOnTop()
	{
		var liquid = new Mock<ILiquid>();
		liquid.SetupGet(x => x.Id).Returns(65);
		liquid.SetupGet(x => x.Density).Returns(1.0);
		var liquids = new Mock<IUneditableAll<ILiquid>>();
		liquids.Setup(x => x.Get(65)).Returns(liquid.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Liquids).Returns(liquids.Object);
		var item = new Mock<IGameItem>();

		var function = Compile(
			gameworld.Object,
			new ConstantFunction(item.Object, ProgVariableTypes.Item),
			new ConstantFunction(new NumberVariable(65.0M)),
			new ConstantFunction(new NumberVariable(0.025M)));

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(true, function.Result.GetObject);
		item.Verify(x => x.ExposeToLiquid(
			It.Is<LiquidMixture>(mixture =>
				Math.Abs(mixture.TotalVolume - 0.025) < 0.000001 &&
				mixture.Instances.Single().Liquid == liquid.Object),
			null,
			LiquidExposureDirection.FromOnTop), Times.Once);
	}

	[TestMethod]
	public void Execute_InvalidLiquidOrVolume_ReturnsFalseWithoutExposure()
	{
		var liquid = new Mock<ILiquid>();
		liquid.SetupGet(x => x.Density).Returns(1.0);
		var liquids = new Mock<IUneditableAll<ILiquid>>();
		liquids.Setup(x => x.Get(65)).Returns(liquid.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Liquids).Returns(liquids.Object);
		var item = new Mock<IGameItem>();

		var unknownLiquid = Compile(
			gameworld.Object,
			new ConstantFunction(item.Object, ProgVariableTypes.Item),
			new ConstantFunction(new NumberVariable(999.0M)),
			new ConstantFunction(new NumberVariable(0.025M)));
		var zeroVolume = Compile(
			gameworld.Object,
			new ConstantFunction(item.Object, ProgVariableTypes.Item),
			new ConstantFunction(new NumberVariable(65.0M)),
			new ConstantFunction(new NumberVariable(0.0M)));

		Assert.AreEqual(StatementResult.Normal, unknownLiquid.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(false, unknownLiquid.Result.GetObject);
		Assert.AreEqual(StatementResult.Normal, zeroVolume.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(false, zeroVolume.Result.GetObject);
		item.Verify(x => x.ExposeToLiquid(
			It.IsAny<LiquidMixture>(),
			It.IsAny<IBodypart>(),
			It.IsAny<LiquidExposureDirection>()), Times.Never);
	}

	private static IFunction Compile(IFuturemud gameworld, params IFunction[] parameters)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var types = parameters.Select(x => x.ReturnType).ToList();
		var compiler = FutureProg.GetFunctionCompilerInformations()
			.Single(x => x.FunctionName.EqualTo("exposetoliquid") &&
			             x.Parameters.SequenceEqual(types, FutureProgVariableComparer.Instance));
		return compiler.CompilerFunction(parameters.ToList(), gameworld);
	}

	private sealed class ConstantFunction : IFunction
	{
		public ConstantFunction(IProgVariable result, ProgVariableTypes? returnType = null)
		{
			Result = result;
			ReturnType = returnType ?? result.Type;
		}

		public IProgVariable Result { get; private set; }
		public ProgVariableTypes ReturnType { get; }
		public string ErrorMessage => string.Empty;
		public StatementResult ExpectedResult => StatementResult.Normal;

		public StatementResult Execute(IVariableSpace variables) => StatementResult.Normal;
		public bool IsReturnOrContainsReturnOnAllBranches() => false;
	}
}
