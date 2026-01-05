using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Statements;

internal class ModulusEqualsVariable : Statement
{
    private static readonly Regex CompileRegex = new(@"^\s*(\w+?)\s*%\=\s*(.+)\s*$",
        RegexOptions.Multiline | RegexOptions.IgnoreCase);

    protected string NameToSet;
    protected ProgVariableTypes TypeToSet;
    protected IFunction ValueFunction;

    protected ModulusEqualsVariable(string nameToSet, ProgVariableTypes typeToSet, IFunction valueFunction)
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
                           .CreateError($"Error with RHS of %= statement: {rhsInfo.ErrorMessage}",
                               lineNumber);
        }

        var lhsType = variableSpace[match.Groups[1].Value.ToLowerInvariant()];
        if (!lhsType.CompatibleWith(ProgVariableTypes.Number))
        {
            return CompileInfo.GetFactory().CreateError(
                $"Tried to use %= operator on variable of type {lhsType.Describe()}, which is not supported.", lineNumber);
        }

        var function = (IFunction)rhsInfo.CompiledStatement;
        if (!lhsType.CompatibleWith(function.ReturnType))
        {
            return CompileInfo.GetFactory().CreateError(
                $"Tried to add incompatible types with %= operator. Variable is of type {lhsType.Describe()} and RHS is of type {function.ReturnType.Describe()}.",
                lineNumber);
        }

        return
            CompileInfo.GetFactory()
                       .CreateNew(
                           new ModulusEqualsVariable(match.Groups[1].Value.ToLowerInvariant(), function.ReturnType, function),
                           variableSpace, lines.Skip(1), lineNumber, lineNumber);
    }

    private static string ColouriseStatement(string line)
    {
        var match = CompileRegex.Match(line);
        return $"{match.Groups[1].Value} %= {FunctionHelper.ColouriseFunction(match.Groups[2].Value)}";
    }

    private static string ColouriseStatementDarkMode(string line)
    {
        var match = CompileRegex.Match(line);
        return $"{match.Groups[1].Value.Colour(Telnet.VariableCyan)} %= {FunctionHelper.ColouriseFunction(match.Groups[2].Value, true)}";
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

        FutureProg.RegisterStatementHelp("%=", @"The %= statement is used to assign a value to a number variable that has already been declared and also get the modulus of it.

This can only be used with numbers.

The core syntax is as follows:

	name %= #J<divisor>#0

For example:

	number %= #212#0

See also #3=#0, #3+=#0, #3-=#0, #3*=#0, #3/=#0 and #3^=#0 for other ways of assigning values combined with an operation.");
    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        var result = ValueFunction.Execute(variables);
        if (result == StatementResult.Error)
        {
            ErrorMessage = ValueFunction.ErrorMessage;
            return StatementResult.Error;
        }

        var current = variables.GetVariable(NameToSet);
        var number = (decimal?)current?.GetObject;
        if (number is null)
        {
            ErrorMessage = "Tried to %= a null number.";
            return StatementResult.Error;
        }
        var newValue = number % (decimal?)ValueFunction.Result?.GetObject ?? 0.0M;
        variables.SetVariable(NameToSet, new NumberVariable(newValue));
        return StatementResult.Normal;
    }
}
