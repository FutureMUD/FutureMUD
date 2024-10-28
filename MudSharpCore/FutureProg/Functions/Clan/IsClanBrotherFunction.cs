using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Clan;

internal class IsClanBrotherFunction : BuiltInFunction
{
	public IsClanBrotherFunction(IList<IFunction> parameters)
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
		var characterFunction1 = ParameterFunctions.ElementAt(0);
		var characterFunction2 = ParameterFunctions.ElementAt(1);
		var thirdFunction = ParameterFunctions.ElementAtOrDefault(2);

		if (characterFunction1.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "First Character Function in IsClanBrother Function returned an error: " +
			               characterFunction1.ErrorMessage;
			return StatementResult.Error;
		}

		if (characterFunction2.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "Second Character Function in IsClanBrother Function returned an error: " +
			               characterFunction2.ErrorMessage;
			return StatementResult.Error;
		}

		if (thirdFunction != null && thirdFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "Third Function in IsClanBrother Function returned an error: " +
			               thirdFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var excludedClans = new List<IClan>();
		if (thirdFunction != null)
		{
			if (thirdFunction.ReturnType == ProgVariableTypes.Clan)
			{
				excludedClans.Add((IClan)thirdFunction.Result);
			}
			else
			{
				excludedClans.AddRange((IEnumerable<IClan>)thirdFunction.Result.GetObject);
			}
		}

		var character1 = (ICharacter)characterFunction1.Result;
		var character2 = (ICharacter)characterFunction2.Result;

		if (character1 == null || character2 == null)
		{
			ErrorMessage = "One of the Characters being compared in IsClanBrother was null.";
			return StatementResult.Error;
		}

		Result =
			new BooleanVariable(
				character1.ClanMemberships.Any(
					x => !excludedClans.Contains(x.Clan) && character2.ClanMemberships.Any(y => y.Clan == x.Clan)));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"isclanbrother",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Character,
					ProgVariableTypes.Clan
				},
				(pars, gameworld) => new IsClanBrotherFunction(pars)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"isclanbrother",
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Character },
				(pars, gameworld) => new IsClanBrotherFunction(pars)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"isclanbrother",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Character,
					ProgVariableTypes.Collection | ProgVariableTypes.Clan
				},
				(pars, gameworld) => new IsClanBrotherFunction(pars)
			)
		);
	}
}