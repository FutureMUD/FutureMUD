using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class DateTimeComponentFunction : BuiltInFunction
{
	private readonly DateTimeOperation _operation;
	private readonly ProgVariableTypes _returnType;

	private enum DateTimeOperation
	{
		DateYear,
		DateMonth,
		DateDay,
		DateHour,
		DateMinute,
		DateSecond,
		DateMillisecond,
		DateDayOfYear,
		DateDayOfWeek,
		DateIsWeekend,
		DateOnly,
		SpanDays,
		SpanHours,
		SpanMinutes,
		SpanSeconds,
		SpanMilliseconds,
		TotalDays,
		TotalHours,
		TotalMinutes,
		TotalSeconds,
		TotalMilliseconds,
		SpanAbsolute,
		SpanNegate,
		TimeSpanFromDays,
		TimeSpanFromHours,
		TimeSpanFromMinutes,
		TimeSpanFromSeconds,
		TimeSpanFromMilliseconds,
		MakeTimeSpan
	}

	private DateTimeComponentFunction(IList<IFunction> parameters, DateTimeOperation operation,
		ProgVariableTypes returnType) : base(parameters)
	{
		_operation = operation;
		_returnType = returnType;
	}

	public override ProgVariableTypes ReturnType
	{
		get => _returnType;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		switch (_operation)
		{
			case DateTimeOperation.DateYear:
				Result = new NumberVariable(GetDateTime(0).Year);
				return StatementResult.Normal;
			case DateTimeOperation.DateMonth:
				Result = new NumberVariable(GetDateTime(0).Month);
				return StatementResult.Normal;
			case DateTimeOperation.DateDay:
				Result = new NumberVariable(GetDateTime(0).Day);
				return StatementResult.Normal;
			case DateTimeOperation.DateHour:
				Result = new NumberVariable(GetDateTime(0).Hour);
				return StatementResult.Normal;
			case DateTimeOperation.DateMinute:
				Result = new NumberVariable(GetDateTime(0).Minute);
				return StatementResult.Normal;
			case DateTimeOperation.DateSecond:
				Result = new NumberVariable(GetDateTime(0).Second);
				return StatementResult.Normal;
			case DateTimeOperation.DateMillisecond:
				Result = new NumberVariable(GetDateTime(0).Millisecond);
				return StatementResult.Normal;
			case DateTimeOperation.DateDayOfYear:
				Result = new NumberVariable(GetDateTime(0).DayOfYear);
				return StatementResult.Normal;
			case DateTimeOperation.DateDayOfWeek:
				Result = new NumberVariable((int)GetDateTime(0).DayOfWeek);
				return StatementResult.Normal;
			case DateTimeOperation.DateIsWeekend:
				var day = GetDateTime(0).DayOfWeek;
				Result = new BooleanVariable(day is DayOfWeek.Saturday or DayOfWeek.Sunday);
				return StatementResult.Normal;
			case DateTimeOperation.DateOnly:
				Result = new DateTimeVariable(GetDateTime(0).Date);
				return StatementResult.Normal;
			case DateTimeOperation.SpanDays:
				Result = new NumberVariable(GetTimeSpan(0).Days);
				return StatementResult.Normal;
			case DateTimeOperation.SpanHours:
				Result = new NumberVariable(GetTimeSpan(0).Hours);
				return StatementResult.Normal;
			case DateTimeOperation.SpanMinutes:
				Result = new NumberVariable(GetTimeSpan(0).Minutes);
				return StatementResult.Normal;
			case DateTimeOperation.SpanSeconds:
				Result = new NumberVariable(GetTimeSpan(0).Seconds);
				return StatementResult.Normal;
			case DateTimeOperation.SpanMilliseconds:
				Result = new NumberVariable(GetTimeSpan(0).Milliseconds);
				return StatementResult.Normal;
			case DateTimeOperation.TotalDays:
				Result = new NumberVariable(GetTimeSpan(0).TotalDays);
				return StatementResult.Normal;
			case DateTimeOperation.TotalHours:
				Result = new NumberVariable(GetTimeSpan(0).TotalHours);
				return StatementResult.Normal;
			case DateTimeOperation.TotalMinutes:
				Result = new NumberVariable(GetTimeSpan(0).TotalMinutes);
				return StatementResult.Normal;
			case DateTimeOperation.TotalSeconds:
				Result = new NumberVariable(GetTimeSpan(0).TotalSeconds);
				return StatementResult.Normal;
			case DateTimeOperation.TotalMilliseconds:
				Result = new NumberVariable(GetTimeSpan(0).TotalMilliseconds);
				return StatementResult.Normal;
			case DateTimeOperation.SpanAbsolute:
				Result = new TimeSpanVariable(SafeTimeSpan(() => GetTimeSpan(0).Duration()));
				return StatementResult.Normal;
			case DateTimeOperation.SpanNegate:
				Result = new TimeSpanVariable(SafeTimeSpan(() => GetTimeSpan(0).Negate()));
				return StatementResult.Normal;
			case DateTimeOperation.TimeSpanFromDays:
				Result = new TimeSpanVariable(SafeTimeSpan(() => TimeSpan.FromDays(GetDouble(0))));
				return StatementResult.Normal;
			case DateTimeOperation.TimeSpanFromHours:
				Result = new TimeSpanVariable(SafeTimeSpan(() => TimeSpan.FromHours(GetDouble(0))));
				return StatementResult.Normal;
			case DateTimeOperation.TimeSpanFromMinutes:
				Result = new TimeSpanVariable(SafeTimeSpan(() => TimeSpan.FromMinutes(GetDouble(0))));
				return StatementResult.Normal;
			case DateTimeOperation.TimeSpanFromSeconds:
				Result = new TimeSpanVariable(SafeTimeSpan(() => TimeSpan.FromSeconds(GetDouble(0))));
				return StatementResult.Normal;
			case DateTimeOperation.TimeSpanFromMilliseconds:
				Result = new TimeSpanVariable(SafeTimeSpan(() => TimeSpan.FromMilliseconds(GetDouble(0))));
				return StatementResult.Normal;
			case DateTimeOperation.MakeTimeSpan:
				Result = new TimeSpanVariable(MakeTimeSpan());
				return StatementResult.Normal;
			default:
				throw new NotSupportedException($"Unknown DateTime utility operation {_operation}.");
		}
	}

	private System.DateTime GetDateTime(int index)
	{
		return ParameterFunctions[index].Result?.GetObject is System.DateTime value ? value : default;
	}

	private TimeSpan GetTimeSpan(int index)
	{
		return ParameterFunctions[index].Result?.GetObject is TimeSpan value ? value : default;
	}

	private double GetDouble(int index)
	{
		return ParameterFunctions[index].Result?.GetObject is decimal value ? (double)value : 0.0;
	}

	private TimeSpan MakeTimeSpan()
	{
		return SafeTimeSpan(() =>
			TimeSpan.FromDays(GetDouble(0)) +
			TimeSpan.FromHours(GetDouble(1)) +
			TimeSpan.FromMinutes(GetDouble(2)) +
			TimeSpan.FromSeconds(GetDouble(3)) +
			TimeSpan.FromMilliseconds(GetDouble(4))
		);
	}

	private static TimeSpan SafeTimeSpan(Func<TimeSpan> factory)
	{
		try
		{
			return factory();
		}
		catch (OverflowException)
		{
			return TimeSpan.Zero;
		}
	}

	private static void RegisterDateFunction(string name, DateTimeOperation operation, ProgVariableTypes returnType,
		string functionHelp)
	{
		RegisterFunction(
			name,
			operation,
			returnType,
			new[] { ProgVariableTypes.DateTime },
			new[] { "date" },
			new[] { "The real-world DateTime value to inspect or transform" },
			functionHelp
		);
	}

	private static void RegisterSpanFunction(string name, DateTimeOperation operation, ProgVariableTypes returnType,
		string functionHelp)
	{
		RegisterFunction(
			name,
			operation,
			returnType,
			new[] { ProgVariableTypes.TimeSpan },
			new[] { "timespan" },
			new[] { "The TimeSpan value to inspect or transform" },
			functionHelp
		);
	}

	private static void RegisterNumberToSpanFunction(string name, DateTimeOperation operation, string parameterName,
		string functionHelp)
	{
		RegisterFunction(
			name,
			operation,
			ProgVariableTypes.TimeSpan,
			new[] { ProgVariableTypes.Number },
			new[] { parameterName },
			new[] { $"The number of {parameterName} to convert into a TimeSpan" },
			functionHelp
		);
	}

	private static void RegisterFunction(string name, DateTimeOperation operation, ProgVariableTypes returnType,
		IEnumerable<ProgVariableTypes> parameters, IEnumerable<string> parameterNames,
		IEnumerable<string> parameterHelp, string functionHelp)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			parameters,
			(pars, gameworld) => new DateTimeComponentFunction(pars, operation, returnType),
			parameterNames,
			parameterHelp,
			functionHelp,
			"DateTime",
			returnType
		));
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterDateFunction("dateyear", DateTimeOperation.DateYear, ProgVariableTypes.Number,
			"Returns the year component of the supplied real-world DateTime value.");
		RegisterDateFunction("datemonth", DateTimeOperation.DateMonth, ProgVariableTypes.Number,
			"Returns the month component of the supplied real-world DateTime value, from 1 to 12.");
		RegisterDateFunction("dateday", DateTimeOperation.DateDay, ProgVariableTypes.Number,
			"Returns the day-of-month component of the supplied real-world DateTime value.");
		RegisterDateFunction("datehour", DateTimeOperation.DateHour, ProgVariableTypes.Number,
			"Returns the hour component of the supplied real-world DateTime value, from 0 to 23.");
		RegisterDateFunction("dateminute", DateTimeOperation.DateMinute, ProgVariableTypes.Number,
			"Returns the minute component of the supplied real-world DateTime value.");
		RegisterDateFunction("datesecond", DateTimeOperation.DateSecond, ProgVariableTypes.Number,
			"Returns the second component of the supplied real-world DateTime value.");
		RegisterDateFunction("datemillisecond", DateTimeOperation.DateMillisecond, ProgVariableTypes.Number,
			"Returns the millisecond component of the supplied real-world DateTime value.");
		RegisterDateFunction("datedayofyear", DateTimeOperation.DateDayOfYear, ProgVariableTypes.Number,
			"Returns the one-based day-of-year number for the supplied real-world DateTime value.");
		RegisterDateFunction("datedayofweek", DateTimeOperation.DateDayOfWeek, ProgVariableTypes.Number,
			"Returns the day-of-week number for the supplied real-world DateTime value, where Sunday is 0 and Saturday is 6.");
		RegisterDateFunction("dateisweekend", DateTimeOperation.DateIsWeekend, ProgVariableTypes.Boolean,
			"Returns true if the supplied real-world DateTime value falls on Saturday or Sunday.");
		RegisterDateFunction("dateonly", DateTimeOperation.DateOnly, ProgVariableTypes.DateTime,
			"Returns the supplied real-world DateTime value with the time component set to midnight.");

		RegisterSpanFunction("spandays", DateTimeOperation.SpanDays, ProgVariableTypes.Number,
			"Returns the day component of the supplied TimeSpan value.");
		RegisterSpanFunction("spanhours", DateTimeOperation.SpanHours, ProgVariableTypes.Number,
			"Returns the hour component of the supplied TimeSpan value after whole days are removed.");
		RegisterSpanFunction("spanminutes", DateTimeOperation.SpanMinutes, ProgVariableTypes.Number,
			"Returns the minute component of the supplied TimeSpan value after whole hours are removed.");
		RegisterSpanFunction("spanseconds", DateTimeOperation.SpanSeconds, ProgVariableTypes.Number,
			"Returns the second component of the supplied TimeSpan value after whole minutes are removed.");
		RegisterSpanFunction("spanmilliseconds", DateTimeOperation.SpanMilliseconds, ProgVariableTypes.Number,
			"Returns the millisecond component of the supplied TimeSpan value after whole seconds are removed.");
		RegisterSpanFunction("totaldays", DateTimeOperation.TotalDays, ProgVariableTypes.Number,
			"Returns the total duration of the supplied TimeSpan value expressed in days, including fractional days.");
		RegisterSpanFunction("totalhours", DateTimeOperation.TotalHours, ProgVariableTypes.Number,
			"Returns the total duration of the supplied TimeSpan value expressed in hours, including fractional hours.");
		RegisterSpanFunction("totalminutes", DateTimeOperation.TotalMinutes, ProgVariableTypes.Number,
			"Returns the total duration of the supplied TimeSpan value expressed in minutes, including fractional minutes.");
		RegisterSpanFunction("totalseconds", DateTimeOperation.TotalSeconds, ProgVariableTypes.Number,
			"Returns the total duration of the supplied TimeSpan value expressed in seconds, including fractional seconds.");
		RegisterSpanFunction("totalmilliseconds", DateTimeOperation.TotalMilliseconds, ProgVariableTypes.Number,
			"Returns the total duration of the supplied TimeSpan value expressed in milliseconds, including fractional milliseconds.");
		RegisterSpanFunction("spanabs", DateTimeOperation.SpanAbsolute, ProgVariableTypes.TimeSpan,
			"Returns the absolute value of the supplied TimeSpan. Unrepresentable values return a zero TimeSpan.");
		RegisterSpanFunction("spannegate", DateTimeOperation.SpanNegate, ProgVariableTypes.TimeSpan,
			"Returns the supplied TimeSpan with its sign reversed. Unrepresentable values return a zero TimeSpan.");

		RegisterNumberToSpanFunction("timespanfromdays", DateTimeOperation.TimeSpanFromDays, "days",
			"Returns a TimeSpan representing the supplied number of days.");
		RegisterNumberToSpanFunction("timespanfromhours", DateTimeOperation.TimeSpanFromHours, "hours",
			"Returns a TimeSpan representing the supplied number of hours.");
		RegisterNumberToSpanFunction("timespanfromminutes", DateTimeOperation.TimeSpanFromMinutes, "minutes",
			"Returns a TimeSpan representing the supplied number of minutes.");
		RegisterNumberToSpanFunction("timespanfromseconds", DateTimeOperation.TimeSpanFromSeconds, "seconds",
			"Returns a TimeSpan representing the supplied number of seconds.");
		RegisterNumberToSpanFunction("timespanfrommilliseconds", DateTimeOperation.TimeSpanFromMilliseconds, "milliseconds",
			"Returns a TimeSpan representing the supplied number of milliseconds.");

		RegisterFunction(
			"maketimespan",
			DateTimeOperation.MakeTimeSpan,
			ProgVariableTypes.TimeSpan,
			new[]
			{
				ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Number,
				ProgVariableTypes.Number, ProgVariableTypes.Number
			},
			new[] { "days", "hours", "minutes", "seconds", "milliseconds" },
			new[]
			{
				"The day component of the TimeSpan",
				"The hour component of the TimeSpan",
				"The minute component of the TimeSpan",
				"The second component of the TimeSpan",
				"The millisecond component of the TimeSpan"
			},
			"Returns a TimeSpan constructed from the supplied day, hour, minute, second and millisecond components. Unrepresentable values return a zero TimeSpan."
		);
	}
}
