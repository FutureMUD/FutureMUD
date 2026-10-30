#nullable enable

using MudSharp.Combat;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal sealed class CeaseCombatFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var subject = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var combat = subject?.Combat;
		var native = combat as ICombatSelectiveCessation;
		var admission = subject is not null ? native?.PrepareCessation(subject, subject.CombatTarget, combat!) : null;
		Result = new BooleanVariable(admission is not null &&
			native!.CeaseCombatFor(admission).HasFlag(CombatCessationChanges.SubjectRemoved));
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new("ceasecombat",
		[ProgVariableTypes.Character], (parameters, _) => new CeaseCombatFunction(parameters), ["subject"],
		["The character leaving their current fight."],
		"Uses native selective combat cessation to remove this participant and clear incoming targeting. Other participants may retarget normally. Does not end an entire fight; refuses unsupported combat implementations.", "Combat", ProgVariableTypes.Boolean));
}
