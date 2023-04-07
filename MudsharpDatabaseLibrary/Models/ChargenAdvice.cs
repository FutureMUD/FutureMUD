using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ChargenAdvice
    {
        public ChargenAdvice()
        {
            ChargenAdvicesChargenRoles = new HashSet<ChargenAdvicesChargenRoles>();
            ChargenAdvicesCultures = new HashSet<ChargenAdvicesCultures>();
            ChargenAdvicesEthnicities = new HashSet<ChargenAdvicesEthnicities>();
            ChargenAdvicesRaces = new HashSet<ChargenAdvicesRaces>();
        }

        public long Id { get; set; }
        public int ChargenStage { get; set; }
        public string AdviceTitle { get; set; }
        public string AdviceText { get; set; }
        public long? ShouldShowAdviceProgId { get; set; }

        public virtual FutureProg ShouldShowAdviceProg { get; set; }
        public virtual ICollection<ChargenAdvicesChargenRoles> ChargenAdvicesChargenRoles { get; set; }
        public virtual ICollection<ChargenAdvicesCultures> ChargenAdvicesCultures { get; set; }
        public virtual ICollection<ChargenAdvicesEthnicities> ChargenAdvicesEthnicities { get; set; }
        public virtual ICollection<ChargenAdvicesRaces> ChargenAdvicesRaces { get; set; }
    }
}
