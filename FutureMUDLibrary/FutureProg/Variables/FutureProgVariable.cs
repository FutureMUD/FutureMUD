using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Variables {
    public abstract class FutureProgVariable : IFutureProgVariable {
        private static readonly Dictionary<FutureProgVariableTypes, FutureProgVariableCompileInfo> _dotReferenceCompileInfos = new();

        public abstract IFutureProgVariable GetProperty(string property);

        public abstract FutureProgVariableTypes Type { get; }

        public abstract object GetObject { get; }

        public static void RegisterDotReferenceCompileInfo(FutureProgVariableTypes type,
            IReadOnlyDictionary<string,FutureProgVariableTypes> typeDictionary, IReadOnlyDictionary<string,string> helpDictionary) {
            if (_dotReferenceCompileInfos.ContainsKey(type)) {
                throw new NotSupportedException();
            }

            _dotReferenceCompileInfos.Add(type, new FutureProgVariableCompileInfo
            {
                VariableType = type,
                PropertyTypeMap = typeDictionary.ToDictionary(x => x.Key, x => x.Value, StringComparer.InvariantCultureIgnoreCase),
                PropertyHelpInfo = helpDictionary
            });
        }

        public static FutureProgVariableTypes DotReferenceReturnTypeFor(FutureProgVariableTypes type, string property) {
            if (_dotReferenceCompileInfos.ContainsKey(type)) {
                if (!_dotReferenceCompileInfos[type].PropertyTypeMap.ContainsKey(property))
                {
                    return FutureProgVariableTypes.Error;
                }
                return _dotReferenceCompileInfos[type].PropertyTypeMap[property];
            }
            throw new ApplicationException(
                $"There was no DotReferenceCompileInfo for type {type.Describe()} {(long) type} property {property}");
        }

        public static IReadOnlyDictionary<FutureProgVariableTypes, FutureProgVariableCompileInfo> DotReferenceCompileInfos =>
            _dotReferenceCompileInfos;
    }
}