using MudSharp.Construction.Grids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface ICanConnectToElectricalGrid : ICanConnectToGrid
    {
        IElectricalGrid ElectricalGrid { get; set; }
    }
}
