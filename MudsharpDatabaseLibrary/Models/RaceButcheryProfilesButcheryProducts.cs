using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RaceButcheryProfilesButcheryProducts
    {
        public long RaceButcheryProfileId { get; set; }
        public long ButcheryProductId { get; set; }

        public virtual ButcheryProducts ButcheryProduct { get; set; }
        public virtual RaceButcheryProfile RaceButcheryProfile { get; set; }
    }
}
