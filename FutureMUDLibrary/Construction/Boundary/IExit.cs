using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Construction.Boundary {
    public interface IExit : IPerceivable {
        bool AcceptsDoor { get; set; }
        SizeCategory DoorSize { get; set; }
        IDoor Door { get; set; }
        double TimeMultiplier { get; set; }

        SizeCategory MaximumSizeToEnterUpright { get; set; }

        SizeCategory MaximumSizeToEnter { get; set; }

        IEnumerable<ICell> Cells { get; }

        ICellExit CellExitFor(ICell cell);
        ICell Opposite(ICell cell);
        bool IsExit(ICell cell, string verb);
        bool IsExitKeyword(ICell cell, string keyword);

        IExit Clone();
        void PostLoadTasks(MudSharp.Models.Exit exit);
        ICell FallCell { get; set; }
        bool IsClimbExit { get; set; }
        Difficulty ClimbDifficulty { get; set; }
        IEnumerable<RoomLayer> BlockedLayers { get; }
        void AddBlockedLayer(RoomLayer layer);
        void RemoveBlockedLayer(RoomLayer layer);
        void Delete();
    }
}