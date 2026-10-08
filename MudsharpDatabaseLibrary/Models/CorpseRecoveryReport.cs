using System;

namespace MudSharp.Models
{
    public class CorpseRecoveryReport
    {
        public long Id { get; set; }
        public long LegalAuthorityId { get; set; }
        public long EconomicZoneId { get; set; }
        public long CorpseId { get; set; }
        public long SourceRoomId { get; set; }
        public long DestinationRoomId { get; set; }
        public long? ReporterId { get; set; }
        public long? AssignedPatrolId { get; set; }
        public int Status { get; set; }

        public virtual LegalAuthority LegalAuthority { get; set; }
        public virtual EconomicZone EconomicZone { get; set; }
        public virtual GameItem Corpse { get; set; }
        public virtual Room SourceRoom { get; set; }
        public virtual Room DestinationRoom { get; set; }
        public virtual Character Reporter { get; set; }
        public virtual Patrol AssignedPatrol { get; set; }
    }
}
