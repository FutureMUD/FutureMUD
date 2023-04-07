using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BloodtypesBloodtypeAntigens
    {
        public long BloodtypeId { get; set; }
        public long BloodtypeAntigenId { get; set; }

        public virtual Bloodtype Bloodtype { get; set; }
        public virtual BloodtypeAntigen BloodtypeAntigen { get; set; }
    }
}
