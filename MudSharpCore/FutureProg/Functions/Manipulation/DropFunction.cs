using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
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

		var dropper = (ICharacter)ParameterFunctions[0].Result?.GetObject;
		if (dropper == null)
		{
			ErrorMessage = "Dropper Character was null in Drop function.";
			return StatementResult.Error;
		}

		var target = (IGameItem)ParameterFunctions[1].Result?.GetObject;
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
			var holdable = target.GetItemType<IHoldable>();
			if (holdable?.HeldBy != null && holdable.HeldBy != dropper.Body)
			{
				holdable.HeldBy.Take(target);
			}
			else
			{
				var containedInContainer = target.ContainedIn?.GetItemType<IContainer>();
				containedInContainer?.Take(null, target, 0);
			}

			target.Location?.Extract(target);
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
                        new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text },
                        (pars, gameworld) => new DropFunction(pars, 0, false),
                        new[] { "Character", "Item", "Emote" },
                        new[]
                        {
                                "The character dropping the item",
                                "The item to be dropped",
                                "An optional emote performed while dropping"
                        },
                        "Drops an item from a character, optionally with an emote.",
                        "Manipulation",
                        ProgVariableTypes.Boolean
                ));

                FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
                        "silentdrop",
                        new[] { ProgVariableTypes.Character, ProgVariableTypes.Item },
                        (pars, gameworld) => new DropFunction(pars, 0, true),
                        new[] { "Character", "Item" },
                        new[]
                        {
                                "The character dropping the item",
                                "The item to be dropped"
                        },
                        "Drops an item from a character with no associated emote.",
                        "Manipulation",
                        ProgVariableTypes.Boolean
                ));
	}
}