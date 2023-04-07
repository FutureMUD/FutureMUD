using System;
using System.Collections.Generic;
using System.Globalization;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class DateToTextFunction : BuiltInFunction
{
	public DateToTextFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Text;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var date = (System.DateTime)ParameterFunctions[0].Result.GetObject;
		switch (ParameterFunctions.Count)
		{
			case 1:
				Result = new TextVariable(date.ToString(CultureInfo.InvariantCulture));
				break;
			case 2:
				Result = new TextVariable(date.ToString((IFormatProvider)ParameterFunctions[1].Result.GetObject));
				break;
			case 3:
				Result =
					new TextVariable(date.ToString((string)ParameterFunctions[1].Result.GetObject,
						(IFormatProvider)ParameterFunctions[2].Result.GetObject));
				break;
		}

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"totext",
			new[] { FutureProgVariableTypes.DateTime },
			(pars, gameworld) => new DateToTextFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"totext",
			new[] { FutureProgVariableTypes.DateTime, FutureProgVariableTypes.Toon },
			(pars, gameworld) => new DateToTextFunction(pars)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"totext",
			new[] { FutureProgVariableTypes.DateTime, FutureProgVariableTypes.Text, FutureProgVariableTypes.Toon },
			(pars, gameworld) => new DateToTextFunction(pars)
		));
	}
}

internal class MudDateToTextFunction : BuiltInFunction
{
	public MudDateToTextFunction(IList<IFunction> parameters, CalendarDisplayMode mode, TimeDisplayTypes type)
		: base(parameters)
	{
		_mode = mode;
		_type = type;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Text;
		protected set { }
	}

	private CalendarDisplayMode _mode { get; }
	private TimeDisplayTypes _type { get; }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var date = (MudDateTime)ParameterFunctions[0].Result.GetObject;
		Result = new TextVariable(date?.ToString(_mode, _type) ?? "Never");
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"totext",
			new[] { FutureProgVariableTypes.MudDateTime },
			(pars, gameworld) => new MudDateToTextFunction(pars, CalendarDisplayMode.Long, TimeDisplayTypes.Long)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"totextshort",
			new[] { FutureProgVariableTypes.MudDateTime },
			(pars, gameworld) => new MudDateToTextFunction(pars, CalendarDisplayMode.Short, TimeDisplayTypes.Short)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"totextwordy",
			new[] { FutureProgVariableTypes.MudDateTime },
			(pars, gameworld) => new MudDateToTextFunction(pars, CalendarDisplayMode.Wordy, TimeDisplayTypes.Long)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"totextcrude",
			new[] { FutureProgVariableTypes.MudDateTime },
			(pars, gameworld) => new MudDateToTextFunction(pars, CalendarDisplayMode.Long, TimeDisplayTypes.Crude)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"totextvague",
			new[] { FutureProgVariableTypes.MudDateTime },
			(pars, gameworld) => new MudDateToTextFunction(pars, CalendarDisplayMode.Long, TimeDisplayTypes.Vague)
		));
	}
}