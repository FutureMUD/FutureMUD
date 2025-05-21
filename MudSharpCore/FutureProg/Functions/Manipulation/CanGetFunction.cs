using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.Manipulation;

internal class CanGetFunction : BuiltInFunction
{
	internal CanGetFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var getter = (ICharacter)ParameterFunctions[0].Result?.GetObject;
		if (getter == null)
		{
			ErrorMessage = "Getter Character was null in CanGet function.";
			return StatementResult.Error;
		}

		var target = (IGameItem)ParameterFunctions[1].Result?.GetObject;
		if (target == null)
		{
			ErrorMessage = "Target GameItem was null in CanGet function.";
			return StatementResult.Error;
		}

		var quantity = ParameterFunctions.Count == 3
			? ((int?)(decimal?)ParameterFunctions[2].Result.GetObject ?? 0)
			: 0;

		Result = new BooleanVariable(getter.Body.CanGet(target, quantity));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"canget",
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Item },
			(pars, gameworld) => new CanGetFunction(pars),
			[
				"who",
				"thing"
			],
			[
				"The character doing the getting",
				"The thing being gotten"
			],
			"This function tells you if a player could pick up an item into hands/inventory. Returns true if so. Respects all normal inventory rules.",
			"Manipulation",
			ProgVariableTypes.Boolean
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"canget",
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Number },
			(pars, gameworld) => new CanGetFunction(pars),
			[
				"who",
				"thing",
				"quantity"
			],
			[
				"The character doing the getting",
				"The thing being gotten",
				"The number of things being gotten, or 0 for the full stack"
			],
			"This function tells you if a player could pick up a specified quantity of item into hands/inventory. Returns true if so. Respects all normal inventory rules.",
			"Manipulation",
			ProgVariableTypes.Boolean
		));
	}
}

internal class CanGetContainerFunction : BuiltInFunction
{
	internal CanGetContainerFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var getter = (ICharacter)ParameterFunctions[0].Result?.GetObject;
		if (getter == null)
		{
			ErrorMessage = "Getter Character was null in GetContainer function.";
			return StatementResult.Error;
		}

		var target = (IGameItem)ParameterFunctions[1].Result?.GetObject;
		if (target == null)
		{
			ErrorMessage = "Target GameItem was null in CanGetContainer function.";
			return StatementResult.Error;
		}

		var container = (IGameItem)ParameterFunctions[2].Result?.GetObject;
		if (container == null)
		{
			ErrorMessage = "Container GameItem was null in CanGetContainer function.";
			return StatementResult.Error;
		}

		var quantity = ParameterFunctions.Count == 4
			? ((int?)(decimal?)ParameterFunctions[3].Result.GetObject ?? 0)
			: 0;
		Result = new BooleanVariable(getter.Body.CanGet(target, container, quantity));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"canget",
			new[]
			{
				ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item
			},
			(pars, gameworld) => new CanGetContainerFunction(pars),
			[
				"who",
				"thing",
				"container"
			],
			[
				"The character doing the getting",
				"The thing being gotten",
				"The container to get it from"
			],
			"This function tells you if a player could pick up an item into hands/inventory from a container. Returns true if so. Respects all normal inventory rules.",
			"Manipulation",
			ProgVariableTypes.Boolean
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"canget",
			new[]
			{
				ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item,
				ProgVariableTypes.Number
			},
			(pars, gameworld) => new CanGetContainerFunction(pars),
			[
				"who",
				"thing",
				"container",
				"quantity"
			],
			[
				"The character doing the getting",
				"The thing being gotten",
				"The container to get it from",
				"The number of things being gotten, or 0 for the full stack"
			],
			"This function tells you if a player could pick up a specified quantity of an item into hands/inventory from a container. Returns true if so. Respects all normal inventory rules.",
			"Manipulation",
			ProgVariableTypes.Boolean
		));
	}
}