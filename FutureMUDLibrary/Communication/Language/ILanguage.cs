using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Communication.Language.DifficultyModels;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Communication.Language
{
    public interface ILanguage : ICommunicationLanguage
    {
        IEnumerable<IAccent> Accents { get; }
        IAccent DefaultLearnerAccent { get; set; }

        /// <summary>
        ///     The string to display when a spoken language is not known to its perceiver, e.g. "an unknown tongue", or "a
        ///     glottal, face-paced language"
        /// </summary>
        string UnknownLanguageSpokenDescription { get; }

		string ICommunicationLanguage.UnknownLanguageDescription => UnknownLanguageSpokenDescription;

        Difficulty MutualIntelligability(ILanguage otherLanguage);
    }
}
