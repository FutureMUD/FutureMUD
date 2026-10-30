#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.Health;

namespace MudSharp.FutureProg.Functions.Characters;

/// <summary>Pure current-body capacity access for configurable proportional bodily prices.</summary>
internal sealed class HealthCapacityFunction(IList<IFunction> parameters, bool remaining) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Number; protected set { } }

	public static void RegisterFunctionCompiler()
	{
		foreach (var (name, remaining) in new[] { ("healthcapacity", false), ("healthremaining", true) })
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(name,
				[ProgVariableTypes.Character, ProgVariableTypes.Text],
				(pars, _) => new HealthCapacityFunction(pars, remaining), ["character", "channel"],
				["The character whose current body is inspected", "health, stun, pain or stamina"],
				remaining
					? "Returns remaining native health/stun/pain capacity or current stamina without mutation. Wound channels use the current body's own maxima and wound totals."
					: "Returns the current body's native maximum for one channel without changing condition. Invalid, missing or nonpositive capacities fail rather than supply a free price.",
				"Character", ProgVariableTypes.Number));
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		if (ParameterFunctions[0].Result?.GetObject is not ICharacter actor || actor.Body is null || actor.HealthStrategy is null)
		{
			ErrorMessage = "A live embodied character with a health strategy is required.";
			return StatementResult.Error;
		}
		var channel = (ParameterFunctions[1].Result?.GetObject as string)?.Trim().ToLowerInvariant();
		double value;
		switch (channel)
		{
			case "health": value = actor.HealthStrategy.MaxHP(actor); break;
			case "stun": value = actor.HealthStrategy.MaxStun(actor); break;
			case "pain": value = actor.HealthStrategy.MaxPain(actor); break;
			case "stamina": value = actor.Body.MaximumStamina; break;
			default:
				ErrorMessage = "Health capacity channel must be health, stun, pain or stamina.";
				return StatementResult.Error;
		}
		if (!double.IsFinite(value) || value <= 0.0 || value > (double)decimal.MaxValue)
		{
			ErrorMessage = "The current body's capacity is not a finite positive FutureProg number.";
			return StatementResult.Error;
		}
		if (remaining)
		{
			value = channel switch
			{
				"health" => actor.HealthStrategy.CurrentHitPoints(actor),
				"stun" => value - actor.Wounds.Sum(x => x.CurrentStun),
				"pain" => value - actor.Wounds.Sum(x => x.CurrentPain),
				_ => actor.Body.CurrentStamina
			};
			if (!double.IsFinite(value) || value > (double)decimal.MaxValue)
			{
				ErrorMessage = "The current body's remaining condition is invalid.";
				return StatementResult.Error;
			}
			value = Math.Max(0.0, value);
		}
		try { Result = new NumberVariable(value); }
		catch (OverflowException)
		{
			ErrorMessage = "The current body's capacity is outside the FutureProg number range.";
			return StatementResult.Error;
		}
		return StatementResult.Normal;
	}
}
