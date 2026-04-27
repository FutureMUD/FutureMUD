using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class WeekdayFunction : BuiltInFunction
{
	private readonly int _direction;
	private readonly ProgVariableTypes _returnType;

	private WeekdayFunction(IList<IFunction> parameters, int direction, ProgVariableTypes returnType)
		: base(parameters)
	{
		_direction = direction;
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

		var count = _direction * GetOccurrenceCount();
		if (count == 0)
		{
			ErrorMessage = $"{FunctionName} must be supplied a non-zero occurrence number.";
			return StatementResult.Error;
		}

		switch ((_returnType & ~ProgVariableTypes.Literal).LegacyCode)
		{
			case ProgVariableTypeCode.DateTime:
				return ExecuteSystemDateTime(count);
			case ProgVariableTypeCode.MudDateTime:
				return ExecuteMudDateTime(count);
			default:
				throw new NotSupportedException($"Unsupported weekday function return type {_returnType.Describe()}.");
		}
	}

	private string FunctionName => _direction > 0 ? "NextWeekday" : "LastWeekday";

	private int GetOccurrenceCount()
	{
		if (ParameterFunctions.Count == 2)
		{
			return 1;
		}

		if (ParameterFunctions[2].Result?.GetObject is not decimal value ||
		    decimal.Truncate(value) != value ||
		    value > int.MaxValue ||
		    value <= int.MinValue)
		{
			return 0;
		}

		return (int)value;
	}

	private StatementResult ExecuteSystemDateTime(int count)
	{
		if (ParameterFunctions[0].Result?.GetObject is not System.DateTime date)
		{
			ErrorMessage = $"{FunctionName} must be supplied a non-null DateTime.";
			return StatementResult.Error;
		}

		if (!TryGetSystemWeekday(ParameterFunctions[1].Result?.GetObject?.ToString(), out var weekday))
		{
			ErrorMessage = $"{FunctionName} could not identify the supplied weekday name.";
			return StatementResult.Error;
		}

		try
		{
			Result = new DateTimeVariable(GetSystemWeekday(date, weekday, count));
			return StatementResult.Normal;
		}
		catch (ArgumentOutOfRangeException)
		{
			ErrorMessage = $"{FunctionName} would produce a DateTime outside the supported range.";
			return StatementResult.Error;
		}
	}

	private StatementResult ExecuteMudDateTime(int count)
	{
		if (ParameterFunctions[0].Result?.GetObject is not MudDateTime dateTime || dateTime.Date is null)
		{
			Result = MudDateTime.Never;
			return StatementResult.Normal;
		}

		if (!TryGetMudWeekday(dateTime, ParameterFunctions[1].Result?.GetObject?.ToString(), out var weekday))
		{
			ErrorMessage = $"{FunctionName} could not identify the supplied weekday name for that calendar.";
			return StatementResult.Error;
		}

		Result = GetMudWeekday(dateTime, weekday, count);
		return StatementResult.Normal;
	}

	private static System.DateTime GetSystemWeekday(System.DateTime date, DayOfWeek weekday, int count)
	{
		var occurrences = Math.Abs(count);
		var direction = Math.Sign(count);
		var current = (int)date.DayOfWeek;
		var target = (int)weekday;
		var delta = direction > 0
			? (target - current + 7) % 7
			: (current - target + 7) % 7;

		if (delta == 0)
		{
			delta = 7;
		}

		return date.AddDays(direction * (delta + (occurrences - 1) * 7));
	}

	private static MudDateTime GetMudWeekday(MudDateTime dateTime, int weekday, int count)
	{
		var result = new MudDateTime(dateTime);
		result.Time.DaysOffsetFromDatum = 0;
		var occurrences = Math.Abs(count);
		var direction = Math.Sign(count);

		while (occurrences > 0)
		{
			result.Date.AdvanceDays(direction);
			if (result.Date.WeekdayIndex == weekday)
			{
				occurrences--;
			}
		}

		return result;
	}

	private static bool TryGetSystemWeekday(string text, out DayOfWeek weekday)
	{
		if (!string.IsNullOrWhiteSpace(text) &&
		    Enum.TryParse(text, ignoreCase: true, out weekday) &&
		    Enum.IsDefined(weekday))
		{
			return true;
		}

		var dateFormat = CultureInfo.InvariantCulture.DateTimeFormat;
		for (var i = 0; i < dateFormat.DayNames.Length; i++)
		{
			if (dateFormat.DayNames[i].EqualTo(text) || dateFormat.AbbreviatedDayNames[i].EqualTo(text))
			{
				weekday = (DayOfWeek)i;
				return true;
			}
		}

		weekday = default;
		return false;
	}

	private static bool TryGetMudWeekday(MudDateTime dateTime, string text, out int weekday)
	{
		weekday = dateTime.Calendar.Weekdays.FindIndex(x => x.EqualTo(text));
		if (weekday >= 0)
		{
			return true;
		}

		var matches = dateTime.Calendar.Weekdays
		                      .Select((value, index) => (Value: value, Index: index))
		                      .Where(x => x.Value.StartsWith(text ?? string.Empty,
			                      StringComparison.InvariantCultureIgnoreCase))
		                      .ToList();

		if (matches.Count != 1)
		{
			return false;
		}

		weekday = matches[0].Index;
		return true;
	}

	public static void RegisterFunctionCompiler()
	{
		Register("nextweekday", 1);
		Register("lastweekday", -1);
	}

	private static void Register(string name, int direction)
	{
		RegisterForType(name, direction, ProgVariableTypes.DateTime);
		RegisterForType(name, direction, ProgVariableTypes.MudDateTime);
	}

	private static void RegisterForType(string name, int direction, ProgVariableTypes type)
	{
		var returnType = type;
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			new[] { type, ProgVariableTypes.Text },
			(pars, gameworld) => new WeekdayFunction(pars, direction, returnType),
			new[] { "Date", "Weekday" },
			new[]
			{
				"The date to use as the exclusive starting point.",
				"The weekday name to seek."
			},
			$"Returns the {(direction > 0 ? "next" : "last")} matching weekday after the supplied date.",
			"DateTime",
			returnType
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			new[] { type, ProgVariableTypes.Text, ProgVariableTypes.Number },
			(pars, gameworld) => new WeekdayFunction(pars, direction, returnType),
			new[] { "Date", "Weekday", "Occurrences" },
			new[]
			{
				"The date to use as the exclusive starting point.",
				"The weekday name to seek.",
				"The nth matching weekday to seek. Negative values reverse the direction."
			},
			$"Returns the nth {(direction > 0 ? "next" : "last")} matching weekday after the supplied date.",
			"DateTime",
			returnType
		));
	}
}
