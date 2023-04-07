using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Manipulation;

internal class WearFunction : BuiltInFunction
{
	internal WearFunction(IList<IFunction> parameters, bool silent)
		: base(parameters)
	{
		Silent = silent;
	}

	public bool Silent { get; set; }
	public string Profile { get; set; }

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

		var wearer = (ICharacter)ParameterFunctions[0].Result;
		if (wearer == null)
		{
			ErrorMessage = "Wearer Character was null in Wear function.";
			return StatementResult.Error;
		}

		var target = (IGameItem)ParameterFunctions[1].Result;
		if (target == null)
		{
			ErrorMessage = "Target GameItem was null in Wear function.";
			return StatementResult.Error;
		}

		PlayerEmote emote = null;
		if (!Silent && !string.IsNullOrEmpty(ParameterFunctions[3].Result.GetObject?.ToString()))
		{
			emote = new PlayerEmote(ParameterFunctions[3].Result.GetObject.ToString(), wearer);
			if (!emote.Valid)
			{
				emote = null;
			}
		}

		if (wearer.Body.CanWear(target))
		{
			wearer.Body.Wear(target, emote, Silent);
			Result = new BooleanVariable(true);
		}
		else
		{
			Result = new BooleanVariable(false);
		}

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"wear",
			new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item, FutureProgVariableTypes.Text },
			(pars, gameworld) => new WearFunction(pars, false)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"silentwear",
			new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item },
			(pars, gameworld) => new WearFunction(pars, true)
		));
	}
}