using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Lock
    {
        public Lock()
        {
            Doors = new HashSet<Door>();
        }

        public string Name { get; set; }
        public long Id { get; set; }
        public int Style { get; set; }
        public int Strength { get; set; }

        public virtual ICollection<Door> Doors { get; set; }
    }
}
