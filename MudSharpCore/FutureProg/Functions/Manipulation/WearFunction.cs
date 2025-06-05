using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
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
			var holdable = target.GetItemType<IHoldable>();
			if (holdable?.HeldBy != null && holdable.HeldBy != wearer.Body)
			{
				holdable.HeldBy.Take(target);
			}
			else
			{
				var containedInContainer = target.ContainedIn?.GetItemType<IContainer>();
				containedInContainer?.Take(null, target, 0);
			}

			target.Location?.Extract(target);
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
                        new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text },
                        (pars, gameworld) => new WearFunction(pars, false),
                        new[] { "who", "item", "emote" },
                        new[]
                        {
                                "The character wearing the item",
                                "The item to be worn",
                                "An optional emote to accompany the action"
                        },
                        "Has a character wear an item. Returns true if successful.",
                        "Manipulation",
                        ProgVariableTypes.Boolean
                ));

                FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
                        "silentwear",
                        new[] { ProgVariableTypes.Character, ProgVariableTypes.Item },
                        (pars, gameworld) => new WearFunction(pars, true),
                        new[] { "who", "item" },
                        new[]
                        {
                                "The character wearing the item",
                                "The item to be worn"
                        },
                        "Has a character wear an item with no associated emote.",
                        "Manipulation",
                        ProgVariableTypes.Boolean
                ));
        }
}