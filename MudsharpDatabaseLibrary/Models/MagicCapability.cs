using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class MagicCapability
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string CapabilityModel { get; set; }
        public int PowerLevel { get; set; }
        public string Definition { get; set; }
        public long MagicSchoolId { get; set; }

        public virtual MagicSchool MagicSchool { get; set; }
    }
}
