using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class EntityDescriptions
    {
        public EntityDescriptions()
        {
            EntityDescriptionPatternsEntityDescriptions = new HashSet<EntityDescriptionPatternsEntityDescriptions>();
        }

        public long Id { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public short DisplaySex { get; set; }

        public virtual ICollection<EntityDescriptionPatternsEntityDescriptions> EntityDescriptionPatternsEntityDescriptions { get; set; }
    }
}
