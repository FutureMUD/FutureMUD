using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FutureProgHookMutationPersistenceTests
{
	[TestMethod]
	public void AddHook_SuccessfulPersistentOwners_MarkCanonicalHookStateDirty()
	{
		AssertSuccessfulMutation(new Mock<ICharacter>(), "addhook", useName: false);
		AssertSuccessfulMutation(new Mock<IGameItem>(), "addhook", useName: true);
		AssertSuccessfulMutation(new Mock<ICell>(), "addhook", useName: false);
	}

	[TestMethod]
	public void RemoveHook_SuccessfulPersistentOwners_MarkCanonicalHookStateDirty()
	{
		AssertSuccessfulMutation(new Mock<ICharacter>(), "removehook", useName: true);
		AssertSuccessfulMutation(new Mock<IGameItem>(), "removehook", useName: false);
		AssertSuccessfulMutation(new Mock<ICell>(), "removehook", useName: true);
	}

	[TestMethod]
	public void AddHook_IdempotentFailure_DoesNotMarkHookStateDirty()
	{
		AssertFailedMutation(new Mock<ICharacter>(), "addhook", useName: false);
	}

	[TestMethod]
	public void RemoveHook_IdempotentFailure_DoesNotMarkHookStateDirty()
	{
		AssertFailedMutation(new Mock<IGameItem>(), "removehook", useName: true);
	}

	[TestMethod]
	public void AddAndRemoveHook_TemporaryPerceivable_RemainNonPersistentFailures()
	{
		var temporary = new Mock<TemporaryPerceivable>();

		var add = Compile("addhook", temporary.Object, useName: false, out _);
		var remove = Compile("removehook", temporary.Object, useName: true, out _);

		Assert.AreEqual(StatementResult.Normal, add.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(false, add.Result.GetObject);
		Assert.IsFalse(temporary.Object.HooksChanged);
		Assert.AreEqual(StatementResult.Normal, remove.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(false, remove.Result.GetObject);
		Assert.IsFalse(temporary.Object.HooksChanged);
	}

	private static void AssertSuccessfulMutation<T>(Mock<T> target, string functionName, bool useName)
		where T : class, IPerceivable
	{
		var function = Compile(functionName, target.Object, useName, out var hook);
		if (functionName.EqualTo("addhook"))
		{
			target.Setup(x => x.InstallHook(hook.Object)).Returns(true);
		}
		else
		{
			target.Setup(x => x.RemoveHook(hook.Object)).Returns(true);
		}

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(true, function.Result.GetObject);
		target.VerifySet(x => x.HooksChanged = true, Times.Once);
	}

	private static void AssertFailedMutation<T>(Mock<T> target, string functionName, bool useName)
		where T : class, IPerceivable
	{
		if (functionName.EqualTo("addhook"))
		{
			target.Setup(x => x.InstallHook(It.IsAny<IHook>())).Returns(false);
		}
		else
		{
			target.Setup(x => x.RemoveHook(It.IsAny<IHook>())).Returns(false);
		}

		var function = Compile(functionName, target.Object, useName, out _);

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(false, function.Result.GetObject);
		target.VerifySet(x => x.HooksChanged = It.IsAny<bool>(), Times.Never);
	}

	private static IFunction Compile(
		string functionName,
		IPerceivable target,
		bool useName,
		out Mock<IHook> hook)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		hook = new Mock<IHook>();
		hook.SetupGet(x => x.Id).Returns(42);
		hook.SetupGet(x => x.Name).Returns("test hook");
		var hooks = new Mock<IUneditableAll<IHook>>();
		hooks.Setup(x => x.Get(42)).Returns(hook.Object);
		hooks.Setup(x => x.GetByName("test hook")).Returns(hook.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Hooks).Returns(hooks.Object);
		var hookParameter = useName
			? new ConstantFunction(new TextVariable("test hook"))
			: new ConstantFunction(new NumberVariable(42));
		var compiler = FutureProg.GetFunctionCompilerInformations()
			.Single(x => x.FunctionName.EqualTo(functionName) &&
			             x.Parameters.SequenceEqual(
				             [ProgVariableTypes.Perceivable, hookParameter.ReturnType],
				             FutureProgVariableComparer.Instance));
		return compiler.CompilerFunction(
			[new ConstantFunction(target, ProgVariableTypes.Perceivable), hookParameter],
			gameworld.Object);
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
