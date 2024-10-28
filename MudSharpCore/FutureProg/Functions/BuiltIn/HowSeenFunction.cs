using System.Collections.Generic;
using System.Linq;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class HowSeenFunction : BuiltInFunction
{
	protected DescriptionType DescriptionType;

	public HowSeenFunction(IList<IFunction> parameters, DescriptionType type)
		: base(parameters)
	{
		DescriptionType = type;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Text;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		var perceivableFunction = ParameterFunctions.ElementAt(0);
		var perceiverFunction = ParameterFunctions.ElementAt(1);
		var properFunction = ParameterFunctions.ElementAt(2);
		var colourFunction = ParameterFunctions.ElementAt(3);

		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result =
			new TextVariable(
				((IPerceivable)perceivableFunction.Result).HowSeen((IPerceiver)perceiverFunction.Result,
					(bool?)properFunction.Result.GetObject ?? false, DescriptionType,
					(bool?)colourFunction.Result.GetObject ?? false));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"howseensdesc",
				new[]
				{
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceiver,
					ProgVariableTypes.Boolean, ProgVariableTypes.Boolean
				},
				(pars, gameworld) => new HowSeenFunction(pars, DescriptionType.Short),
				new List<string> { "thing", "perceiver", "proper", "coloured" },
				new List<string>
				{
					"The thing for which you want a description",
					"The perceiver through whose perception you want to filter the description",
					"Whether or not to change the output to proper case",
					"Whether or not to include colour in the description"
				},
				"This function gets the short description of something as if it was seen in the engine by the specified perceiver.",
				"Perception",
				ProgVariableTypes.Text
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"howseen",
				new[]
				{
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceiver,
					ProgVariableTypes.Boolean, ProgVariableTypes.Boolean
				},
				(pars, gameworld) => new HowSeenFunction(pars, DescriptionType.Short),
				new List<string> { "thing", "perceiver", "proper", "coloured" },
				new List<string>
				{
					"The thing for which you want a description",
					"The perceiver through whose perception you want to filter the description",
					"Whether or not to change the output to proper case",
					"Whether or not to include colour in the description"
				},
				"This function gets the short description of something as if it was seen in the engine by the specified perceiver.",
				"Perception",
				ProgVariableTypes.Text
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"howseenfull",
				new[]
				{
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceiver,
					ProgVariableTypes.Boolean, ProgVariableTypes.Boolean
				},
				(pars, gameworld) => new HowSeenFunction(pars, DescriptionType.Full),
				new List<string> { "thing", "perceiver", "proper", "coloured" },
				new List<string>
				{
					"The thing for which you want a description",
					"The perceiver through whose perception you want to filter the description",
					"Whether or not to change the output to proper case",
					"Whether or not to include colour in the description"
				},
				"This function gets the full (i.e. look) description of something as if it was seen in the engine by the specified perceiver.",
				"Perception",
				ProgVariableTypes.Text
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"howseenlong",
				new[]
				{
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceiver,
					ProgVariableTypes.Boolean, ProgVariableTypes.Boolean
				},
				(pars, gameworld) => new HowSeenFunction(pars, DescriptionType.Long),
				new List<string> { "thing", "perceiver", "proper", "coloured" },
				new List<string>
				{
					"The thing for which you want a description",
					"The perceiver through whose perception you want to filter the description",
					"Whether or not to change the output to proper case",
					"Whether or not to include colour in the description"
				},
				"This function gets the long (i.e. room) description of something as if it was seen in the engine by the specified perceiver.",
				"Perception",
				ProgVariableTypes.Text
			)
		);
	}
}