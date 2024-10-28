using MudSharp.Character;
using MudSharp.Effects.Concrete;
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

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ClearSDesc : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ClearSDesc".ToLowerInvariant(),
				new[] { ProgVariableTypes.Perceivable }, // the parameters the function takes
				(pars, gameworld) => new ClearSDesc(pars, gameworld),
				new List<string> {
					"perceivable",
				}, // parameter names
				new List<string> {
					"The perceivable who's description is being altered",
				}, // parameter help text
				"Clears all custom SDesc overrides set by progs from this perceivable", // help text for the function,
				"Perception", // the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ClearSDesc".ToLowerInvariant(),
				new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Text }, // the parameters the function takes
				(pars, gameworld) => new ClearSDesc(pars, gameworld),
				new List<string> {
					"perceivable",
					"tag"
				}, // parameter names
				new List<string> {
					"The perceivable who's description is being altered",
					"A short bit of text identifying this specific effect set by the original OverrideSDesc call",
				}, // parameter help text
				"Clears a custom SDesc override with the specified tag set by progs from this perceivable", // help text for the function,
				"Perception", // the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected ClearSDesc(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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
		var perceivable = ParameterFunctions[0].Result?.GetObject as IPerceivable;
		if (perceivable is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var tag = ParameterFunctions[1].Result?.GetObject as string;
		if (string.IsNullOrEmpty(tag))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		perceivable.RemoveAllEffects<OverrideSDescFromProg>(x => x.Tag.EqualTo(tag), true);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}