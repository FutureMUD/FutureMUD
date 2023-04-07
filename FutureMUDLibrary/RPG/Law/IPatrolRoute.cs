using MudSharp.Celestial;
using MudSharp.Construction;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework.Revision;

namespace MudSharp.RPG.Law
{
    public interface IPatrolRoute : IEditableItem
    {
        IEnumerable<ICell> PatrolNodes { get; }
        Counter<IEnforcementAuthority> PatrollerNumbers { get; }
        IEnumerable<TimeOfDay> TimeOfDays { get; }
        TimeSpan LingerTimeMajorNode { get; }
        TimeSpan LingerTimeMinorNode { get; }
        int Priority { get; }
        void Delete();
        IPatrolStrategy PatrolStrategy { get; }
        bool IsReady { get; }
        bool ShouldBeginPatrol();
    }
}
