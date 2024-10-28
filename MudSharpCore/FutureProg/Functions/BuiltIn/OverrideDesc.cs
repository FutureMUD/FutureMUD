using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class OverrideDesc : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"OverrideDesc".ToLowerInvariant(),
				new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Text }, // the parameters the function takes
				(pars, gameworld) => new OverrideDesc(pars, gameworld),
				new List<string> {
					"Perceivable",
					"Tag",
					"Description"
				}, // parameter names
				new List<string> {
					"The perceivable who's description is being altered",
					"A short bit of text identifying this specific effect and allowing you to remove it later",
					"The overriden description for this perceivable. Can include description markups"
				}, // parameter help text
				"This function adds an override to the target's full description, which applies to all perceivers. Lasts until the ClearDesc function is used on this perceivable.", // help text for the function,
				"Perception", // the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"OverrideDesc".ToLowerInvariant(),
				new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.TimeSpan }, // the parameters the function takes
				(pars, gameworld) => new OverrideDesc(pars, gameworld),
				new List<string> {
					"Perceivable",
					"Tag",
					"Description",
					"Time"
				}, // parameter names
				new List<string> {
					"The perceivable who's description is being altered",
					"A short bit of text identifying this specific effect and allowing you to remove it later",
					"The overriden description for this perceivable. Can include description markups",
					"The time that this effect will apply for. If null, applies forever"
				}, // parameter help text
				"This function adds an override to the target's full description, which applies to all perceivers. Lasts until the specified duration, or until ClearDesc called if null.", // help text for the function,
				"Perception", // the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"OverrideDesc".ToLowerInvariant(),
				new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.TimeSpan, ProgVariableTypes.Perceiver }, // the parameters the function takes
				(pars, gameworld) => new OverrideDesc(pars, gameworld),
				new List<string> {
					"Perceivable",
					"Tag",
					"Description",
					"Time",
					"Perceiver"
				}, // parameter names
				new List<string> {
					"The perceivable who's description is being altered",
					"A short bit of text identifying this specific effect and allowing you to remove it later",
					"The overriden description for this perceivable. Can include description markups",
					"The time that this effect will apply for. If null, applies forever",
					"If specified, the description is only overriden for this perceiver. If null, applies to everyone"
				}, // parameter help text
				"This function adds an override to the target's full description, which applies to the specified perceiver only (or all perceivers if null). Lasts until the specified duration, or until ClearDesc called if null.", // help text for the function,
				"Perception", // the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"OverrideDesc".ToLowerInvariant(),
				new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.TimeSpan, ProgVariableTypes.Perceiver, ProgVariableTypes.Text }, // the parameters the function takes
				(pars, gameworld) => new OverrideDesc(pars, gameworld),
				new List<string> {
					"Perceivable",
					"Tag",
					"Description",
					"Time",
					"Perceiver",
					"Prog"
				}, // parameter names
				new List<string> {
					"The perceivable who's description is being altered",
					"A short bit of text identifying this specific effect and allowing you to remove it later",
					"The overriden description for this perceivable. Can include description markups",
					"The time that this effect will apply for. If null, applies forever",
					"If specified, the description is only overriden for this perceiver. If null, applies to everyone",
					"A prog taking a perceivable and perceiver input (or just a single perceivable) and returning boolean that controls whether this applies"
				}, // parameter help text
				"This function adds an override to the target's full description, which applies to the specified perceiver only (or all perceivers if null) and only when the filter prog applies. Lasts until the specified duration, or until ClearDesc called if null.", // help text for the function,
				"Perception", // the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"OverrideDesc".ToLowerInvariant(),
				new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.TimeSpan, ProgVariableTypes.Perceiver, ProgVariableTypes.Number }, // the parameters the function takes
				(pars, gameworld) => new OverrideDesc(pars, gameworld),
				new List<string> {
					"Perceivable",
					"Tag",
					"Description",
					"Time",
					"Perceiver",
					"Prog"
				}, // parameter names
				new List<string> {
					"The perceivable who's description is being altered",
					"A short bit of text identifying this specific effect and allowing you to remove it later",
					"The overriden description for this perceivable. Can include description markups",
					"The time that this effect will apply for. If null, applies forever",
					"If specified, the description is only overriden for this perceiver. If null, applies to everyone",
					"A prog taking a perceivable and perceiver input (or just a single perceivable) and returning boolean that controls whether this applies"
				}, // parameter help text
				"This function adds an override to the target's full description, which applies to the specified perceiver only (or all perceivers if null) and only when the filter prog applies. Lasts until the specified duration, or until ClearDesc called if null.", // help text for the function,
				"Perception", // the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected OverrideDesc(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		var text = ParameterFunctions[2].Result?.GetObject as string;
		if (string.IsNullOrEmpty(text))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var timespan = ParameterFunctions.Count > 3 ? ParameterFunctions[3].Result?.GetObject as TimeSpan? : null;
		var perceiver = (ParameterFunctions.Count > 4 ? ParameterFunctions[4].Result?.GetObject : null) as IPerceiver;
		IFutureProg prog = null;
		if (ParameterFunctions.Count > 5)
		{
			if (ParameterFunctions[5].ReturnType.CompatibleWith(ProgVariableTypes.Text))
			{
				prog = Gameworld.FutureProgs.GetByName(ParameterFunctions[5].Result?.GetObject?.ToString() ?? "");
			}
			else
			{
				prog = Gameworld.FutureProgs.Get((long)(ParameterFunctions[5].Result?.GetObject as decimal? ?? 0.0M));
			}

			if (prog is not null)
			{
				if (!prog.Parameters.CompatibleWith(new[] { ProgVariableTypes.Perceivable }) && !prog.Parameters.CompatibleWith(new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Perceiver }))
				{
					prog = null;
				}

				if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
				{
					prog = null;
				}
			}
		}

		var effect = new OverrideDescFromProg(perceivable, text, tag, perceiver, prog);
		perceivable.RemoveAllEffects<OverrideDescFromProg>(x => x.Tag.EqualTo(tag), true);
		if (timespan is not null)
		{
			perceivable.AddEffect(effect, timespan.Value);
		}
		else
		{
			perceivable.AddEffect(effect);
		}

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}
