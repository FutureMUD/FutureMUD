using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Manipulation;

internal class WieldFunction : BuiltInFunction
{
	internal WieldFunction(IList<IFunction> parameters, bool silent)
		: base(parameters)
	{
		Silent = silent;
	}

	public bool Silent { get; set; }

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

		var wielder = (ICharacter)ParameterFunctions[0].Result;
		if (wielder == null)
		{
			ErrorMessage = "Wielder Character was null in Wield function.";
			return StatementResult.Error;
		}

		var target = (IGameItem)ParameterFunctions[1].Result;
		if (target == null)
		{
			ErrorMessage = "Target GameItem was null in Wield function.";
			return StatementResult.Error;
		}

		PlayerEmote emote = null;
		if (!Silent && !string.IsNullOrEmpty(ParameterFunctions[3].Result.GetObject?.ToString()))
		{
			emote = new PlayerEmote(ParameterFunctions[3].Result.GetObject.ToString(), wielder);
			if (!emote.Valid)
			{
				emote = null;
			}
		}

		if (wielder.Body.CanWield(target))
		{
			wielder.Body.Wield(target, emote, Silent);
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
			"wield",
			new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item, FutureProgVariableTypes.Text },
			(pars, gameworld) => new WieldFunction(pars, false)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"silentwield",
			new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item },
			(pars, gameworld) => new WieldFunction(pars, true)
		));
	}
}