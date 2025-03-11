using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Clan;

internal class OutranksOrPeerFunction : BuiltInFunction
{
	public OutranksOrPeerFunction(IList<IFunction> parameters)
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
		var clanFunction = ParameterFunctions.ElementAt(2);

		if (characterFunction1.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "First Character Function in OutranksOrPeerFunction Function returned an error: " +
			               characterFunction1.ErrorMessage;
			return StatementResult.Error;
		}

		if (characterFunction2.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "Second Character Function in OutranksOrPeerFunction Function returned an error: " +
			               characterFunction2.ErrorMessage;
			return StatementResult.Error;
		}

		if (clanFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = "Clan Function in OutranksOrPeerFunction Function returned an error: " + clanFunction.ErrorMessage;
			return StatementResult.Error;
		}

		var character1 = (ICharacter)characterFunction1.Result.GetObject;
		if (character1 == null)
		{
			ErrorMessage = "First Character in OutranksOrPeerFunction Function was null.";
			return StatementResult.Error;
		}

		var character2 = (ICharacter)characterFunction2.Result.GetObject;
		if (character2 == null)
		{
			ErrorMessage = "Second Character in OutranksOrPeerFunction Function was null.";
			return StatementResult.Error;
		}

		var clan = (IClan)clanFunction.Result.GetObject;
		if (clan == null)
		{
			ErrorMessage = "Clan in OutranksOrPeerFunction Function was null.";
			return StatementResult.Error;
		}

		var charMembership1 = character1.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
		var charMembership2 = character2.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
		if (charMembership1 == null || charMembership2 == null)
		{
			Result = new BooleanVariable(false);
		}
		else
		{
			Result = new BooleanVariable(charMembership1.Rank.RankNumber >= charMembership2.Rank.RankNumber);
		}

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"outranksorpeer",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Character,
					ProgVariableTypes.Clan
				},
				(pars, gameworld) => new OutranksOrPeerFunction(pars)
			)
		);
	}
}