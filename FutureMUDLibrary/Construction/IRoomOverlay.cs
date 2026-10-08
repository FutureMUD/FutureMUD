using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System.Collections.Generic;

namespace MudSharp.Construction
{
    public interface IRoomOverlay : IFrameworkItem, ISaveable
    {
        IFluid Atmosphere { get; }
        IRoomOverlayPackage Package { get; }
        string RoomName { get; }
        string RoomDescription { get; }
        IEnumerable<long> ExitIDs { get; }
        ITerrain Terrain { get; }
        IHearingProfile HearingProfile { get; }
        IRoom Room { get; }
        RoomOutdoorsType OutdoorsType { get; }

        /// <summary>
        ///     The Ambient Light Factor is a multiplier of the ambient (e.g. celestial) light reaching this location
        /// </summary>
        double AmbientLightFactor { get; }

        /// <summary>
        ///     The added light is a flat number of Lux added to the light levels at this location at any instant
        /// </summary>
        double AddedLight { get; }

        IEditableRoomOverlay CreateClone(IRoomOverlayPackage package);

        bool SafeQuit { get; }
    }
}