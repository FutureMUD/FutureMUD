using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class EmailTemplate
    {
        public int TemplateType { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        public string ReturnAddress { get; set; }
    }
}
