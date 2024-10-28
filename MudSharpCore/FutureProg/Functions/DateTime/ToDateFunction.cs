using System.Collections.Generic;
using System.Globalization;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Time;
using Calendar = MudSharp.TimeAndDate.Date.Calendar;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class ToDateFunction : BuiltInFunction
{
	private ToDateFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.DateTime;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var dateText = (string)ParameterFunctions[0].Result.GetObject;
		var dateMask = (string)ParameterFunctions[1].Result.GetObject;

		if (
			!System.DateTime.TryParseExact(dateText, dateMask, CultureInfo.InvariantCulture,
				DateTimeStyles.AdjustToUniversal, out var date))
		{
			ErrorMessage = "Invalid date in ToDate function";
			return StatementResult.Error;
		}

		Result = new DateTimeVariable(date);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"todate",
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Text },
			(pars, gameworld) => new ToDateFunction(pars)
		));
	}
}

internal class ToMudDateFunction : BuiltInFunction
{
	private ToMudDateFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.MudDateTime;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result?.GetObject is not Calendar calendar)
		{
			Result = MudDateTime.Never;
			return StatementResult.Normal;
		}

		var clock = ParameterFunctions[1].Result?.GetObject as Clock ?? calendar.FeedClock;
		var text = ParameterFunctions?[2].Result?.GetObject?.ToString() ??
		           ParameterFunctions[1].Result?.GetObject?.ToString();

		if (string.IsNullOrEmpty(text))
		{
			Result = MudDateTime.Never;
			return StatementResult.Normal;
		}

		Result = MudDateTime.TryParse(text, calendar, clock, out var dt) ? dt : MudDateTime.Never;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"todate",
			new[] { ProgVariableTypes.Calendar, ProgVariableTypes.Clock, ProgVariableTypes.Text },
			(pars, gameworld) => new ToMudDateFunction(pars)
		));
	}
}