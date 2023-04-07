using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class StackDecorator
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Definition { get; set; }
        public string Description { get; set; }
    }
}
