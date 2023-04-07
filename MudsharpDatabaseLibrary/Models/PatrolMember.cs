using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
    public class PatrolMember
    {
        public long PatrolId { get; set; }
        public long CharacterId { get; set; }

        public virtual Patrol Patrol { get; set; }
        public virtual Character Character { get; set; }
    }
}
