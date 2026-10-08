using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
    public class PropertyLocation
    {
        public long PropertyId { get; set; }
        public long RoomId { get; set; }

        public virtual Property Property { get; set; }
        public virtual Room Room { get; set; }
    }
}
