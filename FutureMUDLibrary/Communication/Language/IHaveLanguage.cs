using System.Collections.Generic;
using MudSharp.Body.Traits;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Communication.Language {
    public interface IHaveLanguage : IHaveTraits {
        IEnumerable<ILanguage> Languages { get; }
        ILanguage CurrentLanguage { get; set; }
        ILanguage CurrentWritingLanguage { get; set; }

        IEnumerable<IAccent> Accents { get; }
        IAccent CurrentAccent { get; set; }
        Difficulty AccentDifficulty(IAccent accent, bool canImprove = true);
        void SetPreferredAccent(IAccent accent);
        IAccent PreferredAccent(ILanguage language);

        IEnumerable<IScript> Scripts { get; }
        IScript CurrentScript { get; set; }
        WritingStyleDescriptors WritingStyle { get; set; }

        void LearnLanguage(ILanguage language);
        void LearnAccent(IAccent accent, Difficulty difficulty = Difficulty.Normal);
        void LearnScript(IScript script);
        void ForgetLanguage(ILanguage language);
        void ForgetAccent(IAccent accent);
        void ForgetScript(IScript script);
        bool IsLiterate { get; }
        bool CanRead(IWriting writing);
        string WhyCannotRead(IWriting writing);
        string GetWritingHeader(IWriting writing);
        bool Read(IWriting writing);
        bool CanWrite();
    }

    public interface ILanguagePerceiver : IPerceiver, IPerceivableHaveTraits, IHaveLanguage
    {
	    bool CanIdentifyLanguage(ILanguage language);
    }
}