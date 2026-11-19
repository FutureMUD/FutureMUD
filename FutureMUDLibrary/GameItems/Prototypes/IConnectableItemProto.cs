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
        /// <summary>
        /// Physical connector definitions. Each entry has a persistent <see cref="ConnectorType.PortId"/>
        /// that must survive prototype edits and serialization; connection compatibility remains descriptor based.
        /// </summary>
        IEnumerable<ConnectorType> Connections { get; }
    }
}
