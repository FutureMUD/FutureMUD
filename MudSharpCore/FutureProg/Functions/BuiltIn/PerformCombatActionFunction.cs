#nullable enable

using MudSharp.Combat;
using MudSharp.FutureProg.Variables;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;

namespace MudSharp.FutureProg.Functions.BuiltIn;

/// <summary>Explicit bonus manoeuvres use normal native admission, response and stamina handling.</summary>
internal sealed class PerformCombatActionFunction(IList<IFunction> parameters, bool auxiliary = false) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public static void RegisterFunctionCompiler()
	{
		foreach (var byAction in new[] { false, true })
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
				"performcombataction", [ProgVariableTypes.Character, ProgVariableTypes.Character, byAction ? ProgVariableTypes.Number : ProgVariableTypes.Text],
				(pars, _) => new PerformCombatActionFunction(pars, byAction), ["actor", "target", byAction ? "auxiliaryId" : "command"],
				["The active combatant", "The exact target", byAction ? "An exact native auxiliary combat action ID" : "An NPC-usable native manual combat command primary verb"],
				"Immediately attempts one additional native action without changing the scheduled turn. Native race, reach, usability, stamina and defences apply. Numeric selection needs no player command. Recursive additional actions are refused.", "Combat", ProgVariableTypes.Boolean));
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new BooleanVariable(false);
		if (ParameterFunctions[0].Result?.GetObject is not ICharacter actor ||
			ParameterFunctions[1].Result?.GetObject is not ICharacter target || actor.Combat is not IAdditionalCombatActionResolver resolver) return StatementResult.Normal;
		var verb = ParameterFunctions[2].Result?.GetObject as string;
		IManualCombatCommand? command = null;
		var performed = resolver.TryResolveAdditionalAction(actor, () =>
		{
			if (auxiliary)
			{
				var id = Convert.ToDecimal(ParameterFunctions[2].Result?.GetObject);
				if (id < 1 || id > long.MaxValue || decimal.Truncate(id) != id || actor.Gameworld.AuxiliaryCombatActions.Get((long)id) is not { } action) return null;
				var selected = ManualCombatCommandResolver.TryResolveAuxiliaryAction(actor, target, action);
				return selected.Success ? selected.Move : null;
			}
			command = actor.Gameworld.ManualCombatCommands.FirstOrDefault(x => x.NpcUsable && x.PrimaryVerb.EqualTo(verb));
			if (command is null || actor.CombinedEffectsOfType<ICommandDelay>().Any(x => x.IsDelayed(command.PrimaryVerb))) return null;
			var resolved = ManualCombatCommandResolver.TryResolve(actor, command, target);
			return resolved.Success ? resolved.Move : null;
		});
		if (performed && command is not null)
		{
			Result = new BooleanVariable(true);
			if (command.CooldownSeconds > 0)
				actor.AddEffect(new CommandDelay(actor, command.CommandWords, command.CooldownMessage), TimeSpan.FromSeconds(command.CooldownSeconds));
		}
		return StatementResult.Normal;
	}
}
