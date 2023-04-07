using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodiesProsthetics
    {
        public long BodyId { get; set; }
        public long ProstheticId { get; set; }

        public virtual Body Body { get; set; }
        public virtual GameItem Prosthetic { get; set; }
    }
}
