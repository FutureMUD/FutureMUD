using System;
using System.Collections;
using System.Collections.Generic;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Variables {
	public class CollectionVariable : ProgVariable, IEnumerable {
		private readonly IList _underlyingList;

		private readonly ProgVariableTypes _underlyingType;

		public CollectionVariable(IList underlyingList, ProgVariableTypes underlyingType) {
			_underlyingList = underlyingList;
			_underlyingType = underlyingType;
		}

		public override ProgVariableTypes Type => ProgVariableTypes.Collection | _underlyingType;

		public override object GetObject => _underlyingList;

		#region IEnumerable Members

		public IEnumerator GetEnumerator() {
			return _underlyingList.GetEnumerator();
		}

		#endregion

		private static IReadOnlyDictionary<string,ProgVariableTypes> DotReferenceHandler(ProgVariableTypes type)
		{
			return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
			{
				{"count", ProgVariableTypes.Number},
				{"any", ProgVariableTypes.Boolean},
				{"empty", ProgVariableTypes.Boolean},
				{ "first", type},
				{ "last", type},
				{ "reverse", ProgVariableTypes.Collection | type},
			};
		}

		private static IReadOnlyDictionary<string,string> DotReferenceHelp()
		{
			return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
			{
				{"count", "The number of items in the collection"},
				{"any", "True if there are any items in the collection"},
				{"empty", "True if the collection is empty"},
				{ "first", "The first item in the collection or null if empty"},
				{ "last", "The last item in the collection or null if empty"},
				{ "reverse", "The collection in reverse order"},
			};
		}

		public static void RegisterFutureProgCompiler() {
			ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Collection, DotReferenceHandler(ProgVariableTypes.Void), DotReferenceHelp());
			foreach (var flag in ProgVariableTypes.CollectionItem.GetAllFlags())
			{
				if (flag == ProgVariableTypes.Void)
				{
					continue;
				}
				ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Collection | flag, DotReferenceHandler(flag), DotReferenceHelp());
			}
		}

		public override IProgVariable GetProperty(string property) {
			switch (property.ToLowerInvariant()) {
				case "count":
					return new NumberVariable(_underlyingList.Count);
				case "any":
					return new BooleanVariable(_underlyingList.Count > 0);
				case "empty":
					return new BooleanVariable(_underlyingList.Count == 0);
				case "first":
					return _underlyingList.Count > 0 ? _underlyingList[0] as IProgVariable : new NullVariable(_underlyingType);
				case "last":
					return _underlyingList.Count > 0 ? _underlyingList[^1] as IProgVariable : new NullVariable(_underlyingType);
				case "reverse":
					var list = new List<IProgVariable>();
					for (var i = _underlyingList.Count - 1; i >= 0; i--)
					{
						list.Add((IProgVariable)_underlyingList[i]);
					}

					return new CollectionVariable(list, _underlyingType);

			}
			throw new NotSupportedException("Invalid property requested in CollectionVariable.GetProperty");
		}
	}
}