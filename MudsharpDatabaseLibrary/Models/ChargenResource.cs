using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ChargenResource
    {
        public ChargenResource()
        {
            AccountsChargenResources = new HashSet<AccountsChargenResources>();
            ChargenRolesCosts = new HashSet<ChargenRolesCost>();
            CulturesChargenResources = new HashSet<CulturesChargenResources>();
            EthnicitiesChargenResources = new HashSet<EthnicitiesChargenResources>();
            MeritsChargenResources = new HashSet<MeritsChargenResources>();
            RacesChargenResources = new HashSet<RacesChargenResources>();
            TraitDefinitionsChargenResources = new HashSet<TraitDefinitionsChargenResources>();
            KnowledgesCosts = new HashSet<KnowledgesCosts>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string PluralName { get; set; }
        public string Alias { get; set; }
        public int MinimumTimeBetweenAwards { get; set; }
        public double MaximumNumberAwardedPerAward { get; set; }
        public int PermissionLevelRequiredToAward { get; set; }
        public int PermissionLevelRequiredToCircumventMinimumTime { get; set; }
        public bool ShowToPlayerInScore { get; set; }
        public string TextDisplayedToPlayerOnAward { get; set; }
        public string TextDisplayedToPlayerOnDeduct { get; set; }
        public long? MaximumResourceId { get; set; }
        public string MaximumResourceFormula { get; set; }
        public string Type { get; set; }

        public virtual ICollection<AccountsChargenResources> AccountsChargenResources { get; set; }
        public virtual ICollection<ChargenRolesCost> ChargenRolesCosts { get; set; }
        public virtual ICollection<CulturesChargenResources> CulturesChargenResources { get; set; }
        public virtual ICollection<EthnicitiesChargenResources> EthnicitiesChargenResources { get; set; }
        public virtual ICollection<MeritsChargenResources> MeritsChargenResources { get; set; }
        public virtual ICollection<RacesChargenResources> RacesChargenResources { get; set; }
        public virtual ICollection<TraitDefinitionsChargenResources> TraitDefinitionsChargenResources { get; set; }
        public virtual ICollection<KnowledgesCosts> KnowledgesCosts { get; set; }
    }
}
