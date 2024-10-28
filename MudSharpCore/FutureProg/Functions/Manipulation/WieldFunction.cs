using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
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

		if (wielder.Body.CanWield(target))
		{
			var holdable = target.GetItemType<IHoldable>();
			if (holdable?.HeldBy != null && holdable.HeldBy != wielder.Body)
			{
				holdable.HeldBy.Take(target);
			}
			else
			{
				var containedInContainer = target.ContainedIn?.GetItemType<IContainer>();
				containedInContainer?.Take(null, target, 0);
			}

			target.Location?.Extract(target);
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
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text },
			(pars, gameworld) => new WieldFunction(pars, false)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"silentwield",
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Item },
			(pars, gameworld) => new WieldFunction(pars, true)
		));
	}
}