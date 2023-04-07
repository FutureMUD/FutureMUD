using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CorpseModel
    {
        public CorpseModel()
        {
            Races = new HashSet<Race>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Definition { get; set; }
        public string Type { get; set; }

        public virtual ICollection<Race> Races { get; set; }
    }
}
