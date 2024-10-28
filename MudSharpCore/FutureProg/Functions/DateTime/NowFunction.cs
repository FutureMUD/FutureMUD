using System.Collections.Generic;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class NowFunction : BuiltInFunction
{
	public NowFunction(IList<IFunction> parameters)
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
		Result = new DateTimeVariable(System.DateTime.UtcNow);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"now",
			new ProgVariableTypes[] { },
			(pars, gameworld) => new NowFunction(pars)
		));
	}
}

internal class MudNowFunction : BuiltInFunction
{
	public MudNowFunction(IList<IFunction> parameters)
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
			Result = new MudDateTime(default, default, default(MudTimeZone));
			return StatementResult.Normal;
		}

		var date = calendar.CurrentDate;

		var clock = ParameterFunctions?[1].Result?.GetObject as Clock ?? calendar.FeedClock;
		if (clock == null)
		{
			Result = new MudDateTime(default, default, default(MudTimeZone));
			return StatementResult.Normal;
		}

		var time = clock.CurrentTime;

		var timezone = ParameterFunctions?[2].Result?.GetObject as MudTimeZone ?? clock.PrimaryTimezone;

		if (timezone != clock.PrimaryTimezone)
		{
			time = new MudTime(time).GetTimeByTimezone(timezone);
			if (time.DaysOffsetFromDatum != 0)
			{
				date = new MudDate(date);
				date.AdvanceDays(time.DaysOffsetFromDatum);
			}
		}

		Result = new MudDateTime(date, time, timezone);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"now",
			new[] { ProgVariableTypes.Calendar },
			(pars, gameworld) => new MudNowFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"now",
			new[] { ProgVariableTypes.Calendar, ProgVariableTypes.Clock },
			(pars, gameworld) => new MudNowFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"now",
			new[] { ProgVariableTypes.Calendar, ProgVariableTypes.Clock, ProgVariableTypes.Text },
			(pars, gameworld) => new MudNowFunction(pars)
		));
	}
}