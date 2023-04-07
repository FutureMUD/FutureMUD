using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class WearProfile
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long BodyPrototypeId { get; set; }
        public string WearStringInventory { get; set; }
        public string WearAction1st { get; set; }
        public string WearAction3rd { get; set; }
        public string WearAffix { get; set; }
        public string WearlocProfiles { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool RequireContainerIsEmpty { get; set; }
    }
}
