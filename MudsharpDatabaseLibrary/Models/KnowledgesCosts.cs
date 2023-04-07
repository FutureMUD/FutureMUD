using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Models
{
    public class KnowledgesCosts
    {
        public KnowledgesCosts()
        {
        }

        public long KnowledgeId { get; set; }
        public long ChargenResourceId { get; set; }
        public int Cost { get; set; }

        public virtual Knowledge Knowledge { get; set; }
        public virtual ChargenResource ChargenResource { get; set; }
    }
}
