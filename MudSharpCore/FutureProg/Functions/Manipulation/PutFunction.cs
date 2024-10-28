using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Manipulation;

internal class PutFunction : BuiltInFunction
{
	internal PutFunction(IList<IFunction> parameters, int quantity, bool silent)
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

		var putter = (ICharacter)ParameterFunctions[0].Result?.GetObject;
		if (putter == null)
		{
			ErrorMessage = "Putter Character was null in Put function.";
			return StatementResult.Error;
		}

		var target = (IGameItem)ParameterFunctions[1].Result?.GetObject;
		if (target == null)
		{
			ErrorMessage = "Target GameItem was null in Put function.";
			return StatementResult.Error;
		}

		var container = (IGameItem)ParameterFunctions[2].Result?.GetObject;
		if (container == null)
		{
			ErrorMessage = "Container GameItem was null in Put function.";
			return StatementResult.Error;
		}

		PlayerEmote emote = null;
		if (!Silent && !string.IsNullOrEmpty(ParameterFunctions[3].Result.GetObject?.ToString()))
		{
			emote = new PlayerEmote(ParameterFunctions[3].Result.GetObject.ToString(), putter);
			if (!emote.Valid)
			{
				emote = null;
			}
		}
		var holdable = target.GetItemType<IHoldable>();
		if (holdable?.HeldBy != null && holdable.HeldBy != putter.Body)
		{
			holdable.HeldBy.Take(target);
		}
		else
		{
			var containedInContainer = target.ContainedIn?.GetItemType<IContainer>();
			containedInContainer?.Take(null, target, 0);
		}

		target.Location?.Extract(target);

		if (putter.Body.CanPut(target, container, null, Quantity, false))
		{
			putter.Body.Put(target, container, null, Quantity, emote, Silent);
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
			"put",
			new[]
			{
				ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item,
				ProgVariableTypes.Text
			},
			(pars, gameworld) => new PutFunction(pars, 0, false),
			[ 
				"who",
				"thing",
				"container",
				"emote"
			],
			[
				"The character doing the putting",
				"The thing being put into something else",
				"The container the thing is being put into",
				"The optional player emote to add to the standard echo"
			],
			"This function causes a player to put an item into a container using the ordinary inventory code. It echoes to all, and returns true if successful.",
			"Manipulation",
			ProgVariableTypes.Boolean
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"silentput",
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item },
			(pars, gameworld) => new PutFunction(pars, 0, true),
			[
				"who",
				"thing",
				"container"
			],
			[
				"The character doing the putting",
				"The thing being put into something else",
				"The container the thing is being put into"
			],
			"This function causes a player to put an item into a container using the ordinary inventory code. It does not echo to anyone, and returns true if successful.",
			"Manipulation",
			ProgVariableTypes.Boolean
		));
	}
}

internal class PutContainerFunction : BuiltInFunction
{
	internal PutContainerFunction(IList<IFunction> parameters, bool force)
		: base(parameters)
	{
		ForcePut = force;
	}

	/// <summary>
	///     Ignore whether you can put something in the container, just do it
	/// </summary>
	public bool ForcePut { get; set; }

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

		var target = (IGameItem)ParameterFunctions[0].Result?.GetObject;
		if (target == null)
		{
			ErrorMessage = "Target GameItem was null in Put function.";
			return StatementResult.Error;
		}

		if (ParameterFunctions[1].Result?.GetObject is not IGameItem containerItem)
		{
			ErrorMessage = "Container GameItem was null in Put function.";
			return StatementResult.Error;
		}

		var container = containerItem.GetItemType<IContainer>();
		if (container == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (!ForcePut && !container.CanPut(target))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var holdable = target.GetItemType<IHoldable>();
		if (holdable?.HeldBy != null)
		{
			holdable.HeldBy.Take(target);
		}
		else
		{
			var containedInContainer = target.ContainedIn?.GetItemType<IContainer>();
			containedInContainer?.Take(null, target, 0);
		}

		target.Location?.Extract(target);

		container.Put(null, target);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"put",
			new[] { ProgVariableTypes.Item, ProgVariableTypes.Item },
			(pars, gameworld) => new PutContainerFunction(pars, false),
			[
				"thing",
				"container"
			],
			[
				"The thing being put into something else",
				"The container the thing is being put into"
			],
			"This function puts an item into a container (respecting normal inventory rules), does not echo anything, and returns true if successful.",
			"Manipulation",
			ProgVariableTypes.Boolean
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"forceput",
			new[] { ProgVariableTypes.Item, ProgVariableTypes.Item },
			(pars, gameworld) => new PutContainerFunction(pars, true),
			[
				"thing",
				"container"
			],
			[
				"The thing being put into something else",
				"The container the thing is being put into"
			],
			"This function puts an item into a container (ignoring normal inventory rules), does not echo anything, and returns true if successful.",
			"Manipulation",
			ProgVariableTypes.Boolean
		));
	}
}

internal class PutLocationFunction : BuiltInFunction
{
	internal PutLocationFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

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

		var target = (IGameItem)ParameterFunctions[0].Result?.GetObject;
		if (target == null)
		{
			ErrorMessage = "Target GameItem was null in Put function.";
			return StatementResult.Error;
		}

		var location = (ILocation)ParameterFunctions[1].Result?.GetObject;
		if (location == null)
		{
			ErrorMessage = "Location was null in Put function.";
			return StatementResult.Error;
		}

		var layer = RoomLayer.GroundLevel;
		if (ParameterFunctions.Count == 3)
		{
			if (!(ParameterFunctions[2].Result?.GetObject?.ToString() ?? "").TryParseEnum(out layer))
			{
				ErrorMessage = "An invalid layer was supplied in Put function.";
				return StatementResult.Error;
			}
		}

		var holdable = target.GetItemType<IHoldable>();
		if (holdable?.HeldBy != null)
		{
			holdable.HeldBy.Take(target);
		}
		else
		{
			var containedInContainer = target.ContainedIn?.GetItemType<IContainer>();
			containedInContainer?.Take(null, target, 0);
		}

		target.Location?.Extract(target);
		target.RoomLayer = layer;
		location.Insert(target);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"put",
			new[] { ProgVariableTypes.Item, ProgVariableTypes.Location },
			(pars, gameworld) => new PutLocationFunction(pars),
			new List<string> { "Item", "Location" },
			new List<string>
			{
				"The item you want to drop",
				"The location you want to put the item in"
			},
			"This function puts an item in a room, using the GroundLevel (or closest layer to).",
			"Manipulation",
			ProgVariableTypes.Boolean
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"put",
			new[] { ProgVariableTypes.Item, ProgVariableTypes.Location, ProgVariableTypes.Text },
			(pars, gameworld) => new PutLocationFunction(pars),
			new List<string> { "Item", "Location", "Layer" },
			new List<string>
			{
				"The item you want to drop",
				"The location you want to put the item in",
				"The layer of the location that you want to put the item in"
			},
			"This function puts an item in a room in the specified layer. Possible values for layers are VeryDeepUnderwater, DeepUnderwater, Underwater, GroundLevel, OnRooftops, InTrees, HighInTrees, InAir, HighInAir. See function ROOMLAYERS for how to obtain the list of room layers for a location.",
			"Manipulation",
			ProgVariableTypes.Boolean
		));
	}
}