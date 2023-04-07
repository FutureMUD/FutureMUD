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
        FutureProgVariableTypes UnderlyingType { get; }
        bool Add(string key, IFutureProgVariable item);
        bool Remove(string key);
    }

    public class DictionaryVariable : FutureProgVariable, IEnumerable, IDictionaryVariable
    {
        private readonly Dictionary<string,IFutureProgVariable> _underlyingDictionary;

        public FutureProgVariableTypes UnderlyingType { get; }

        public DictionaryVariable(Dictionary<string, IFutureProgVariable> underlyingList, FutureProgVariableTypes underlyingType)
        {
            _underlyingDictionary = underlyingList;
            UnderlyingType = underlyingType;
        }

        public override FutureProgVariableTypes Type => FutureProgVariableTypes.Dictionary | UnderlyingType;

        public override object GetObject => _underlyingDictionary;

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _underlyingDictionary.Values.GetEnumerator();
        }
        #endregion

        private static FutureProgVariableTypes DotReferenceHandler(string property)
        {
            switch (property.ToLowerInvariant())
            {
                case "count":
                    return FutureProgVariableTypes.Number;
                case "any":
                    return FutureProgVariableTypes.Boolean;
                case "empty":
                    return FutureProgVariableTypes.Boolean;
                case "values":
                    return FutureProgVariableTypes.Collection | FutureProgVariableTypes.CollectionItem;
                case "keys":
                    return FutureProgVariableTypes.Collection | FutureProgVariableTypes.Text;
            }
            return FutureProgVariableTypes.Error;
        }

        private static IReadOnlyDictionary<string,FutureProgVariableTypes> DotReferenceHandler()
        {
            return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"count", FutureProgVariableTypes.Number},
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
                {"any", ""},
                {"empty", ""},
                {"values", ""},
                {"keys", ""},
            };
        }

        public static void RegisterFutureProgCompiler() {
            FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Dictionary, DotReferenceHandler(), DotReferenceHelp());
            foreach (var flag in FutureProgVariableTypes.CollectionItem.GetAllFlags())
            {
                if (flag == FutureProgVariableTypes.Void)
                {
                    continue;
                }
                FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Dictionary | flag, DotReferenceHandler(), DotReferenceHelp());
            }
        }

        public override IFutureProgVariable GetProperty(string property)
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
                    return new CollectionVariable(_underlyingDictionary.Values.ToList(), FutureProgVariableTypes.CollectionItem);
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
            _underlyingDictionary[key] = item;
            return true;
        }

        public bool Remove(string key)
        {
            return _underlyingDictionary.Remove(key);
        }
    }
}
