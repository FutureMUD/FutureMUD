using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ChargenAdvicesChargenRoles
    {
        public long ChargenAdviceId { get; set; }
        public long ChargenRoleId { get; set; }

        public virtual ChargenAdvice ChargenAdvice { get; set; }
        public virtual ChargenRole ChargenRole { get; set; }
    }
}
