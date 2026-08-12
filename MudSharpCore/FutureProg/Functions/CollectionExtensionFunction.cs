
namespace MudSharp.FutureProg.Functions;

internal abstract class CollectionExtensionFunction : Function
{
    private static readonly List<CollectionExtensionFunctionCompilerInformation> _functionCompilers =
        new();
	private static readonly Dictionary<string, CollectionExtensionFunctionCompilerInformation> _functionCompilersByName =
		new(StringComparer.OrdinalIgnoreCase);

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
		_functionCompilersByName[compiler.FunctionName] = compiler;
    }

    public static FunctionCompilerResult GetCollectionExtensionFunctionCompiler(string functionName,
        string variableName, string functionText, IDictionary<string, ProgVariableTypes> variableSpace,
        IFunction collectionFunction, int lineNumber, IFuturemud gameworld)
    {
		return _functionCompilersByName.TryGetValue(functionName, out var compiler)
            ? compiler.Compile(variableName, functionText, variableSpace, collectionFunction, lineNumber, gameworld)
            : new FunctionCompilerResult(false, "There is no such collection extension function", null);
    }

    public static IEnumerable<CollectionExtensionFunctionCompilerInformation> FunctionCompilerInformations =>
        _functionCompilers;
}
