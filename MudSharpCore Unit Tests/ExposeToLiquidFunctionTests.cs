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
	public void Execute_ItemFiftyPercentDry_SplitsFreshLiquidAndResidue()
	{
		var residue = new Mock<ISolid>();
		var liquid = CreateDryableLiquid(residue.Object);
		var gameworld = CreateGameworld(liquid.Object);
		var state = new SurfaceLiquidState(gameworld.Object);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.SurfaceLiquidState).Returns(state);

		var function = Compile(
			gameworld.Object,
			new ConstantFunction(item.Object, ProgVariableTypes.Item),
			new ConstantFunction(new NumberVariable(65.0M)),
			new ConstantFunction(new NumberVariable(0.1M)),
			new ConstantFunction(new NumberVariable(50.0M)));

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(true, function.Result.GetObject);
		item.Verify(x => x.ExposeToLiquid(
			It.Is<LiquidMixture>(mixture => Math.Abs(mixture.TotalVolume - 0.05) < 0.000001),
			null,
			LiquidExposureDirection.FromOnTop), Times.Once);
		Assert.AreEqual(0.0025, state.ResidueWeight, 0.000001);
		Assert.AreEqual(0.0, state.LiquidVolume, 0.000001);
	}

	[TestMethod]
	public void Execute_TextItemVolumeFullyDry_AddsResidueWithoutFreshExposure()
	{
		var residue = new Mock<ISolid>();
		var liquid = CreateDryableLiquid(residue.Object);
		var parseSuccess = true;
		var unitManager = new Mock<IUnitManager>();
		unitManager
			.Setup(x => x.GetBaseUnits("50 ml", UnitType.FluidVolume, out parseSuccess))
			.Returns(0.05);
		var gameworld = CreateGameworld(liquid.Object);
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		var state = new SurfaceLiquidState(gameworld.Object);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.SurfaceLiquidState).Returns(state);

		var function = Compile(
			gameworld.Object,
			new ConstantFunction(item.Object, ProgVariableTypes.Item),
			new ConstantFunction(new NumberVariable(65.0M)),
			new ConstantFunction(new TextVariable("50 ml")),
			new ConstantFunction(new NumberVariable(100.0M)));

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(true, function.Result.GetObject);
		item.Verify(x => x.ExposeToLiquid(
			It.IsAny<LiquidMixture>(),
			It.IsAny<IBodypart>(),
			It.IsAny<LiquidExposureDirection>()), Times.Never);
		Assert.AreEqual(0.0025, state.ResidueWeight, 0.000001);
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
	public void Execute_CharacterFiftyPercentDry_PlacesResidueOnOutermostGarment()
	{
		var residue = new Mock<ISolid>();
		var liquid = CreateDryableLiquid(residue.Object);
		var gameworld = CreateGameworld(liquid.Object);
		var bodypart = new Mock<IExternalBodypart>();
		var innerItem = new Mock<IGameItem>();
		var outerState = new SurfaceLiquidState(gameworld.Object);
		var outerItem = new Mock<IGameItem>();
		outerItem.SetupGet(x => x.SurfaceLiquidState).Returns(outerState);
		var bodyState = new SurfaceLiquidState(gameworld.Object);
		var body = new Mock<IBody>();
		body.SetupGet(x => x.SurfaceLiquidState).Returns(bodyState);
		body.Setup(x => x.GetTargetBodypart("chest")).Returns(bodypart.Object);
		body.Setup(x => x.WornItemsFor(bodypart.Object)).Returns([innerItem.Object, outerItem.Object]);
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Body).Returns(body.Object);
		body.SetupGet(x => x.Actor).Returns(character.Object);

		var function = Compile(
			"exposecharactertoliquid",
			gameworld.Object,
			new ConstantFunction(character.Object, ProgVariableTypes.Character),
			new ConstantFunction(new NumberVariable(65.0M)),
			new ConstantFunction(new NumberVariable(0.1M)),
			new ConstantFunction(new TextVariable("chest")),
			new ConstantFunction(new NumberVariable(50.0M)));

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(true, function.Result.GetObject);
		body.Verify(x => x.ExposeToLiquid(
			It.Is<LiquidMixture>(mixture => Math.Abs(mixture.TotalVolume - 0.05) < 0.000001),
			bodypart.Object,
			LiquidExposureDirection.Irrelevant), Times.Once);
		Assert.AreEqual(0.0025, outerState.ResidueWeight, 0.000001);
		Assert.AreEqual(0.0, bodyState.ResidueWeight, 0.000001);
	}

	[TestMethod]
	public void Execute_CharacterFiftyPercentDry_PlacesResidueOnUncoveredBodypart()
	{
		var residue = new Mock<ISolid>();
		var liquid = CreateDryableLiquid(residue.Object);
		var gameworld = CreateGameworld(liquid.Object);
		var bodypart = new Mock<IExternalBodypart>();
		var bodyState = new SurfaceLiquidState(gameworld.Object);
		var body = new Mock<IBody>();
		body.SetupGet(x => x.SurfaceLiquidState).Returns(bodyState);
		body.Setup(x => x.GetTargetBodypart("left arm")).Returns(bodypart.Object);
		body.Setup(x => x.WornItemsFor(bodypart.Object)).Returns([]);
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Body).Returns(body.Object);
		body.SetupGet(x => x.Actor).Returns(character.Object);

		var function = Compile(
			"exposecharactertoliquid",
			gameworld.Object,
			new ConstantFunction(character.Object, ProgVariableTypes.Character),
			new ConstantFunction(new NumberVariable(65.0M)),
			new ConstantFunction(new NumberVariable(0.1M)),
			new ConstantFunction(new TextVariable("left arm")),
			new ConstantFunction(new NumberVariable(50.0M)));

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(true, function.Result.GetObject);
		Assert.AreEqual(0.0025, bodyState.ResidueWeight, 0.000001);
	}

	[TestMethod]
	public void Execute_InvalidDryPercentageOrResidue_ReturnsFalseWithoutMutation()
	{
		var residue = new Mock<ISolid>();
		var dryableLiquid = CreateDryableLiquid(residue.Object);
		var dryableGameworld = CreateGameworld(dryableLiquid.Object);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.SurfaceLiquidState).Returns(new SurfaceLiquidState(dryableGameworld.Object));
		var invalidPercentage = Compile(
			dryableGameworld.Object,
			new ConstantFunction(item.Object, ProgVariableTypes.Item),
			new ConstantFunction(new NumberVariable(65.0M)),
			new ConstantFunction(new NumberVariable(0.05M)),
			new ConstantFunction(new NumberVariable(101.0M)));
		var nonDryableLiquid = new Mock<ILiquid>();
		nonDryableLiquid.SetupGet(x => x.Density).Returns(1.0);
		var nonDryableGameworld = CreateGameworld(nonDryableLiquid.Object);
		var missingResidue = Compile(
			nonDryableGameworld.Object,
			new ConstantFunction(item.Object, ProgVariableTypes.Item),
			new ConstantFunction(new NumberVariable(65.0M)),
			new ConstantFunction(new NumberVariable(0.05M)),
			new ConstantFunction(new NumberVariable(50.0M)));

		Assert.AreEqual(StatementResult.Normal, invalidPercentage.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(false, invalidPercentage.Result.GetObject);
		Assert.AreEqual(StatementResult.Normal, missingResidue.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(false, missingResidue.Result.GetObject);
		item.Verify(x => x.ExposeToLiquid(
			It.IsAny<LiquidMixture>(),
			It.IsAny<IBodypart>(),
			It.IsAny<LiquidExposureDirection>()), Times.Never);
		Assert.AreEqual(0.0, item.Object.SurfaceLiquidState.ResidueWeight, 0.000001);
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

	private static Mock<ILiquid> CreateDryableLiquid(ISolid residue)
	{
		var liquid = new Mock<ILiquid>();
		liquid.SetupGet(x => x.Id).Returns(65);
		liquid.SetupGet(x => x.Density).Returns(1.0);
		liquid.SetupGet(x => x.DriedResidue).Returns(residue);
		liquid.SetupGet(x => x.ResidueVolumePercentage).Returns(0.05);
		return liquid;
	}

	private static Mock<IFuturemud> CreateGameworld(ILiquid liquid)
	{
		var liquids = new Mock<IUneditableAll<ILiquid>>();
		liquids.Setup(x => x.Get(65)).Returns(liquid);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Liquids).Returns(liquids.Object);
		return gameworld;
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
