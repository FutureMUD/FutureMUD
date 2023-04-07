using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions;

internal abstract class CollectionExtensionFunction : Function
{
	private static readonly List<CollectionExtensionFunctionCompilerInformation> _functionCompilers =
		new();

	protected IFunction CollectionFunction;
	protected IFunction CollectionItemFunction;
	protected string VariableName;

	protected CollectionExtensionFunction(string variableName, IFunction collectionItemFunction,
		IFunction collectionFunction)
	{
		VariableName = variableName;
		CollectionItemFunction = collectionItemFunction;
		CollectionFunction = collectionFunction;
	}

	protected static void RegisterCollectionExtensionFunctionCompiler(
		CollectionExtensionFunctionCompilerInformation compiler)
	{
		_functionCompilers.Add(compiler);
	}

	public static FunctionCompilerResult GetCollectionExtensionFunctionCompiler(string functionName,
		string variableName, string functionText, IDictionary<string, FutureProgVariableTypes> variableSpace,
		IFunction collectionFunction, int lineNumber, IFuturemud gameworld)
	{
		var compiler = _functionCompilers.FirstOrDefault(x => x.FunctionName == functionName.ToLowerInvariant());
		return compiler != null
			? compiler.Compile(variableName, functionText, variableSpace, collectionFunction, lineNumber, gameworld)
			: new FunctionCompilerResult(false, "There is no such collection extension function", null);
	}

	public static IEnumerable<CollectionExtensionFunctionCompilerInformation> FunctionCompilerInformations =>
		_functionCompilers;
}