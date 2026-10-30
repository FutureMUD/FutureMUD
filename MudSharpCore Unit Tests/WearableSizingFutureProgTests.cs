#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Size;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WearableSizingFutureProgTests
{
	[ClassInitialize]
	public static void Initialise(TestContext _) => FutureProgTestBootstrap.EnsureInitialised();

	[TestMethod]
	public void WearableFitGetters_ReturnStoredFitAndLoadValueSnapshot()
	{
		var item = CreateWearable(out var wearable);
		var fit = WearableFit.ForMeasurements(1.8, 72.5);
		wearable.SetupGet(x => x.Fit).Returns(fit);

		Assert.AreEqual(0.0M, Run("wearablefitscalar", item.Object).GetObject);
		Assert.AreEqual(1.8M, Run("wearablefitheight", item.Object).GetObject);
		Assert.AreEqual(72.5M, Run("wearablefitweight", item.Object).GetObject);
		Assert.AreEqual("Measurements", Run("wearablefitstate", item.Object).GetObject);
		Assert.AreEqual("measurements:1.8,72.5", Run("wearablefit", item.Object).GetObject);
	}

	[TestMethod]
	public void SetWearableFitForCharacter_UsesWearableFitPolicy()
	{
		var item = CreateWearable(out var wearable);
		var body = new Mock<IBody>();
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		character.SetupGet(x => x.GetObject).Returns(character.Object);
		character.SetupGet(x => x.Body).Returns(body.Object);
		var selectedFit = WearableFit.ForScalar(2.5);
		wearable.Setup(x => x.FitFor(body.Object)).Returns(selectedFit);

		Assert.AreEqual(true, Run("setwearablefit", item.Object, character.Object).GetObject);
		wearable.Verify(x => x.SetFit(selectedFit), Times.Once);
	}

	[TestMethod]
	public void SetWearableFitText_ParsesMeasurementSnapshotAndRejectsNegativeScalar()
	{
		var item = CreateWearable(out var wearable);

		Assert.AreEqual(true, Run("setwearablefit", item.Object, new TextVariable("measurements:1.72,64.5")).GetObject);
		wearable.Verify(x => x.SetFit(It.Is<WearableFit>(fit => fit.Kind == WearableFitKind.Measurements &&
			fit.Height == 1.72 && fit.Weight == 64.5)), Times.Once);
		Assert.AreEqual(false, Run("setwearablefit", item.Object, new TextVariable("-1")).GetObject);
		Assert.AreEqual(false, Run("setwearablefitscalar", item.Object, new NumberVariable(-1.0M)).GetObject);
		wearable.Verify(x => x.SetFit(It.IsAny<WearableFit>()), Times.Once);
	}

	[TestMethod]
	public void WearableFits_DelegatesPureCompatibilityCheckToWearable()
	{
		var item = CreateWearable(out var wearable);
		var body = new Mock<IBody>();
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		character.SetupGet(x => x.GetObject).Returns(character.Object);
		character.SetupGet(x => x.Body).Returns(body.Object);
		wearable.Setup(x => x.Fits(body.Object)).Returns(true);

		Assert.AreEqual(true, Run("wearablefits", item.Object, character.Object).GetObject);
		wearable.Verify(x => x.Fits(body.Object), Times.Once);
	}

	private static Mock<IGameItem> CreateWearable(out Mock<IWearable> wearable)
	{
		var item = new Mock<IGameItem>();
		item.As<IProgVariable>().SetupGet(x => x.Type).Returns(ProgVariableTypes.Item);
		item.As<IProgVariable>().SetupGet(x => x.GetObject).Returns(item.Object);
		item.SetupGet(x => x.Gameworld).Returns(Mock.Of<IFuturemud>());
		wearable = new Mock<IWearable>();
		item.Setup(x => x.GetItemType<IWearable>()).Returns(wearable.Object);
		return item;
	}

	private static IProgVariable Run(string name, params IProgVariable[] arguments)
	{
		var compiler = FutureProg.GetFunctionCompilerInformations().Single(x => x.FunctionName.EqualTo(name) &&
			x.Parameters.SequenceEqual(arguments.Select(x => x.Type), FutureProgVariableComparer.Instance));
		var parameters = arguments.Select(argument =>
		{
			var function = new Mock<IFunction>();
			function.SetupGet(x => x.ReturnType).Returns(argument.Type);
			function.SetupGet(x => x.Result).Returns(argument);
			function.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Normal);
			return function.Object;
		}).ToList();
		var compiled = compiler.CompilerFunction(parameters, Mock.Of<IFuturemud>());
		Assert.AreEqual(StatementResult.Normal, compiled.Execute(Mock.Of<IVariableSpace>()));
		return compiled.Result;
	}
}
