using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Models
{
    public partial class ElectionVote
    {
        public ElectionVote()
        {

        }

        public long ElectionId { get; set; }
        public long VoterId { get; set; }
        public long VoterClanId { get; set; }
        public long NomineeId { get; set; }
        public long NomineeClanId { get; set; }
        public int NumberOfVotes { get; set; }

        public virtual Election Election { get; set; }
        public virtual ClanMembership Voter { get; set; }
        public virtual ClanMembership Nominee { get; set; }
    }
}
