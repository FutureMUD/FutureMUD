using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Communication.Language.DifficultyModels;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;

namespace MudSharp.Communication.Language
{
    public interface ILanguage : IEditableItem, IProgVariable
    {
        IEnumerable<IAccent> Accents { get; }
        ILanguageDifficultyModel Model { get; }
        ITraitDefinition LinkedTrait { get; }

        IAccent DefaultLearnerAccent { get; set; }

        /// <summary>
        ///     The string to display when a spoken language is not known to its perceiver, e.g. "an unknown tongue", or "a
        ///     glottal, face-paced language"
        /// </summary>
        string UnknownLanguageSpokenDescription { get; }

        /// <summary>
        ///     Provides a base multiplier in the Language Obfuscation code for failures to understand this language
        /// </summary>
        double LanguageObfuscationFactor { get; }

        Difficulty MutualIntelligability(ILanguage otherLanguage);
    }
}
