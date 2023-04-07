using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Magic
{
    [Flags]
    public enum MagicResourceType
    {
        None = 0,
        PlayerResource = 1,
        ItemResource = 2,
        LocationResource = 4
    }

    public interface IMagicResource : IFrameworkItem
    {
        MagicResourceType ResourceType { get; }
        bool ShouldStartWithResource(IHaveMagicResource thing);
        double StartingResourceAmount(IHaveMagicResource thing);
        double ResourceCap(IHaveMagicResource thing);
        string ShortName { get; }
        string ClassicPromptString(double percentage);
    }
}
