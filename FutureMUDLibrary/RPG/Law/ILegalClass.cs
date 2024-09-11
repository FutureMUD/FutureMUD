using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.FutureProg;

namespace MudSharp.RPG.Law
{
    public interface ILegalClass : IFrameworkItem, ISaveable, IEditableItem
    {
        ILegalAuthority Authority { get; }
        int LegalClassPriority { get; }
        bool CanBeDetainedUntilFinesPaid { get; }
        IFutureProg MembershipProg { get; }

        bool IsMemberOfClass(ICharacter actor);
        void Delete();
    }
}
