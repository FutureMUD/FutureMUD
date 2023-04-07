using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CheckTemplateDifficulty
    {
        public long CheckTemplateId { get; set; }
        public int Difficulty { get; set; }
        public double Modifier { get; set; }

        public virtual CheckTemplate CheckTemplate { get; set; }
    }
}
