using MudSharp.Community;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Clan;

internal class ToAppointmentFunction : BuiltInFunction
{
    private readonly IFuturemud _gameworld;

    public ToAppointmentFunction(IList<IFunction> parameters, IFuturemud gameworld)
        : base(parameters)
    {
        _gameworld = gameworld;
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.ClanAppointment;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        IClan clan = (IClan)ParameterFunctions.ElementAt(0).Result;
        if (clan == null)
        {
            ErrorMessage = "Clan Function in ToAppointment returned null.";
            return StatementResult.Error;
        }

        Result = ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Text)
            ? clan.Appointments.FirstOrDefault(
                x =>
                    x.Name.Equals((string)ParameterFunctions.ElementAt(1).Result.GetObject,
                        StringComparison.InvariantCultureIgnoreCase))
            : clan.Appointments.FirstOrDefault(
                x => x.Id == (int)(decimal)ParameterFunctions.ElementAt(1).Result.GetObject);

        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "toappointment",
            new[] { ProgVariableTypes.Clan, ProgVariableTypes.Number },
            (pars, gameworld) => new ToAppointmentFunction(pars, gameworld),
            new List<string> { "clan", "id" },
            new List<string> { "The clan whose appointments should be searched.", "The numeric ID of the appointment to find." },
            "Looks up an appointment within a clan by ID or name. Errors if the clan is null; returns null if no appointment matches.",
            "Clans",
            ProgVariableTypes.ClanAppointment
        ));

        FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
            "toappointment",
            new[] { ProgVariableTypes.Clan, ProgVariableTypes.Text },
            (pars, gameworld) => new ToAppointmentFunction(pars, gameworld),
            new List<string> { "clan", "name" },
            new List<string> { "The clan whose appointments should be searched.", "The appointment name to find, matched case-insensitively." },
            "Looks up an appointment within a clan by ID or name. Errors if the clan is null; returns null if no appointment matches.",
            "Clans",
            ProgVariableTypes.ClanAppointment
        ));
    }
}
