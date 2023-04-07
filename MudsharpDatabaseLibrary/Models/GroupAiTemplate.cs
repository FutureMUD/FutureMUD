using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class GroupAiTemplate
    {
        public GroupAiTemplate()
        {
            GroupAis = new HashSet<GroupAi>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }

        public virtual ICollection<GroupAi> GroupAis { get; set; }
    }
}
