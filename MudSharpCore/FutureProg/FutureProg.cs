using MoreLinq.Extensions;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.Commands.Socials;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Compiler;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Statements;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MudSharp.FutureProg;

public class FutureProg : SaveableItem, IFutureProg
{
    protected static List<FunctionCompilerInformation> BuiltInFunctionCompilers =
        new();

    protected static List<Tuple<Regex, Func<string, string>>> StatementColourisers =
        new();

    protected static List<Tuple<Regex, Func<string, string>>> StatementColourisersDarkMode =
        new();

    protected static
        List
        <
            Tuple
            <Regex,
                Func
                <IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo>>>
        StatementCompilers =
            new();

    protected static Dictionary<string, (string HelpText, string Related)> StatementHelps = new(StringComparer.InvariantCultureIgnoreCase);

    public static IReadOnlyDictionary<string, (string HelpText, string Related)> StatementHelpTexts => StatementHelps;

    private static readonly Regex _depthDecreasingStatementsRegex =
        new(@"^\s*(end if|end while|end for|end foreach)", RegexOptions.IgnoreCase);

    private static readonly Regex _doubleDepthDecreasingStatementsRegex = new(@"^\s*end switch",
        RegexOptions.IgnoreCase);

    private static readonly Regex _depthIncreasingStatementsRegex = new(@"^\s*(for|while|if|foreach)( |$)",
        RegexOptions.IgnoreCase);

    private static readonly Regex _doubleDepthIncreasingStatementsRegex = new(@"^\s*switch",
        RegexOptions.IgnoreCase);

    private static readonly Regex _unendedDepthIncreasingStatementsRegex = new(
        @"^\s*(case|default|else|elseif)( |$)", RegexOptions.IgnoreCase);

    private static readonly Regex _commentRegex = new(@"^\s*(?:--|//|').*$", RegexOptions.IgnoreCase);

    private readonly List<IStatement> _statements = new();

    private string _functionText;

    public FutureProg(IFuturemud gameworld, string functionName, ProgVariableTypes returnType,
        IEnumerable<Tuple<ProgVariableTypes, string>> parameters, string text)
    {
        Gameworld = gameworld;
        _noSave = true;
        FunctionName = functionName;
        _name = functionName;
        ReturnType = returnType;
        NamedParameters = parameters.ToList();
        FunctionText = text;
        ColouriseFunctionText();
    }

    internal FutureProg(MudSharp.Models.FutureProg prog, IFuturemud gameworld)
    {
        _noSave = true;
        _id = prog.Id;
        _name = prog.FunctionName;
        Gameworld = gameworld;
        FunctionName = prog.FunctionName;
        FunctionComment = prog.FunctionComment;
        FunctionText = prog.FunctionText;
        CompileError = string.Empty;
        Category = prog.Category;
        Subcategory = prog.Subcategory;
        ReturnType = ProgVariableTypes.FromStorageString(prog.ReturnTypeDefinition);
        NamedParameters =
            prog.FutureProgsParameters.OrderBy(x => x.ParameterIndex)
                .Select(x => Tuple.Create(ProgVariableTypes.FromStorageString(x.ParameterTypeDefinition), x.ParameterName))
                .ToList();
        Public = prog.Public;
        ColouriseFunctionText();
        AcceptsAnyParameters = prog.AcceptsAnyParameters;
        StaticType = (FutureProgStaticType)prog.StaticType;
        _noSave = false;
    }

    public override string FrameworkItemType => "FutureProg";

    public TimeSpan CompileTime { get; protected set; }

    public string ColourisedFunctionText { get; protected set; }

    public string CompileError { get; protected set; }

    public string FunctionComment { get; set; }

    private string _functionName;

    public string FunctionName
    {
        get => _functionName;
        set
        {
            _functionName = value;
            _name = value;
        }
    }

    public string Category { get; set; }

    public string Subcategory { get; set; }

    public string FunctionText
    {
        get => _functionText;
        set
        {
            _functionText = value;
            ColouriseFunctionText();
            if (!_noSave)
            {
                Changed = true;
            }
        }
    }

    public bool Public { get; set; }

    public List<Tuple<ProgVariableTypes, string>> NamedParameters { get; }

    public IEnumerable<ProgVariableTypes> Parameters => NamedParameters.Select(x => x.Item1);

    public ProgVariableTypes ReturnType { get; set; }

    public bool AcceptsAnyParameters { get; set; }

    public FutureProgStaticType StaticType
    {
        get => _staticType;
        set
        {
            _staticType = value;
            Changed = true;
            _staticReturnValue = null;
            _staticValueSet = false;
        }
    }

    public bool MatchesParameters(IEnumerable<ProgVariableTypes> parameters)
    {
        if (AcceptsAnyParameters)
        {
            return true;
        }

        if (NamedParameters.Count != parameters.Count())
        {
            return false;
        }

        for (int i = 0; i < NamedParameters.Count; i++)
        {
            if (!parameters.ElementAt(i).CompatibleWith(Parameters.ElementAt(i)))
            {
                return false;
            }
        }

        return true;
    }

    public bool Compile()
    {
#if DEBUG
#else
		try
		{
#endif
        CompileError = null;
        _staticReturnValue = null;
        _staticValueSet = false;
        _statements.Clear();

        Stopwatch sw = new();
        sw.Start();
        // Split the raw text into lines
        List<string> lines = FunctionText.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

        if (!lines.Any())
        {
            return true;
        }

        // Create the initial variable space from the parameters
        Dictionary<string, ProgVariableTypes> variableSpace = NamedParameters.ToDictionary(x => x.Item2, x => x.Item1);

        // If the program has a non-void return type, create the return variable
        if (ReturnType != ProgVariableTypes.Void)
        {
            variableSpace.Add("return", ReturnType);
        }

        // Iteratively compile the program one statement at a time
        ICompileInfo compileResult = CompileNextStatement(lines, variableSpace, 1, Gameworld);
        while (true)
        {
            if (compileResult.IsError)
            {
                CompileError = $"Line {compileResult.ErrorLineNumber:N0}: {compileResult.ErrorMessage}";
                _statements.Clear();
                return false;
            }

            // We don't expect to see continue or return statements at top level
            if (!compileResult.IsComment)
            {
                if (compileResult.CompiledStatement.ExpectedResult == StatementResult.Continue ||
                    compileResult.CompiledStatement.ExpectedResult == StatementResult.Break)
                {
                    CompileError =
                        $"Line {compileResult.EndingLineNumber:N0}: Found a {(compileResult.CompiledStatement.ExpectedResult == StatementResult.Break ? "break" : "continue")} statement outside of an appropriate block.";
                    _statements.Clear();
                    return false;
                }

                _statements.Add(compileResult.CompiledStatement);
            }

            if (!compileResult.RemainingLines.Any())
            {
                break;
            }

            compileResult = CompileNextStatement(compileResult.RemainingLines, compileResult.VariableSpace,
                compileResult.EndingLineNumber + 1, Gameworld);
        }

        // Check for a final return statement if there is one
        if (ReturnType != ProgVariableTypes.Void)
        {
            if (!_statements.Any() ||
                !_statements.Last().IsReturnOrContainsReturnOnAllBranches()
            )
            {

                CompileError = "The prog did not end with a return statement, despite having a return type";
                _statements.Clear();
                return false;
            }
        }

        sw.Stop();
        CompileError = string.Empty;
        CompileTime = TimeSpan.FromTicks(sw.ElapsedTicks);
        return true;
#if DEBUG
#else
		}
		catch (Exception e)
		{
			throw new ApplicationException(
				$"Exception thrown while compiling prog {FunctionName} ID {Id} - {e.Message}", e);
		}
#endif
    }

    public string MXPClickableFunctionName()
    {
        return $"{FunctionName}".FluentTagMXP("send", $"href='show futureprog {Id}'");
    }

    public string MXPClickableFunctionNameWithId()
    {
        return $"{FunctionName} (#{Id})".FluentTagMXP("send", $"href='show futureprog {Id}'");
    }

    public T Execute<T>(params object[] variables)
    {
        return (T)Execute(variables);
    }

    public IReadOnlyCollectionDictionary<string, T> ExecuteCollectionDictionary<T>(params object[] variables)
    {
        if (ReturnType.HasFlag(ProgVariableTypes.CollectionDictionary))
        {
            CollectionDictionary<string, IProgVariable> result = Execute(variables) as CollectionDictionary<string, IProgVariable>;
            if (result is null)
            {
                return new CollectionDictionary<string, T>().AsReadOnlyCollectionDictionary();
            }

            CollectionDictionary<string, T> dictionary = new();
            foreach ((string key, List<IProgVariable> value) in result)
            {
                dictionary.AddRange(value.OfType<T>().Select(x => (key, x)));
            }
            return dictionary.AsReadOnlyCollectionDictionary();
        }

        return new CollectionDictionary<string, T>().AsReadOnlyCollectionDictionary();
    }

    public IReadOnlyDictionary<string, T> ExecuteDictionary<T>(params object[] variables)
    {
        if (ReturnType.HasFlag(ProgVariableTypes.Dictionary))
        {
            Dictionary<string, IProgVariable> result = Execute(variables) as Dictionary<string, IProgVariable>;
            if (result is null)
            {
                return new Dictionary<string, T>();
            }

            Dictionary<string, T> dictionary = new();
            foreach ((string key, IProgVariable value) in result)
            {
                if (value is not T tvalue)
                {
                    continue;
                }

                dictionary[key] = tvalue;
            }
            return dictionary;
        }

        return new Dictionary<string, T>();
    }

    public IEnumerable<T> ExecuteCollection<T>(params object[] variables)
    {
        if (ReturnType.HasFlag(ProgVariableTypes.Collection))
        {
            IList result = Execute(variables) as IList;
            if (result is null)
            {
                return Enumerable.Empty<T>();
            }

            return result
                   .OfType<object>()
                   .Select(x => x is IProgVariable pv ? (object)pv.GetObject : x)
                   .OfType<T>();
        }

        if (ReturnType.HasFlag(ProgVariableTypes.CollectionDictionary))
        {
            CollectionDictionary<string, IProgVariable> result = Execute(variables) as CollectionDictionary<string, IProgVariable>;
            if (result is null)
            {
                return Enumerable.Empty<T>();
            }

            return result.SelectMany(x => x.Value)
                         .OfType<object>()
                         .Select(x => x is IProgVariable pv ? (object)pv.GetObject : x)
                         .OfType<T>();
        }

        if (ReturnType.HasFlag(ProgVariableTypes.Dictionary))
        {
            Dictionary<string, IProgVariable> result = Execute(variables) as Dictionary<string, IProgVariable>;
            if (result is null)
            {
                return Enumerable.Empty<T>();
            }

            return result.Values
                         .OfType<object>()
                         .Select(x => x is IProgVariable pv ? (object)pv.GetObject : x)
                         .OfType<T>();
        }

        return Enumerable.Empty<T>();
    }

    public string ExecuteString(params object[] variables)
    {
        object value = Execute(variables);
        if (value is null)
        {
            return string.Empty;
        }

        return value.ToString();
    }

    public double ExecuteDouble(params object[] variables)
    {
        if (ReturnType != ProgVariableTypes.Number)
        {
            return 0.0;
        }

        return (double?)(decimal?)Execute(variables) ?? 0.0;
    }

    public double ExecuteDouble(double defaultIfNull, params object[] variables)
    {
        if (ReturnType != ProgVariableTypes.Number)
        {
            return defaultIfNull;
        }

        return (double?)(decimal?)Execute(variables) ?? defaultIfNull;
    }

    public decimal ExecuteDecimal(params object[] variables)
    {
        if (ReturnType != ProgVariableTypes.Number)
        {
            return 0.0M;
        }

        return (decimal?)Execute(variables) ?? 0.0M;
    }

    public decimal ExecuteDecimal(decimal defaultIfNull, params object[] variables)
    {
        if (ReturnType != ProgVariableTypes.Number)
        {
            return defaultIfNull;
        }

        return (decimal?)Execute(variables) ?? defaultIfNull;
    }

    public int ExecuteInt(params object[] variables)
    {
        if (ReturnType != ProgVariableTypes.Number)
        {
            return 0;
        }

        return (int?)(decimal?)Execute(variables) ?? 0;
    }

    public int ExecuteInt(int defaultIfNull, params object[] variables)
    {
        if (ReturnType != ProgVariableTypes.Number)
        {
            return defaultIfNull;
        }

        return (int?)(decimal?)Execute(variables) ?? defaultIfNull;
    }

    public long ExecuteLong(params object[] variables)
    {
        if (ReturnType != ProgVariableTypes.Number)
        {
            return 0L;
        }

        return (long?)(decimal?)Execute(variables) ?? 0L;
    }

    public long ExecuteLong(long defaultIfNull, params object[] variables)
    {
        if (ReturnType != ProgVariableTypes.Number)
        {
            return defaultIfNull;
        }

        return (long?)(decimal?)Execute(variables) ?? defaultIfNull;
    }

    public bool ExecuteBool(params object[] variables)
    {
        if (ReturnType != ProgVariableTypes.Boolean)
        {
            return false;
        }

        return (bool?)Execute(variables) ?? false;
    }
    public bool ExecuteBool(bool defaultIfNull, params object[] variables)
    {
        if (ReturnType != ProgVariableTypes.Boolean)
        {
            return defaultIfNull;
        }

        return (bool?)Execute(variables) ?? defaultIfNull;
    }

    private object _staticReturnValue;
    private bool _staticValueSet;

    private static int RecursionDepth { get; set; }

    public object ExecuteWithRecursionProtection(params object[] variables)
    {
        if (StaticType == FutureProgStaticType.FullyStatic && _staticValueSet)
        {
            return _staticReturnValue;
        }

        Dictionary<string, IProgVariable> variableSpaceDict = new();
        for (int i = 0; i < NamedParameters.Count; i++)
        {
            try
            {
                variableSpaceDict.Add(NamedParameters[i].Item2, GetVariable(NamedParameters[i].Item1, variables.ElementAtOrDefault(i)));
            }
            catch (Exception e)
            {
                Gameworld.DiscordConnection.NotifyProgError(Id, FunctionName, $"There was an exception while assigning parameter #{i} ({NamedParameters[i].Item2}) in prog {Id} ({FunctionName}).\nParameters:\n{NamedParameters.Select(x => $"{x.Item2}: {variables.ElementAtOrDefault(NamedParameters.IndexOf(x))?.ToString() ?? "null"}").ArrangeStringsOntoLines(1, 120)}\n\nException:\n\n{e}");
                variableSpaceDict.Add(NamedParameters[i].Item2, new NullVariable(NamedParameters[i].Item1));
            }

        }

        VariableSpace variableSpace = new(variableSpaceDict);
        if (ReturnType != ProgVariableTypes.Void)
        {
            variableSpace.SetVariable("return", new NullVariable(ReturnType));
        }

        if (RecursionDepth++ > 250)
        {
            Gameworld.DiscordConnection.NotifyProgError(Id, FunctionName, $"There was a termination due to excessive recursion in prog {Id} ({FunctionName}).\nParameters:\n{NamedParameters.Select(x => $"{x.Item2}: {variables.ElementAtOrDefault(NamedParameters.IndexOf(x))?.ToString() ?? "null"}").ArrangeStringsOntoLines(1, 120)}");
            return null;
        }

        return InternalExecute(variables, variableSpace);
    }

    public object Execute(params object[] variables)
    {
        if (StaticType == FutureProgStaticType.FullyStatic && _staticValueSet)
        {
            return _staticReturnValue;
        }

        Dictionary<string, IProgVariable> variableSpaceDict = new();
        for (int i = 0; i < NamedParameters.Count; i++)
        {
            try
            {
                variableSpaceDict.Add(NamedParameters[i].Item2, GetVariable(NamedParameters[i].Item1, variables.ElementAtOrDefault(i)));
            }
            catch (Exception e)
            {
                Gameworld.DiscordConnection.NotifyProgError(Id, FunctionName, $"There was an exception while assigning parameter #{i} ({NamedParameters[i].Item2}) in prog {Id} ({FunctionName}).\nParameters:\n{NamedParameters.Select(x => $"{x.Item2}: {variables.ElementAtOrDefault(NamedParameters.IndexOf(x))?.ToString() ?? "null"}").ArrangeStringsOntoLines(1, 120)}\n\nException:\n\n{e}");
                variableSpaceDict.Add(NamedParameters[i].Item2, new NullVariable(NamedParameters[i].Item1));
            }

        }

        VariableSpace variableSpace = new(variableSpaceDict);
        if (ReturnType != ProgVariableTypes.Void)
        {
            variableSpace.SetVariable("return", new NullVariable(ReturnType));
        }

        RecursionDepth = 0;

        return InternalExecute(variables, variableSpace);
    }

    private object InternalExecute(object[] variables, VariableSpace variableSpace)
    {
#if DEBUG
#else
		try
		{
#endif
        foreach (IStatement statement in _statements)
        {
            StatementResult result = statement.Execute(variableSpace);
            switch (result)
            {
                case StatementResult.Return:
                    return ReturnType != ProgVariableTypes.Void
                        ? variableSpace.GetVariable("return")?.GetObject
                        : null;
                case StatementResult.Error:
                    Gameworld.DiscordConnection.NotifyProgError(Id, FunctionName, $"There was a prog error in prog {Id} ({FunctionName}).\nParameters:\n{NamedParameters.Select(x => $"{x.Item2}: {variables.ElementAtOrDefault(NamedParameters.IndexOf(x))?.ToString() ?? "null"}").ArrangeStringsOntoLines(1, 120)}\n\nError:\n\n{statement.ErrorMessage}");
                    return null;
            }
        }
#if DEBUG
#else
		}
		catch (Exception e)
		{
			Gameworld.DiscordConnection.NotifyProgError(Id, FunctionName, $"There was an unhandled exception in prog {Id} ({FunctionName}).\nParameters:\n{NamedParameters.Select(x => $"{x.Item2}: {variables.ElementAtOrDefault(NamedParameters.IndexOf(x))?.ToString() ?? "null"}").ArrangeStringsOntoLines(1, 120)}\n\nException:\n\n{e.ToString()}");
			return null;
		}
#endif

        if (StaticType == FutureProgStaticType.FullyStatic)
        {
            _staticReturnValue = variableSpace.GetVariable("return")?.GetObject;
            _staticValueSet = true;
            return _staticReturnValue;
        }

        return ReturnType != ProgVariableTypes.Void ? variableSpace.GetVariable("return")?.GetObject : null;
    }

    public override void Save()
    {
        Models.FutureProg dbitem = FMDB.Context.FutureProgs.Find(Id);
        dbitem.FunctionName = FunctionName;
        dbitem.FunctionComment = FunctionComment;
        dbitem.FunctionText = FunctionText;
        dbitem.Public = Public;
        dbitem.ReturnTypeDefinition = ReturnType.ToStorageString();
        dbitem.Category = Category;
        dbitem.Subcategory = Subcategory;
        dbitem.AcceptsAnyParameters = AcceptsAnyParameters;
        dbitem.StaticType = (int)StaticType;

        FMDB.Context.FutureProgsParameters.RemoveRange(dbitem.FutureProgsParameters);
        int index = 0;
        foreach (Tuple<ProgVariableTypes, string> item in NamedParameters)
        {
            Models.FutureProgsParameter dbparam = new();
            FMDB.Context.FutureProgsParameters.Add(dbparam);
            dbparam.FutureProg = dbitem;
            dbparam.ParameterIndex = index++;
            dbparam.ParameterName = item.Item2;
            dbparam.ParameterTypeDefinition = item.Item1.ToStorageString();
        }

        FMDB.Context.SaveChanges();
        Changed = false;
    }

    public static ProgVariableTypes GetTypeByName(string name)
    {
        return ProgVariableTypes.TryParse(name, out ProgVariableTypes returnType) ? returnType : ProgVariableTypes.Error;
    }

    public static ICompileInfo CompileNextStatement(IEnumerable<string> lines,
        IDictionary<string, ProgVariableTypes> variableSpace, int lineNumber, IFuturemud gameworld)
    {
        string line = lines.First().Trim();
        if (string.IsNullOrWhiteSpace(line))
        {
            return CompileInfo.GetFactory().CreateComment(variableSpace, lines.Skip(1), lineNumber);
        }

        if (_commentRegex.IsMatch(line))
        {
            return CompileInfo.GetFactory().CreateComment(variableSpace, lines.Skip(1), lineNumber);
        }

        Tuple<Regex, Func<IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo>> compiler = StatementCompilers.FirstOrDefault(x => x.Item1.IsMatch(line));
        if (compiler == null)
        {
            ICompileInfo functionResult = FunctionHelper.CompileFunction(line, variableSpace, lineNumber, gameworld);
            if (functionResult.IsError)
            {
                return CompileInfo.GetFactory().CreateError(
                    $"Syntax error in statement: {functionResult.ErrorMessage}", lineNumber);
            }

            return
                CompileInfo.GetFactory()
                           .CreateNew(new FunctionStatement((IFunction)functionResult.CompiledStatement), variableSpace,
                               lines.Skip(1), lineNumber, lineNumber);
        }

        return compiler.Item2(lines, variableSpace, lineNumber, gameworld);
    }

    public static XElement GetFunctionInfo()
    {
        XElement root = new("Functions");
        foreach (FunctionCompilerInformation fn in BuiltInFunctionCompilers)
        {
            root.Add(new XElement("Function",
                new XElement("Name", fn.FunctionName),
                new XElement("Parameters",
                    from pr in fn.Parameters
                    select new XElement("Parameter", pr.Describe())
                )
            ));
        }

        return root;
    }

    public static IEnumerable<FunctionCompilerInformation> GetFunctionCompilerInformations()
    {
        return BuiltInFunctionCompilers.ToList();
    }

    public static FunctionCompilerResult GetBuiltInFunctionCompiler(string functionName, IList<IFunction> parameters,
        IFuturemud gameworld)
    {
        FunctionCompilerInformation compiler =
            BuiltInFunctionCompilers.FirstOrDefault(
                x =>
                    x.FunctionName == functionName.ToLowerInvariant() &&
                    x.Parameters.SequenceEqual(parameters.Select(y => y.ReturnType),
                        FutureProgVariableComparer.Instance) &&
                    x.CompilerFilterFunction(parameters.Select(y => y.ReturnType), gameworld));
        if (compiler != null)
        {
            return compiler.Compile(parameters, gameworld);
        }

        return new FunctionCompilerResult(false,
            BuiltInFunctionCompilers.Any(x => x.FunctionName.ToLowerInvariant() == functionName)
                ? $"No built in function named \"{functionName}\" with matching parameters {parameters.Select(y => y.ReturnType.Describe()).ListToCommaSeparatedValues()}."
                : $"\"{functionName}\" is not a valid built in function", null);
    }

	public static IProgVariable GetVariable(ProgVariableTypes type, object value)
	{
		if (value == null)
		{
			return new NullVariable(type);
        }

        if (type.HasFlag(ProgVariableTypes.Collection))
        {
            if (value is StringStack stack && type.CompatibleWith(ProgVariableTypes.Text))
            {
                stack.PopSpeechAll();
                return new CollectionVariable(stack.Memory.ToList(), ProgVariableTypes.Text);
            }

            ProgVariableTypes underlyingType = type ^ ProgVariableTypes.Collection;
            List<IProgVariable> list = (from object item in value as IEnumerable ?? Enumerable.Empty<object>()
                                        select GetVariable(underlyingType, item)).ToList();
            return new CollectionVariable(list, underlyingType);
        }

        if (type.HasFlag(ProgVariableTypes.Dictionary))
        {
            ProgVariableTypes underlyingType = type ^ ProgVariableTypes.Dictionary;
            IDictionary idict = value as IDictionary;
            if (idict is null)
            {
                return new DictionaryVariable(new Dictionary<string, IProgVariable>(), underlyingType);
            }

            Dictionary<string, IProgVariable> ndict = new();
            foreach (DictionaryEntry entry in idict)
            {
                if (entry.Key is string keyString)
                {
                    ndict[keyString] = GetVariable(underlyingType, entry.Value);
                }
            }

            return new DictionaryVariable(new Dictionary<string, IProgVariable>(ndict), underlyingType);

        }

		if (type.HasFlag(ProgVariableTypes.CollectionDictionary))
		{
			ProgVariableTypes underlyingType = type ^ ProgVariableTypes.CollectionDictionary;
			if (value is not ICollectionDictionaryWithKey<string> cdString)
			{
                return new CollectionDictionaryVariable(new CollectionDictionary<string, IProgVariable>(), underlyingType);
            }

			IEnumerable<KeyValuePair<string, List<IProgVariable>>> values = cdString.KeysAndValues.Select(x => new KeyValuePair<string, List<IProgVariable>>(x.Key, x.Value.Select(y => GetVariable(underlyingType, y)).ToList()));
			return new CollectionDictionaryVariable(new CollectionDictionary<string, IProgVariable>(values), underlyingType);
		}

		if (type == ProgVariableTypes.LegalClass)
		{
			return value as IProgVariable;
		}

		switch (type.LegacyCode)
		{
            case ProgVariableTypeCode.Text:
                if (value is Enum evalue)
                {
                    return new TextVariable(evalue.DescribeEnum(true));
                }

                if (value is ISocial social)
                {
                    return new TextVariable(social.Name);
                }

                if (value is IFrameworkItem frameworkItem)
                {
                    return new TextVariable(frameworkItem.Name);
                }

                return new TextVariable(value.ToString());

            case ProgVariableTypeCode.Number:
                if (value is Enum)
                {
                    try
                    {
                        return new NumberVariable(Convert.ToInt64(value));
                    }
                    catch
                    {
                        return new NumberVariable(0);
                    }
                }

                return new NumberVariable(Convert.ToDecimal(value));

            case ProgVariableTypeCode.Boolean:
                return new BooleanVariable(Convert.ToBoolean(value));

            case ProgVariableTypeCode.Gender:
                return new GenderVariable((Gender)(value as short? ?? 0));

            case ProgVariableTypeCode.DateTime:
                return new DateTimeVariable((DateTime)value);

            case ProgVariableTypeCode.TimeSpan:
                return new TimeSpanVariable((TimeSpan)value);

            default:
                return value as IProgVariable;
        }
    }

    public static void Initialise()
    {
        InitialiseCompilers();
    }

    public static void RegisterBuiltInFunctionCompiler(FunctionCompilerInformation information)
    {
        BuiltInFunctionCompilers.Add(information);
    }

    public static void RegisterStatementColouriser(Tuple<Regex, Func<string, string>> colouriser, bool darkMode = false)
    {
        if (darkMode)
        {
            StatementColourisersDarkMode.Add(colouriser);
        }
        else
        {
            StatementColourisers.Add(colouriser);
        }
    }

    public static void RegisterStatementCompiler(
        Tuple
            <Regex,
                Func<IEnumerable<string>, IDictionary<string, ProgVariableTypes>, int, IFuturemud, ICompileInfo>>
            compiler)
    {
        StatementCompilers.Add(compiler);
    }

    public static void RegisterStatementHelp(string statement, string helpText, string related = "")
    {
        StatementHelps[statement] = (helpText, related);
    }

    protected static void InitialiseCompilers()
    {
        /*
		 * This section requires classes that derive from Statements to register a compile-time handler for themselves.
		 */
        foreach (
            Type type in Futuremud.GetAllTypes().Where(x => x.IsSubclassOf(typeof(Statement))))
        {
            MethodInfo method = type.GetMethod("RegisterCompiler", BindingFlags.Public | BindingFlags.Static);
            method?.Invoke(null, null);
        }

        // Add in the colouriser for comments ourselves
        RegisterStatementColouriser(
            new Tuple<Regex, Func<string, string>>(_commentRegex, line => line.Colour(Telnet.Green, Telnet.Black))
        );
        RegisterStatementColouriser(
            new Tuple<Regex, Func<string, string>>(_commentRegex, line => line.Colour(Telnet.Green)), true
        );

        /*
		 * This section allows classes that derive from IFutureProgVariable to register a compile-time handler for a FutureProgVariableType
		 *
		 * It is the responsibility of the implementor to ensure that only one class of each FutureProgVariableType implements this function,
		 * and that the function handles it adequately (ideally through interface calls)
		 *
		 * If you add a new type of FutureProgVariableType, you must implement such a handler
		 */
        Type fpType = typeof(IProgVariable);
        foreach (
            Type type in Futuremud.GetAllTypes().Where(x => x.GetInterfaces().Contains(fpType)))
        {
            MethodInfo method = type.GetMethod("RegisterFutureProgCompiler", BindingFlags.Public | BindingFlags.Static);
            method?.Invoke(null, null);
        }


        foreach (
            Type type in
            Futuremud.GetAllTypes().Where(x => x.IsSubclassOf(typeof(BuiltInFunction))))
        {
            MethodInfo compileInfo = type.GetMethod("RegisterFunctionCompiler", BindingFlags.Static | BindingFlags.Public);
            compileInfo?.Invoke(null, null);
        }

        /*
		 *  This section allows classes that derive from CollectionExtensionFunction to register themselves a compiler.
		 */
        foreach (
            Type type in
            Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(x => x.IsSubclassOf(typeof(CollectionExtensionFunction))))
        {
            MethodInfo compileInfo = type.GetMethod("RegisterFunctionCompiler", BindingFlags.Static | BindingFlags.Public);
            compileInfo?.Invoke(null, null);
        }
    }

    protected bool DisplayInDarkMode => Gameworld.GetStaticBool("DisplayProgsInDarkMode");

    public void ColouriseFunctionText()
    {
        int lineNumber = 1;
        int depth = 0;

        if (string.IsNullOrEmpty(FunctionText))
        {
            ColourisedFunctionText = "";
            return;
        }

        List<string> lines =
            FunctionText.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList();
        StringBuilder sb = new();
        if (!DisplayInDarkMode)
        {
            sb.Append(Telnet.Black.Colour);
        }

        foreach (string line in lines)
        {
            if (_unendedDepthIncreasingStatementsRegex.IsMatch(line))
            {
                depth = Math.Max(0, depth - 1);
            }

            Tuple<Regex, Func<string, string>> colouriser = (DisplayInDarkMode ? StatementColourisersDarkMode : StatementColourisers).FirstOrDefault(x => x.Item1.IsMatch(line));
            if (colouriser == null)
            {
                sb.AppendLine(
                    ($"{lineNumber++,3} |" + new string(' ', 2 * depth + 1) + FunctionHelper.ColouriseFunction(line, DisplayInDarkMode))
                    .RawTextPadRight(110)
                );
                continue;
            }

            if (_depthDecreasingStatementsRegex.IsMatch(line))
            {
                depth = Math.Max(0, depth - 1);
            }

            if (_doubleDepthDecreasingStatementsRegex.IsMatch(line))
            {
                depth = Math.Max(0, depth - 2);
            }

            sb.AppendLine(
                ($"{lineNumber++,3} |" + new string(' ', 2 * depth + 1) + colouriser.Item2(line)).RawTextPadRight(110)
            );
            if (_depthIncreasingStatementsRegex.IsMatch(line) ||
                _unendedDepthIncreasingStatementsRegex.IsMatch(line))
            {
                depth++;
            }

            if (_doubleDepthIncreasingStatementsRegex.IsMatch(line))
            {
                depth += 2;
            }
        }

        if (DisplayInDarkMode)
        {
            ColourisedFunctionText = sb.ToString().NoWrap();
        }
        else
        {
            ColourisedFunctionText = sb.ToString().ColourBackground(Telnet.White).NoWrap();
        }

    }

    public static string VariableValueToText(IProgVariable variable, IPerceiver voyeur,
        bool newLineCollectionItems = false)
    {
        if (variable?.GetObject == null)
        {
            return "null";
        }

        if (variable.Type.HasFlag(ProgVariableTypes.Collection))
        {
            StringBuilder sb = new();
            List<IProgVariable> items = ((IList)variable.GetObject).OfType<IProgVariable>().ToList();
            if (!items.Any())
            {
                sb.Append(" {Empty}");
            }
            else
            {
                if (newLineCollectionItems)
                {
                    sb.AppendLine();
                    foreach (IProgVariable ci in items)
                    {
                        sb.AppendLine($"\t{VariableValueToText(ci, voyeur)}");
                    }
                }
                else
                {
                    sb.Append(" {");
                    sb.Append(items.Select(x => VariableValueToText(x, voyeur))
                                   .ListToString(conjunction: "", twoItemJoiner: ", "));
                    sb.Append("}");
                }
            }

            return sb.ToString();
        }

        if (variable.Type.HasFlag(ProgVariableTypes.Dictionary))
        {
            StringBuilder sb = new();
            Dictionary<string, IProgVariable> items = (Dictionary<string, IProgVariable>)variable.GetObject;
            if (!items.Any())
            {
                sb.Append(" {Empty}");
            }
            else
            {
                if (newLineCollectionItems)
                {
                    sb.AppendLine();
                    foreach (KeyValuePair<string, IProgVariable> ci in items)
                    {
                        sb.AppendLine($"\t\"{ci.Key}\": {VariableValueToText(ci.Value, voyeur)}");
                    }
                }
                else
                {
                    sb.Append(" {");
                    sb.Append(items.Select(x => $"\"{x.Key}\": {VariableValueToText(x.Value, voyeur)}")
                                   .ListToString(conjunction: "", twoItemJoiner: ", "));
                    sb.Append("}");
                }
            }

            return sb.ToString();
        }

        if (variable.Type.HasFlag(ProgVariableTypes.CollectionDictionary))
        {
            StringBuilder sb = new();
            CollectionDictionary<string, IProgVariable> items = (CollectionDictionary<string, IProgVariable>)variable.GetObject;
            if (!items.Any())
            {
                sb.Append(" {Empty}");
            }
            else
            {
                if (newLineCollectionItems)
                {
                    sb.AppendLine();
                    foreach (KeyValuePair<string, List<IProgVariable>> ci in items)
                    {
                        sb.AppendLine($"\t\"{ci.Key}\": ");
                        foreach (IProgVariable inner in ci.Value)
                        {
                            sb.AppendLine($"\t\t{VariableValueToText(inner, voyeur, newLineCollectionItems)}");
                        }
                    }
                }
                else
                {
                    sb.Append(" {");
                    sb.Append(items
                              .Select(x =>
                                  $"\"{x.Key}\": {{{x.Value.Select(y => VariableValueToText(y, voyeur)).ListToString(conjunction: "", twoItemJoiner: ", ")}}}")
                              .ListToString(conjunction: "", twoItemJoiner: ", "));
                    sb.Append("}");
                }
            }

            return sb.ToString();
        }

		ProgVariableTypes type = variable.Type & ~ProgVariableTypes.Literal;
		IFrameworkItem thing = variable.GetObject as IFrameworkItem;
		if (type == ProgVariableTypes.LegalClass)
		{
			return $"Legal Class #{thing.Id} - {thing.Name}";
		}

		switch (type.LegacyCode)
		{
            case ProgVariableTypeCode.Text:
                return $"\"{variable.GetObject}\"";
            case ProgVariableTypeCode.Number:
                return ((decimal)variable.GetObject).ToString("N", voyeur);
            case ProgVariableTypeCode.Boolean:
                return ((bool)variable.GetObject).ToString(voyeur);
            case ProgVariableTypeCode.Character:
                ICharacter ch = (ICharacter)variable;
                return
                    $"Character #{ch.Id.ToString("N0", voyeur)} ({ch.PersonalName.GetName(NameStyle.FullWithNickname)}) - {ch.HowSeen(voyeur)}";
            case ProgVariableTypeCode.Location:
                ICell cell = (ICell)variable;
                return $"Cell #{cell.Id.ToString("N0", voyeur)}: {cell.CurrentOverlay.CellName}";
            case ProgVariableTypeCode.Item:
                IGameItem item = (IGameItem)variable;
                return
                    $"Item #{item.Id.ToString("N0", voyeur)} Proto {item.Prototype.Id.ToString("N0", voyeur)}r{item.Prototype.RevisionNumber.ToString("N0", voyeur)} - {item.HowSeen(voyeur)}";
            case ProgVariableTypeCode.Gender:
                return Gendering.Get((Gender)variable.GetObject).GenderClass();
            case ProgVariableTypeCode.Shard:
                return $"Shard #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Zone:
                return $"Zone #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Race:
                return $"Race #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Culture:
                return $"Culture #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Chargen:
                Chargen chargen = (Chargen)variable.GetObject;
                return
                    $"Chargen #{chargen.Id} - {chargen.SelectedName?.GetName(NameStyle.FullWithNickname) ?? "Unnamed"}";
            case ProgVariableTypeCode.Trait:
                return $"Trait #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Clan:
                return $"Clan #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.ClanRank:
                return $"Clan Rank #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.ClanAppointment:
                return $"Clan Appointment #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.ClanPaygrade:
                return $"Clan Paygrade #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Currency:
                return $"Currency #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Exit:
                ICellExit exit = (ICellExit)variable.GetObject;
                return
                    $"Exit #{exit.Exit.Id} - {exit.OutboundMovementSuffix} from {exit.Origin.CurrentOverlay.CellName} to {exit.Destination.CurrentOverlay.CellName}";
            case ProgVariableTypeCode.DateTime:
                return ((DateTime)variable.GetObject).GetLocalDateString(voyeur?.Account ?? DummyAccount.Instance);
            case ProgVariableTypeCode.TimeSpan:
                return ((TimeSpan)variable.GetObject).Describe(voyeur);
            case ProgVariableTypeCode.Language:
                return $"Language #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Accent:
                return $"Accent #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Merit:
                return $"Merit #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.MudDateTime:
                MudDateTime mdt = (MudDateTime)variable.GetObject;
                return mdt.GetDateTimeString();
            case ProgVariableTypeCode.Calendar:
                return $"Calendar #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Clock:
                return $"Clock #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Effect:
                IEffectSubtype effect = (IEffectSubtype)variable.GetObject;
                return $"Effect #{effect.Id} - {effect.Describe(voyeur)}";
            case ProgVariableTypeCode.Knowledge:
                return $"Knowledge #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Role:
                return $"Role #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Ethnicity:
                return $"Ethnicity #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Drug:
                return $"Drug #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.WeatherEvent:
                return $"WeatherEvent #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Perceivable:
                return $"Perceivable {thing.Id} type {thing.FrameworkItemType}";
            case ProgVariableTypeCode.Perceiver:
                return $"Perceiver {thing.Id} type {thing.FrameworkItemType}";
            case ProgVariableTypeCode.MagicResourceHaver:
                return $"MagicResourceHaver {thing.Id} type {thing.FrameworkItemType}";
            case ProgVariableTypeCode.Shop:
                return $"Shop {thing.Id} \"{thing.Name}\"";
            case ProgVariableTypeCode.Merchandise:
                return $"Merchandise {thing.Id} \"{thing.Name}\"";
            case ProgVariableTypeCode.Outfit:
                return $"Outfit {thing.Id} \"{thing.Name}\"";
            case ProgVariableTypeCode.LiquidMixture:
                return $"Liquid Mixture {((LiquidMixture)variable.GetObject).ColouredLiquidDescription}";
            case ProgVariableTypeCode.Area:
                return $"Area #{thing.Id} - {thing.Name}";
            case ProgVariableTypeCode.Writing:
                IWriting writing = (IWriting)thing;
                return $"Writing #{thing.Id} - {writing.DescribeInLook(null)}";
            case ProgVariableTypeCode.Script:
                return $"Script #{thing.Id} - {thing.Name}";
        }

        return variable.GetObject.ToString();
    }

    private static IEnumerable<ProgVariableTypes> _allreferenceTypes;
    private FutureProgStaticType _staticType = FutureProgStaticType.NotStatic;

    public static IEnumerable<ProgVariableTypes> AllReferenceTypes
    {
        get
        {
            if (_allreferenceTypes == null)
            {
                _allreferenceTypes = ProgVariableTypes.ReferenceType.GetFlags().Cast<ProgVariableTypes>()
                                                            .ToList();
            }

            return _allreferenceTypes;
        }
    }
}
