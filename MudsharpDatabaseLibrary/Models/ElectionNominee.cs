using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Models
{
    public partial class ElectionNominee
    {
        public ElectionNominee()
        {

        }

        public long ElectionId { get; set; }
        public long NomineeId { get; set; }
        public long NomineeClanId { get; set; }

        public virtual Election Election { get; set; }
        public virtual ClanMembership Nominee { get; set; }
    }
}
