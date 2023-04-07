using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodyDrugDose
    {
        public long BodyId { get; set; }
        public long DrugId { get; set; }
        public double Grams { get; set; }
        public int OriginalVector { get; set; }
        public bool Active { get; set; }

        public virtual Body Body { get; set; }
        public virtual Drug Drug { get; set; }
    }
}
