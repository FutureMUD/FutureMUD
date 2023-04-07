using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class MagicGenerator
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Definition { get; set; }
    }
}
