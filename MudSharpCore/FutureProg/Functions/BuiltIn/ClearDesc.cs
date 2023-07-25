using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ClearDesc : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ClearDesc".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Perceivable }, // the parameters the function takes
				(pars, gameworld) => new ClearDesc(pars, gameworld),
				new List<string> {
					"perceivable",
				}, // parameter names
				new List<string> {
					"The perceivable who's description is being altered",
				}, // parameter help text
				"Clears all custom full Desc overrides set by progs from this perceivable", // help text for the function,
				"Perception", // the category to which this function belongs,
				FutureProgVariableTypes.Boolean // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ClearDesc".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Perceivable, FutureProgVariableTypes.Text }, // the parameters the function takes
				(pars, gameworld) => new ClearDesc(pars, gameworld),
				new List<string> {
					"perceivable",
					"tag"
				}, // parameter names
				new List<string> {
					"The perceivable who's description is being altered",
					"A short bit of text identifying this specific effect set by the original OverrideDesc call",
				}, // parameter help text
				"Clears a custom full Desc override with the specified tag set by progs from this perceivable", // help text for the function,
				"Perception", // the category to which this function belongs,
				FutureProgVariableTypes.Boolean // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected ClearDesc(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		perceivable.RemoveAllEffects<OverrideDescFromProg>(x => x.Tag.EqualTo(tag), true);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}
