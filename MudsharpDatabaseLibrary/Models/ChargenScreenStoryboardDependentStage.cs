using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Models
{
    public class ChargenScreenStoryboardDependentStage
    {
        public long OwnerId { get; set; }
        public virtual ChargenScreenStoryboard Owner { get; set; }

        public int Dependency { get; set; }
    }
}
