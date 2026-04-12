using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Law;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.Crime;

internal class GetLegalClassFunction : BuiltInFunction
{
	protected GetLegalClassFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.LegalClass;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result?.GetObject is not ICharacter character)
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		var authority = ParameterFunctions[1].Result?.GetObject as ILegalAuthority;
		if (authority is null)
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		Result = authority.GetLegalClass(character) is { } legalClass
			? legalClass
			: new NullVariable(ReturnType);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"getlegalclass",
				[ProgVariableTypes.Character, ProgVariableTypes.LegalAuthority],
				(pars, gameworld) => new GetLegalClassFunction(pars, gameworld),
				["character", "authority"],
				["The character whose legal class you want to check", "The legal authority to check in"],
				"Returns the legal class a character belongs to in the specified legal authority",
				"Crime",
				ProgVariableTypes.LegalClass
			)
		);
	}
}
