using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
    public class BodyProtosPositions
    {
        public long BodyProtoId { get; set; }
        public int Position { get; set; }

        public virtual BodyProto BodyProto { get; set; }
    }
}
