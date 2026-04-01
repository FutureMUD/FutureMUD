using MudSharp.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IProduceHeat : IGameItemComponent
    {
        double HeatProduced(Proximity proximity);
    }
}
