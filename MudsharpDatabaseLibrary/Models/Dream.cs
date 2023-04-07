using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Dream
    {
        public Dream()
        {
            DreamPhases = new HashSet<DreamPhase>();
            DreamsAlreadyDreamt = new HashSet<DreamsAlreadyDreamt>();
            DreamsCharacters = new HashSet<DreamsCharacters>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? CanDreamProgId { get; set; }
        public long? OnDreamProgId { get; set; }
        public long? OnWakeDuringDreamingProgId { get; set; }
        public bool OnlyOnce { get; set; }
        public int Priority { get; set; }

        public virtual FutureProg CanDreamProg { get; set; }
        public virtual FutureProg OnDreamProg { get; set; }
        public virtual FutureProg OnWakeDuringDreamingProg { get; set; }
        public virtual ICollection<DreamPhase> DreamPhases { get; set; }
        public virtual ICollection<DreamsAlreadyDreamt> DreamsAlreadyDreamt { get; set; }
        public virtual ICollection<DreamsCharacters> DreamsCharacters { get; set; }
    }
}
