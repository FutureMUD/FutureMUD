#nullable enable
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SetLiquidFunctionTests
{
	[TestMethod]
	public void ThreeArgumentOverloadSetsPartialAndFullFinalVolumes()
	{
		var fixture = new Fixture();
		var partial = fixture.Compile(new NumberVariable(65), new NumberVariable(0.25M));
		var full = fixture.Compile(new NumberVariable(65), new NumberVariable(1.0M));

		Assert.IsTrue(fixture.Execute(partial));
		fixture.Container.VerifySet(x => x.LiquidMixture = It.Is<LiquidMixture>(m => m.TotalVolume == 0.25), Times.Once);
		Assert.IsTrue(fixture.Execute(full));
		fixture.Container.VerifySet(x => x.LiquidMixture = It.Is<LiquidMixture>(m => m.TotalVolume == 1.0), Times.Once);
	}

	[TestMethod]
	public void ThreeArgumentOverloadEmptiesOnlyForZeroAndAllowsValidLiquidAtZeroVolume()
	{
		var fixture = new Fixture();
		Assert.IsTrue(fixture.Execute(fixture.Compile(new NumberVariable(0), new NumberVariable(0))));
		Assert.IsTrue(fixture.Execute(fixture.Compile(new NumberVariable(65), new NumberVariable(0))));
		fixture.Container.VerifySet(x => x.LiquidMixture = null, Times.Exactly(2));
		Assert.IsFalse(fixture.Execute(fixture.Compile(new NumberVariable(0), new NumberVariable(0.1M))));
		Assert.IsFalse(fixture.Execute(fixture.Compile(new NumberVariable(999), new NumberVariable(0))));
		fixture.Container.VerifySet(x => x.LiquidMixture = null, Times.Exactly(2));
	}

	[TestMethod]
	public void ThreeArgumentInvalidInputsFailWithoutMutatingContainer()
	{
		var fixture = new Fixture();
		Assert.IsFalse(fixture.Execute(fixture.Compile(new NumberVariable(999), new NumberVariable(0.1M))));
		Assert.IsFalse(fixture.Execute(fixture.Compile(new NumberVariable(-1), new NumberVariable(0.1M))));
		Assert.IsFalse(fixture.Execute(fixture.Compile(new NumberVariable(65), new NumberVariable(-0.1M))));
		Assert.IsFalse(fixture.Execute(fixture.Compile(new NumberVariable(65), new NumberVariable(1.01M))));
		Assert.IsFalse(fixture.Execute(fixture.Compile(new NumberVariable(65), new NumberVariable(decimal.MaxValue))));
		Assert.IsFalse(fixture.Execute(fixture.Compile(new NumberVariable(65), new NonFiniteNumberVariable(double.NaN))));
		Assert.IsFalse(fixture.Execute(fixture.Compile(new NumberVariable(65), new NonFiniteNumberVariable(double.PositiveInfinity))));
		fixture.Container.VerifySet(x => x.LiquidMixture = It.IsAny<LiquidMixture>(), Times.Never);
	}

	[TestMethod]
	public void TwoArgumentOverloadRetainsFullFillAndInvalidIdEmptyBehavior()
	{
		var fixture = new Fixture();
		Assert.IsTrue(fixture.Execute(fixture.CompileLegacy(new NumberVariable(65))));
		fixture.Container.VerifySet(x => x.LiquidMixture = It.Is<LiquidMixture>(m => m.TotalVolume == 1.0), Times.Once);
		Assert.IsTrue(fixture.Execute(fixture.CompileLegacy(new NumberVariable(999))));
		fixture.Container.Verify(x => x.ReduceLiquidQuantity(1.0, null, "futurescript"), Times.Once);
	}

	private sealed class Fixture
	{
		private readonly Mock<ILiquid> _liquid = new();
		private readonly Mock<IUneditableAll<ILiquid>> _liquids = new();
		private readonly Mock<IFuturemud> _world = new();
		private readonly Mock<IGameItem> _item = new();
		public Mock<ILiquidContainer> Container { get; } = new();

		public Fixture()
		{
			_liquid.SetupGet(x => x.Id).Returns(65);
			_liquid.SetupGet(x => x.Density).Returns(1.0);
			_liquids.Setup(x => x.Get(65)).Returns(_liquid.Object);
			_world.SetupGet(x => x.Liquids).Returns(_liquids.Object);
			Container.SetupGet(x => x.LiquidCapacity).Returns(1.0);
			Container.SetupGet(x => x.Gameworld).Returns(_world.Object);
			_item.Setup(x => x.GetItemType<ILiquidContainer>()).Returns(Container.Object);
		}

		public IFunction CompileLegacy(IProgVariable liquidId) => Compile(
			_world.Object,
			new ConstantFunction(_item.Object, ProgVariableTypes.Item), new ConstantFunction(liquidId, ProgVariableTypes.Number));

		public IFunction Compile(IProgVariable liquidId, IProgVariable volume) => Compile(
			_world.Object,
			new ConstantFunction(_item.Object, ProgVariableTypes.Item), new ConstantFunction(liquidId, ProgVariableTypes.Number),
			new ConstantFunction(volume, ProgVariableTypes.Number));

		public bool Execute(IFunction function)
		{
			Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
			return (bool)function.Result.GetObject;
		}

		private static IFunction Compile(IFuturemud gameworld, params IFunction[] parameters)
		{
			FutureProgTestBootstrap.EnsureInitialised();
			var types = parameters.Select(x => x.ReturnType).ToList();
			var compiler = FutureProg.GetFunctionCompilerInformations().Single(x =>
				x.FunctionName.EqualTo("setliquid") && x.Parameters.SequenceEqual(types, FutureProgVariableComparer.Instance));
			return compiler.CompilerFunction(parameters.ToList(), gameworld);
		}
	}

	private sealed class ConstantFunction(IProgVariable result, ProgVariableTypes returnType) : IFunction
	{
		public IProgVariable Result { get; private set; } = result;
		public ProgVariableTypes ReturnType { get; } = returnType;
		public string ErrorMessage => string.Empty;
		public StatementResult ExpectedResult => StatementResult.Normal;
		public StatementResult Execute(IVariableSpace variables) => StatementResult.Normal;
		public bool IsReturnOrContainsReturnOnAllBranches() => false;
	}

	private sealed record NonFiniteNumberVariable(double Value) : IProgVariable
	{
		public ProgVariableTypes Type => ProgVariableTypes.Number;
		public object GetObject => Value;
		public IProgVariable GetProperty(string property) => throw new System.NotSupportedException();
	}
}
