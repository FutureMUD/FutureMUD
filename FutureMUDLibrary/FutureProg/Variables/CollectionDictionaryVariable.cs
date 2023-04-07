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
        FutureProgVariableTypes UnderlyingType { get; }
        bool Add(string key, IFutureProgVariable item);
        bool Remove(string key, IFutureProgVariable item);
    }

    public class CollectionDictionaryVariable : FutureProgVariable, IEnumerable, ICollectionDictionaryVariable
    {
        private readonly CollectionDictionary<string, IFutureProgVariable> _underlyingDictionary;

        public FutureProgVariableTypes UnderlyingType { get; }

        public CollectionDictionaryVariable(CollectionDictionary<string, IFutureProgVariable> underlyingList, FutureProgVariableTypes underlyingType)
        {
            _underlyingDictionary = underlyingList;
            UnderlyingType = underlyingType;
        }

        public override FutureProgVariableTypes Type => FutureProgVariableTypes.CollectionDictionary | UnderlyingType;

        public override object GetObject => _underlyingDictionary;

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _underlyingDictionary.SelectMany(x => x.Value).GetEnumerator();
        }
        #endregion

        private static IReadOnlyDictionary<string,FutureProgVariableTypes> DotReferenceHandler()
        {
            return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"count", FutureProgVariableTypes.Number},
                {"longcount", FutureProgVariableTypes.Number},
                {"any", FutureProgVariableTypes.Boolean},
                {"empty", FutureProgVariableTypes.Boolean},
                {"values", FutureProgVariableTypes.Collection | FutureProgVariableTypes.CollectionItem},
                {"keys", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Text},
            };
        }

        private static IReadOnlyDictionary<string,string> DotReferenceHelp()
        {
            return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"count", ""},
                {"longcount", ""},
                {"any", ""},
                {"empty", ""},
                {"values", ""},
                {"keys", ""},
            };
        }

        public static void RegisterFutureProgCompiler() {
            FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.CollectionDictionary, DotReferenceHandler(), DotReferenceHelp());
            foreach (var flag in FutureProgVariableTypes.CollectionItem.GetAllFlags())
            {
                if (flag == FutureProgVariableTypes.Void)
                {
                    continue;
                }
                FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.CollectionDictionary | flag, DotReferenceHandler(), DotReferenceHelp());
            }
        }

        public override IFutureProgVariable GetProperty(string property)
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
                    return new CollectionVariable(_underlyingDictionary.SelectMany(x => x.Value).ToList(), FutureProgVariableTypes.CollectionItem);
                case "keys":
                    return new CollectionVariable(_underlyingDictionary.Keys.ToList(), FutureProgVariableTypes.Text);
            }
            throw new NotSupportedException("Invalid property requested in DictionaryVariable.GetProperty");
        }

        public bool Add(string key, IFutureProgVariable item)
        {
            if (item?.Type.CompatibleWith(UnderlyingType) != true)
            {
                return false;
            }
            _underlyingDictionary.Add(key, item);
            return true;
        }

        public bool Remove(string key, IFutureProgVariable item)
        {
            _underlyingDictionary.Remove(key, item);
            return true;
        }
    }
}
