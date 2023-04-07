using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Ban
    {
        public long Id { get; set; }
        public string IpMask { get; set; }
        public long? BannerAccountId { get; set; }
        public string Reason { get; set; }
        public DateTime? Expiry { get; set; }

        public virtual Account BannerAccount { get; set; }
    }
}
