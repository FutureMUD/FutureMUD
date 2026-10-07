using MudSharp.Construction.Boundary;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Framework.Save;

namespace MudSharp.Construction
{
    public interface IEditableRoomOverlay : IRoomOverlay, ISaveable
    {
        new string RoomName { get; set; }
        new string RoomDescription { get; set; }
        new ITerrain Terrain { get; set; }
        new IHearingProfile HearingProfile { get; set; }
        new RoomOutdoorsType OutdoorsType { get; set; }
        new double AmbientLightFactor { get; set; }
        new double AddedLight { get; set; }
        void AddExit(IExit exit);
        void RemoveExit(IExit exit);
        new IFluid Atmosphere { get; set; }
        new bool SafeQuit { get; set; }
    }
}