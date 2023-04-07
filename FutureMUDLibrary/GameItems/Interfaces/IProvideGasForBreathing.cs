using MudSharp.Form.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IProvideGasForBreathing : IGameItemComponent
    {
        IGas Gas { get; }

        /// <summary>
        /// Tests whether a certain weight of gas is able to be consumed from the gas source
        /// </summary>
        /// <param name="volume">The volume (at sea level) of the gas to consume</param>
        /// <returns>True if the gas can be consumed</returns>
        bool CanConsumeGas(double volume);

        /// <summary>
        /// Consumes gas from the breathing device
        /// </summary>
        /// <param name="volume">The volume (at sea level) of the gas to consume</param>
        /// <returns>True if the gas is not empty after this consumption</returns>
        bool ConsumeGas(double volume);

        bool WaterTight { get; }
    }
}
