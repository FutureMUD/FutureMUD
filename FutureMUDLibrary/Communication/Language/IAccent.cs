using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.CharacterCreation;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;

namespace MudSharp.Communication.Language
{
    public interface IAccent : IEditableItem, IFutureProgVariable
    {
        string AccentSuffix { get; }

        /// <summary>
        ///     The suffix added to the language string in vocal echoes when the listener is unfamiliar with the accent, of a form
        ///     such as "with an american accent"
        /// </summary>
        string VagueSuffix { get; }


        string Description { get; }

        /// <summary>
        ///     The accent group to which this accent belongs, such as American, English, Australian. Speakers of related accents
        ///     have an easier time understanding it.
        /// </summary>
        string Group { get; }

        ILanguage Language { get; }

        /// <summary>
        ///     The difficulty of understanding this accent if it is one with which you are unfamiliar
        /// </summary>
        Difficulty Difficulty { get; }

        IFutureProg ChargenAvailabilityProg { get;  }
        bool IsAvailableInChargen(ICharacterTemplate template);
    }
}
