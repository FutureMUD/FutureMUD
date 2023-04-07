using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Dub
    {
        public long Id { get; set; }
        public string Keywords { get; set; }
        public long TargetId { get; set; }
        public string TargetType { get; set; }
        public string LastDescription { get; set; }
        public DateTime LastUsage { get; set; }
        public long CharacterId { get; set; }
        public string IntroducedName { get; set; }

        public virtual Character Character { get; set; }
    }
}
