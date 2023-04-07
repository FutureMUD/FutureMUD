using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class EntityDescriptionPatternsEntityDescriptions
    {
        public long PatternId { get; set; }
        public long EntityDescriptionId { get; set; }

        public virtual EntityDescriptions EntityDescription { get; set; }
        public virtual EntityDescriptionPattern Pattern { get; set; }
    }
}
