using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Script
    {
        public Script()
        {
            Characters = new HashSet<Character>();
            CharactersScripts = new HashSet<CharactersScripts>();
            ScriptsDesignedLanguages = new HashSet<ScriptsDesignedLanguage>();
            Writings = new HashSet<Writing>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string KnownScriptDescription { get; set; }
        public string UnknownScriptDescription { get; set; }
        public long KnowledgeId { get; set; }
        public double DocumentLengthModifier { get; set; }
        public double InkUseModifier { get; set; }

        public virtual Knowledge Knowledge { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
        public virtual ICollection<CharactersScripts> CharactersScripts { get; set; }
        public virtual ICollection<ScriptsDesignedLanguage> ScriptsDesignedLanguages { get; set; }
        public virtual ICollection<Writing> Writings { get; set; }
    }
}
