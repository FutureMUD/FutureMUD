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

namespace MudSharp.FutureProg.Functions.Economy
{
	internal class EndInfluenceByTemplate : BuiltInFunction
	{
		public IFuturemud Gameworld { get; set; }
		#region Static Initialisation
		public static void RegisterFunctionCompiler()
		{
			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"EndInfluenceByTemplate".ToLowerInvariant(),
					new[] { ProgVariableTypes.Number }, // the parameters the function takes
					(pars, gameworld) => new EndInfluenceByTemplate(pars, gameworld),
					new List<string> { }, // parameter names
					new List<string> { }, // parameter help text
					"", // help text for the function,

					"", // the category to which this function belongs,

					ProgVariableTypes.Boolean // the return type of the function
				)
			);
		}
		#endregion

		#region Constructors
		protected EndInfluenceByTemplate(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
		{
			Gameworld = gameworld;
		}
		#endregion

		public override ProgVariableTypes ReturnType
		{
			get { return ProgVariableTypes.Boolean; }
			protected set { }
		}

		public override StatementResult Execute(IVariableSpace variables)
		{
			if (base.Execute(variables) == StatementResult.Error)
			{
				return StatementResult.Error;
			}

			var market = Gameworld.Markets.Get((long?)(decimal?)ParameterFunctions[0].Result?.GetObject ?? 0L);
			if (market is null)
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			var template = ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Text)
				? Gameworld.MarketInfluenceTemplates.Get((string)ParameterFunctions[1].Result?.GetObject ?? "").FirstOrDefault()
				: Gameworld.MarketInfluenceTemplates.Get((long?)(decimal?)ParameterFunctions[1].Result?.GetObject ?? 0L);
			if (template is null)
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			foreach (var influence in market.MarketInfluences.Where(x => x.MarketInfluenceTemplate == template).ToList())
			{
				influence.EndOrCancel();
			}

			Result = new BooleanVariable(true);
			return StatementResult.Normal;
		}
	}
}
