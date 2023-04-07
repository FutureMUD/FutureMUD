using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodiesImplants
    {
        public long BodyId { get; set; }
        public long ImplantId { get; set; }

        public virtual Body Body { get; set; }
        public virtual GameItem Implant { get; set; }
    }
}
