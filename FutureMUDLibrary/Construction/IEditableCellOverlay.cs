using MudSharp.Construction.Boundary;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Framework.Save;

namespace MudSharp.Construction {
    public interface IEditableCellOverlay : ICellOverlay, ISaveable {
        new string CellName { get; set; }
        new string CellDescription { get; set; }
        new ITerrain Terrain { get; set; }
        new IHearingProfile HearingProfile { get; set; }
        new CellOutdoorsType OutdoorsType { get; set; }
        new double AmbientLightFactor { get; set; }
        new double AddedLight { get; set; }
        void AddExit(IExit exit);
        void RemoveExit(IExit exit);
        new IFluid Atmosphere { get; set; }
        new bool SafeQuit { get; set; }
    }
}