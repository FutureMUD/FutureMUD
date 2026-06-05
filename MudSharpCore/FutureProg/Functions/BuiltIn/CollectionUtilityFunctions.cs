using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class CollectionUtilityFunction : BuiltInFunction
{
	private readonly CollectionOperation _operation;
	private readonly ProgVariableTypes _metadataReturnType;

	private enum CollectionOperation
	{
		Count,
		Any,
		Empty,
		First,
		Last,
		At,
		Reverse,
		Take,
		Skip,
		Range,
		Append,
		Prepend,
		WithoutIndex,
		Concat,
		Contains,
		IndexOf,
		Distinct,
		Shuffle,
		IsValidIndex
	}

	private CollectionUtilityFunction(IList<IFunction> parameters, CollectionOperation operation,
		ProgVariableTypes metadataReturnType) : base(parameters)
	{
		_operation = operation;
		_metadataReturnType = metadataReturnType;
	}

	private ProgVariableTypes ElementType => ParameterFunctions[0].ReturnType ^ ProgVariableTypes.Collection;

	public override ProgVariableTypes ReturnType
	{
		get
		{
			return _operation switch
			{
				CollectionOperation.First or CollectionOperation.Last or CollectionOperation.At => ElementType,
				CollectionOperation.Reverse or CollectionOperation.Take or CollectionOperation.Skip or CollectionOperation.Range
					or CollectionOperation.Append or CollectionOperation.Prepend or CollectionOperation.WithoutIndex
					or CollectionOperation.Concat or CollectionOperation.Distinct or CollectionOperation.Shuffle => ParameterFunctions[0].ReturnType,
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

		var collection = GetCollection(0);
		switch (_operation)
		{
			case CollectionOperation.Count:
				Result = new NumberVariable(collection.Count);
				return StatementResult.Normal;
			case CollectionOperation.Any:
				Result = new BooleanVariable(collection.Count > 0);
				return StatementResult.Normal;
			case CollectionOperation.Empty:
				Result = new BooleanVariable(collection.Count == 0);
				return StatementResult.Normal;
			case CollectionOperation.First:
				Result = collection.FirstOrDefault() ?? new NullVariable(ElementType);
				return StatementResult.Normal;
			case CollectionOperation.Last:
				Result = collection.LastOrDefault() ?? new NullVariable(ElementType);
				return StatementResult.Normal;
			case CollectionOperation.At:
				Result = At(collection, GetInteger(1)) ?? new NullVariable(ElementType);
				return StatementResult.Normal;
			case CollectionOperation.Reverse:
				Result = ToCollection(collection.AsEnumerable().Reverse());
				return StatementResult.Normal;
			case CollectionOperation.Take:
				Result = ToCollection(collection.Take(Math.Max(0, GetInteger(1))));
				return StatementResult.Normal;
			case CollectionOperation.Skip:
				Result = ToCollection(collection.Skip(Math.Max(0, GetInteger(1))));
				return StatementResult.Normal;
			case CollectionOperation.Range:
				Result = ToCollection(collection.Skip(Math.Max(0, GetInteger(1))).Take(Math.Max(0, GetInteger(2))));
				return StatementResult.Normal;
			case CollectionOperation.Append:
				Result = ToCollection(collection.Append(ParameterFunctions[1].Result ?? new NullVariable(ElementType)));
				return StatementResult.Normal;
			case CollectionOperation.Prepend:
				Result = ToCollection(collection.Prepend(ParameterFunctions[1].Result ?? new NullVariable(ElementType)));
				return StatementResult.Normal;
			case CollectionOperation.WithoutIndex:
				Result = ToCollection(collection.Where((_, index) => index != GetInteger(1)));
				return StatementResult.Normal;
			case CollectionOperation.Concat:
				Result = ToCollection(collection.Concat(GetCollection(1)));
				return StatementResult.Normal;
			case CollectionOperation.Contains:
				Result = new BooleanVariable(collection.Any(x => ValuesEqual(x, ParameterFunctions[1].Result)));
				return StatementResult.Normal;
			case CollectionOperation.IndexOf:
				Result = new NumberVariable(IndexOf(collection, ParameterFunctions[1].Result));
				return StatementResult.Normal;
			case CollectionOperation.Distinct:
				Result = ToCollection(Distinct(collection));
				return StatementResult.Normal;
			case CollectionOperation.Shuffle:
				Result = ToCollection(collection.OrderBy(_ => Random.Shared.Next()));
				return StatementResult.Normal;
			case CollectionOperation.IsValidIndex:
				var index = GetInteger(1);
				Result = new BooleanVariable(index >= 0 && index < collection.Count);
				return StatementResult.Normal;
			default:
				throw new NotSupportedException($"Unknown collection utility operation {_operation}.");
		}
	}

	private List<IProgVariable> GetCollection(int index)
	{
		return ParameterFunctions[index].Result?.GetObject is IList list
			? list.OfType<IProgVariable>().ToList()
			: new List<IProgVariable>();
	}

	private int GetInteger(int index)
	{
		return ParameterFunctions[index].Result?.GetObject is decimal value ? (int)value : 0;
	}

	private IProgVariable At(IReadOnlyList<IProgVariable> collection, int index)
	{
		return index >= 0 && index < collection.Count ? collection[index] : null;
	}

	private CollectionVariable ToCollection(IEnumerable<IProgVariable> collection)
	{
		return new CollectionVariable(collection.ToList(), ElementType);
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

	private static int IndexOf(IReadOnlyList<IProgVariable> collection, IProgVariable item)
	{
		for (var i = 0; i < collection.Count; i++)
		{
			if (ValuesEqual(collection[i], item))
			{
				return i;
			}
		}

		return -1;
	}

	private static IEnumerable<IProgVariable> Distinct(IEnumerable<IProgVariable> collection)
	{
		List<IProgVariable> seen = new();
		foreach (var item in collection)
		{
			if (seen.Any(x => ValuesEqual(x, item)))
			{
				continue;
			}

			seen.Add(item);
			yield return item;
		}
	}

	private static bool ItemCompatibleWithCollection(IEnumerable<ProgVariableTypes> types, IFuturemud gameworld)
	{
		var actual = types.ToList();
		var elementType = actual[0] ^ ProgVariableTypes.Collection;
		return elementType == ProgVariableTypes.Void || actual[1].CompatibleWith(elementType);
	}

	private static bool CollectionsCompatible(IEnumerable<ProgVariableTypes> types, IFuturemud gameworld)
	{
		var actual = types.ToList();
		var firstType = actual[0] ^ ProgVariableTypes.Collection;
		var secondType = actual[1] ^ ProgVariableTypes.Collection;
		return firstType == ProgVariableTypes.Void ||
		       secondType == ProgVariableTypes.Void ||
		       secondType.CompatibleWith(firstType);
	}

	private static void RegisterCollectionFunction(string name, CollectionOperation operation, ProgVariableTypes returnType,
		IEnumerable<ProgVariableTypes> parameters, IEnumerable<string> parameterNames,
		IEnumerable<string> parameterHelp, string functionHelp,
		Func<IEnumerable<ProgVariableTypes>, IFuturemud, bool> filter = null)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			parameters,
			(pars, gameworld) => new CollectionUtilityFunction(pars, operation, returnType),
			parameterNames,
			parameterHelp,
			functionHelp,
			"Collections",
			returnType,
			filter
		));
	}

	private static void RegisterUnaryCollectionFunction(string name, CollectionOperation operation,
		ProgVariableTypes returnType, string functionHelp)
	{
		RegisterCollectionFunction(
			name,
			operation,
			returnType,
			new[] { ProgVariableTypes.Collection },
			new[] { "collection" },
			new[] { "The collection to inspect or transform" },
			functionHelp
		);
	}

	private static void RegisterCollectionNumberFunction(string name, CollectionOperation operation,
		ProgVariableTypes returnType, string numberName, string numberHelp, string functionHelp)
	{
		RegisterCollectionFunction(
			name,
			operation,
			returnType,
			new[] { ProgVariableTypes.Collection, ProgVariableTypes.Number },
			new[] { "collection", numberName },
			new[] { "The collection to inspect or transform", numberHelp },
			functionHelp
		);
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterUnaryCollectionFunction("collectioncount", CollectionOperation.Count, ProgVariableTypes.Number,
			"Returns the number of items in the supplied collection.");
		RegisterUnaryCollectionFunction("collectionany", CollectionOperation.Any, ProgVariableTypes.Boolean,
			"Returns true if the supplied collection contains at least one item.");
		RegisterUnaryCollectionFunction("collectionempty", CollectionOperation.Empty, ProgVariableTypes.Boolean,
			"Returns true if the supplied collection has no items.");
		RegisterUnaryCollectionFunction("collectionfirst", CollectionOperation.First, ProgVariableTypes.CollectionItem,
			"Returns the first item in the supplied collection, or null if the collection is empty.");
		RegisterUnaryCollectionFunction("collectionlast", CollectionOperation.Last, ProgVariableTypes.CollectionItem,
			"Returns the last item in the supplied collection, or null if the collection is empty.");
		RegisterUnaryCollectionFunction("collectionreverse", CollectionOperation.Reverse, ProgVariableTypes.Collection | ProgVariableTypes.CollectionItem,
			"Returns a new collection with the supplied collection's items in reverse order.");
		RegisterUnaryCollectionFunction("collectiondistinct", CollectionOperation.Distinct, ProgVariableTypes.Collection | ProgVariableTypes.CollectionItem,
			"Returns a new collection with duplicate values removed, keeping the first occurrence of each value.");
		RegisterUnaryCollectionFunction("collectionshuffle", CollectionOperation.Shuffle, ProgVariableTypes.Collection | ProgVariableTypes.CollectionItem,
			"Returns a new collection with the supplied collection's items in random order.");

		RegisterCollectionNumberFunction("collectionat", CollectionOperation.At, ProgVariableTypes.CollectionItem,
			"index", "The zero-based index of the item to retrieve",
			"Returns the item at the zero-based index, or null if the index is outside the collection.");
		RegisterCollectionNumberFunction("collectiontake", CollectionOperation.Take, ProgVariableTypes.Collection | ProgVariableTypes.CollectionItem,
			"count", "The maximum number of items to keep from the start of the collection",
			"Returns a new collection containing up to count items from the start of the supplied collection.");
		RegisterCollectionNumberFunction("collectionskip", CollectionOperation.Skip, ProgVariableTypes.Collection | ProgVariableTypes.CollectionItem,
			"count", "The number of items to skip from the start of the collection",
			"Returns a new collection excluding up to count items from the start of the supplied collection.");
		RegisterCollectionNumberFunction("collectionwithoutindex", CollectionOperation.WithoutIndex, ProgVariableTypes.Collection | ProgVariableTypes.CollectionItem,
			"index", "The zero-based index of the item to omit",
			"Returns a new collection with the item at the zero-based index omitted. Invalid indexes return a copy of the original collection.");
		RegisterCollectionNumberFunction("collectionisvalidindex", CollectionOperation.IsValidIndex, ProgVariableTypes.Boolean,
			"index", "The zero-based index to test",
			"Returns true if the supplied zero-based index points at an item in the collection.");

		RegisterCollectionFunction(
			"collectionrange",
			CollectionOperation.Range,
			ProgVariableTypes.Collection | ProgVariableTypes.CollectionItem,
			new[] { ProgVariableTypes.Collection, ProgVariableTypes.Number, ProgVariableTypes.Number },
			new[] { "collection", "index", "count" },
			new[] { "The collection to slice", "The zero-based index at which the slice begins", "The maximum number of items to return" },
			"Returns a new collection containing up to count items from the supplied zero-based index."
		);
		RegisterCollectionFunction(
			"collectionappend",
			CollectionOperation.Append,
			ProgVariableTypes.Collection | ProgVariableTypes.CollectionItem,
			new[] { ProgVariableTypes.Collection, ProgVariableTypes.CollectionItem },
			new[] { "collection", "item" },
			new[] { "The collection to copy", "The item to append to the end of the returned collection" },
			"Returns a new collection with the supplied item appended to the end.",
			ItemCompatibleWithCollection
		);
		RegisterCollectionFunction(
			"collectionprepend",
			CollectionOperation.Prepend,
			ProgVariableTypes.Collection | ProgVariableTypes.CollectionItem,
			new[] { ProgVariableTypes.Collection, ProgVariableTypes.CollectionItem },
			new[] { "collection", "item" },
			new[] { "The collection to copy", "The item to prepend to the start of the returned collection" },
			"Returns a new collection with the supplied item prepended to the start.",
			ItemCompatibleWithCollection
		);
		RegisterCollectionFunction(
			"collectionconcat",
			CollectionOperation.Concat,
			ProgVariableTypes.Collection | ProgVariableTypes.CollectionItem,
			new[] { ProgVariableTypes.Collection, ProgVariableTypes.Collection },
			new[] { "first", "second" },
			new[] { "The collection whose type and leading items should be used", "The collection whose items should be appended" },
			"Returns a new collection with the second collection appended to the first collection.",
			CollectionsCompatible
		);
		RegisterCollectionFunction(
			"collectioncontains",
			CollectionOperation.Contains,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Collection, ProgVariableTypes.CollectionItem },
			new[] { "collection", "item" },
			new[] { "The collection to search", "The item to search for" },
			"Returns true if the supplied collection contains the supplied item. Text comparison ignores case.",
			ItemCompatibleWithCollection
		);
		RegisterCollectionFunction(
			"collectionindexof",
			CollectionOperation.IndexOf,
			ProgVariableTypes.Number,
			new[] { ProgVariableTypes.Collection, ProgVariableTypes.CollectionItem },
			new[] { "collection", "item" },
			new[] { "The collection to search", "The item whose position should be returned" },
			"Returns the zero-based index of the first matching item in the supplied collection, or -1 if absent. Text comparison ignores case.",
			ItemCompatibleWithCollection
		);
	}
}
