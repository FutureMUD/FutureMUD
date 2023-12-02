using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.FutureProg.Functions.Characters
{
	internal class GiveTraitFunction : BuiltInFunction
	{
		public IFuturemud Gameworld { get; set; }
		#region Static Initialisation
		public static void RegisterFunctionCompiler()
		{
			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"givetrait",
					new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Trait }, // the parameters the function takes
					(pars, gameworld) => new GiveTraitFunction(pars, gameworld),
					new List<string> { 
						"who",
						"trait"
					}, // parameter names
					new List<string> { 
						"The character to give the trait to",
						"The trait to give the character"
					}, // parameter help text
					"Gives a trait (skill or attribute) to a player. Uses the opening value as if the trait had branched (for skills) or minimum value (for attributes). Returns the value of the trait", // help text for the function,
					"Character",// the category to which this function belongs,
					FutureProgVariableTypes.Number // the return type of the function
				)
			);

			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"givetrait",
					new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Trait, FutureProgVariableTypes.Number }, // the parameters the function takes
					(pars, gameworld) => new GiveTraitFunction(pars, gameworld),
					new List<string> {
						"who",
						"trait",
						"value"
					}, // parameter names
					new List<string> {
						"The character to give the trait to",
						"The trait to give the character",
						"The value of the trait"
					}, // parameter help text
					"Gives a trait (skill or attribute) to a player with the specified value. If they already have the trait, sets the value if it is higher. Returns the value of the trait", // help text for the function,
					"Character",// the category to which this function belongs,
					FutureProgVariableTypes.Number // the return type of the function
				)
			);
		}
		#endregion

		#region Constructors
		protected GiveTraitFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
		{
			Gameworld = gameworld;
		}
		#endregion

		public override FutureProgVariableTypes ReturnType
		{
			get { return FutureProgVariableTypes.Number; }
			protected set { }
		}

		public override StatementResult Execute(IVariableSpace variables)
		{
			if (base.Execute(variables) == StatementResult.Error)
			{
				return StatementResult.Error;
			}

			var target = ParameterFunctions[0].Result as ICharacter;
			if (target == null)
			{
				ErrorMessage = "The target parameter in GiveTrait returned null";
				return StatementResult.Error;
			}

			var trait = (ITraitDefinition)ParameterFunctions[1].Result;
			if (trait == null)
			{
				ErrorMessage = "The trait parameter in GiveTrait returned null";
				return StatementResult.Error;
			}

			var traitAsSkill = trait as ISkillDefinition;

			var openingValue = 0.0;
			if (ParameterFunctions.Count == 3)
			{
				openingValue = (double)(decimal)ParameterFunctions[2].Result.GetObject;
			}
			else if (traitAsSkill is not null)
			{
				openingValue = target.Culture.SkillStartingValueProg.ExecuteDouble(target, trait, 0.0);
			}
			else
			{
				openingValue = 10.0;
			}

			if (target.HasTrait(trait))
			{
				if (target.TraitRawValue(trait) >= openingValue)
				{
					Result = new NumberVariable(target.TraitRawValue(trait));
					return StatementResult.Normal;
				}

				target.SetTraitValue(trait, openingValue);
				Result = new NumberVariable(openingValue);
				return StatementResult.Normal;
			}

			target.AddTrait(trait, openingValue);
			Result = new NumberVariable(openingValue);
			return StatementResult.Normal;
		}
	}
}
