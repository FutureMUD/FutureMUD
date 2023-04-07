using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Models
{
    public partial class PatrolRouteNode
    {
        public PatrolRouteNode()
        {
        }

        public long PatrolRouteId { get; set; }
        public long CellId { get; set; }
        public int Order { get; set; }

        public virtual PatrolRoute PatrolRoute { get; set; }
        public virtual Cell Cell { get; set; }
    }
}
