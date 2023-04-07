using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using MudSharp.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.ArtificialIntelligence;

internal class WildAnimalHerdRoleFunction : BuiltInFunction
{
	protected WildAnimalHerdRoleFunction(IList<IFunction> parameters) : base(parameters)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Text;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = (ICharacter)ParameterFunctions[0].Result;
		if (character == null)
		{
			ErrorMessage = "Character was null in WildAnimalHerdRole function.";
			return StatementResult.Error;
		}

		Result = new TextVariable(
			character.EffectsOfType<WildAnimalHerdEffect>().FirstOrDefault()?.Role.DescribeEnum() ?? "None");
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"wildanimalherdrole",
			new[] { FutureProgVariableTypes.Character },
			(pars, gameworld) => new WildAnimalHerdRoleFunction(pars)
		));
	}
}