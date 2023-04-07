using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Construction.Grids
{
    public interface IElectricalGrid : IGrid
    {
        double TotalSupply { get; }
        double TotalDrawdown { get; }
        void JoinGrid(IConsumePower consumer);
        void LeaveGrid(IConsumePower consumer);
        void JoinGrid(IProducePower producer);
        void LeaveGrid(IProducePower producer);
        void RecalculateGrid();
        bool DrawdownSpike(double wattage);
    }
}
