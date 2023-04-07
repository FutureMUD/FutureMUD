using System.Collections.Generic;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Construction {
    public interface ICellOverlay : IFrameworkItem, ISaveable {
        IFluid Atmosphere { get; }
        ICellOverlayPackage Package { get; }
        string CellName { get; }
        string CellDescription { get; }
        IEnumerable<long> ExitIDs { get; }
        ITerrain Terrain { get; }
        IHearingProfile HearingProfile { get; }
        ICell Cell { get; }
        CellOutdoorsType OutdoorsType { get; }

        /// <summary>
        ///     The Ambient Light Factor is a multiplier of the ambient (e.g. celestial) light reaching this location
        /// </summary>
        double AmbientLightFactor { get; }

        /// <summary>
        ///     The added light is a flat number of Lux added to the light levels at this location at any instant
        /// </summary>
        double AddedLight { get; }

        IEditableCellOverlay CreateClone(ICellOverlayPackage package);

        bool SafeQuit { get; }
    }
}