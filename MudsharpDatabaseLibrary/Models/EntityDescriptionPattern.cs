using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class EntityDescriptionPattern
    {
        public EntityDescriptionPattern()
        {
            BodiesFullDescriptionPattern = new HashSet<Body>();
            BodiesShortDescriptionPattern = new HashSet<Body>();
            EntityDescriptionPatternsEntityDescriptions = new HashSet<EntityDescriptionPatternsEntityDescriptions>();
        }

        public long Id { get; set; }
        public string Pattern { get; set; }
        public int Type { get; set; }
        public long? ApplicabilityProgId { get; set; }
        public int RelativeWeight { get; set; }

        public virtual FutureProg ApplicabilityProg { get; set; }
        public virtual ICollection<Body> BodiesFullDescriptionPattern { get; set; }
        public virtual ICollection<Body> BodiesShortDescriptionPattern { get; set; }
        public virtual ICollection<EntityDescriptionPatternsEntityDescriptions> EntityDescriptionPatternsEntityDescriptions { get; set; }
    }
}
