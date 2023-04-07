using MudSharp.Form.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IGasContainer : IGameItemComponent
    {
        IGas Gas { get; set; }
        double GasVolumeAtOneAtmosphere { get; set; }
        double GasCapacityAtOneAtmosphere { get; }
    }
}
