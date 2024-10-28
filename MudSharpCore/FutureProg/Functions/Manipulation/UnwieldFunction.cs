using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Manipulation;

internal class UnwieldFunction : BuiltInFunction
{
	internal UnwieldFunction(IList<IFunction> parameters, bool silent)
		: base(parameters)
	{
		Silent = silent;
	}

	public bool Silent { get; set; }

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

		if (wielder.Body.CanUnwield(target))
		{
			wielder.Body.Unwield(target, emote, Silent);
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
			"unwield",
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text },
			(pars, gameworld) => new UnwieldFunction(pars, false)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"silentunwield",
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Item },
			(pars, gameworld) => new UnwieldFunction(pars, true)
		));
	}
}