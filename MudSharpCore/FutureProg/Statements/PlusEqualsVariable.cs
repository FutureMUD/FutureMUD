using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Functions.BuiltIn;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;

namespace MudSharp.FutureProg.Statements;

internal class PlusEqualsVariable : Statement
{
    private static readonly Regex CompileRegex = new(@"^\s*(\w+?)\s*\+\=\s*(.+)\s*$",
        RegexOptions.Multiline | RegexOptions.IgnoreCase);

    protected string NameToSet;
    protected ProgVariableTypes TypeToSet;
    protected IFunction ValueFunction;

    protected PlusEqualsVariable(string nameToSet, ProgVariableTypes typeToSet, IFunction valueFunction)
    {
        NameToSet = nameToSet;
        TypeToSet = typeToSet;
        ValueFunction = valueFunction;
    }

    private static ICompileInfo SetVariableCompile(IEnumerable<string> lines,
        IDictionary<string, ProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
    {
        var match = CompileRegex.Match(lines.First());

        if (!variableSpace.ContainsKey(match.Groups[1].Value.ToLowerInvariant()))
        {
            return
                CompileInfo.GetFactory()
                           .CreateError($"Variable {match.Groups[1].Value} has not been declared.", lineNumber);
        }

        var rhsInfo = FunctionHelper.CompileFunction(match.Groups[2].Value, variableSpace, lineNumber, gameworld);
        if (rhsInfo.IsError)
        {
            return
                CompileInfo.GetFactory()
                           .CreateError($"Error with RHS of set variable statement: {rhsInfo.ErrorMessage}",
                               lineNumber);
        }

        var lhsType = variableSpace[match.Groups[1].Value.ToLowerInvariant()];
        if (!lhsType.In(
            ProgVariableTypes.Number,
            ProgVariableTypes.Text,
            ProgVariableTypes.TimeSpan,
            ProgVariableTypes.DateTime,
            ProgVariableTypes.MudDateTime
            ) &&
            !lhsType.HasFlag(ProgVariableTypes.Collection)
            )
        {
            return CompileInfo.GetFactory().CreateError(
                $"Tried to use += operator on variable of type {lhsType.Describe()}, which is not supported.", lineNumber);
        }

        var function = (IFunction)rhsInfo.CompiledStatement;
        if (lhsType.HasFlag(ProgVariableTypes.Collection))
        {
            if (!(lhsType ^ ProgVariableTypes.Collection).CompatibleWith(function.ReturnType))
            {
                return CompileInfo.GetFactory()
                                  .CreateError("The item is not of the same type as the collection.", lineNumber);
            }
        }
        else if (lhsType.CompatibleWith(ProgVariableTypes.DateTime) || lhsType.CompatibleWith(ProgVariableTypes.MudDateTime))
        {
            if (!function.ReturnType.CompatibleWith(ProgVariableTypes.TimeSpan))
            {
                return CompileInfo.GetFactory().CreateError(
                    $"Tried to add a non-TimeSpan type to a DateTime type variable. Expected TimeSpan and got {function.ReturnType.Describe()}.",
                    lineNumber);
            }
        }
        else if (!lhsType.CompatibleWith(function.ReturnType))
        {
            return CompileInfo.GetFactory().CreateError(
                $"Tried to add incompatible types with += operator. Variable is of type {lhsType.Describe()} and RHS is of type {function.ReturnType.Describe()}.",
                lineNumber);
        }

        return
            CompileInfo.GetFactory()
                       .CreateNew(
                           new PlusEqualsVariable(match.Groups[1].Value.ToLowerInvariant(), lhsType, function),
                           variableSpace, lines.Skip(1), lineNumber, lineNumber);
    }

    private static string ColouriseStatement(string line)
    {
        var match = CompileRegex.Match(line);
        return $"{match.Groups[1].Value} += {FunctionHelper.ColouriseFunction(match.Groups[2].Value)}";
    }

    private static string ColouriseStatementDarkMode(string line)
    {
        var match = CompileRegex.Match(line);
        return $"{match.Groups[1].Value.Colour(Telnet.VariableCyan)} += {FunctionHelper.ColouriseFunction(match.Groups[2].Value, true)}";
    }

    public static void RegisterCompiler()
    {
        FutureProg.RegisterStatementCompiler(
            new Tuple
            <Regex,
                Func
                <IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
                CompileRegex, SetVariableCompile)
        );

        FutureProg.RegisterStatementColouriser(
            new Tuple<Regex, Func<string, string>>(CompileRegex, ColouriseStatement)
        );

        FutureProg.RegisterStatementColouriser(
            new Tuple<Regex, Func<string, string>>(CompileRegex, ColouriseStatementDarkMode), true
        );

        FutureProg.RegisterStatementHelp("+=", @"The += statement is used to assign a value to a variable that has already been declared and add something to it.

This can be used with numbers (numeric addition), text (concatenation), datetime/muddatetime with a timespan value (add), and collections (add item to collection).

The core syntax is as follows:

	name += #J<addition>#0

For example:

	number += #26#0
	date += #j24h#0
	targets += #M@character#0

See also #3=#0, #3-=#0, #3*=#0, #3/=#0, #3%=#0 and #3^=#0 for other ways of assigning values combined with an operation.");
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        var result = ValueFunction.Execute(variables);
        if (result == StatementResult.Error)
        {
            ErrorMessage = ValueFunction.ErrorMessage;
            return StatementResult.Error;
        }

        if (TypeToSet.HasFlag(ProgVariableTypes.Collection))
        {
            var collection = variables.GetVariable(NameToSet);
            ((IList<IProgVariable>)collection.GetObject).Add(ValueFunction.Result);
            return StatementResult.Normal;
        }
        if (TypeToSet.CompatibleWith(ProgVariableTypes.DateTime))
        {
            var current = variables.GetVariable(NameToSet)?.GetObject as DateTime?;
            if (current is null)
            {
                ErrorMessage = "Tried to += a null datetime.";
                return StatementResult.Error;
            }

            var newValue = current + ((TimeSpan?)ValueFunction.Result?.GetObject ?? TimeSpan.Zero);
            variables.SetVariable(NameToSet, new DateTimeVariable(newValue.Value));
            return StatementResult.Normal;
        }

        if (TypeToSet.CompatibleWith(ProgVariableTypes.MudDateTime))
        {
            var current = variables.GetVariable(NameToSet)?.GetObject as MudDateTime;
            if (current is null)
            {
                ErrorMessage = "Tried to += a null muddatetime.";
                return StatementResult.Error;
            }

            var newValue = current + ((TimeSpan?)ValueFunction.Result?.GetObject ?? TimeSpan.Zero);
            variables.SetVariable(NameToSet, newValue);
            return StatementResult.Normal;
        }

        else
        {
            var current = variables.GetVariable(NameToSet);
            if (TypeToSet.CompatibleWith(ProgVariableTypes.Number))
            {
                var number = (decimal?)current?.GetObject;
                if (number is null)
                {
                    ErrorMessage = "Tried to += a null number.";
                    return StatementResult.Error;
                }
                var newValue = number + (decimal?)ValueFunction.Result?.GetObject ?? 0.0M;
                variables.SetVariable(NameToSet, new NumberVariable(newValue));
                return StatementResult.Normal;
            }

            if (TypeToSet.CompatibleWith(ProgVariableTypes.Text))
            {
                var text = current?.GetObject?.ToString();
                if (text is null)
                {
                    ErrorMessage = "Tried to += a null text.";
                    return StatementResult.Error;
                }
                var newValue = text + (string)ValueFunction.Result?.GetObject ?? "";
                variables.SetVariable(NameToSet, new TextVariable(newValue));
                return StatementResult.Normal;
            }

            throw new ApplicationException("Unknown += type in Prog");
        }
    }
}
