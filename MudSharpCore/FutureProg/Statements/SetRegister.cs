using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;

namespace MudSharp.FutureProg.Statements;

internal class SetRegister : Statement
{
	private static readonly Regex SetRegisterCompileRegex = new(@"^\s*setregister (.+)\s*$",
		RegexOptions.Multiline | RegexOptions.IgnoreCase);

	protected IFuturemud Gameworld;

	protected string NameToSet;
	protected IFunction TypeFunction;
	protected IFunction ValueFunction;

	public SetRegister(string nameToSet, IFunction typeFunction, IFunction valueFunction, IFuturemud gameworld)
	{
		NameToSet = nameToSet;
		TypeFunction = typeFunction;
		ValueFunction = valueFunction;
		Gameworld = gameworld;
	}

	private static ICompileInfo SetRegisterCompile(IEnumerable<string> lines,
		IDictionary<string, FutureProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
	{
		var match = SetRegisterCompileRegex.Match(lines.First());
		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[1].Value, ' ');
		if (splitArgs.IsError)
		{
			return CompileInfo.GetFactory()
			                  .CreateError("Error with arguments of SetRegister statement.", lineNumber);
		}

		var compiledArgs =
			splitArgs.ParameterStrings.Select(
				x => FunctionHelper.CompileFunction(x, variableSpace, lineNumber, gameworld));
		if (compiledArgs.Any(x => x.IsError))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           $"Compile error with SetRegister statement arguments: {compiledArgs.First(x => x.IsError).ErrorMessage}", lineNumber);
		}

		if (compiledArgs.Count() != 3)
		{
			return CompileInfo.GetFactory()
			                  .CreateError("SetRegister statement must have exactly 3 arguments.", lineNumber);
		}

		var typeParameter = (IFunction)compiledArgs.ElementAt(0).CompiledStatement;
		if (!typeParameter.ReturnType.CompatibleWith(FutureProgVariableTypes.ReferenceType))
		{
			return
				CompileInfo.GetFactory()
				           .CreateError("The first argument of SetRegister must be a reference type.", lineNumber);
		}

		var nameParameter = (IFunction)compiledArgs.ElementAt(1).CompiledStatement;
		if (!nameParameter.ReturnType.CompatibleWith(FutureProgVariableTypes.Text | FutureProgVariableTypes.Literal))
		{
			return CompileInfo.GetFactory()
			                  .CreateError("The second argument of SetRegister must be a text literal.", lineNumber);
		}

		var registeredType = gameworld.VariableRegister.GetType(typeParameter.ReturnType,
			(string)nameParameter.Result.GetObject);
		if (registeredType == FutureProgVariableTypes.Error)
		{
			return
				CompileInfo.GetFactory()
				           .CreateError(
					           "That variable type does not have a variable with that name registered in SetRegister.",
					           lineNumber);
		}

		var valueParameter = (IFunction)compiledArgs.ElementAt(2).CompiledStatement;
		if (!valueParameter.ReturnType.CompatibleWith(registeredType))
		{
			return CompileInfo.GetFactory().CreateError(
				$"The type {valueParameter.ReturnType} is not compatible with the type {registeredType} in SetRegister.",
				lineNumber);
		}

		return
			CompileInfo.GetFactory()
			           .CreateNew(
				           new SetRegister((string)nameParameter.Result.GetObject, typeParameter, valueParameter,
					           gameworld), variableSpace, lines.Skip(1), lineNumber, lineNumber);
	}

	private static string ColouriseStatement(string line)
	{
		var match = SetRegisterCompileRegex.Match(line);
		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[1].Value, ' ');
		return
			$"{"setregister".Colour(Telnet.Cyan, Telnet.Black)} {(splitArgs.IsError ? match.Groups[1].Value.ColourBold(Telnet.Magenta, Telnet.Black) : splitArgs.ParameterStrings.Select(x => FunctionHelper.ColouriseFunction(x)).ListToString(separator: " ", conjunction: ""))}";
	}

	private static string ColouriseStatementDarkMode(string line)
	{
		var match = SetRegisterCompileRegex.Match(line);
		var splitArgs = FunctionHelper.ParameterStringSplit(match.Groups[1].Value, ' ');
		return
			$"{"setregister".Colour(Telnet.KeywordPink)} {(splitArgs.IsError ? match.Groups[1].Value.ColourBold(Telnet.Magenta) : splitArgs.ParameterStrings.Select(x => FunctionHelper.ColouriseFunction(x, true)).ListToString(separator: " ", conjunction: ""))}";
	}

	public static void RegisterCompiler()
	{
		FutureProg.RegisterStatementCompiler(
			new Tuple
			<Regex,
				Func
				<IEnumerable<string>, IDictionary<string, FutureProgVariableTypes>, int, IFuturemud, ICompileInfo>>(
				SetRegisterCompileRegex, SetRegisterCompile)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(SetRegisterCompileRegex, ColouriseStatement)
		);

		FutureProg.RegisterStatementColouriser(
			new Tuple<Regex, Func<string, string>>(SetRegisterCompileRegex, ColouriseStatementDarkMode), true
		);

		FutureProg.RegisterStatementHelp("setregister",
			@"The SETREGISTER statement is used to set values in the variable register. See REGISTER HELP for more information on the variable register.

The syntax is as follows:

	#Osetregister#0 #Mtarget#0 #Nvariablename#0 #Mvalue#0 - sets the nominated register variable on the target");
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (TypeFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = $"Type function in SetRegister statement returned an error: {TypeFunction.ErrorMessage}";
			return StatementResult.Error;
		}

		if (ValueFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = $"Value function in SetRegister statement returned an error: {ValueFunction.ErrorMessage}";
			return StatementResult.Error;
		}

		if (TypeFunction.Result?.GetObject == null)
		{
			ErrorMessage = "Type function cannot return null";
			return StatementResult.Error;
		}

		if (Gameworld.VariableRegister.SetValue(TypeFunction.Result, NameToSet, ValueFunction.Result))
		{
			return StatementResult.Normal;
		}

		ErrorMessage = "Could not set value in variable register";
		return StatementResult.Error;
	}
}