using MudSharp.Body.Traits.Subtypes;
using MudSharp.Body.Traits;
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
	internal class ImproveTraitFunction : BuiltInFunction
	{
		public IFuturemud Gameworld { get; set; }
		#region Static Initialisation
		public static void RegisterFunctionCompiler()
		{
			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"improvetrait",
					new[] { ProgVariableTypes.Character, ProgVariableTypes.Trait }, // the parameters the function takes
					(pars, gameworld) => new ImproveTraitFunction(pars, gameworld),
					new List<string> {
						"who",
						"trait"
					}, // parameter names
					new List<string> {
						"The character to improve the trait of",
						"The trait to improve"
					}, // parameter help text
					"Gives a trait improvement tick to a trait. This is as if the player had rolled an improvement naturally. Returns the new value.", // help text for the function,
					"Character",// the category to which this function belongs,
					ProgVariableTypes.Number // the return type of the function
				)
			);
		}
		#endregion

		#region Constructors
		protected ImproveTraitFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
		{
			Gameworld = gameworld;
		}
		#endregion

		public override ProgVariableTypes ReturnType
		{
			get { return ProgVariableTypes.Number; }
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

			if (!target.HasTrait(trait))
			{
				Result = new NumberVariable(0.0);
				return StatementResult.Normal;
			}

			var chTrait = target.GetTrait(trait);
			chTrait.TraitUsed(target, RPG.Checks.Outcome.MajorPass, RPG.Checks.Difficulty.Impossible, TraitUseType.Practical, null);
			Result = new NumberVariable(chTrait.RawValue);
			return StatementResult.Normal;
		}
	}
}
