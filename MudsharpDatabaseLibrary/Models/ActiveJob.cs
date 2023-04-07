using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Models
{
    public class ActiveJob
    {
        public long Id { get; set; }
        public long JobListingId { get; set; }
        public long CharacterId { get; set; }
        public string JobCommenced { get; set; }
        public string JobDueToEnd { get; set; }
        public string JobEnded { get; set; }
        public bool IsJobComplete { get; set; }
        public bool AlreadyHadClanPosition { get; set; }
        public string BackpayOwed { get; set; }
        public string RevenueEarned { get; set; }
        public double CurrentPerformance { get; set; }
        public long? ActiveProjectId { get; set; }

        public virtual JobListing JobListing { get; set; }
        public virtual Character Character { get; set; }
        public virtual ActiveProject ActiveProject { get; set; }
    }
}
