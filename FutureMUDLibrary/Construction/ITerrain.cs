using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Combat;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Work.Foraging;

namespace MudSharp.Construction
{
    public interface ITerrain : IFrameworkItem, ISaveable, IHaveFuturemud, IFutureProgVariable
    {
        double MovementRate { get; }
        double StaminaCost { get; }
        double InfectionMultiplier { get; }
        InfectionType PrimaryInfection { get; }
        Difficulty InfectionVirulence { get; }
        IForagableProfile ForagableProfile { get; }
        bool DefaultTerrain { get; set; }
        IFluid Atmosphere { get; }
        Difficulty HideDifficulty { get; }
        Difficulty SpotDifficulty { get; }
        IWeatherController OverrideWeatherController { get; }
        IEnumerable<IRangedCover> TerrainCovers { get; }
        bool BuildingCommand(ICharacter actor, StringStack command);
        string Show(ICharacter actor);
        IEnumerable<RoomLayer> TerrainLayers { get; }
        string RoomNameForLayer(string baseRoomName, RoomLayer layer);
        IFluid WaterFluid { get; }
        CellOutdoorsType DefaultCellOutdoorsType { get; }
        string TerrainBehaviourString { get; }
        string TerrainEditorColour { get; }
        string TerrainEditorText { get; }
        string TerrainANSIColour { get; }
    }
}
