using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Appointment
    {
        public Appointment()
        {
            AppointmentsAbbreviations = new HashSet<AppointmentsAbbreviations>();
            AppointmentsTitles = new HashSet<AppointmentsTitles>();
            ClanMembershipsAppointments = new HashSet<ClanMembershipsAppointments>();
            ExternalClanControlsControlledAppointment = new HashSet<ExternalClanControl>();
            ExternalClanControlsControllingAppointment = new HashSet<ExternalClanControl>();
            InverseParentAppointment = new HashSet<Appointment>();
            Elections = new HashSet<Election>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int MaximumSimultaneousHolders { get; set; }
        public long? MinimumRankId { get; set; }
        public long? ParentAppointmentId { get; set; }
        public long? PaygradeId { get; set; }
        public long? InsigniaGameItemId { get; set; }
        public int? InsigniaGameItemRevnum { get; set; }
        public long ClanId { get; set; }
        public long Privileges { get; set; }
        public long? MinimumRankToAppointId { get; set; }
        public int FameType { get; set; }

        public bool IsAppointedByElection { get; set; }
        public double? ElectionTermMinutes { get; set; }
        public double? ElectionLeadTimeMinutes { get; set; }
        public double? NominationPeriodMinutes { get; set; }
        public double? VotingPeriodMinutes { get; set; }
        public int? MaximumConsecutiveTerms { get; set; }
        public int? MaximumTotalTerms { get; set; }
        public bool? IsSecretBallot { get; set; }
        public long? CanNominateProgId { get; set; }
        public long? WhyCantNominateProgId { get; set; }
        public long? NumberOfVotesProgId { get; set; }

        public virtual Clan Clan { get; set; }
        public virtual GameItemProto InsigniaGameItem { get; set; }
        public virtual Rank MinimumRank { get; set; }
        public virtual Rank MinimumRankToAppoint { get; set; }
        public virtual Appointment ParentAppointment { get; set; }
        public virtual Paygrade Paygrade { get; set; }
        public virtual FutureProg CanNominateProg { get; set; }
        public virtual FutureProg NumberOfVotesProg { get; set; }
        public virtual FutureProg WhyCantNominateProg { get; set; }
        public virtual ICollection<AppointmentsAbbreviations> AppointmentsAbbreviations { get; set; }
        public virtual ICollection<AppointmentsTitles> AppointmentsTitles { get; set; }
        public virtual ICollection<ClanMembershipsAppointments> ClanMembershipsAppointments { get; set; }
        public virtual ICollection<ExternalClanControl> ExternalClanControlsControlledAppointment { get; set; }
        public virtual ICollection<ExternalClanControl> ExternalClanControlsControllingAppointment { get; set; }
        public virtual ICollection<Appointment> InverseParentAppointment { get; set; }
        public virtual ICollection<Election> Elections { get; set; }
    }
}
