using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class GroupAi
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long GroupAiTemplateId { get; set; }
        public string Data { get; set; }
        public string Definition { get; set; }

        public virtual GroupAiTemplate GroupAiTemplate { get; set; }
    }
}
