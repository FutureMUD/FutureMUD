using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class LoginIp
    {
        public string IpAddress { get; set; }
        public long AccountId { get; set; }
        public DateTime FirstDate { get; set; }
        public bool AccountRegisteredOnThisIp { get; set; }

        public virtual Account Account { get; set; }
    }
}
