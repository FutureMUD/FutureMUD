using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BloodtypeAntigen
    {
        public BloodtypeAntigen()
        {
            BloodtypesBloodtypeAntigens = new HashSet<BloodtypesBloodtypeAntigens>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BloodtypesBloodtypeAntigens> BloodtypesBloodtypeAntigens { get; set; }
    }
}
