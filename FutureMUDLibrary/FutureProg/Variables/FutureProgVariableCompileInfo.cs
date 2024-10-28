using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.FutureProg.Variables
{
    public class FutureProgVariableCompileInfo
    {
        public ProgVariableTypes VariableType { get; set; }
        public IReadOnlyDictionary<string, ProgVariableTypes> PropertyTypeMap { get; init; }
        public IReadOnlyDictionary<string,string> PropertyHelpInfo { get; init; }
    }
}
