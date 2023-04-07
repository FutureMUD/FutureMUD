using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Wound
    {
        public Wound()
        {
            Infections = new HashSet<Infection>();
        }

        public long Id { get; set; }
        public long? BodyId { get; set; }
        public long? GameItemId { get; set; }
        public double OriginalDamage { get; set; }
        public double CurrentDamage { get; set; }
        public double CurrentPain { get; set; }
        public double CurrentShock { get; set; }
        public double CurrentStun { get; set; }
        public long? LodgedItemId { get; set; }
        public int DamageType { get; set; }
        public bool Internal { get; set; }
        public long? BodypartProtoId { get; set; }
        public string ExtraInformation { get; set; }
        public long? ActorOriginId { get; set; }
        public long? ToolOriginId { get; set; }
        public string WoundType { get; set; }

        public virtual Character ActorOrigin { get; set; }
        public virtual Body Body { get; set; }
        public virtual GameItem GameItem { get; set; }
        public virtual GameItem LodgedItem { get; set; }
        public virtual GameItem ToolOrigin { get; set; }
        public virtual ICollection<Infection> Infections { get; set; }
    }
}
