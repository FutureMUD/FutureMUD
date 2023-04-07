using System.Collections.Generic;

namespace MudSharp.GameItems.Inventory.Plans
{
    public interface IInventoryPlanPhase
    {
        int PhaseNumber { get; }
        IInventoryPlanPhaseTemplate Template { get; set; }
        List<(IInventoryPlanAction Action, IGameItem Primary, IGameItem Secondary)> ScoutedItems { get; }
    }
}