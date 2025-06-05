using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Manipulation;

internal class RemoveFunction : BuiltInFunction
{
	internal RemoveFunction(IList<IFunction> parameters, bool silent)
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

		var wearer = (ICharacter)ParameterFunctions[0].Result;
		if (wearer == null)
		{
			ErrorMessage = "Wearer Character was null in Remove function.";
			return StatementResult.Error;
		}

		var target = (IGameItem)ParameterFunctions[1].Result;
		if (target == null)
		{
			ErrorMessage = "Target GameItem was null in Remove function.";
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

		if (wearer.Body.CanRemoveItem(target))
		{
			wearer.Body.RemoveItem(target, emote, Silent);
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
                        "remove",
                        new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text },
                        (pars, gameworld) => new RemoveFunction(pars, false),
                        new[] { "who", "item", "emote" },
                        new[]
                        {
                                "The character removing the item",
                                "The item to be removed",
                                "An optional emote to accompany the action"
                        },
                        "Has a character remove a worn item. Returns true if successful.",
                        "Manipulation",
                        ProgVariableTypes.Boolean
                ));

                FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
                        "silentremove",
                        new[] { ProgVariableTypes.Character, ProgVariableTypes.Item },
                        (pars, gameworld) => new RemoveFunction(pars, true),
                        new[] { "who", "item" },
                        new[]
                        {
                                "The character removing the item",
                                "The item to be removed"
                        },
                        "Has a character remove a worn item with no associated emote.",
                        "Manipulation",
                        ProgVariableTypes.Boolean
                ));
        }
}