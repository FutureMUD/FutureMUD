using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Variables {
    public abstract class ProgVariable : IProgVariable {
        private static readonly Dictionary<ProgVariableTypes, FutureProgVariableCompileInfo> _dotReferenceCompileInfos = new();

        public abstract IProgVariable GetProperty(string property);

        public abstract ProgVariableTypes Type { get; }

        public abstract object GetObject { get; }

        public static void RegisterDotReferenceCompileInfo(ProgVariableTypes type,
            IReadOnlyDictionary<string,ProgVariableTypes> typeDictionary, IReadOnlyDictionary<string,string> helpDictionary) {
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

        public static ProgVariableTypes DotReferenceReturnTypeFor(ProgVariableTypes type, string property) {
            if (_dotReferenceCompileInfos.ContainsKey(type)) {
                if (!_dotReferenceCompileInfos[type].PropertyTypeMap.ContainsKey(property))
                {
                    return ProgVariableTypes.Error;
                }
                return _dotReferenceCompileInfos[type].PropertyTypeMap[property];
            }
            throw new ApplicationException(
                $"There was no DotReferenceCompileInfo for type {type.Describe()} {(long) type} property {property}");
        }

        public static IReadOnlyDictionary<ProgVariableTypes, FutureProgVariableCompileInfo> DotReferenceCompileInfos =>
            _dotReferenceCompileInfos;
    }
}