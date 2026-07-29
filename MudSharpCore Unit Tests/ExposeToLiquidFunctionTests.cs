#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
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
	public void Execute_TextItemVolume_ConvertsFluidUnitsBeforeExposure()
	{
		var liquid = new Mock<ILiquid>();
		liquid.SetupGet(x => x.Density).Returns(1.0);
		var liquids = new Mock<IUneditableAll<ILiquid>>();
		liquids.Setup(x => x.Get(65)).Returns(liquid.Object);
		var parseSuccess = true;
		var unitManager = new Mock<IUnitManager>();
		unitManager
			.Setup(x => x.GetBaseUnits("25 ml", UnitType.FluidVolume, out parseSuccess))
			.Returns(0.025);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Liquids).Returns(liquids.Object);
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		var item = new Mock<IGameItem>();

		var function = Compile(
			gameworld.Object,
			new ConstantFunction(item.Object, ProgVariableTypes.Item),
			new ConstantFunction(new NumberVariable(65.0M)),
			new ConstantFunction(new TextVariable("25 ml")));

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(true, function.Result.GetObject);
		item.Verify(x => x.ExposeToLiquid(
			It.Is<LiquidMixture>(mixture => Math.Abs(mixture.TotalVolume - 0.025) < 0.000001),
			null,
			LiquidExposureDirection.FromOnTop), Times.Once);
	}

	[TestMethod]
	public void Execute_ValidCharacterLiquidVolumeAndBodypart_UsesBodypartExposure()
	{
		var liquid = new Mock<ILiquid>();
		liquid.SetupGet(x => x.Density).Returns(1.0);
		var liquids = new Mock<IUneditableAll<ILiquid>>();
		liquids.Setup(x => x.Get(65)).Returns(liquid.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Liquids).Returns(liquids.Object);
		var bodypart = new Mock<IExternalBodypart>();
		var body = new Mock<IBody>();
		body.Setup(x => x.GetTargetBodypart("left arm")).Returns(bodypart.Object);
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Body).Returns(body.Object);

		var function = Compile(
			"exposecharactertoliquid",
			gameworld.Object,
			new ConstantFunction(character.Object, ProgVariableTypes.Character),
			new ConstantFunction(new NumberVariable(65.0M)),
			new ConstantFunction(new NumberVariable(0.025M)),
			new ConstantFunction(new TextVariable("left arm")));

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(true, function.Result.GetObject);
		body.Verify(x => x.ExposeToLiquid(
			It.Is<LiquidMixture>(mixture =>
				Math.Abs(mixture.TotalVolume - 0.025) < 0.000001 &&
				mixture.Instances.Single().Liquid == liquid.Object),
			bodypart.Object,
			LiquidExposureDirection.Irrelevant), Times.Once);
	}

	[TestMethod]
	public void Execute_TextCharacterVolume_ConvertsFluidUnitsBeforeBodypartExposure()
	{
		var liquid = new Mock<ILiquid>();
		liquid.SetupGet(x => x.Density).Returns(1.0);
		var liquids = new Mock<IUneditableAll<ILiquid>>();
		liquids.Setup(x => x.Get(65)).Returns(liquid.Object);
		var parseSuccess = true;
		var unitManager = new Mock<IUnitManager>();
		unitManager
			.Setup(x => x.GetBaseUnits("1 fl oz", UnitType.FluidVolume, out parseSuccess))
			.Returns(0.0284131);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Liquids).Returns(liquids.Object);
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		var bodypart = new Mock<IExternalBodypart>();
		var body = new Mock<IBody>();
		body.Setup(x => x.GetTargetBodypart("chest")).Returns(bodypart.Object);
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Body).Returns(body.Object);

		var function = Compile(
			"exposecharactertoliquid",
			gameworld.Object,
			new ConstantFunction(character.Object, ProgVariableTypes.Character),
			new ConstantFunction(new NumberVariable(65.0M)),
			new ConstantFunction(new TextVariable("1 fl oz")),
			new ConstantFunction(new TextVariable("chest")));

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(true, function.Result.GetObject);
		body.Verify(x => x.ExposeToLiquid(
			It.Is<LiquidMixture>(mixture => Math.Abs(mixture.TotalVolume - 0.0284131) < 0.000001),
			bodypart.Object,
			LiquidExposureDirection.Irrelevant), Times.Once);
	}

	[TestMethod]
	public void Execute_InvalidTextVolume_ReturnsFalseWithoutExposure()
	{
		var liquid = new Mock<ILiquid>();
		var liquids = new Mock<IUneditableAll<ILiquid>>();
		liquids.Setup(x => x.Get(65)).Returns(liquid.Object);
		var parseSuccess = false;
		var unitManager = new Mock<IUnitManager>();
		unitManager
			.Setup(x => x.GetBaseUnits("a bucket or so", UnitType.FluidVolume, out parseSuccess))
			.Returns(0.0);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Liquids).Returns(liquids.Object);
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		var item = new Mock<IGameItem>();

		var function = Compile(
			gameworld.Object,
			new ConstantFunction(item.Object, ProgVariableTypes.Item),
			new ConstantFunction(new NumberVariable(65.0M)),
			new ConstantFunction(new TextVariable("a bucket or so")));

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(false, function.Result.GetObject);
		item.Verify(x => x.ExposeToLiquid(
			It.IsAny<LiquidMixture>(),
			It.IsAny<IBodypart>(),
			It.IsAny<LiquidExposureDirection>()), Times.Never);
	}

	[TestMethod]
	public void Execute_InvalidCharacterBodypart_ReturnsFalseWithoutExposure()
	{
		var liquid = new Mock<ILiquid>();
		liquid.SetupGet(x => x.Density).Returns(1.0);
		var liquids = new Mock<IUneditableAll<ILiquid>>();
		liquids.Setup(x => x.Get(65)).Returns(liquid.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Liquids).Returns(liquids.Object);
		var body = new Mock<IBody>();
		body.Setup(x => x.GetTargetBodypart("spleen")).Returns(Mock.Of<IBodypart>());
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Body).Returns(body.Object);

		var function = Compile(
			"exposecharactertoliquid",
			gameworld.Object,
			new ConstantFunction(character.Object, ProgVariableTypes.Character),
			new ConstantFunction(new NumberVariable(65.0M)),
			new ConstantFunction(new NumberVariable(0.025M)),
			new ConstantFunction(new TextVariable("spleen")));

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(false, function.Result.GetObject);
		body.Verify(x => x.ExposeToLiquid(
			It.IsAny<LiquidMixture>(),
			It.IsAny<IBodypart>(),
			It.IsAny<LiquidExposureDirection>()), Times.Never);
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
		return Compile("exposetoliquid", gameworld, parameters);
	}

	private static IFunction Compile(string functionName, IFuturemud gameworld, params IFunction[] parameters)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var types = parameters.Select(x => x.ReturnType).ToList();
		var compiler = FutureProg.GetFunctionCompilerInformations()
			.Single(x => x.FunctionName.EqualTo(functionName) &&
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
