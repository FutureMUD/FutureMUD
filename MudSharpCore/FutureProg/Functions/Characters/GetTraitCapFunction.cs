using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class GetTraitCapFunction : BuiltInFunction
{
	protected GetTraitCapFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Number;
		protected set { }
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"gettraitcap",
			new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Trait },
			(pars, gameworld) => new GetTraitCapFunction(pars, gameworld),
			new List<string> { "who", "trait" },
			new List<string>
				{ "The person whose traits you would like to interrogate", "The trait you want to know the cap value of" },
			"This function returns the trait cap of the selected trait on the specified person",
			"Character",
			FutureProgVariableTypes.Number
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
			ErrorMessage = "The target parameter in GetTraitCap returned null";
			return StatementResult.Error;
		}

		var trait = (ITraitDefinition)ParameterFunctions[1].Result;
		if (trait == null)
		{
			ErrorMessage = "The trait parameter in GetTrait returned null";
			return StatementResult.Error;
		}

		if (target.Type == FutureProgVariableTypes.Character)
		{
			var character = (ICharacter)target;
			Result = new NumberVariable(character.TraitMaxValue(trait));
		}
		else
		{
			var chargen = (ICharacterTemplate)target;
			if (trait.TraitType == TraitType.Attribute)
			{
				var attr = chargen.SelectedAttributes.FirstOrDefault(x => x.Definition == trait);
				Result = attr == null ? new NumberVariable(0.0M) : new NumberVariable(attr.Definition.MaxValue);
			}
			else
			{
				var skill = chargen.SkillValues.FirstOrDefault(x => x.Item1 == trait).Item1 as ISkillDefinition;
				Result = skill == null ? new NumberVariable(0.0M) : new NumberVariable(skill.Cap.Evaluate(chargen));
			}
		}

		return StatementResult.Normal;
	}
}
