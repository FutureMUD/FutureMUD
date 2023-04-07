using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Room
    {
        public Room()
        {
            AreasRooms = new HashSet<AreasRooms>();
            Cells = new HashSet<Cell>();
        }

        public long Id { get; set; }
        public long ZoneId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public virtual Zone Zone { get; set; }
        public virtual ICollection<AreasRooms> AreasRooms { get; set; }
        public virtual ICollection<Cell> Cells { get; set; }
    }
}
