using MudSharp.Form.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IGasSupply : IGameItemComponent, IConnectable
    {
        IGas Gas { get; }
        bool CanConsumeGas(double amount);
        bool ConsumeGas(double amount);
    }
}
