#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Effects;

/// <summary>One persistent, prog-owned description addendum; a pure applicability prog controls visibility.</summary>
internal sealed class AddDescriptionAdditionFunction(IList<IFunction> parameters, IFuturemud gameworld)
	: BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(
		new FunctionCompilerInformation("adddescriptionaddition",
			[ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Number],
			(pars, world) => new AddDescriptionAdditionFunction(pars, world), ["owner", "text", "applicabilityProgId"],
			["The perceivable receiving an addendum", "The additional description text",
				"A non-static boolean prog accepting this owner type and a perceiver"],
			"Adds a persistent non-player description addendum controlled by an exact applicability prog. Repeating identical text and prog is idempotent; a different text under that prog is refused without replacing existing authoring.",
			"Effects", ProgVariableTypes.Boolean));

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		if (ParameterFunctions[0].Result?.GetObject is not IPerceivable owner ||
			ParameterFunctions[1].Result?.GetObject is not string text || string.IsNullOrWhiteSpace(text) || text.Length > 4000 ||
			ParameterFunctions[2].Result?.GetObject is not decimal id || id <= 0 || id > long.MaxValue || id != decimal.Truncate(id))
		{
			ErrorMessage = "A perceivable, bounded nonempty description and positive integral applicability prog ID are required.";
			return StatementResult.Error;
		}
		var prog = gameworld.FutureProgs.Get((long)id);
		if (prog is null || prog.AcceptsAnyParameters || prog.StaticType != FutureProgStaticType.NotStatic ||
			prog.ReturnType != ProgVariableTypes.Boolean || !string.IsNullOrEmpty(prog.CompileError) ||
			!prog.MatchesParameters([owner.Type, ProgVariableTypes.Perceiver]))
		{
			ErrorMessage = "The description applicability prog has no compatible exact live signature.";
			return StatementResult.Error;
		}
		var existing = owner.Effects.OfType<DescriptionAddition>()
			.Where(x => !x.PlayerSet && x.ApplicabilityProg?.Id == prog.Id).ToArray();
		if (existing.Length > 1 || existing.Any(x => x.AdditionalText != text))
		{
			ErrorMessage = "An existing addendum with this applicability prog differs; resolve the retained authoring explicitly.";
			return StatementResult.Error;
		}
		if (existing.Length == 0) owner.AddEffect(new DescriptionAddition(owner, text, false, null!, prog));
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}
