using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Variables
{
	public interface IDictionaryVariable
	{
		ProgVariableTypes UnderlyingType { get; }
		bool Add(string key, IProgVariable item);
		bool Remove(string key);
	}

	public class DictionaryVariable : ProgVariable, IEnumerable, IDictionaryVariable
	{
		private readonly Dictionary<string,IProgVariable> _underlyingDictionary;

		public ProgVariableTypes UnderlyingType { get; }

		public DictionaryVariable(Dictionary<string, IProgVariable> underlyingList, ProgVariableTypes underlyingType)
		{
			_underlyingDictionary = underlyingList;
			UnderlyingType = underlyingType;
		}

		public override ProgVariableTypes Type => ProgVariableTypes.Dictionary | UnderlyingType;

		public override object GetObject => _underlyingDictionary;

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return _underlyingDictionary.Values.GetEnumerator();
		}
		#endregion

		private static IReadOnlyDictionary<string,ProgVariableTypes> DotReferenceHandler(ProgVariableTypes type)
		{
			return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
			{
				{"count", ProgVariableTypes.Number},
				{"any", ProgVariableTypes.Boolean},
				{"empty", ProgVariableTypes.Boolean},
				{"values", ProgVariableTypes.Collection | type},
				{"keys", ProgVariableTypes.Collection | ProgVariableTypes.Text},
			};
		}

		private static IReadOnlyDictionary<string,string> DotReferenceHelp()
		{
			return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
			{
				{"count", "The number of items in this dictionary"},
				{"any", "True if there are 1 or more items, otherwise false"},
				{"empty", "True if empty"},
				{"values", "A collection of the values without their keys"},
				{"keys", "A collection of the keys without their values"},
			};
		}

		public static void RegisterFutureProgCompiler() {
			ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Dictionary, DotReferenceHandler(ProgVariableTypes.Void), DotReferenceHelp());
			foreach (var flag in ProgVariableTypes.CollectionItem.GetAllFlags())
			{
				if (flag == ProgVariableTypes.Void)
				{
					continue;
				}
				ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Dictionary | flag, DotReferenceHandler(flag), DotReferenceHelp());
			}
		}

		public override IProgVariable GetProperty(string property)
		{
			switch (property.ToLowerInvariant())
			{
				case "count":
					return new NumberVariable(_underlyingDictionary.Count);
				case "any":
					return new BooleanVariable(_underlyingDictionary.Count > 0);
				case "empty":
					return new BooleanVariable(_underlyingDictionary.Count == 0);
				case "values":
					return new CollectionVariable(_underlyingDictionary.Values.ToList(), UnderlyingType);
				case "keys":
					return new CollectionVariable(_underlyingDictionary.Keys.ToList(), ProgVariableTypes.Text);
			}
			throw new NotSupportedException("Invalid property requested in DictionaryVariable.GetProperty");
		}

		public bool Add(string key, IProgVariable item)
		{
			if (item?.Type.CompatibleWith(UnderlyingType) != true)
			{
				return false;
			}
			_underlyingDictionary[key] = item;
			return true;
		}

		public bool Remove(string key)
		{
			return _underlyingDictionary.Remove(key);
		}
	}
}
