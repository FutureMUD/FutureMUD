using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Dictionaries;

internal class DictionaryUtilityFunction : BuiltInFunction
{
	private readonly DictionaryOperation _operation;
	private readonly ProgVariableTypes _metadataReturnType;

	private enum DictionaryOperation
	{
		DictionaryCount,
		DictionaryAny,
		DictionaryEmpty,
		DictionaryKeys,
		DictionaryValues,
		DictionaryContainsKey,
		DictionaryGet,
		DictionaryGetDefault,
		DictionaryContainsValue,
		DictionaryWithoutKey,
		DictionarySet,
		DictionaryFirstKey,
		CollectionDictionaryCount,
		CollectionDictionaryLongCount,
		CollectionDictionaryAny,
		CollectionDictionaryEmpty,
		CollectionDictionaryKeys,
		CollectionDictionaryValues,
		CollectionDictionaryContainsKey,
		CollectionDictionaryGet,
		CollectionDictionaryGetFirst,
		CollectionDictionaryContainsValue,
		CollectionDictionaryWithoutKey
	}

	private DictionaryUtilityFunction(IList<IFunction> parameters, DictionaryOperation operation,
		ProgVariableTypes metadataReturnType) : base(parameters)
	{
		_operation = operation;
		_metadataReturnType = metadataReturnType;
	}

	private ProgVariableTypes ElementType
	{
		get
		{
			return ParameterFunctions[0].ReturnType.HasFlag(ProgVariableTypes.CollectionDictionary)
				? ParameterFunctions[0].ReturnType ^ ProgVariableTypes.CollectionDictionary
				: ParameterFunctions[0].ReturnType ^ ProgVariableTypes.Dictionary;
		}
	}

	public override ProgVariableTypes ReturnType
	{
		get
		{
			return _operation switch
			{
				DictionaryOperation.DictionaryGet or DictionaryOperation.DictionaryGetDefault
					or DictionaryOperation.CollectionDictionaryGetFirst => ElementType,
				DictionaryOperation.DictionaryValues or DictionaryOperation.CollectionDictionaryValues
					or DictionaryOperation.CollectionDictionaryGet => ProgVariableTypes.Collection | ElementType,
				DictionaryOperation.DictionaryWithoutKey or DictionaryOperation.DictionarySet
					or DictionaryOperation.CollectionDictionaryWithoutKey => ParameterFunctions[0].ReturnType,
				_ => _metadataReturnType
			};
		}
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		switch (_operation)
		{
			case DictionaryOperation.DictionaryCount:
				Result = new NumberVariable(GetDictionary().Count);
				return StatementResult.Normal;
			case DictionaryOperation.DictionaryAny:
				Result = new BooleanVariable(GetDictionary().Count > 0);
				return StatementResult.Normal;
			case DictionaryOperation.DictionaryEmpty:
				Result = new BooleanVariable(GetDictionary().Count == 0);
				return StatementResult.Normal;
			case DictionaryOperation.DictionaryKeys:
				Result = new CollectionVariable(GetDictionary().Keys.Select(x => new TextVariable(x)).ToList<IProgVariable>(), ProgVariableTypes.Text);
				return StatementResult.Normal;
			case DictionaryOperation.DictionaryValues:
				Result = new CollectionVariable(GetDictionary().Values.ToList(), ElementType);
				return StatementResult.Normal;
			case DictionaryOperation.DictionaryContainsKey:
				Result = new BooleanVariable(GetDictionary().ContainsKey(GetText(1)));
				return StatementResult.Normal;
			case DictionaryOperation.DictionaryGet:
				Result = GetDictionary().TryGetValue(GetText(1), out var value) ? value : new NullVariable(ElementType);
				return StatementResult.Normal;
			case DictionaryOperation.DictionaryGetDefault:
				Result = GetDictionary().TryGetValue(GetText(1), out var defaultedValue)
					? defaultedValue
					: ParameterFunctions[2].Result ?? new NullVariable(ElementType);
				return StatementResult.Normal;
			case DictionaryOperation.DictionaryContainsValue:
				Result = new BooleanVariable(GetDictionary().Values.Any(x => ValuesEqual(x, ParameterFunctions[1].Result)));
				return StatementResult.Normal;
			case DictionaryOperation.DictionaryWithoutKey:
				Result = new DictionaryVariable(
					GetDictionary()
						.Where(x => !string.Equals(x.Key, GetText(1), StringComparison.InvariantCulture))
						.ToDictionary(x => x.Key, x => x.Value),
					ElementType
				);
				return StatementResult.Normal;
			case DictionaryOperation.DictionarySet:
				Result = SetDictionaryValue(GetText(1), ParameterFunctions[2].Result ?? new NullVariable(ElementType));
				return StatementResult.Normal;
			case DictionaryOperation.DictionaryFirstKey:
				Result = new TextVariable(GetDictionary().Keys.FirstOrDefault() ?? string.Empty);
				return StatementResult.Normal;
			case DictionaryOperation.CollectionDictionaryCount:
				Result = new NumberVariable(GetCollectionDictionary().Keys.Count());
				return StatementResult.Normal;
			case DictionaryOperation.CollectionDictionaryLongCount:
				Result = new NumberVariable(GetCollectionDictionary().Sum(x => x.Value.Count));
				return StatementResult.Normal;
			case DictionaryOperation.CollectionDictionaryAny:
				Result = new BooleanVariable(GetCollectionDictionary().Any(x => x.Value.Count > 0));
				return StatementResult.Normal;
			case DictionaryOperation.CollectionDictionaryEmpty:
				Result = new BooleanVariable(GetCollectionDictionary().All(x => x.Value.Count == 0));
				return StatementResult.Normal;
			case DictionaryOperation.CollectionDictionaryKeys:
				Result = new CollectionVariable(GetCollectionDictionary().Keys.Select(x => new TextVariable(x)).ToList<IProgVariable>(), ProgVariableTypes.Text);
				return StatementResult.Normal;
			case DictionaryOperation.CollectionDictionaryValues:
				Result = new CollectionVariable(GetCollectionDictionary().SelectMany(x => x.Value).ToList(), ElementType);
				return StatementResult.Normal;
			case DictionaryOperation.CollectionDictionaryContainsKey:
				Result = new BooleanVariable(GetCollectionDictionary().ContainsKey(GetText(1)));
				return StatementResult.Normal;
			case DictionaryOperation.CollectionDictionaryGet:
				Result = new CollectionVariable(GetCollectionDictionary().ContainsKey(GetText(1)) ? GetCollectionDictionary()[GetText(1)].ToList() : new List<IProgVariable>(), ElementType);
				return StatementResult.Normal;
			case DictionaryOperation.CollectionDictionaryGetFirst:
				Result = GetCollectionDictionary().ContainsKey(GetText(1))
					? GetCollectionDictionary()[GetText(1)].FirstOrDefault() ?? new NullVariable(ElementType)
					: new NullVariable(ElementType);
				return StatementResult.Normal;
			case DictionaryOperation.CollectionDictionaryContainsValue:
				Result = new BooleanVariable(GetCollectionDictionary().SelectMany(x => x.Value).Any(x => ValuesEqual(x, ParameterFunctions[1].Result)));
				return StatementResult.Normal;
			case DictionaryOperation.CollectionDictionaryWithoutKey:
				Result = new CollectionDictionaryVariable(CloneCollectionDictionaryWithoutKey(GetText(1)), ElementType);
				return StatementResult.Normal;
			default:
				throw new NotSupportedException($"Unknown dictionary utility operation {_operation}.");
		}
	}

	private Dictionary<string, IProgVariable> GetDictionary()
	{
		return ParameterFunctions[0].Result?.GetObject as Dictionary<string, IProgVariable> ??
		       new Dictionary<string, IProgVariable>();
	}

	private CollectionDictionary<string, IProgVariable> GetCollectionDictionary()
	{
		return ParameterFunctions[0].Result?.GetObject as CollectionDictionary<string, IProgVariable> ??
		       new CollectionDictionary<string, IProgVariable>();
	}

	private string GetText(int index)
	{
		return ParameterFunctions[index].Result?.GetObject?.ToString() ?? string.Empty;
	}

	private DictionaryVariable SetDictionaryValue(string key, IProgVariable item)
	{
		var clone = GetDictionary().ToDictionary(x => x.Key, x => x.Value);
		clone[key] = item;
		return new DictionaryVariable(clone, ElementType);
	}

	private CollectionDictionary<string, IProgVariable> CloneCollectionDictionaryWithoutKey(string key)
	{
		CollectionDictionary<string, IProgVariable> clone = new();
		foreach (var item in GetCollectionDictionary())
		{
			if (string.Equals(item.Key, key, StringComparison.InvariantCulture))
			{
				continue;
			}

			clone.AddRange(item.Key, item.Value);
		}

		return clone;
	}

	private static bool ValuesEqual(IProgVariable lhs, IProgVariable rhs)
	{
		if (lhs is null || rhs is null)
		{
			return lhs is null && rhs is null;
		}

		if (lhs.GetObject is string lhsText && rhs.GetObject is string rhsText)
		{
			return string.Equals(lhsText, rhsText, StringComparison.InvariantCultureIgnoreCase);
		}

		return Equals(lhs.GetObject, rhs.GetObject);
	}

	private static bool DictionaryValueCompatible(IEnumerable<ProgVariableTypes> types, IFuturemud gameworld)
	{
		var actual = types.ToList();
		var elementType = actual[0] ^ ProgVariableTypes.Dictionary;
		return elementType == ProgVariableTypes.Void || actual[1].CompatibleWith(elementType);
	}

	private static bool DictionaryDefaultCompatible(IEnumerable<ProgVariableTypes> types, IFuturemud gameworld)
	{
		var actual = types.ToList();
		var elementType = actual[0] ^ ProgVariableTypes.Dictionary;
		return elementType == ProgVariableTypes.Void || actual[2].CompatibleWith(elementType);
	}

	private static bool CollectionDictionaryValueCompatible(IEnumerable<ProgVariableTypes> types, IFuturemud gameworld)
	{
		var actual = types.ToList();
		var elementType = actual[0] ^ ProgVariableTypes.CollectionDictionary;
		return elementType == ProgVariableTypes.Void || actual[1].CompatibleWith(elementType);
	}

	private static void RegisterDictionaryFunction(string name, DictionaryOperation operation, ProgVariableTypes returnType,
		IEnumerable<ProgVariableTypes> parameters, IEnumerable<string> parameterNames,
		IEnumerable<string> parameterHelp, string functionHelp,
		Func<IEnumerable<ProgVariableTypes>, IFuturemud, bool> filter = null)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			parameters,
			(pars, gameworld) => new DictionaryUtilityFunction(pars, operation, returnType),
			parameterNames,
			parameterHelp,
			functionHelp,
			"Dictionaries",
			returnType,
			filter
		));
	}

	private static void RegisterUnaryDictionaryFunction(string name, DictionaryOperation operation,
		ProgVariableTypes returnType, string functionHelp)
	{
		RegisterDictionaryFunction(
			name,
			operation,
			returnType,
			new[] { ProgVariableTypes.Dictionary },
			new[] { "dictionary" },
			new[] { "The dictionary to inspect or transform" },
			functionHelp
		);
	}

	private static void RegisterUnaryCollectionDictionaryFunction(string name, DictionaryOperation operation,
		ProgVariableTypes returnType, string functionHelp)
	{
		RegisterDictionaryFunction(
			name,
			operation,
			returnType,
			new[] { ProgVariableTypes.CollectionDictionary },
			new[] { "dictionary" },
			new[] { "The collection dictionary to inspect or transform" },
			functionHelp
		);
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterUnaryDictionaryFunction("dictionarycount", DictionaryOperation.DictionaryCount, ProgVariableTypes.Number,
			"Returns the number of keyed values in the supplied dictionary.");
		RegisterUnaryDictionaryFunction("dictionaryany", DictionaryOperation.DictionaryAny, ProgVariableTypes.Boolean,
			"Returns true if the supplied dictionary has at least one keyed value.");
		RegisterUnaryDictionaryFunction("dictionaryempty", DictionaryOperation.DictionaryEmpty, ProgVariableTypes.Boolean,
			"Returns true if the supplied dictionary has no keyed values.");
		RegisterUnaryDictionaryFunction("dictionarykeys", DictionaryOperation.DictionaryKeys, ProgVariableTypes.Collection | ProgVariableTypes.Text,
			"Returns a text collection containing all keys in the supplied dictionary.");
		RegisterUnaryDictionaryFunction("dictionaryvalues", DictionaryOperation.DictionaryValues, ProgVariableTypes.Collection | ProgVariableTypes.CollectionItem,
			"Returns a collection containing all values in the supplied dictionary.");
		RegisterUnaryDictionaryFunction("dictionaryfirstkey", DictionaryOperation.DictionaryFirstKey, ProgVariableTypes.Text,
			"Returns the first key in the supplied dictionary, or blank if the dictionary is empty.");

		RegisterDictionaryFunction(
			"dictionarycontainskey",
			DictionaryOperation.DictionaryContainsKey,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Dictionary, ProgVariableTypes.Text },
			new[] { "dictionary", "key" },
			new[] { "The dictionary to inspect", "The text key to search for" },
			"Returns true if the supplied dictionary contains the specified text key."
		);
		RegisterDictionaryFunction(
			"dictionaryget",
			DictionaryOperation.DictionaryGet,
			ProgVariableTypes.CollectionItem,
			new[] { ProgVariableTypes.Dictionary, ProgVariableTypes.Text },
			new[] { "dictionary", "key" },
			new[] { "The dictionary to inspect", "The text key whose value should be returned" },
			"Returns the value stored at the specified text key, or null if the key is absent."
		);
		RegisterDictionaryFunction(
			"dictionarygetdefault",
			DictionaryOperation.DictionaryGetDefault,
			ProgVariableTypes.CollectionItem,
			new[] { ProgVariableTypes.Dictionary, ProgVariableTypes.Text, ProgVariableTypes.CollectionItem },
			new[] { "dictionary", "key", "default" },
			new[] { "The dictionary to inspect", "The text key whose value should be returned", "The value to return when the key is absent" },
			"Returns the value stored at the specified text key, or the supplied default value if the key is absent.",
			DictionaryDefaultCompatible
		);
		RegisterDictionaryFunction(
			"dictionarycontainsvalue",
			DictionaryOperation.DictionaryContainsValue,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Dictionary, ProgVariableTypes.CollectionItem },
			new[] { "dictionary", "value" },
			new[] { "The dictionary to inspect", "The value to search for" },
			"Returns true if any value in the dictionary matches the supplied value. Text comparison ignores case.",
			DictionaryValueCompatible
		);
		RegisterDictionaryFunction(
			"dictionarywithoutkey",
			DictionaryOperation.DictionaryWithoutKey,
			ProgVariableTypes.Dictionary | ProgVariableTypes.CollectionItem,
			new[] { ProgVariableTypes.Dictionary, ProgVariableTypes.Text },
			new[] { "dictionary", "key" },
			new[] { "The dictionary to copy", "The text key to omit from the returned dictionary" },
			"Returns a new dictionary with the specified key omitted. If the key is absent, the returned dictionary is a copy of the original."
		);
		RegisterDictionaryFunction(
			"dictionaryset",
			DictionaryOperation.DictionarySet,
			ProgVariableTypes.Dictionary | ProgVariableTypes.CollectionItem,
			new[] { ProgVariableTypes.Dictionary, ProgVariableTypes.Text, ProgVariableTypes.CollectionItem },
			new[] { "dictionary", "key", "value" },
			new[] { "The dictionary to copy", "The text key to set in the returned dictionary", "The value to store at that key" },
			"Returns a new dictionary with the specified key set to the supplied value. The original dictionary is not changed.",
			DictionaryDefaultCompatible
		);

		RegisterUnaryCollectionDictionaryFunction("collectiondictionarycount", DictionaryOperation.CollectionDictionaryCount, ProgVariableTypes.Number,
			"Returns the number of keys in the supplied collection dictionary.");
		RegisterUnaryCollectionDictionaryFunction("collectiondictionarylongcount", DictionaryOperation.CollectionDictionaryLongCount, ProgVariableTypes.Number,
			"Returns the total number of values stored across all keys in the supplied collection dictionary.");
		RegisterUnaryCollectionDictionaryFunction("collectiondictionaryany", DictionaryOperation.CollectionDictionaryAny, ProgVariableTypes.Boolean,
			"Returns true if the supplied collection dictionary contains at least one value.");
		RegisterUnaryCollectionDictionaryFunction("collectiondictionaryempty", DictionaryOperation.CollectionDictionaryEmpty, ProgVariableTypes.Boolean,
			"Returns true if the supplied collection dictionary contains no values.");
		RegisterUnaryCollectionDictionaryFunction("collectiondictionarykeys", DictionaryOperation.CollectionDictionaryKeys, ProgVariableTypes.Collection | ProgVariableTypes.Text,
			"Returns a text collection containing all keys in the supplied collection dictionary.");
		RegisterUnaryCollectionDictionaryFunction("collectiondictionaryvalues", DictionaryOperation.CollectionDictionaryValues, ProgVariableTypes.Collection | ProgVariableTypes.CollectionItem,
			"Returns a collection containing all values across all keys in the supplied collection dictionary.");

		RegisterDictionaryFunction(
			"collectiondictionarycontainskey",
			DictionaryOperation.CollectionDictionaryContainsKey,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.CollectionDictionary, ProgVariableTypes.Text },
			new[] { "dictionary", "key" },
			new[] { "The collection dictionary to inspect", "The text key to search for" },
			"Returns true if the supplied collection dictionary contains the specified text key."
		);
		RegisterDictionaryFunction(
			"collectiondictionaryget",
			DictionaryOperation.CollectionDictionaryGet,
			ProgVariableTypes.Collection | ProgVariableTypes.CollectionItem,
			new[] { ProgVariableTypes.CollectionDictionary, ProgVariableTypes.Text },
			new[] { "dictionary", "key" },
			new[] { "The collection dictionary to inspect", "The text key whose values should be returned" },
			"Returns the collection of values stored at the specified text key, or an empty collection if the key is absent."
		);
		RegisterDictionaryFunction(
			"collectiondictionarygetfirst",
			DictionaryOperation.CollectionDictionaryGetFirst,
			ProgVariableTypes.CollectionItem,
			new[] { ProgVariableTypes.CollectionDictionary, ProgVariableTypes.Text },
			new[] { "dictionary", "key" },
			new[] { "The collection dictionary to inspect", "The text key whose first value should be returned" },
			"Returns the first value stored at the specified text key, or null if the key is absent or has no values."
		);
		RegisterDictionaryFunction(
			"collectiondictionarycontainsvalue",
			DictionaryOperation.CollectionDictionaryContainsValue,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.CollectionDictionary, ProgVariableTypes.CollectionItem },
			new[] { "dictionary", "value" },
			new[] { "The collection dictionary to inspect", "The value to search for across all keys" },
			"Returns true if any value in the collection dictionary matches the supplied value. Text comparison ignores case.",
			CollectionDictionaryValueCompatible
		);
		RegisterDictionaryFunction(
			"collectiondictionarywithoutkey",
			DictionaryOperation.CollectionDictionaryWithoutKey,
			ProgVariableTypes.CollectionDictionary | ProgVariableTypes.CollectionItem,
			new[] { ProgVariableTypes.CollectionDictionary, ProgVariableTypes.Text },
			new[] { "dictionary", "key" },
			new[] { "The collection dictionary to copy", "The text key to omit from the returned collection dictionary" },
			"Returns a new collection dictionary with the specified key omitted. If the key is absent, the returned collection dictionary is a copy of the original."
		);
	}
}
