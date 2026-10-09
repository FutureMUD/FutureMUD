#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Health;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WoundFunctionTests
{
	[TestMethod]
	public void Execute_BodylessItemCrushing_AppliesDamageAndReturnsTrue()
	{
		AssertBodylessItemDamage(DamageType.Crushing);
	}

	[TestMethod]
	public void Execute_BodylessItemSlashing_AppliesDamageAndReturnsTrue()
	{
		AssertBodylessItemDamage(DamageType.Slashing);
	}

	[TestMethod]
	public void Execute_NonMortalPerceiver_ReturnsFalseWithoutDamage()
	{
		var perceiver = new Mock<IPerceiver>();
		var function = Compile(
			new ConstantFunction(perceiver.Object, ProgVariableTypes.Perceiver),
			new ConstantFunction(new TextVariable(nameof(DamageType.Crushing))),
			new ConstantFunction(new NumberVariable(12.0M)));

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.IsNotNull(function.Result);
		Assert.AreEqual(false, function.Result.GetObject);
	}

	private static void AssertBodylessItemDamage(DamageType damageType)
	{
		AssertBodylessItemDamage(damageType, new NumberVariable(12.0M), 12.0);
	}

	[DataTestMethod]
	[DataRow("1d1 e1")]
	[DataRow("1000000000d6")]
	[DataRow("999999999999999999999999d6")]
	[DataRow("1d6 m999999999999999999999999")]
	public void Execute_UnsafeDiceText_ReturnsFalseWithoutDamage(string expression)
	{
		var item = new Mock<IGameItem>();
		var function = Compile(new ConstantFunction(item.Object, ProgVariableTypes.Item),
			new ConstantFunction(new TextVariable(nameof(DamageType.Crushing))),
			new ConstantFunction(new TextVariable(expression)));
		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(false, function.Result.GetObject);
		item.Verify(x => x.SufferDamage(It.IsAny<IDamage>()), Times.Never);
	}

	[TestMethod]
	public void Execute_ValidDiceText_PreservesDamage()
	{
		AssertBodylessItemDamage(DamageType.Crushing, new TextVariable("1d1+4"), 5.0);
	}

	private static void AssertBodylessItemDamage(DamageType damageType, IProgVariable amount, double expectedDamage)
	{
		IDamage? appliedDamage = null;
		var item = new Mock<IGameItem>();
		item.Setup(x => x.SufferDamage(It.IsAny<IDamage>()))
			.Callback<IDamage>(damage => appliedDamage = damage)
			.Returns(Array.Empty<IWound>());
		var function = Compile(
			new ConstantFunction(item.Object, ProgVariableTypes.Item),
			new ConstantFunction(new TextVariable(damageType.ToString())),
			new ConstantFunction(amount));

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.IsNotNull(function.Result);
		Assert.AreEqual(true, function.Result.GetObject);
		Assert.IsNotNull(appliedDamage);
		Assert.AreEqual(damageType, appliedDamage.DamageType);
		Assert.AreEqual(expectedDamage, appliedDamage.DamageAmount);
		Assert.IsNull(appliedDamage.Bodypart);
		item.Verify(x => x.SufferDamage(It.IsAny<IDamage>()), Times.Once);
	}

	private static IFunction Compile(params IFunction[] parameters)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var types = parameters.Select(x => x.ReturnType).ToList();
		var compiler = FutureProg.GetFunctionCompilerInformations()
			.Single(x => x.FunctionName.EqualTo("wound") &&
			             x.Parameters.SequenceEqual(types, FutureProgVariableComparer.Instance));
		return compiler.CompilerFunction(parameters.ToList(), Mock.Of<IFuturemud>());
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
