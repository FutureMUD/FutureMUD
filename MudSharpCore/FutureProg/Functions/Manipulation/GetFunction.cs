﻿using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Manipulation;

internal class GetFunction : BuiltInFunction
{
	internal GetFunction(IList<IFunction> parameters, bool silent)
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

		var getter = (ICharacter)ParameterFunctions[0].Result;
		if (getter == null)
		{
			ErrorMessage = "Getter Character was null in Get function.";
			return StatementResult.Error;
		}

		var target = (IGameItem)ParameterFunctions[1].Result;
		if (target == null)
		{
			ErrorMessage = "Target GameItem was null in Get function.";
			return StatementResult.Error;
		}

		var quantity = 0;
		var emoteText = "";
		if (ParameterFunctions.Count > 2)
		{
			quantity = ParameterFunctions[2].ReturnType.CompatibleWith(ProgVariableTypes.Number)
				? (int)(decimal)ParameterFunctions[2].Result.GetObject
				: 0;

			emoteText = ParameterFunctions[2].ReturnType.CompatibleWith(ProgVariableTypes.Number)
				? (string)ParameterFunctions[3].Result.GetObject
				: (string)ParameterFunctions[2].Result.GetObject;
		}

		PlayerEmote emote = null;
		if (!Silent && !string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, getter);
			if (!emote.Valid)
			{
				emote = null;
			}
		}

		if (getter.Body.CanGet(target, quantity))
		{
			var holdable = target.GetItemType<IHoldable>();
			if (holdable?.HeldBy != null && holdable.HeldBy != getter.Body)
			{
				holdable.HeldBy.Take(target);
			}
			else
			{
				var containedInContainer = target.ContainedIn?.GetItemType<IContainer>();
				containedInContainer?.Take(null, target, 0);
			}

			target.Location?.Extract(target);
			getter.Body.Get(target, quantity, emote, Silent);
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
                        "get",
                        new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text },
                        (pars, gameworld) => new GetFunction(pars, false),
                        new[] { "who", "thing", "emote" },
                        new[]
                        {
                                "The character getting the item",
                                "The item being gotten",
                                "An optional emote to accompany the action"
                        },
                        "Has a character pick up an item. Returns true if successful.",
                        "Manipulation",
                        ProgVariableTypes.Boolean
                ));

                FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
                        "get",
                        new[]
                        {
                                ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Number,
                                ProgVariableTypes.Text
                        },
                        (pars, gameworld) => new GetFunction(pars, false),
                        new[] { "who", "thing", "quantity", "emote" },
                        new[]
                        {
                                "The character getting the item",
                                "The item being gotten",
                                "The number of items to get, or 0 for the whole stack",
                                "An optional emote to accompany the action"
                        },
                        "Has a character pick up an item. Returns true if successful.",
                        "Manipulation",
                        ProgVariableTypes.Boolean
                ));

                FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
                        "silentget",
                        new[] { ProgVariableTypes.Character, ProgVariableTypes.Item },
                        (pars, gameworld) => new GetFunction(pars, true),
                        new[] { "who", "thing" },
                        new[]
                        {
                                "The character getting the item",
                                "The item being gotten"
                        },
                        "Has a character pick up an item with no associated emote.",
                        "Manipulation",
                        ProgVariableTypes.Boolean
                ));

                FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
                        "silentget",
                        new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Number },
                        (pars, gameworld) => new GetFunction(pars, true),
                        new[] { "who", "thing", "quantity" },
                        new[]
                        {
                                "The character getting the item",
                                "The item being gotten",
                                "The number of items to get, or 0 for the whole stack"
                        },
                        "Has a character pick up an item with no associated emote.",
                        "Manipulation",
                        ProgVariableTypes.Boolean
                ));
	}
}

internal class GetContainerFunction : BuiltInFunction
{
	internal GetContainerFunction(IList<IFunction> parameters, bool silent)
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

		var getter = (ICharacter)ParameterFunctions[0].Result?.GetObject;
		if (getter == null)
		{
			ErrorMessage = "Getter Character was null in GetContainer function.";
			return StatementResult.Error;
		}

		var target = (IGameItem)ParameterFunctions[1].Result?.GetObject;
		if (target == null)
		{
			ErrorMessage = "Target GameItem was null in GetContainer function.";
			return StatementResult.Error;
		}

		var container = (IGameItem)ParameterFunctions[2].Result?.GetObject;
		if (container == null)
		{
			ErrorMessage = "Container GameItem was null in GetContainer function.";
			return StatementResult.Error;
		}

		var quantity = ParameterFunctions[3].ReturnType.CompatibleWith(ProgVariableTypes.Number)
			? ((int?)(decimal?)ParameterFunctions[3].Result.GetObject ?? 0)
			: 0;

		var emoteText = ParameterFunctions[3].ReturnType.CompatibleWith(ProgVariableTypes.Number)
			? (string)ParameterFunctions[4].Result.GetObject
			: (string)ParameterFunctions[3].Result.GetObject;

		PlayerEmote emote = null;
		if (!Silent && !string.IsNullOrEmpty(emoteText))
		{
			emote = new PlayerEmote(emoteText, getter);
			if (!emote.Valid)
			{
				emote = null;
			}
		}

		if (getter.Body.CanGet(target, container, quantity))
		{
			var holdable = target.GetItemType<IHoldable>();
			if (holdable?.HeldBy != null && holdable.HeldBy != getter.Body)
			{
				holdable.HeldBy.Take(target);
			}
			else if (target.ContainedIn?.GetItemType<IContainer>() is { } containedInContainer && target.ContainedIn != container)
			{
				containedInContainer.Take(null, target, 0);
			}

			target.Location?.Extract(target);
			getter.Body.Get(target, container, quantity, emote, Silent);
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
                        "get",
                        new[]
                        {
                                ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item,
                                ProgVariableTypes.Text
                        },
                        (pars, gameworld) => new GetContainerFunction(pars, false),
                        new[] { "who", "thing", "container", "emote" },
                        new[]
                        {
                                "The character getting the item",
                                "The item being gotten",
                                "The container the item is being taken from",
                                "An optional emote to accompany the action"
                        },
                        "Has a character get an item from a container. Returns true if successful.",
                        "Manipulation",
                        ProgVariableTypes.Boolean
                ));

                FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
                        "get",
                        new[]
                        {
                                ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item,
                                ProgVariableTypes.Number, ProgVariableTypes.Text
                        },
                        (pars, gameworld) => new GetContainerFunction(pars, false),
                        new[] { "who", "thing", "container", "quantity", "emote" },
                        new[]
                        {
                                "The character getting the item",
                                "The item being gotten",
                                "The container the item is being taken from",
                                "The number of items to get, or 0 for the whole stack",
                                "An optional emote to accompany the action"
                        },
                        "Has a character get an item from a container. Returns true if successful.",
                        "Manipulation",
                        ProgVariableTypes.Boolean
                ));

                FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
                        "silentget",
                        new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item },
                        (pars, gameworld) => new GetContainerFunction(pars, true),
                        new[] { "who", "thing", "container" },
                        new[]
                        {
                                "The character getting the item",
                                "The item being gotten",
                                "The container the item is being taken from"
                        },
                        "Has a character get an item from a container with no associated emote.",
                        "Manipulation",
                        ProgVariableTypes.Boolean
                ));

                FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
                        "silentget",
                        new[]
                        {
                                ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item,
                                ProgVariableTypes.Number
                        },
                        (pars, gameworld) => new GetContainerFunction(pars, true),
                        new[] { "who", "thing", "container", "quantity" },
                        new[]
                        {
                                "The character getting the item",
                                "The item being gotten",
                                "The container the item is being taken from",
                                "The number of items to get, or 0 for the whole stack"
                        },
                        "Has a character get an item from a container with no associated emote.",
                        "Manipulation",
                        ProgVariableTypes.Boolean
                ));
        }
}