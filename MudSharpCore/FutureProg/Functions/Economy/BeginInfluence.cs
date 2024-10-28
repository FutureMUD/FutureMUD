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
using MudSharp.Economy.Markets;
using MudSharp.TimeAndDate;

namespace MudSharp.FutureProg.Functions.Economy
{
	internal class BeginInfluence : BuiltInFunction
	{
		public IFuturemud Gameworld { get; set; }
		#region Static Initialisation
		public static void RegisterFunctionCompiler()
		{
			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"BeginInfluence".ToLowerInvariant(),
					new[] { ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.MudDateTime, ProgVariableTypes.MudDateTime }, // the parameters the function takes
					(pars, gameworld) => new BeginInfluence(pars, gameworld),
					new List<string>
					{
						"Market",
						"Template",
						"Start",
						"End"
					}, // parameter names
					new List<string>
					{
						"The ID of the market you want to begin an influence in",
						"The ID of the market influence template you want to use",
						"The MUD DateTime of when the influence should start",
						"The MUD DateTime of when the influence should end"
					}, // parameter help text
					"Creates a market influence based on the supplied template. Returns the ID of the influence or 0 if there is any error.", // help text for the function,

					"Markets", // the category to which this function belongs,

					ProgVariableTypes.Number // the return type of the function
				)
			);

			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"BeginInfluence".ToLowerInvariant(),
					new[] { ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.MudDateTime, ProgVariableTypes.TimeSpan }, // the parameters the function takes
					(pars, gameworld) => new BeginInfluence(pars, gameworld),
					new List<string>
					{
						"Market",
						"Template",
						"Start",
						"Duration"
					}, // parameter names
					new List<string>
					{
						"The ID of the market you want to begin an influence in",
						"The ID of the market influence template you want to use",
						"The MUD DateTime of when the influence should start",
						"A duration for the influence to last"
					}, // parameter help text
					"Creates a market influence based on the supplied template. Returns the ID of the influence or 0 if there is any error.", // help text for the function,

					"Markets", // the category to which this function belongs,

					ProgVariableTypes.Number // the return type of the function
				)
			);

			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"BeginInfluence".ToLowerInvariant(),
					new[] { ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.MudDateTime }, // the parameters the function takes
					(pars, gameworld) => new BeginInfluence(pars, gameworld),
					new List<string>
					{
						"Market",
						"Template",
						"Start"
					}, // parameter names
					new List<string>
					{
						"The ID of the market you want to begin an influence in",
						"The ID of the market influence template you want to use",
						"The MUD DateTime of when the influence should start"
					}, // parameter help text
					"Creates a market influence based on the supplied template. This influence is indefinite until revoked. Returns the ID of the influence or 0 if there is any error.", // help text for the function,

					"Markets", // the category to which this function belongs,

					ProgVariableTypes.Number // the return type of the function
				)
			);

			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"BeginInfluence".ToLowerInvariant(),
					new[] { ProgVariableTypes.Number, ProgVariableTypes.Number }, // the parameters the function takes
					(pars, gameworld) => new BeginInfluence(pars, gameworld),
					new List<string>
					{
						"Market",
						"Template"
					}, // parameter names
					new List<string>
					{
						"The ID of the market you want to begin an influence in",
						"The ID of the market influence template you want to use"
					}, // parameter help text
					"Creates a market influence based on the supplied template. This influence begins immediately and is indefinite until revoked. Returns the ID of the influence or 0 if there is any error.", // help text for the function,

					"Markets", // the category to which this function belongs,

					ProgVariableTypes.Number // the return type of the function
				)
			);

			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"BeginInfluence".ToLowerInvariant(),
					new[] { ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.MudDateTime, ProgVariableTypes.MudDateTime }, // the parameters the function takes
					(pars, gameworld) => new BeginInfluence(pars, gameworld),
					new List<string>
					{
						"Market",
						"Template",
						"Start",
						"End"
					}, // parameter names
					new List<string>
					{
						"The ID of the market you want to begin an influence in",
						"The name of the market influence template you want to use",
						"The MUD DateTime of when the influence should start",
						"The MUD DateTime of when the influence should end"
					}, // parameter help text
					"Creates a market influence based on the supplied template. Returns the ID of the influence or 0 if there is any error.", // help text for the function,

					"Markets", // the category to which this function belongs,

					ProgVariableTypes.Number // the return type of the function
				)
			);

			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"BeginInfluence".ToLowerInvariant(),
					new[] { ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.MudDateTime, ProgVariableTypes.TimeSpan }, // the parameters the function takes
					(pars, gameworld) => new BeginInfluence(pars, gameworld),
					new List<string>
					{
						"Market",
						"Template",
						"Start",
						"Duration"
					}, // parameter names
					new List<string>
					{
						"The ID of the market you want to begin an influence in",
						"The name of the market influence template you want to use",
						"The MUD DateTime of when the influence should start",
						"A duration for the influence to last"
					}, // parameter help text
					"Creates a market influence based on the supplied template. Returns the ID of the influence or 0 if there is any error.", // help text for the function,

					"Markets", // the category to which this function belongs,

					ProgVariableTypes.Number // the return type of the function
				)
			);

			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"BeginInfluence".ToLowerInvariant(),
					new[] { ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.MudDateTime }, // the parameters the function takes
					(pars, gameworld) => new BeginInfluence(pars, gameworld),
					new List<string>
					{
						"Market",
						"Template",
						"Start"
					}, // parameter names
					new List<string>
					{
						"The ID of the market you want to begin an influence in",
						"The name of the market influence template you want to use",
						"The MUD DateTime of when the influence should start"
					}, // parameter help text
					"Creates a market influence based on the supplied template. This influence is indefinite until revoked. Returns the ID of the influence or 0 if there is any error.", // help text for the function,

					"Markets", // the category to which this function belongs,

					ProgVariableTypes.Number // the return type of the function
				)
			);

			FutureProg.RegisterBuiltInFunctionCompiler(
				new FunctionCompilerInformation(
					"BeginInfluence".ToLowerInvariant(),
					new[] { ProgVariableTypes.Number, ProgVariableTypes.Text }, // the parameters the function takes
					(pars, gameworld) => new BeginInfluence(pars, gameworld),
					new List<string>
					{
						"Market",
						"Template"
					}, // parameter names
					new List<string>
					{
						"The ID of the market you want to begin an influence in",
						"The name of the market influence template you want to use"
					}, // parameter help text
					"Creates a market influence based on the supplied template. This influence begins immediately and is indefinite until revoked. Returns the ID of the influence or 0 if there is any error.", // help text for the function,

					"Markets", // the category to which this function belongs,

					ProgVariableTypes.Number // the return type of the function
				)
			);
		}
		#endregion

		#region Constructors
		protected BeginInfluence(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

			var market = Gameworld.Markets.Get((long?)(decimal?)ParameterFunctions[0].Result?.GetObject ?? 0L);
			if (market is null)
			{
				Result = new NumberVariable(0);
				return StatementResult.Normal;
			}

			var template = ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Text)
				? Gameworld.MarketInfluenceTemplates.Get((string)ParameterFunctions[1].Result?.GetObject ?? "").FirstOrDefault()
				: Gameworld.MarketInfluenceTemplates.Get((long?)(decimal?)ParameterFunctions[1].Result?.GetObject ?? 0L);
			if (template is null)
			{
				Result = new NumberVariable(0);
				return StatementResult.Normal;
			}

			var start = market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
			var end = default(MudDateTime);

			if (ParameterFunctions.Count > 2)
			{
				if (ParameterFunctions[2].Result?.GetObject is not MudDateTime mdt)
				{
					Result = new NumberVariable(0);
					return StatementResult.Normal;
				}

				start = mdt;

				if (ParameterFunctions.Count > 3)
				{
					if (ParameterFunctions[3].ReturnType.CompatibleWith(ProgVariableTypes.MudDateTime) && ParameterFunctions[2].Result?.GetObject is MudDateTime mdte)
					{
						end = mdte;
					}
					else if (ParameterFunctions[3].ReturnType.CompatibleWith(ProgVariableTypes.TimeSpan) && ParameterFunctions[2].Result?.GetObject is TimeSpan ts)
					{
						end = start + ts;
					}
				}
			}

			var influence = new MarketInfluence(market, template, template.Name, start, end);
			Gameworld.Add(influence);
			Result = new NumberVariable(influence.Id);
			return StatementResult.Normal;
		}
	}
}
