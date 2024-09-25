using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Crime
    {
        public long Id { get; set; }
        public long LawId { get; set; }
        public long CriminalId { get; set; }
        public long? VictimId { get; set; }
        public string TimeOfCrime { get; set; }
        public DateTime RealTimeOfCrime { get; set; }
        public long? LocationId { get; set; }
        public string TimeOfReport { get; set; }
        public long? AccuserId { get; set; }
        public long? ThirdPartyId { get; set; }
        public string? ThirdPartyIItemType { get; set; }
        public string CriminalShortDescription { get; set; }
        public string CriminalFullDescription { get; set; }
        public string CriminalCharacteristics { get; set; }
        public bool IsKnownCrime { get; set; }
        public bool IsStaleCrime { get; set; }
        public bool IsFinalised { get; set; }
        public bool ConvictionRecorded { get; set; }
        public bool IsCriminalIdentityKnown { get; set; }
        public bool BailHasBeenPosted { get; set; }
        public bool HasBeenEnforced { get; set; }
        public decimal CalculatedBail { get; set; }
        public decimal FineRecorded { get; set; }
        public double CustodialSentenceLength { get; set; }
        public string WitnessIds { get; set; }
        public bool ExecutionPunishment { get; set; }
        public bool FineHasBeenPaid { get; set; }
        public bool SentenceHasBeenServed { get; set; }
        public double GoodBehaviourBond { get; set; }

        public virtual Character Accuser { get; set; }
        public virtual Character Criminal { get; set; }
        public virtual Law Law { get; set; }
        public virtual Cell Location { get; set; }
        public virtual Character Victim { get; set; }
    }
}
