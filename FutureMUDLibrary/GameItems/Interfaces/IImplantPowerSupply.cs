using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IImplantPowerSupply : IProducePower
    {
        IImplantPowerPlant PowerPlant { get; set; }
    }
}
