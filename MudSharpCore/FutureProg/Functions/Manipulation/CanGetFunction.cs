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

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var getter = (ICharacter)ParameterFunctions[0].Result;
		if (getter == null)
		{
			ErrorMessage = "Getter Character was null in CanGet function.";
			return StatementResult.Error;
		}

		var target = (IGameItem)ParameterFunctions[1].Result;
		if (target == null)
		{
			ErrorMessage = "Target GameItem was null in CanGet function.";
			return StatementResult.Error;
		}

		var quantity = ParameterFunctions.Count == 3
			? (int)(decimal)ParameterFunctions[2].Result.GetObject
			: 0;

		Result = new BooleanVariable(getter.Body.CanGet(target, quantity));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"canget",
			new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item },
			(pars, gameworld) => new CanGetFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"canget",
			new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item, FutureProgVariableTypes.Number },
			(pars, gameworld) => new CanGetFunction(pars)
		));
	}
}

internal class CanGetContainerFunction : BuiltInFunction
{
	internal CanGetContainerFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var getter = (ICharacter)ParameterFunctions[0].Result;
		if (getter == null)
		{
			ErrorMessage = "Getter Character was null in GetContainer function.";
			return StatementResult.Error;
		}

		var target = (IGameItem)ParameterFunctions[1].Result;
		if (target == null)
		{
			ErrorMessage = "Target GameItem was null in CanGetContainer function.";
			return StatementResult.Error;
		}

		var container = (IGameItem)ParameterFunctions[2].Result;
		if (container == null)
		{
			ErrorMessage = "Container GameItem was null in CanGetContainer function.";
			return StatementResult.Error;
		}

		var quantity = ParameterFunctions.Count == 3
			? (int)(decimal)ParameterFunctions[3].Result.GetObject
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
				FutureProgVariableTypes.Character, FutureProgVariableTypes.Item, FutureProgVariableTypes.Item,
				FutureProgVariableTypes.Text
			},
			(pars, gameworld) => new CanGetContainerFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"canget",
			new[]
			{
				FutureProgVariableTypes.Character, FutureProgVariableTypes.Item, FutureProgVariableTypes.Item,
				FutureProgVariableTypes.Number, FutureProgVariableTypes.Text
			},
			(pars, gameworld) => new CanGetContainerFunction(pars)
		));
	}
}