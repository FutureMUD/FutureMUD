using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Construction.Boundary {
    public interface INonCardinalExitTemplate : IFrameworkItem, ISaveable {
        string OriginOutboundPreface { get; set; }
        string OriginInboundPreface { get; set; }
        string DestinationOutboundPreface { get; set; }
        string DestinationInboundPreface { get; set; }
        string OutboundVerb { get; set; }
        string InboundVerb { get; set; }
    }
}