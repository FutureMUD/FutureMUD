using System.Collections.Generic;
using System.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class IsAdminFunction : BuiltInFunction
{
	protected IFunction InnerFunction;

	public IsAdminFunction(IList<IFunction> parameters)
		: base(parameters)
	{
		InnerFunction = parameters.First();
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (InnerFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = InnerFunction.ErrorMessage;
			return StatementResult.Error;
		}

		if (InnerFunction.Result.GetObject is not IHaveAccount character)
		{
			ErrorMessage = "The Inner Function did not return an IHaveAccount in IsAdmin.";
			return StatementResult.Error;
		}

		if (character is ICharacter)
		{
			Result = new BooleanVariable((character as ICharacter).IsAdministrator());
		}
		else
		{
			Result = new BooleanVariable(character.Account.Authority.Level >= PermissionLevel.JuniorAdmin);
		}

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"isadmin",
				new[] { FutureProgVariableTypes.Toon },
				(pars, gameworld) => new IsAdminFunction(pars),
				new[] { "toon" },
				new[] { "The character or toon whose status as an admin you want to test" },
				"This function tells you if the person you specify, which can be a character, chargen or NPC template, is an admin",
				"Character",
				FutureProgVariableTypes.Boolean
			)
		);
	}
}