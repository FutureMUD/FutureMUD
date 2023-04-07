using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Chargen
    {
        public Chargen()
        {
            Characters = new HashSet<Character>();
        }

        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public int Status { get; set; }
        public DateTime? SubmitTime { get; set; }
        public int? MinimumApprovalAuthority { get; set; }
        public long? ApprovedById { get; set; }
        public DateTime? ApprovalTime { get; set; }

        public virtual Account Account { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
    }
}
