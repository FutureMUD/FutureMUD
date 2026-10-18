using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IImplantPowerPlant : IImplant
    {
        bool CanBeginDrawDown(double wattage);
        void BeginDrawdown(IConsumePower item);

        void EndDrawdown(IConsumePower item);

        bool CanDrawdownSpike(double wattage);

        bool DrawdownSpike(double wattage);

		/// <summary>Preserves discrete operation duration when routing implant power to a finite producer.</summary>
		bool CanDrawdownSpike(double wattage, TimeSpan duration) =>
			double.IsFinite(wattage) && wattage >= 0.0 && duration > TimeSpan.Zero && CanDrawdownSpike(wattage);

		bool DrawdownSpike(double wattage, TimeSpan duration) =>
			CanDrawdownSpike(wattage, duration) && DrawdownSpike(wattage);

        bool ProducingPower { get; }
        double MaximumPowerInWatts { get; }
    }
}
