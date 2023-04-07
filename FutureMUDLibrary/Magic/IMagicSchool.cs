using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Magic
{
    public interface IMagicSchool : IFrameworkItem, IFutureProgVariable, IHaveFuturemud
    {
        IMagicSchool ParentSchool { get; }

        /// <summary>
        /// The "Verb" used for the command to invoke this school, e.g. "psy", "magic", "invoke", etc
        /// </summary>
        string SchoolVerb { get; }

        /// <summary>
        /// The adjective used when talking about spells and powers of this school, e.g. psychic, magical, mutant, etc
        /// </summary>
        string SchoolAdjective { get; }

        ANSIColour PowerListColour { get; }
    }
}
