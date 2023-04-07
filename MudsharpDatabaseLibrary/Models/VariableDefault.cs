using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class VariableDefault
    {
        public long OwnerType { get; set; }
        public string Property { get; set; }
        public string DefaultValue { get; set; }
    }
}
