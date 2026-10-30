using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.Magic;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.FutureProg.Functions.Health;

/// <summary>Runs a builder-configured poison save; physical dosing remains a separate explicit action.</summary>
internal sealed class ResistPoisonFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var difficulty = Math.Clamp((decimal?)ParameterFunctions[1].Result?.GetObject ?? 5m, 0m, 10m);
		var result = TryResist(actor, (Difficulty)(int)difficulty);
		if (result is null)
		{
			ErrorMessage = "The poison resistance check requires a living physical body that remains current throughout the check.";
			return StatementResult.Error;
		}
		Result = new BooleanVariable(result.Value);
		return StatementResult.Normal;
	}

	internal static bool? TryResist(ICharacter? actor, Difficulty difficulty)
	{
		if (actor?.Body is not { } body || !ReferenceEquals(body.Actor, actor) ||
			actor.State.HasFlag(CharacterState.Dead) || actor.EffectsOfType<ISpellProjectionBoundary>().Any() ||
			!CommandExecutionScope.TryContinue(actor)) return null;
		var result = actor.Gameworld.GetCheck(CheckType.PoisonResistanceCheck).Check(actor, difficulty);
		if (!ReferenceEquals(actor.Body, body) || actor.State.HasFlag(CharacterState.Dead) ||
			!CommandExecutionScope.TryContinue(actor)) return null;
		return result.IsPass();
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"resistpoison", [ProgVariableTypes.Character, ProgVariableTypes.Number], (parameters, _) => new ResistPoisonFunction(parameters),
		["character", "difficulty"], ["The living physical recipient.", "Native difficulty, 0 Automatic through 10 Impossible; 5 is Normal."],
		"Runs the native Poison Resistance check and returns true on a pass. Does not administer or remove any dose. Invalid or changed bodies stop the calling prog.",
		"Health", ProgVariableTypes.Boolean));
}
