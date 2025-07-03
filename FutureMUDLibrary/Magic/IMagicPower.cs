using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.Magic
{
    public interface IMagicPower : IFrameworkItem, IEditableItem
    {
        IMagicSchool School { get; }

        string ShowHelp(ICharacter voyeur);
        void UseCommand(ICharacter actor, string verb, StringStack command);
        string Blurb { get; }
        IEnumerable<string> Verbs { get; }
        string PowerType { get; }

        /// <summary>
        /// True if this power is considered psionic rather than magical
        /// </summary>
        bool IsPsionic { get; }
    }
}
