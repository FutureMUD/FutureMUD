using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.FutureProg.Variables
{
    public class FutureProgVariableCompileInfo
    {
        public FutureProgVariableTypes VariableType { get; set; }
        public IReadOnlyDictionary<string, FutureProgVariableTypes> PropertyTypeMap { get; set; }
        public IReadOnlyDictionary<string,string> PropertyHelpInfo { get; set; }
    }
}
