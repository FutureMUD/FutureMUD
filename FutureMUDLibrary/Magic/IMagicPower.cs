using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Magic
{
    public interface IMagicPower : IFrameworkItem
    {
        IMagicSchool School { get; }

        string ShowHelp(ICharacter voyeur);
        void UseCommand(ICharacter actor, string verb, StringStack command);
        string Blurb { get; }
        IEnumerable<string> Verbs { get; }
    }
}
