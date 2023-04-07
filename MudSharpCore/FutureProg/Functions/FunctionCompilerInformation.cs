using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions;

public class FunctionCompilerResult
{
	public FunctionCompilerResult(bool success, string errorMessage, IFunction compiledFunction)
	{
		Success = success;
		ErrorMessage = errorMessage;
		CompiledFunction = compiledFunction;
	}

	public bool Success { get; protected set; }
	public string ErrorMessage { get; protected set; }
	public IFunction CompiledFunction { get; protected set; }
}

public class FunctionCompilerInformation
{
	public Func<IList<IFunction>, IFuturemud, IFunction> CompilerFunction { get; protected set; }

	public FunctionCompilerInformation(string functionName, IEnumerable<FutureProgVariableTypes> parameters,
		Func<IList<IFunction>, IFuturemud, IFunction> compilerFunction,
		IEnumerable<string> parameterNames = null,
		IEnumerable<string> parameterHelp = null,
		string functionHelp = null,
		string category = "Uncategorised",
		FutureProgVariableTypes returnType = FutureProgVariableTypes.Error,
		Func<IEnumerable<FutureProgVariableTypes>, IFuturemud, bool> filterFunction = null)
	{
		FunctionName = functionName;
		Parameters = parameters;
		CompilerFunction = compilerFunction;
		ParameterNames = parameterNames;
		ParameterHelp = parameterHelp;
		FunctionHelp = functionHelp;
		Category = category;
		ReturnType = returnType;
		if (filterFunction == null)
		{
			CompilerFilterFunction = (x, y) => true;
		}
		else
		{
			CompilerFilterFunction = filterFunction;
		}
	}

	public string FunctionName { get; protected set; }
	public IEnumerable<FutureProgVariableTypes> Parameters { get; protected set; }
	public IEnumerable<string> ParameterNames { get; protected set; }
	public IEnumerable<string> ParameterHelp { get; protected set; }
	public string FunctionHelp { get; protected set; }
	public string Category { get; protected set; }
	public FutureProgVariableTypes ReturnType { get; protected set; }

	public string FunctionDisplayForm
	{
		get
		{
			var sb = new StringBuilder();
			if (string.IsNullOrEmpty(FunctionHelp) || ReturnType == FutureProgVariableTypes.Error)
			{
				sb.Append("Unknown ".Colour(Telnet.Cyan));
				sb.Append(FunctionName.ToUpperInvariant().Colour(Telnet.Yellow));
				sb.Append("(");
				for (var i = 0; i < Parameters.Count(); i++)
				{
					if (i > 0)
					{
						sb.Append(", ");
					}

					sb.Append(Parameters.ElementAt(i).Describe().Colour(Telnet.Cyan));
				}

				sb.Append(")");
				return sb.ToString();
			}

			sb.Append(ReturnType.Describe().Colour(Telnet.Cyan));
			sb.Append(" ");
			sb.Append(FunctionName.ToUpperInvariant().Colour(Telnet.Yellow));
			sb.Append("(");
			for (var i = 0; i < Parameters.Count(); i++)
			{
				if (i > 0)
				{
					sb.Append(", ");
				}

				sb.Append(Parameters.ElementAt(i).Describe().Colour(Telnet.Cyan));
				sb.Append(" ");
				sb.Append(ParameterNames.ElementAt(i));
			}

			sb.Append(")");
			return sb.ToString();
		}
	}

	/// <summary>
	///     Used to specify more advanced constraints on arguments
	/// </summary>
	public Func<IEnumerable<FutureProgVariableTypes>, IFuturemud, bool> CompilerFilterFunction { get; protected set; }

	public FunctionCompilerResult Compile(IList<IFunction> parameterFunctions, IFuturemud gameworld)
	{
		if (
			Parameters.SequenceEqual(parameterFunctions.Select(x => x.ReturnType),
				FutureProgVariableComparer.Instance) &&
			CompilerFilterFunction(parameterFunctions.Select(x => x.ReturnType), gameworld))
		{
			return new FunctionCompilerResult(true, "", CompilerFunction(parameterFunctions, gameworld));
		}

		return new FunctionCompilerResult(false,
			"Function Parameters do not match in " + FunctionName + " function.", null);
	}
}

public class CollectionExtensionFunctionCompilerInformation
{
	protected Func<string, IFunction, IFunction, IFunction> CompilerFunction { get; init; }
	public string FunctionName { get; init; }
	public FutureProgVariableTypes InnerFunctionReturnType { get; init; }
	public string FunctionReturnInfo { get; init; }
	public string FunctionHelp { get; init; }

	public CollectionExtensionFunctionCompilerInformation(string functionName,
		FutureProgVariableTypes innerFunctionReturnType,
		Func<string, IFunction, IFunction, IFunction> compilerFunction,
		string functionHelp,
		string functionReturnInfo)
	{
		FunctionName = functionName;
		InnerFunctionReturnType = innerFunctionReturnType;
		CompilerFunction = compilerFunction;
		FunctionHelp = functionHelp;
		FunctionReturnInfo = functionReturnInfo;
	}

	public FunctionCompilerResult Compile(string variableName, string functionText,
		IDictionary<string, FutureProgVariableTypes> variableSpace, IFunction collectionFunction, int lineNumber,
		IFuturemud gameworld)
	{
		IDictionary<string, FutureProgVariableTypes> localVariables =
			new Dictionary<string, FutureProgVariableTypes>(variableSpace);
		if (localVariables.ContainsKey(variableName))
		{
			return new FunctionCompilerResult(false,
				"The Collection Item Variable was already declared in the " + FunctionName + " function.", null);
		}

		localVariables.Add(variableName, collectionFunction.ReturnType ^ FutureProgVariableTypes.Collection);

		var innerFunction = FunctionHelper.CompileFunction(functionText, localVariables, lineNumber, gameworld);
		if (innerFunction.IsError)
		{
			return new FunctionCompilerResult(false,
				"The Inner Function of Collection Extension Function " + FunctionName +
				" returned the following error: " + innerFunction.ErrorMessage + ".", null);
		}

		return !((IFunction)innerFunction.CompiledStatement).ReturnType.CompatibleWith(InnerFunctionReturnType)
			? new FunctionCompilerResult(false, "The Inner Function is not of the appropriate return type.", null)
			: new FunctionCompilerResult(true, "",
				CompilerFunction(variableName, collectionFunction, (IFunction)innerFunction.CompiledStatement));
	}
}