using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class VariableDefinition
    {
        public long OwnerType { get; set; }
        public string Property { get; set; }
        public long ContainedType { get; set; }
    }
}
