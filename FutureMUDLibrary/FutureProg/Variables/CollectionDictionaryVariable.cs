using MudSharp.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Variables
{
    public interface ICollectionDictionaryVariable
    {
        ProgVariableTypes UnderlyingType { get; }
        bool Add(string key, IProgVariable item);
        bool Remove(string key, IProgVariable item);
    }

    public class CollectionDictionaryVariable : ProgVariable, IEnumerable, ICollectionDictionaryVariable
    {
        private readonly CollectionDictionary<string, IProgVariable> _underlyingDictionary;

        public ProgVariableTypes UnderlyingType { get; }

        public CollectionDictionaryVariable(CollectionDictionary<string, IProgVariable> underlyingList, ProgVariableTypes underlyingType)
        {
            _underlyingDictionary = underlyingList;
            UnderlyingType = underlyingType;
        }

        public override ProgVariableTypes Type => ProgVariableTypes.CollectionDictionary | UnderlyingType;

        public override object GetObject => _underlyingDictionary;

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _underlyingDictionary.SelectMany(x => x.Value).GetEnumerator();
        }
        #endregion

        private static IReadOnlyDictionary<string,ProgVariableTypes> DotReferenceHandler(ProgVariableTypes type)
        {
            return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"count", ProgVariableTypes.Number},
                {"longcount", ProgVariableTypes.Number},
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
                {"count", "The number of keys in the collection dictionary"},
                {"longcount", "The number of values in all of the sub collections"},
                {"any", "True if there are any values in this collection dictionary"},
                {"empty", "True if the collection dictionary is empty"},
                {"values", "Returns all of the values in all of the collection dictionary collections"},
                {"keys", "Returns all of the keys in the collection dictionary"},
            };
        }

        public static void RegisterFutureProgCompiler() {
            ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.CollectionDictionary, DotReferenceHandler(ProgVariableTypes.Void), DotReferenceHelp());
            foreach (var flag in ProgVariableTypes.CollectionItem.GetAllFlags())
            {
                if (flag == ProgVariableTypes.Void)
                {
                    continue;
                }
                ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.CollectionDictionary | flag, DotReferenceHandler(flag), DotReferenceHelp());
            }
        }

        public override IProgVariable GetProperty(string property)
        {
            switch (property.ToLowerInvariant())
            {
                case "count":
                    return new NumberVariable(_underlyingDictionary.Keys.Count());
                case "longcount":
                    return new NumberVariable(_underlyingDictionary.Sum(x => x.Value.Count));
                case "any":
                    return new BooleanVariable(_underlyingDictionary.Any());
                case "empty":
                    return new BooleanVariable(!_underlyingDictionary.Any());
                case "values":
                    return new CollectionVariable(_underlyingDictionary.SelectMany(x => x.Value).ToList(), ProgVariableTypes.CollectionItem);
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
            _underlyingDictionary.Add(key, item);
            return true;
        }

        public bool Remove(string key, IProgVariable item)
        {
            _underlyingDictionary.Remove(key, item);
            return true;
        }
    }
}
