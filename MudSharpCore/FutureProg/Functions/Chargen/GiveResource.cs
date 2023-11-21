using MudSharp.Accounts;
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

namespace MudSharp.FutureProg.Functions.Chargen
{
	internal class GiveResource : BuiltInFunction
	{
		public IFuturemud Gameworld { get; set; }
		#region Static Initialisation
		public static void RegisterFunctionCompiler()
		{
			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"GiveResource".ToLowerInvariant(),
					new[] { FutureProgVariableTypes.Toon, FutureProgVariableTypes.Text, FutureProgVariableTypes.Number }, // the parameters the function takes
					(pars, gameworld) => new GiveResource(pars, gameworld),
					new List<string> {
						"character", 
						"resource", 
						"amount"
					},
					new List<string> {
						"The character whose account should get the resources",
						"The resource to give that account",
						"The amount of resource to give"
					},
					"This function gives (or takes if you use a negative number) a specified amount of chargen resource (Karma, RPP, BP, etc) to an account. It returns the new amount of resource that account has after the change.", // help text for the function,

					"Chargen", // the category to which this function belongs,

					FutureProgVariableTypes.Number // the return type of the function
				)
			);
		}
		#endregion

		#region Constructors
		protected GiveResource(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
		{
			Gameworld = gameworld;
		}
		#endregion

		public override FutureProgVariableTypes ReturnType
		{
			get { return FutureProgVariableTypes.Boolean; }
			protected set { }
		}

		public override StatementResult Execute(IVariableSpace variables)
		{
			if (base.Execute(variables) == StatementResult.Error)
			{
				return StatementResult.Error;
			}

			var chargen = (IHaveAccount)ParameterFunctions[0]?.Result;
			if (chargen is null)
			{
				ErrorMessage = $"Chargen parameter was null";
				return StatementResult.Error;
			}

			var text = (string)ParameterFunctions[1].Result?.GetObject;
			var resource =
				Gameworld.ChargenResources.FirstOrDefault(
					x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase)) ??
				Gameworld.ChargenResources.FirstOrDefault(
					x => x.Alias.Equals(text, StringComparison.InvariantCultureIgnoreCase));
			if (resource == null)
			{
				ErrorMessage = $"{text} is not a valid Chargen Resource.";
				return StatementResult.Error;
			}

			var amount = (int)((decimal?)ParameterFunctions[2].Result?.GetObject ?? 0.0M);
			var current = chargen.Account.AccountResources.GetValueOrDefault(resource);
			chargen.Account.AccountResources[resource] = Math.Max(0, current + amount);
			chargen.Account.AccountResourcesLastAwarded[resource] = System.DateTime.UtcNow;
			chargen.Account.Changed = true;
			Result = new NumberVariable(chargen.Account.AccountResources[resource]);
			return StatementResult.Normal;
		}
	}
}
