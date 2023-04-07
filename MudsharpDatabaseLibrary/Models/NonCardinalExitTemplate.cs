using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class NonCardinalExitTemplate
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string OriginOutboundPreface { get; set; }
        public string OriginInboundPreface { get; set; }
        public string DestinationOutboundPreface { get; set; }
        public string DestinationInboundPreface { get; set; }
        public string OutboundVerb { get; set; }
        public string InboundVerb { get; set; }
    }
}
