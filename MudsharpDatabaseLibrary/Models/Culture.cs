using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Culture
    {
        public Culture()
        {
            Characters = new HashSet<Character>();
            ChargenAdvicesCultures = new HashSet<ChargenAdvicesCultures>();
            CulturesChargenResources = new HashSet<CulturesChargenResources>();
            CulturesNameCultures = new HashSet<CulturesNameCultures>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PersonWordMale { get; set; }
        public string PersonWordFemale { get; set; }
        public string PersonWordNeuter { get; set; }
        public string PersonWordIndeterminate { get; set; }
        public long PrimaryCalendarId { get; set; }
        public long SkillStartingValueProgId { get; set; }
        public long? AvailabilityProgId { get; set; }
        public double TolerableTemperatureFloorEffect { get; set; }
        public double TolerableTemperatureCeilingEffect { get; set; }

        public virtual FutureProg AvailabilityProg { get; set; }
        public virtual FutureProg SkillStartingValueProg { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
        public virtual ICollection<ChargenAdvicesCultures> ChargenAdvicesCultures { get; set; }
        public virtual ICollection<CulturesChargenResources> CulturesChargenResources { get; set; }
        public virtual ICollection<CulturesNameCultures> CulturesNameCultures { get; set; }
    }
}
