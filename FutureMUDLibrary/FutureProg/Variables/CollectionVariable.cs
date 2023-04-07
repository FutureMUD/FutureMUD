using System;
using System.Collections;
using System.Collections.Generic;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Variables {
    public class CollectionVariable : FutureProgVariable, IEnumerable {
        private readonly IList _underlyingList;

        private readonly FutureProgVariableTypes _underlyingType;

        public CollectionVariable(IList underlyingList, FutureProgVariableTypes underlyingType) {
            _underlyingList = underlyingList;
            _underlyingType = underlyingType;
        }

        public override FutureProgVariableTypes Type => FutureProgVariableTypes.Collection | _underlyingType;

        public override object GetObject => _underlyingList;

        #region IEnumerable Members

        public IEnumerator GetEnumerator() {
            return _underlyingList.GetEnumerator();
        }

        #endregion

        private static IReadOnlyDictionary<string,FutureProgVariableTypes> DotReferenceHandler()
        {
            return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"count", FutureProgVariableTypes.Number},
                {"any", FutureProgVariableTypes.Boolean},
                {"empty", FutureProgVariableTypes.Boolean},
            };
        }

        private static IReadOnlyDictionary<string,string> DotReferenceHelp()
        {
            return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"count", ""},
                {"any", ""},
                {"empty", ""},
            };
        }

        public static void RegisterFutureProgCompiler() {
            FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Collection, DotReferenceHandler(), DotReferenceHelp());
            foreach (var flag in FutureProgVariableTypes.CollectionItem.GetAllFlags())
            {
                if (flag == FutureProgVariableTypes.Void)
                {
                    continue;
                }
                FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Collection | flag, DotReferenceHandler(), DotReferenceHelp());
            }
        }

        public override IFutureProgVariable GetProperty(string property) {
            switch (property.ToLowerInvariant()) {
                case "count":
                    return new NumberVariable(_underlyingList.Count);
                case "any":
                    return new BooleanVariable(_underlyingList.Count > 0);
                case "empty":
                    return new BooleanVariable(_underlyingList.Count == 0);
            }
            throw new NotSupportedException("Invalid property requested in CollectionVariable.GetProperty");
        }
    }
}