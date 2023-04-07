using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ButcheryProducts
    {
        public ButcheryProducts()
        {
            ButcheryProductItems = new HashSet<ButcheryProductItems>();
            ButcheryProductsBodypartProtos = new HashSet<ButcheryProductsBodypartProtos>();
            RaceButcheryProfilesButcheryProducts = new HashSet<RaceButcheryProfilesButcheryProducts>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long TargetBodyId { get; set; }
        public bool IsPelt { get; set; }
        public string Subcategory { get; set; }
        public long? CanProduceProgId { get; set; }

        public virtual FutureProg CanProduceProg { get; set; }
        public virtual BodyProto TargetBody { get; set; }
        public virtual ICollection<ButcheryProductItems> ButcheryProductItems { get; set; }
        public virtual ICollection<ButcheryProductsBodypartProtos> ButcheryProductsBodypartProtos { get; set; }
        public virtual ICollection<RaceButcheryProfilesButcheryProducts> RaceButcheryProfilesButcheryProducts { get; set; }
    }
}
