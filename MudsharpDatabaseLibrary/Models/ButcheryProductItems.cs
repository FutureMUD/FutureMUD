using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ButcheryProductItems
    {
        public long Id { get; set; }
        public long ButcheryProductId { get; set; }
        public long NormalProtoId { get; set; }
        public long? DamagedProtoId { get; set; }
        public int NormalQuantity { get; set; }
        public int DamagedQuantity { get; set; }
        public string ButcheryProductItemscol { get; set; }
        public double DamageThreshold { get; set; }

        public virtual ButcheryProducts ButcheryProduct { get; set; }
    }
}
