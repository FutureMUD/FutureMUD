using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
    public class LegalAuthorityFine
    {
        public long LegalAuthorityId { get; set; }
        public long CharacterId { get; set; }
        public decimal FinesOwned { get; set; }
        public string PaymentRequiredBy { get; set; }

        public virtual LegalAuthority LegalAuthority { get; set; }
        public virtual Character Character { get; set; }
    }
}
