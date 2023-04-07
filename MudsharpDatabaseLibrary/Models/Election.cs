using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Models
{
    public partial class Election
    {
        public Election()
        {
            ElectionNominees = new HashSet<ElectionNominee>();
            ElectionVotes = new HashSet<ElectionVote>();
        }

        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public string NominationStartDate { get; set; }
        public string VotingStartDate { get; set; }
        public string VotingEndDate { get; set; }
        public string ResultsInEffectDate { get; set; }
        public bool IsFinalised { get; set; }
        public int NumberOfAppointments { get; set; }
        public bool IsByElection { get; set; }
        public int ElectionStage { get; set; }

        public virtual Appointment Appointment { get; set; }
        public virtual ICollection<ElectionNominee> ElectionNominees { get; set; }
        public virtual ICollection<ElectionVote> ElectionVotes { get; set; }
    }
}
