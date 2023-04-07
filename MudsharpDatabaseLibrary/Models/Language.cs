using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Language
    {
        public Language()
        {
            Accents = new HashSet<Accent>();
            CharactersCurrentLanguage = new HashSet<Character>();
            CharactersCurrentWritingLanguage = new HashSet<Character>();
            CharactersLanguages = new HashSet<CharactersLanguages>();
            MutualIntelligabilitiesListenerLanguage = new HashSet<MutualIntelligability>();
            MutualIntelligabilitiesTargetLanguage = new HashSet<MutualIntelligability>();
            ScriptsDesignedLanguages = new HashSet<ScriptsDesignedLanguage>();
            Writings = new HashSet<Writing>();
        }

        public long Id { get; set; }
        public long DifficultyModel { get; set; }
        public long LinkedTraitId { get; set; }
        public string UnknownLanguageDescription { get; set; }
        public double LanguageObfuscationFactor { get; set; }
        public string Name { get; set; }
        public long? DefaultLearnerAccentId { get; set; }

        public virtual Accent DefaultLearnerAccent { get; set; }
        public virtual LanguageDifficultyModels DifficultyModelNavigation { get; set; }
        public virtual TraitDefinition LinkedTrait { get; set; }
        public virtual ICollection<Accent> Accents { get; set; }
        public virtual ICollection<Character> CharactersCurrentLanguage { get; set; }
        public virtual ICollection<Character> CharactersCurrentWritingLanguage { get; set; }
        public virtual ICollection<CharactersLanguages> CharactersLanguages { get; set; }
        public virtual ICollection<MutualIntelligability> MutualIntelligabilitiesListenerLanguage { get; set; }
        public virtual ICollection<MutualIntelligability> MutualIntelligabilitiesTargetLanguage { get; set; }
        public virtual ICollection<ScriptsDesignedLanguage> ScriptsDesignedLanguages { get; set; }
        public virtual ICollection<Writing> Writings { get; set; }
    }
}
