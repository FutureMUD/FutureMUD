using MudSharp.Celestial;
using MudSharp.Construction;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.Climate;

internal class TimeOfDayFunction : BuiltInFunction
{
	public TimeOfDayFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Text;
		protected set { }
	}

	public override string ErrorMessage
	{
		get => ParameterFunctions.First().ErrorMessage;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var result = ParameterFunctions.First().Result?.GetObject;
		if (result == null)
		{
			Result = new TextVariable("Night");
			return StatementResult.Normal;
		}

		if (result is ICell cell)
		{
			Result = new TextVariable(cell.CurrentTimeOfDay.Describe());
			return StatementResult.Normal;
		}

		if (result is IZone zone)
		{
			Result = new TextVariable(zone.CurrentTimeOfDay.Describe());
			return StatementResult.Normal;
		}

		throw new ApplicationException("Invalid object passed to TimeOfDayFunction prog.");
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"timeofday",
			new[] { FutureProgVariableTypes.Location },
			(pars, gameworld) => new TimeOfDayFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"timeofday",
			new[] { FutureProgVariableTypes.Zone },
			(pars, gameworld) => new TimeOfDayFunction(pars)
		));
	}
}