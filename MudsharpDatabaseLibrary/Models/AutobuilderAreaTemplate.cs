using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class AutobuilderAreaTemplate
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string TemplateType { get; set; }
        public string Definition { get; set; }
    }
}
