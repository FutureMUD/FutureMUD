using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class MagicPower
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Blurb { get; set; }
        public string ShowHelp { get; set; }
        public string PowerModel { get; set; }
        public string Definition { get; set; }
        public long MagicSchoolId { get; set; }

        public virtual MagicSchool MagicSchool { get; set; }
    }
}
