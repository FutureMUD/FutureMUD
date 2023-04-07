using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Law
{
    public interface ILaw : IFrameworkItem, ISaveable, IEditableItem, IFutureProgVariable
    {
        ILegalAuthority Authority { get; }
        CrimeTypes CrimeType { get; }
        bool CanBeAppliedAutomatically { get; }
        bool DoNotAutomaticallyApplyRepeats { get; }
        IEnumerable<ILegalClass> VictimClasses { get; }
        IEnumerable<ILegalClass> OffenderClasses { get; }
        bool CanBeArrested { get; }
        bool CanBeOfferedBail { get; }
        TimeSpan ActivePeriod { get; }
        int EnforcementPriority { get; }
        EnforcementStrategy EnforcementStrategy { get; }
        IPunishmentStrategy PunishmentStrategy { get; }
        void Delete();
        void RemoveAllReferencesTo(ILegalClass legalClass);
        void ApplyInflation(decimal rate);
        bool IsCrime(ICharacter criminal, ICharacter victim, IGameItem item);
    }
}
