using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ChargenRolesApprovers
    {
        public long ChargenRoleId { get; set; }
        public long ApproverId { get; set; }

        public virtual Account Approver { get; set; }
        public virtual ChargenRole ChargenRole { get; set; }
    }
}
