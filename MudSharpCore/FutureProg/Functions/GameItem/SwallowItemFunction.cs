using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.NPC.AI;

#nullable enable
namespace MudSharp.FutureProg.Functions.GameItem;

internal sealed class SwallowItemFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var item = ParameterFunctions[1].Result?.GetObject as IGameItem;
		Result = new BooleanVariable(actor?.Body is { } body && ReferenceEquals(body.Actor, actor) &&
			CharacterState.Able.HasFlag(actor.State) && !actor.Effects.Any(x => x.Applies() && x.IsBlockingEffect("general")) &&
			!actor.EffectsOfType<ISpellProjectionBoundary>().Any() && CommandExecutionScope.TryContinue(actor) &&
			item?.GetItemType<ISwallowable>() is { } swallowable && body.Swallow(swallowable, null, null, null));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"swallowitem", [ProgVariableTypes.Character, ProgVariableTypes.Item], (parameters, _) => new SwallowItemFunction(parameters),
		["character", "item"], ["The physical swallower.", "An accessible native swallowable item."],
		"Performs the normal native swallow action, including access checks, output, events and consumption progs. Returns whether the action succeeds.", "Items", ProgVariableTypes.Boolean));
}
