using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using System.Collections.Generic;

namespace MudSharp.Construction.Boundary
{
    public interface IExit : IPerceivable
    {
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

	/// <summary>
	/// A runtime-only exit whose database-independent identity can be used by effects that must survive reconstruction.
	/// </summary>
	public interface ITransientExit : IExit
	{
		/// <summary>
		/// A namespaced identity that remains stable whenever the same logical exit is rebuilt.
		/// </summary>
		string StableKey { get; }
	}
}
