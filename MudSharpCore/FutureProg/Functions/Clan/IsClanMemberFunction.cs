using MudSharp.Character;
using MudSharp.Community;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Clan;

internal class IsClanMemberFunction : BuiltInFunction
{
    public IsClanMemberFunction(IList<IFunction> parameters)
        : base(parameters)
    {
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Boolean;
        protected set { }
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        IFunction characterFunction = ParameterFunctions.ElementAt(0);
        IFunction clanFunction = ParameterFunctions.ElementAt(1);
        IFunction thirdFunction = ParameterFunctions.ElementAtOrDefault(2);
        IFunction rankFunction = null, appointmentFunction = null, paygradeFunction = null;
        if (thirdFunction != null)
        {
            switch (thirdFunction.ReturnType.LegacyCode)
            {
                case ProgVariableTypeCode.ClanRank:
                    rankFunction = thirdFunction;
                    paygradeFunction = ParameterFunctions.ElementAtOrDefault(3);
                    break;
                case ProgVariableTypeCode.ClanAppointment:
                    appointmentFunction = thirdFunction;
                    break;
            }
        }

        if (characterFunction.Execute(variables) == StatementResult.Error)
        {
            ErrorMessage = "Character Function in IsClanMember Function returned an error: " +
                           characterFunction.ErrorMessage;
            return StatementResult.Error;
        }

        if (clanFunction.Execute(variables) == StatementResult.Error)
        {
            ErrorMessage = "Clan Function in IsClanMember Function returned an error: " + clanFunction.ErrorMessage;
            return StatementResult.Error;
        }

        if (characterFunction.Result?.GetObject is not ICharacter character)
        {
            ErrorMessage = "Character was null in IsClanMember Function.";
            return StatementResult.Error;
        }

        if (clanFunction.Result?.GetObject is not IClan clan)
        {
            ErrorMessage = "Clan was null in IsClanMember Function.";
            return StatementResult.Error;
        }

        Func<IClanMembership, bool> function;
        if (rankFunction != null)
        {
            if (rankFunction.Execute(variables) == StatementResult.Error)
            {
                ErrorMessage = "Rank Function in IsClanMember Function returned an error: " +
                               rankFunction.ErrorMessage;
                return StatementResult.Error;
            }

            if (paygradeFunction != null)
            {
                if (paygradeFunction.Execute(variables) == StatementResult.Error)
                {
                    ErrorMessage = "Paygrade Function in IsClanMember Function returned an error: " +
                                   paygradeFunction.ErrorMessage;
                    return StatementResult.Error;
                }

                if (rankFunction.Result?.GetObject is not IRank rank)
                {
                    ErrorMessage = "Rank was null in IsClanMember function.";
                    return StatementResult.Error;
                }

                if (paygradeFunction.Result?.GetObject is not IPaygrade paygrade)
                {
                    ErrorMessage = "Paygrade was null in IsClanMember function.";
                    return StatementResult.Error;
                }

                function = member =>
                    member.Clan.Equals(clan) &&
                    (rank.RankNumber < member.Rank.RankNumber ||
                     (rank.RankNumber == member.Rank.RankNumber &&
                      rank.Paygrades.IndexOf(member.Paygrade) >= rank.Paygrades.IndexOf(paygrade))
                    );
            }
            else
            {
                if (rankFunction.Result?.GetObject is not IRank rank)
                {
                    ErrorMessage = "Rank was null in IsClanMember function.";
                    return StatementResult.Error;
                }

                function =
                    member =>
                        member.Clan.Equals(clan) && rank.RankNumber <= member.Rank.RankNumber;
            }
        }
        else if (appointmentFunction != null)
        {
            if (appointmentFunction.Execute(variables) == StatementResult.Error)
            {
                ErrorMessage = "Appointment Function in IsClanMember Function returned an error: " +
                               appointmentFunction.ErrorMessage;
                return StatementResult.Error;
            }

            if (appointmentFunction.Result?.GetObject is not IAppointment appointment)
            {
                ErrorMessage = "Appointment was null in IsClanMember function.";
                return StatementResult.Error;
            }

            function =
                member =>
                    member.Clan.Equals(clan) && member.Appointments.Contains(appointment);
        }
        else
        {
            function = member => member.Clan.Equals(clan);
        }

        Result = new BooleanVariable(character.ClanMemberships.Any(function));
        return StatementResult.Normal;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "isclanmember",
                new[] { ProgVariableTypes.Character, ProgVariableTypes.Clan },
                (pars, gameworld) => new IsClanMemberFunction(pars),
                new List<string> { "character", "clan" },
                new List<string> { "The character whose memberships should be checked.", "The clan to check for membership." },
                "Checks whether a character is a member of a clan, optionally constrained by minimum rank, appointment, or rank and paygrade. Errors if required character, clan, rank, paygrade, or appointment inputs are null; returns false when the membership requirement is not met.",
                "Clans",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "isclanmember",
                new[]
                {
                    ProgVariableTypes.Character, ProgVariableTypes.Clan,
                    ProgVariableTypes.ClanRank
                },
                (pars, gameworld) => new IsClanMemberFunction(pars),
                new List<string> { "character", "clan", "rank" },
                new List<string> { "The character whose memberships should be checked.", "The clan to check for membership.", "The minimum clan rank the character must hold." },
                "Checks whether a character is a member of a clan, optionally constrained by minimum rank, appointment, or rank and paygrade. Errors if required character, clan, rank, paygrade, or appointment inputs are null; returns false when the membership requirement is not met.",
                "Clans",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "isclanmember",
                new[]
                {
                    ProgVariableTypes.Character, ProgVariableTypes.Clan,
                    ProgVariableTypes.ClanAppointment
                },
                (pars, gameworld) => new IsClanMemberFunction(pars),
                new List<string> { "character", "clan", "appointment" },
                new List<string> { "The character whose memberships should be checked.", "The clan to check for membership.", "The clan appointment the character must hold." },
                "Checks whether a character is a member of a clan, optionally constrained by minimum rank, appointment, or rank and paygrade. Errors if required character, clan, rank, paygrade, or appointment inputs are null; returns false when the membership requirement is not met.",
                "Clans",
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "isclanmember",
                new[]
                {
                    ProgVariableTypes.Character, ProgVariableTypes.Clan,
                    ProgVariableTypes.ClanRank, ProgVariableTypes.ClanPaygrade
                },
                (pars, gameworld) => new IsClanMemberFunction(pars),
                new List<string> { "character", "clan", "rank", "paygrade" },
                new List<string> { "The character whose memberships should be checked.", "The clan to check for membership.", "The minimum clan rank the character must hold.", "The minimum paygrade within the supplied rank." },
                "Checks whether a character is a member of a clan, optionally constrained by minimum rank, appointment, or rank and paygrade. Errors if required character, clan, rank, paygrade, or appointment inputs are null; returns false when the membership requirement is not met.",
                "Clans",
                ProgVariableTypes.Boolean
            )
        );
    }
}
