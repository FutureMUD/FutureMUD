using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BloodModelsBloodtypes
    {
        public long BloodModelId { get; set; }
        public long BloodtypeId { get; set; }

        public virtual BloodModel BloodModel { get; set; }
        public virtual Bloodtype Bloodtype { get; set; }
    }
}
