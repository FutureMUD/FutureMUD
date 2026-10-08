using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
    public class LegalAuthorityJailRoom
    {
        public long LegalAuthorityId { get; set; }
        public long RoomId { get; set; }

        public virtual LegalAuthority LegalAuthority { get; set; }
        public virtual Room Room { get; set; }
    }
}
