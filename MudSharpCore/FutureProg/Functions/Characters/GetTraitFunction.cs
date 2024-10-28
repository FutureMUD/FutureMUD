using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class GetTraitFunction : BuiltInFunction
{
	public GetTraitFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"gettrait",
			new[] { ProgVariableTypes.Toon, ProgVariableTypes.Trait },
			(pars, gameworld) => new GetTraitFunction(pars, gameworld),
			new List<string> { "who", "trait" },
			new List<string>
				{ "The person whose traits you would like to interrogate", "The trait you want to know the value of" },
			"This function returns the current trait value of the selected trait on the specified person",
			"Character",
			ProgVariableTypes.Number
		));
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var target = ParameterFunctions[0].Result;
		if (target == null)
		{
			ErrorMessage = "The target parameter in GetTrait returned null";
			return StatementResult.Error;
		}

		var trait = (ITraitDefinition)ParameterFunctions[1].Result;
		if (trait == null)
		{
			ErrorMessage = "The trait parameter in GetTrait returned null";
			return StatementResult.Error;
		}

		if (target.Type == ProgVariableTypes.Character)
		{
			var character = (ICharacter)target;
			Result = new NumberVariable(character.TraitValue(trait));
		}
		else
		{
			var chargen = (ICharacterTemplate)target;
			if (trait.TraitType == TraitType.Attribute)
			{
				var attr = chargen.SelectedAttributes.FirstOrDefault(x => x.Definition == trait);
				Result = attr == null ? new NumberVariable(0.0M) : new NumberVariable(attr.Value);
			}
			else
			{
				var skill = chargen.SkillValues.FirstOrDefault(x => x.Item1 == trait);
				Result = skill.Item1 == null ? new NumberVariable(0.0M) : new NumberVariable(skill.Item2);
			}
		}

		return StatementResult.Normal;
	}
}