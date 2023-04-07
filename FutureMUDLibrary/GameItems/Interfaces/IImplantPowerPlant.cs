using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IImplantPowerPlant : IImplant {
        bool CanBeginDrawDown(double wattage);
        void BeginDrawdown(IConsumePower item);

        void EndDrawdown(IConsumePower item);

        bool CanDrawdownSpike(double wattage);

        bool DrawdownSpike(double wattage);

        bool ProducingPower { get; }
        double MaximumPowerInWatts { get; }
    }
}
