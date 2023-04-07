using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Manipulation;

internal class DropFunction : BuiltInFunction
{
	internal DropFunction(IList<IFunction> parameters, int quantity, bool silent)
		: base(parameters)
	{
		Silent = silent;
		Quantity = quantity;
	}

	public bool Silent { get; set; }
	public int Quantity { get; set; }

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

		var dropper = (ICharacter)ParameterFunctions[0].Result;
		if (dropper == null)
		{
			ErrorMessage = "Dropper Character was null in Drop function.";
			return StatementResult.Error;
		}

		var target = (IGameItem)ParameterFunctions[1].Result;
		if (target == null)
		{
			ErrorMessage = "Target GameItem was null in Drop function.";
			return StatementResult.Error;
		}

		PlayerEmote emote = null;
		if (!Silent && !string.IsNullOrEmpty(ParameterFunctions[2].Result.GetObject?.ToString()))
		{
			emote = new PlayerEmote(ParameterFunctions[2].Result.GetObject.ToString(), dropper);
			if (!emote.Valid)
			{
				emote = null;
			}
		}

		if (dropper.Body.CanDrop(target, Quantity))
		{
			dropper.Body.Drop(target, Quantity, false, emote, Silent);
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
			"drop",
			new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item, FutureProgVariableTypes.Text },
			(pars, gameworld) => new DropFunction(pars, 0, false)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"silentdrop",
			new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item },
			(pars, gameworld) => new DropFunction(pars, 0, true)
		));
	}
}