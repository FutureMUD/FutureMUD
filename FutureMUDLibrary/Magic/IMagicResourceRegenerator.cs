using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Magic
{
    public interface IMagicResourceRegenerator : IFrameworkItem, IEditableItem
    {
        string RegeneratorTypeName { get; }
        HeartbeatManagerDelegate GetOnMinuteDelegate(IHaveMagicResource thing);
        IEnumerable<IMagicResource> GeneratedResources { get; }
        IMagicResourceRegenerator Clone(string newName);
    }
}
