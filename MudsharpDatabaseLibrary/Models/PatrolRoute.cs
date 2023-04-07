using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace MudSharp.Models
{
    public partial class PatrolRoute
    {
        public PatrolRoute()
        {
            TimesOfDay = new HashSet<PatrolRouteTimeOfDay>();
            PatrolRouteNodes = new HashSet<PatrolRouteNode>();
            PatrolRouteNumbers = new HashSet<PatrolRouteNumbers>();
            Patrols = new HashSet<Patrol>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long LegalAuthorityId { get; set; }
        public double LingerTimeMajorNode { get; set; }
        public double LingerTimeMinorNode { get; set; }
        public int Priority { get; set; }
        public string PatrolStrategy { get; set; }
        public long? StartPatrolProgId { get; set; }
        public bool IsReady { get; set; }

        public virtual LegalAuthority LegalAuthority { get; set; }
        [CanBeNull] public virtual FutureProg StartPatrolProg { get; set; }
        public virtual ICollection<PatrolRouteTimeOfDay> TimesOfDay { get; set; }
        public virtual ICollection<PatrolRouteNode> PatrolRouteNodes { get; set; }
        public virtual ICollection<PatrolRouteNumbers> PatrolRouteNumbers { get; set; }
        public virtual ICollection<Patrol> Patrols { get; set; }
    }
}
