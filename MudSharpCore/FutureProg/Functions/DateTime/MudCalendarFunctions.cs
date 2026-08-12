using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;

namespace MudSharp.FutureProg.Functions.DateTime;

internal sealed class MudCalendarFunction : BuiltInFunction
{
	private readonly MudCalendarOperation _operation;
	private readonly ProgVariableTypes _returnType;

	private enum MudCalendarOperation
	{
		DaysBetween,
		MonthStart,
		MonthEnd,
		WeekdayName
	}

	private MudCalendarFunction(IList<IFunction> parameters, MudCalendarOperation operation,
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

		var value = ParameterFunctions[0].Result?.GetObject as MudDateTime;
		switch (_operation)
		{
			case MudCalendarOperation.DaysBetween:
				var end = ParameterFunctions[1].Result?.GetObject as MudDateTime;
				if (value?.Date is null || end?.Date is null)
				{
					ErrorMessage = "DaysBetween cannot be used with the Never mud date/time.";
					return StatementResult.Error;
				}

				if (value.Calendar.Id != end.Calendar.Id)
				{
					end = end.ConvertToOtherCalendar(value.Calendar);
				}

				Result = new NumberVariable(end.Date.SignedDaysDifference(value.Date));
				return StatementResult.Normal;
			case MudCalendarOperation.MonthStart:
				Result = value?.StartOfMonth() ?? MudDateTime.Never;
				return StatementResult.Normal;
			case MudCalendarOperation.MonthEnd:
				Result = value?.EndOfMonth() ?? MudDateTime.Never;
				return StatementResult.Normal;
			case MudCalendarOperation.WeekdayName:
				Result = new TextVariable(value?.Date?.Weekday ?? string.Empty);
				return StatementResult.Normal;
			default:
				throw new NotSupportedException($"Unsupported mud calendar operation {_operation}.");
		}
	}

	public static void RegisterFunctionCompiler()
	{
		Register("daysbetween", MudCalendarOperation.DaysBetween, ProgVariableTypes.Number,
			[ProgVariableTypes.MudDateTime, ProgVariableTypes.MudDateTime],
			["start", "end"],
			[
				"The start of the calendar-day range.",
				"The end of the calendar-day range. A later value produces a positive result."
			],
			"Returns the signed number of calendar days between two mud date/times. Different calendars are aligned using their current-date anchor. Never values are an execution error.");
		Register("monthstart", MudCalendarOperation.MonthStart, ProgVariableTypes.MudDateTime,
			[ProgVariableTypes.MudDateTime], ["date"], ["The mud date/time whose month should be inspected."],
			"Returns the exact opening boundary of the supplied mud date/time's month. Never returns Never.");
		Register("monthend", MudCalendarOperation.MonthEnd, ProgVariableTypes.MudDateTime,
			[ProgVariableTypes.MudDateTime], ["date"], ["The mud date/time whose month should be inspected."],
			"Returns the final representable in-game second before the supplied mud date/time's next month begins. Never returns Never.");
		Register("weekdayname", MudCalendarOperation.WeekdayName, ProgVariableTypes.Text,
			[ProgVariableTypes.MudDateTime], ["date"], ["The mud date/time whose weekday should be inspected."],
			"Returns the calendar weekday name, or empty text for Never and non-weekday intercalary days.");
	}

	private static void Register(string name, MudCalendarOperation operation, ProgVariableTypes returnType,
		IEnumerable<ProgVariableTypes> parameterTypes, IEnumerable<string> parameterNames,
		IEnumerable<string> parameterHelp, string functionHelp)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			parameterTypes,
			(pars, _) => new MudCalendarFunction(pars, operation, returnType),
			parameterNames,
			parameterHelp,
			functionHelp,
			"Date/Time",
			returnType));
	}
}
