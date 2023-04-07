using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CharactersChargenRoles
    {
        public long CharacterId { get; set; }
        public long ChargenRoleId { get; set; }

        public virtual Character Character { get; set; }
        public virtual ChargenRole ChargenRole { get; set; }
    }
}
