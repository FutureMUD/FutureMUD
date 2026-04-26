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

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Text;
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

        object result = ParameterFunctions.First().Result?.GetObject;
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
            new[] { ProgVariableTypes.Location },
            (pars, gameworld) => new TimeOfDayFunction(pars),
            new List<string> { "location" },
            new List<string> { "The room whose zone clock should be used to determine the current time-of-day band." },
            "Returns the current time-of-day band for a room or zone, such as night or dawn. A null argument falls back to Night.",
            "Climate",
            ProgVariableTypes.Text
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "timeofday",
            new[] { ProgVariableTypes.Zone },
            (pars, gameworld) => new TimeOfDayFunction(pars),
            new List<string> { "zone" },
            new List<string> { "The zone whose clock should be used to determine the current time-of-day band." },
            "Returns the current time-of-day band for a room or zone, such as night or dawn. A null argument falls back to Night.",
            "Climate",
            ProgVariableTypes.Text
        ));
    }
}
