using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class LanguageDifficultyModels
    {
        public LanguageDifficultyModels()
        {
            Languages = new HashSet<Language>();
        }

        public long Id { get; set; }
        public string Definition { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Language> Languages { get; set; }
    }
}
