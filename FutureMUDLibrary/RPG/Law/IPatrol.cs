using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.RPG.Law
{

    public enum PatrolPhase
    {
        Preperation,
        Deployment,
        Patrol,
        Return
    }

    public interface IPatrol : IFrameworkItem, ISaveable
    {
        ILegalAuthority LegalAuthority { get; }
        IPatrolRoute PatrolRoute { get; }
        IPatrolStrategy PatrolStrategy { get; }
        IEnumerable<ICharacter> PatrolMembers { get; }
        ICharacter PatrolLeader { get; set; }
        PatrolPhase PatrolPhase { get; set; }
        ICell OriginLocation { get; }
        ICell LastMajorNode { get; set; }
        ICell NextMajorNode { get; set; }
        DateTime LastArrivedTime { get; set; }
        ICharacter ActiveEnforcementTarget { get; set; }
        ICrime ActiveEnforcementCrime { get; set; }

        void AbortPatrol();
        void CompletePatrol();
        void ConcludePatrol();

        void CriminalFailedToComply(ICharacter criminal, ICrime crime);
        void CriminalStartedMoving(ICharacter criminal, ICrime crime);
        void WarnCriminal(ICharacter criminal, ICrime crime);
        void InvalidateActiveCrime();
        void RemovePatrolMember(ICharacter character);
    }
}
