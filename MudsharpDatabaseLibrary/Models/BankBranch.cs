using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
    public class BankBranch
    {
        public long BankId { get; set; }
        public long RoomId { get; set; }

        public virtual Bank Bank { get; set; }
        public virtual Room Room { get; set; }
    }
}
