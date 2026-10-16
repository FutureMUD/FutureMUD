#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Magic;

internal sealed class CanCommandSpellNpc(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(
		new FunctionCompilerInformation("cancommandspellnpc", [ProgVariableTypes.Character, ProgVariableTypes.Character],
			(pars, _) => new CanCommandSpellNpc(pars), ["creature", "commander"],
			["The exact loaded spell-owned creature", "Its exact loaded creator instance"],
			"True while the creature has one active creator bond and its native lifecycle and magical admission permit control. Lost, retiring or held control is refused.",
			"Magic", ProgVariableTypes.Boolean));

	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var creature = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var commander = ParameterFunctions[1].Result?.GetObject as ICharacter;
		var bonds = creature?.EffectsOfType<SpellNpcControlEffect>().ToArray() ?? [];
		Result = new BooleanVariable(creature is not null && commander is not null &&
			ReferenceEquals(creature.Gameworld, commander.Gameworld) && bonds.Length == 1 && bonds[0].CanAcceptOrder(commander));
		return StatementResult.Normal;
	}
}
