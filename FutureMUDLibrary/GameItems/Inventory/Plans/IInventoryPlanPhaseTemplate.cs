using System.Collections.Generic;
using MudSharp.Framework;

namespace MudSharp.GameItems.Inventory.Plans
{
    public interface IInventoryPlanPhaseTemplate : IXmlSavable
    {
        int PhaseNumber { get; set; }
        IEnumerable<IInventoryPlanAction> Actions { get; }
        void AddAction(IInventoryPlanAction action);
        void RemoveAction(IInventoryPlanAction action);
    }
}