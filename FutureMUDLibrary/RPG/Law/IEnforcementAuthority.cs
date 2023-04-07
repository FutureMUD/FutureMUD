using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Law
{
    public interface IEnforcementAuthority : IFrameworkItem, ISaveable, IEditableItem
    {
        IEnumerable<ILegalClass> ArrestableClasses { get; }
        IEnumerable<ILegalClass> AccusableClasses { get; }
        bool CanAccuse { get; }
        bool CanForgive { get; }
        bool CanConvict { get; }
        bool AlsoIncludesAuthorityFrom(IEnforcementAuthority otherAuthority);
        IEnumerable<IEnforcementAuthority> IncludedAuthorities { get; }
        IEnumerable<IEnforcementAuthority> AllIncludedAuthorities { get; }
        void Delete();
        void RemoveAllReferencesTo(ILegalClass legalClass);
        int Priority { get; }
        bool HasAuthority(ICharacter actor);
    }
}
