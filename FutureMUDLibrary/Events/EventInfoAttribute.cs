using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.FutureProg;

namespace MudSharp.Events
{
    public class EventInfoAttribute : Attribute
    {
        public string Description { get; }
        public IEnumerable<(string type,string name)> Parameters { get; }

        public IEnumerable<ProgVariableTypes> ProgTypes { get; }

        public EventInfoAttribute(string description, string[] parameterTypes, string[] parameterNames, ProgVariableTypeCode[] progTypes) {
            Description = description;
            Parameters = parameterTypes.Zip(parameterNames, (type, name) => (type, name)).ToList();
            ProgTypes = progTypes.Select(x => ProgVariableTypes.FromLegacyLong((long)x)).ToList();
        }
    }
}
