using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Law;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.Crime;

internal class LegalClassOutranksFunction : BuiltInFunction
{
	protected LegalClassOutranksFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		ILegalClass offenderClass = null;
		ILegalClass victimClass = null;

		if (ParameterFunctions.Count == 2)
		{
			offenderClass = ParameterFunctions[0].Result?.GetObject as ILegalClass;
			victimClass = ParameterFunctions[1].Result?.GetObject as ILegalClass;
		}
		else
		{
			var offender = ParameterFunctions[0].Result?.GetObject as ICharacter;
			var victim = ParameterFunctions[1].Result?.GetObject as ICharacter;
			var authority = ParameterFunctions[2].Result?.GetObject as ILegalAuthority;

			if (offender is null || victim is null || authority is null)
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			offenderClass = authority.GetLegalClass(offender);
			victimClass = authority.GetLegalClass(victim);
		}

		Result = new BooleanVariable(
			offenderClass is not null &&
			victimClass is not null &&
			offenderClass.LegalClassPriority > victimClass.LegalClassPriority);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"legalclassoutranks",
				[ProgVariableTypes.LegalClass, ProgVariableTypes.LegalClass],
				(pars, gameworld) => new LegalClassOutranksFunction(pars, gameworld),
				["offenderclass", "victimclass"],
				["The legal class to test as the higher-ranked side", "The legal class to test as the lower-ranked side"],
				"Returns true if the first legal class outranks the second legal class",
				"Crime",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"legalclassoutranks",
				[ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.LegalAuthority],
				(pars, gameworld) => new LegalClassOutranksFunction(pars, gameworld),
				["offender", "victim", "authority"],
				["The offender to evaluate", "The victim to evaluate", "The legal authority to resolve the legal classes in"],
				"Returns true if the first character's legal class outranks the second character's legal class in the specified authority",
				"Crime",
				ProgVariableTypes.Boolean
			)
		);
	}
}
