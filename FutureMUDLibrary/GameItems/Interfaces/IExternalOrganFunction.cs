using MudSharp.Body;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    /// <summary>
    /// Implementors of this interface provide organ function to a body when attached to it
    /// </summary>
    public interface IExternalOrganFunction : IConnectable
    {
        IEnumerable<(IOrganProto Organ, double Function)> OrganFunctions { get; }
    }
}
