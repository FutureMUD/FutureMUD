using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ChargenAdvicesEthnicities
    {
        public long ChargenAdviceId { get; set; }
        public long EthnicityId { get; set; }

        public virtual ChargenAdvice ChargenAdvice { get; set; }
        public virtual Ethnicity Ethnicity { get; set; }
    }
}
