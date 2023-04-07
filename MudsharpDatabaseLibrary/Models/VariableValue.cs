using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class VariableValue
    {
        public long ReferenceType { get; set; }
        public long ReferenceId { get; set; }
        public string ReferenceProperty { get; set; }
        public string ValueDefinition { get; set; }
        public long ValueType { get; set; }
    }
}
