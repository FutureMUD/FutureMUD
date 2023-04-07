using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class WitnessProfile
    {
        public WitnessProfile()
        {
            WitnessProfilesCooperatingAuthorities = new HashSet<WitnessProfilesCooperatingAuthorities>();
            WitnessProfilesIgnoredCriminalClasses = new HashSet<WitnessProfilesIgnoredCriminalClasses>();
            WitnessProfilesIgnoredVictimClasses = new HashSet<WitnessProfilesIgnoredVictimClasses>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long IdentityKnownProgId { get; set; }
        public long ReportingMultiplierProgId { get; set; }
        public double ReportingReliability { get; set; }
        public double MinimumSkillToDetermineTimeOfDay { get; set; }
        public double MinimumSkillToDetermineBiases { get; set; }
        public double BaseReportingChanceNight { get; set; }
        public double BaseReportingChanceDawn { get; set; }
        public double BaseReportingChanceMorning { get; set; }
        public double BaseReportingChanceAfternoon { get; set; }
        public double BaseReportingChanceDusk { get; set; }

        public virtual FutureProg IdentityKnownProg { get; set; }
        public virtual FutureProg ReportingMultiplierProg { get; set; }
        public virtual ICollection<WitnessProfilesCooperatingAuthorities> WitnessProfilesCooperatingAuthorities { get; set; }
        public virtual ICollection<WitnessProfilesIgnoredCriminalClasses> WitnessProfilesIgnoredCriminalClasses { get; set; }
        public virtual ICollection<WitnessProfilesIgnoredVictimClasses> WitnessProfilesIgnoredVictimClasses { get; set; }
    }
}
