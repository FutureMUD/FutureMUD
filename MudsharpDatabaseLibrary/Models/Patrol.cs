using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MudSharp.Models
{
    public class Patrol
    {
        public Patrol()
        {
            PatrolMembers = new HashSet<PatrolMember>();
        }

        public long Id { get;set; }
        public long PatrolRouteId { get; set; }
        public virtual PatrolRoute PatrolRoute { get; set; }
        public long LegalAuthorityId { get; set; }
        public virtual LegalAuthority LegalAuthority { get; set; }
        public int PatrolPhase { get; set; }
        public long? LastMajorNodeId { get; set; }
        [CanBeNull] public virtual Cell LastMajorNode { get; set; }
        public long? NextMajorNodeId { get; set; }
        [CanBeNull] public virtual Cell NextMajorNode { get; set; }
        public long? PatrolLeaderId { get; set; }
        [CanBeNull] public virtual Character PatrolLeader { get; set; }

        public virtual ICollection<PatrolMember> PatrolMembers { get; set; }
    }
}
