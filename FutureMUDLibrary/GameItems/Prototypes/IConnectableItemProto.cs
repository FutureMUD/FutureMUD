using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Prototypes
{
    public interface IConnectableItemProto : IGameItemComponentProto
    {
        IEnumerable<ConnectorType> Connections { get; }
    }
}
