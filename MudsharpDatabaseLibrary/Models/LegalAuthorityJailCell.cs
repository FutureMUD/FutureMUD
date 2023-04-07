using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
    public class LegalAuthorityJailCell
    {
        public long LegalAuthorityId { get; set; }
        public long CellId { get; set; }

        public virtual LegalAuthority LegalAuthority { get; set; }
        public virtual Cell Cell { get; set; }
    }
}
