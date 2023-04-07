using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Models
{
    public class LineOfCreditAccount
    {
        public LineOfCreditAccount()
        {
            AccountUsers = new HashSet<LineOfCreditAccountUser>();
        }

        public long Id { get; set; }
        public string AccountName { get; set; }
        public long ShopId { get; set; }
        public virtual Shop Shop { get; set; }
        public bool IsSuspended { get; set; }
        public decimal AccountLimit { get; set; }
        public decimal OutstandingBalance { get; set; }
        public long AccountOwnerId { get; set; }
        public string AccountOwnerName { get; set; }
        public virtual Character AccountOwner { get; set; }

        public virtual ICollection<LineOfCreditAccountUser> AccountUsers { get; set; }
    }
}
