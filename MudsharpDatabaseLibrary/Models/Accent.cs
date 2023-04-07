using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Accent
    {
        public Accent()
        {
            Characters = new HashSet<Character>();
            CharactersAccents = new HashSet<CharacterAccent>();
            Languages = new HashSet<Language>();
        }

        public long Id { get; set; }
        public long LanguageId { get; set; }
        public string Name { get; set; }
        public string Suffix { get; set; }
        public string VagueSuffix { get; set; }
        public int Difficulty { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public long? ChargenAvailabilityProgId { get; set; }

        public virtual Language Language { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
        public virtual ICollection<CharacterAccent> CharactersAccents { get; set; }
        public virtual ICollection<Language> Languages { get; set; }
    }
}
