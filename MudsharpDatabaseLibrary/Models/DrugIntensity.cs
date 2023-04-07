using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class DrugIntensity
    {
        public long DrugId { get; set; }
        public int DrugType { get; set; }
        public double RelativeIntensity { get; set; }
        public string AdditionalEffects { get; set; }

        public virtual Drug Drug { get; set; }
    }
}
