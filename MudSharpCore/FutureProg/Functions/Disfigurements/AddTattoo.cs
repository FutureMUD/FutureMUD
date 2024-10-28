using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Body.Disfigurements;

namespace MudSharp.FutureProg.Functions.Disfigurements;

internal class AddTattoo : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"AddTattoo".ToLowerInvariant(),
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Text,
					ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Number
				},
				(pars, gameworld) => new AddTattoo(pars, gameworld),
				new List<string> { "Character", "TattooID", "Bodypart", "Tattooist", "TattooistSkill", "Completion" },
				new List<string>
				{
					"The character who is getting the tattoo",
					"The ID number of the tattoo template to apply",
					"The name of the bodypart to install the tattoo on",
					"The tattooist who is responsible for the tattoo. Can be null",
					"The skill of the tattooist. If it is null, checks the tattooist's skill",
					"The completion percentage between 0.0 and 1.0"
				},
				"This command attempts to add the specified tattoo to a character. Returns true if it was successful.",
				"Disfigurements",
				ProgVariableTypes.Boolean
			)
		);
	}

	#endregion

	#region Constructors

	protected AddTattoo(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

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

		if (ParameterFunctions[0].Result?.GetObject is not ICharacter character)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var tattooid = Convert.ToInt64(ParameterFunctions[1].Result?.GetObject ?? 0M);
		if (tattooid == 0)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (Gameworld.DisfigurementTemplates.Get(tattooid) is not ITattooTemplate tattoo)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var bodypartName = ParameterFunctions[2].Result?.GetObject?.ToString();
		if (string.IsNullOrEmpty(bodypartName))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var bodypart = character.Body.GetTargetBodypart(bodypartName);
		if (bodypart == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var tattooist = ParameterFunctions[3].Result?.GetObject as ICharacter;
		var tattooistSkill = ParameterFunctions[4].Result?.GetObject == null
			? tattooist?.GetTrait(TattooTemplate.TattooistTrait)?.Value ?? 100.0
			: Convert.ToDouble(ParameterFunctions[4].Result.GetObject);

		var completion = Convert.ToDouble(ParameterFunctions[5].Result?.GetObject ?? -1);
		if (completion < 0.0 || completion > 1.0)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var newTattoo = new Tattoo(tattoo, Gameworld, tattooist, tattooistSkill, bodypart,
			character.Location.DateTime());
		character.Body.AddTattoo(newTattoo);
		newTattoo.CompletionPercentage = completion;
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}