using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
    public class NPCSpawnerRoom
    {
        public long NPCSpawnerId { get; set; }
        public long RoomId { get; set; }

        public virtual NPCSpawner NPCSpawner { get; set; }
        public virtual Room Room { get; set; }
    }
}
