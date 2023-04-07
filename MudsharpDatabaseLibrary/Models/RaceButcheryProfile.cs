using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RaceButcheryProfile
    {
        public RaceButcheryProfile()
        {
            RaceButcheryProfilesBreakdownChecks = new HashSet<RaceButcheryProfilesBreakdownChecks>();
            RaceButcheryProfilesBreakdownEmotes = new HashSet<RaceButcheryProfilesBreakdownEmotes>();
            RaceButcheryProfilesButcheryProducts = new HashSet<RaceButcheryProfilesButcheryProducts>();
            RaceButcheryProfilesSkinningEmotes = new HashSet<RaceButcheryProfilesSkinningEmotes>();
            Races = new HashSet<Race>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int Verb { get; set; }
        public long? RequiredToolTagId { get; set; }
        public int DifficultySkin { get; set; }
        public long? CanButcherProgId { get; set; }
        public long? WhyCannotButcherProgId { get; set; }

        public virtual FutureProg CanButcherProg { get; set; }
        public virtual Tag RequiredToolTag { get; set; }
        public virtual FutureProg WhyCannotButcherProg { get; set; }
        public virtual ICollection<RaceButcheryProfilesBreakdownChecks> RaceButcheryProfilesBreakdownChecks { get; set; }
        public virtual ICollection<RaceButcheryProfilesBreakdownEmotes> RaceButcheryProfilesBreakdownEmotes { get; set; }
        public virtual ICollection<RaceButcheryProfilesButcheryProducts> RaceButcheryProfilesButcheryProducts { get; set; }
        public virtual ICollection<RaceButcheryProfilesSkinningEmotes> RaceButcheryProfilesSkinningEmotes { get; set; }
        public virtual ICollection<Race> Races { get; set; }
    }
}
