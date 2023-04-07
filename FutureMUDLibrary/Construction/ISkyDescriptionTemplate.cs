using MudSharp.Framework;

namespace MudSharp.Construction {
    public interface ISkyDescriptionTemplate : IFrameworkItem {
        RankedRange<string> SkyDescriptions { get; }
    }
}