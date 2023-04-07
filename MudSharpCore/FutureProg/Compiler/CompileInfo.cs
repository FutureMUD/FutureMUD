using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Compiler;

/// <summary>
///     This class contains information about a compiled block
/// </summary>
internal class CompileInfo : ICompileInfo
{
	private static readonly ICompileInfoFactory _factory = new CompileInfoFactory();


	public CompileInfo(IStatement compiledStatement,
		IDictionary<string, FutureProgVariableTypes> variableSpace = null, IEnumerable<string> remainingLines = null,
		string errorMessage = "", int beginningLineNumber = 0, int endingLineNumber = 0, int errorLineNumber = 0)
	{
		IsError = !string.IsNullOrEmpty(errorMessage);
		ErrorMessage = errorMessage;
		CompiledStatement = compiledStatement;
		RemainingLines = remainingLines;
		VariableSpace = variableSpace;
		BeginningLineNumber = beginningLineNumber;
		EndingLineNumber = endingLineNumber;
		ErrorLineNumber = errorLineNumber;
	}

	public static ICompileInfoFactory GetFactory()
	{
		return _factory;
	}

	public interface ICompileInfoFactory
	{
		ICompileInfo CreateNew(IStatement statement, IDictionary<string, FutureProgVariableTypes> variableSpace,
			IEnumerable<string> remainingLines, int startLineNumber, int endLineNumber);

		ICompileInfo CreateNew(IFunction function, int lineNumber);
		ICompileInfo CreateError(string errorMessage, int errorLineNumber);

		ICompileInfo CreateComment(IDictionary<string, FutureProgVariableTypes> variableSpace,
			IEnumerable<string> remainingLines, int lineNumber);
	}

	public class CompileInfoFactory : ICompileInfoFactory
	{
		internal CompileInfoFactory()
		{
		}

		public ICompileInfo CreateNew(IStatement statement,
			IDictionary<string, FutureProgVariableTypes> variableSpace, IEnumerable<string> remainingLines,
			int startLineNumber, int endLineNumber)
		{
			return new CompileInfo(statement, variableSpace, remainingLines, beginningLineNumber: startLineNumber,
				endingLineNumber: endLineNumber);
		}

		public ICompileInfo CreateNew(IFunction function, int lineNumber)
		{
			return new CompileInfo(function, null, Enumerable.Empty<string>(), beginningLineNumber: lineNumber,
				endingLineNumber: lineNumber);
		}

		public ICompileInfo CreateError(string errorMessage, int errorLineNumber)
		{
			return new CompileInfo(null, errorMessage: errorMessage, errorLineNumber: errorLineNumber);
		}

		public ICompileInfo CreateComment(IDictionary<string, FutureProgVariableTypes> variableSpace,
			IEnumerable<string> remainingLines, int lineNumber)
		{
			return new CompileInfo(null, variableSpace, remainingLines, beginningLineNumber: lineNumber,
				endingLineNumber: lineNumber) { IsComment = true };
		}
	}

	#region ICompileInfo Members

	/// <summary>
	///     Whether the compile statement resulted in an error
	/// </summary>
	public bool IsError { get; protected set; }

	public bool IsComment { get; protected set; }

	/// <summary>
	///     The error message associated with any compilation error
	/// </summary>
	public string ErrorMessage { get; protected set; }

	public IStatement CompiledStatement { get; protected set; }

	public IDictionary<string, FutureProgVariableTypes> VariableSpace { get; protected set; }

	public IEnumerable<string> RemainingLines { get; protected set; }

	public int ErrorLineNumber { get; }

	public int BeginningLineNumber { get; }

	public int EndingLineNumber { get; }

	#endregion
}